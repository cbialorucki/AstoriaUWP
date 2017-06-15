using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.org.xmlpull.v1
{
    public class XmlPullParserException : Exception
    {
        public XmlPullParserException()
        { }

        public XmlPullParserException(string message) : base(message)
        {

        }

        public XmlPullParserException(string message, object a, object b) : base(message)
        {

        }
    }
}
