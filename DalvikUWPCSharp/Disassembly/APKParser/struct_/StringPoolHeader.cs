using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_
{
    public class StringPoolHeader : ChunkHeader
    {
        public StringPoolHeader(int chunkType, int headerSize, long chunkSize) : base(chunkType, headerSize, chunkSize) { }

        // Number of style span arrays in the pool (number of uint32_t indices
        // follow the string indices).
        private long stringCount;
        // Number of style span arrays in the pool (number of uint32_t indices
        // follow the string indices).
        private long styleCount;

        // If set, the string index is sorted by the string values (based on strcmp16()).
        public const int SORTED_FLAG = 1;
        // String pool is encoded in UTF-8
        public const int UTF8_FLAG = 1 << 8;
        private long flags;

        // Index from header of the string data.
        private long stringsStart;
        // Index from header of the style data.
        private long stylesStart;

        public long getStringCount()
        {
            return stringCount;
        }

        public void setStringCount(long stringCount)
        {
            this.stringCount = stringCount;
        }

        public long getStyleCount()
        {
            return styleCount;
        }

        public void setStyleCount(long styleCount)
        {
            this.styleCount = styleCount;
        }

        public long getFlags()
        {
            return flags;
        }

        public void setFlags(long flags)
        {
            this.flags = flags;
        }

        public long getStringsStart()
        {
            return stringsStart;
        }

        public void setStringsStart(long stringsStart)
        {
            this.stringsStart = stringsStart;
        }

        public long getStylesStart()
        {
            return stylesStart;
        }

        public void setStylesStart(long stylesStart)
        {
            this.stylesStart = stylesStart;
        }
    }
}
