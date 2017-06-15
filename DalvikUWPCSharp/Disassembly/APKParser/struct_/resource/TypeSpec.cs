using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class TypeSpec
    {
        private long[] entryFlags;
        private string name;
        private short id;

        public TypeSpec(TypeSpecHeader header)
        {
            this.id = header.getId();
        }

        public bool exists(int id)
        {
            return id < entryFlags.Length;
        }

        public long[] getEntryFlags()
        {
            return entryFlags;
        }

        public void setEntryFlags(long[] entryFlags)
        {
            this.entryFlags = entryFlags;
        }

        public string getName()
        {
            return name;
        }

        public void setName(String name)
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

        public string toString()
        {
            return "TypeSpec{" +
                    "name='" + name + '\'' +
                    ", id=" + id +
                    '}';
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
