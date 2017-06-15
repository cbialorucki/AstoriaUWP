using DalvikUWPCSharp.Disassembly.APKParser.struct_.xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    public class CompositeXmlStreamer : XmlStreamer
    {
        public XmlStreamer[] xmlStreamers;

        public CompositeXmlStreamer(params XmlStreamer[] xmlStreamers)
        {
            this.xmlStreamers = xmlStreamers;
        }

        public void onStartTag(XmlNodeStartTag xmlNodeStartTag)
        {
            foreach (XmlStreamer xmlStreamer in xmlStreamers)
            {
                xmlStreamer.onStartTag(xmlNodeStartTag);
            }
        }

        public void onEndTag(XmlNodeEndTag xmlNodeEndTag)
        {
            foreach (XmlStreamer xmlStreamer in xmlStreamers)
            {
                xmlStreamer.onEndTag(xmlNodeEndTag);
            }
        }

        public void onCData(XmlCData xmlCData)
        {
            foreach (XmlStreamer xmlStreamer in xmlStreamers)
            {
                xmlStreamer.onCData(xmlCData);
            }
        }

        public void onNamespaceStart(XmlNamespaceStartTag tag)
        {
            foreach (XmlStreamer xmlStreamer in xmlStreamers)
            {
                xmlStreamer.onNamespaceStart(tag);
            }
        }

        public void onNamespaceEnd(XmlNamespaceEndTag tag)
        {
            foreach (XmlStreamer xmlStreamer in xmlStreamers)
            {
                xmlStreamer.onNamespaceEnd(tag);
            }
        }
    }
}
