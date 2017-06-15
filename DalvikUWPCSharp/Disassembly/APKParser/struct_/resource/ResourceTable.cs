using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class ResourceTable
    {
        private Dictionary<short, ResourcePackage> packageMap = new Dictionary<short, ResourcePackage>();
        private StringPool stringPool;

        public static Dictionary<int, string> sysStyle = ResourceLoader.loadSystemStyles().Result; //may cause program to freeze

        public void addPackage(ResourcePackage resourcePackage)
        {
            this.packageMap[resourcePackage.getId()] = resourcePackage; //.put(resourcePackage.getId(), resourcePackage);
        }

        public ResourcePackage getPackage(short id)
        {
            return this.packageMap[id]; //.get(id);
        }

        public StringPool getStringPool()
        {
            return stringPool;
        }

        public void setStringPool(StringPool stringPool)
        {
            this.stringPool = stringPool;
        }
    }
}
