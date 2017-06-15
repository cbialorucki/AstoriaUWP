using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class ResourceMapEntry : ResourceEntry
    {
        // Resource identifier of the parent mapping, or 0 if there is none.
        //ResTable_ref specifies the parent Resource, if any, of this Resource.
        // struct ResTable_ref { uint32_t ident; };
        private long parent;

        // Number of name/value pairs that follow for FLAG_COMPLEX. uint32_t
        private long count;

        private ResourceTableMap[] resourceTableMaps;

        public ResourceMapEntry(ResourceEntry resourceEntry)
        {
            this.setSize(resourceEntry.getSize());
            this.setFlags(resourceEntry.getFlags());
            this.setKey(resourceEntry.getKey());
        }

        public long getParent()
        {
            return parent;
        }

        public void setParent(long parent)
        {
            this.parent = parent;
        }

        public long getCount()
        {
            return count;
        }

        public void setCount(long count)
        {
            this.count = count;
        }

        public ResourceTableMap[] getResourceTableMaps()
        {
            return resourceTableMaps;
        }

        public void setResourceTableMaps(ResourceTableMap[] resourceTableMaps)
        {
            this.resourceTableMaps = resourceTableMaps;
        }

        /**
         * get value as string
         *
         * @return
         */
        public new string toStringValue(ResourceTable resourceTable, CultureInfo locale)
        {
            if (resourceTableMaps.Length > 0)
            {
                return resourceTableMaps[0].ToString();
            }
            else {
                return null;
            }
        }

        public new string toString()
        {
            return "ResourceMapEntry{" +
                    "parent=" + parent +
                    ", count=" + count +
                    ", resourceTableMaps=" + resourceTableMaps.ToString() +
                    '}';
            //Arrays.toString(resourceTableMaps)
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
