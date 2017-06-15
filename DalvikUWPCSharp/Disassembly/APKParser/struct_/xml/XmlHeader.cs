using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.xml
{
    public class XmlHeader : ChunkHeader
    {
        public XmlHeader(int chunkType, int headerSize, long chunkSize) : base(chunkType, headerSize, chunkSize) { }
    }
}
