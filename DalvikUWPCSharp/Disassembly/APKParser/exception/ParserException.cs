using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.exception
{
    public class ParserException : Exception
    {
        public ParserException(string ex) : base(ex) { }
    }
}
