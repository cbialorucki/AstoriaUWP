using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class LibraryEntry
    {
        // uint32. The package-id this shared library was assigned at build time.
        private int packageId;

        //The package name of the shared library. \0 terminated. max 128
        private string name;

        public LibraryEntry(int packageId, string name)
        {
            this.packageId = packageId;
            this.name = name;
        }
    }
}
