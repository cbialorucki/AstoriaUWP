using AndroidInteropLib.android.util;
using AndroidInteropLib.org.xmlpull.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content.res
{
    public abstract class XmlResourceParser : XmlPullParser
    {
        public abstract void close();
    }
}
