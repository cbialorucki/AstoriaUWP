using AndroidInteropLib.com.android._internal.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    public class LongSparseArray<E>
    {
        private static readonly object DELETED = new object();
        private bool mGarbage = false;

        private long[] mKeys;
        private object[] mValues;
        private int mSize;

        public LongSparseArray() : this(10) { }

        public LongSparseArray(int initialCapacity)
        {
            if (initialCapacity == 0)
            {
                mKeys = Array.Empty<long>(); //EmptyArray.LONG;
                mValues = Array.Empty<object>(); //EmptyArray.OBJECT;
            }
            else
            {
                mKeys = ArrayUtils.newUnpaddedLongArray(initialCapacity);
                mValues = ArrayUtils.newUnpaddedObjectArray(initialCapacity);
            }

            mSize = 0;
        }

        public E get(long key)
        {
            return get(key, default(E));
        }

        public E get(long key, E valueIfKeyNotFound)
        {
            int i = ContainerHelpers.binarySearch(mKeys, mSize, key);

            if (i < 0 || mValues[i] == DELETED)
            {
                return valueIfKeyNotFound;
            }
            else
            {
                return (E)mValues[i];
            }
        }

        public void delete(long key)
        {
            int i = ContainerHelpers.binarySearch(mKeys, mSize, key);

            if (i >= 0)
            {
                if (mValues[i] != DELETED)
                {
                    mValues[i] = DELETED;
                    mGarbage = true;
                }
            }
        }

        public void remove(long key)
        {
            delete(key);
        }

        public void removeAt(int index)
        {
            if (mValues[index] != DELETED)
            {
                mValues[index] = DELETED;
                mGarbage = true;
            }
        }

        private void gc()
        {
            // Log.e("SparseArray", "gc start with " + mSize);

            int n = mSize;
            int o = 0;
            long[] keys = mKeys;
            object[] values = mValues;

            for (int i = 0; i < n; i++)
            {
                object val = values[i];

                if (val != DELETED)
                {
                    if (i != o)
                    {
                        keys[o] = keys[i];
                        values[o] = val;
                        values[i] = null;
                    }

                    o++;
                }
            }

            mGarbage = false;
            mSize = o;

            // Log.e("SparseArray", "gc end with " + mSize);
        }

        public void put(long key, E value)
        {
            int i = ContainerHelpers.binarySearch(mKeys, mSize, key);

            if (i >= 0)
            {
                mValues[i] = value;
            }
            else
            {
                i = ~i;

                if (i < mSize && mValues[i] == DELETED)
                {
                    mKeys[i] = key;
                    mValues[i] = value;
                    return;
                }

                if (mGarbage && mSize >= mKeys.Length)
                {
                    gc();

                    // Search again because indices may have changed.
                    i = ~ContainerHelpers.binarySearch(mKeys, mSize, key);
                }

                mKeys = GrowingArrayUtils.insert(mKeys, mSize, i, key);
                mValues = GrowingArrayUtils.insert(mValues, mSize, i, value);
                mSize++;
            }
        }

        public int size()
        {
            if (mGarbage)
            {
                gc();
            }

            return mSize;
        }

        public long keyAt(int index)
        {
            if (mGarbage)
            {
                gc();
            }

            return mKeys[index];
        }

        public E valueAt(int index)
        {
            if (mGarbage)
            {
                gc();
            }

            return (E)mValues[index];
        }

        public void setValueAt(int index, E value)
        {
            if (mGarbage)
            {
                gc();
            }

            mValues[index] = value;
        }

        public int indexOfKey(long key)
        {
            if (mGarbage)
            {
                gc();
            }

            return ContainerHelpers.binarySearch(mKeys, mSize, key);
        }

        public int indexOfValue(E value)
        {
            if (mGarbage)
            {
                gc();
            }

            for (int i = 0; i < mSize; i++)
                if (mValues[i].Equals(value))
                    return i;

            return -1;
        }

        public void clear()
        {
            int n = mSize;
            object[] values = mValues;

            for (int i = 0; i < n; i++)
            {
                values[i] = null;
            }

            mSize = 0;
            mGarbage = false;
        }

        public void append(long key, E value)
        {
            if (mSize != 0 && key <= mKeys[mSize - 1])
            {
                put(key, value);
                return;
            }

            if (mGarbage && mSize >= mKeys.Length)
            {
                gc();
            }

            mKeys = GrowingArrayUtils.append(mKeys, mSize, key);
            mValues = GrowingArrayUtils.append(mValues, mSize, value);
            mSize++;
        }

        public string toString()
        {
            if (size() <= 0)
            {
                return "{}";
            }

            StringBuilder buffer = new StringBuilder(mSize * 28);
            buffer.Append('{');
            for (int i = 0; i < mSize; i++)
            {
                if (i > 0)
                {
                    buffer.Append(", ");
                }
                long key = keyAt(i);
                buffer.Append(key);
                buffer.Append('=');
                object value = valueAt(i);
                if (value != this)
                {
                    buffer.Append(value);
                }
                else {
                    buffer.Append("(this Map)");
                }
            }
            buffer.Append('}');

            return buffer.ToString();
        }

        public override string ToString()
        {
            return toString();
        }


    }
}
