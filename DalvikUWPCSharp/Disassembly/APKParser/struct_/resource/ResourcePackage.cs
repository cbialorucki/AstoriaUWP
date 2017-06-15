using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class ResourcePackage
    {
        // the packageName
        private string name;
        private short id;
        // contains the names of the types of the Resources defined in the ResourcePackage
        private StringPool typeStringPool;
        //  contains the names (keys) of the Resources defined in the ResourcePackage.
        private StringPool keyStringPool;

        public ResourcePackage(PackageHeader header)
        {
            this.name = header.getName();
            this.id = (short)header.getId();
        }

        private Dictionary<short, TypeSpec> typeSpecMap = new Dictionary<short, TypeSpec>();

        private Dictionary<short, List<RType>> typesMap = new Dictionary<short, List<RType>>();

        public void addTypeSpec(TypeSpec typeSpec)
        {
            this.typeSpecMap[typeSpec.getId()] = typeSpec; //put(typeSpec.getId(), typeSpec);
        }

        public TypeSpec getTypeSpec(short id)
        {
            return this.typeSpecMap[id]; //.get(id);
        }

        public void addType(RType type)
        {
            List<RType> types = this.typesMap[type.getId()]; //.get(type.getId());
            if (types == null)
            {
                types = new List<RType>();
                this.typesMap[type.getId()] = types; //.put(type.getId(), types);
            }

            types.Add(type);
        }

        public List<RType> getTypes(short id)
        {
            return this.typesMap[id]; //(id);
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

        public StringPool getTypeStringPool()
        {
            return typeStringPool;
        }

        public void setTypeStringPool(StringPool typeStringPool)
        {
            this.typeStringPool = typeStringPool;
        }

        public StringPool getKeyStringPool()
        {
            return keyStringPool;
        }

        public void setKeyStringPool(StringPool keyStringPool)
        {
            this.keyStringPool = keyStringPool;
        }

        public Dictionary<short, TypeSpec> getTypeSpecMap()
        {
            return typeSpecMap;
        }

        public void setTypeSpecMap(Dictionary<short, TypeSpec> typeSpecMap)
        {
            this.typeSpecMap = typeSpecMap;
        }

        public Dictionary<short, List<RType>> getTypesMap()
        {
            return typesMap;
        }

        public void setTypesMap(Dictionary<short, List<RType>> typesMap)
        {
            this.typesMap = typesMap;
        }
    }
}
