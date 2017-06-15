using AndroidInteropLib.android.util;
using AndroidInteropLib.org.xmlpull.v1;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content.res
{
    public class Resources
    {
        public const string TAG = "Resources";
        private const bool DEBUG_LOAD = false;
        private const bool DEBUG_CONFIG = false;

        private const int ID_OTHER = 0x01000004;
        private static readonly object sSync = new object();
        public DisplayMetrics mMetrics;
        public AssetManager mAssets;
        //private readonly Theme mTheme;


        public support.v4.Pools.SynchronizedPool<TypedArray> mTypedArrayPool = new support.v4.Pools.SynchronizedPool<TypedArray>(5);
        //new SynchronizedPool<TypedArray>(5);

        public virtual XmlResourceParser getLayout(int id)
        {
            return loadXmlResourceParser(id, "layout"); 
        }

        public virtual int getColor(int id)
        {
            throw new Exception("Must be overriden.");
        }

        public virtual string getString(int id)
        {
            throw new Exception("Must be overriden.");
        }

        public virtual string[] getStringArray(int id)
        {
            throw new Exception("Must be overriden.");
        }

        public virtual XmlResourceParser loadXmlResourceParser(int id, string type)
        {
            throw new Exception("Must be overriden.");
        }

        public virtual XmlResourceParser loadXmlResourceParser(string file, string type)
        {
            throw new Exception("Must be overriden.");
        }

        public virtual ColorStateList loadColorStateList(TypedValue tv, int id)
        {
            throw new Exception("Must be overriden.");
        }

        public virtual string[] getTextArray(int id)
        {
            throw new Exception("Must be overriden.");
        }

        /*public class Theme
        {
            public void applyStyle(int resId, bool force)
            {
                AssetManager.applyThemeStyle(mTheme, resId, force);

                mThemeResId = resId;
                mKey += resId.ToString("X") + (force ? "! " : " "); //Integer.toHexString(resId) + (force ? "! " : " ");
            }


            public void setTo(Theme other)
            {
                AssetManager.copyTheme(mTheme, other.mTheme);

                mThemeResId = other.mThemeResId;
                mKey = other.mKey;
            }


            public TypedArray obtainStyledAttributes(int[] attrs)
            {
                int len = attrs.Length;
                TypedArray array = TypedArray.obtain(mResources, len);
                array.mTheme = this;
                AssetManager.applyStyle(mTheme, 0, 0, 0, attrs, array.mData, array.mIndices);
                return array;
            }


            public TypedArray obtainStyledAttributes(int resid, int[] attrs)
            {
                int len = attrs.Length;
                TypedArray array = TypedArray.obtain(mResources, len);
                array.mTheme = this;

                //HUH?!?!?! Was this for debugging? WHY is this in production code?
                
                if (false)
                {
                    int[] data = array.mData;

                    Debug.WriteLine("**********************************************************");
                    Debug.WriteLine("**********************************************************");
                    Debug.WriteLine("**********************************************************");
                    Debug.WriteLine("Attributes:");
                    String s = "  Attrs:";
                    int i;
                    for (i=0; i<attrs.Length; i++)
                    {
                        s = s + " 0x" + Integer.toHexString(attrs[i]);
                    }
                    Debug.WriteLine(s);
                    s = "  Found:";
                    TypedValue value = new TypedValue();
                    for (i=0; i<attrs.Length; i++)
                    {
                        int d = i * AssetManager.STYLE_NUM_ENTRIES;
                        value.type = data[d + AssetManager.STYLE_TYPE];
                        value.data = data[d + AssetManager.STYLE_DATA];
                        value.assetCookie = data[d + AssetManager.STYLE_ASSET_COOKIE];
                        value.resourceId = data[d + AssetManager.STYLE_RESOURCE_ID];
                        s = s + " 0x" + Integer.toHexString(attrs[i]) + "=" + value;
                    }
                    Debug.WriteLine(s);
                }

                AssetManager.applyStyle(mTheme, 0, resid, 0, attrs, array.mData, array.mIndices);
                return array;
            }

        
            public virtual TypedArray obtainStyledAttributes(AttributeSet set, int[] attrs, int defStyleAttr, int defStyleRes)
            {
                int len = attrs.Length;
                TypedArray array = TypedArray.obtain(mResources, len);

                // XXX note that for now we only work with compiled XML files.
                // To support generic XML files we will need to manually parse
                // out the attributes from the XML file (applying type information
                // contained in the resources and such).
                XmlBlock.Parser parser = (XmlBlock.Parser)set;
                AssetManager.applyStyle(mTheme, defStyleAttr, defStyleRes, parser != null ? parser.mParseState : 0, attrs, array.mData, array.mIndices);

                array.mTheme = this;
                array.mXml = parser;

                /*if (false)
                {
                    int[] data = array.mData;

                    Debug.WriteLine("Attributes:");
                    String s = "  Attrs:";
                    int i;
                    for (i = 0; i < set.getAttributeCount(); i++)
                    {
                        s = s + " " + set.getAttributeName(i);
                        int id = set.getAttributeNameResource(i);
                        if (id != 0)
                        {
                            s = s + "(0x" + Integer.toHexString(id) + ")";
                        }
                        s = s + "=" + set.getAttributeValue(i);
                    }
                    Debug.WriteLine(s);
                    s = "  Found:";
                    TypedValue value = new TypedValue();
                    for (i = 0; i < attrs.length; i++)
                    {
                        int d = i * AssetManager.STYLE_NUM_ENTRIES;
                        value.type = data[d + AssetManager.STYLE_TYPE];
                        value.data = data[d + AssetManager.STYLE_DATA];
                        value.assetCookie = data[d + AssetManager.STYLE_ASSET_COOKIE];
                        value.resourceId = data[d + AssetManager.STYLE_RESOURCE_ID];
                        s = s + " 0x" + Integer.toHexString(attrs[i])
                            + "=" + value;
                    }
                    Debug.WriteLine(s);
                }

                return array;
            }


            public virtual TypedArray resolveAttributes(int[] values, int[] attrs)
            {
                int len = attrs.Length;
                if (values == null || len != values.Length)
                {
                    throw new ArgumentException("Base attribute values must the same length as attrs");
                }

                TypedArray array = TypedArray.obtain(mResources, len);
                //AssetManager.resolveAttrs(mTheme, 0, 0, values, attrs, array.mData, array.mIndices);
                array.mTheme = this;
                array.mXml = null;

                return array;
            }


            /*public virtual bool resolveAttribute(int resid, TypedValue outValue, bool resolveRefs)
            {
                bool got = mAssets.getThemeValue(mTheme, resid, outValue, resolveRefs);

                if (false)
                {
                    Debug.WriteLine("resolveAttribute #" + resid.ToString("X") + " got=" + got + ", type=0x" + outValue.type.ToString("X") + ", data=0x" + outValue.data.ToString("X"));
                }

                return got;
            }


            public int[] getAllAttributes()
            {
                return mAssets.getStyleAttributes(getAppliedStyleResId());
            }


            public Resources getResources()
            {
                return mResources;
            }


            /*public Drawable getDrawable(int id)
            {
                return mResources.getDrawable(id, this);
            }*/


        /*public void dump(int priority, string tag, string prefix)
        {
            AssetManager.dumpTheme(mTheme, priority, tag, prefix);
        }*/

        /*protected void finalize()
        {
            super.finalize();
            mAssets.releaseTheme(mTheme);
        }

        public Theme(Resources res)
        {
            mResources = res;
            mAssets = mResources.mAssets;
            mTheme = mAssets.createTheme();
        }

        private AssetManager mAssets;
        private long mTheme;
        private Resources mResources;


        private int mThemeResId = 0;


        private string mKey = "";

        // Needed by layoutlib.
        long getNativeTheme()
        {
            return mTheme;
        }

        int getAppliedStyleResId()
        {
            return mThemeResId;
        }

        string getKey()
        {
            return mKey;
        }

        /*private string getResourceNameFromHexString(string hexString)
        {
            return getResourceName(int.Parse(hexString, System.Globalization.NumberStyles.HexNumber));
            //return getResourceName(int.Parse(hexString, 16));
        }


//@ViewDebug.ExportedProperty(category = "theme", hasAdjacentMapping = true)
        public String[] getTheme()
        {
            string[] themeData = mKey.Split(' ');
            string[] themes = new String[themeData.Length * 2];
            string theme;
            bool forced;

            for (int i = 0, j = themeData.Length - 1; i < themes.Length; i += 2, --j)
            {
                theme = themeData[j];
                forced = theme.EndsWith("!");
                themes[i] = forced ?
                        getResourceNameFromHexString(theme.Substring(0, theme.Length - 1)) :
                        getResourceNameFromHexString(theme);
                themes[i + 1] = forced ? "forced" : "not forced";
            }
            return themes;
        }

    }*/

        protected Resources()
        {
            mMetrics = new DisplayMetrics();
            mAssets = new AssetManager();
            //mTheme = new Theme(this);
        }

    }
}
