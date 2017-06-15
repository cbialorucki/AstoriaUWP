using AndroidInteropLib.com.android._internal.util;
using AndroidInteropLib.java.lang;
using AndroidInteropLib.libcore.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    public class SparseArray<E>
    {
        private readonly static object DELETED = new Object();
        private bool mGarbage = false;

        private int[] mKeys;
        private object[] mValues;
        private int mSize;

        /**
         * Creates a new SparseArray containing no mappings.
         */
        public SparseArray() : this(10) { }

        /**
         * Creates a new SparseArray containing no mappings that will not
         * require any additional memory allocation to store the specified
         * number of mappings.  If you supply an initial capacity of 0, the
         * sparse array will be initialized with a light-weight representation
         * not requiring any additional array allocations.
         */
        public SparseArray(int initialCapacity)
        {
            if (initialCapacity == 0)
            {
                mKeys = EmptyArray.INT;
                mValues = EmptyArray.OBJECT;
            }
            else
            {
                mValues = ArrayUtils.newUnpaddedObjectArray(initialCapacity);
                mKeys = new int[mValues.Length];
            }
            mSize = 0;
        }

        public SparseArray<E> clone()
        {
            SparseArray<E> clone = null;
            try
            {
                clone = (SparseArray<E>)clone.MemberwiseClone();
                clone.mKeys = mKeys;
                clone.mValues = mValues;
            }

            catch
            {
                /* ignore */
            }

            return clone;
        }

        /**
         * Gets the Object mapped from the specified key, or <code>null</code>
         * if no such mapping has been made.
         */
        public E get(int key)
        {
            return get(key, default(E));
        }

        /**
         * Gets the Object mapped from the specified key, or the specified Object
         * if no such mapping has been made.
         */
        public E get(int key, E valueIfKeyNotFound)
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

        /**
         * Removes the mapping from the specified key, if there was any.
         */
        public void delete(int key)
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

        /**
         * Alias for {@link #delete(int)}.
         */
        public void remove(int key)
        {
            delete(key);
        }

        /**
         * Removes the mapping at the specified index.
         */
        public void removeAt(int index)
        {
            if (mValues[index] != DELETED)
            {
                mValues[index] = DELETED;
                mGarbage = true;
            }
        }

        /**
         * Remove a range of mappings as a batch.
         *
         * @param index Index to begin at
         * @param size Number of mappings to remove
         */
        public void removeAtRange(int index, int size)
        {
            int end = Math.Min(mSize, index + size);
            for (int i = index; i < end; i++)
            {
                removeAt(i);
            }
        }

        private void gc()
        {
            // Log.e("SparseArray", "gc start with " + mSize);

            int n = mSize;
            int o = 0;
            int[] keys = mKeys;
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

        /**
         * Adds a mapping from the specified key to the specified value,
         * replacing the previous mapping from the specified key if there
         * was one.
         */
        public void put(int key, E value)
        {
            int i = ContainerHelpers.binarySearch(mKeys, mSize, key);

            if (i >= 0)
            {
                mValues[i] = value;
            }
            else {
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

        /**
         * Returns the number of key-value mappings that this SparseArray
         * currently stores.
         */
        public int size()
        {
            if (mGarbage)
            {
                gc();
            }

            return mSize;
        }

        /**
         * Given an index in the range <code>0...size()-1</code>, returns
         * the key from the <code>index</code>th key-value mapping that this
         * SparseArray stores.
         *
         * <p>The keys corresponding to indices in ascending order are guaranteed to
         * be in ascending order, e.g., <code>keyAt(0)</code> will return the
         * smallest key and <code>keyAt(size()-1)</code> will return the largest
         * key.</p>
         */
        public int keyAt(int index)
        {
            if (mGarbage)
            {
                gc();
            }

            return mKeys[index];
        }

    /**
     * Given an index in the range <code>0...size()-1</code>, returns
     * the value from the <code>index</code>th key-value mapping that this
     * SparseArray stores.
     *
     * <p>The values corresponding to indices in ascending order are guaranteed
     * to be associated with keys in ascending order, e.g.,
     * <code>valueAt(0)</code> will return the value associated with the
     * smallest key and <code>valueAt(size()-1)</code> will return the value
     * associated with the largest key.</p>
     */
        public E valueAt(int index)
        {
            if (mGarbage)
            {
                gc();
            }

            return (E)mValues[index];
        }

        /**
         * Given an index in the range <code>0...size()-1</code>, sets a new
         * value for the <code>index</code>th key-value mapping that this
         * SparseArray stores.
         */
        public void setValueAt(int index, E value)
        {
            if (mGarbage)
            {
                gc();
            }

            mValues[index] = value;
        }

        /**
         * Returns the index for which {@link #keyAt} would return the
         * specified key, or a negative number if the specified
         * key is not mapped.
         */
        public int indexOfKey(int key)
        {
            if (mGarbage)
            {
                gc();
            }

            return ContainerHelpers.binarySearch(mKeys, mSize, key);
        }

        /**
         * Returns an index for which {@link #valueAt} would return the
         * specified key, or a negative number if no keys map to the
         * specified value.
         * <p>Beware that this is a linear search, unlike lookups by key,
         * and that multiple keys can map to the same value and this will
         * find only one of them.
         * <p>Note also that unlike most collections' {@code indexOf} methods,
         * this method compares values using {@code ==} rather than {@code equals}.
         */
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

        /**
         * Removes all key-value mappings from this SparseArray.
         */
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

        /**
         * Puts a key/value pair into the array, optimizing for the case where
         * the key is greater than all existing keys in the array.
         */
        public void append(int key, E value)
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

        /**
         * {@inheritDoc}
         *
         * <p>This implementation composes a string by iterating over its mappings. If
         * this map contains itself as a value, the string "(this Map)"
         * will appear in its place.
         */
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
                int key = keyAt(i);
                buffer.Append(key);
                buffer.Append('=');
                object value = valueAt(i);
                if (value != this)
                {
                    buffer.Append(value);
                }
                else
                {
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
