using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	public class Class
	{
		public const uint NO_INDEX = 0xffffffff;

		// Index into the Type Ids list for this class
		internal uint ClassIndex;

		// Access flags for this class
		internal AccessFlag AccessFlags;

		// Index for the Super Class or the constant NO_INDEX if it's a root class
		internal uint SuperClassIndex;

		// Index into the Types List for the interfaces implemented by this class
		internal List<ushort> Interfaces;

		// Index into the String Ids for the file name that contained the source for this class
		internal uint SourceFileIndex;

		// Annotations to this class
		internal IEnumerable<Annotation> Annotations = new Annotation[0];

		// 
		internal Field[] StaticFields;
		internal Field[] InstanceFields;

		internal Method[] DirectMethods;
		internal Method[] VirtualMethods;

		// Offset to the list of initial values for static fields, 0 if none
		public EncodedValue[] StaticFieldsValues { get; internal set; }

		private Dex Dex;

		public string Name {
			get {
				return Dex.GetTypeName(ClassIndex);
			}
		}
		
		public string SuperClass {
			get {
				return SuperClassIndex != NO_INDEX ? Dex.GetTypeName (SuperClassIndex) : string.Empty;
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name='dex'>
		/// Pointer to the DEX file this class was loaded from
		/// </param>
		internal Class (Dex dex, BinaryReader reader)
		{
			Dex = dex;

			ClassIndex = reader.ReadUInt32 ();
			AccessFlags = (AccessFlag)reader.ReadUInt32 ();
			SuperClassIndex = reader.ReadUInt32 ();
			var interfacesOffset = reader.ReadUInt32 ();
			SourceFileIndex = reader.ReadUInt32 ();
			var annotationsOffset = reader.ReadUInt32 ();
			var classDataOffset = reader.ReadUInt32 ();
			var staticValuesOffset = reader.ReadUInt32 ();

			Interfaces = dex.ReadTypeList (interfacesOffset);

			var fieldAnnotations = new Dictionary<uint,List<Annotation>>();
			var methodAnnotations = new Dictionary<uint,List<Annotation>>();
			var parameterAnnotations = new Dictionary<uint,List<Annotation>>();
						
			if (annotationsOffset != 0) {
				reader.BaseStream.Position = annotationsOffset;

				// annotations_directory_item
				var classAnnotationsOffset = reader.ReadUInt32 ();
				var annotatedFieldsCount = reader.ReadUInt32 ();
				var annotatedMethodsCount = reader.ReadUInt32 ();
				var annotatedParametersCount = reader.ReadUInt32 ();

				for (int i=0; i<annotatedFieldsCount; i++) {
					// field_annotation
					var fieldIndex = reader.ReadUInt32 ();
					var annotations = Annotation.ReadAnnotations (reader);
					fieldAnnotations.Add (fieldIndex, annotations);
				}

				for (int i=0; i<annotatedMethodsCount; i++) {
					// method_annotation
					var methodIndex = reader.ReadUInt32 ();
					var annotations = Annotation.ReadAnnotations (reader);
					methodAnnotations.Add (methodIndex, annotations);
				}

				for (int i=0; i<annotatedParametersCount; i++) {
					// parameter_annotation
					var methodIndex = reader.ReadUInt32 ();
					var annotations = Annotation.ReadAnnotations (reader);
					parameterAnnotations.Add (methodIndex, annotations);
				}

				if (classAnnotationsOffset > 0) {
					reader.BaseStream.Position = classAnnotationsOffset;
					Annotations = Annotation.ReadAnnotationSetItem (reader);
				}
			}

			if (classDataOffset != 0) {
				reader.BaseStream.Position = classDataOffset;

				var staticFieldsCount = Leb128.ReadUleb(reader);
				var instanceFieldsCount = Leb128.ReadUleb(reader);
				var directMethodsCount = Leb128.ReadUleb(reader);
				var virtualMethodsCount = Leb128.ReadUleb(reader);

				StaticFields = ReadFields (staticFieldsCount, reader, fieldAnnotations);
				InstanceFields = ReadFields (instanceFieldsCount, reader, fieldAnnotations);
				DirectMethods = ReadMethods (directMethodsCount, reader, methodAnnotations, parameterAnnotations);
				VirtualMethods = ReadMethods (virtualMethodsCount, reader, methodAnnotations, parameterAnnotations);
			} else {
				StaticFields = new Field[0];
				InstanceFields = new Field[0];
				DirectMethods = new Method[0];
				VirtualMethods = new Method[0];
			}

			if (staticValuesOffset != 0) {
				reader.BaseStream.Position = staticValuesOffset;

				var size = Leb128.ReadUleb(reader);
				var values = new EncodedValue[size];

				for (int i=0; i<(int)size; i++) {
					values[i] = EncodedValue.parse(reader);
				}

				StaticFieldsValues = values;
			} else {
				StaticFieldsValues = new EncodedValue[0];
			}
		}

		private Field[] ReadFields (ulong count, BinaryReader reader, Dictionary<uint,List<Annotation>> annotations)
		{
			var fields = new Field[count];
			long position = 0;
			ulong currentFieldIndex = 0;

			for (ulong index=0; index<count; index++) {
				currentFieldIndex += Leb128.ReadUleb(reader);
				var accessFlags = Leb128.ReadUleb(reader);

				position = reader.BaseStream.Position;

				var field = Dex.GetField((uint)currentFieldIndex);
				field.AccessFlags = (AccessFlag)accessFlags;

				List<Annotation> annotation = null;
				annotations.TryGetValue((uint)currentFieldIndex, out annotation);
				if (annotation != null) {
					field.Annotations = annotation;
				}

				fields[index] = field;

				reader.BaseStream.Position = position;
			}

			return fields;
		}
		
		private Method[] ReadMethods (ulong count, BinaryReader reader, Dictionary<uint,List<Annotation>> annotations, Dictionary<uint,List<Annotation>> parameterAnnotations)
		{
			var methods = new Method[count];
			long position = 0;
			ulong currentMethodIndex = 0;

			for (ulong currentMethod=0; currentMethod<count; currentMethod++) {
				currentMethodIndex += Leb128.ReadUleb(reader);
				var accessFlags = Leb128.ReadUleb(reader);
				var codeOffset = (uint)Leb128.ReadUleb(reader);

				position = reader.BaseStream.Position;

				var method = Dex.GetMethod((uint)currentMethodIndex, codeOffset);
				method.AccessFlags = accessFlags;
				
				List<Annotation> annotation = null;
				annotations.TryGetValue((uint)currentMethodIndex, out annotation);
				if (annotation != null) {
					method.Annotations = annotation;
				}

				List<Annotation> parameterAnnotation = null;
				parameterAnnotations.TryGetValue((uint)currentMethodIndex, out parameterAnnotation);
				if (parameterAnnotation != null) {
					method.ParameterAnnotations = parameterAnnotation;
				}

				methods[currentMethod] = method;

				reader.BaseStream.Position = position;
			}

			return methods;
		}

		public bool HasFields() 
		{
			return (StaticFields.Length > 0) || (InstanceFields.Length > 0);
		}
		
		public IEnumerable<Field> GetFields ()
		{
			foreach (var field in StaticFields)
				yield return field;

			foreach (var field in InstanceFields)
				yield return field;
		}
		
		public bool HasMethods() 
		{
			return (DirectMethods.Length > 0) || (VirtualMethods.Length > 0);
		}
		
		public IEnumerable<Method> GetMethods ()
		{
			foreach (var method in DirectMethods)
				yield return method;

			foreach (var method in VirtualMethods)
				yield return method;
		}
		
		public bool ImplementsInterfaces() {
			return Interfaces.Count > 0;
		}
		
		public IEnumerable<ushort> ImplementedInterfaces() {
			foreach (var iface in Interfaces) {
				yield return iface;
			}
		}

		public override string ToString ()
		{
			return string.Format("class {0} : {1}", Name, SuperClass) ;
		}

	}
}
