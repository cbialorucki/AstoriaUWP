// Copyright (c) 2012 Markus Jarderot
// Copyright (c) 2016 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;

namespace AndroidXml.Res
{
#if !NETSTANDARD1_3
    //[Serializable]
#endif
    public class ResChunk_header
    {
        /// <summary>
        /// The number of bytes the <see cref="ResChunk_header"/> class occupies on disk.
        /// </summary>
        public static ushort DataSize
        {
            get
            {
                // Type and HeaderSize are uint16_t; size is uint32_t
                return 8;
            }
        }

        /// Type identifier for this chunk.  The meaning of this value depends on the containing chunk.
        public ResourceType Type { get; set; }

        /// Size of the chunk header (in bytes).  Adding this value to the address of the chunk allows
        /// you to find its associated data (if any).
        public ushort HeaderSize { get; set; }

        /// Total size of this chunk (in bytes).  This is the chunkSize plus the size of any data 
        /// associated with the chunk.  Adding this value to the chunk allows you to completely skip
        /// its contents (including any child chunks).  If this value is the same as chunkSize, there 
        /// is no data associated with the chunk.
        public uint Size { get; set; }
    }

    public enum ResourceType
    {
        RES_NULL_TYPE = 0x0000,
        RES_STRING_POOL_TYPE = 0x0001,
        RES_TABLE_TYPE = 0x0002,
        RES_XML_TYPE = 0x0003,
        //RES_XML_FIRST_CHUNK_TYPE = 0x0100,
        RES_XML_START_NAMESPACE_TYPE = 0x0100,
        RES_XML_END_NAMESPACE_TYPE = 0x0101,
        RES_XML_START_ELEMENT_TYPE = 0x0102,
        RES_XML_END_ELEMENT_TYPE = 0x0103,
        RES_XML_CDATA_TYPE = 0x0104,
        //RES_XML_LAST_CHUNK_TYPE = 0x017f,
        RES_XML_RESOURCE_MAP_TYPE = 0x0180,
        RES_TABLE_PACKAGE_TYPE = 0x0200,
        RES_TABLE_TYPE_TYPE = 0x0201,
        RES_TABLE_TYPE_SPEC_TYPE = 0x0202
    };
}