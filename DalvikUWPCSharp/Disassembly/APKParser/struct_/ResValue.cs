using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_
{
    public class ResValue
    {
        // Number of bytes in this structure. uint16; always 8
        private int size;
        // Always set to 0. uint8
        private short res0;
        // Type of the data value. uint8
        private short dataType;
        // The data for this item; as interpreted according to dataType. unit32

        /*
         * The data field is a fixed size 32-bit integer.
         * How it is interpreted depends upon the value of the type field.
         * Some of the possible interpretations are as
         *     a boolean value
         *     a float value
         *     an integer value
         *     an index into the Table chunk’s StringPool
         *     a composite value
         */
        /**
         * the real data represented by string
         */
        private ResourceValue data;

        public string toString()
        {
            return "ResValue{" +
                    "size=" + size +
                    ", res0=" + res0 +
                    ", dataType=" + dataType +
                    ", data=" + data +
                    '}';
        }

        public static class ResType
        {
            // Contains no data.
            public const short NULL = 0x00;
            // The 'data' holds a ResTable_ref; a reference to another resource
            // table entry.
            public const short REFERENCE = 0x01;
            // The 'data' holds an attribute resource identifier.
            public const short ATTRIBUTE = 0x02;
            // The 'data' holds an index into the containing resource table's
            // global value string pool.
            public const short STRING = 0x03;
            // The 'data' holds a single-precision floating point number.
            public const short FLOAT = 0x04;
            // The 'data' holds a complex number encoding a dimension value;
            // such as "100in".
            public const short DIMENSION = 0x05;
            // The 'data' holds a complex number encoding a fraction of a
            // container.
            public const short FRACTION = 0x06;

            // Beginning of integer flavors...
            public const short FIRST_INT = 0x10;

            // The 'data' is a raw integer value of the form n..n.
            public const short INT_DEC = 0x10;
            // The 'data' is a raw integer value of the form 0xn..n.
            public const short INT_HEX = 0x11;
            // The 'data' is either 0 or 1; for input "false" or "true" respectively.
            public const short INT_BOOLEAN = 0x12;

            // Beginning of color integer flavors...
            public const short FIRST_COLOR_INT = 0x1c;

            // The 'data' is a raw integer value of the form #aarrggbb.
            public const short INT_COLOR_ARGB8 = 0x1c;
            // The 'data' is a raw integer value of the form #rrggbb.
            public const short INT_COLOR_RGB8 = 0x1d;
            // The 'data' is a raw integer value of the form #argb.
            public const short INT_COLOR_ARGB4 = 0x1e;
            // The 'data' is a raw integer value of the form #rgb.
            public const short INT_COLOR_RGB4 = 0x1f;

            // ...end of integer flavors.
            public const short LAST_COLOR_INT = 0x1f;

            // ...end of integer flavors.
            public const short LAST_INT = 0x1f;
        }

        // A number of constants used when the data is interpreted as a composite value are defined
        // by the following anonymous C++ enum
        public static class ResDataCOMPLEX
        {
            // Where the unit type information is.  This gives us 16 possible
            // types; as defined below.
            public const short UNIT_SHIFT = 0;
            public const short UNIT_MASK = 0xf;

            // TYPE_DIMENSION: Value is raw pixels.
            public const short UNIT_PX = 0;
            // TYPE_DIMENSION: Value is Device Independent Pixels.
            public const short UNIT_DIP = 1;
            // TYPE_DIMENSION: Value is a Scaled device independent Pixels.
            public const short UNIT_SP = 2;
            // TYPE_DIMENSION: Value is in points.
            public const short UNIT_PT = 3;
            // TYPE_DIMENSION: Value is in inches.
            public const short UNIT_IN = 4;
            // TYPE_DIMENSION: Value is in millimeters.
            public const short UNIT_MM = 5;

            // TYPE_FRACTION: A basic fraction of the overall size.
            public const short UNIT_FRACTION = 0;
            // TYPE_FRACTION: A fraction of the parent size.
            public const short UNIT_FRACTION_PARENT = 1;

            // Where the radix information is; telling where the decimal place
            // appears in the mantissa.  This give us 4 possible fixed point
            // representations as defined below.
            public const short RADIX_SHIFT = 4;
            public const short RADIX_MASK = 0x3;

            // The mantissa is an integral number -- i.e.; 0xnnnnnn.0
            public const short RADIX_23p0 = 0;
            // The mantissa magnitude is 16 bits -- i.e; 0xnnnn.nn
            public const short RADIX_16p7 = 1;
            // The mantissa magnitude is 8 bits -- i.e; 0xnn.nnnn
            public const short RADIX_8p15 = 2;
            // The mantissa magnitude is 0 bits -- i.e; 0x0.nnnnnn
            public const short RADIX_0p23 = 3;

            // Where the actual value is.  This gives us 23 bits of
            // precision.  The top bit is the sign.
            public const short MANTISSA_SHIFT = 8;
            public const int MANTISSA_MASK = 0xffffff;
        }
    }
}
