using DalvikUWPCSharp.Disassembly.APKParser.struct_;
using DalvikUWPCSharp.Disassembly.APKParser.struct_.resource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils.xml
{
    public class Attribute_
    {
        private string nspace;
        private string name;
        // The original raw string value of this 
        private string rawValue;
        // Processed typed value of this
        private ResourceValue typedValue;
        // the final value as string
        private string value;

        public string toStringValue(ResourceTable resourceTable, CultureInfo locale)
        {
            if (rawValue != null)
            {
                return rawValue;
            }
            else if (typedValue != null)
            {
                return typedValue.toStringValue(resourceTable, locale);
            }
            else {
                // something happen;
                return "";
            }
        }

        /**
         * These are attribute resource constants for the platform; as found in android.R.attr
         *
         * @author dongliu
         */
        public static class AttrIds
        {

            private static Dictionary<int, string> ids;

            public static async Task<string> getString(long id)
            {
                if(ids == null)
                {
                    ids = await ResourceLoader.loadSystemAttrIds();
                }

                string value = ids[(int)id]; //.get((int)id);
                if (value == null)
                {
                    value = "AttrId:0x" + id.ToString("X");
                }
                return value;
            }

        }

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

        public string getRawValue()
        {
            return rawValue;
        }

        public void setRawValue(string rawValue)
        {
            this.rawValue = rawValue;
        }

        public ResourceValue getTypedValue()
        {
            return typedValue;
        }

        public void setTypedValue(ResourceValue typedValue)
        {
            this.typedValue = typedValue;
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
            return "Attribute{" +
                    "name='" + name + '\'' +
                    ", nspace='" + nspace + '\'' +
                    '}';
        }
    }
}
