using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class RType
    {
        private string name;
        private short id;

        private CultureInfo locale;

        private StringPool keyStringPool;
        private ByteBuffer buffer;
        private long[] offsets;
        private StringPool stringPool;

        public RType(TypeHeader header)
        {
            this.id = header.getId();
            locale = new CultureInfo(header.getConfig().getLanguage() + "-" + header.getConfig().getCountry());
            //this.locale = new Locale(header.getConfig().getLanguage(), header.getConfig().getCountry());
        }

        public async Task<ResourceEntry> getResourceEntry(int id)
        {
            if (id >= offsets.Length)
            {
                return null;
            }

            if (offsets[id] == TypeHeader.NO_ENTRY)
            {
                return null;
            }

            // read Resource Entries
            buffer.position((int)offsets[id]);
            return await readResourceEntry();
        }

        private async Task<ResourceEntry> readResourceEntry()
        {
            long beginPos = buffer.position();
            ResourceEntry resourceEntry = new ResourceEntry();
            // size is always 8(simple), or 16(complex)
            resourceEntry.setSize(await Buffers.readUShort(buffer));
            resourceEntry.setFlags(await Buffers.readUShort(buffer));
            long keyRef = buffer.getInt();
            string key = keyStringPool.get((int)keyRef);
            resourceEntry.setKey(key);

            if ((resourceEntry.getFlags() & ResourceEntry.FLAG_COMPLEX) != 0)
            {
                ResourceMapEntry resourceMapEntry = new ResourceMapEntry(resourceEntry);

                // Resource identifier of the parent mapping, or 0 if there is none.
                resourceMapEntry.setParent(Buffers.readUInt(buffer));
                resourceMapEntry.setCount(Buffers.readUInt(buffer));

                buffer.position((int)(beginPos + resourceEntry.getSize()));

                //An individual complex Resource entry comprises an entry immediately followed by one or more fields.
                ResourceTableMap[] resourceTableMaps = new ResourceTableMap[(int)resourceMapEntry.getCount()];
                for (int i = 0; i < resourceMapEntry.getCount(); i++)
                {
                    resourceTableMaps[i] = await readResourceTableMap();
                }

                resourceMapEntry.setResourceTableMaps(resourceTableMaps);
                return resourceMapEntry;
            }
            else {
                buffer.position((int)(beginPos + resourceEntry.getSize()));
                resourceEntry.setValue(await ParseUtils.readResValue(buffer, stringPool));
                return resourceEntry;
            }
        }

        private async Task<ResourceTableMap> readResourceTableMap()
        {
            ResourceTableMap resourceTableMap = new ResourceTableMap();
            resourceTableMap.setNameRef(Buffers.readUInt(buffer));
            resourceTableMap.setResValue(await ParseUtils.readResValue(buffer, stringPool));

            if ((resourceTableMap.getNameRef() & 0x02000000) != 0)
            {
                //read arrays
            }
            else if ((resourceTableMap.getNameRef() & 0x01000000) != 0)
            {
                // read attrs
            }
            else
            {

            }

            return resourceTableMap;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public short getId()
        {
            return id;
        }

        public void setId(short id)
        {
            this.id = id;
        }

        public CultureInfo getLocale()
        {
            return locale;
        }

        public void setLocale(CultureInfo locale)
        {
            this.locale = locale;
        }

        public StringPool getKeyStringPool()
        {
            return keyStringPool;
        }

        public void setKeyStringPool(StringPool keyStringPool)
        {
            this.keyStringPool = keyStringPool;
        }

        public ByteBuffer getBuffer()
        {
            return buffer;
        }

        public void setBuffer(ByteBuffer buffer)
        {
            this.buffer = buffer;
        }

        public long[] getOffsets()
        {
            return offsets;
        }

        public void setOffsets(long[] offsets)
        {
            this.offsets = offsets;
        }

        public StringPool getStringPool()
        {
            return stringPool;
        }

        public void setStringPool(StringPool stringPool)
        {
            this.stringPool = stringPool;
        }

        public string toString()
        {
            return "Type{" +
                    "name='" + name + '\'' +
                    ", id=" + id +
                    ", locale=" + locale +
                    '}';
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
