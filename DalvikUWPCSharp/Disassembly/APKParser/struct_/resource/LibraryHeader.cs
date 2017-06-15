using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class LibraryHeader : ChunkHeader
    {
        /**
     * A package-id to package name mapping for any shared libraries used
     * in this resource table. The package-id's encoded in this resource
     * table may be different than the id's assigned at runtime. We must
     * be able to translate the package-id's based on the package name.
     */

        /**
         * uint32 value, The number of shared libraries linked in this resource table.
         */
        private long count;

        public LibraryHeader(int chunkType, int headerSize, long chunkSize) : base(chunkType, headerSize, chunkSize) { }

        public long getCount()
        {
            return count;
        }

        public void setCount(long count)
        {
            this.count = count;
        }
    }
}
