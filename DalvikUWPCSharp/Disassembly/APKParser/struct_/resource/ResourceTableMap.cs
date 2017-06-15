using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_.resource
{
    public class ResourceTableMap
    {
        // ...elided
        // ResTable_ref; unit32
        private long nameRef;

        private ResourceValue resValue;
        private string  data;

        public long getNameRef()
        {
            return nameRef;
        }

        public void setNameRef(long nameRef)
        {
            this.nameRef = nameRef;
        }

        public ResourceValue getResValue()
        {
            return resValue;
        }

        public void setResValue(ResourceValue resValue)
        {
            this.resValue = resValue;
        }

        public string  getData()
        {
            return data;
        }

        public void setData(string  data)
        {
            this.data = data;
        }

        public string tostring ()
        {
            return data;
        }

        public static class MapAttr
        {
            public static readonly int TYPE = 0x01000000 | (0 & 0xFFFF);

            // For integral attributes; this is the minimum value it can hold.
            public static readonly int MIN = 0x01000000 | (1 & 0xFFFF);

            // For integral attributes; this is the maximum value it can hold.
            public static readonly int MAX = 0x01000000 | (2 & 0xFFFF);

            // Localization of this resource is can be encouraged or required with
            // an aapt flag if this is set
            public static readonly int L10N = 0x01000000 | (3 & 0xFFFF);

            // for plural support; see android.content.res.PluralRules#attrForQuantity(int)
            public static readonly int OTHER = 0x01000000 | (4 & 0xFFFF);
            public static readonly int ZERO = 0x01000000 | (5 & 0xFFFF);
            public static readonly int ONE = 0x01000000 | (6 & 0xFFFF);
            public static readonly int TWO = 0x01000000 | (7 & 0xFFFF);
            public static readonly int FEW = 0x01000000 | (8 & 0xFFFF);
            public static readonly int MANY = 0x01000000 | (9 & 0xFFFF);

            public static int makeArray(int entry)
            {
                return (0x02000000 | (entry & 0xFFFF));
            }

        }

        public static class AttributeType
        {
            // No type has been defined for this attribute; use generic
            // type handling.  The low 16 bits are for types that can be
            // handled generically; the upper 16 require additional information
            // in the bag so can not be handled generically for ANY.
            public static readonly int ANY = 0x0000FFFF;

            // Attribute holds a references to another resource.
            public static readonly int REFERENCE = 1;

            // Attribute holds a generic string.
            public static readonly int STRING = 1 << 1;

            // Attribute holds an integer value.  ATTR_MIN and ATTR_MIN can
            // optionally specify a constrained range of possible integer values.
            public static readonly int INTEGER = 1 << 2;

            // Attribute holds a boolean integer.
            public static readonly int BOOLEAN = 1 << 3;

            // Attribute holds a color value.
            public static readonly int COLOR = 1 << 4;

            // Attribute holds a floating point value.
            public static readonly int FLOAT = 1 << 5;

            // Attribute holds a dimension value; such as "20px".
            public static readonly int DIMENSION = 1 << 6;

            // Attribute holds a fraction value; such as "20%".
            public static readonly int FRACTION = 1 << 7;

            // Attribute holds an enumeration.  The enumeration values are
            // supplied as additional entries in the map.
            public static readonly int ENUM = 1 << 16;

            // Attribute holds a bitmaks of flags.  The flag bit values are
            // supplied as additional entries in the map.
            public static readonly int FLAGS = 1 << 17;
        }
    }
}
