using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.xml
{
    public class XmlNodeEndTag
    {
        private string nspace;
        private string name;

        public string getNamespace()
        {
            return nspace;
        }

        public void setNamespace(string nspace) {
            this.nspace = nspace;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("</");
            if (nspace != null)
            {
                sb.Append(nspace).Append(":");
            }
            sb.Append(name).Append('>');
            return sb.ToString();
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
