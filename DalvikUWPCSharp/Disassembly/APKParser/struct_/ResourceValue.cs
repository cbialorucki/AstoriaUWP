using DalvikUWPCSharp.Disassembly.APKParser.struct_.resource;
using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_
{
    public abstract class ResourceValue
    {
        protected readonly int value;

        protected ResourceValue(int val)
        {
            this.value = val;
        }

        /**
         * get value as string
         * Will be overridden by base classes.
         */
        public abstract string toStringValue(ResourceTable resourceTable, CultureInfo locale);

        public static ResourceValue decimal_(int value)
        {
            return new DecimalResourceValue(value);
        }

        public static ResourceValue hexadecimal(int value)
        {
            return new HexadecimalResourceValue(value);
        }

        public static ResourceValue bool_(int value)
        {
            return new BooleanResourceValue(value);
        }

        public static ResourceValue string_(int value, StringPool stringPool)
        {
            return new StringResourceValue(value, stringPool);
        }

        public static ResourceValue reference(int value)
        {
            return new ReferenceResourceValue(value);
        }

        public static ResourceValue nullValue()
        {
            return NullResourceValue.instance;
        }

        public static ResourceValue rgb(int value, int len)
        {
            return new RGBResourceValue(value, len);
        }

        public static ResourceValue dimension(int value)
        {
            return new DimensionValue(value);
        }

        public static ResourceValue fraction(int value)
        {
            return new FractionValue(value);
        }

        public static ResourceValue raw(int value, short type)
        {
            return new RawValue(value, type);
        }

        private class DecimalResourceValue : ResourceValue
        {
            public DecimalResourceValue(int val) : base(val) { }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                return value.ToString(); //string.valueOf(value); <-Hopefully it's the same :/
            }
        }

        private class HexadecimalResourceValue : ResourceValue
        {
            public HexadecimalResourceValue(int value) : base(value) { }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                return "0x" + value.ToString("X"); //Integer.toHexString(value);
            }
        }

        private class BooleanResourceValue : ResourceValue
        {

            public BooleanResourceValue(int value) : base(value) { }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                return (value == 1).ToString().ToLower(); //String.valueOf(value == 1);
            }
        }

        private class StringResourceValue : ResourceValue
        {
            private readonly StringPool stringPool;

            public StringResourceValue(int value, StringPool stringPool) : base(value)
            {
                //super(value);
                this.stringPool = stringPool;
            }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                if (value >= 0)
                {
                    return stringPool.get(value);
                }
                else
                {
                    return null;
                }
            }
        }

        // make public for cyclic reference detect
        public class ReferenceResourceValue : ResourceValue
        {
            public ReferenceResourceValue(int value) : base(value) { }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                long resourceId = getReferenceResourceId();
                return ParseUtils.getResourceById(resourceId, resourceTable, locale).Result;  //May cause program to hang :/
            }

            public long getReferenceResourceId()
            {
                return value & 0xFFFFFFFFL;
            }
        }

        private class NullResourceValue : ResourceValue
        {
            public static readonly NullResourceValue instance = new NullResourceValue();

            public NullResourceValue() : base(-1) { }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                return "";
            }
        }

        private class RGBResourceValue : ResourceValue
        {
            private readonly int len;

            public RGBResourceValue(int value, int len) : base(value)
            {
                this.len = len;
            }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = len / 2 - 1; i >= 0; i--)
                {
                    sb.Append(((value >> i * 8) & 0xff).ToString("X"));
                }
                return sb.ToString();
            }
        }

        private class DimensionValue : ResourceValue
        {
            public DimensionValue(int value) : base(value) { }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                short unit = (short)(value & 0xff);
                string unitStr;
                switch (unit)
                {
                    case ResValue.ResDataCOMPLEX.UNIT_MM:
                        unitStr = "mm";
                        break;
                    case ResValue.ResDataCOMPLEX.UNIT_PX:
                        unitStr = "px";
                        break;
                    case ResValue.ResDataCOMPLEX.UNIT_DIP:
                        unitStr = "dp";
                        break;
                    case ResValue.ResDataCOMPLEX.UNIT_SP:
                        unitStr = "sp";
                        break;
                    case ResValue.ResDataCOMPLEX.UNIT_PT:
                        unitStr = "pt";
                        break;
                    case ResValue.ResDataCOMPLEX.UNIT_IN:
                        unitStr = "in";
                        break;
                    default:
                        unitStr = "unknown unit:0x" + unit.ToString("X");
                        break;
                }
                return (value >> 8) + unitStr;
            }
        }

        private class FractionValue : ResourceValue
        {
            public FractionValue(int value) : base(value){ }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                // The low-order 4 bits of the data value specify the type of the fraction
                short type = (short)(value & 0xf);
                string pstr;
                switch (type)
                {
                    case ResValue.ResDataCOMPLEX.UNIT_FRACTION:
                        pstr = "%";
                        break;
                    case ResValue.ResDataCOMPLEX.UNIT_FRACTION_PARENT:
                        pstr = "%p";
                        break;
                    default:
                        pstr = "unknown type:0x" + type.ToString("X");
                        break;
                }

                float f = BitConverter.ToSingle(BitConverter.GetBytes(value >> 4), 0); //Float.intBitsToFloat(value >> 4);
                return f + pstr;
            }
        }

        private class RawValue : ResourceValue
        {
            private readonly short dataType;

            public RawValue(int value, short dataType) : base(value)
            {
                this.dataType = dataType;
            }

            public override string toStringValue(ResourceTable resourceTable, CultureInfo locale)
            {
                return "{" + dataType + ":" + (value & 0xFFFFFFFFL) + "}";
            }
        }


    }
}
