using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.org.xmlpull.v1
{
    public abstract class XmlPullParser
    {
        public const string NO_NAMESPACE = "";
        public const int START_DOCUMENT = 0;
        public const int END_DOCUMENT = 1;
        public const int START_TAG = 2;
        public const int END_TAG = 3;
        public const int TEXT = 4;
        public const int CDSECT = 5;
        public const int ENTITY_REF = 6;
        public const int IGNORABLE_WHITESPACE = 7;
        public const int PROCESSING_INSTRUCTION = 8;
        public const int COMMENT = 9;
        public const int DOCDECL = 10;
        public static readonly string[] TYPES = { "START_DOCUMENT", "END_DOCUMENT", "START_TAG", "END_TAG", "TEXT", "CDSECT", "ENTITY_REF", "IGNORABLE_WHITESPACE", "PROCESSING_INSTRUCTION", "COMMENT", "DOCDECL" };

        public const string FEATURE_PROCESS_NAMESPACES = "http://xmlpull.org/v1/doc/features.html#process-namespaces";
        public const string FEATURE_REPORT_NAMESPACE_ATTRIBUTES = "http://xmlpull.org/v1/doc/features.html#report-namespace-prefixes";
        public const string FEATURE_PROCESS_DOCDECL = "http://xmlpull.org/v1/doc/features.html#process-docdecl";
        public const string FEATURE_VALIDATION = "http://xmlpull.org/v1/doc/features.html#validation";
        public const string ANDROID_NAMESPACE = "http://schemas.android.com/apk/res/android";


        /*string NO_NAMESPACE = "";
        int START_DOCUMENT = 0;
        int END_DOCUMENT = 1;
        int START_TAG = 2;
        int END_TAG = 3;
        int TEXT = 4;
        int CDSECT = 5;
        int ENTITY_REF = 6;
        int IGNORABLE_WHITESPACE = 7;
        int PROCESSING_INSTRUCTION = 8;
        int COMMENT = 9;
        int DOCDECL = 10;
        string[] TYPES = {"START_DOCUMENT", "END_DOCUMENT", "START_TAG", "END_TAG", "TEXT", "CDSECT", "ENTITY_REF", "IGNORABLE_WHITESPACE", "PROCESSING_INSTRUCTION", "COMMENT", "DOCDECL"};

        string FEATURE_PROCESS_NAMESPACES = "http://xmlpull.org/v1/doc/features.html#process-namespaces";
        string FEATURE_REPORT_NAMESPACE_ATTRIBUTES = "http://xmlpull.org/v1/doc/features.html#report-namespace-prefixes";
        string FEATURE_PROCESS_DOCDECL = "http://xmlpull.org/v1/doc/features.html#process-docdecl";
        string FEATURE_VALIDATION = "http://xmlpull.org/v1/doc/features.html#validation";
        string ANDROID_NAMESPACE = "http://xmlpull.org/v1/doc/features.html#validation";*/


        public abstract void setFeature(string name, bool state);
        public abstract bool getFeature(string name);
        public abstract void setProperty(string name, object value);
        public abstract object getProperty(string name);
        //void setInput(Reader in1);
        //void setInput(InputStream inputStream, string inputEncoding);
        public abstract string getInputEncoding();
        public abstract void defineEntityReplacementText(string entityName, string replacementText);
        public abstract int getNamespaceCount(int depth);
        public abstract string getNamespacePrefix(int pos);
        public abstract string getNamespaceUri(int pos);
        public abstract string getNamespace(string prefix);

        public abstract int getDepth();
        public abstract string getPositionDescription();
        public abstract int getLineNumber();
        public abstract int getColumnNumber();

        public abstract bool isWhitespace();
        public abstract string getText();
        public abstract string getTextCharacters(int[] holderForStartAndLength);

        public abstract string getNamespace();
        public abstract string getName();
        public abstract string getPrefix();


        public abstract bool isEmptyElementTag();

        // --------------------------------------------------------------------------
        // START_TAG Attributes retrieval methods


        public abstract int getAttributeCount();
        public abstract string getAttributeNamespace(int index);
        public abstract string getAttributeName(int index);
        public abstract string getAttributePrefix(int index);
        public abstract string getAttributeType(int index);
        public abstract bool isAttributeDefault(int index);
        public abstract string getAttributeValue(int index);
        public abstract string getAttributeValue(string nspace, string name);

        // --------------------------------------------------------------------------
        // actual parsing methods

        public abstract int getEventType();
        public abstract int next();
        public abstract int nextToken();

        //-----------------------------------------------------------------------------
        // utility methods to mak XML parsing easier ...


        public abstract void require(int type, string nspace, string name);
        public abstract string nextText();
        public abstract int nextTag();


        //    
        //    public void skipSubTree() throws XmlPullParserException, IOException;

    }
}
