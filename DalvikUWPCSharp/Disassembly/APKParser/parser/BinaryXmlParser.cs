using DalvikUWPCSharp.Disassembly.APKParser.bean;
using DalvikUWPCSharp.Disassembly.APKParser.exception;
using DalvikUWPCSharp.Disassembly.APKParser.struct_;
using DalvikUWPCSharp.Disassembly.APKParser.struct_.resource;
using DalvikUWPCSharp.Disassembly.APKParser.struct_.xml;
using DalvikUWPCSharp.Disassembly.APKParser.utils;
using DalvikUWPCSharp.Disassembly.APKParser.utils.xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    public class BinaryXmlParser
    {
        /**
     * By default the data buffer Chunks is buffer little-endian byte order both at runtime and when stored buffer
     * files.
     */
        private ByteOrder byteOrder = ByteOrder.LITTLE_ENDIAN;
        private StringPool stringPool;
        // some attribute name stored by resource id
        private string[] resourceMap;
        private ByteBuffer buffer;
        private XmlStreamer xmlStreamer;
        private ResourceTable resourceTable;
        /**
         * default locale.
         */
        private CultureInfo locale = Locales.any;

        public BinaryXmlParser(ByteBuffer buffer, ResourceTable resourceTable)
        {
            this.buffer = buffer.duplicate();
            //this.buffer.order(byteOrder).RunSynchronously(); // slow :(
            this.resourceTable = resourceTable;
        }

        /**
         * Parse binary xml.
         */
        public async Task parse()
        {
            ChunkHeader chunkHeader = await readChunkHeader();
            if (chunkHeader == null)
            {
                return;
            }
            if (chunkHeader.getChunkType() != ChunkType.XML && chunkHeader.getChunkType() != ChunkType.NULL)
            {
                // notice that some apk mark xml header type as 0, really weird
                // see https://github.com/clearthesky/apk-parser/issues/49#issuecomment-256852727
                return;
            }

            // read string pool chunk
            chunkHeader = await readChunkHeader();
            if (chunkHeader == null)
            {
                return;
            }
            ParseUtils.checkChunkType(ChunkType.STRING_POOL, chunkHeader.getChunkType());
            stringPool = await ParseUtils.readStringPool(buffer, (StringPoolHeader)chunkHeader);

            // read on chunk, check if it was an optional XMLResourceMap chunk
            chunkHeader = await readChunkHeader();
            if (chunkHeader == null)
            {
                return;
            }
            if (chunkHeader.getChunkType() == ChunkType.XML_RESOURCE_MAP)
            {
                long[] resourceIds = readXmlResourceMap((XmlResourceMapHeader)chunkHeader);
                resourceMap = new string[resourceIds.Length];
                for (int i = 0; i < resourceIds.Length; i++)
                {
                    resourceMap[i] = await Attribute_.AttrIds.getString(resourceIds[i]);
                }
                chunkHeader = await readChunkHeader();
            }

            while (chunkHeader != null)
            {
                /*if (chunkHeader.chunkType == ChunkType.XML_END_NAMESPACE) {
                    break;
                }*/
                long beginPos = buffer.position();
                switch (chunkHeader.getChunkType())
                {
                    case ChunkType.XML_END_NAMESPACE:
                        XmlNamespaceEndTag xmlNamespaceEndTag = readXmlNamespaceEndTag();
                        xmlStreamer.onNamespaceEnd(xmlNamespaceEndTag);
                        break;
                    case ChunkType.XML_START_NAMESPACE:
                        XmlNamespaceStartTag namespaceStartTag = readXmlNamespaceStartTag();
                        xmlStreamer.onNamespaceStart(namespaceStartTag);
                        break;
                    case ChunkType.XML_START_ELEMENT:
                        XmlNodeStartTag xmlNodeStartTag = await readXmlNodeStartTag();
                        break;
                    case ChunkType.XML_END_ELEMENT:
                        XmlNodeEndTag xmlNodeEndTag = readXmlNodeEndTag();
                        break;
                    case ChunkType.XML_CDATA:
                        XmlCData xmlCData = await readXmlCData();
                        break;
                    default:
                        if (chunkHeader.getChunkType() >= ChunkType.XML_FIRST_CHUNK &&
                                chunkHeader.getChunkType() <= ChunkType.XML_LAST_CHUNK)
                        {
                            Buffers.skip(buffer, chunkHeader.getBodySize());
                        }
                        else {
                            throw new ParserException("Unexpected chunk type:" + chunkHeader.getChunkType());
                        }
                        break;
                }
                buffer.position((int)(beginPos + chunkHeader.getBodySize()));
                chunkHeader = await readChunkHeader();
            }
        }

        private async Task<XmlCData> readXmlCData()
        {
            XmlCData xmlCData = new XmlCData();
            int dataRef = buffer.getInt();
            if (dataRef > 0)
            {
                xmlCData.setData(stringPool.get(dataRef));
            }
            xmlCData.setTypedData(await ParseUtils.readResValue(buffer, stringPool));
            if (xmlStreamer != null)
            {
                //TODO: to know more about cdata. some cdata appears buffer xml tags
                //            String value = xmlCData.toStringValue(resourceTable, locale);
                //            xmlCData.setValue(value);
                //            xmlStreamer.onCData(xmlCData);
            }
            return xmlCData;
        }

        private XmlNodeEndTag readXmlNodeEndTag()
        {
            XmlNodeEndTag xmlNodeEndTag = new XmlNodeEndTag();
            int nsRef = buffer.getInt();
            int nameRef = buffer.getInt();
            if (nsRef > 0)
            {
                xmlNodeEndTag.setNamespace(stringPool.get(nsRef));
            }
            xmlNodeEndTag.setName(stringPool.get(nameRef));
            if (xmlStreamer != null)
            {
                xmlStreamer.onEndTag(xmlNodeEndTag);
            }
            return xmlNodeEndTag;
        }

        private async Task<XmlNodeStartTag> readXmlNodeStartTag()
        {
            int nsRef = buffer.getInt();
            int nameRef = buffer.getInt();
            XmlNodeStartTag xmlNodeStartTag = new XmlNodeStartTag();
            if (nsRef > 0)
            {
                xmlNodeStartTag.setNamespace(stringPool.get(nsRef));
            }
            xmlNodeStartTag.setName(stringPool.get(nameRef));

            // read attributes.
            // attributeStart and attributeSize are always 20 (0x14)
            int attributeStart = await Buffers.readUShort(buffer);
            int attributeSize = await Buffers.readUShort(buffer);
            int attributeCount = await Buffers.readUShort(buffer);
            int idIndex = await Buffers.readUShort(buffer);
            int classIndex = await Buffers.readUShort(buffer);
            int styleIndex = await Buffers.readUShort(buffer);

            // read attributes
            Attributes attributes = new Attributes(attributeCount);
            for (int count = 0; count < attributeCount; count++)
            {
                Attribute_ attribute = await readAttribute();
                if (xmlStreamer != null)
                {
                    string value = attribute.toStringValue(resourceTable, locale);
                    if (intAttributes.Contains(attribute.getName()) && Utils.isNumeric(value))
                    {
                        try
                        {
                            value = getFinalValueAsString(attribute.getName(), value);
                        }
                        catch
                        {
                            //ignore exception
                        }
                    }
                    attribute.setValue(value);
                    attributes.set(count, attribute);
                }
            }
            xmlNodeStartTag.setAttributes(attributes);

            if (xmlStreamer != null)
            {
                xmlStreamer.onStartTag(xmlNodeStartTag);
            }

            return xmlNodeStartTag;
        }

        private static HashSet<string> intAttributes = new HashSet<string>(new string[] { "screenOrientation", "configChanges", "windowSoftInputMode", "launchMode", "installLocation", "protectionLevel" });

            //Arrays.asList("screenOrientation", "configChanges", "windowSoftInputMode", "launchMode", "installLocation", "protectionLevel"));

        //trans int attr value to string
        private string getFinalValueAsString(string attributeName, string str)
        {
            uint value = uint.Parse(str);
            switch (attributeName)
            {
                case "screenOrientation":
                    return AttributeValues.getScreenOrientation(value);
                case "configChanges":
                    return AttributeValues.getConfigChanges(value);
                case "windowSoftInputMode":
                    return AttributeValues.getWindowSoftInputMode(value);
                case "launchMode":
                    return AttributeValues.getLaunchMode(value);
                case "installLocation":
                    return AttributeValues.getInstallLocation(value);
                case "protectionLevel":
                    return AttributeValues.getProtectionLevel(value);
                default:
                    return str;
            }
        }

        private async Task<Attribute_> readAttribute()
        {
            int nsRef = buffer.getInt();
            int nameRef = buffer.getInt();
            Attribute_ attribute = new Attribute_();
            if (nsRef > 0)
            {
                attribute.setNamespace(stringPool.get(nsRef));
            }

            attribute.setName(stringPool.get(nameRef));
            if (attribute.getName().Equals(string.Empty) && resourceMap != null && nameRef < resourceMap.Length)
            {
                // some processed apk file make the string pool value empty, if it is a xmlmap attr.
                attribute.setName(resourceMap[nameRef]);
                //TODO: how to get the namespace of attribute
            }

            int rawValueRef = buffer.getInt();
            if (rawValueRef > 0)
            {
                attribute.setRawValue(stringPool.get(rawValueRef));
            }
            ResourceValue resValue = await ParseUtils.readResValue(buffer, stringPool);
            attribute.setTypedValue(resValue);

            return attribute;
        }

        private XmlNamespaceStartTag readXmlNamespaceStartTag()
        {
            int prefixRef = buffer.getInt();
            int uriRef = buffer.getInt();
            XmlNamespaceStartTag nameSpace = new XmlNamespaceStartTag();
            if (prefixRef > 0)
            {
                nameSpace.setPrefix(stringPool.get(prefixRef));
            }
            if (uriRef > 0)
            {
                nameSpace.setUri(stringPool.get(uriRef));
            }
            return nameSpace;
        }

        private XmlNamespaceEndTag readXmlNamespaceEndTag()
        {
            int prefixRef = buffer.getInt();
            int uriRef = buffer.getInt();
            XmlNamespaceEndTag nameSpace = new XmlNamespaceEndTag();
            if (prefixRef > 0)
            {
                nameSpace.setPrefix(stringPool.get(prefixRef));
            }
            if (uriRef > 0)
            {
                nameSpace.setUri(stringPool.get(uriRef));
            }
            return nameSpace;
        }

        private long[] readXmlResourceMap(XmlResourceMapHeader chunkHeader)
        {
            int count = chunkHeader.getBodySize() / 4;
            long[] resourceIds = new long[count];
            for (int i = 0; i < count; i++)
            {
                resourceIds[i] = Buffers.readUInt(buffer);
            }
            return resourceIds;
        }


        private async Task<ChunkHeader> readChunkHeader()
        {
            // finished
            if (!buffer.hasRemaining())
            {
                return null;
            }

            long begin = buffer.position();
            int chunkType = await Buffers.readUShort(buffer);
            int headerSize = await Buffers.readUShort(buffer);
            long chunkSize = Buffers.readUInt(buffer);

            switch (chunkType)
            {
                case ChunkType.XML:
                    return new XmlHeader(chunkType, headerSize, chunkSize);
                case ChunkType.STRING_POOL:
                    StringPoolHeader stringPoolHeader = new StringPoolHeader(chunkType, headerSize, chunkSize);
                    stringPoolHeader.setStringCount(Buffers.readUInt(buffer));
                    stringPoolHeader.setStyleCount(Buffers.readUInt(buffer));
                    stringPoolHeader.setFlags(Buffers.readUInt(buffer));
                    stringPoolHeader.setStringsStart(Buffers.readUInt(buffer));
                    stringPoolHeader.setStylesStart(Buffers.readUInt(buffer));
                    buffer.position((int)(begin + headerSize));
                    return stringPoolHeader;
                case ChunkType.XML_RESOURCE_MAP:
                    buffer.position((int)(begin + headerSize));
                    return new XmlResourceMapHeader(chunkType, headerSize, chunkSize);
                case ChunkType.XML_START_NAMESPACE:
                case ChunkType.XML_END_NAMESPACE:
                case ChunkType.XML_START_ELEMENT:
                case ChunkType.XML_END_ELEMENT:
                case ChunkType.XML_CDATA:
                    XmlNodeHeader header = new XmlNodeHeader(chunkType, headerSize, chunkSize);
                    header.setLineNum((int)Buffers.readUInt(buffer));
                    header.setCommentRef((int)Buffers.readUInt(buffer));
                    buffer.position((int)(begin + headerSize));
                    return header;
                case ChunkType.NULL:
                    return new NullHeader(chunkType, headerSize, chunkSize);
                default:
                    throw new ParserException("Unexpected chunk type:" + chunkType);
            }
        }

        public void setLocale(CultureInfo locale)
        {
            if (locale != null)
            {
                this.locale = locale;
            }
        }

        public CultureInfo getLocale()
        {
            return locale;
        }

        public XmlStreamer getXmlStreamer()
        {
            return xmlStreamer;
        }

        public void setXmlStreamer(XmlStreamer xmlStreamer)
        {
            this.xmlStreamer = xmlStreamer;
        }
    }
}
