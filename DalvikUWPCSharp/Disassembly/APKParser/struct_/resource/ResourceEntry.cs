using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class ResourceEntry
    {
        // Number of bytes in this structure. uint16_t
        private int size;

        // If set, this is a complex entry, holding a set of name/value
        // mappings.  It is followed by an array of ResTable_map structures.
        public static readonly int FLAG_COMPLEX = 0x0001;
        // If set, this resource has been declared public, so libraries
        // are allowed to reference it.
        public static readonly int FLAG_PUBLIC = 0x0002;
        // uint16_t
        private int flags;

        // Reference into ResTable_package::keyStrings identifying this entry.
        //public long keyRef;

        private string key;

        // the resvalue following this resource entry.
        private ResourceValue value;

        /**
         * get value as string
         *
         * @return
         */
        public string toStringValue(ResourceTable resourceTable, CultureInfo locale)
        {
            if (value != null)
            {
                return value.toStringValue(resourceTable, locale);
            }
            else
            {
                return "null";
            }
        }

        public int getSize()
        {
            return size;
        }

        public void setSize(int size)
        {
            this.size = size;
        }

        public int getFlags()
        {
            return flags;
        }

        public void setFlags(int flags)
        {
            this.flags = flags;
        }

        public string getKey()
        {
            return key;
        }

        public void setKey(string key)
        {
            this.key = key;
        }

        public ResourceValue getValue()
        {
            return value;
        }

        public void setValue(ResourceValue value)
        {
            this.value = value;
        }

        public string toString()
        {
            return "ResourceEntry{" +
                    "size=" + size +
                    ", flags=" + flags +
                    ", key='" + key + '\'' +
                    ", value=" + value +
                    '}';
        }
    }
}
