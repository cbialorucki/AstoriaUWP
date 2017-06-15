using DalvikUWPCSharp.Disassembly.APKParser.utils.xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.xml
{
    public class XmlNodeStartTag
    {
        private string nspace;
        private string name;

        // Byte offset from the start of this structure where the attributes start. uint16
        //public int attributeStart;
        // Size of the ResXMLTree_attribute structures that follow. unit16
        //public int attributeSize;
        // Number of attributes associated with an ELEMENT. uint 16
        // These are available as an array of ResXMLTree_attribute structures immediately following this node.
        //public int attributeCount;
        // Index (1-based) of the "id" attribute. 0 if none. uint16
        //public short idIndex;
        // Index (1-based) of the "style" attribute. 0 if none. uint16
        //public short styleIndex;

        private Attributes attributes;

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

        public Attributes getAttributes()
        {
            return attributes;
        }

        public void setAttributes(Attributes attributes)
        {
            this.attributes = attributes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('<');
            if (nspace != null)
            {
                sb.Append(nspace).Append(":");
            }
            sb.Append(name);
            sb.Append('>');
            return sb.ToString();
        }

        public string toString()
        {
            return this.ToString();
        }
    }
}
