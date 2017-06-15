using AndroidInteropLib.com.android._internal.util;
using AndroidInteropLib.java.lang;
using AndroidInteropLib.libcore.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.text
{
    /*public abstract class SpannableStringInternal
    {
        private string mText;
        private object[] mSpans;
        private int[] mSpanData;
        private int mSpanCount;

        //package
        static object[] EMPTY = new object[0];

        private static int START = 0;
        private static int END = 1;
        private static int FLAGS = 2;
        private static int COLUMNS = 3;

        public SpannableStringInternal(CharSequence source, int start, int end)
        {
            if (start == 0 && end == source.length())
                mText = source.toString();
            else
                mText = source.toString().Substring(start, end);

            mSpans = EmptyArray.OBJECT;
            mSpanData = EmptyArray.INT;

            if (source is Spanned) 
            {
                Spanned sp = (Spanned)source;
                object[] spans = sp.getSpans<object>(start, end, typeof(object));

                for (int i = 0; i < spans.Length; i++)
                {
                    int st = sp.getSpanStart(spans[i]);
                    int en = sp.getSpanEnd(spans[i]);
                    int fl = sp.getSpanFlags(spans[i]);

                    if (st<start)
                        st = start;
                    if (en > end)
                        en = end;

                    setSpan(spans[i], st - start, en - start, fl);
                }
            }
        }

        public int length()
        {
            return mText.Length;
        }

        public char charAt(int i)
        {
            return mText.charAt(i);
        }

        public string toString()
        {
            return mText;
        }

        //subclasses must do subSequence() to preserve type

        public void getChars(int start, int end, char[] dest, int off)
        {
            mText.getChars(start, end, dest, off);
        }

        //package
        void setSpan(object what, int start, int end, int flags)
        {
            int nstart = start;
            int nend = end;

            checkRange("setSpan", start, end);

            if ((flags & SpannedVals.SPAN_PARAGRAPH) == SpannedVals.SPAN_PARAGRAPH)
            {
                if (start != 0 && start != length())
                {
                    char c = charAt(start - 1);

                    if (c != '\n')
                        throw new Exception("PARAGRAPH span must start at paragraph boundary" + " (" + start + " follows " + c + ")");
                }

                if (end != 0 && end != length())
                {
                    char c = charAt(end - 1);

                    if (c != '\n')
                        throw new Exception("PARAGRAPH span must end at paragraph boundary" + " (" + end + " follows " + c + ")");
                }
            }

            int count = mSpanCount;
            object[] spans = mSpans;
            int[] data = mSpanData;

            for (int i = 0; i < count; i++)
            {
                if (spans[i] == what)
                {
                    int ostart = data[i * COLUMNS + START];
                    int oend = data[i * COLUMNS + END];

                    data[i * COLUMNS + START] = start;
                    data[i * COLUMNS + END] = end;
                    data[i * COLUMNS + FLAGS] = flags;

                    sendSpanChanged(what, ostart, oend, nstart, nend);
                    return;
                }
            }

            if (mSpanCount + 1 >= mSpans.Length)
            {
                object[] newtags = ArrayUtils.newUnpaddedObjectArray(GrowingArrayUtils.growSize(mSpanCount));
                int[] newdata = new int[newtags.Length * 3];

                System.Array.Copy(mSpans, 0, newtags, 0, mSpanCount);
                System.Array.Copy(mSpanData, 0, newdata, 0, mSpanCount * 3);

                mSpans = newtags;
                mSpanData = newdata;
            }

            mSpans[mSpanCount] = what;
            mSpanData[mSpanCount * COLUMNS + START] = start;
            mSpanData[mSpanCount * COLUMNS + END] = end;
            mSpanData[mSpanCount * COLUMNS + FLAGS] = flags;
            mSpanCount++;

            if (this is Spannable)
                    sendSpanAdded(what, nstart, nend);
        }

        //package
        void removeSpan(object what)
        {
            int count = mSpanCount;
            object[] spans = mSpans;
            int[] data = mSpanData;

            for (int i = count - 1; i >= 0; i--)
            {
                if (spans[i] == what)
                {
                    int ostart = data[i * COLUMNS + START];
                    int oend = data[i * COLUMNS + END];

                    int c = count - (i + 1);

                    System.Array.Copy(spans, i + 1, spans, i, c);
                    System.Array.Copy(data, (i + 1) * COLUMNS,
                                     data, i * COLUMNS, c * COLUMNS);

                    mSpanCount--;

                    sendSpanRemoved(what, ostart, oend);
                    return;
                }
            }
        }

        public int getSpanStart(object what)
        {
            int count = mSpanCount;
            object[] spans = mSpans;
            int[] data = mSpanData;

            for (int i = count - 1; i >= 0; i--)
            {
                if (spans[i] == what)
                {
                    return data[i * COLUMNS + START];
                }
            }

            return -1;
        }

        public int getSpanEnd(object what)
        {
            int count = mSpanCount;
            object[] spans = mSpans;
            int[] data = mSpanData;

            for (int i = count - 1; i >= 0; i--)
            {
                if (spans[i] == what)
                {
                    return data[i * COLUMNS + END];
                }
            }

            return -1;
        }

        public int getSpanFlags(object what)
        {
            int count = mSpanCount;
            object[] spans = mSpans;
            int[] data = mSpanData;

            for (int i = count - 1; i >= 0; i--)
            {
                if (spans[i] == what)
                {
                    return data[i * COLUMNS + FLAGS];
                }
            }

            return 0;
        }

        public T[] getSpans<T>(int queryStart, int queryEnd, Type kind)
        {
            //Not sure if this is correct :/
            kind = typeof(T);

            int count = 0;

            int spanCount = mSpanCount;
            object[] spans = mSpans;
            int[] data = mSpanData;
            object[] ret = null;
            object ret1 = null;

            for (int i = 0; i < spanCount; i++)
            {
                if (kind != null && !kind.IsInstanceOfType(spans[i]))
                {
                    continue;
                }

                int spanStart = data[i * COLUMNS + START];
                int spanEnd = data[i * COLUMNS + END];

                if (spanStart > queryEnd)
                {
                    continue;
                }
                if (spanEnd < queryStart)
                {
                    continue;
                }

                if (spanStart != spanEnd && queryStart != queryEnd)
                {
                    if (spanStart == queryEnd)
                    {
                        continue;
                    }
                    if (spanEnd == queryStart)
                    {
                        continue;
                    }
                }

                if (count == 0)
                {
                    ret1 = spans[i];
                    count++;
                }

                else 
                {
                    if (count == 1)
                    {
                        ret = (Object[])Array.CreateInstance(kind, spanCount - i + 1);
                        ret[0] = ret1;
                    }

                    int prio = data[i * COLUMNS + FLAGS] & SpannedVals.SPAN_PRIORITY;
                    if (prio != 0)
                    {
                        int j;

                        for (j = 0; j < count; j++)
                        {
                            int p = getSpanFlags(ret[j]) & SpannedVals.SPAN_PRIORITY;

                            if (prio > p)
                            {
                                break;
                            }
                        }

                        System.Array.Copy(ret, j, ret, j + 1, count - j);
                        ret[j] = spans[i];
                        count++;
                    }

                    else 
                    {
                        ret[count++] = spans[i];
                    }
                }
            }

            if (count == 0)
            {
                return (T[])ArrayUtils.emptyArray(kind);
            }

            if (count == 1)
            {
                ret = (object[])Array.CreateInstance(kind, 1);
                ret[0] = ret1;
                return (T[])ret;
            }

            if (count == ret.Length)
            {
                return (T[])ret;
            }

            object[] nret = (object[])Array.CreateInstance(kind, count);
            System.Array.Copy(ret, 0, nret, 0, count);
            return (T[])nret;
        }

        public int nextSpanTransition(int start, int limit, Type kind)
        {
            int count = mSpanCount;
            object[] spans = mSpans;
            int[] data = mSpanData;

            if (kind == null)
            {
                kind = typeof(object);
                //kind = Object ._class;
            }

            for (int i = 0; i<count; i++) 
            {
                int st = data[i * COLUMNS + START];
                int en = data[i * COLUMNS + END];

                if (st > start && st<limit && kind.IsInstanceOfType(spans[i]))
                    limit = st;
                if (en > start && en<limit && kind.IsInstanceOfType(spans[i]))
                    limit = en;
            }

            return limit;
        }

        private void sendSpanAdded(object what, int start, int end)
        {
            SpanWatcher[] recip = getSpans<SpanWatcher>(start, end, typeof(SpanWatcher));
            int n = recip.Length;

            for (int i = 0; i<n; i++)
            {
                recip[i].onSpanAdded((Spannable) this, what, start, end);
            }
        }

        private void sendSpanRemoved(object what, int start, int end)
        {
            SpanWatcher[] recip = getSpans<SpanWatcher>(start, end, typeof(SpanWatcher));
            int n = recip.Length;

            for (int i = 0; i<n; i++)
            {
                recip[i].onSpanRemoved((Spannable) this, what, start, end);
            }
        }

        private void sendSpanChanged(object what, int s, int e, int st, int en)
        {
            SpanWatcher[] recip = getSpans<SpanWatcher>(Math.Min(s, st), Math.Max(e, en), typeof(SpanWatcher));
            int n = recip.Length;

            for (int i = 0; i<n; i++)
            {
                recip[i].onSpanChanged((Spannable) this, what, s, e, st, en);
            }
        }

        private static string region(int start, int end)
        {
            return "(" + start + " ... " + end + ")";
        }

        private void checkRange(string operation, int start, int end)
        {
            if (end < start)
            {
                throw new IndexOutOfRangeException(operation + " " + region(start, end) + " has end before start");
            }

            int len = length();

            if (start > len || end > len)
            {
                throw new IndexOutOfRangeException(operation + " " + region(start, end) + " ends beyond length " + len);
            }

            if (start < 0 || end < 0)
            {
                throw new IndexOutOfRangeException(operation + " " + region(start, end) + " starts before 0");
            }
        }

    // Same as SpannableStringBuilder
    //@Override
        public bool equals(object o)
        {
            if (o is Spanned && toString().Equals(o.ToString()))
            {
                Spanned other = (Spanned)o;
                // Check span data
                object[] otherSpans = other.getSpans<object>(0, other.length(), typeof(object));
                if (mSpanCount == otherSpans.Length)
                {
                    for (int i = 0; i<mSpanCount; ++i)
                    {
                        object thisSpan = mSpans[i];
                        object otherSpan = otherSpans[i];
                        if (thisSpan == this)
                        {
                            if (other != otherSpan || getSpanStart(thisSpan) != other.getSpanStart(otherSpan) || getSpanEnd(thisSpan) != other.getSpanEnd(otherSpan) || getSpanFlags(thisSpan) != other.getSpanFlags(otherSpan))
                            {
                                return false;
                            }
                        }

                        else if (!thisSpan.Equals(otherSpan) || getSpanStart(thisSpan) != other.getSpanStart(otherSpan) || getSpanEnd(thisSpan) != other.getSpanEnd(otherSpan) || getSpanFlags(thisSpan) != other.getSpanFlags(otherSpan))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return equals(obj);
        }

        // Same as SpannableStringBuilder
        //@Override
        public int hashCode()
        {
            int hash = toString().GetHashCode(); //.hashCode();
            hash = hash * 31 + mSpanCount;
            for (int i = 0; i < mSpanCount; ++i)
            {
                Object span = mSpans[i];
                if (span != this)
                {
                    hash = hash * 31 + span.GetHashCode();
                }
                hash = hash * 31 + getSpanStart(span);
                hash = hash * 31 + getSpanEnd(span);
                hash = hash * 31 + getSpanFlags(span);
            }

            return hash;
        }

        public override int GetHashCode()
        {
            return hashCode();
        }

    }*/
}
