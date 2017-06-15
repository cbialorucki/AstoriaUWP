using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils.xml
{
    public class Attributes
    {
        private Attribute_[] attributes;

        public Attributes(int size)
        {
            this.attributes = new Attribute_[size];
        }

        public void set(int i, Attribute_ attribute)
        {
            attributes[i] = attribute;
        }

        public string get(string name)
        {
            foreach (Attribute_ attribute in attributes)
            {
                if (attribute.getName().Equals(name))
                {
                    return attribute.getValue();
                }
            }
            return null;
        }

        public int size()
        {
            return attributes.Length;
        }

        public bool getBoolean(string name, bool b)
        {
            string value = get(name);
            return value == null ? b : bool.Parse(value);
        }

        public int getInt(string name)
        {
            string value = get(name);
            if (value == null)
            {
                return int.MinValue;
            }
            if (value.StartsWith("0x"))
            {
                //return int.Parse(value.Substring(2), 16);
                //Number is in hex
                return int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }

            return int.Parse(value);
        }

        public long getLong(string name)
        {
            string value = get(name);
            if (value == null)
            {
                return long.MinValue;
            }
            if (value.StartsWith("0x"))
            {
                //return long.valueOf(value.substring(2), 16);
                //Number is also in hex.
                return long.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }
            return long.Parse(value);
        }

        public Attribute_[] value()
        {
            return this.attributes;
        }
    }
}
