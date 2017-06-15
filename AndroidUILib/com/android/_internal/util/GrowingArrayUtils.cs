using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.com.android._internal.util
{
    public static class GrowingArrayUtils
    {
        public static object[] append(object[] array, int currentSize, object element)
        {
            Debug.Assert(currentSize <= array.Length);

            if (currentSize + 1 > array.Length)
            {
                //@SuppressWarnings("unchecked")
                object[] newArray = ArrayUtils.newUnpaddedArray(null, growSize(currentSize));
                System.Array.Copy(array, 0, newArray, 0, currentSize);
                array = newArray;
            }

            array[currentSize] = element;
            return array;
        }

        public static int[] append(int[] array, int currentSize, int element)
        {
            Debug.Assert(currentSize <= array.Length);

            if (currentSize + 1 > array.Length)
            {
                int[] newArray = ArrayUtils.newUnpaddedIntArray(growSize(currentSize));
                System.Array.Copy(array, 0, newArray, 0, currentSize);
                array = newArray;
            }

            array[currentSize] = element;
            return array;
        }

        public static long[] append(long[] array, int currentSize, long element)
        {
            //assert is used to check conditions in Java
            //assert currentSize <= array.Length;
            Debug.Assert(currentSize <= array.Length);

            if (currentSize + 1 > array.Length)
            {
                long[] newArray = ArrayUtils.newUnpaddedLongArray(growSize(currentSize));
                System.Array.Copy(array, 0, newArray, 0, currentSize);
                array = newArray;
            }
            array[currentSize] = element;

            return array;
        }

        public static bool[] append(bool[] array, int currentSize, bool element)
        {
            Debug.Assert(currentSize <= array.Length);

            if (currentSize + 1 > array.Length)
            {
                bool[] newArray = ArrayUtils.newUnpaddedBooleanArray(growSize(currentSize));
                System.Array.Copy(array, 0, newArray, 0, currentSize);
                array = newArray;
            }

            array[currentSize] = element;
            return array;
        }

        public static int[] insert(int[] array, int currentSize, int index, int element)
        {
            Debug.Assert(currentSize <= array.Length);

            if (currentSize + 1 <= array.Length)
            {
                System.Array.Copy(array, index, array, index + 1, currentSize - index);
                array[index] = element;
                return array;
            }

            int[] newArray = ArrayUtils.newUnpaddedIntArray(growSize(currentSize));
            System.Array.Copy(array, 0, newArray, 0, index);
            newArray[index] = element;
            System.Array.Copy(array, index, newArray, index + 1, array.Length - index);
            return newArray;
        }

        public static object[] insert(object[] array, int currentSize, int index, object element)
        {
            Debug.Assert(currentSize <= array.Length);

            if (currentSize + 1 <= array.Length)
            {
                System.Array.Copy(array, index, array, index + 1, currentSize - index);
                array[index] = element;
                return array;
            }

            //@SuppressWarnings("unchecked")
            //object[] newArray = ArrayUtils.newUnpaddedArray((Class<T>)array.getClass().getComponentType(), growSize(currentSize));
            object[] newArray = ArrayUtils.newUnpaddedArray(null, growSize(currentSize));
            System.Array.Copy(array, 0, newArray, 0, index);
            newArray[index] = element;
            System.Array.Copy(array, index, newArray, index + 1, array.Length - index);
            return newArray;
        }

        public static long[] insert(long[] array, int currentSize, int index, long element)
        {
            Debug.Assert(currentSize <= array.Length);

            if (currentSize + 1 <= array.Length)
            {
                System.Array.Copy(array, index, array, index + 1, currentSize - index);
                array[index] = element;
                return array;
            }

            long[] newArray = ArrayUtils.newUnpaddedLongArray(growSize(currentSize));
            System.Array.Copy(array, 0, newArray, 0, index);
            newArray[index] = element;
            System.Array.Copy(array, index, newArray, index + 1, array.Length - index);
            return newArray;
        }

        public static int growSize(int currentSize)
        {
            return currentSize <= 4 ? 8 : currentSize * 2;
        }

    }
}
