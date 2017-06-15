using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_
{
    public class ChunkType
    {
        public const int NULL = 0x0000;
        public const int STRING_POOL = 0x0001;
        public const int TABLE = 0x0002;
        public const int XML = 0x0003;

        // Chunk types in XML
        public const int XML_FIRST_CHUNK = 0x0100;
        public const int XML_START_NAMESPACE = 0x0100;
        public const int XML_END_NAMESPACE = 0x0101;
        public const int XML_START_ELEMENT = 0x0102;
        public const int XML_END_ELEMENT = 0x0103;
        public const int XML_CDATA = 0x0104;
        public const int XML_LAST_CHUNK = 0x017f;
        // This contains a uint32_t array mapping strings in the string
        // pool back to resource identifiers.  It is optional.
        public const int XML_RESOURCE_MAP = 0x0180;

        // Chunk types in RES_TABLE_TYPE
        public const int TABLE_PACKAGE = 0x0200;
        public const int TABLE_TYPE = 0x0201;
        public const int TABLE_TYPE_SPEC = 0x0202;
        // android5.0+
        // DynamicRefTable
        public const int TABLE_LIBRARY = 0x0203;
    }
}
