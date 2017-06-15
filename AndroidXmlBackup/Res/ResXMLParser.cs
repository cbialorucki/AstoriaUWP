// Copyright (c) 2012 Markus Jarderot
// Copyright (c) 2016 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using System.Collections.Generic;
using System.IO;
using AndroidXml.Utils;

namespace AndroidXml.Res
{
    public class ResXMLParser
    {
        #region XmlParserEventCode enum

        public enum XmlParserEventCode
        {
            NOT_STARTED,
            BAD_DOCUMENT,
            START_DOCUMENT,
            END_DOCUMENT,
            CLOSED,

            START_NAMESPACE = ResourceType.RES_XML_START_NAMESPACE_TYPE,
            END_NAMESPACE = ResourceType.RES_XML_END_NAMESPACE_TYPE,
            START_TAG = ResourceType.RES_XML_START_ELEMENT_TYPE,
            END_TAG = ResourceType.RES_XML_END_ELEMENT_TYPE,
            TEXT = ResourceType.RES_XML_CDATA_TYPE
        }

        #endregion

        private readonly IEnumerator<XmlParserEventCode> _parserIterator;

        private readonly Stream _source;
        private List<ResXMLTree_attribute> _attributes;
        private object _currentExtension;
        private ResXMLTree_node _currentNode;
        private XmlParserEventCode _eventCode;
        private ResReader _reader;
        private ResResourceMap _resourceMap;
        private ResStringPool _strings;

        public ResXMLParser(Stream source)
        {
            _source = source;
            _reader = new ResReader(_source);
            _eventCode = XmlParserEventCode.NOT_STARTED;
            _parserIterator = ParserIterator().GetEnumerator();
        }

        public ResStringPool Strings
        {
            get { return _strings; }
        }

        public ResResourceMap ResourceMap
        {
            get { return _resourceMap; }
        }

        public XmlParserEventCode EventCode
        {
            get { return _eventCode; }
        }

        public uint? CommentID
        {
            get { return _currentNode == null ? null : _currentNode.Comment.Index; }
        }

        public string Comment
        {
            get { return this.GetString(CommentID); }
        }

        public uint? LineNumber
        {
            get { return _currentNode == null ? (uint?) null : _currentNode.LineNumber; }
        }

        public uint? NamespacePrefixID
        {
            get
            {
                var namespaceExt = _currentExtension as ResXMLTree_namespaceExt;
                return namespaceExt == null ? null : namespaceExt.Prefix.Index;
            }
        }

        public string NamespacePrefix
        {
            get { return this.GetString(NamespacePrefixID); }
        }

        public uint? NamespaceUriID
        {
            get
            {
                var namespaceExt = _currentExtension as ResXMLTree_namespaceExt;
                return namespaceExt == null ? null : namespaceExt.Uri.Index;
            }
        }

        public string NamespaceUri
        {
            get { return this.GetString(NamespaceUriID); }
        }

        public uint? CDataID
        {
            get
            {
                var cdataExt = _currentExtension as ResXMLTree_cdataExt;
                return cdataExt == null ? null : cdataExt.Data.Index;
            }
        }

        public string CData
        {
            get { return this.GetString(CDataID); }
        }

        public uint? ElementNamespaceID
        {
            get
            {
                var attrExt = _currentExtension as ResXMLTree_attrExt;
                if (attrExt != null) return attrExt.Namespace.Index;
                var endElementExt = _currentExtension as ResXMLTree_endElementExt;
                if (endElementExt != null) return endElementExt.Namespace.Index;
                return null;
            }
        }

        public string ElementNamespace
        {
            get { return this.GetString(ElementNamespaceID); }
        }

        public uint? ElementNameID
        {
            get
            {
                var attrExt = _currentExtension as ResXMLTree_attrExt;
                if (attrExt != null) return attrExt.Name.Index;
                var endElementExt = _currentExtension as ResXMLTree_endElementExt;
                if (endElementExt != null) return endElementExt.Name.Index;
                return null;
            }
        }

        public string ElementName
        {
            get { return this.GetString(ElementNameID); }
        }

        public uint? ElementIdIndex
        {
            get
            {
                var attrExt = _currentExtension as ResXMLTree_attrExt;
                if (attrExt != null) return attrExt.IdIndex;
                return null;
            }
        }

        public AttributeInfo ElementId
        {
            get { return GetAttribute(ElementIdIndex); }
        }

        public uint? ElementClassIndex
        {
            get
            {
                var attrExt = _currentExtension as ResXMLTree_attrExt;
                if (attrExt != null) return attrExt.ClassIndex;
                return null;
            }
        }

        public AttributeInfo ElementClass
        {
            get { return GetAttribute(ElementClassIndex); }
        }

        public uint? ElementStyleIndex
        {
            get
            {
                var attrExt = _currentExtension as ResXMLTree_attrExt;
                if (attrExt != null) return attrExt.StyleIndex;
                return null;
            }
        }

        public AttributeInfo ElementStyle
        {
            get { return GetAttribute(ElementStyleIndex); }
        }

        public uint AttributeCount
        {
            get { return _attributes == null ? 0 : (uint) _attributes.Count; }
        }

        public void Restart()
        {
            throw new NotSupportedException();
        }

        public XmlParserEventCode Next()
        {
            if (_parserIterator.MoveNext())
            {
                _eventCode = _parserIterator.Current;
                return _parserIterator.Current;
            }
            _eventCode = XmlParserEventCode.END_DOCUMENT;
            return _eventCode;
        }

        internal string GetString(uint? index)
        {
            if (index == null) return "";
            if (index < this.ResourceMap.ResouceIds.Count)
            {
                uint identifier = this.ResourceMap.ResouceIds[(int)index];
                if (PublicValuesReader.Values.ContainsKey(identifier))
                {
                    return PublicValuesReader.Values[identifier];
                }
            }
            return Strings.GetString(index);
        }

        public string GetString(ResStringPool_ref reference)
        {
            return this.GetString(reference.Index);
        }

        private void ClearState()
        {
            _currentNode = null;
            _currentExtension = null;
            _attributes = null;
        }

        private IEnumerable<XmlParserEventCode> ParserIterator()
        {
            while (true)
            {
                ClearState();

                if(_reader.BaseStream.Position >= _reader.BaseStream.Length)
                {
                    // If we're at the end of the file, stop reading chunks.
                    // Don't try to catch an EndOfStreamException - this way,
                    // we avoid an exception being created.
                    break;
                }

                ResChunk_header header;
                try
                {
                    header = _reader.ReadResChunk_header();
                }
                catch (EndOfStreamException)
                {
                    // Keep this just in case.
                    break;
                }

                var subStream = new BoundedStream(_reader.BaseStream, header.Size - 8);
                var subReader = new ResReader(subStream);
                switch (header.Type)
                {
                    case ResourceType.RES_XML_TYPE:
                        yield return XmlParserEventCode.START_DOCUMENT;
                        _reader = subReader; // Bound whole file
                        continue; // Don't skip content
                    case ResourceType.RES_STRING_POOL_TYPE:
                        ResStringPool_header stringPoolHeader = subReader.ReadResStringPool_header(header);
                        _strings = subReader.ReadResStringPool(stringPoolHeader);
                        break;
                    case ResourceType.RES_XML_RESOURCE_MAP_TYPE:
                        ResResourceMap resourceMap = subReader.ReadResResourceMap(header);
                        _resourceMap = resourceMap;
                        break;
                    case ResourceType.RES_XML_START_NAMESPACE_TYPE:
                        _currentNode = subReader.ReadResXMLTree_node(header);
                        _currentExtension = subReader.ReadResXMLTree_namespaceExt();
                        yield return XmlParserEventCode.START_NAMESPACE;
                        break;
                    case ResourceType.RES_XML_END_NAMESPACE_TYPE:
                        _currentNode = subReader.ReadResXMLTree_node(header);
                        _currentExtension = subReader.ReadResXMLTree_namespaceExt();
                        yield return XmlParserEventCode.END_NAMESPACE;
                        break;
                    case ResourceType.RES_XML_START_ELEMENT_TYPE:
                        _currentNode = subReader.ReadResXMLTree_node(header);
                        ResXMLTree_attrExt attrExt = subReader.ReadResXMLTree_attrExt();
                        _currentExtension = attrExt;

                        _attributes = new List<ResXMLTree_attribute>();
                        for (int i = 0; i < attrExt.AttributeCount; i++)
                        {
                            _attributes.Add(subReader.ReadResXMLTree_attribute());
                        }
                        yield return XmlParserEventCode.START_TAG;
                        break;
                    case ResourceType.RES_XML_END_ELEMENT_TYPE:
                        _currentNode = subReader.ReadResXMLTree_node(header);
                        _currentExtension = subReader.ReadResXMLTree_endElementExt();
                        yield return XmlParserEventCode.END_TAG;
                        break;
                    case ResourceType.RES_XML_CDATA_TYPE:
                        _currentNode = subReader.ReadResXMLTree_node(header);
                        _currentExtension = subReader.ReadResXMLTree_cdataExt();
                        yield return XmlParserEventCode.TEXT;
                        break;
                    default:
#if !CORECLR
                        Console.WriteLine("Warning: Skipping chunk of type {0} (0x{1:x4})",
                                          header.Type, (int) header.Type);
#endif
                        break;
                }
                byte[] junk = subStream.ReadFully();
                if (junk.Length > 0)
                {
#if !CORECLR
                    Console.WriteLine("Warning: Skipping {0} bytes at the end of a {1} (0x{2:x4}) chunk.",
                                      junk.Length, header.Type, (int) header.Type);
#endif
                }
            }
        }

        public AttributeInfo GetAttribute(uint? index)
        {
            if (index == null || _attributes == null) return null;
            if (index >= _attributes.Count) throw new ArgumentOutOfRangeException("index");
            ResXMLTree_attribute attr = _attributes[(int) index];
            return new AttributeInfo(this, attr);
        }

        public uint? IndexOfAttribute(string ns, string attribute)
        {
            uint? nsID = _strings.IndexOfString(ns);
            uint? nameID = _strings.IndexOfString(attribute);
            if (nameID == null) return null;
            uint index = 0;
            foreach (ResXMLTree_attribute attr in _attributes)
            {
                if (attr.Namespace.Index == nsID && attr.Name.Index == nameID)
                {
                    return index;
                }
                index++;
            }
            return null;
        }

        public void Close()
        {
            if (_eventCode == XmlParserEventCode.CLOSED) return;
            _eventCode = XmlParserEventCode.CLOSED;
            _reader.Dispose();
        }

        #region Nested type: AttributeInfo

        public class AttributeInfo
        {
            private readonly ResXMLParser _parser;

            public AttributeInfo(ResXMLParser parser, ResXMLTree_attribute attribute)
            {
                _parser = parser;
                TypedValue = attribute.TypedValue;
                ValueStringID = attribute.RawValue.Index;
                NameID = attribute.Name.Index;
                NamespaceID = attribute.Namespace.Index;
            }

            public uint? NamespaceID { get; private set; }

            public string Namespace
            {
                get { return _parser.GetString(NamespaceID); }
            }

            public uint? NameID { get; private set; }

            public string Name
            {
                get { return _parser.GetString(NameID); }
            }

            public uint? ValueStringID { get; private set; }

            public string ValueString
            {
                get { return _parser.GetString(ValueStringID); }
            }

            public Res_value TypedValue { get; private set; }
        }

        #endregion
    }
}