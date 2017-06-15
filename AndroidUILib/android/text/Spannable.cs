using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.text
{
    public interface Spannable : Spanned
    {
        void setSpan(object what, int start, int end, int flags);
        void removeSpan(object what);

    }

    /*public class SpannableFactory
    {
        private static SpannableFactory sInstance = new SpannableFactory();

        //Returns the standard Spannable Factory.
        public static SpannableFactory getInstance()
        {
            return sInstance;
        }

        //Returns a new SpannableString from the specified CharSequence.
        //You can override this to provide a different kind of Spannable.
        public Spannable newSpannable(CharSequence source)
        {
            return new SpannableString(source);
        }
    }*/
}
