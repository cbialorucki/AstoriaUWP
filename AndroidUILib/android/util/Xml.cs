using AndroidInteropLib.android.content;
using AndroidInteropLib.org.xmlpull.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    public class Xml
    {
        public static AttributeSet asAttributeSet(XmlPullParser parser)
        {
            //return (parser instanceof AttributeSet) ?? (AttributeSet)parser: new XmlPullAttributes(parser);
            //return new XmlPullAttributes(parser);
            return new XmlPullAttributes(parser);
        }

        //This is a hack for the layout inflater.
        public static AttributeSet asAttributeSet(XmlPullParser parser, Context c)
        {
            //return (parser instanceof AttributeSet) ?? (AttributeSet)parser: new XmlPullAttributes(parser);
            //return new XmlPullAttributes(parser);
            return new XmlPullAttributesFromResString(parser, c);
        }

    }
}
