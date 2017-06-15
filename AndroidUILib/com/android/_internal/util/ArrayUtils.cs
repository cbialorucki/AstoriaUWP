using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.com.android._internal.util
{
    public class ArrayUtils
    {
        private const int CACHE_SIZE = 73;
        private static object[] sCache = new object[CACHE_SIZE];

        public static byte[] newUnpaddedByteArray(int minLen)
        {
            //return (byte[])VMRuntime.getRuntime().newUnpaddedArray(byte.class1, minLen);
            //this may not be big enough
            return new byte[minLen];
        }

        public static char[] newUnpaddedCharArray(int minLen)
        {
            return new char[minLen];
            //return (char[])VMRuntime.getRuntime().newUnpaddedArray(char.class, minLen);
        }

        public static int[] newUnpaddedIntArray(int minLen)
        {
            //return (int[])VMRuntime.getRuntime().newUnpaddedArray(int.class, minLen);
            return new int[minLen];
        }

        public static bool[] newUnpaddedBooleanArray(int minLen)
        {
            //return (boolean[])VMRuntime.getRuntime().newUnpaddedArray(boolean.class, minLen);
            return new bool[minLen];
        }

        public static long[] newUnpaddedLongArray(int minLen)
        {
            return new long[minLen];
            //return (long[])VMRuntime.getRuntime().newUnpaddedArray(long.class1, minLen);
        }

        public static float[] newUnpaddedFloatArray(int minLen)
        {
            //return (float[])VMRuntime.getRuntime().newUnpaddedArray(float.class, minLen);
            return new float[minLen];
        }

        public static object[] newUnpaddedObjectArray(int minLen)
        {
            //return (Object[])VMRuntime.getRuntime().newUnpaddedArray(Object.class, minLen);
            return new object[minLen];
        }

        public static object[] newUnpaddedArray(object clazz, int minLen)
        {
            return new object[minLen];
            //return (T[])VMRuntime.getRuntime().newUnpaddedArray(clazz, minLen);
        }
    }
}
