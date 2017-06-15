using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class ResourceTableHeader : ChunkHeader
    {
        // The number of ResTable_package structures. uint32
        private long packageCount;

        public ResourceTableHeader(int chunkType, int headerSize, long chunkSize) : base(chunkType, headerSize, chunkSize)
        {
            //super(chunkType, headerSize, chunkSize);
        }

        public long getPackageCount()
        {
            return packageCount;
        }

        public void setPackageCount(long packageCount)
        {
            this.packageCount = packageCount;
        }
    }
}
