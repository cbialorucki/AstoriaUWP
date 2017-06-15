using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_
{
    public class StringPool
    {
        private string[] pool;

        public StringPool(int poolSize)
        {
            pool = new string[poolSize];
        }

        public string get(int idx)
        {
            return pool[idx];
        }

        public void set(int idx, string value)
        {
            if(!pool.Contains(value))
            {
                pool[idx] = value;
            }
            else
            {
                //pool[idx] = value.re
            }
        }
    }
}
