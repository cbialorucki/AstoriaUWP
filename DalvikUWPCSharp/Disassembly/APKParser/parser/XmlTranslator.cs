using DalvikUWPCSharp.Disassembly.APKParser.struct_.xml;
using DalvikUWPCSharp.Disassembly.APKParser.utils.xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    class XmlTranslator : XmlStreamer
    {
        private StringBuilder sb;
        private int shift = 0;
        private XmlNamespaces namespaces;
        private bool isLastStartTag;

        public XmlTranslator()
        {
            sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
            this.namespaces = new XmlNamespaces();
        }

        public void onStartTag(XmlNodeStartTag xmlNodeStartTag)
        {
            if (isLastStartTag)
            {
                sb.Append(">\n");
            }
            appendShift(shift++);
            sb.Append('<');
            if (xmlNodeStartTag.getNamespace() != null)
            {
                string prefix = namespaces.getPrefixViaUri(xmlNodeStartTag.getNamespace());
                if (prefix != null)
                {
                    sb.Append(prefix).Append(":");
                }
                else
                {
                    sb.Append(xmlNodeStartTag.getNamespace()).Append(":");
                }
            }
            sb.Append(xmlNodeStartTag.getName());

            List<XmlNamespaces.XmlNamespace> nps = namespaces.consumeNameSpaces();
            //if (!nps.isEmpty())
            if(nps.Count != 0)
            {
                foreach(XmlNamespaces.XmlNamespace np in nps)
                {
                    sb.Append(" xmlns:").Append(np.getPrefix()).Append("=\"").Append(np.getUri()).Append("\"");
                }
            }

            isLastStartTag = true;

            foreach (Attribute_ attribute in xmlNodeStartTag.getAttributes().value())
            {
                onAttribute(attribute);
            }
        }

        private void onAttribute(Attribute_ attribute)
        { 
            sb.Append(" ");
            string nspace = this.namespaces.getPrefixViaUri(attribute.getNamespace());

            if (nspace == null)
            {
                nspace = attribute.getNamespace();
            }
            if (nspace != null && !nspace.Equals(string.Empty))
            {
                sb.Append(nspace).Append(':');
            }
            string escapedFinalValue = XmlEscaper.escapeXml10(attribute.getValue());
            sb.Append(attribute.getName()).Append('=').Append('"').Append(escapedFinalValue).Append('"');
        }


        public void onEndTag(XmlNodeEndTag xmlNodeEndTag)
        {
            --shift;
            if (isLastStartTag)
            {
                sb.Append(" />\n");
            }
            else
            {
                appendShift(shift);
                sb.Append("</");
                if (xmlNodeEndTag.getNamespace() != null)
                {
                    sb.Append(xmlNodeEndTag.getNamespace()).Append(":");
                }
                sb.Append(xmlNodeEndTag.getName());
                sb.Append(">\n");
            }

            isLastStartTag = false;
        }

        public void onCData(XmlCData xmlCData)
        {
            appendShift(shift);
            sb.Append(xmlCData.getValue()).Append('\n');
            isLastStartTag = false;
        }

        public void onNamespaceStart(XmlNamespaceStartTag tag)
        {
            this.namespaces.addNamespace(tag);
        }

        public void onNamespaceEnd(XmlNamespaceEndTag tag)
        {
            this.namespaces.removeNamespace(tag);
        }

        private void appendShift(int shift)
        {
            for (int i = 0; i < shift; i++)
            {
                sb.Append("\t");
            }
        }

        public string getXml()
        {
            return sb.ToString();
        }
    }
}
