using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class PackageHeader : ChunkHeader
    {
        // ResourcePackage IDs start at 1 (corresponding to the value of the package bits in a resource identifier).
        // 0 means this is not a base package.
        // uint32_t
        // 0 framework-res.apk
        // 2-9 other framework files
        // 127 application package

        // Anroid 5.0+: Shared libraries will be assigned a package ID of 0x00 at build-time.
        // At runtime, all loaded shared libraries will be assigned a new package ID.
        private long id;

        // Actual name of this package, -terminated.
        // char16_t name[128]
        private string name;

        // Offset to a ResStringPool_header defining the resource type symbol table.
        //  If zero, this package is inheriting from another base package (overriding specific values in it).
        // uinit 32
        private long typeStrings;


        // Last index into typeStrings that is for public use by others.
        // uint32_t
        private long lastPublicType;

        // Offset to a ResStringPool_header defining the resource
        // key symbol table.  If zero, this package is inheriting from
        // another base package (overriding specific values in it).
        // uint32_t
        private long keyStrings;

        // Last index into keyStrings that is for public use by others.
        // uint32_t
        private long lastPublicKey;

        public PackageHeader(int chunkType, int headerSize, long chunkSize) : base(chunkType, headerSize, chunkSize) { }

        public long getId()
        {
            return id;
        }

        public void setId(long id)
        {
            //        if (id == 0) {
            //            id = 2;
            //        }
            this.id = id;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public long getTypeStrings()
        {
            return typeStrings;
        }

        public void setTypeStrings(long typeStrings)
        {
            this.typeStrings = typeStrings;
        }

        public long getLastPublicType()
        {
            return lastPublicType;
        }

        public void setLastPublicType(long lastPublicType)
        {
            this.lastPublicType = lastPublicType;
        }

        public long getKeyStrings()
        {
            return keyStrings;
        }

        public void setKeyStrings(long keyStrings)
        {
            this.keyStrings = keyStrings;
        }

        public long getLastPublicKey()
        {
            return lastPublicKey;
        }

        public void setLastPublicKey(long lastPublicKey)
        {
            this.lastPublicKey = lastPublicKey;
        }
    }
}
