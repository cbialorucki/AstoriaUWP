using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.com.android._internal.util
{
    public class XmlUtils
    {
        public static int convertValueToList(string value, string[] options, int defaultValue)
        {
            if (null != value)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    if (value.Equals(options[i]))
                        return i;
                }
            }

            return defaultValue;
        }


        public static bool convertValueToBoolean(string value, bool defaultValue)
        {
            bool result = false;

            if (null == value)
                return defaultValue;

            if (value.Equals("1") || value.Equals("true") || value.Equals("TRUE"))
                result = true;

            return result;
        }

        public static int convertValueToInt(string charSeq, int defaultValue)
        {
            //recieved @layout/content_main? why?

            if (null == charSeq)
                return defaultValue;

            string nm = charSeq.ToString();

            // XXX This code is copied from Integer.decode() so we don't
            // have to instantiate an Integer!

            //int value;
            int sign = 1;
            int index = 0;
            int len = nm.Length;
            int bse = 10;

            if ('-' == nm.ToCharArray()[0])
            {
                sign = -1;
                index++;
            }

            if ('0' == nm.ToCharArray()[index])
            {
                //  Quick check for a zero by itself
                if (index == (len - 1))
                    return 0;

                char c = nm.ToCharArray()[index + 1];

                if ('x' == c || 'X' == c)
                {
                    index += 2;
                    bse = 16;
                }
                else
                {
                    index++;
                    bse = 8;
                }
            }
            else if ('#' == nm.ToCharArray()[index])
            {
                index++;
                bse = 16;
            }

            return Convert.ToInt32(nm.Substring(index), bse) * sign; //int.Parse(nm.Substring(index), bse) * sign;
        }

        public static uint convertValueToUnsignedInt(string value, uint defaultValue)
        {
            if (null == value)
            {
                return defaultValue;
            }

            return parseUnsignedIntAttribute(value);
        }

        public static uint parseUnsignedIntAttribute(string charSeq)
        {
            string value = charSeq.ToString();

            //long bits;
            int index = 0;
            int len = value.Length;
            int bse = 10;

            if ('0' == value.ToCharArray()[index]) //.charAt(index))
            {
                //  Quick check for zero by itself
                if (index == (len - 1))
                    return 0;

                char c = value.ToCharArray()[index + 1]; //.charAt(index + 1);

                if ('x' == c || 'X' == c)
                {     //  check for hex
                    index += 2;
                    bse = 16;
                }
                else {                        //  check for octal
                    index++;
                    bse = 8;
                }
            }
            else if ('#' == value.ToCharArray()[index])
            {
                index++;
                bse = 16;
            }

            return Convert.ToUInt32(value.Substring(index), bse); //(uint)long.Parse(value.Substring(index), /bse);
        }



    }
}
