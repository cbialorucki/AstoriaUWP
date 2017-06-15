using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils.xml
{
    public class XmlEscaper
    {
        /**
     * <p>Escapes the characters in a {@code String} using XML entities.</p>
     */
        public static string escapeXml10(string input)
        {
            return XmlEscape(input);
            //return System.Security.SecurityElement.Escape(input);
            //return ESCAPE_XML10.translate(input);
        }

        public static string XmlEscape(string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }

    }
}
