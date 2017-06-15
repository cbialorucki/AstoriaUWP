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
    public class ResXMLTree_attrExt
    {
        public ResStringPool_ref Namespace { get; set; }
        public ResStringPool_ref Name { get; set; }
        public ushort AttributeStart { get; set; }
        public ushort AttributeSize { get; set; }
        public ushort AttributeCount { get; set; }
        public ushort IdIndex { get; set; }
        public ushort ClassIndex { get; set; }
        public ushort StyleIndex { get; set; }
    }
}