using DalvikUWPCSharp.Disassembly.APKParser.struct_.xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    public interface XmlStreamer
    {
        void onStartTag(XmlNodeStartTag xmlNodeStartTag);

        void onEndTag(XmlNodeEndTag xmlNodeEndTag);

        void onCData(XmlCData xmlCData);

        void onNamespaceStart(XmlNamespaceStartTag tag);

        void onNamespaceEnd(XmlNamespaceEndTag tag);
    }
}
