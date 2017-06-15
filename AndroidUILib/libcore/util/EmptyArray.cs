using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.libcore.util
{
    public static class EmptyArray
    {
        public static readonly bool[] BOOLEAN = new bool[0];
        public static readonly byte[] BYTE = new byte[0];
        public static readonly char[] CHAR = new char[0];
        public static readonly double[] DOUBLE = new double[0];
        public static readonly float[] FLOAT = new float[0];
        public static readonly int[] INT = new int[0];
        public static readonly long[] LONG = new long[0];

        //public static final Class<?>[] CLASS = new Class[0];
        public static readonly object[] OBJECT = new object[0];
        public static readonly string[] STRING = new string[0];
        //public static Throwable[] THROWABLE = new Throwable[0];
        //public static final StackTraceElement[] STACK_TRACE_ELEMENT = new StackTraceElement[0];
        //public static final java.lang.reflect.Type[] TYPE = new java.lang.reflect.Type[0];
        //public static final java.lang.reflect.TypeVariable[] TYPE_VARIABLE = new java.lang.reflect.TypeVariable[0];

    }
}
