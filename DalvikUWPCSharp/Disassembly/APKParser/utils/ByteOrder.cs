using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils
{
    //Ported from Java Source
    public class ByteOrder
    {
        private string name;

        private ByteOrder(string name)
        {
            this.name = name;
        }

        /**
         * Constant denoting big-endian byte order.  In this order, the bytes of a
         * multibyte value are ordered from most significant to least significant.
         * </p>
         */
        //public static ByteOrder BIG_ENDIAN = new ByteOrder("BIG_ENDIAN");

        /**
         * Constant denoting little-endian byte order.  In this order, the bytes of
         * a multibyte value are ordered from least significant to most
         * significant.  </p>
         */
        public static ByteOrder LITTLE_ENDIAN = new ByteOrder("LITTLE_ENDIAN");

        /**
         * Retrieves the native byte order of the underlying platform.
         *
         * <p> This method is defined so that performance-sensitive Java code can
         * allocate direct buffers with the same byte order as the hardware.
         * Native code libraries are often more efficient when such buffers are
         * used.  </p>
         *
         * @return  The native byte order of the hardware upon which this Java
         *          virtual machine is running
         */
        //public static ByteOrder nativeOrder()
        //{
            //return Bits.byteOrder();
        //}

        /**
         * Constructs a string describing this object.
         *
         * <p> This method returns the string <tt>"BIG_ENDIAN"</tt> for {@link
         * #BIG_ENDIAN} and <tt>"LITTLE_ENDIAN"</tt> for {@link #LITTLE_ENDIAN}.
         * </p>
         *
         * @return  The specified string
         */
        public string toString()
        {
            return name;
        }

        public override string ToString()
        {
            return toString();
        }

        /*public override bool Equals(object obj)
        {
            return obj.ToString().Equals(name);
        }*/
    }
}
