using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace dex.net
{
	public class PlainDexWriter : IDexWriter
	{
		private Dex _dex;
		public Dex dex 
		{ 
			get { return _dex; } 
			set {
				_dex = value;
				_helper._dex = _dex;
			}
		}

		private TypeHelper _helper;

		public PlainDexWriter() {
			_helper = new TypeHelper((index, currentClass, isClass) => {return "v" + index;} , WriteOutAnnotation);
		}

		#region DexWriter

		public string GetName()
		{
			return "Plain Dex";
		}

		public string GetExtension ()
		{
			return ".pdex";
		}

		public void WriteOutMethod (Class dexClass, Method method, TextWriter output, Indentation indent, bool renderOpcodes=false)
		{
			var stringIndent = indent.ToString ();
			var proto = _dex.GetPrototype (method.PrototypeIndex);

			output.WriteLine (string.Format("{0}.METHOD {1} : {2}", stringIndent, method.Name, _dex.GetTypeName(proto.ReturnTypeIndex)));
			indent++;
			var paramCount = 0;
			foreach (var param in proto.Parameters) {
				output.WriteLine(string.Format("{0}.PARAM {1}", indent.ToString(), _dex.GetTypeName(param)));
				if (method.ParameterAnnotations.Count > paramCount) {
					indent++;
					WriteOutAnnotation (output, method.ParameterAnnotations[paramCount].Values, dexClass, indent);
					indent--;
				}
				paramCount++;
			}
			indent--;

			output.WriteLine (string.Format("{0}.MODIFIERS {1}", stringIndent, _helper.AccessFlagsToString(((AccessFlag)method.AccessFlags))));
			output.WriteLine (string.Format("{0}.REGISTERS {1}", stringIndent, method.GetRegisterCount()));

			foreach (var annotation in method.Annotations) {
				WriteOutAnnotation(output, annotation.Values, dexClass, indent);
			}

			if (renderOpcodes) {
				output.WriteLine (string.Format ("{0}.CODE", stringIndent));

				indent++;
				stringIndent = indent.ToString ();

				long offset = 0;
				var lastTryBlockId = 0;
				var activeTryBlocks = new List<TryCatchBlock> ();
				TryCatchBlock currentTryBlock = null;

				foreach (var opcode in method.GetInstructions()) {
					offset = opcode.OpCodeOffset;

					// Test for the end of the current try block
					if (currentTryBlock != null && !currentTryBlock.IsInBlock(offset)) {
						WriteOutCatchStatements (output, indent, currentTryBlock);
						activeTryBlocks.Remove (currentTryBlock);

						if (activeTryBlocks.Count > 0) {
							currentTryBlock = activeTryBlocks [activeTryBlocks.Count - 1];
						} else {
							currentTryBlock = null;
						}
					}

					// Should open a new try block?
					if (method.TryCatchBlocks != null && method.TryCatchBlocks.Length > lastTryBlockId) {
						var tryBlock = method.TryCatchBlocks [lastTryBlockId];
						if (tryBlock.IsInBlock (offset)) {
							output.WriteLine (string.Format ("{0}{1}   {2} #{3}", stringIndent, "".PadLeft (12, ' '), ".TRY", lastTryBlockId));
							activeTryBlocks.Add (tryBlock);
							currentTryBlock = tryBlock;
							lastTryBlockId++;
						}
					}

					if (opcode.Instruction != Instructions.Nop) {
						output.WriteLine (string.Format("{0}{1}  {2}", stringIndent,offset.ToString().PadLeft(12, ' '), opcode.ToString()));
					}
				}
				if (currentTryBlock != null) {
					WriteOutCatchStatements (output, indent, currentTryBlock);
				}

				indent--;
			}
		}

		private void WriteOutCatchStatements(TextWriter output, Indentation indent, TryCatchBlock currentTryBlock)
		{
			output.WriteLine (string.Format ("{0}{1}   {2}", indent.ToString (), "".PadLeft (12, ' '), ".CATCH"));
			indent++;
			foreach (var catchBlock in currentTryBlock.Handlers) {
				output.WriteLine (string.Format ("{0}{1}   {2} address:{3}", indent.ToString(), "".PadLeft (12, ' '), 
					catchBlock.TypeIndex == 0 ? "ALL" : _dex.GetTypeName(catchBlock.TypeIndex), 
					catchBlock.HandlerOffset));
			}
			indent--;
		}

		public void WriteOutClass (Class dexClass, ClassDisplayOptions options, TextWriter output)
		{
			WriteOutClassDefinition(output, dexClass, options);

			// Display fields
			var indent = new Indentation (1);
			if ((options & ClassDisplayOptions.Fields) != 0) {
				WriteOutFields(output, dexClass, options, indent);
			}

			if ((options & ClassDisplayOptions.Methods) != 0 && dexClass.HasMethods()) {
				foreach (var method in dexClass.GetMethods()) {
					WriteOutMethod (dexClass, method, output, indent, (options & ClassDisplayOptions.OpCodes) != 0);
					output.WriteLine ();
				}
			}
		}

		public List<HightlightInfo> GetCodeHightlight ()
		{
			var highlight = new List<HightlightInfo> ();

			// Directive
			highlight.Add (new HightlightInfo("^\\s*(\\..*?)\\s", 0x81, 0x5B, 0xA4));

			// Keywords
			highlight.Add (new HightlightInfo("^\\s+\\d+\\s+(.*?)\\s", 158, 28, 78));

			// Integers
			highlight.Add (new HightlightInfo("\\s(#?-?\\d+)", 252, 120, 8));

			// Offset
			highlight.Add (new HightlightInfo("^\\s+(\\d+)", 135, 135, 129));

			// Strings
			highlight.Add (new HightlightInfo("(\".*?\")", 58, 92, 120));

			// Labels
			highlight.Add (new HightlightInfo("\\s(:.+)\\b", 55, 193, 58));

			return highlight;
		}

		#endregion

		
		void WriteOutAnnotation(TextWriter output, EncodedAnnotation annotation, Class currentClass, Indentation indent)
		{
			output.WriteLine(string.Format ("{0}.ANNOTATION {1}", indent.ToString(), _dex.GetTypeName(annotation.AnnotationType)));

			indent++;
			var stringIndent = indent.ToString ();

			foreach (var pair in annotation.GetAnnotations()) {
				output.WriteLine (string.Format("{0}{1}={2}", stringIndent, pair.GetName(_dex), _helper.EncodedValueToString(pair.Value, currentClass)));
			}

			indent--;
		}

		void WriteOutClassDefinition(TextWriter output, Class dexClass, ClassDisplayOptions options)
		{
			output.WriteLine (string.Format(".TYPE {0}", _dex.GetTypeName(dexClass.ClassIndex)));

			if ((options & ClassDisplayOptions.ClassDetails) != 0) {
				output.WriteLine (string.Format(".MODIFIERS {0}", _helper.AccessAndType (dexClass)));
			}

			if ((options & ClassDisplayOptions.ClassDetails) != 0) {
				if (dexClass.SuperClassIndex != Class.NO_INDEX) {
					output.WriteLine (string.Format(".SUPER {0}", _dex.GetTypeName (dexClass.SuperClassIndex)));
				}

				if (dexClass.ImplementsInterfaces()) {
					output.Write (".INTERFACES");

					foreach (var iface in dexClass.ImplementedInterfaces()) {
						output.Write (" ");
						output.Write (_dex.GetTypeName(iface));
					}
					output.WriteLine ();
				}
			}
			
			if ((options & ClassDisplayOptions.ClassAnnotations) != 0) {
				foreach (var annotation in dexClass.Annotations) {
					WriteOutAnnotation(output, annotation.Values, dexClass, new Indentation());
				}
			}
		}

		void WriteOutFields(TextWriter output, Class dexClass, ClassDisplayOptions options, Indentation indent, bool renderAnnotations=true)
		{
			if ((options & ClassDisplayOptions.Fields) != 0 && dexClass.HasFields()) {
				output.WriteLine ();
				int i=0;
				foreach (var field in dexClass.GetFields()) {
					// Field modifiers, type and name
					if (i < dexClass.StaticFieldsValues.Length) {
						output.WriteLine (string.Format (".FIELD {0} {1} {2} = {3}", _helper.AccessFlagsToString (field.AccessFlags), _dex.GetTypeName (field.TypeIndex), field.Name, _helper.EncodedValueToString(dexClass.StaticFieldsValues[i], dexClass)));
					} else {
						output.WriteLine (string.Format (".FIELD {0} {1} {2}", _helper.AccessFlagsToString (field.AccessFlags), _dex.GetTypeName (field.TypeIndex), field.Name));
					}

					// Field Annotations
					if (renderAnnotations) {
						indent++;
						foreach (var annotation in field.Annotations) {
							WriteOutAnnotation (output, annotation.Values, dexClass, indent);
						}
						indent--;
					}

					i++;
				}
			}
		}

		//		private string PrototypeToString (Prototype proto)
		//		{
		//			var shorty = dex.GetString(proto.ShortyIndex);
		//			var returnType = dex.GetTypeName (proto.ReturnTypeIndex);
		//
		//			var builder = new StringBuilder ();
		//			foreach (var param in proto.Parameters) {
		//				builder.Append(string.Format("\t{0}\n", dex.GetTypeName(param)));
		//			}
		//
		//			return string.Format ("Short Form:\n\t{0}\nReturn Type:\n\t{1}\nParameters\n{2}",
		//			                      shorty, returnType, builder.ToString());
		//		}

	}
}
