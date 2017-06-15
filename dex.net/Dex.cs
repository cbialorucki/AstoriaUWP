using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	internal class DexReader : BinaryReader
	{
		long ReturnPos;
		object Lock;

		public DexReader(Stream stream, long position, object theLock) : base(stream, Encoding.UTF8, true) //base(stream, System.Text.Encoding.Default, true) {
        {
			ReturnPos = stream.Position;
			stream.Position = position;
			Lock = theLock;
		}

		public new void Dispose ()
		{
			BaseStream.Position = ReturnPos;
			Monitor.Exit (Lock);
			base.Dispose ();
		}
	}

	/// <summary>
	/// Dex
	/// </summary>
	/// <exception cref='ArgumentException'>
	/// Is thrown when an argument passed to a method is invalid.
	/// </exception>
	/// <exception cref='OverflowException'>
	/// Is thrown when the result of an arithmetic operation is too large to be represented by the destination type.
	/// </exception>
	public class Dex : IDisposable
	{
		private Dictionary<TypeCode,MapItem> SectionsMap;
		private Stream DexStream;
		private DexHeader DexHeader;

		public Dex (Stream dexStream)
		{
			if (!dexStream.CanSeek) {
				throw new ArgumentException ("Must be able to seek the DEX stream");
			}

			DexStream = dexStream;
			DexHeader = DexHeader.Parse(dexStream);
			SectionsMap = ReadSectionsMap();
		}

		#region IDisposable implementation

		public void Dispose ()
		{
            DexStream.Dispose(); //.Close();
			SectionsMap.Clear();
			DexHeader = null;
		}

		#endregion

		internal BinaryReader GetReader(long pos)
		{
			Monitor.Enter (this);
			return new DexReader (DexStream, pos, this);
		}

		/// <summary>
		/// Return a String given an index
		/// </summary>
		/// <returns>
		/// The string
		/// </returns>
		/// <param name='index'>
		/// Index in the Strings table
		/// </param>
		public string GetString (uint index)
		{
			// Sanity check
			if (index >= DexHeader.StringIdsCount)
				throw new ArgumentException(String.Format("String id {0} outside the range {1}", index, DexHeader.StringIdsCount));

			// Find the offset of the String ID in the Strings table 
			// and position the stream at the entry
			var offset = DexHeader.StringIdsOffset + (index*4);
			using (var reader = GetReader (offset)) {
				return ReadString(reader.ReadUInt32(), reader);
			}
		}

		/// <summary>
		/// Iterate over all Strings
		/// </summary>
		/// <returns>
		/// All Strings in the DEX
		/// </returns>
		public IEnumerable<string> GetStrings ()
		{
			for (uint i=0; i<DexHeader.StringIdsCount; i++) {
				yield return GetString (i);
			}
		}

		/// <summary>
		/// Read a string at the specified offset in the DEX file. The layout of a String is:
		/// 	length: length of the decoded UTF16 string
		/// 	data: string encoded in MUTF8 ending in a \0
		/// </summary>
		/// <returns>
		/// The string
		/// </returns>
		/// <param name='offset'>
		/// Offset of the string from the beginning of the file
		/// </param>
		/// <param name='reader'>
		/// stream reader to parse values from the Dex file
		/// </param>
		private string ReadString (uint offset, BinaryReader reader)
		{
			reader.BaseStream.Position = offset;

			// find out the length of the decoded string
			var stringLength = (int)Leb128.ReadUleb(reader);

			// strings are encoded in MUTF-8 format
			char[] chararr = new char[stringLength];
			int c, char2, char3;
			int chararr_count=0;

			while (chararr_count < stringLength) {
				c = reader.ReadByte();

				switch (c >> 4) {
					/* 0xxxxxxx */
					case 0: 
					case 1: 
					case 2: 
					case 3: 
					case 4: 
					case 5: 
					case 6: 
					case 7:
						chararr[chararr_count++]=(char)c;
						break;

					/* 110x xxxx || 10xx xxxx */
					case 12: 
					case 13:
						char2 = reader.ReadByte();
						if ((char2 & 0xC0) != 0x80) {
							throw new InvalidDataException("MUTF-8 parsing error. 2nd byte must be 10xxxxxx. Dex offset:" + (DexStream.Position-2));
						}
						chararr[chararr_count++]=(char)(((c & 0x1F) << 6) | (char2 & 0x3F));
						break;

					/* 1110 xxxx || 10xx xxxx || 10xx xxxx */
					case 14:
						char2 = reader.ReadByte();
						char3 = reader.ReadByte();
						if (((char2 & 0xC0) ^ (char3 & 0xC0)) != 0) {
							throw new InvalidDataException("MUTF-8 parsing error. Both bytes must be 10xxxxxx. Dex offset:" + (DexStream.Position-3));
						}
						chararr[chararr_count++]=(char)(((c     & 0x0F) << 12) |
						                                ((char2 & 0x3F) << 6)  |
						                                ((char3 & 0x3F) << 0));
						break;

					default:
						throw new InvalidDataException("Invalid MUTF-8 encoding. Dex offset: " + (DexStream.Position-1));
				}
			}

			// The number of chars produced may be less than utflen
			return new string(chararr, 0, chararr_count);
		}


		private Dictionary<TypeCode,MapItem> ReadSectionsMap ()
		{
			// Position the stream at the beginning of the map
			using (var reader = GetReader (DexHeader.MapOffset)) {
				// Number of entries in the map
				var count = reader.ReadUInt32();

				// Read all entries from the DEX and add to the dictionary
				var map = new Dictionary<TypeCode,MapItem> ();
				for (int i=0; i<count; i++) {
					var itemType = (TypeCode)reader.ReadUInt16();
					// Skip the unused field
					reader.ReadUInt16();

					MapItem item = new MapItem();
					item.Count = reader.ReadUInt32();
					item.Offset = reader.ReadUInt32();

					map.Add(itemType, item);
				}

				return map;
			}
		}

		public Prototype GetPrototype (uint id)
		{
			if (id >= DexHeader.PrototypeIdsCount)
				throw new ArgumentException (string.Format ("Prototype Id {0} out of range 0-{1}", id, DexHeader.PrototypeIdsCount));

			using (var reader = GetReader (DexHeader.PrototypeIdsOffset + (id * 12))) {
				return new Prototype (this, reader);
			}
		}

		internal List<ushort> ReadTypeList (uint offset)
		{
			// Offset 0 means the type list is empty
			if (offset == 0) {
				return new List<ushort>();
			}

			using (var reader = GetReader (offset)) {
				var count = reader.ReadUInt32();
				var types = new List<ushort> ((int)count);

				while (count-- > 0) {
					types.Add(reader.ReadUInt16());
				}

				return types;
			}
		}

		public IEnumerable<Prototype> GetPrototypes ()
		{
			for (uint i=0; i<DexHeader.PrototypeIdsCount; i++) {
				yield return GetPrototype (i);
			}
		}

		public Field GetField (uint id)
		{
			if (id > DexHeader.FieldIdsCount)
				throw new ArgumentException (string.Format ("Field Id {0} out of range 0-{1}", id, DexHeader.FieldIdsCount));

			using (var reader = GetReader(DexHeader.FieldIdsOffset + (id * 8))) {
				return new Field (id, this, reader);
			}
		}

		public IEnumerable<Field> GetFields ()
		{
			for (uint i=0; i<DexHeader.FieldIdsCount; i++) {
				yield return GetField (i);
			}
		}

		public Method GetMethod (uint id, uint codeOffset = 0)
		{
			if (id >= DexHeader.MethodIdsCount)
				throw new ArgumentException (string.Format ("Method Id {0} out of range 0-{1}", id, DexHeader.MethodIdsCount));

			using (var reader = GetReader (DexHeader.MethodIdsOffset + (id * 8))) {
				return new Method (id, this, reader, codeOffset);
			}
		}

		public IEnumerable<Method> GetMethods ()
		{
			for (uint i=0; i<DexHeader.MethodIdsCount; i++) {
				yield return GetMethod (i);
			}
		}

		public int ClassCount
		{
			get { return (int)DexHeader.ClassDefinitionsCount; }
		}

		public Class GetClass (uint id)
		{
			if (id >= DexHeader.ClassDefinitionsCount)
				throw new ArgumentException (string.Format ("Class Id {0} out of range 0-{1}", id, DexHeader.ClassDefinitionsCount-1));

			using (var reader = GetReader (DexHeader.ClassDefinitionsOffset + (id * 32))) {
				return new Class (this, reader);
			}
		}

		public IEnumerable<Class> GetClasses ()
		{
			for (uint i=0; i<DexHeader.ClassDefinitionsCount; i++) {
				yield return GetClass (i);
			}
		}

		public string GetTypeName (uint id)
		{
			if (id >= DexHeader.TypeIdsCount)
				throw new ArgumentException(string.Format("Type Id {0} out of range 0-{1}", id, DexHeader.TypeIdsCount));

			using (var reader = GetReader (DexHeader.TypeIdsOffset + (id*4))) {
				return TypeToString(GetString(reader.ReadUInt32()));
			}
		}

		public IEnumerable<string> GetTypeNames ()
		{
			for (uint i=0; i<DexHeader.TypeIdsCount; i++) {
				yield return GetTypeName (i);
			}
		}

		internal string TypeToString (string typeDescriptor)
		{
			if (String.IsNullOrWhiteSpace(typeDescriptor))
				return "";

			switch (typeDescriptor [0]) {
				case 'V':
				return "void";

				case 'Z':
				return "boolean";

				case 'B':
				return "byte";

				case 'S':
				return "short";

				case 'C':
				return "char";

				case 'I':
				return "int";

				case 'J':
				return "long";

				case 'F':
				return "float";

				case 'D':
				return "double";

				case 'L':
				return typeDescriptor.Replace('/', '.').Substring(1, typeDescriptor.Length-2);

				case '[':
				return TypeToString(typeDescriptor.Substring(1)) + "[]";

				default:
				return "unknown";
			}
		}
	}

	struct MapItem
	{
		// Number of items at the offset
		internal uint Count;

		// Offset from the start of the file
		internal uint Offset;

		public override string ToString ()
		{
			return string.Format("Offset:{0} Count:{1}", Offset, Count);
		}
	}
}
