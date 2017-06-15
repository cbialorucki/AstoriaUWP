using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    public static class ContainerHelpers
    {
        public static int binarySearch(int[] array, int size, int value)
        {
            int lo = 0;
            int hi = size - 1;

            while (lo <= hi)
            {
                //int mid = (lo + hi) >>> 1;
                int mid = unchecked((lo + hi) >> 1);
                int midVal = array[mid];

                if (midVal < value)
                {
                    lo = mid + 1;
                }
                else if (midVal > value)
                {
                    hi = mid - 1;
                }
                else
                {
                    return mid;  // value found
                }
            }
            return ~lo;  // value not present
        }

        public static int binarySearch(long[] array, int size, long value)
        {
            int lo = 0;
            int hi = size - 1;

            while (lo <= hi)
            {
                //int mid = (lo + hi) >>> 1;
                int mid = unchecked((lo + hi) >> 1);
                long midVal = array[mid];

                if (midVal < value)
                {
                    lo = mid + 1;
                }
                else if (midVal > value)
                {
                    hi = mid - 1;
                }
                else
                {
                    return mid;  // value found
                }
            }

            return ~lo;  // value not present
        }




    }
}
