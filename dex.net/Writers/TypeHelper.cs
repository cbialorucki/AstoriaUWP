using System;
using System.Collections.Generic;
using System.IO;

namespace dex.net
{
	public class TypeHelper
	{
		public delegate string GetFieldNameDelegate (uint index, Class currentClass, bool isClass=false);
		public delegate void AnnotationToStringDelegate (TextWriter output, EncodedAnnotation annotation, Class currentClass, Indentation indent);

		internal Dex _dex;
		private GetFieldNameDelegate _getFieldName;
		private AnnotationToStringDelegate _annToString;

		public TypeHelper (GetFieldNameDelegate fieldNameDelegate, AnnotationToStringDelegate annToStringDelegate)
		{
			_getFieldName = fieldNameDelegate;
			_annToString = annToStringDelegate;
		}

		public string EncodedValueToString (EncodedValue value, Class currentClass)
		{
			switch (value.EncodedType) {
				case EncodedValueType.VALUE_BYTE:
				return ((EncodedNumber)value).AsByte().ToString();

				case EncodedValueType.VALUE_NULL:
				return "null";

				case EncodedValueType.VALUE_BOOLEAN:
				return ((EncodedNumber)value).AsBoolean().ToString().ToLower();

				case EncodedValueType.VALUE_SHORT:
				return ((EncodedNumber)value).AsShort().ToString();

				case EncodedValueType.VALUE_CHAR:
				return ((EncodedNumber)value).AsChar().ToString();

				case EncodedValueType.VALUE_INT:
				return ((EncodedNumber)value).AsInt().ToString();

				case EncodedValueType.VALUE_LONG:
				return ((EncodedNumber)value).AsLong().ToString();

				case EncodedValueType.VALUE_FLOAT:
				return ((EncodedNumber)value).AsFloat().ToString();

				case EncodedValueType.VALUE_DOUBLE:
				return ((EncodedNumber)value).AsDouble().ToString();

				case EncodedValueType.VALUE_STRING:
				return String.Format("\"{0}\"", _dex.GetString(((EncodedNumber)value).AsId()).Replace("\n", "\\n"));

				case EncodedValueType.VALUE_TYPE:
				return _dex.GetTypeName (((EncodedNumber)value).AsId ());

				case EncodedValueType.VALUE_FIELD:
				case EncodedValueType.VALUE_ENUM:
				return _getFieldName (((EncodedNumber)value).AsId (), currentClass, true);

				case EncodedValueType.VALUE_METHOD:
				var method = _dex.GetMethod (((EncodedNumber)value).AsId ());
				return string.Format("{0}.{1}", _dex.GetTypeName(method.ClassIndex), method.Name);

				case EncodedValueType.VALUE_ARRAY:
				var encodedArray = (EncodedArray)value;
				var array = new List<string> ((int)encodedArray.Count);
				foreach (var arrayValue in encodedArray.GetValues()) {
					array.Add (EncodedValueToString(arrayValue, currentClass));
				}
				return string.Format("[{0}]", string.Join(",", array));

				case EncodedValueType.VALUE_ANNOTATION:
				var stringAnnotation = new StringWriter ();
				_annToString (stringAnnotation, (EncodedAnnotation)value, currentClass, new Indentation ());
				return stringAnnotation.ToString ();
			}

			return "Unknown";
		}


		public string AccessFlagsToString (AccessFlag flag)
		{
			var access = new List<string>();

			if ((flag & AccessFlag.ACC_PUBLIC) != 0)
				access.Add ("public");

			if ((flag & AccessFlag.ACC_PRIVATE) != 0)
				access.Add ("private");

			if ((flag & AccessFlag.ACC_PROTECTED) != 0)
				access.Add ("protected");

			if ((flag & AccessFlag.ACC_STATIC) != 0)
				access.Add ("static");

			if ((flag & AccessFlag.ACC_FINAL) != 0)
				access.Add ("final");

			if ((flag & AccessFlag.ACC_SYNCHRONIZED) != 0)
				access.Add ("synchronized");

			if ((flag & AccessFlag.ACC_VOLATILE) != 0)
				access.Add ("volatile");

			if ((flag & AccessFlag.ACC_BRIDGE) != 0)
				access.Add ("bridge");

			if ((flag & AccessFlag.ACC_TRANSIENT) != 0)
				access.Add ("transient");

			if ((flag & AccessFlag.ACC_VARARGS) != 0)
				access.Add ("varargs");

			if ((flag & AccessFlag.ACC_NATIVE) != 0)
				access.Add ("native");

			if ((flag & AccessFlag.ACC_INTERFACE) != 0)
				access.Add ("interface");
			else if ((flag & AccessFlag.ACC_ABSTRACT) != 0)
				access.Add ("abstract");

			if ((flag & AccessFlag.ACC_STRICT) != 0)
				access.Add("strictfp");

			if ((flag & AccessFlag.ACC_SYNTHETIC) != 0)
				access.Add("synthetic");

			if ((flag & AccessFlag.ACC_ANNOTATION) != 0)
				access.Add("annotation");

			if ((flag & AccessFlag.ACC_ENUM) != 0)
				access.Add("enum");

			if ((flag & AccessFlag.ACC_CONSTRUCTOR) != 0)
				access.Add("constructor");

			if ((flag & AccessFlag.ACC_DECLARED_SYNCHRONIZED) != 0)
				access.Add("synchronized");

			return string.Join (" ", access);
		}

		public string AccessAndType(Class dexClass) {
			var accessAndType = AccessFlagsToString (dexClass.AccessFlags);
			if ((dexClass.AccessFlags & (AccessFlag.ACC_INTERFACE | AccessFlag.ACC_ENUM)) == 0) {
				if (string.IsNullOrWhiteSpace (accessAndType)) {
					accessAndType += "class";
				} else {
					accessAndType += " class";
				}
			}

			return accessAndType;
		}
	}
}