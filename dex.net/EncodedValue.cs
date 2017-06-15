using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	public abstract class EncodedValue
	{
		public EncodedValueType EncodedType { get; internal set; }

		internal static EncodedValue parse(BinaryReader reader) 
		{
			var rawType = reader.ReadByte();

			var type = (EncodedValueType)(rawType & 0x1f);
			var valueType = (byte)(rawType >> 5);

			if (type == EncodedValueType.VALUE_ANNOTATION) {
				return new EncodedAnnotation (reader) { EncodedType = type };
			} else if (type == EncodedValueType.VALUE_ARRAY) {
				return new EncodedArray (reader) { EncodedType = type };
			} else {
				return new EncodedNumber (reader, valueType, type);
			}
		}

	}

	public class EncodedArray : EncodedValue
	{
		internal EncodedValue[] EncodedValues { get; private set; }
		public ulong Count { get; private set; }

		public IEnumerable<EncodedValue> GetValues()
		{
			for (uint i=0; i<EncodedValues.Length; i++) {
				yield return EncodedValues[i];
			}
		}

		internal EncodedArray(BinaryReader reader)
		{
			Count = Leb128.ReadUleb(reader);
			EncodedValues = new EncodedValue[Count];

			for (ulong i=0; i<Count; i++) {
				EncodedValues[i] = EncodedValue.parse(reader);
			}
		}
	}

	public class EncodedNumber : EncodedValue
	{
		private readonly byte[] Value;
		private readonly byte valueType;

		internal EncodedNumber(BinaryReader reader, byte valueType, EncodedValueType type)
		{
			this.valueType = valueType;

			EncodedType = type;
			switch (EncodedType) {
				case EncodedValueType.VALUE_BYTE:
				Value = new byte[1];
				Value[0] = reader.ReadByte();
				break;

				case EncodedValueType.VALUE_NULL:
				case EncodedValueType.VALUE_BOOLEAN:
				Value = null;
				break;

				case EncodedValueType.VALUE_FLOAT:
				case EncodedValueType.VALUE_DOUBLE:
				case EncodedValueType.VALUE_SHORT:
				case EncodedValueType.VALUE_CHAR:
				case EncodedValueType.VALUE_INT:
				case EncodedValueType.VALUE_LONG:
				case EncodedValueType.VALUE_STRING:
				case EncodedValueType.VALUE_TYPE:
				case EncodedValueType.VALUE_FIELD:
				case EncodedValueType.VALUE_METHOD:
				case EncodedValueType.VALUE_ENUM:
				Value = new byte[valueType+1];
				for (int i=0; i<=valueType; i++) {
					Value[i] = reader.ReadByte();
				}
				break;
			}
		}

		private byte[] GetDataExtended(byte length) {
			if (length == valueType+1)
				return Value;

			var data = new byte[length];
			Array.Copy (Value, 0, data, length-1-valueType, valueType+1);
			return data;
		}

		public sbyte AsByte ()
		{
			return (sbyte)Value[0];
		}

		public short AsShort ()
		{
			return BitConverter.ToInt16(GetDataExtended(2), 0);
		}

		public char AsChar ()
		{
			return (char)BitConverter.ToUInt16(GetDataExtended(2), 0);
		}

		public int AsInt ()
		{
			return BitConverter.ToInt32(GetDataExtended(4), 0);
		}

		public long AsLong ()
		{
			return BitConverter.ToInt64(GetDataExtended(8), 0);
		}

		public float AsFloat ()
		{
			return BitConverter.ToSingle(GetDataExtended(4), 0);
		}

		public double AsDouble ()
		{
			return BitConverter.ToDouble(GetDataExtended(8), 0);
		}

		public uint AsId ()
		{
			if (valueType == 3)
				return BitConverter.ToUInt32(Value, 0);

			var data = new byte[4];
			Array.Copy (Value, data, valueType+1);
			return BitConverter.ToUInt32(data, 0);
		}

		public bool AsBoolean ()
		{
			return valueType == 1 ? true : false;
		}

		public object AsNull ()
		{
			return null;
		}
	}
}