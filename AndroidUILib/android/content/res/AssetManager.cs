using AndroidInteropLib.android.util;
using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content.res
{
    public class AssetManager
    {
        public const int STYLE_NUM_ENTRIES = 6;
        public const int STYLE_TYPE = 0;
        public const int STYLE_DATA = 1;
        public const int STYLE_ASSET_COOKIE = 2;
        public const int STYLE_RESOURCE_ID = 3;
        public const int STYLE_CHANGING_CONFIGURATIONS = 4;
        public const int STYLE_DENSITY = 5;

        //private StringBlock[] mStringBlocks = null;
        private int mNumRefs = 1;
        private Dictionary<long, Exception> mRefStacks;
        private const bool DEBUG_REFS = false;


        /*public CharSequence getPooledStringForCookie(int cookie, int id)
        {
            // Cookies map to string blocks starting at 1.
            return mStringBlocks[cookie - 1].get(id);
        }*/

        public void xmlBlockGone(int id)
        {
            lock(this)
            {
                decRefsLocked(id);
            }
        }

        private void decRefsLocked(long id)
        {
            if (DEBUG_REFS && mRefStacks != null)
            {
                mRefStacks.Remove(id);
            }

            mNumRefs--;
            //System.out.println("Dec streams: mNumRefs=" + mNumRefs
            //                   + " mReleased=" + mReleased);
            if (mNumRefs == 0)
            {
                destroy();
            }
        }

        private void destroy()
        {
            //TODO: close resources
        }


        /*public static bool applyStyle(long theme, int defStyleAttr, int defStyleRes, long xmlParser, int[] inAttrs, int[] outValues, int[] outIndices)
        {

        }

        public static bool resolveAttrs(long theme, int defStyleAttr, int defStyleRes, int[] inValues, int[] inAttrs, int[] outValues, int[] outIndices)
        {

        }

        public int[] getStyleAttributes(int themeRes)
        {

        }

        public static void applyThemeStyle(long theme, int styleRes, bool force)
        {
            //TODO: Implement
        }


        public static void copyTheme(long dest, long source)
        {
            //TODO: Implement
        }

        public bool getThemeValue(long theme, int ident, TypedValue outValue, bool resolveRefs)
        {
            int block = loadThemeAttributeValue(theme, ident, outValue, resolveRefs);
            if (block >= 0)
            {
                if (outValue.type != TypedValue.TYPE_STRING)
                {
                    return true;
                }

                StringBlock[] blocks = mStringBlocks;
                if (blocks == null)
                {
                    ensureStringBlocks();
                    blocks = mStringBlocks;
                }

                outValue.string1 = blocks[block].get(outValue.data);
                return true;
            }

            return false;

        }

        public long createTheme()
        {
            lock(this)
            {
                if (!mOpen)
                {
                    throw new Exception("Assetmanager has been closed");
                }

                long res = newTheme();
                incRefsLocked(res);
                return res;
            }
        }*/


    }
}
