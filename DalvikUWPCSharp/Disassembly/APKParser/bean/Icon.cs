using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class Icon
    {
        private string path;
        private byte[] data;

        public Icon(string path, byte[] data)
        {
            this.path = path;
            this.data = data;
        }

        public string getPath()
        {
            return path;
        }

        public byte[] getData()
        {
            return data;
        }

        public string toString()
        {
            return "Icon{path='" + path + '\'' + ", size=" + (data == null ? 0 : data.Length) + '}';
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
