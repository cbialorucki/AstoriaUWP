using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.text
{
    public interface Spanned : CharSequence
    {
        T[] getSpans<T>(int start, int end, Type type);
        int getSpanStart(object tag);
        int getSpanEnd(object tag);
        int getSpanFlags(object tag);

    }

    public struct SpannedVals
    {
        public const int SPAN_POINT_MARK_MASK = 0x33;
        public const int SPAN_MARK_MARK = 0x11;
        public const int SPAN_MARK_POINT = 0x12;
        public const int SPAN_POINT_MARK = 0x21;
        public const int SPAN_POINT_POINT = 0x22;
        public const int SPAN_PARAGRAPH = 0x33;
        public const int SPAN_INCLUSIVE_EXCLUSIVE = SPAN_MARK_MARK;
        public const int SPAN_INCLUSIVE_INCLUSIVE = SPAN_MARK_POINT;
        public const int SPAN_EXCLUSIVE_EXCLUSIVE = SPAN_POINT_MARK;
        public const int SPAN_EXCLUSIVE_INCLUSIVE = SPAN_POINT_POINT;
        public const int SPAN_COMPOSING = 0x100;
        public const int SPAN_INTERMEDIATE = 0x200;
        public const int SPAN_USER_SHIFT = 24;
        public const uint SPAN_USER = 0xFFFFFFFF << SPAN_USER_SHIFT;
        public const int SPAN_PRIORITY_SHIFT = 16;
        public const int SPAN_PRIORITY = 0xFF << SPAN_PRIORITY_SHIFT;
    }
}
