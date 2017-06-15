using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class NullHeader : ChunkHeader
    {
        public NullHeader(int chunkType, int headerSize, long chunkSize) : base(chunkType, headerSize, chunkSize) { }
    }
}
