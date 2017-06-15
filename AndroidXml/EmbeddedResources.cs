using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AndroidXml
{
    internal class EmbeddedResources
    {
        public static Stream PublicXml
        {
            get
            {
                return typeof(EmbeddedResources)
                    .GetTypeInfo()
                    .Assembly
                    .GetManifestResourceStream("AndroidXml.Values.public.xml");
                    //.GetManifestResourceStream("")
            }
        }
    }
}
