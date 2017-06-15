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
    public class ResTable_package
    {
        public ResChunk_header Header { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; } // 128 x char16_t
        public uint TypeStrings { get; set; }
        public uint LastPublicType { get; set; }
        public uint KeyStrings { get; set; }
        public uint LastPublicKey { get; set; }
        public uint TypeIdOffset { get; set; }
    }
}