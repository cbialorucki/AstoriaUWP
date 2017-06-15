using AndroidInteropLib.android.content;
using AndroidInteropLib.android.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.view
{
    public abstract class ViewGroup : View, ViewParent, ViewManager
    {
        public abstract void addView(View view, LayoutParams param);
        public abstract void addView(View view);
        public abstract void removeView(View view);
        public void updateViewLayout(View view, LayoutParams param) { throw new NotImplementedException(); }

        public ViewGroup(Context c, AttributeSet a) : base(c, a)
        { }


        public LayoutParams generateLayoutParams(AttributeSet attrs)
        {
            //not implemented yet.
            //return null;

            return new LayoutParams(mContext, attrs);
        }

        public class LayoutParams
        {
            public const int FILL_PARENT = -1;
            public const int MATCH_PARENT = -1;
            public const int WRAP_CONTENT = -2;
            public int width;
            public int height;

            public LayoutParams(Context c, AttributeSet attrs)
            {

            }

            public LayoutParams(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            public LayoutParams(LayoutParams source)
            {
                this.width = source.width;
                this.height = source.height;
            }

            protected LayoutParams() { } 
        }

        public class MarginLayoutParams : LayoutParams
        {
            public int leftMargin;
            public int topMargin;
            public int rightMargin;
            public int bottomMargin;

            private int startMargin = DEFAULT_MARGIN_RELATIVE;
            private int endMargin = DEFAULT_MARGIN_RELATIVE;
            public const int DEFAULT_MARGIN_RELATIVE = int.MinValue;

            byte mMarginFlags;

            private const int LAYOUT_DIRECTION_MASK = 0x00000003;
            private const int LEFT_MARGIN_UNDEFINED_MASK = 0x00000004;
            private const int RIGHT_MARGIN_UNDEFINED_MASK = 0x00000008;
            private const int RTL_COMPATIBILITY_MODE_MASK = 0x00000010;
            private const int NEED_RESOLUTION_MASK = 0x00000020;

            private const int DEFAULT_MARGIN_RESOLVED = 0;
            private const int UNDEFINED_MARGIN = DEFAULT_MARGIN_RELATIVE;

            public MarginLayoutParams(int width, int height) : base(width, height)
            {
                mMarginFlags |= LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags |= RIGHT_MARGIN_UNDEFINED_MASK;

                mMarginFlags &= NEED_RESOLUTION_MASK;
                mMarginFlags &= RTL_COMPATIBILITY_MODE_MASK;
            }

            public void setMargins(int left, int top, int right, int bottom)
            {
                leftMargin = left;
                topMargin = top;
                rightMargin = right;
                bottomMargin = bottom;
                mMarginFlags &= LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags &= RIGHT_MARGIN_UNDEFINED_MASK;

                if (isMarginRelative())
                {
                    mMarginFlags |= NEED_RESOLUTION_MASK;
                }
                else
                {
                    mMarginFlags &= NEED_RESOLUTION_MASK;
                }
            }

            public void setMarginsRelative(int start, int top, int end, int bottom)
            {
                startMargin = start;
                topMargin = top;
                endMargin = end;
                bottomMargin = bottom;
                mMarginFlags |= NEED_RESOLUTION_MASK;
            }

            public void setMarginStart(int start)
            {
                startMargin = start;
                mMarginFlags |= NEED_RESOLUTION_MASK;
            }

            public void setMarginEnd(int end)
            {
                endMargin = end;
                mMarginFlags |= NEED_RESOLUTION_MASK;
            }

            public bool isMarginRelative()
            {
                return (startMargin != DEFAULT_MARGIN_RELATIVE || endMargin != DEFAULT_MARGIN_RELATIVE);
            }





        }
    }
}
