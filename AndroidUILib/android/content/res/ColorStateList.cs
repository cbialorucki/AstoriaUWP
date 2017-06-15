using AndroidInteropLib.android.graphics;
using AndroidInteropLib.android.util;
using AndroidInteropLib.org.xmlpull.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content.res
{
    public class ColorStateList
    {
        private int[][] mStateSpecs; // must be parallel to mColors
        private int[] mColors;      // must be parallel to mStateSpecs
        private uint mDefaultColor = 0xffff0000;

        private static int[][] EMPTY = new int[][] { new int[0] };
        //private static SparseArray<WeakReference<ColorStateList>> sCache = new SparseArray<WeakReference<ColorStateList>>();

        private ColorStateList() { }

        /**
         * Creates a ColorStateList that returns the specified mapping from
         * states to colors.
         */
        public ColorStateList(int[][] states, int[] colors)
        {
            mStateSpecs = states;
            mColors = colors;

            if (states.Length > 0)
            {
                mDefaultColor = (uint)colors[0];

                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i].Length == 0)
                    {
                        mDefaultColor = (uint)colors[i];
                    }
                }
            }
        }

        /**
         * Creates or retrieves a ColorStateList that always returns a single color.
         */
        /*public static ColorStateList valueOf(int color)
        {
            // TODO: should we collect these eventually?
            lock(sCache)
            {
                WeakReference<ColorStateList> ref1 = sCache.get(color);

                ColorStateList csl1;
                ref1.TryGetTarget(out csl1);

                ColorStateList csl = ref1 != null ? csl1 : null;
                if (csl != null)
                {
                    return csl;
                }

                csl = new ColorStateList(EMPTY, new int[] { color });
                sCache.put(color, new WeakReference<ColorStateList>(csl));
                return csl;
            }
        }*/

        /**
         * Create a ColorStateList from an XML document, given a set of {@link Resources}.
         */
        public static ColorStateList createFromXml(Resources r, XmlPullParser parser)
        {
            AttributeSet attrs = Xml.asAttributeSet(parser);

            int type;
            while ((type=parser.next()) != XmlPullParser.START_TAG && type != XmlPullParser.END_DOCUMENT)
            {

            }

            if (type != XmlPullParser.START_TAG)
            {
                throw new Exception("No start tag found");
            }

            return createFromXmlInner(r, parser, attrs);
        }

        /**
         * Create from inside an XML document. Called on a parser positioned at a
         * tag in an XML document, tries to create a ColorStateList from that tag.
         *
         * @throws XmlPullParserException if the current tag is not &lt;selector>
         * @return A color state list for the current tag.
         */
        private static ColorStateList createFromXmlInner(Resources r, XmlPullParser parser, AttributeSet attrs)
        {
            ColorStateList colorStateList;
            string name = parser.getName();
            if (name.Equals("selector"))
            {
                colorStateList = new ColorStateList();
            }
            else
            {
                throw new Exception(parser.getPositionDescription() + ": invalid drawable tag " + name);
            }

            //colorStateList.inflate(r, parser, attrs);
            return colorStateList;
        }

        /**
         * Creates a new ColorStateList that has the same states and
         * colors as this one but where each color has the specified alpha value
         * (0-255).
         */
        public ColorStateList withAlpha(int alpha)
        {
            int[] colors = new int[mColors.Length];
            int len = colors.Length;
            for (int i = 0; i < len; i++)
            {
                colors[i] = (mColors[i] & 0xFFFFFF) | (alpha << 24);
            }

            return new ColorStateList(mStateSpecs, colors);
        }

        /**
         * Indicates whether this color state list contains more than one state spec
         * and will change color based on state.
         *
         * @return True if this color state list changes color based on state, false
         *         otherwise.
         * @see #getColorForState(int[], int)
         */
        public bool isStateful()
        {
            return mStateSpecs.Length > 1;
        }

        /**
         * Indicates whether this color state list is opaque, which means that every
         * color returned from {@link #getColorForState(int[], int)} has an alpha
         * value of 255.
         *
         * @return True if this color state list is opaque.
         */
        public bool isOpaque()
        {
            int n = mColors.Length;
            for (int i = 0; i < n; i++)
            {
                if (Color.alpha(mColors[i]) != 0xFF)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Return the color associated with the given set of {@link android.view.View} states.
         *
         * @param stateSet an array of {@link android.view.View} states
         * @param defaultColor the color to return if there's not state spec in this
         * {@link ColorStateList} that matches the stateSet.
         *
         * @return the color associated with that set of states in this {@link ColorStateList}.
         */
        /*public int getColorForState(int[] stateSet, int defaultColor)
        {
            int setLength = mStateSpecs.Length;
            for (int i = 0; i < setLength; i++)
            {
                int[] stateSpec = mStateSpecs[i];
                if (StateSet.stateSetMatches(stateSpec, stateSet))
                {
                    return mColors[i];
                }
            }
            return defaultColor;
        }*/

        /**
         * Return the default color in this {@link ColorStateList}.
         *
         * @return the default color in this {@link ColorStateList}.
         */
        public uint getDefaultColor()
        {
            return mDefaultColor;
        }

        /**
         * Return the states in this {@link ColorStateList}.
         * @return the states in this {@link ColorStateList}
         * @hide
         */
        public int[][] getStates()
        {
            return mStateSpecs;
        }

        /**
         * Return the colors in this {@link ColorStateList}.
         * @return the colors in this {@link ColorStateList}
         * @hide
         */
        public int[] getColors()
        {
            return mColors;
        }

        /**
         * If the color state list does not already have an entry matching the
         * specified state, prepends a state set and color pair to a color state
         * list.
         * <p>
         * This is a workaround used in TimePicker and DatePicker until we can
         * add support for theme attributes in ColorStateList.
         *
         * @param colorStateList the source color state list
         * @param state the state to prepend
         * @param color the color to use for the given state
         * @return a new color state list, or the source color state list if there
         *         was already a matching state set
         *
         * @hide Remove when we can support theme attributes.
         */
        public static ColorStateList addFirstIfMissing(ColorStateList colorStateList, int state, int color)
        {
            int[][] inputStates = colorStateList.getStates();
            for (int i = 0; i < inputStates.Length; i++)
            {
                int[] inputState = inputStates[i];
                for (int j = 0; j < inputState.Length; j++)
                {
                    if (inputState[j] == state)
                    {
                        return colorStateList;
                    }
                }
            }

            int[][] outputStates = new int[inputStates.Length + 1][];
            System.Array.Copy(inputStates, 0, outputStates, 1, inputStates.Length);
            outputStates[0] = new int[] { state };

            int[] inputColors = colorStateList.getColors();
            int[] outputColors = new int[inputColors.Length + 1];
            System.Array.Copy(inputColors, 0, outputColors, 1, inputColors.Length);
            outputColors[0] = color;

            return new ColorStateList(outputStates, outputColors);
        }

        public string toString()
        {
            return "string";
            //return "ColorStateList{" + "mStateSpecs=" + Arrays.deepToString(mStateSpecs) + "mColors=" + Arrays.toString(mColors) + "mDefaultColor=" + mDefaultColor + '}';
        }

        public int describeContents()
        {
            return 0;
        }

        /*public void writeToParcel(Parcel dest, int flags)
        {
            int N = mStateSpecs.Length;
            dest.writeInt(N);
            for (int i = 0; i < N; i++)
            {
                dest.writeIntArray(mStateSpecs[i]);
            }
            dest.writeIntArray(mColors);
        }*/

        /*public static Parcelable.Creator<ColorStateList> CREATOR = new Parcelable.Creator<ColorStateList>() {
        public ColorStateList[] newArray(int size)
        {
            return new ColorStateList[size];
        }

        public ColorStateList createFromParcel(Parcel source)
        {
            int N = source.readInt();
            int[][] stateSpecs = new int[N][];
            for (int i = 0; i < N; i++)
            {
                stateSpecs[i] = source.createIntArray();
            }
            int[] colors = source.createIntArray();

            return new ColorStateList(stateSpecs, colors);
        }
    };*/

    }
}
