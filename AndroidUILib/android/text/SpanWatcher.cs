using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.text
{
    public interface SpanWatcher
    {
        /**
        * This method is called to notify you that the specified object
        * has been attached to the specified range of the text.
        */
        void onSpanAdded(Spannable text, object what, int start, int end);
        /**
         * This method is called to notify you that the specified object
         * has been detached from the specified range of the text.
         */
        void onSpanRemoved(Spannable text, object what, int start, int end);
        /**
         * This method is called to notify you that the specified object
         * has been relocated from the range <code>ostart&hellip;oend</code>
         * to the new range <code>nstart&hellip;nend</code> of the text.
         */
        void onSpanChanged(Spannable text, object what, int ostart, int oend, int nstart, int nend);

    }
}
