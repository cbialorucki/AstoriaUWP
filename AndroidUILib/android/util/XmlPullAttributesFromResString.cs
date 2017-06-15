using AndroidInteropLib.android.content;
using AndroidInteropLib.com.android._internal.util;
using AndroidInteropLib.org.xmlpull.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    class XmlPullAttributesFromResString : AttributeSet
    {
        public XmlPullAttributesFromResString(XmlPullParser parser, Context c)
        {
            mParser = parser;
            mContext = c;
        }

        public int getAttributeCount()
        {
            return mParser.getAttributeCount();
        }

        public string getAttributeName(int index)
        {
            return mParser.getAttributeName(index);
        }

        public string getAttributeValue(int index)
        {
            return mParser.getAttributeValue(index);
        }

        public string getAttributeValue(string nspace, string name)
        {
            return mParser.getAttributeValue(nspace, name);
        }

        public string getPositionDescription()
        {
            return mParser.getPositionDescription();
        }

        public int getAttributeNameResource(int index)
        {
            return 0;
        }

        public int getAttributeListValue(string nspace, string attribute, string[] options, int defaultValue)
        {
            return XmlUtils.convertValueToList(getAttributeValue(nspace, attribute), options, defaultValue);
        }

        public bool getAttributeBooleanValue(string nspace, string attribute, bool defaultValue)
        {
            return XmlUtils.convertValueToBoolean(getAttributeValue(nspace, attribute), defaultValue);
        }

        public int getAttributeResourceValue(string nspace, string attribute, int defaultValue)
        {
            string val = getAttributeValue(nspace, attribute);
            string[] vals = val.Split('/');
            switch(vals[0])
            {
                case "@layout":
                    val = mContext.getR().layout.get(vals[1]).ToString();
                    break;
            }

            return XmlUtils.convertValueToInt(val, defaultValue);
        }

        public int getAttributeIntValue(string nspace, string attribute, int defaultValue)
        {
            return XmlUtils.convertValueToInt(getAttributeValue(nspace, attribute), defaultValue);
        }

        public uint getAttributeUnsignedIntValue(string nspace, string attribute, uint defaultValue)
        {
            return XmlUtils.convertValueToUnsignedInt(getAttributeValue(nspace, attribute), defaultValue);
        }

        public float getAttributeFloatValue(string nspace, string attribute, float defaultValue)
        {
            string s = getAttributeValue(nspace, attribute);
            if (s != null)
            {
                return float.Parse(s);
            }
            return defaultValue;
        }

        public int getAttributeListValue(int index, string[] options, int defaultValue)
        {
            return XmlUtils.convertValueToList(getAttributeValue(index), options, defaultValue);
        }

        public bool getAttributeBooleanValue(int index, bool defaultValue)
        {
            return XmlUtils.convertValueToBoolean(getAttributeValue(index), defaultValue);
        }

        public int getAttributeResourceValue(int index, int defaultValue)
        {
            return XmlUtils.convertValueToInt(getAttributeValue(index), defaultValue);
        }

        public int getAttributeIntValue(int index, int defaultValue)
        {
            return XmlUtils.convertValueToInt(getAttributeValue(index), defaultValue);
        }

        public uint getAttributeUnsignedIntValue(int index, uint defaultValue)
        {
            return XmlUtils.convertValueToUnsignedInt(getAttributeValue(index), defaultValue);
        }

        public float getAttributeFloatValue(int index, float defaultValue)
        {
            string s = getAttributeValue(index);
            if (s != null)
            {
                return float.Parse(s);
            }
            return defaultValue;
        }

        public string getIdAttribute()
        {
            return getAttributeValue(null, "id");
        }

        public string getClassAttribute()
        {
            return getAttributeValue(null, "class");
        }

        public int getIdAttributeResourceValue(int defaultValue)
        {
            return getAttributeResourceValue(null, "id", defaultValue);
        }

        public int getStyleAttribute()
        {
            return getAttributeResourceValue(null, "style", 0);
        }

        /*package*/
        XmlPullParser mParser;
        Context mContext;
    }
}
