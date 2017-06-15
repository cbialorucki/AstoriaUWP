// Copyright (c) 2012 Markus Jarderot
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;

namespace AndroidXml.Res
{
    /// <summary>
    /// Not sure this class is correct, since the original code dealing 
    /// with this is pretty cryptic.
    /// TODO: Check this, and remove this comment.
    /// </summary>
#if !NETSTANDARD1_3
    //[Serializable]
#endif
    public class ResTable_map
    {
        public ResTable_ref Name { get; set; }
        public Res_value Value { get; set; }

        public MapMetaAttributes? MetaName
        {
            get
            {
                var ident = (MapMetaAttributes?) Name.Ident;
                switch (ident)
                {
                    case MapMetaAttributes.ATTR_TYPE:
                    case MapMetaAttributes.ATTR_MIN:
                    case MapMetaAttributes.ATTR_MAX:
                    case MapMetaAttributes.ATTR_L10N:
                    case MapMetaAttributes.ATTR_OTHER:
                    case MapMetaAttributes.ATTR_ZERO:
                    case MapMetaAttributes.ATTR_ONE:
                    case MapMetaAttributes.ATTR_TWO:
                    case MapMetaAttributes.ATTR_FEW:
                    case MapMetaAttributes.ATTR_MANY:
                        return ident;
                    default:
                        return null;
                }
            }
            set
            {
                if (value != null)
                {
                    Name.Ident = (uint) value.Value;
                }
                else if (MetaName != null)
                {
                    Name.Ident = 0;
                }
            }
        }

        public MapAllowedTypes? AllowedTypes
        {
            get
            {
                if (MetaName != MapMetaAttributes.ATTR_TYPE) return null;
                return (MapAllowedTypes?) Value.RawData;
            }
            set
            {
                if (MetaName != MapMetaAttributes.ATTR_TYPE)
                {
                    throw new InvalidOperationException(
                        "Can't set AllowedTypes unless MetaName is ATTR_TYPE (0x01000000)");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                Value.RawData = (uint) value.Value;
            }
        }

        public MapL10N? L10N
        {
            get
            {
                if (MetaName != MapMetaAttributes.ATTR_L10N) return null;
                return (MapL10N?) Value.RawData;
            }
            set
            {
                if (MetaName != MapMetaAttributes.ATTR_L10N)
                {
                    throw new InvalidOperationException(
                        "Can't set L10N unless MetaName is ATTR_L10N (0x01000003)");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                Value.RawData = (uint) value.Value;
            }
        }
    }

    public enum MapMetaAttributes
    {
        ATTR_TYPE = 0x01000000,
        ATTR_MIN = 0x01000001,
        ATTR_MAX = 0x01000002,
        ATTR_L10N = 0x01000003,
        ATTR_OTHER = 0x01000004,
        ATTR_ZERO = 0x01000005,
        ATTR_ONE = 0x01000006,
        ATTR_TWO = 0x01000007,
        ATTR_FEW = 0x01000008,
        ATTR_MANY = 0x01000009
    }

    [Flags]
    public enum MapAllowedTypes
    {
        TYPE_ANY = 0x0000FFFF,
        TYPE_REFERENCE = 1 << 0,
        TYPE_STRING = 1 << 1,
        TYPE_INTEGER = 1 << 2,
        TYPE_BOOLEAN = 1 << 3,
        TYPE_COLOR = 1 << 4,
        TYPE_FLOAT = 1 << 5,
        TYPE_DIMENSION = 1 << 6,
        TYPE_FRACTION = 1 << 7,
        TYPE_ENUM = 1 << 16,
        TYPE_FLAGS = 1 << 17
    }

    public enum MapL10N
    {
        L10N_NOT_REQUIRED = 0,
        L10N_SUGGESTED = 1
    }
}