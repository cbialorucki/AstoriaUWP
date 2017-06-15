using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	public class Prototype
	{
		// Index into the String Ids list for the short-form descriptor string of this prototype. 
		// The string corresponds to the return type and parameters of this item
		internal uint ShortyIndex;

		// Index into the type_ids list for the return type of this prototype
		internal uint ReturnTypeIndex;

		internal List<ushort> Parameters; 

		private Dex Dex;

		internal Prototype (Dex dex, BinaryReader reader)
		{
			Dex = dex;

			ShortyIndex = reader.ReadUInt32 ();
			ReturnTypeIndex = reader.ReadUInt32 ();
			Parameters = dex.ReadTypeList(reader.ReadUInt32());
		}

		public override string ToString ()
		{
			return Dex.GetString(ShortyIndex);
		}

	}
}