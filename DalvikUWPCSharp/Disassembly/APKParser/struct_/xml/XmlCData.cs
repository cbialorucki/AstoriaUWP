using DalvikUWPCSharp.Disassembly.APKParser.struct_.resource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.xml
{
    public class XmlCData
    {
        public const string CDATA_START = "<![CDATA[";
        public const string CDATA_END = "]]>";

        // The raw CDATA character data.
        private string data;

        // The typed value of the character data if this is a CDATA node.
        private ResourceValue typedData;

        // the final value as string
        private string value;

        /**
         * get value as string
         *
         * @return
         */
        public string toStringValue(ResourceTable resourceTable, CultureInfo locale)
        {
            if (data != null)
            {
                return CDATA_START + data + CDATA_END;
            }
            else
            {
                return CDATA_START + typedData.toStringValue(resourceTable, locale) + CDATA_END;
            }
        }

        public string getData()
        {
            return data;
        }

        public void setData(string data)
        {
            this.data = data;
        }

        public ResourceValue getTypedData()
        {
            return typedData;
        }

        public void setTypedData(ResourceValue typedData)
        {
            this.typedData = typedData;
        }

        public string getValue()
        {
            return value;
        }

        public void setValue(string value)
        {
            this.value = value;
        }

        public string toString()
        {
            return "XmlCData{" +
                    "data='" + data + '\'' +
                    ", typedData=" + typedData +
                    '}';
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
