using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace DalvikUWPCSharp.Disassembly.AXMLPort
{
    class AXmlResourceParser : XmlPullParser
    {
        /////////////////////////////////// data

        /*
         * All values are essentially indices, e.g. m_name is
         * an index of name in m_strings.
         */

        private IntReader m_reader;
        private bool m_operational = false;

        private StringBlock m_strings;
        private int[] m_resourceIDs;
        private NamespaceStack m_namespaces = new NamespaceStack();

        private bool m_decreaseDepth;

        private int m_event;
        private int m_lineNumber;
        private int m_name;
        private int m_namespaceUri;
        private int[] m_attributes;
        private int m_idAttribute;
        private int m_classAttribute;
        private int m_styleAttribute;

        private static readonly string E_NOT_SUPPORTED = "Method is not supported.";

        private static readonly int
            ATTRIBUTE_IX_NAMESPACE_URI = 0,
            ATTRIBUTE_IX_NAME = 1,
            ATTRIBUTE_IX_VALUE_string = 2,
            ATTRIBUTE_IX_VALUE_TYPE = 3,
            ATTRIBUTE_IX_VALUE_DATA = 4,
            ATTRIBUTE_LENGHT = 5;

        private static readonly int
            CHUNK_AXML_FILE = 0x00080003,
            CHUNK_RESOURCEIDS = 0x00080180,
            CHUNK_XML_FIRST = 0x00100100,
            CHUNK_XML_START_NAMESPACE = 0x00100100,
            CHUNK_XML_END_NAMESPACE = 0x00100101,
            CHUNK_XML_START_TAG = 0x00100102,
            CHUNK_XML_END_TAG = 0x00100103,
            CHUNK_XML_TEXT = 0x00100104,
            CHUNK_XML_LAST = 0x00100104;

        public AXmlResourceParser()
        {
            resetEventInfo();
        }

        public void open(FileStream stream)
        {
            close();
            if (stream != null)
            {
                m_reader = new IntReader(stream, false);
            }
        }

        public void close()
        {
            if (!m_operational)
            {
                return;
            }
            m_operational = false;
            m_reader.close();
            m_reader = null;
            m_strings = null;
            m_resourceIDs = null;
            m_namespaces.reset();
            resetEventInfo();
        }

        /////////////////////////////////// iteration

        public override int next()
        {
            if (m_reader == null)
            {
                throw new XmlPullParserException("Parser is not opened.", this, null);
            }
            try
            {
                doNext();
                return m_event;
            }
            catch (IOException e)
            {
                close();
                throw e;
            }
        }

        public override int nextToken()
        {
            return next();
        }

        public override int nextTag()
        {
            int eventType = next();
            if (eventType == TEXT && isWhitespace())
            {
                eventType = next();
            }
            if (eventType != START_TAG && eventType != END_TAG)
            {
                throw new XmlPullParserException("Expected start or end tag.", this, null);
            }
            return eventType;
        }

        public override string nextText()
        {
            if (getEventType() != START_TAG)
            {
                throw new XmlPullParserException("Parser must be on START_TAG to read next text.", this, null);
            }
            int eventType = next();
            if (eventType == TEXT)
            {
                string result = getText();
                eventType = next();
                if (eventType != END_TAG)
                {
                    throw new XmlPullParserException("Event TEXT must be immediately followed by END_TAG.", this, null);
                }
                return result;
            }
            else if (eventType == END_TAG)
            {
                return "";
            }
            else
            {
                throw new XmlPullParserException("Parser must be on START_TAG or TEXT to read text.", this, null);
            }
        }

        public override void require(int type, string nspace, string name)
        {
            if (type != getEventType() || (nspace != null && !nspace.Equals(getNamespace())) || (name != null && !name.Equals(getName())))
            {
                throw new XmlPullParserException(TYPES[type] + " is expected.", this, null);
            }
        }

        public override int getDepth()
        {
            return m_namespaces.getDepth() - 1;
        }

        public override int getEventType()
        {
            return m_event;
        }

        public override int getLineNumber()
        {
            return m_lineNumber;
        }

        public override string getName()
        {
            if (m_name == -1 || (m_event != START_TAG && m_event != END_TAG))
            {
                return null;
            }
            return m_strings.getstring(m_name);
        }

        public override string getText()
        {
            if (m_name == -1 || m_event != TEXT)
            {
                return null;
            }
            return m_strings.getstring(m_name);
        }

        public override char[] getTextCharacters(int[] holderForStartAndLength)
        {
            string text = getText();
            if (text == null)
            {
                return null;
            }
            holderForStartAndLength[0] = 0;
            holderForStartAndLength[1] = text.Length;
            char[] chars = new char[text.Length];
            chars = text.ToCharArray(0, text.Length);
            return chars;
        }

        public override string getNamespace()
        {
            return m_strings.getstring(m_namespaceUri);
        }

        public override string getPrefix()
        {
            int prefix = m_namespaces.findPrefix(m_namespaceUri);
            return m_strings.getstring(prefix);
        }

        public override string getPositionDescription()
        {
            return "XML line #" + getLineNumber();
        }

        public override int getNamespaceCount(int depth)
        {
            return m_namespaces.getAccumulatedCount(depth);
        }

        public override string getNamespacePrefix(int pos)
        {
            int prefix = m_namespaces.getPrefix(pos);
            return m_strings.getstring(prefix);
        }

        public override string getNamespaceUri(int pos)
        {
            int uri = m_namespaces.getUri(pos);
            return m_strings.getstring(uri);
        }

        /////////////////////////////////// attributes

        public string getClassAttribute()
        {
            if (m_classAttribute == -1)
            {
                return null;
            }
            int offset = getAttributeOffset(m_classAttribute);
            int value = m_attributes[offset + ATTRIBUTE_IX_VALUE_string];
            return m_strings.getstring(value);
        }

        public string getIdAttribute()
        {
            if (m_idAttribute == -1)
            {
                return null;
            }
            int offset = getAttributeOffset(m_idAttribute);
            int value = m_attributes[offset + ATTRIBUTE_IX_VALUE_string];
            return m_strings.getstring(value);
        }

        public int getIdAttributeResourceValue(int defaultValue)
        {
            if (m_idAttribute == -1)
            {
                return defaultValue;
            }
            int offset = getAttributeOffset(m_idAttribute);
            int valueType = m_attributes[offset + ATTRIBUTE_IX_VALUE_TYPE];
            if (valueType != TypedValue.TYPE_REFERENCE)
            {
                return defaultValue;
            }
            return m_attributes[offset + ATTRIBUTE_IX_VALUE_DATA];
        }

        public int getStyleAttribute()
        {
            if (m_styleAttribute == -1)
            {
                return 0;
            }
            int offset = getAttributeOffset(m_styleAttribute);
            return m_attributes[offset + ATTRIBUTE_IX_VALUE_DATA];
        }

        public override int getAttributeCount()
        {
            if (m_event != START_TAG)
            {
                return -1;
            }
            return m_attributes.Length / ATTRIBUTE_LENGHT;
        }

        public override string getAttributeNamespace(int index)
        {
            int offset = getAttributeOffset(index);
            int nspace = m_attributes[offset + ATTRIBUTE_IX_NAMESPACE_URI];
            if (nspace == -1)
            {
                return "";
            }
            return m_strings.getstring(nspace);
        }

        public override string getAttributePrefix(int index)
        {
            int offset = getAttributeOffset(index);
            int uri = m_attributes[offset + ATTRIBUTE_IX_NAMESPACE_URI];
            int prefix = m_namespaces.findPrefix(uri);
            if (prefix == -1)
            {
                return "";
            }
            return m_strings.getstring(prefix);
        }

        public override string getAttributeName(int index)
        {
            int offset = getAttributeOffset(index);
            int name = m_attributes[offset + ATTRIBUTE_IX_NAME];
            if (name == -1)
            {
                return "";
            }
            return m_strings.getstring(name);
        }

        public int getAttributeNameResource(int index)
        {
            int offset = getAttributeOffset(index);
            int name = m_attributes[offset + ATTRIBUTE_IX_NAME];
            if (m_resourceIDs == null ||
                name < 0 || name >= m_resourceIDs.Length)
            {
                return 0;
            }
            return m_resourceIDs[name];
        }

        public int getAttributeValueType(int index)
        {
            int offset = getAttributeOffset(index);
            return m_attributes[offset + ATTRIBUTE_IX_VALUE_TYPE];
        }

        public int getAttributeValueData(int index)
        {
            int offset = getAttributeOffset(index);
            return m_attributes[offset + ATTRIBUTE_IX_VALUE_DATA];
        }

        public override string getAttributeValue(int index)
        {
            int offset = getAttributeOffset(index);
            int valueType = m_attributes[offset + ATTRIBUTE_IX_VALUE_TYPE];
            if (valueType == TypedValue.TYPE_STRING)
            {
                int valuestring = m_attributes[offset + ATTRIBUTE_IX_VALUE_string];
                return m_strings.getstring(valuestring);
            }
            int valueData = m_attributes[offset + ATTRIBUTE_IX_VALUE_DATA];
            return "";//TypedValue.coerceTostring(valueType,valueData);
        }

        public bool getAttributeboolValue(int index, bool defaultValue)
        {
            return getAttributeIntValue(index, defaultValue ? 1 : 0) != 0;
        }

        public float getAttributeFloatValue(int index, float defaultValue)
        {
            int offset = getAttributeOffset(index);
            int valueType = m_attributes[offset + ATTRIBUTE_IX_VALUE_TYPE];
            if (valueType == TypedValue.TYPE_FLOAT)
            {
                int valueData = m_attributes[offset + ATTRIBUTE_IX_VALUE_DATA];
                
                //return Float.intBitsToFloat(valueData);
                byte[] intBytes = BitConverter.GetBytes(valueData);
                float f = BitConverter.ToSingle(intBytes, 0);
                return f;
                
            }
            return defaultValue;
        }

        public int getAttributeIntValue(int index, int defaultValue)
        {
            int offset = getAttributeOffset(index);
            int valueType = m_attributes[offset + ATTRIBUTE_IX_VALUE_TYPE];
            if (valueType >= TypedValue.TYPE_FIRST_INT &&
                valueType <= TypedValue.TYPE_LAST_INT)
            {
                return m_attributes[offset + ATTRIBUTE_IX_VALUE_DATA];
            }
            return defaultValue;
        }

        public int getAttributeUnsignedIntValue(int index, int defaultValue)
        {
            return getAttributeIntValue(index, defaultValue);
        }

        public int getAttributeResourceValue(int index, int defaultValue)
        {
            int offset = getAttributeOffset(index);
            int valueType = m_attributes[offset + ATTRIBUTE_IX_VALUE_TYPE];
            if (valueType == TypedValue.TYPE_REFERENCE)
            {
                return m_attributes[offset + ATTRIBUTE_IX_VALUE_DATA];
            }
            return defaultValue;
        }

        public override string getAttributeValue(string nspace, string attribute)
        {
            int index = findAttribute(nspace, attribute);
            if (index == -1)
            {
                return null;
            }
            return getAttributeValue(index);
        }

        public bool getAttributeboolValue(string nspace, string attribute, bool defaultValue)
        {
            int index = findAttribute(nspace, attribute);
            if (index == -1)
            {
                return defaultValue;
            }
            return getAttributeboolValue(index, defaultValue);
        }

        public float getAttributeFloatValue(string nspace, string attribute, float defaultValue)
        {
            int index = findAttribute(nspace, attribute);
            if (index == -1)
            {
                return defaultValue;
            }
            return getAttributeFloatValue(index, defaultValue);
        }

        public int getAttributeIntValue(string nspace, string attribute, int defaultValue)
        {
            int index = findAttribute(nspace, attribute);
            if (index == -1)
            {
                return defaultValue;
            }
            return getAttributeIntValue(index, defaultValue);
        }

        public int getAttributeUnsignedIntValue(string nspace, string attribute, int defaultValue)
        {
            int index = findAttribute(nspace, attribute);
            if (index == -1)
            {
                return defaultValue;
            }
            return getAttributeUnsignedIntValue(index, defaultValue);
        }

        public int getAttributeResourceValue(string nspace, string attribute, int defaultValue)
        {
            int index = findAttribute(nspace, attribute);
            if (index == -1)
            {
                return defaultValue;
            }
            return getAttributeResourceValue(index, defaultValue);
        }

        public int getAttributeListValue(int index, string[] options, int defaultValue)
        {
            // TODO implement
            return 0;
        }

        public int getAttributeListValue(string nspace, string attribute, string[] options, int defaultValue)
        {
            // TODO implement
            return 0;
        }

        public override string getAttributeType(int index)
        {
            return "CDATA";
        }

        public override bool isAttributeDefault(int index)
        {
            return false;
        }

        /////////////////////////////////// dummies

        public override void setInput(Stream stream, string inputEncoding)
        {
            throw new XmlPullParserException(E_NOT_SUPPORTED);
        }

        public override void setInput(StreamReader reader)
        {
            throw new XmlPullParserException(E_NOT_SUPPORTED);
        }

        public override string getInputEncoding()
        {
            return null;
        }

        public override int getColumnNumber()
        {
            return -1;
        }

        public override bool isEmptyElementTag()
        {
            return false;
        }

        public override bool isWhitespace()
        {
            return false;
        }

        public override void defineEntityReplacementText(string entityName, string replacementText)
        {
            throw new XmlPullParserException(E_NOT_SUPPORTED);
        }

        public override string getNamespace(string prefix)
        {
            throw new NotSupportedException(E_NOT_SUPPORTED);
        }

        public override object getProperty(string name)
        {
            return null;
        }
        public override void setProperty(string name, object value)
        {
            throw new XmlPullParserException(E_NOT_SUPPORTED);
        }

        public override bool getFeature(string feature)
        {
            return false;
        }
        public override void setFeature(string name, bool value)
        {
            throw new XmlPullParserException(E_NOT_SUPPORTED);
        }

        StringBlock getstrings()
        {
            return m_strings;
        }

        ///////////////////////////////////

        private int getAttributeOffset(int index)
        {
            if (m_event != START_TAG)
            {
                throw new IndexOutOfRangeException("Current event is not START_TAG.");
            }
            int offset = index * 5;
            if (offset >= m_attributes.Length)
            {
                throw new IndexOutOfRangeException("Invalid attribute index (" + index + ").");
            }
            return offset;
        }

        private int findAttribute(string nspace, string attribute)
        {
            if (m_strings == null || attribute == null)
            {
                return -1;
            }
            int name = m_strings.find(attribute);
            if (name == -1)
            {
                return -1;
            }
            int uri = (nspace != null) ?
                  m_strings.find(nspace) :
                -1;
            for (int o = 0; o != m_attributes.Length; ++o)
            {
                if (name == m_attributes[o + ATTRIBUTE_IX_NAME] &&
                    (uri == -1 || uri == m_attributes[o + ATTRIBUTE_IX_NAMESPACE_URI]))
                {
                    return o / ATTRIBUTE_LENGHT;
                }
            }
            return -1;
        }

        private void resetEventInfo()
        {
            m_event = -1;
            m_lineNumber = -1;
            m_name = -1;
            m_namespaceUri = -1;
            m_attributes = null;
            m_idAttribute = -1;
            m_classAttribute = -1;
            m_styleAttribute = -1;
        }

        private void doNext()
        {
            // Delayed initialization.
            if (m_strings == null)
            {
                ChunkUtil.readCheckType(m_reader, CHUNK_AXML_FILE);
                /*chunkSize*/
                m_reader.skipInt();
                m_strings = StringBlock.read(m_reader);
                m_namespaces.increaseDepth();
                m_operational = true;
            }

            if (m_event == END_DOCUMENT)
            {
                return;
            }

            int tempEvent = m_event;
            resetEventInfo();

            while (true)
            {
                if (m_decreaseDepth)
                {
                    m_decreaseDepth = false;
                    m_namespaces.decreaseDepth();
                }

                // Fake END_DOCUMENT event.
                if (tempEvent == END_TAG && m_namespaces.getDepth() == 1 && m_namespaces.getCurrentCount() == 0)
                {
                    m_event = END_DOCUMENT;
                    break;
                }

                int chunkType;
                if (tempEvent == START_DOCUMENT)
                {
                    // Fake event, see CHUNK_XML_START_TAG handler.
                    chunkType = CHUNK_XML_START_TAG;
                }
                else {
                    chunkType = m_reader.readInt();
                }

                if (chunkType == CHUNK_RESOURCEIDS)
                {
                    int chunkSize = m_reader.readInt();
                    if (chunkSize < 8 || (chunkSize % 4) != 0)
                    {
                        throw new IOException("Invalid resource ids size (" + chunkSize + ").");
                    }
                    m_resourceIDs = m_reader.readIntArray(chunkSize / 4 - 2);
                    continue;
                }

                if (chunkType < CHUNK_XML_FIRST || chunkType > CHUNK_XML_LAST)
                {
                    throw new IOException("Invalid chunk type (" + chunkType + ").");
                }

                // Fake START_DOCUMENT event.
                if (chunkType == CHUNK_XML_START_TAG && tempEvent == -1)
                {
                    m_event = START_DOCUMENT;
                    break;
                }

                // Common header.
                /*chunkSize*/
                m_reader.skipInt();
                int lineNumber = m_reader.readInt();
                /*0xFFFFFFFF*/
                m_reader.skipInt();

                if (chunkType == CHUNK_XML_START_NAMESPACE ||
                    chunkType == CHUNK_XML_END_NAMESPACE)
                {
                    if (chunkType == CHUNK_XML_START_NAMESPACE)
                    {
                        int prefix = m_reader.readInt();
                        int uri = m_reader.readInt();
                        m_namespaces.push(prefix, uri);
                    }
                    else {
                        /*prefix*/
                        m_reader.skipInt();
                        /*uri*/
                        m_reader.skipInt();
                        m_namespaces.pop();
                    }
                    continue;
                }

                m_lineNumber = lineNumber;

                if (chunkType == CHUNK_XML_START_TAG)
                {
                    m_namespaceUri = m_reader.readInt();
                    m_name = m_reader.readInt();
                    /*flags?*/
                    m_reader.skipInt();
                    int attributeCount = m_reader.readInt();
                    m_idAttribute = (unchecked((int)(uint)attributeCount >> 16)) - 1;
                    attributeCount &= 0xFFFF;
                    m_classAttribute = m_reader.readInt();
                    m_styleAttribute = (unchecked((int)(uint)m_classAttribute >> 16)) - 1;
                    m_classAttribute = (m_classAttribute & 0xFFFF) - 1;
                    m_attributes = m_reader.readIntArray(attributeCount * ATTRIBUTE_LENGHT);
                    for (int i = ATTRIBUTE_IX_VALUE_TYPE; i < m_attributes.Length;)
                    {
                        m_attributes[i] = (unchecked((int)(uint)m_attributes[i] >> 24));
                        i += ATTRIBUTE_LENGHT;
                    }
                    m_namespaces.increaseDepth();
                    m_event = START_TAG;
                    break;
                }

                if (chunkType == CHUNK_XML_END_TAG)
                {
                    m_namespaceUri = m_reader.readInt();
                    m_name = m_reader.readInt();
                    m_event = END_TAG;
                    m_decreaseDepth = true;
                    break;
                }

                if (chunkType == CHUNK_XML_TEXT)
                {
                    m_name = m_reader.readInt();
                    /*?*/
                    m_reader.skipInt();
                    /*?*/
                    m_reader.skipInt();
                    m_event = TEXT;
                    break;
                }
            }
        }


        ///////////////////////////////////////////// implementation

        /**
         * Namespace stack, holds prefix+uri pairs, as well as
         *  depth information.
         * All information is stored in one int[] array.
         * Array consists of depth frames:
         *  Data=DepthFrame*;
         *  DepthFrame=Count+[Prefix+Uri]*+Count;
         *  Count='count of Prefix+Uri pairs';
         * Yes, count is stored twice, to enable bottom-up traversal.
         * increaseDepth adds depth frame, decreaseDepth removes it.
         * push/pop operations operate only in current depth frame.
         * decreaseDepth removes any remaining (not pop'ed) namespace pairs.
         * findXXX methods search all depth frames starting 
         * from the last namespace pair of current depth frame.
         * All functions that operate with int, use -1 as 'invalid value'.
         * 
         * !! functions expect 'prefix'+'uri' pairs, not 'uri'+'prefix' !!
         * 
         */
        public class NamespaceStack
        {

            private int[] m_data;
            private int m_dataLength;
            private int m_count;
            private int m_depth;

            public NamespaceStack()
            {
                m_data = new int[32];
            }

            public void reset()
            {
                m_dataLength = 0;
                m_count = 0;
                m_depth = 0;
            }

            public int getTotalCount()
            {
                return m_count;
            }

            public int getCurrentCount()
            {
                if (m_dataLength == 0)
                {
                    return 0;
                }
                int offset = m_dataLength - 1;
                return m_data[offset];
            }

            public int getAccumulatedCount(int depth)
            {
                if (m_dataLength == 0 || depth < 0)
                {
                    return 0;
                }
                if (depth > m_depth)
                {
                    depth = m_depth;
                }
                int accumulatedCount = 0;
                int offset = 0;
                for (; depth != 0; --depth)
                {
                    int count = m_data[offset];
                    accumulatedCount += count;
                    offset += (2 + count * 2);
                }
                return accumulatedCount;
            }

            public void push(int prefix, int uri)
            {
                if (m_depth == 0)
                {
                    increaseDepth();
                }
                ensureDataCapacity(2);
                int offset = m_dataLength - 1;
                int count = m_data[offset];
                m_data[offset - 1 - count * 2] = count + 1;
                m_data[offset] = prefix;
                m_data[offset + 1] = uri;
                m_data[offset + 2] = count + 1;
                m_dataLength += 2;
                m_count += 1;
            }

            public bool pop(int prefix, int uri)
            {
                if (m_dataLength == 0)
                {
                    return false;
                }
                int offset = m_dataLength - 1;
                int count = m_data[offset];
                for (int i = 0, o = offset - 2; i != count; ++i, o -= 2)
                {
                    if (m_data[o] != prefix || m_data[o + 1] != uri)
                    {
                        continue;
                    }
                    count -= 1;
                    if (i == 0)
                    {
                        m_data[o] = count;
                        o -= (1 + count * 2);
                        m_data[o] = count;
                    }
                    else {
                        m_data[offset] = count;
                        offset -= (1 + 2 + count * 2);
                        m_data[offset] = count;
                        System.Array.Copy(
                            m_data, o + 2,
                            m_data, o,
                            m_dataLength - o);
                    }
                    m_dataLength -= 2;
                    m_count -= 1;
                    return true;
                }
                return false;
            }

            public bool pop()
            {
                if (m_dataLength == 0)
                {
                    return false;
                }
                int offset = m_dataLength - 1;
                int count = m_data[offset];
                if (count == 0)
                {
                    return false;
                }
                count -= 1;
                offset -= 2;
                m_data[offset] = count;
                offset -= (1 + count * 2);
                m_data[offset] = count;
                m_dataLength -= 2;
                m_count -= 1;
                return true;
            }

            public int getPrefix(int index)
            {
                return get(index, true);
            }

            public int getUri(int index)
            {
                return get(index, false);
            }

            public int findPrefix(int uri)
            {
                return find(uri, false);
            }

            public int findUri(int prefix)
            {
                return find(prefix, true);
            }

            public int getDepth()
            {
                return m_depth;
            }

            public void increaseDepth()
            {
                ensureDataCapacity(2);
                int offset = m_dataLength;
                m_data[offset] = 0;
                m_data[offset + 1] = 0;
                m_dataLength += 2;
                m_depth += 1;
            }
            public void decreaseDepth()
            {
                if (m_dataLength == 0)
                {
                    return;
                }
                int offset = m_dataLength - 1;
                int count = m_data[offset];
                if ((offset - 1 - count * 2) == 0)
                {
                    return;
                }
                m_dataLength -= 2 + count * 2;
                m_count -= count;
                m_depth -= 1;
            }

            private void ensureDataCapacity(int capacity)
            {
                int available = (m_data.Length - m_dataLength);
                if (available > capacity)
                {
                    return;
                }
                int newLength = (m_data.Length + available) * 2;
                int[] newData = new int[newLength];
                System.Array.Copy(m_data, 0, newData, 0, m_dataLength);
                m_data = newData;
            }

            private int find(int prefixOrUri, bool prefix)
            {
                if (m_dataLength == 0)
                {
                    return -1;
                }
                int offset = m_dataLength - 1;
                for (int i = m_depth; i != 0; --i)
                {
                    int count = m_data[offset];
                    offset -= 2;
                    for (; count != 0; --count)
                    {
                        if (prefix)
                        {
                            if (m_data[offset] == prefixOrUri)
                            {
                                return m_data[offset + 1];
                            }
                        }
                        else {
                            if (m_data[offset + 1] == prefixOrUri)
                            {
                                return m_data[offset];
                            }
                        }
                        offset -= 2;
                    }
                }
                return -1;
            }

            private int get(int index, bool prefix)
            {
                if (m_dataLength == 0 || index < 0)
                {
                    return -1;
                }
                int offset = 0;
                for (int i = m_depth; i != 0; --i)
                {
                    int count = m_data[offset];
                    if (index >= count)
                    {
                        index -= count;
                        offset += (2 + count * 2);
                        continue;
                    }
                    offset += (1 + index * 2);
                    if (!prefix)
                    {
                        offset += 1;
                    }
                    return m_data[offset];
                }
                return -1;
            }
        }
    }
}

/////////////////////////////////// package-visible

//	final void fetchAttributes(int[] styleableIDs,TypedArray result) {
//		result.resetIndices();
//		if (m_attributes==null || m_resourceIDs==null) {
//			return;
//		}
//		bool needstrings=false;
//		for (int i=0,e=styleableIDs.length;i!=e;++i) {
//			int id=styleableIDs[i];
//			for (int o=0;o!=m_attributes.length;o+=ATTRIBUTE_LENGHT) {
//				int name=m_attributes[o+ATTRIBUTE_IX_NAME];
//				if (name>=m_resourceIDs.length ||
//					m_resourceIDs[name]!=id)
//				{
//					continue;
//				}
//				int valueType=m_attributes[o+ATTRIBUTE_IX_VALUE_TYPE];
//				int valueData;
//				int assetCookie;
//				if (valueType==TypedValue.TYPE_string) {
//					valueData=m_attributes[o+ATTRIBUTE_IX_VALUE_string];
//					assetCookie=-1;
//					needstrings=true;
//				} else {
//					valueData=m_attributes[o+ATTRIBUTE_IX_VALUE_DATA];
//					assetCookie=0;
//				}
//				result.addValue(i,valueType,valueData,assetCookie,id,0);
//			}
//		}
//		if (needstrings) {
//			result.setstrings(m_strings);
//		}
//	}

