using System;
using System.IO;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	public class Leb128
	{
		public static uint ReadUleb (BinaryReader reader)
		{
			return ParseLeb(reader);
		}

		public static int ReadLeb (BinaryReader reader)
		{
			return (int)ParseLeb(reader, true);
		}
		
		public static int ReadULebP1 (BinaryReader reader)
		{
			return ((int)ParseLeb(reader)) - 1;
		}

		private static uint ParseLeb (BinaryReader reader, bool isSignExtended=false)
		{
			uint value = 0;
			byte bytesRead = 0;
			byte currentByte;

			do
			{
				if (bytesRead == 5)
					throw new OverflowException("LEB number too long, Android only encodes 32bit values as LEB");

				currentByte = reader.ReadByte();
				value |= (uint)((currentByte&0x7f) << (7*bytesRead));

				bytesRead++;
			} while ((currentByte & 0x80) != 0);

			// Negative number, must be sign extended
			if (isSignExtended && (currentByte & 0x40) != 0) {
				switch (bytesRead) {
					case 1:
						value |= 0xffffff80;
						break;
					case 2:
						value |= 0xffffc000;
						break;
					case 3:
						value |= 0xffe00000;
						break;
					case 4:
						value |= 0xf0000000;
						break;
				}
			}

			return value;
		}
	}
}