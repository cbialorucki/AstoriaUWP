// Copyright (c) 2012 Markus Jarderot
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System.Collections.Generic;

namespace AndroidXml.Res
{
    public class ResXMLTree_startelement
    {
        public ResXMLTree_node Node { get; set; }
        public ResXMLTree_attrExt AttrExt { get; set; }
        public List<ResXMLTree_attribute> Attributes { get; set; }
    }
}