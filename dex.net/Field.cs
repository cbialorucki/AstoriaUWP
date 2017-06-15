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
	public class Field
	{
		private Dex Dex;

		public uint Id;

		// Index into the Type Ids list for the definer of this field
		public ushort ClassIndex { get; internal set; }

		// Index into the Type Ids list for the type of this field
		public ushort TypeIndex { get; internal set; }

		// index into the string_ids list for the name of this field
		internal uint NameIndex;

		internal AccessFlag AccessFlags;

		public string Name 
		{
			get { return Dex.GetString (NameIndex); }
		}

		public IEnumerable<Annotation> Annotations { get; internal set; }

		internal Field (uint id, Dex dex, BinaryReader reader)
		{
			Id = id;
			Dex = dex;

			ClassIndex = reader.ReadUInt16 ();
			TypeIndex = reader.ReadUInt16 ();
			NameIndex = reader.ReadUInt32 ();

			Annotations = new Annotation[0];
		}
		
		public override string ToString ()
		{
			return string.Format ("Field: Class={0} Type={1} Name={2} Id={3}", Dex.GetTypeName(ClassIndex), Dex.GetTypeName(TypeIndex), Name, Id);
		}
	}
}

