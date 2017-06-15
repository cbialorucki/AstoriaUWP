using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.ticomware.interop
{
    public class StringCharSequence : CharSequence
    {
        string mString;

        public StringCharSequence(string source)
        {
            mString = source;
        }

        public static implicit operator StringCharSequence(string s)
        {
            return new StringCharSequence(s);
        }

        public char charAt(int index)
        {
            char[] array = mString.ToArray();
            return array[index];
        }

        public int length()
        {
            return mString.Length;
        }

        public CharSequence subSequence(int begin, int end)
        {
            int dif = end - begin;
            return new StringCharSequence(mString.Substring(begin, end));
        }

        public string toString()
        {
            return mString;
        }
    }
}
