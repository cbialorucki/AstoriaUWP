// Copyright (c) 2015 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AndroidXml
{
    public class PublicValuesReader
    {
        private static readonly object valuesLock = new object();
        private static Dictionary<uint, string> values;

        public static Dictionary<uint, string> Values
        {
            get
            {
                // Prevent two threads from initializing the values field
                // at the same time. This can happen when this code is called from
                // parallel threads.
                lock (valuesLock)
                {
                    if (values == null)
                    {
                        using (var stream = EmbeddedResources.PublicXml)
                        {
                            XDocument xdoc = XDocument.Load(stream);
                            var publicValues = xdoc.Element("resources").Elements("public");
                            values = new Dictionary<uint, string>();
                            publicValues.ToList().ForEach(pv => AddValue(pv));
                        }
                    }
                }

                return values;
            }
        }

        private static void AddValue(XElement publicValue)
        {
            var id = publicValue.Attribute("id");
            var name = publicValue.Attribute("name");
            if (id != null && name != null)
            {
                var identifier = Convert.ToUInt32(id.Value, 16);
                values.Add(identifier, name.Value);
            }
        }
    }
}
