using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.java.lang
{
    public class CloneNotSupportedException : Exception
    {
        public CloneNotSupportedException() : base() { }

        public CloneNotSupportedException(string message) : base(message) { }
    }
}
