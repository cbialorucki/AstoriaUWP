using System;
using System.IO;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
using System.Text;


namespace dex.net
{
	public class DexHeader
	{
		/// <summary>
		/// Miscelaneous Header Fields
		/// </summary>
		internal uint Checksum;
		internal byte[] Signature;
		internal uint FileSize;
		internal uint HeaderSize;
		internal bool IsLittleEndian;
		internal uint MapOffset;

		/// <summary>
		/// String Ids Section
		/// </summary>
		internal uint StringIdsCount;
		internal uint StringIdsOffset;

		/// <summary>
		/// Type Identifiers Section
		/// </summary>
		internal uint TypeIdsCount;
		internal uint TypeIdsOffset;

		/// <summary>
		/// Method Prototypes Section
		/// </summary>
		internal uint PrototypeIdsCount;
		internal uint PrototypeIdsOffset;

		/// <summary>
		/// Field Ids Section
		/// </summary>
		internal uint FieldIdsCount;
		internal uint FieldIdsOffset;

		/// <summary>
		/// Methods Section
		/// </summary>
		internal uint MethodIdsCount;
		internal uint MethodIdsOffset;

		/// <summary>
		/// Class Definitions Section
		/// </summary>
		internal uint ClassDefinitionsCount;
		internal uint ClassDefinitionsOffset;

		/// <summary>
		/// Data Section
		/// </summary>
		internal uint DataSize;
		internal uint DataOffset;

		/// <summary>
		/// Link Section
		/// </summary>
		internal uint LinkSize;
		internal uint LinkOffset;

		internal string ApiVersion;

		private DexHeader ()
		{
		}

		internal static DexHeader Parse(Stream dexStream) 
		{
			dexStream.Seek(0, SeekOrigin.Begin);
			var reader = new BinaryReader(dexStream);

			// Check for the DEX signature in the first bytes of the file
			var fileMagic = Encoding.UTF8.GetString (reader.ReadBytes (4));
			if (fileMagic.Equals("dey\n")) {
				throw new ArgumentException("Optimized DEX files are not supported");
			} else if (!fileMagic.Equals("dex\n")) {
				throw new ArgumentException(string.Format("Invalid DEX file - wrong signature. Found {0}", fileMagic));
			}


			DexHeader header = new DexHeader();
			header.ApiVersion = Encoding.UTF8.GetString (reader.ReadBytes (4));
			header.Checksum = reader.ReadUInt32();
			header.Signature = reader.ReadBytes(20);
			header.FileSize = reader.ReadUInt32();
			header.HeaderSize = reader.ReadUInt32();
			header.IsLittleEndian = reader.ReadUInt32() == 0x12345678;
			header.LinkSize = reader.ReadUInt32();
			header.LinkOffset = reader.ReadUInt32();
			header.MapOffset = reader.ReadUInt32();
			header.StringIdsCount = reader.ReadUInt32();
			header.StringIdsOffset = reader.ReadUInt32();
			header.TypeIdsCount = reader.ReadUInt32();
			header.TypeIdsOffset = reader.ReadUInt32();
			header.PrototypeIdsCount = reader.ReadUInt32();
			header.PrototypeIdsOffset = reader.ReadUInt32();
			header.FieldIdsCount = reader.ReadUInt32();
			header.FieldIdsOffset = reader.ReadUInt32();
			header.MethodIdsCount = reader.ReadUInt32();
			header.MethodIdsOffset = reader.ReadUInt32();
			header.ClassDefinitionsCount = reader.ReadUInt32();
			header.ClassDefinitionsOffset = reader.ReadUInt32();
			header.DataSize = reader.ReadUInt32();
			header.DataOffset = reader.ReadUInt32();

			return header;
		}

		public override string ToString ()
		{
			return String.Format(@"
Checksum={0}
Signature={1}
FileSize={2}
HeaderSize={3}
IsLittleEndian={4}
LinkSize={5}
LinkOffset={6}
MapOffset={7}
StringIdsCount={8}
StringIdsOffset={9}
TypeIdsCount={10}
TypeIdsOffset={11}
PrototypeIdsCount={12}
PrototypeIdsOffset={13}
FieldIdsCount={14}
FieldIdsOffset={15}
MethodIdsCount={16}
MethodIdsOffset={17}
ClassDefinitionsCount={18}
ClassDefinitionsOffset={19}
DataSize={20}
DataOffset={21}
ApiVersion={22}
", 
					   Checksum, Signature, FileSize, HeaderSize, IsLittleEndian,
					   LinkSize, LinkOffset, MapOffset, StringIdsCount, StringIdsOffset,
					   TypeIdsCount, TypeIdsOffset, PrototypeIdsCount, PrototypeIdsOffset,
					   FieldIdsCount, FieldIdsOffset, MethodIdsCount, MethodIdsOffset,
					   ClassDefinitionsCount, ClassDefinitionsOffset, DataSize, DataOffset, ApiVersion);
		}
	}
}