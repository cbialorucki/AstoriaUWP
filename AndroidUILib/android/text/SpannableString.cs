using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.text
{
    /*public class SpannableString : SpannableStringInternal, CharSequence, GetChars, Spannable
    {
        public SpannableString(CharSequence source) : base(source, 0, source.length()) { }

        private SpannableString(CharSequence source, int start, int end) : base(source, start, end) { }

        public static SpannableString valueOf(CharSequence source)
        {
            if (source is SpannableString) 
            {
                return (SpannableString)source;
            }
            else
            {
                return new SpannableString(source);
            }
        }

        public void setSpan(object what, int start, int end, int flags)
        {
            base.setSpan(what, start, end, flags);
            //super.setSpan(what, start, end, flags);
        }

        public void removeSpan(object what)
        {
            base.removeSpan(what);
        }

        public CharSequence subSequence(int start, int end)
        {
            return new SpannableString(this, start, end);
        }
    }*/
}
