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
    public class ResXMLTree_cdataExt
    {
        public ResStringPool_ref Data { get; set; }
        public Res_value TypedData { get; set; }
    }
}