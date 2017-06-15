using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace dex.net
{
	public class EnhancedDexWriter : IDexWriter
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

		public EnhancedDexWriter() {
			_helper = new TypeHelper(GetFieldName, AnnotationToString);
		}

		#region DexWriter

		public string GetName()
		{
			return "Dex";
		}
		
		public string GetExtension ()
		{
			return ".dex";
		}

        public string WriteOutMethod(Class dexClass, Method method, Indentation indent, bool renderOpcodes = true)
        {
            string output = "";
            var jumpTable = BuildJumpTable(method);

            /*foreach (var annotation in method.Annotations)
            {
                AnnotationToString(output, annotation.Values, dexClass, indent);
            }

            foreach (var annotation in method.ParameterAnnotations)
            {
                AnnotationToString(output, annotation.Values, dexClass, indent);
            }*/

            // Initialize a pointer to the first jump target
            var targets = jumpTable.Targets.Keys.GetEnumerator();
            targets.MoveNext();

            output += (indent.ToString());
            output += (MethodToString(method));
            output += (" {") + "\n";
            if (renderOpcodes)
            {
                indent++;

                var lastTryBlockId = 0;
                var activeTryBlocks = new List<TryCatchBlock>();
                TryCatchBlock currentTryBlock = null;

                foreach (var opcode in method.GetInstructions())
                {
                    long offset = opcode.OpCodeOffset;

                    // Print out jump target labels
                    while (offset == targets.Current)
                    {
                        output += "\n";
                        //output += "{0}:{1}", indent.ToString(), string.Join(", ", jumpTable.GetTargetLabels(targets.Current)) \n ;
                        output += indent.ToString() + ":" + string.Join(", ", jumpTable.GetTargetLabels(targets.Current)) + "\n";

                        if (!targets.MoveNext())
                        {
                            break;
                        }
                    }

                    // Test for the end of the current try block
                    if (currentTryBlock != null && !currentTryBlock.IsInBlock(offset))
                    {
                        //WriteOutCatchStatements(output, indent, currentTryBlock, jumpTable);
                        activeTryBlocks.Remove(currentTryBlock);

                        if (activeTryBlocks.Count > 0)
                        {
                            currentTryBlock = activeTryBlocks[activeTryBlocks.Count - 1];
                        }
                        else {
                            currentTryBlock = null;
                        }
                    }

                    // Should open a new try block?
                    if (method.TryCatchBlocks != null && method.TryCatchBlocks.Length > lastTryBlockId)
                    {
                        var tryBlock = method.TryCatchBlocks[lastTryBlockId];
                        if (tryBlock.IsInBlock(offset))
                        {
                            output += indent.ToString();
                            output += "try" + "\n";

                            activeTryBlocks.Add(tryBlock);
                            currentTryBlock = tryBlock;
                            lastTryBlockId++;

                            indent++;
                        }
                    }

                    if (opcode.Instruction != Instructions.Nop)
                    {
                        output += indent.ToString();
                        output += OpCodeToString(opcode, dexClass, method, jumpTable) + "\n";
                    }
                }
                if (currentTryBlock != null)
                {
                    //WriteOutCatchStatements(output, indent, currentTryBlock, jumpTable);
                }
                indent--;
            }

            output += indent.ToString();
            output += "}" + "\n";

            return output;
        }

		public void WriteOutMethod (Class dexClass, Method method, TextWriter output, Indentation indent, bool renderOpcodes=false)
		{
			var jumpTable = BuildJumpTable (method);

			foreach (var annotation in method.Annotations) {
				AnnotationToString(output, annotation.Values, dexClass, indent);
			}

			foreach (var annotation in method.ParameterAnnotations) {
				AnnotationToString(output, annotation.Values, dexClass, indent);
			}

			// Initialize a pointer to the first jump target
			var targets = jumpTable.Targets.Keys.GetEnumerator();
			targets.MoveNext ();

			output.Write (indent.ToString());
			output.Write (MethodToString(method));
			output.WriteLine (" {");
			if (renderOpcodes) {
				indent++;

				var lastTryBlockId = 0;
				var activeTryBlocks = new List<TryCatchBlock> ();
				TryCatchBlock currentTryBlock = null;

				foreach (var opcode in method.GetInstructions()) {
					long offset = opcode.OpCodeOffset;

					// Print out jump target labels
					while (offset == targets.Current) {
						output.WriteLine ();
						output.WriteLine("{0}:{1}", indent.ToString(), string.Join(", ", jumpTable.GetTargetLabels(targets.Current)));

						if (!targets.MoveNext ()) {
							break;
						}
					}

					// Test for the end of the current try block
					if (currentTryBlock != null && !currentTryBlock.IsInBlock(offset)) {
						WriteOutCatchStatements(output, indent, currentTryBlock, jumpTable);
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
							output.Write (indent.ToString ());
							output.WriteLine ("try");

							activeTryBlocks.Add (tryBlock);
							currentTryBlock = tryBlock;
							lastTryBlockId++;

							indent++;
						}
					}

					if (opcode.Instruction != Instructions.Nop) {
						output.Write (indent.ToString ());
						output.WriteLine(OpCodeToString(opcode, dexClass, method, jumpTable));
					}
				}
				if (currentTryBlock != null) {
					WriteOutCatchStatements (output, indent, currentTryBlock, jumpTable);
				}
				indent--;
			}

			output.Write (indent.ToString());
			output.WriteLine ("}");
		}

		private void WriteOutCatchStatements(TextWriter output, Indentation indent, TryCatchBlock currentTryBlock, JumpTable jumpTable)
		{
			indent--;
			foreach (var catchBlock in currentTryBlock.Handlers) {
				output.WriteLine (string.Format ("{0}catch({1}) :{2}", 
					indent.ToString(),
					catchBlock.TypeIndex == 0 ? "ALL" : _dex.GetTypeName(catchBlock.TypeIndex),
					jumpTable.GetHandlerLabel(catchBlock)));
			}
		}

		public void WriteOutClass (Class dexClass, ClassDisplayOptions options, TextWriter output)
		{
			WriteOutClassDefinition(output, dexClass, options);
			output.WriteLine (" {");

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

			output.WriteLine ("}");
		}

		public List<HightlightInfo> GetCodeHightlight ()
		{
			var highlight = new List<HightlightInfo> ();

			// Annotation
			highlight.Add (new HightlightInfo("@.+", 0x81, 0x5B, 0xA4));

			// Keywords
			highlight.Add (new HightlightInfo("\\b(return-void|return|goto|packed-switch|sparse-switch|filled-new-array/range|new|instance-of|check-cast|const-class|fill-array-data|move-result-object|move-result|move-exception|try|catch|throw|extends|implements)\\b", 158, 28, 78));

			// Strings
			highlight.Add (new HightlightInfo("(\".*?\")", 58, 92, 120));

			// Integers
			highlight.Add (new HightlightInfo("\\b(0x[\\da-f]+|\\d+)\\b", 252, 120, 8));

			// Labels
			highlight.Add (new HightlightInfo("\\s(:.+)\\b", 55, 193, 58));

			return highlight;
		}

		#endregion

		class JumpTable
		{
			internal SortedDictionary<long,ISet<string>> Targets = new SortedDictionary<long,ISet<string>>();
			SortedDictionary<long,ISet<string>> Referrers = new SortedDictionary<long,ISet<string>>();
			Dictionary<CatchHandler, string> Handlers = new Dictionary<CatchHandler,string>();

			string NextTargetLabel (long offset, string labelPrefix, ref int counter) 
			{
				ISet<string> targets;
				if (Targets.TryGetValue(offset, out targets)) {
					foreach (var target in targets) {
						if (target.StartsWith (labelPrefix)) {
							return target;
						}
					}
				}

				return labelPrefix + counter++;
			}

			internal string AddTarget(long from, long to, string labelPrefix, ref int counter)
			{
				var label = NextTargetLabel (to, labelPrefix, ref counter);

				ISet<string> target;
				if (!Targets.TryGetValue (to, out target)) {
					target = new HashSet<string> ();
					Targets.Add (to, target);
				}
				target.Add (label);

				ISet<string> referrers;
				if (!Referrers.TryGetValue (from, out referrers)) {
					referrers = new HashSet<string> ();
					Referrers.Add (from, referrers);
				}
				referrers.Add(label);

				return label;
			}

			internal void AddHandler(CatchHandler handler, string labelPrefix, ref int counter) {
				var label = AddTarget(-1, handler.HandlerOffset, labelPrefix, ref counter);
				Handlers.Add (handler, label);
			}

			internal string GetHandlerLabel(CatchHandler handler) {
				var label = string.Empty;
				Handlers.TryGetValue (handler, out label);

				return label;
			}

			internal string GetReferrerLabel(long offset)
			{
				ISet<string> labels;
				if (!Referrers.TryGetValue (offset, out labels)) {
					return string.Empty;
				}

				return (labels as HashSet<string>).First ();
			}
			
			internal IEnumerable<string> GetReferrerLabels(long offset)
			{
				ISet<string> referrers;
				if (Referrers.TryGetValue (offset, out referrers)) {
					foreach (var referrer in referrers) {
						yield return referrer;
					}
				}
			}

			internal IEnumerable<string> GetTargetLabels(long offset) {
				ISet<string> targets;
				if (Targets.TryGetValue (offset, out targets)) {
					foreach (var target in targets) {
						yield return target;
					}
				}
			}
		}

		JumpTable BuildJumpTable(Method method)
		{
			var jumpTable = new JumpTable ();

			int gotoCounter = 0;
			int ifCounter = 0;
			int switchCounter = 0;

			foreach (var opcode in method.GetInstructions()) {
				switch (opcode.Instruction) {
				case Instructions.Goto:
				case Instructions.Goto16:
				case Instructions.Goto32:
					var gotoOpCode = (dynamic)opcode;
					jumpTable.AddTarget(gotoOpCode.OpCodeOffset, gotoOpCode.GetTargetAddress(), "goto_", ref gotoCounter);
					break;

				case Instructions.PackedSwitch:
				case Instructions.SparseSwitch:
					dynamic switchOpCode = opcode;
					foreach (var switchTarget in switchOpCode.GetTargetAddresses()) {
						jumpTable.AddTarget (switchOpCode.OpCodeOffset, switchTarget, "switch_", ref switchCounter);
					}
					break;

				case Instructions.IfEq:
				case Instructions.IfNe:
				case Instructions.IfLt:
				case Instructions.IfGe:
				case Instructions.IfGt:
				case Instructions.IfLe:
				case Instructions.IfEqz:
				case Instructions.IfNez:
				case Instructions.IfLtz:
				case Instructions.IfGez:
				case Instructions.IfGtz:
				case Instructions.IfLez:
					dynamic ifOpCode = opcode;
					jumpTable.AddTarget(ifOpCode.OpCodeOffset, ifOpCode.GetTargetAddress(), "if_", ref ifCounter);
					break;
				}
			}

			var catchCounter = 0;

			if (method.TryCatchBlocks != null) {
				foreach (var tryBlock in method.TryCatchBlocks) {
					foreach (var catchBlock in tryBlock.Handlers) {
						jumpTable.AddHandler (catchBlock, "catch_", ref catchCounter);
					}
				}
			}

			return jumpTable;
		}

		public void AnnotationToString(TextWriter output, EncodedAnnotation annotation, Class currentClass, Indentation indent)
		{
			var attributes = new List<string> ();
			foreach (var pair in annotation.GetAnnotations()) {
				attributes.Add (string.Format("{0}={1}", pair.GetName(_dex), _helper.EncodedValueToString(pair.Value, currentClass)));
			}

			output.WriteLine(string.Format ("@{0}({1})", _dex.GetTypeName(annotation.AnnotationType), string.Join(",", attributes)));
		}

		void WriteOutClassDefinition(TextWriter output, Class dexClass, ClassDisplayOptions options)
		{
			if ((options & ClassDisplayOptions.ClassAnnotations) != 0) {
				var indent = new Indentation ();
				foreach (var annotation in dexClass.Annotations) {
					AnnotationToString(output, annotation.Values, dexClass, indent);
				}
			}

			if ((options & ClassDisplayOptions.ClassDetails) != 0) {
				output.Write (_helper.AccessAndType(dexClass));
			}

			if ((options & ClassDisplayOptions.ClassName) != 0) {
				output.Write (_dex.GetTypeName(dexClass.ClassIndex));
			}

			if ((options & ClassDisplayOptions.ClassDetails) != 0) {
				if (dexClass.SuperClassIndex != Class.NO_INDEX) {
					output.Write (" extends ");
					output.Write (_dex.GetTypeName (dexClass.SuperClassIndex));
				}

				if (dexClass.ImplementsInterfaces()) {
					output.Write (" implements");

					var ifaces = new List<string> ();
					foreach (var iface in dexClass.ImplementedInterfaces()) {
						ifaces.Add (_dex.GetTypeName(iface));
					}
					output.Write (string.Join(", ", ifaces));
				}
			}
		}


		string MethodToString (Method method)
		{
			var proto = _dex.GetPrototype (method.PrototypeIndex);
			var parameters = new List<string>();
			int paramCounter = 0;

			foreach (var param in proto.Parameters) {
				parameters.Add(string.Format("{0} {1}{2}", _dex.GetTypeName(param), 'a', paramCounter++));
			}

			return string.Format ("{0} {1} {2} ({3})",
			                      _helper.AccessFlagsToString(((AccessFlag)method.AccessFlags)), 
			                      _dex.GetTypeName(proto.ReturnTypeIndex), 
			                      method.Name, string.Join(",", parameters));
		}

		/// <summary>
		/// Gets the register name within a method. The layout of a registers is:
		/// [v0, v1, ..., vn, this(vn+1)[for instance methods], a0(vn+2), a1(vn+3)] where n is the 
		/// number of registers defined in a method
		/// </summary>
		/// <returns>The register name</returns>
		/// <param name="index">Index of the register in the register array for a method</param>
		string GetRegisterName(uint index, Method method)
		{
			if (index < method.LocalsCount) {
				return string.Format("v{0}", index);
			}

			if (!method.IsStatic()) {
				if (index == method.LocalsCount) {
					return "this";
				}

				// Remove 'this' from the list
				index--;
			}

			return string.Format("a{0}", (index-method.LocalsCount));
		}

		void WriteOutFields(TextWriter output, Class dexClass, ClassDisplayOptions options, Indentation indent, bool renderAnnotations=true)
		{
			if ((options & ClassDisplayOptions.Fields) != 0 && dexClass.HasFields()) {
				int i=0;
				foreach (var field in dexClass.GetFields()) {
					// Field Annotations
					if (renderAnnotations) {
						indent++;
						foreach (var annotation in field.Annotations) {
							AnnotationToString (output, annotation.Values, dexClass, indent);
						}
						indent--;
					}

					// Field modifiers, type and name
					if (i < dexClass.StaticFieldsValues.Length) {
						output.WriteLine (string.Format ("{4}{0} {1} {2} = {3}", 
						                                 _helper.AccessFlagsToString (field.AccessFlags), 
						                                 _dex.GetTypeName (field.TypeIndex), 
						                                 field.Name, 
						                                 _helper.EncodedValueToString(dexClass.StaticFieldsValues[i], dexClass), 
						                                 indent.ToString()));
					} else {
						output.WriteLine (string.Format ("{3}{0} {1} {2}", 
						                                 _helper.AccessFlagsToString (field.AccessFlags), 
						                                 _dex.GetTypeName (field.TypeIndex), 
						                                 field.Name, indent.ToString()));
					}

					i++;
				}
			}
		}

		public string GetFieldName(uint index, Class currentClass, bool isClass=false)
		{
			var field = _dex.GetField (index);

			// Static reference. Return class.field
			if (isClass) {
				// Remove the package name for fields in the current class
				var className = field.ClassIndex == currentClass.ClassIndex ? 
					currentClass.Name.Substring(currentClass.Name.LastIndexOf('.')+1)  : 
						_dex.GetTypeName(field.ClassIndex);

				return className + "." + field.Name;
			}

			if (field.ClassIndex == currentClass.ClassIndex) {
				return "this." + field.Name;
			}

			return field.Name;
		}

		string OpCodeToString(OpCode opcode, Class currentClass, Method method, JumpTable jumpTable)
		{
			switch (opcode.Instruction) {

				case Instructions.Const:
				case Instructions.Const4:
				case Instructions.Const16:
				case Instructions.ConstHigh:
				case Instructions.ConstWide16:
				case Instructions.ConstWide32:
				case Instructions.ConstWide:
				case Instructions.ConstWideHigh:
				dynamic constOpCode = opcode;
				return string.Format("{1} = #{2}", constOpCode.Name, GetRegisterName(constOpCode.Destination, method), constOpCode.Value);

				case Instructions.Move:
				case Instructions.MoveFrom16:
				case Instructions.Move16:
				case Instructions.MoveWide:
				case Instructions.MoveWideFrom16:
				case Instructions.MoveWide16:
				case Instructions.MoveObject:
				case Instructions.MoveObjectFrom16:
				case Instructions.MoveObject16:
				dynamic moveOpCode = opcode;
				return string.Format("{1} = {2}", moveOpCode.Name, GetRegisterName(moveOpCode.To, method), GetRegisterName(moveOpCode.From, method));

				case Instructions.MoveResult:
				case Instructions.MoveResultWide:
				case Instructions.MoveResultObject:
				case Instructions.MoveException:
				dynamic moveResultOpCode = opcode;
				return string.Format("{0} {1}", moveResultOpCode.Name, GetRegisterName(moveResultOpCode.Destination, method));

				case Instructions.ConstString:
				case Instructions.ConstStringJumbo:
				dynamic constStringOpCode = opcode;
				return string.Format("{1} = \"{2}\"", constStringOpCode.Name, GetRegisterName(constStringOpCode.Destination, method), _dex.GetString(constStringOpCode.StringIndex).Replace("\n", "\\n"));

				case Instructions.ConstClass:
				return string.Format("{0} = {1}", GetRegisterName(((ConstClassOpCode)opcode).Destination, method), _dex.GetTypeName(((ConstClassOpCode)opcode).TypeIndex));

				case Instructions.CheckCast:
				var typeCheckCast = _dex.GetTypeName(((CheckCastOpCode)opcode).TypeIndex);
				return string.Format("({1}){0}", GetRegisterName(((CheckCastOpCode)opcode).Destination, method), typeCheckCast);

				case Instructions.InstanceOf:
				var typeInstanceOf = _dex.GetTypeName(((InstanceOfOpCode)opcode).TypeIndex);
				return string.Format("instance-of v{0}, v{1}, {2}", ((InstanceOfOpCode)opcode).Destination, ((InstanceOfOpCode)opcode).Reference, typeInstanceOf);

				case Instructions.NewInstance:
				var typeNewInstance = _dex.GetTypeName(((NewInstanceOpCode)opcode).TypeIndex);
				return string.Format("{0} = new {1}", GetRegisterName(((NewInstanceOpCode)opcode).Destination, method), typeNewInstance);

				case Instructions.NewArrayOf:
				var newArrayOpCode = (NewArrayOfOpCode)opcode;
				var typeNewArrayOfOpCode = _dex.GetTypeName(newArrayOpCode.TypeIndex);
				return string.Format("{0} = new {1}", GetRegisterName(newArrayOpCode.Destination, method), 
				                     typeNewArrayOfOpCode.Replace("[]", string.Format("[{0}]", GetRegisterName(newArrayOpCode.Size, method))));

				case Instructions.FilledNewArrayOf:
				var typeFilledNewArrayOf = _dex.GetTypeName(((FilledNewArrayOfOpCode)opcode).TypeIndex);
				return string.Format("filled-new-array {0}, {1}", string.Join(",", ((FilledNewArrayOfOpCode)opcode).Values), typeFilledNewArrayOf);

				case Instructions.FilledNewArrayRange:
				var typeFilledNewArrayRange = _dex.GetTypeName(((FilledNewArrayRangeOpCode)opcode).TypeIndex);
				return string.Format("filled-new-array/range {0}, {1}", string.Join(",", ((FilledNewArrayRangeOpCode)opcode).Values), typeFilledNewArrayRange);

				case Instructions.ArrayLength:
				var arrayLengthOpcode = (ArrayLengthOpCode)opcode;
				return string.Format("{0} = {1}.length", GetRegisterName(arrayLengthOpcode.Destination, method), GetRegisterName(arrayLengthOpcode.ArrayReference, method));

			case Instructions.InvokeVirtual:
			case Instructions.InvokeSuper:
			case Instructions.InvokeDirect:
			case Instructions.InvokeStatic:
			case Instructions.InvokeInterface:
				var invokeOpcode = (InvokeOpCode)opcode;
				var invokeMethod = _dex.GetMethod (invokeOpcode.MethodIndex);

				string invokeObject;
				if (invokeMethod.IsStatic () || invokeOpcode.Instruction == Instructions.InvokeStatic) {
					invokeObject = _dex.GetTypeName (invokeMethod.ClassIndex);
				} else {
					invokeObject =  GetRegisterName (invokeOpcode.ArgumentRegisters [0], method);
				}

				string[] registerNames = {};
				if (invokeOpcode.ArgumentRegisters.Length > 0) {
					var instanceAdjustment = invokeMethod.IsStatic()?0:1;
					registerNames = new string[invokeOpcode.ArgumentRegisters.Length - instanceAdjustment];
					for (int i=0; i<registerNames.Length; i++) {
						registerNames [i] = GetRegisterName (invokeOpcode.ArgumentRegisters[i+instanceAdjustment], method);
					}
				}
				return string.Format("{2}.{3}({1})", ((InvokeOpCode)opcode).Name, string.Join(",", registerNames), invokeObject, invokeMethod.Name);

				case Instructions.Sput:
				case Instructions.SputWide:
				case Instructions.SputObject:
				case Instructions.SputBoolean:
				case Instructions.SputChar:
				case Instructions.SputShort:
				case Instructions.SputByte:
				var sputOpcode = (StaticOpOpCode)opcode;
				return string.Format("{2} = {1}", sputOpcode.Name, GetRegisterName(sputOpcode.Destination, method), GetFieldName(sputOpcode.Index, currentClass, true));

				case Instructions.Sget:
				case Instructions.SgetWide:
				case Instructions.SgetObject:
				case Instructions.SgetBoolean:
				case Instructions.SgetChar:
				case Instructions.SgetShort:
				case Instructions.SgetByte:
				var sgetOpcode = (StaticOpOpCode)opcode;
				return string.Format("{1} = {2}", sgetOpcode.Name, GetRegisterName(sgetOpcode.Destination, method), GetFieldName(sgetOpcode.Index, currentClass, true));

				case Instructions.Iput:
				case Instructions.IputWide:
				case Instructions.IputObject:
				case Instructions.IputBoolean:
				case Instructions.IputChar:
				case Instructions.IputShort:
				case Instructions.IputByte:
				var iputOpCode = (IinstanceOpOpCode)opcode;
				return string.Format("{2} = {1}", iputOpCode.Name, GetRegisterName(iputOpCode.Destination, method), GetFieldName(iputOpCode.Index, currentClass));

				case Instructions.Iget:
				case Instructions.IgetWide:
				case Instructions.IgetObject:
				case Instructions.IgetBoolean:
				case Instructions.IgetChar:
				case Instructions.IgetShort:
				case Instructions.IgetByte:
				var igetOpCode = (IinstanceOpOpCode)opcode;
				return string.Format("{1} = {2}", igetOpCode.Name, GetRegisterName(igetOpCode.Destination, method), GetFieldName(igetOpCode.Index, currentClass));
				 
				case Instructions.Aput:
				case Instructions.AputWide:
				case Instructions.AputObject:
				case Instructions.AputBoolean:
				case Instructions.AputChar:
				case Instructions.AputShort:
				case Instructions.AputByte:
				var aputOpCode = (ArrayOpOpCode)opcode;
				return string.Format("{2}[{3}] = {1}", aputOpCode.Name, GetRegisterName(aputOpCode.Destination, method), GetRegisterName(aputOpCode.Array, method), GetRegisterName(aputOpCode.Index, method));

				case Instructions.Aget:
				case Instructions.AgetWide:
				case Instructions.AgetObject:
				case Instructions.AgetBoolean:
				case Instructions.AgetChar:
				case Instructions.AgetShort:
				case Instructions.AgetByte:
				var agetOpCode = (ArrayOpOpCode)opcode;
				return string.Format("{1} = {2}[{3}]", agetOpCode.Name, GetRegisterName(agetOpCode.Destination, method), GetRegisterName(agetOpCode.Array, method), GetRegisterName(agetOpCode.Index, method));

				case Instructions.AddInt:
				case Instructions.AddLong:
				case Instructions.AddFloat:
				case Instructions.AddDouble:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "+", method);

				case Instructions.SubInt:
				case Instructions.SubLong:
				case Instructions.SubFloat:
				case Instructions.SubDouble:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "-", method);

				case Instructions.MulInt:
				case Instructions.MulLong:
				case Instructions.MulFloat:
				case Instructions.MulDouble:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "*", method);

				case Instructions.DivInt:
				case Instructions.DivLong:
				case Instructions.DivFloat:
				case Instructions.DivDouble:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "/", method);

				case Instructions.RemInt:
				case Instructions.RemLong:
				case Instructions.RemFloat:
				case Instructions.RemDouble:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "&", method);

				case Instructions.AndInt:
				case Instructions.AndLong:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "&", method);

				case Instructions.OrInt:
				case Instructions.OrLong:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "|", method);

				case Instructions.XorInt:
				case Instructions.XorLong:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "^", method);

				case Instructions.ShlInt:
				case Instructions.ShlLong:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, "<<", method);

				case Instructions.ShrInt:
				case Instructions.UshrInt:
				case Instructions.ShrLong:
				case Instructions.UshrLong:
				return FormatBinaryOperation((BinaryOpOpCode)opcode, ">>", method);

				case Instructions.IfEq:
				return FormatIfOperation((IfOpCode)opcode, "=", method, jumpTable);

				case Instructions.IfNe:
				return FormatIfOperation((IfOpCode)opcode, "!=", method, jumpTable);

				case Instructions.IfLt:
				return FormatIfOperation((IfOpCode)opcode, "<", method, jumpTable);

				case Instructions.IfGe:
				return FormatIfOperation((IfOpCode)opcode, ">=", method, jumpTable);

				case Instructions.IfGt:
				return FormatIfOperation((IfOpCode)opcode, ">", method, jumpTable);

				case Instructions.IfLe:
				return FormatIfOperation((IfOpCode)opcode, "<=", method, jumpTable);

				case Instructions.IfEqz:
				return FormatIfzOperation((IfzOpCode)opcode, "=", method, jumpTable);

				case Instructions.IfNez:
				return FormatIfzOperation((IfzOpCode)opcode, "!=", method, jumpTable);

				case Instructions.IfLtz:
				return FormatIfzOperation((IfzOpCode)opcode, "<", method, jumpTable);

				case Instructions.IfGez:
				return FormatIfzOperation((IfzOpCode)opcode, ">=", method, jumpTable);

				case Instructions.IfGtz:
				return FormatIfzOperation((IfzOpCode)opcode, ">", method, jumpTable);

				case Instructions.IfLez:
				return FormatIfzOperation((IfzOpCode)opcode, "<=", method, jumpTable);

				case Instructions.RsubInt:
				case Instructions.RsubIntLit8:
				dynamic rsubIntOpCode = opcode;
				return string.Format("{0} = #{2} - {1}", GetRegisterName(rsubIntOpCode.Destination, method), GetRegisterName(rsubIntOpCode.Source, method), rsubIntOpCode.Constant);

				case Instructions.AddIntLit16:
				case Instructions.AddIntLit8:
				return FormatBinaryLiteralOperation(opcode, "+", method);

				case Instructions.MulIntLit16:
				case Instructions.MulIntLit8:
				return FormatBinaryLiteralOperation(opcode, "*", method);

				case Instructions.DivIntLit16:
				case Instructions.DivIntLit8:
				return FormatBinaryLiteralOperation(opcode, "/", method);

				case Instructions.RemIntLit16:
				case Instructions.RemIntLit8:
				return FormatBinaryLiteralOperation(opcode, "%", method);

				case Instructions.AndIntLit16:
				case Instructions.AndIntLit8:
				return FormatBinaryLiteralOperation(opcode, "&", method);

				case Instructions.OrIntLit16:
				case Instructions.OrIntLit8:
				return FormatBinaryLiteralOperation(opcode, "|", method);

				case Instructions.XorIntLit16:
				case Instructions.XorIntLit8:
				return FormatBinaryLiteralOperation(opcode, "^", method);

				case Instructions.ShlIntLit8:
				return FormatBinaryLiteralOperation(opcode, "<<", method);

				case Instructions.ShrIntLit8:
				return FormatBinaryLiteralOperation(opcode, ">>", method);

				case Instructions.UshrIntLit8:
				return FormatBinaryLiteralOperation(opcode, ">>", method);

				case Instructions.AddInt2Addr:
				case Instructions.AddLong2Addr:
				case Instructions.AddFloat2Addr:
				case Instructions.AddDouble2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "+", method);

				case Instructions.SubInt2Addr:
				case Instructions.SubLong2Addr:
				case Instructions.SubFloat2Addr:
				case Instructions.SubDouble2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "-", method);

				case Instructions.MulInt2Addr:
				case Instructions.MulLong2Addr:
				case Instructions.MulFloat2Addr:
				case Instructions.MulDouble2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "*", method);

				case Instructions.DivInt2Addr:
				case Instructions.DivLong2Addr:
				case Instructions.DivFloat2Addr:
				case Instructions.DivDouble2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "/", method);

				case Instructions.RemInt2Addr:
				case Instructions.RemLong2Addr:
				case Instructions.RemFloat2Addr:
				case Instructions.RemDouble2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "&", method);

				case Instructions.AndInt2Addr:
				case Instructions.AndLong2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "&", method);

				case Instructions.OrInt2Addr:
				case Instructions.OrLong2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "|", method);

				case Instructions.XorInt2Addr:
				case Instructions.XorLong2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "^", method);

				case Instructions.ShlInt2Addr:
				case Instructions.ShlLong2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, "<<", method);

				case Instructions.ShrInt2Addr:
				case Instructions.UshrInt2Addr:
				case Instructions.ShrLong2Addr:
				case Instructions.UshrLong2Addr:
				return FormatBinary2AddrOperation((BinaryOp2OpCode)opcode, ">>", method);

				case Instructions.Goto:
				case Instructions.Goto16:
				case Instructions.Goto32:
				dynamic gotoOpCode = opcode;
				return string.Format("{0} {1}", gotoOpCode.Name, jumpTable.GetReferrerLabel(gotoOpCode.OpCodeOffset));

				case Instructions.PackedSwitch:
				var packedSwitchOpCode = (PackedSwitchOpCode)opcode;
				return string.Format("{0} {1} - First:{2} - [{3}]", opcode.Name, GetRegisterName(packedSwitchOpCode.Destination,method), packedSwitchOpCode.FirstKey, string.Join(",", jumpTable.GetReferrerLabels(packedSwitchOpCode.OpCodeOffset)));
				
				case Instructions.SparseSwitch:
				var sparseSwitchOpCode = (SparseSwitchOpCode)opcode;
				return string.Format("{0} {1} - Keys:[{2}] Targets:[{3}]", opcode.Name, GetRegisterName(sparseSwitchOpCode.Destination,method), string.Join(",",sparseSwitchOpCode.Keys), string.Join(",", jumpTable.GetReferrerLabels(sparseSwitchOpCode.OpCodeOffset)));

				default:
				return opcode.ToString ();
			}
		}

		private string FormatBinaryOperation(BinaryOpOpCode opcode, string operation, Method method)
		{
			return string.Format("{0} = {1} {3} {2}", GetRegisterName(opcode.Destination, method), GetRegisterName(opcode.First, method), GetRegisterName(opcode.Second, method), operation);
		}

		private string FormatBinary2AddrOperation(BinaryOp2OpCode opcode, string operation, Method method)
		{
			return string.Format("{0} = {0} {2} {1}", GetRegisterName(opcode.Destination, method), GetRegisterName(opcode.Source, method), operation);
		}

		private string FormatBinaryLiteralOperation(dynamic opcode, string operation, Method method)
		{
			return string.Format("{0} = {1} {3} #{2}", GetRegisterName(opcode.Destination, method), GetRegisterName(opcode.Source, method), opcode.Constant, operation);
		}

		private string FormatIfOperation(IfOpCode opcode, string comparison, Method method, JumpTable jumpTable)
		{
			return string.Format("if ({0} {3} {1}) :{2}", GetRegisterName(opcode.First, method), GetRegisterName(opcode.Second, method), jumpTable.GetReferrerLabel(opcode.OpCodeOffset), comparison);
		}

		private string FormatIfzOperation(IfzOpCode opcode, string comparison, Method method, JumpTable jumpTable)
		{
			return string.Format("if ({0} {2} 0) :{1}", GetRegisterName(opcode.Destination, method), jumpTable.GetReferrerLabel(opcode.OpCodeOffset), comparison);
		}
	}
}
