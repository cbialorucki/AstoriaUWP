// Copyright (c) 2012 Markus Jarderot
// Copyright (c) 2016 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using Windows.UI;

namespace AndroidXml.Res
{
#if !NETSTANDARD1_3
    //[Serializable]
#endif
    public class Res_value
    {
        /// <summary>
        /// Number of bytes in this structure. Always 8.
        /// </summary>
        public ushort Size { get; set; }

        /// <summary>
        /// Reserved. Always 0.
        /// </summary>
        public byte Res0 { get; set; }

        /// <summary>
        /// The type of the data.
        /// </summary>
        public ValueType DataType { get; set; }

        /// <summary>
        /// The raw value of the data.
        /// </summary>
        public uint RawData { get; set; }

        /// <summary>
        /// Gets or sets the data as a resource reference. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_REFERENCE"/>.
        /// </summary>
        /// <remarks>
        /// Assignments to fields of the <c>ResTable_ref</c> object will 
        /// not be detected. You have to reassign <c>ReferenceValue</c>
        /// upon change.
        /// </remarks>
        public ResTable_ref ReferenceValue
        {
            get
            {
                return new ResTable_ref
                {
                    Ident = RawData == 0xFFFFFFFFu ? (uint?) null : RawData
                };
            }
            set { RawData = value.Ident ?? 0xFFFFFFFFu; }
        }

        /// <summary>
        /// Gets or sets the data as a string reference. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_STRING"/>.
        /// </summary>
        /// <remarks>
        /// Assignments to fields of the <c>ResStringPool_ref</c> object will 
        /// not be detected. You have to reassign <c>StringValue</c>
        /// upon change.
        /// </remarks>
        public ResStringPool_ref StringValue
        {
            get
            {
                return new ResStringPool_ref
                {
                    Index = RawData == 0xFFFFFFFFu ? (uint?) null : RawData
                };
            }
            set { RawData = value.Index ?? 0xFFFFFFFFu; }
        }

        /// <summary>
        /// Gets or sets the data as a floating point value. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_FLOAT"/>.
        /// </summary>
        public float FloatValue
        {
            get { return BitConverter.ToSingle(BitConverter.GetBytes(RawData), 0); }
            set { RawData = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0); }
        }

        /// <summary>
        /// Gets or sets the data as a signed integer value. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_INT_DEC"/>  
        /// or <see cref="ValueType.TYPE_INT_HEX"/>.
        /// </summary>
        public int IntValue
        {
            get { return (int) RawData; }
            set { RawData = (uint) value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ColorValue"/> as a <see cref="System.Drawing.Color"/>.
        /// </summary>
        public Windows.UI.Color DrawingColorValue
        {
            get
            {
                var color = this.ColorValue;

                return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
            }

            set
            {
                this.ColorValue = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets or sets the data as a color value. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_INT_COLOR_ARGB4"/>, 
        /// <see cref="ValueType.TYPE_INT_COLOR_ARGB8"/>, <see cref="ValueType.TYPE_INT_COLOR_RGB4"/> or 
        /// <see cref="ValueType.TYPE_INT_COLOR_RGB8"/>.
        /// </summary>
        public Color ColorValue
        {
            get
            {
                byte[] bytes = BitConverter.GetBytes(RawData);
                /*return Color.FromArgb(
                    alpha: bytes[3],
                    red: bytes[2],
                    green: bytes[1],
                    blue: bytes[0]);*/
                return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            set { RawData = BitConverter.ToUInt32(new[] {value.B, value.G, value.R, value.A}, 0); }
        }

        /// <summary>
        /// Gets or sets the unit of the data as a dimension value. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_DIMENSION"/>.
        /// </summary>
        public DimensionUnit ComplexDimensionUnit
        {
            get { return (DimensionUnit) (RawData & 0xFu); }
            set { RawData = (RawData & ~0xFu) | ((uint) value & 0xFu); }
        }

        /// <summary>
        /// Gets or sets the unit of the data as a fraction value. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_FRACTION"/>.
        /// </summary>
        public FractionUnit ComplexFractionUnit
        {
            get { return (FractionUnit) (RawData & 0xFu); }
            set { RawData = (RawData & ~0xFu) | ((uint) value & 0xFu); }
        }

        /// <summary>
        /// Gets or sets the number of the data as a complex value. Used when <see cref="DataType"/> is <see cref="ValueType.TYPE_DIMENSION"/> 
        /// or <see cref="ValueType.TYPE_FRACTION"/>.
        /// </summary>
        public float ComplexValue
        {
            get
            {
                uint radix = (RawData & 0x30u) >> 4;
                int mantissa = ((int) RawData & ~0xFF) >> 8; // MSB -> sign
                switch (radix)
                {
                    case 0: // 23p0
                        return mantissa;
                    case 1: // 16p7
                        return mantissa/128f;
                    case 2: // 8p15
                        return mantissa/32768f;
                    case 3: // 0p23
                    default:
                        return mantissa/8388608f;
                }
            }
            set
            {
                float abs = value < 0 ? -value : value;
                int sign = value < 0 ? -1 : 1;
                uint radix;
                int mantissa;
                if (abs < 1f)
                {
                    radix = 3; // 0p23
                    mantissa = (int) (abs*8388608f + 0.5f);
                }
                else if (abs < 256f)
                {
                    radix = 2; // 8p15
                    mantissa = (int) (abs*32768f + 0.5f);
                }
                else if (abs < 65536f)
                {
                    radix = 2; // 16p7
                    mantissa = (int) (abs*128f + 0.5f);
                }
                else if (abs < 8388608f)
                {
                    radix = 1; // 23p0
                    mantissa = (int) (abs + 0.5f);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "Too large to store in a complex field");
                }
                mantissa *= sign;
                RawData = ((uint) (mantissa << 8)) | (radix << 4) | (RawData & 0xFu);
            }
        }
    }

    public enum DimensionUnit
    {
        /// Pixels (px)
        PX = 0,

        /// Device Independant Points (dip)
        DIP = 1,

        /// Scaled device independant Points (sp)
        SP = 2,

        /// points (pt)
        PT = 3,

        /// inches (in)
        IN = 4,

        /// millimeters (mm)
        MM = 5,
    }

    public enum FractionUnit
    {
        /// Fraction (%)
        FRACTION = 0,

        /// Fraction of parent (%p)
        FRACTION_PARENT = 1,
    }

    public enum ValueType
    {
        /// Contains no data.
        TYPE_NULL = 0x00,

        /// Resource reference as a <see cref="ResTable_ref"/>.
        TYPE_REFERENCE = 0x01,

        /// Attribute resource identifier (Not supported)
        TYPE_ATTRIBUTE = 0x02,

        /// String reference as a <see cref="ResStringPool_ref"/>.
        TYPE_STRING = 0x03,

        /// Float value.
        TYPE_FLOAT = 0x04,

        /// Complex dimension value. Float value + <see cref="DimensionUnit"/>.
        TYPE_DIMENSION = 0x05,

        /// Complex fraction value. Float value + <see cref="FractionUnit"/>.
        TYPE_FRACTION = 0x06,
        //TYPE_FIRST_INT = 0x10,
        /// Integer rendered in decimal.
        TYPE_INT_DEC = 0x10,

        /// Integer rendered in hexadecimal.
        TYPE_INT_HEX = 0x11,

        /// Integer rendered as a boolean.
        TYPE_INT_BOOLEAN = 0x12,
        //TYPE_FIRST_COLOR_INT = 0x1c,
        /// <see cref="Color"/> value rendered as #AARRGGBB
        TYPE_INT_COLOR_ARGB8 = 0x1c,

        /// <see cref="Color"/> value rendered as #RRGGBB (alpha = FF)
        TYPE_INT_COLOR_RGB8 = 0x1d,

        /// <see cref="Color"/> value rendered as #ARGB
        TYPE_INT_COLOR_ARGB4 = 0x1e,

        /// <see cref="Color"/> value rendered as #RGB
        TYPE_INT_COLOR_RGB4 = 0x1f,
        //TYPE_LAST_COLOR_INT = 0x1f,
        //TYPE_LAST_INT = 0x1f
    }
}