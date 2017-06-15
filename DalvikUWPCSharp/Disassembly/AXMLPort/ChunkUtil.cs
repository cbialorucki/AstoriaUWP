using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.AXMLPort
{
    class ChunkUtil
    {
        public static void readCheckType(IntReader reader, int expectedType)
        {

            int type = reader.readInt();
            if (type != expectedType)
            {
                throw new IOException("Expected chunk of type 0x" + expectedType.ToString("X") + ", read 0x" + type.ToString("X") + ".");
                //throw new IOException("Expected chunk of type 0x" + Integer.toHexString(expectedType) + ", read 0x" + Integer.toHexString(type) + ".");
            }
        }       
	}
}
