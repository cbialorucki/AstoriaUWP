using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    public class TypedValue
    {

        public const int TYPE_NULL = 0x00;
        public const int TYPE_REFERENCE = 0x01;
        public const int TYPE_ATTRIBUTE = 0x02;
        public const int TYPE_STRING = 0x03;
        public const int TYPE_FLOAT = 0x04;
        public const int TYPE_DIMENSION = 0x05;
        public const int TYPE_FRACTION = 0x06;
        public const int TYPE_FIRST_INT = 0x10;
        public const int TYPE_INT_DEC = 0x10;
        public const int TYPE_INT_HEX = 0x11;
        public const int TYPE_INT_BOOLEAN = 0x12;
        public const int TYPE_FIRST_COLOR_INT = 0x1c;
        public const int TYPE_INT_COLOR_ARGB8 = 0x1c;
        public const int TYPE_INT_COLOR_RGB8 = 0x1d;
        public const int TYPE_INT_COLOR_ARGB4 = 0x1e;
        public const int TYPE_INT_COLOR_RGB4 = 0x1f;
        public const int TYPE_LAST_COLOR_INT = 0x1f;
        public const int TYPE_LAST_INT = 0x1f;
        public const int COMPLEX_UNIT_SHIFT = 0;
        public const int COMPLEX_UNIT_MASK = 0xf;
        public const int COMPLEX_UNIT_PX = 0;
        public const int COMPLEX_UNIT_DIP = 1;
        public const int COMPLEX_UNIT_SP = 2;
        public const int COMPLEX_UNIT_PT = 3;
        public const int COMPLEX_UNIT_IN = 4;
        public const int COMPLEX_UNIT_MM = 5;
        public const int COMPLEX_UNIT_FRACTION = 0;
        public const int COMPLEX_UNIT_FRACTION_PARENT = 1;
        public const int COMPLEX_RADIX_SHIFT = 4;
        public const int COMPLEX_RADIX_MASK = 0x3;
        public const int COMPLEX_RADIX_23p0 = 0;
        public const int COMPLEX_RADIX_16p7 = 1;
        public const int COMPLEX_RADIX_8p15 = 2;
        public const int COMPLEX_RADIX_0p23 = 3;
        public const int COMPLEX_MANTISSA_SHIFT = 8;
        public const int COMPLEX_MANTISSA_MASK = 0xffffff;
        public const int DATA_NULL_UNDEFINED = 0;
        public const int DATA_NULL_EMPTY = 1;
        public const int DENSITY_DEFAULT = 0;
        public const int DENSITY_NONE = 0xffff;
        public int type;
        public string string1;
        public int data;
        public int assetCookie;
        public int resourceId;
        public int changingConfigurations = -1;
        public int density;

        public float getFloat()
        {
            return ticomware.interop.Util.intBitsToFloat(data);
        }

        private const float MANTISSA_MULT = 1.0f / (1 << TypedValue.COMPLEX_MANTISSA_SHIFT);
        private static float[] RADIX_MULTS = new float[] { 1.0f*MANTISSA_MULT, 1.0f/(1<<7)*MANTISSA_MULT, 1.0f/(1<<15)*MANTISSA_MULT, 1.0f/(1<<23)*MANTISSA_MULT};


        public static float complexToFloat(int complex)
        {
            return (complex & (TypedValue.COMPLEX_MANTISSA_MASK << TypedValue.COMPLEX_MANTISSA_SHIFT)) * RADIX_MULTS[(complex >> TypedValue.COMPLEX_RADIX_SHIFT) & TypedValue.COMPLEX_RADIX_MASK];
        }


        public static float complexToDimension(int data, DisplayMetrics metrics)
        {
            return applyDimension((data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK, complexToFloat(data), metrics);
        }


        public static int complexToDimensionPixelOffset(int data, DisplayMetrics metrics)
        {
            return (int)applyDimension((data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK, complexToFloat(data), metrics);
        }


        public static int complexToDimensionPixelSize(int data, DisplayMetrics metrics)
        {
            float value = complexToFloat(data);
            float f = applyDimension((data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK, value, metrics);
            int res = (int)(f + 0.5f);
            if (res != 0) return res;
            if (value == 0) return 0;
            if (value > 0) return 1;
            return -1;
        }


        public static float complexToDimensionNoisy(int data, DisplayMetrics metrics)
        {
            return complexToDimension(data, metrics);
        }


        public int getComplexUnit()
        {
            return COMPLEX_UNIT_MASK & (data >> TypedValue.COMPLEX_UNIT_SHIFT);
        }


        public static float applyDimension(int unit, float value, DisplayMetrics metrics)
        {
            switch (unit)
            {
                case COMPLEX_UNIT_PX:
                    return value;
                case COMPLEX_UNIT_DIP:
                    return value * metrics.density;
                case COMPLEX_UNIT_SP:
                    return value * metrics.scaledDensity;
                case COMPLEX_UNIT_PT:
                    return value * metrics.xdpi * (1.0f / 72);
                case COMPLEX_UNIT_IN:
                    return value * metrics.xdpi;
                case COMPLEX_UNIT_MM:
                    return value * metrics.xdpi * (1.0f / 25.4f);
            }

            return 0;
        }


        public float getDimension(DisplayMetrics metrics)
        {
            return complexToDimension(data, metrics);
        }


        public static float complexToFraction(int data, float bse, float pbase)
        {
            switch ((data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK)
            {
                case COMPLEX_UNIT_FRACTION:
                    return complexToFloat(data) * bse;
                case COMPLEX_UNIT_FRACTION_PARENT:
                    return complexToFloat(data) * pbase;
            }

            return 0;
        }


        public float getFraction(float bse, float pbase)
        {
            return complexToFraction(data, bse, pbase);
        }


        public string coerceToString()
        {
            int t = type;
            if (t == TYPE_STRING)
            {
                return string1;
            }
            return coerceToString(t, data);
        }

        private static string[] DIMENSION_UNIT_STRS = new string[] {"px", "dip", "sp", "pt", "in", "mm"};
        private static string[] FRACTION_UNIT_STRS = new string[] {"%", "%p"};

    
        public static string coerceToString(int type, int data)
        {
            switch (type)
            {
                case TYPE_NULL:
                    return null;
                case TYPE_REFERENCE:
                    return "@" + data;
                case TYPE_ATTRIBUTE:
                    return "?" + data;
                case TYPE_FLOAT:
                    return ticomware.interop.Util.intBitsToFloat(data).ToString();
                case TYPE_DIMENSION:
                    return (complexToFloat(data) + DIMENSION_UNIT_STRS[(data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK]).ToString();
                case TYPE_FRACTION:
                    return ((complexToFloat(data) * 100) + FRACTION_UNIT_STRS[(data >> COMPLEX_UNIT_SHIFT) & COMPLEX_UNIT_MASK]).ToString();
                case TYPE_INT_HEX:
                    return "0x" + data.ToString("X");
                case TYPE_INT_BOOLEAN:
                    return data != 0 ? "true" : "false";
            }

            if (type >= TYPE_FIRST_COLOR_INT && type <= TYPE_LAST_COLOR_INT)
            {
                return "#" + data.ToString("X");
            }
            else if (type >= TYPE_FIRST_INT && type <= TYPE_LAST_INT)
            {
                return data.ToString("X");
            }

            return null;
        }

        public void setTo(TypedValue other)
        {
            type = other.type;
            string1 = other.string1;
            data = other.data;
            assetCookie = other.assetCookie;
            resourceId = other.resourceId;
            density = other.density;
        }

        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TypedValue{t=0x").Append(type.ToString("X"));
            sb.Append("/d=0x").Append(data.ToString("X"));
            if (type == TYPE_STRING)
            {
                sb.Append(" \"").Append(string1 != null ? string1 : "<null>").Append("\"");
            }
            if (assetCookie != 0)
            {
                sb.Append(" a=").Append(assetCookie);
            }
            if (resourceId != 0)
            {
                sb.Append(" r=0x").Append(resourceId.ToString("X"));
            }
            sb.Append("}");

            return sb.ToString();
        }

        public override string ToString()
        {
            return toString();
        }
    }
}
