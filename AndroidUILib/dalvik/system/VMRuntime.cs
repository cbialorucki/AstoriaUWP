using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.dalvik.system
{
    public class VMRuntime
    {
        private static readonly VMRuntime THE_ONE = new VMRuntime();

        private VMRuntime() { }

        public static VMRuntime getRuntime()
        {
            return THE_ONE;
        }

        public string[] properties()
        {
            // RoboVM note: This is native Android.
            // TODO: Support command-line supplied properties?
            return Array.Empty<string>(); //EmptyArray.STRING;
        }




    }
}
