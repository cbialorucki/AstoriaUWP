using AndroidInteropLib.android.content.res;
using AndroidInteropLib.android.util;
using AndroidInteropLib.com.android._internal.util;
using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content.res
{
    public class TypedArray
    {
        public static TypedArray obtain(Resources res, int len)
        {
            TypedArray attrs = res.mTypedArrayPool.acquire();
            if (attrs != null)
            {
                attrs.mLength = len;
                attrs.mRecycled = false;

                int fullLen = len * AssetManager.STYLE_NUM_ENTRIES;
                if (attrs.mData.Length >= fullLen)
                {
                    return attrs;
                }

                attrs.mData = new int[fullLen];
                attrs.mIndices = new int[1 + len];
                return attrs;
            }

            return new TypedArray(res, new int[len * AssetManager.STYLE_NUM_ENTRIES], new int[1 + len], len);
        }

        private Resources mResources;
        private DisplayMetrics mMetrics;
        private AssetManager mAssets;

        private bool mRecycled;
        //public XmlBlock.Parser mXml;
        //public Resources.Theme mTheme;
        public AndroidXml.AndroidXmlReader AXR;
        public int[] mData;
        public int[] mIndices;
        public int mLength { get; private set; }
        TypedValue mValue = new TypedValue();


        public int length()
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            return mLength;
        }

        public int getIndexCount()
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            return mIndices[0];
        }
        public int getIndex(int at)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            return mIndices[1 + at];
        }
        public Resources getResources()
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            return mResources;
        }

        public string getText(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return null;
            }
            else if (type == TypedValue.TYPE_STRING)
            {
                return loadStringValueAt(index);
            }

            TypedValue v = mValue;

            if (getValueAt(index, v))
            {
                //Log.w(Resources.TAG, "Converting to string: " + v);
                return v.coerceToString();
            }

            //Log.w(Resources.TAG, "getString of bad type: 0x" + type.ToString("X"));
            return null;
        }
        public string getString(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return null;
            }
            else if (type == TypedValue.TYPE_STRING)
            {
                return loadStringValueAt(index).ToString();
            }
            TypedValue v = mValue;
            if (getValueAt(index, v))
            {
                //Log.w(Resources.TAG, "Converting to string: " + v);
                string cs = v.coerceToString();
                return cs != null ? cs.ToString() : null;
            }
            //Log.w(Resources.TAG, "getstring of bad type: 0x" + Integer.toHexString(type));
            return null;
        }

        /*public string getNonResourceString(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_STRING)
            {
                int cookie = data[index + AssetManager.STYLE_ASSET_COOKIE];
                if (cookie < 0)
                {
                    return mXml.getPooledString(data[index + AssetManager.STYLE_DATA]).ToString();
                }
            }
            return null;
        }*/

        public string getNonConfigurationString(int index, int allowedChangingConfigs)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if ((data[index + AssetManager.STYLE_CHANGING_CONFIGURATIONS] & ~allowedChangingConfigs) != 0)
            {
                return null;
            }
            if (type == TypedValue.TYPE_NULL)
            {
                return null;
            }
            else if (type == TypedValue.TYPE_STRING)
            {
                return loadStringValueAt(index).ToString();
            }
            TypedValue v = mValue;
            if (getValueAt(index, v))
            {
                //Log.w(Resources.TAG, "Converting to string: " + v);
                string cs = v.coerceToString();
                return cs != null ? cs.ToString() : null;
            }
            //Log.w(Resources.TAG, "getstring of bad type: 0x" + Integer.toHexString(type));
            return null;
        }
        public bool getBoolean(int index, bool defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type >= TypedValue.TYPE_FIRST_INT
              && type <= TypedValue.TYPE_LAST_INT)
            {
                return data[index + AssetManager.STYLE_DATA] != 0;
            }
            TypedValue v = mValue;
            if (getValueAt(index, v))
            {
                //Log.w(Resources.TAG, "Converting to boolean: " + v);
                return XmlUtils.convertValueToBoolean(v.coerceToString(), defValue);
            }
            //Log.w(Resources.TAG, "getBoolean of bad type: 0x" + Integer.toHexString(type));
            return defValue;
        }
        public int getInt(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type >= TypedValue.TYPE_FIRST_INT
              && type <= TypedValue.TYPE_LAST_INT)
            {
                return data[index + AssetManager.STYLE_DATA];
            }
            TypedValue v = mValue;
            if (getValueAt(index, v))
            {
                //Log.w(Resources.TAG, "Converting to int: " + v);
                return XmlUtils.convertValueToInt(v.coerceToString(), defValue);
            }
            //Log.w(Resources.TAG, "getInt of bad type: 0x" + Integer.toHexString(type));
            return defValue;
        }
        public float getFloat(int index, float defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type == TypedValue.TYPE_FLOAT)
            {
                //return Float.intBitsToFloat(data[index + AssetManager.STYLE_DATA]);
                return ticomware.interop.Util.intBitsToFloat(data[index + AssetManager.STYLE_DATA]);
            }
            else if (type >= TypedValue.TYPE_FIRST_INT
              && type <= TypedValue.TYPE_LAST_INT)
            {
                return data[index + AssetManager.STYLE_DATA];
            }
            TypedValue v = mValue;
            if (getValueAt(index, v))
            {
                //Log.w(Resources.TAG, "Converting to float: " + v);
                string str = v.coerceToString();
                if (str != null)
                {
                    return float.Parse(str.ToString());
                }
            }
            //Log.w(Resources.TAG, "getFloat of bad type: 0x" + Integer.toHexString(type));
            return defValue;
        }
        public int getColor(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type >= TypedValue.TYPE_FIRST_INT && type <= TypedValue.TYPE_LAST_INT)
            {
                return data[index + AssetManager.STYLE_DATA];
            }

            else if (type == TypedValue.TYPE_STRING)
            {
                TypedValue value = mValue;
                if (getValueAt(index, value))
                {
                    ColorStateList csl = mResources.loadColorStateList(value, value.resourceId);
                    return Convert.ToInt32(csl.getDefaultColor());
                }
                return defValue;
            }
            else if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                throw new Exception("Failed to resolve attribute at index " + index);
            }
            throw new Exception("Can't convert to color: type=0x" + type.ToString("X"));
        }

        public ColorStateList getColorStateList(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            TypedValue value = mValue;
            //TypedValue hack = new TypedValue();
            if (getValueAt(index *= AssetManager.STYLE_NUM_ENTRIES, value))
            {
                if (value.type == TypedValue.TYPE_ATTRIBUTE)
                {
                    throw new Exception("Failed to resolve attribute at index " + index);
                }
                return mResources.loadColorStateList(value, value.resourceId);
            }

            return null;
        }

        public int getInteger(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type >= TypedValue.TYPE_FIRST_INT
              && type <= TypedValue.TYPE_LAST_INT)
            {
                return data[index + AssetManager.STYLE_DATA];
            }
            else if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                throw new Exception("Failed to resolve attribute at index " + index);
            }
            throw new Exception("Can't convert to integer: type=0x"+ type.ToString("X"));
        }
        public float getDimension(int index, float defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type == TypedValue.TYPE_DIMENSION)
            {
                return TypedValue.complexToDimension(data[index + AssetManager.STYLE_DATA], mMetrics);
            }
            else if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                throw new Exception("Failed to resolve attribute at index " + index);
            }
            throw new Exception("Can't convert to dimension: type=0x" + type.ToString("X"));
        }
        public int getDimensionPixelOffset(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type == TypedValue.TYPE_DIMENSION)
            {
                return TypedValue.complexToDimensionPixelOffset(data[index + AssetManager.STYLE_DATA], mMetrics);
            }
            else if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                throw new Exception("Failed to resolve attribute at index " + index);
            }

            throw new Exception("Can't convert to dimension: type=0x"+ type.ToString("X"));
        }

        public int getDimensionPixelSize(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type == TypedValue.TYPE_DIMENSION)
            {
                return TypedValue.complexToDimensionPixelSize(data[index + AssetManager.STYLE_DATA], mMetrics);
            }
            else if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                throw new Exception("Failed to resolve attribute at index " + index);
            }
            throw new Exception("Can't convert to dimension: type=0x" + type.ToString("X"));
        }
        public int getLayoutDimension(int index, string name)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type >= TypedValue.TYPE_FIRST_INT
                    && type <= TypedValue.TYPE_LAST_INT)
            {
                return data[index + AssetManager.STYLE_DATA];
            }
            else if (type == TypedValue.TYPE_DIMENSION)
            {
                return TypedValue.complexToDimensionPixelSize(
                    data[index + AssetManager.STYLE_DATA], mMetrics);
            }
            else if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                throw new Exception("Failed to resolve attribute at index " + index);
            }

            //throw new Exception(getPositionDescription() + ": You must supply a " + name + " attribute.");
            throw new Exception("You must supply a " + name + " attribute.");
        }
        public int getLayoutDimension(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type >= TypedValue.TYPE_FIRST_INT && type <= TypedValue.TYPE_LAST_INT)
            {
                return data[index + AssetManager.STYLE_DATA];
            }

            else if (type == TypedValue.TYPE_DIMENSION)
            {
                return TypedValue.complexToDimensionPixelSize(data[index + AssetManager.STYLE_DATA], mMetrics);
            }

            return defValue;
        }
        public float getFraction(int index, int base1, int pbase, float defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return defValue;
            }
            else if (type == TypedValue.TYPE_FRACTION)
            {
                return TypedValue.complexToFraction(
                    data[index + AssetManager.STYLE_DATA], base1, pbase);
            }
            else if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                throw new Exception("Failed to resolve attribute at index " + index);
            }
            throw new Exception("Can't convert to fraction: type=0x"+ type.ToString("X"));
        }
        public int getResourceId(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            if (data[index + AssetManager.STYLE_TYPE] != TypedValue.TYPE_NULL)
            {
                int resid = data[index + AssetManager.STYLE_RESOURCE_ID];
                if (resid != 0)
                {
                    return resid;
                }
            }
            return defValue;
        }
        public int getThemeAttributeId(int index, int defValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            if (data[index + AssetManager.STYLE_TYPE] == TypedValue.TYPE_ATTRIBUTE)
            {
                return data[index + AssetManager.STYLE_DATA];
            }
            return defValue;
        }

        /*public Drawable getDrawable(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            TypedValue value = mValue;
            if (getValueAt(index *= AssetManager.STYLE_NUM_ENTRIES))
            { 
                if (value.type == TypedValue.TYPE_ATTRIBUTE)
                {
                    throw new Exception("Failed to resolve attribute at index " + index);
                }
                return mResources.loadDrawable(value, value.resourceId, mTheme);
            }
            return null;
        }*/

        public string[] getTextArray(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            TypedValue value = mValue;
            //TypedValue hack = new TypedValue();
            if (getValueAt(index *= AssetManager.STYLE_NUM_ENTRIES, value))
            {
                return mResources.getTextArray(value.resourceId);
            }
                return null;
        }

        public bool getValue(int index, TypedValue outValue)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            return getValueAt(index *= AssetManager.STYLE_NUM_ENTRIES, outValue);
        }

        public int getType(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            index *= AssetManager.STYLE_NUM_ENTRIES;
            return mData[index + AssetManager.STYLE_TYPE];
        }

        public bool hasValue(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            return type != TypedValue.TYPE_NULL;
        }

        public bool hasValueOrEmpty(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            index *= AssetManager.STYLE_NUM_ENTRIES;
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            return type != TypedValue.TYPE_NULL || data[index + AssetManager.STYLE_DATA] == TypedValue.DATA_NULL_EMPTY;
        }

        public TypedValue peekValue(int index)
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            TypedValue value = mValue;
            if (getValueAt(index *= AssetManager.STYLE_NUM_ENTRIES, value))
            { 
                return value;
            }

            return null;
        }

        /*public string getPositionDescription()
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }

            return mXml != null ? mXml.getPositionDescription() : "<internal>";
        }*/

        public void recycle()
        {
            if (mRecycled)
            {
                throw new Exception(toString() + " recycled twice!");
            }
            mRecycled = true;
            //mXml = null;
            //mTheme = null;
            mResources.mTypedArrayPool.release(this);
        }

        public int[] extractThemeAttrs()
        {
            if (mRecycled)
            {
                throw new Exception("Cannot make calls to a recycled instance!");
            }
            int[] attrs = null;
            int[] data = mData;
            int N = length();
            for (int i = 0; i < N; i++)
            {
                int index = i * AssetManager.STYLE_NUM_ENTRIES;
                if (data[index + AssetManager.STYLE_TYPE] != TypedValue.TYPE_ATTRIBUTE)
                {
                    continue;
                }
                data[index + AssetManager.STYLE_TYPE] = TypedValue.TYPE_NULL;
                int attr = data[index + AssetManager.STYLE_DATA];
                if (attr == 0)
                {
                    continue;
                }
                if (attrs == null)
                {
                    attrs = new int[N];
                }
                attrs[i] = attr;
            }
            return attrs;
        }

        public int getChangingConfigurations()
        {
            int changingConfig = 0;
            int[] data = mData;
            int N = length();
            for (int i = 0; i < N; i++)
            {
                int index = i *= AssetManager.STYLE_NUM_ENTRIES;
                int type = data[index + AssetManager.STYLE_TYPE];
                if (type == TypedValue.TYPE_NULL)
                {
                    continue;
                }
                changingConfig |= data[index + AssetManager.STYLE_CHANGING_CONFIGURATIONS];
            }
            return changingConfig;
        }

        private bool getValueAt(int index, TypedValue outValue)
        {
            int[] data = mData;
            int type = data[index + AssetManager.STYLE_TYPE];
            if (type == TypedValue.TYPE_NULL)
            {
                return false;
            }
            outValue.type = type;
            outValue.data = data[index + AssetManager.STYLE_DATA];
            outValue.assetCookie = data[index + AssetManager.STYLE_ASSET_COOKIE];
            outValue.resourceId = data[index + AssetManager.STYLE_RESOURCE_ID];
            outValue.changingConfigurations = data[index + AssetManager.STYLE_CHANGING_CONFIGURATIONS];
            outValue.density = data[index + AssetManager.STYLE_DENSITY];
            outValue.string1 = (type == TypedValue.TYPE_STRING) ? loadStringValueAt(index) : null;
            return true;
        }

        private string loadStringValueAt(int index)
        {
            return AXR.StringPool.StringData[index];

            /*int[] data = mData;
            int cookie = data[index + AssetManager.STYLE_ASSET_COOKIE];
            if (cookie < 0)
            {
                if (mXml != null)
                {
                    return mXml.getPooledString(data[index + AssetManager.STYLE_DATA]);
                }
                return null;
            }

            return mAssets.getPooledStringForCookie(cookie, data[index + AssetManager.STYLE_DATA]).toString();*/
        }

        public TypedArray(Resources resources, int[] data, int[] indices, int len)
        {
            mResources = resources;
            mMetrics = mResources.mMetrics;
            mAssets = mResources.mAssets;
            mData = data;
            mIndices = indices;
            mLength = len;
        }

        public string toString()
        {
            //return Arrays.toString(mData);
            return mData.ToString();
        }

        public override string ToString()
        {
            return toString();
        }

    }
}
