// Copyright (c) 2012 Markus Jarderot
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;

namespace AndroidXml.Res
{
#if !NETSTANDARD1_3
    //[Serializable]
#endif
    public class ResStringPool_header
    {
        public ResChunk_header Header { get; set; }
        public uint StringCount { get; set; }
        public uint StyleCount { get; set; }
        public StringPoolFlags Flags { get; set; }
        public uint StringStart { get; set; }
        public uint StylesStart { get; set; }
    }

    [Flags]
    public enum StringPoolFlags
    {
        SORTED_FLAG = 1 << 0,
        UTF8_FLAG = 1 << 8
    }
}