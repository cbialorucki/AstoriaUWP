using AndroidInteropLib.android.util;
using AndroidInteropLib.com.android._internal.util;
using AndroidInteropLib.org.xmlpull.v1;
using System;
using AndroidXml;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content.res
{
    /*public class XmlBlock
    {
        private static bool DEBUG=false;

        public XmlBlock(byte[] data)
        {
            mAssets = null;
            mNative = nativeCreate(data, 0, data.Length);
            mStrings = new StringBlock(AXR);//new StringBlock(nativeGetStringBlock(mNative), false);
        }

        public XmlBlock(byte[] data, int offset, int size)
        {
            mAssets = null;
            mNative = nativeCreate(data, offset, size);
            mStrings = new StringBlock(AXR); //new StringBlock(nativeGetStringBlock(mNative), false);
        }

        public void close()
        {
            lock(this)
            {
                if (mOpen)
                {
                    mOpen = false;
                    decOpenCountLocked();
                }
            }
        }

        private void decOpenCountLocked()
        {
            mOpenCount--;
            if (mOpenCount == 0)
            {
                nativeDestroy(mNative);
                if (mAssets != null)
                {
                    mAssets.xmlBlockGone(GetHashCode());
                }
            }
        }

        public XmlResourceParser newParser()
        {
            lock(this)
            {
                if (mNative != 0)
                {
                    return new Parser(nativeCreateParseState(mNative), this);
                }
                return null;
            }
        }


        public class Parser : XmlResourceParser
        {
            public bool DEBUG = false;
            public Parser(long parseState, XmlBlock block)
            {
                #if DEBUG
                DEBUG = true;
                #endif

                mParseState = parseState;
                mBlock = block;
                block.mOpenCount++;
            }

            public override void setFeature(string name, bool state)
            {
                if (FEATURE_PROCESS_NAMESPACES.Equals(name) && state) 
                {
                    return;
                }
                if (FEATURE_REPORT_NAMESPACE_ATTRIBUTES.Equals(name) && state)
                {
                    return;
                }

                throw new Exception("Unsupported feature: " + name);
            }

            public override bool getFeature(string name)
            {
                if (FEATURE_PROCESS_NAMESPACES.Equals(name))
                {
                    return true;
                }
                if (FEATURE_REPORT_NAMESPACE_ATTRIBUTES.Equals(name))
                {
                    return true;
                }
                return false;
            }

            public override void setProperty(string name, object value)
            {
                throw new Exception("setProperty() not supported");
            }

            public override object getProperty(string name)
            {
                return null;
            }

            /*public void setInput(Reader in)
            {
                throw new XmlPullParserException("setInput() not supported");
            }

            public void setInput(InputStream inputStream, string inputEncoding)
            {
                throw new XmlPullParserException("setInput() not supported");
            }*/

            /*public override void defineEntityReplacementText(string entityName, string replacementText)
            {
                throw new Exception("defineEntityReplacementText() not supported");
            }
            public override string getNamespacePrefix(int pos)
            {
                throw new XmlPullParserException("getNamespacePrefix() not supported");
            }
            public override string getInputEncoding()
            {
                return null;
            }
            public override string getNamespace(string prefix)
            {
                throw new Exception("getNamespace() not supported");
            }
            public override int getNamespaceCount(int depth)
            {
              throw new XmlPullParserException("getNamespaceCount() not supported");
            }
            public override string getPositionDescription()
            {
                return "Binary XML file line #" + getLineNumber();
            }
            public override string getNamespaceUri(int pos)
            {
                throw new XmlPullParserException("getNamespaceUri() not supported");
            }
            public override int getColumnNumber()
            {
                return -1;
            }
            public override int getDepth()
            {
                return mDepth;
            }

            public override string getText()
            {
                int id = mBlock.nativeGetText(mParseState);
                return id >= 0 ? mBlock.mStrings.get(id).tostring() : null;
            }
            public override int getLineNumber()
            {
                return mBlock.nativeGetLineNumber(mParseState);
            }
            public override int getEventType()
            {
                return mEventType;
            }
            public override bool isWhitespace()
            {
                // whitespace was stripped by aapt.
                return false;
            }

            public override string getPrefix()
            {
                throw new Exception("getPrefix not supported");
            }

            public override string getTextCharacters(int[] holderForStartAndLength)
            {
                string txt = getText();
                char[] chars = null;
                if (txt != null)
                {
                    holderForStartAndLength[0] = 0;
                    holderForStartAndLength[1] = txt.Length;
                    chars = new char[txt.Length];
                    chars = txt.ToCharArray();
                    //txt.getChars(0, txt.Length, chars, 0);
                }

                return new string(chars);
            }

            public override string getNamespace()
            {
                int id = mBlock.nativeGetNamespace(mParseState);
                return id >= 0 ? mBlock.mStrings.get(id).tostring() : "";
            }

            public override string getName()
            {
                int id = mBlock.nativeGetName(mParseState);
                return id >= 0 ? mBlock.mStrings.get(id).tostring() : null;
            }

            public override string getAttributeNamespace(int index)
            {
                int id = mBlock.nativeGetAttributeNamespace(mParseState, index);
                if (DEBUG) Debug.WriteLine("getAttributeNamespace of " + index + " = " + id);
                if (id >= 0)
                    return mBlock.mStrings.get(id).tostring();
                else if (id == -1)
                    return "";

                throw new IndexOutOfRangeException(index.ToString());
            }

            public override string getAttributeName(int index)
            {
                int id = mBlock.nativeGetAttributeName(mParseState, index);
                if (DEBUG)
                    Debug.WriteLine("getAttributeName of " + index + " = " + id);
                if (id >= 0)
                    return mBlock.mStrings.get(id).tostring();

                throw new IndexOutOfRangeException(index.ToString());
            }

            public override string getAttributePrefix(int index)
            {
                throw new Exception("getAttributePrefix not supported");
            }

            public override bool isEmptyElementTag()
            {
                // XXX Need to detect this.
                return false;
            }

            public override int getAttributeCount()
            {
                return mEventType == START_TAG ? mBlock.nativeGetAttributeCount(mParseState) : -1;
            }

            public override string getAttributeValue(int index)
            {
                int id = nativeGetAttributestringValue(mParseState, index);
                if (DEBUG)
                    Debug.WriteLine("getAttributeValue of " + index + " = " + id);
                if (id >= 0)
                    return mBlock.mStrings.get(id).tostring();

                // May be some other type...  check and try to convert if so.
                int t = mBlock.nativeGetAttributeDataType(mParseState, index);
                if (t == TypedValue.TYPE_NULL)
                {
                    throw new IndexOutOfRangeException(index.ToString());
                }

                int v = mBlock.nativeGetAttributeData(mParseState, index);
                return TypedValue.coerceToString(t, v);
            }

            public override string getAttributeType(int index)
            {
                return "CDATA";
            }

            public override bool isAttributeDefault(int index)
            {
                return false;
            }

            public override int nextToken()
            {
                return next();
            }

            public override string getAttributeValue(string nspace, string name)
            {
                int idx = nativeGetAttributeIndex(mParseState, nspace, name);
                if (idx >= 0)
                {
                    if (DEBUG) Debug.WriteLine("getAttributeName of " + nspace + ":" + name + " index = " + idx);
                    if (DEBUG) Debug.WriteLine("Namespace=" + getAttributeNamespace(idx) + "Name=" + getAttributeName(idx) + ", Value=" + getAttributeValue(idx));
                    return getAttributeValue(idx);
                }

                return null;
            }

            public override int next()
            {
                if (!mStarted)
                {
                    mStarted = true;
                    return START_DOCUMENT;
                }

                if (mParseState == 0)
                {
                    return END_DOCUMENT;
                }

                int ev = mBlock.nativeNext(mParseState);
                if (mDecNextDepth)
                {
                    mDepth--;
                    mDecNextDepth = false;
                }

                switch (ev)
                {
                    case START_TAG:
                        mDepth++;
                        break;
                    case END_TAG:
                        mDecNextDepth = true;
                        break;
                }

                mEventType = ev;

                if (ev == END_DOCUMENT)
                {
                    // Automatically close the parse when we reach the end of
                    // a document, since the standard XmlPullParser interface
                    // doesn't have such an API so most clients will leave us
                    // dangling.
                    close();
                }

                return ev;
            }

            public override void require(int type, string nspace, string name) 
            {
                if (type != getEventType() || (nspace != null && !nspace.Equals(getNamespace())) || (name != null && !name.Equals(getName())))
                    throw new XmlPullParserException( "expected "+ TYPES[type]+getPositionDescription());
            }

            public override string nextText()
            {
                if(getEventType() != START_TAG)
                {
                    throw new XmlPullParserException(getPositionDescription() + ": parser must be on START_TAG to read next text", this, null);
                }

                int eventType = next();

                if(eventType == TEXT)
                {
                    string result = getText();
                    eventType = next();
                    if(eventType != END_TAG)
                    {
                        throw new XmlPullParserException(getPositionDescription() + ": event TEXT it must be immediately followed by END_TAG", this, null);
                    }

                    return result;
                }

                else if(eventType == END_TAG)
                {
                    return "";
                }

                else
                {
                    throw new XmlPullParserException(getPositionDescription()+ ": parser must be on START_TAG or TEXT to read text", this, null);
                }
            }

            public override int nextTag()
            {
                int eventType = next();

                if(eventType == TEXT && isWhitespace())
                {   // skip whitespace
                    eventType = next();
                }

                if (eventType != START_TAG && eventType != END_TAG)
                {
                    throw new XmlPullParserException(getPositionDescription() + ": expected start or end tag", this, null);
                }

                return eventType;
            }
    
            public int getAttributeNameResource(int index)
            {
                return mBlock.nativeGetAttributeResource(mParseState, index);
            }

            public int getAttributeListValue(string nspace, string attribute, string[] options, int defaultValue)
            {
                int idx = nativeGetAttributeIndex(mParseState, nspace, attribute);
                if (idx >= 0)
                {
                    return getAttributeListValue(idx, options, defaultValue);
                }

                return defaultValue;
            }

            public bool getAttributeBooleanValue(string nspace, string attribute, bool defaultValue)
            {
                int idx = nativeGetAttributeIndex(mParseState, nspace, attribute);
                if (idx >= 0)
                {
                    return getAttributeBooleanValue(idx, defaultValue);
                }

                return defaultValue;
            }
            public int getAttributeResourceValue(string nspace, string attribute, int defaultValue)
            {
                int idx = nativeGetAttributeIndex(mParseState, nspace, attribute);
                if (idx >= 0)
                {
                    return getAttributeResourceValue(idx, defaultValue);
                }
                return defaultValue;
            }

            public int getAttributeIntValue(string nspace, string attribute, int defaultValue)
            {
                int idx = nativeGetAttributeIndex(mParseState, nspace, attribute);
                if (idx >= 0)
                {
                    return getAttributeIntValue(idx, defaultValue);
                }
                return defaultValue;
            }

            public int getAttributeUnsignedIntValue(string nspace, string attribute, int defaultValue)
            {
                int idx = nativeGetAttributeIndex(mParseState, nspace, attribute);

                if (idx >= 0)
                {
                    return getAttributeUnsignedIntValue(idx, defaultValue);
                }

                return defaultValue;
            }   
            public float getAttributeFloatValue(string nspace, string attribute, float defaultValue)
            {
                int idx = nativeGetAttributeIndex(mParseState, nspace, attribute);
                if (idx >= 0)
                {
                    return getAttributeFloatValue(idx, defaultValue);
                }

                return defaultValue;
            }

            public int getAttributeListValue(int idx, string[] options, int defaultValue)
            {
                int t = mBlock.nativeGetAttributeDataType(mParseState, idx);
                int v = mBlock.nativeGetAttributeData(mParseState, idx);
                if (t == TypedValue.TYPE_STRING)
                {
                    return XmlUtils.convertValueToList(mBlock.mStrings.get(v), options, defaultValue);
                }
                return v;
            }

            public bool getAttributeBooleanValue(int idx, bool defaultValue)
            {
                int t = mBlock.nativeGetAttributeDataType(mParseState, idx);
                // Note: don't attempt to convert any other types, because
                // we want to count on aapt doing the conversion for us.
                if (t >= TypedValue.TYPE_FIRST_INT && t <= TypedValue.TYPE_LAST_INT)
                {
                    return mBlock.nativeGetAttributeData(mParseState, idx) != 0;
                }

                return defaultValue;
            }

            public int getAttributeResourceValue(int idx, int defaultValue)
            {
                int t = mBlock.nativeGetAttributeDataType(mParseState, idx);
                // Note: don't attempt to convert any other types, because
                // we want to count on aapt doing the conversion for us.
                if (t == TypedValue.TYPE_REFERENCE)
                {
                    return mBlock.nativeGetAttributeData(mParseState, idx);
                }
                return defaultValue;
            }
            public int getAttributeIntValue(int idx, int defaultValue)
            {
                int t = mBlock.nativeGetAttributeDataType(mParseState, idx);
                // Note: don't attempt to convert any other types, because
                // we want to count on aapt doing the conversion for us.
                if (t >= TypedValue.TYPE_FIRST_INT &&
                    t <= TypedValue.TYPE_LAST_INT)
                {
                    return mBlock.nativeGetAttributeData(mParseState, idx);
                }
                return defaultValue;
            }

            public int getAttributeUnsignedIntValue(int idx, int defaultValue)
            {
                int t = mBlock.nativeGetAttributeDataType(mParseState, idx);
                // Note: don't attempt to convert any other types, because
                // we want to count on aapt doing the conversion for us.
                if (t >= TypedValue.TYPE_FIRST_INT &&
                    t <= TypedValue.TYPE_LAST_INT)
                {
                    return mBlock.nativeGetAttributeData(mParseState, idx);
                }
                return defaultValue;
            }

            public float getAttributeFloatValue(int idx, float defaultValue)
            {
                int t = mBlock.nativeGetAttributeDataType(mParseState, idx);
                // Note: don't attempt to convert any other types, because
                // we want to count on aapt doing the conversion for us.
                if (t == TypedValue.TYPE_FLOAT)
                {
                    return ticomware.interop.Util.intBitsToFloat(mBlock.nativeGetAttributeData(mParseState, idx));
                }
                throw new Exception("not a float!");
            }

            public string getIdAttribute()
            {
                int id = nativeGetIdAttribute(mParseState);
                return id >= 0 ? mBlock.mStrings.get(id).tostring() : null;
            }
            public string getClassAttribute()
            {
                int id = nativeGetClassAttribute(mParseState);
                return id >= 0 ? mBlock.mStrings.get(id).tostring() : null;
            }

            public int getIdAttributeResourceValue(int defaultValue)
            {
                //todo: create and use native method
                return getAttributeResourceValue(null, "id", defaultValue);
            }

            public int getStyleAttribute()
            {
                return nativeGetStyleAttribute(mParseState);
            }

            public override void close()
            {
                lock(mBlock) {
                    if (mParseState != 0)
                    {
                        mBlock.nativeDestroyParseState(mParseState);
                        mParseState = 0;
                        mBlock.decOpenCountLocked();
                    }
                }
            }

            protected void finalize()
            {
                close();
            }

            //package
            public string getPooledString(int id)
            {
                return mBlock.mStrings.get(id);
            }

            //package
            public long mParseState;
            private XmlBlock mBlock;
            private bool mStarted = false;
            private bool mDecNextDepth = false;
            private int mDepth = 0;
            private int mEventType = START_DOCUMENT;
    }

        protected void finalize()
        {
            close();
        }

        /**
         * Create from an existing xml block native object.  This is
         * -extremely- dangerous -- only use it if you absolutely know what you
         *  are doing!  The given native object must exist for the entire lifetime
         *  of this newly creating XmlBlock.
         */
        /*public XmlBlock(AssetManager assets, long xmlBlock)
        {
            mAssets = assets;
            mNative = xmlBlock;
            mStrings = new StringBlock(AXR); //new StringBlock(nativeGetStringBlock(xmlBlock), false);
        }

        private AssetManager mAssets;
        private long mNative;
        //package
        StringBlock mStrings;
        private bool mOpen = true;
        private int mOpenCount = 1;
        private AndroidXml.AndroidXmlReader AXR;

        private long nativeCreate(byte[] data, int offset, int size)
        {
            if(data == null)
            {
                throw new ArgumentNullException();
            }

            int bLen = data.Length;

            if(offset < 0 || offset >= bLen || size < 0 || size > bLen || (offset + size) > bLen)
            {
                throw new IndexOutOfRangeException();
            }

            //Custom stuff
            AXR = new AndroidXmlReader(new System.IO.MemoryStream(data, offset, size));

            //as far as i can tell, mNative just needs to not be 0 and uniquely identify AXR
            return AXR.GetHashCode();
        }

        private long nativeGetStringBlock(long obj)
        {
            //Ported from C++
            
        }

        private long nativeCreateParseState(long obj)
        {
            AXR._parser.Restart();
            return AXR.GetHashCode();
        }
        //package
        private int nativeNext(long state)
        {

            //ResXMLParser* st = (ResXMLParser*)token;
            return AXR._parser.Next();
        }

        private int nativeGetNamespace(long state)
        {
            if(AXR == null)
            {
                return -1;
            }

            return Convert.ToInt32(AXR._parser.ElementNamespaceID);
        }
        //package
        private int nativeGetName(long state)
        {
            if (AXR == null)
            {
                return -1;
            }

            return Convert.ToInt32(AXR._parser.ElementNameID);
        }

        private int nativeGetText(long state)
        {
            if (AXR == null)
            {
                return -1;
            }

            return Convert.ToInt32(AXR._parser.);
        }

        private int nativeGetLineNumber(long state)
        {
            return Convert.ToInt32(AXR._parser.LineNumber);
            //return (int)RXP.LineNumber;
        }

        private int nativeGetAttributeCount(long state)
        {
            return AXR.AttributeCount;
        }

        private int nativeGetAttributeNamespace(long state, int idx)
        {
            //return AXR.name
            //AXR._parser.
        }

        private int nativeGetAttributeName(long state, int idx)
        {
            //needs C int representation of a string
            //return AXR.GetAttribute(idx);
        }

        private int nativeGetAttributeResource(long state, int idx)
        {

        }

        private int nativeGetAttributeDataType(long state, int idx)
        {

        }

        private int nativeGetAttributeData(long state, int idx)
        {

        }

        private static int nativeGetAttributestringValue(long state, int idx)
        {

        }

        private static int nativeGetIdAttribute(long state)
        {

        }

        private static int nativeGetClassAttribute(long state)
        {

        }

        private static int nativeGetStyleAttribute(long state)
        {

        }

        private static int nativeGetAttributeIndex(long state, string nspace, string name)
        {

        }
        private void nativeDestroyParseState(long state)
        {

        }

        private void nativeDestroy(long obj)
        {
            //Usually used to destroy AXR, lets do this. More Saving. More Doing. That's the Power of the Home Depot.
            if(AXR.GetHashCode().Equals(obj))
            {
                AXR.Dispose();
            }
        }
    }*/
}
