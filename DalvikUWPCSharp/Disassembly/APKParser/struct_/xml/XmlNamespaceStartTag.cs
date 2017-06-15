using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.xml
{
    public class XmlNamespaceStartTag
    {
        private string prefix;
        private string uri;

        public string getPrefix()
        {
            return prefix;
        }

        public void setPrefix(string prefix)
        {
            this.prefix = prefix;
        }

        public string getUri()
        {
            return uri;
        }

        public void setUri(string uri)
        {
            this.uri = uri;
        }

        public string toString()
        {
            return prefix + "=" + uri;
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
