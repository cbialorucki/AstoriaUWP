using AndroidInteropLib.android.text;
using AndroidInteropLib.android.util;
using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content.res
{
    /*public class StringBlock
    {
        private static string TAG = "AssetManager";
        private static bool localLOGV = false;
        private long mNative;
        private bool mUseSparse;
        private bool mOwnsNative;
        private string[] mStrings;
        private SparseArray<string> mSparseStrings;
        StyleIDs mStyleIDs = null;

        public StringBlock(byte[] data, bool useSparse)
        {
            mNative = nativeCreate(data, 0, data.Length);
            mUseSparse = useSparse;
            mOwnsNative = true;
            //if (localLOGV) Log.v(TAG, "Created string block " + this + ": " + nativeGetSize(mNative));
        }

        public StringBlock(byte[] data, int offset, int size, bool useSparse)
        {
            mNative = nativeCreate(data, offset, size);
            mUseSparse = useSparse;
            mOwnsNative = true;
            //if (localLOGV) Log.v(TAG, "Created string block " + this + ": " + nativeGetSize(mNative));
        }

        public string get(int idx)
        {
            lock(this)
            {
                if (mStrings != null)
                {
                    string res1 = mStrings[idx];
                    if (res1 != null)
                    {
                        return res1;
                    }
                }

                else if (mSparseStrings != null)
                {
                    string res1 = mSparseStrings.get(idx);
                    if (res1 != null)
                    {
                        return res1;
                    }
                }

                else
                {
                    int num = nativeGetSize(mNative);
                    if (mUseSparse && num > 250)
                    {
                        mSparseStrings = new SparseArray<string>();
                    }
                    else
                    {
                        mStrings = new string[num];
                    }
                }

                string str = nativeGetString(mNative, idx);
                string res = str;
                int[] style = nativeGetStyle(mNative, idx);
                //if (localLOGV) Log.v(TAG, "Got string: " + str);
                //if (localLOGV) Log.v(TAG, "Got styles: " + Arrays.toString(style));
                if (style != null)
                {
                    if (mStyleIDs == null)
                    {
                        mStyleIDs = new StyleIDs();
                    }

                    // the style array is a flat array of <type, start, end> hence
                    // the magic privateant 3.
                    for (int styleIndex = 0; styleIndex < style.Length; styleIndex += 3)
                    {
                        int styleId = style[styleIndex];

                        if (styleId == mStyleIDs.boldId || styleId == mStyleIDs.italicId
                                || styleId == mStyleIDs.underlineId || styleId == mStyleIDs.ttId
                                || styleId == mStyleIDs.bigId || styleId == mStyleIDs.smallId
                                || styleId == mStyleIDs.subId || styleId == mStyleIDs.supId
                                || styleId == mStyleIDs.strikeId || styleId == mStyleIDs.listItemId
                                || styleId == mStyleIDs.marqueeId)
                        {
                            // id already found skip to next style
                            continue;
                        }

                        string styleTag = nativeGetString(mNative, styleId);

                        if (styleTag.Equals("b"))
                        {
                            mStyleIDs.boldId = styleId;
                        }
                        else if (styleTag.Equals("i"))
                        {
                            mStyleIDs.italicId = styleId;
                        }
                        else if (styleTag.Equals("u"))
                        {
                            mStyleIDs.underlineId = styleId;
                        }
                        else if (styleTag.Equals("tt"))
                        {
                            mStyleIDs.ttId = styleId;
                        }
                        else if (styleTag.Equals("big"))
                        {
                            mStyleIDs.bigId = styleId;
                        }
                        else if (styleTag.Equals("small"))
                        {
                            mStyleIDs.smallId = styleId;
                        }
                        else if (styleTag.Equals("sup"))
                        {
                            mStyleIDs.supId = styleId;
                        }
                        else if (styleTag.Equals("sub"))
                        {
                            mStyleIDs.subId = styleId;
                        }
                        else if (styleTag.Equals("strike"))
                        {
                            mStyleIDs.strikeId = styleId;
                        }
                        else if (styleTag.Equals("li"))
                        {
                            mStyleIDs.listItemId = styleId;
                        }
                        else if (styleTag.Equals("marquee"))
                        {
                            mStyleIDs.marqueeId = styleId;
                        }
                    }

                    res = applyStyles(str, style, mStyleIDs);
                }
                if (mStrings != null) mStrings[idx] = res;
                else mSparseStrings.put(idx, res);
                return res;
            }
        }

        protected void finalize()
        {
            try
            {
                //base.finalize();
            }
            finally
            {
                if (mOwnsNative)
                {
                    //nativeDestroy(mNative);
                }
            }
        }

        public class StyleIDs
        {
            public int boldId = -1;
            public int italicId = -1;
            public int underlineId = -1;
            public int ttId = -1;
            public int bigId = -1;
            public int smallId = -1;
            public int subId = -1;
            public int supId = -1;
            public int strikeId = -1;
            public int listItemId = -1;
            public int marqueeId = -1;
        }

        private string applyStyles(CharSequence str, int[] style, StyleIDs ids)
        {
            if (style.Length == 0)
                return str.toString();

            SpannableString buffer = new SpannableString(str);
            int i = 0;
            while (i < style.Length)
            {
                int type = style[i];
                //if (localLOGV) Log.v(TAG, "Applying style span id=" + type + ", start=" + style[i + 1] + ", end=" + style[i + 2]);


                if (type == ids.boldId)
                {
                    buffer.setSpan(new StyleSpan(Typeface.BOLD), style[i + 1], style[i + 2] + 1, SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.italicId)
                {
                    buffer.setSpan(new StyleSpan(Typeface.ITALIC),
                                   style[i + 1], style[i + 2] + 1,
                                   SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.underlineId)
                {
                    buffer.setSpan(new UnderlineSpan(), style[i + 1], style[i + 2] + 1, SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.ttId)
                {
                    buffer.setSpan(new TypefaceSpan("monospace"), style[i + 1], style[i + 2] + 1, SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.bigId)
                {
                    buffer.setSpan(new RelativeSizeSpan(1.25f), style[i + 1], style[i + 2] + 1, SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.smallId)
                {
                    buffer.setSpan(new RelativeSizeSpan(0.8f),
                                   style[i + 1], style[i + 2] + 1,
                                   SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.subId)
                {
                    buffer.setSpan(new SubscriptSpan(),
                                   style[i + 1], style[i + 2] + 1,
                                   SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.supId)
                {
                    buffer.setSpan(new SuperscriptSpan(),
                                   style[i + 1], style[i + 2] + 1,
                                   SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.strikeId)
                {
                    buffer.setSpan(new StrikethroughSpan(),
                                   style[i + 1], style[i + 2] + 1,
                                   SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
                else if (type == ids.listItemId)
                {
                    addParagraphSpan(buffer, new BulletSpan(10),
                                    style[i + 1], style[i + 2] + 1);
                }
                else if (type == ids.marqueeId)
                {
                    buffer.setSpan(TextUtils.TruncateAt.MARQUEE,
                                   style[i + 1], style[i + 2] + 1,
                                   SpannedVals.SPAN_INCLUSIVE_INCLUSIVE);
                }
                else {
                    string tag = nativeGetString(mNative, type);

                    if (tag.StartsWith("font;"))
                    {
                        string sub;

                        sub = subtag(tag, ";height=");
                        if (sub != null)
                        {
                            int size = int.Parse(sub);
                            addParagraphSpan(buffer, new Height(size),
                                           style[i + 1], style[i + 2] + 1);
                        }

                        sub = subtag(tag, ";size=");
                        if (sub != null)
                        {
                            int size = int.Parse(sub);
                            buffer.setSpan(new AbsoluteSizeSpan(size, true),
                                           style[i + 1], style[i + 2] + 1,
                                           SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                        }

                        sub = subtag(tag, ";fgcolor=");
                        if (sub != null)
                        {
                            buffer.setSpan(getColor(sub, true),
                                           style[i + 1], style[i + 2] + 1,
                                           SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                        }

                        sub = subtag(tag, ";color=");
                        if (sub != null)
                        {
                            buffer.setSpan(getColor(sub, true),
                                    style[i + 1], style[i + 2] + 1,
                                    SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                        }

                        sub = subtag(tag, ";bgcolor=");
                        if (sub != null)
                        {
                            buffer.setSpan(getColor(sub, false),
                                           style[i + 1], style[i + 2] + 1,
                                           SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                        }

                        sub = subtag(tag, ";face=");
                        if (sub != null)
                        {
                            buffer.setSpan(new TypefaceSpan(sub),
                                    style[i + 1], style[i + 2] + 1,
                                    SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                        }
                    }
                    else if (tag.startsWith("a;"))
                    {
                        string sub;

                        sub = subtag(tag, ";href=");
                        if (sub != null)
                        {
                            buffer.setSpan(new URLSpan(sub),
                                           style[i + 1], style[i + 2] + 1,
                                           SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                        }
                    }
                    else if (tag.StartsWith("annotation;"))
                    {
                        int len = tag.Length;
                        int next;

                        for (int t = tag.IndexOf(';'); t < len; t = next)
                        {
                            int eq = tag.IndexOf('=', t);
                            if (eq < 0)
                            {
                                break;
                            }

                            next = tag.IndexOf(';', eq);
                            if (next < 0)
                            {
                                next = len;
                            }

                            string key = tag.Substring(t + 1, eq);
                            string value = tag.Substring(eq + 1, next);

                            buffer.setSpan(new Annotation(key, value),
                                           style[i + 1], style[i + 2] + 1,
                                           SpannedVals.SPAN_EXCLUSIVE_EXCLUSIVE);
                        }
                    }
                }

                i += 3;
            }
            return new SpannedString(buffer);
        }


        private static CharacterStyle getColor(string color, bool foreground)
        {
            uint c = 0xff000000;

            if (!TextUtils.isEmpty(color))
            {
                if (color.StartsWith("@"))
                {
                    Resources res = Resources.getSystem();
                    String name = color.Substring(1);
                    int colorRes = res.getIdentifier(name, "color", "android");
                    if (colorRes != 0)
                    {
                        ColorStateList colors = res.getColorStateList(colorRes);
                        if (foreground)
                        {
                            return new TextAppearanceSpan(null, 0, 0, colors, null);
                        }
                        else {
                            c = colors.getDefaultColor();
                        }
                    }
                }
                else {
                    c = graphics.Color.getHtmlColor(color);
                }
            }

            if (foreground)
            {
                return new ForegroundColorSpan(c);
            }
            else
            {
                return new BackgroundColorSpan(c);
            }
        }


        private static void addParagraphSpan(Spannable buffer, object what, int start, int end)
        {
            int len = buffer.length();

            if (start != 0 && start != len && buffer.charAt(start - 1) != '\n')
            {
                for (start--; start > 0; start--)
                {
                    if (buffer.charAt(start - 1) == '\n')
                    {
                        break;
                    }
                }
            }

            if (end != 0 && end != len && buffer.charAt(end - 1) != '\n')
            {
                for (end++; end < len; end++)
                {
                    if (buffer.charAt(end - 1) == '\n')
                    {
                        break;
                    }
                }
            }

            buffer.setSpan(what, start, end, SpannedVals.SPAN_PARAGRAPH);
        }

        private static string subtag(string full, string attribute)
        {
            int start = full.IndexOf(attribute);
            if (start < 0)
            {
                return null;
            }

            start += attribute.Length;
            int end = full.IndexOf(';', start);

            if (end < 0)
            {
                return full.Substring(start);
            }
            else
            {
                return full.Substring(start, end);
            }
        }


        /*private static class Height : LineHeightSpan.WithDensity
        {
            private int mSize;
            private static float sProportion = 0;

            public Height(int size)
            {
                mSize = size;
            }

            public void chooseHeight(string text, int start, int end, int spanstartv, int v, Paint.FontMetricsInt fm)
            {
                // Should not get called, at least not by StaticLayout.
                chooseHeight(text, start, end, spanstartv, v, fm, null);
            }

            public void chooseHeight(string text, int start, int end, int spanstartv, int v, Paint.FontMetricsInt fm, TextPaint paint)
            {
                int size = mSize;
                if (paint != null)
                {
                    size *= paint.density;
                }

                if (fm.bottom - fm.top < size)
                {
                    fm.top = fm.bottom - size;
                    fm.ascent = fm.ascent - size;
                }
                else
                {
                    if (sProportion == 0)
                    {


                        Paint p = new Paint();
                        p.setTextSize(100);
                        Rect r = new Rect();
                        p.getTextBounds("ABCDEFG", 0, 7, r);

                        sProportion = (r.top) / p.ascent();
                    }

                    int need = (int)Math.Ceiling(-fm.top * sProportion);

                    if (size - fm.descent >= need)
                    {


                        fm.top = fm.bottom - size;
                        fm.ascent = fm.descent - size;
                    }

                    else if (size >= need)
                    {


                        fm.top = fm.ascent = -need;
                        fm.bottom = fm.descent = fm.top + size;
                    }

                    else
                    {
                        fm.top = fm.ascent = -size;
                        fm.bottom = fm.descent = 0;
                    }
                }
            }
        }



        public StringBlock(long obj, bool useSparse)
        {
            mNative = obj;
            mUseSparse = useSparse;
            mOwnsNative = false;
            //if (localLOGV) Log.v(TAG, "Created string block " + this + ": " + nativeGetSize(mNative));
        }

        private static long nativeCreate(byte[] data, int offset, int size)
        {
            if(data == null)
            {
                throw new NullReferenceException();
            }

            int bLen = data.Length;

            if (offset < 0 || offset >= bLen || size < 0 || size > bLen || (offset + size) > bLen)
            {
                throw new IndexOutOfRangeException();
            }



            //ResStringPool osb = new ResStringPool(b + offset, len, true);

        }
        private static int nativeGetSize(long obj) { }
        private static string nativeGetString(long obj, int idx) { }
        private static int[] nativeGetStyle(long obj, int idx) { }
        private static void nativeDestroy(long obj) { }

    }*/
}
