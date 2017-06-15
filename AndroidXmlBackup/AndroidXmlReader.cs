// Copyright (c) 2012 Markus Jarderot
// Copyright (c) 2015-2016 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using AndroidXml.Res;
using System.Drawing;

namespace AndroidXml
{
    public class AndroidXmlReader : XmlReader
    {
        private readonly Dictionary<string, List<string>> _namespaces;
        private readonly ResXMLParser _parser;
        private readonly IEnumerator<bool> _readIterator;
        private uint? _attributeIndex;

        #region Backing fields for overriden properties

        private readonly XmlNameTable _nameTable;
        private int _depth;
        private string _localName;
        private string _namespaceUri;
        private XmlNodeType _nodeType;
        private string _prefix;
        private ReadState _readState;
        private string _value;

        #endregion

        public AndroidXmlReader(Stream source)
        {
            _parser = new ResXMLParser(source);
            _namespaces = new Dictionary<string, List<string>>();
            _nameTable = new NameTable();
            _readIterator = ReadIterator().GetEnumerator();
        }

        public ResStringPool StringPool
        {
            get { return _parser.Strings; }
        }

        private void AddNamespace(string prefix, string uri)
        {
            List<string> definitions;
            if (!_namespaces.TryGetValue(prefix, out definitions))
            {
                definitions = new List<string>();
                _namespaces[prefix] = definitions;
            }
            definitions.Add(uri);
        }

        private void RemoveNamespace(string prefix, string uri)
        {
            List<string> definitions;
            if (!_namespaces.TryGetValue(prefix, out definitions))
            {
                return;
            }
            definitions.Remove(uri);
        }

        #region Overrides of XmlReader

        /// <summary>
        /// Gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name"/>.
        /// </summary>
        /// <returns>
        /// The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned.
        /// </returns>
        /// <param name="name">The qualified name of the attribute. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> is null.</exception>
        public override string GetAttribute(string name)
        {
            return GetAttribute(name, "");
        }

        /// <summary>
        /// Gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName"/> and <see cref="P:System.Xml.XmlReader.NamespaceURI"/>.
        /// </summary>
        /// <returns>
        /// The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned. This method does not move the reader.
        /// </returns>
        /// <param name="name">The local name of the attribute. </param>
        /// <param name="namespaceURI">The namespace URI of the attribute. </param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> is null.</exception>
        public override string GetAttribute(string name, string namespaceURI)
        {
            if (_parser.EventCode != ResXMLParser.XmlParserEventCode.START_TAG) return null;
            uint? index = _parser.IndexOfAttribute(namespaceURI, name);
            if (index == null) return null;
            return GetAttribute((int) index);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified index.
        /// </summary>
        /// <returns>
        /// The value of the specified attribute. This method does not move the reader.
        /// </returns>
        /// <param name="i">The index of the attribute. The index is zero-based. (The first attribute has index 0.) </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="i"/> is out of range. It must be non-negative and less than the size of the attribute collection.</exception>
        public override string GetAttribute(int i)
        {
            if (_parser.EventCode != ResXMLParser.XmlParserEventCode.START_TAG) return null;
            ResXMLParser.AttributeInfo attr = _parser.GetAttribute(i < 0 ? null : (uint?) i);
            if (attr == null) throw new ArgumentOutOfRangeException("i");
            if (attr.ValueStringID != null)
            {
                return attr.ValueString;
            }
            return FormatValue(attr.TypedValue);
        }

        private string FormatValue(Res_value value)
        {
            Color c;
            int index0;
            switch (value.DataType)
            {
                case Res.ValueType.TYPE_STRING:
                    return _parser.GetString(value.StringValue);
                case Res.ValueType.TYPE_NULL:
                    return "null";
                case Res.ValueType.TYPE_FLOAT:
                    return value.FloatValue.ToString("g");
                case Res.ValueType.TYPE_FRACTION:
                    index0 = (int) value.ComplexFractionUnit;
                    return string.Format(
                        "{0:g}{1}", value.ComplexValue,
                        index0 < 2 ? new[] {"%", "%p"}[index0] : "?");
                case Res.ValueType.TYPE_DIMENSION:
                    index0 = (int) value.ComplexDimensionUnit;
                    return string.Format(
                        "{0:g}{1}", value.ComplexValue,
                        index0 < 6 ? new[] {"px", "dip", "sp", "pt", "in", "mm"}[index0] : "?");
                case Res.ValueType.TYPE_INT_DEC:
                    return string.Format("{0:d}", value.IntValue);
                case Res.ValueType.TYPE_INT_HEX:
                    return string.Format("0x{0:x}", value.IntValue);
                case Res.ValueType.TYPE_INT_BOOLEAN:
                    return value.IntValue == 0 ? "false" : "true";
                case Res.ValueType.TYPE_INT_COLOR_ARGB8:
                    c = value.ColorValue;
                    return string.Format("#{0:x2}{1:x2}{2:x2}{3:x2}", c.A, c.R, c.G, c.B);
                case Res.ValueType.TYPE_INT_COLOR_ARGB4:
                    c = value.ColorValue;
                    return string.Format("#{0:x1}{1:x1}{2:x1}{3:x1}", c.A/51, c.R/51, c.G/51, c.B/51);
                case Res.ValueType.TYPE_INT_COLOR_RGB8:
                    c = value.ColorValue;
                    return string.Format("#{0:x2}{1:x2}{2:x2}", c.R, c.G, c.B);
                case Res.ValueType.TYPE_INT_COLOR_RGB4:
                    c = value.ColorValue;
                    return string.Format("#{0:x1}{1:x1}{2:x1}", c.R/51, c.G/51, c.B/51);
                case Res.ValueType.TYPE_REFERENCE:
                    uint? ident = value.ReferenceValue.Ident;
                    if (ident == null) return "@undef";
                    return string.Format("@{0:x8}", ident.Value);
                default:
                    return string.Format("({0}:{1:x8})", value.DataType, value.RawData);
            }
        }

        /// <summary>
        /// Moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.Name"/>.
        /// </summary>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false, the reader's position does not change.
        /// </returns>
        /// <param name="name">The qualified name of the attribute. </param>
        public override bool MoveToAttribute(string name)
        {
            return MoveToAttribute(name, "");
        }

        /// <summary>
        /// Moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName"/> and <see cref="P:System.Xml.XmlReader.NamespaceURI"/>.
        /// </summary>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false, the reader's position does not change.
        /// </returns>
        /// <param name="name">The local name of the attribute. </param><param name="ns">The namespace URI of the attribute. </param>
        public override bool MoveToAttribute(string name, string ns)
        {
            if (_parser.EventCode != ResXMLParser.XmlParserEventCode.START_TAG) return false;
            uint? index = _parser.IndexOfAttribute(ns, name);
            if (index == null) return false;
            MoveToAttribute(index);
            return true;
        }

        /// <summary>
        /// Moves to the first attribute.
        /// </summary>
        /// <returns>
        /// true if an attribute exists (the reader moves to the first attribute); otherwise, false (the position of the reader does not change).
        /// </returns>
        public override bool MoveToFirstAttribute()
        {
            if (_parser.EventCode != ResXMLParser.XmlParserEventCode.START_TAG) return false;
            if (AttributeCount == 0) return false;
            MoveToAttribute(0);
            return true;
        }

        /// <summary>
        /// Moves to the next attribute.
        /// </summary>
        /// <returns>
        /// true if there is a next attribute; false if there are no more attributes.
        /// </returns>
        public override bool MoveToNextAttribute()
        {
            if (_parser.EventCode != ResXMLParser.XmlParserEventCode.START_TAG) return false;
            uint nextIndex = (_attributeIndex ?? 0) + 1;
            if (nextIndex >= AttributeCount) return false;
            MoveToAttribute(nextIndex);
            return true;
        }

        protected bool MoveToAttribute(uint? index)
        {
            if (_parser.EventCode != ResXMLParser.XmlParserEventCode.START_TAG) return false;
            _attributeIndex = index;
            if (index == null)
            {
                string ns = _parser.ElementNamespace;
                SetState(nodeType: XmlNodeType.Element,
                         prefix: LookupPrefix(ns),
                         localName: _parser.ElementName,
                         namespaceUri: ns);
            }
            else
            {
                ResXMLParser.AttributeInfo attr = _parser.GetAttribute(index);
                string ns = attr.Namespace;
                SetState(
                    nodeType: XmlNodeType.Attribute,
                    prefix: LookupPrefix(ns),
                    localName: attr.Name,
                    namespaceUri: ns,
                    value: attr.ValueStringID != null ? attr.ValueString : FormatValue(attr.TypedValue));
            }
            return true;
        }

        private void SetState(ReadState readState = ReadState.Interactive, XmlNodeType nodeType = XmlNodeType.None,
                              string prefix = "", string localName = "", string namespaceUri = "", string value = "")
        {
            _readState = readState;
            _nodeType = nodeType;
            _prefix = prefix;
            _localName = localName;
            _namespaceUri = namespaceUri;
            _value = value;
        }

        /// <summary>
        /// Moves to the element that contains the current attribute node.
        /// </summary>
        /// <returns>
        /// true if the reader is positioned on an attribute (the reader moves to the element that owns the attribute); false if the reader is not positioned on an attribute (the position of the reader does not change).
        /// </returns>
        public override bool MoveToElement()
        {
            if (_parser.EventCode != ResXMLParser.XmlParserEventCode.START_TAG) return false;
            MoveToAttribute(null);
            return true;
        }

        /// <summary>
        /// Parses the attribute value into one or more Text, EntityReference, or EndEntity nodes.
        /// </summary>
        /// <returns>
        /// true if there are nodes to return.false if the reader is not positioned on an attribute node when the initial call is made or if all the attribute values have been read.An empty attribute, such as, misc="", returns true with a single node with a value of String.Empty.
        /// </returns>
        public override bool ReadAttributeValue()
        {
            if (_attributeIndex == null) return false;
            string value = GetAttribute((int) _attributeIndex.Value);
            SetState(
                nodeType: XmlNodeType.Text,
                value: value);
            return true;
        }

        /// <summary>
        /// Reads the next node from the stream.
        /// </summary>
        /// <returns>
        /// true if the next node was read successfully; false if there are no more nodes to read.
        /// </returns>
        /// <exception cref="T:System.Xml.XmlException">An error occurred while parsing the XML. </exception>
        public override bool Read()
        {
            _readIterator.MoveNext();
            return _readIterator.Current;
        }

        private IEnumerable<bool> ReadIterator()
        {
            SetState(readState: ReadState.Initial);
            while (_readState == ReadState.Interactive || _readState == ReadState.Initial)
            {
                ResXMLParser.XmlParserEventCode eventCode = _parser.Next();
                if (_parser.CommentID != null)
                {
                    SetState(nodeType: XmlNodeType.Comment, value: _parser.Comment);
                    yield return true;
                }
                string prefix;
                string uri;
                switch (eventCode)
                {
                    case ResXMLParser.XmlParserEventCode.START_DOCUMENT:
                        break;
                    case ResXMLParser.XmlParserEventCode.END_DOCUMENT:
                        SetState(readState: ReadState.EndOfFile);
                        break;
                    case ResXMLParser.XmlParserEventCode.START_NAMESPACE:
                        prefix = _parser.NamespacePrefix;
                        uri = _parser.NamespaceUri;
                        AddNamespace(prefix, uri);
                        break;
                    case ResXMLParser.XmlParserEventCode.END_NAMESPACE:
                        prefix = _parser.NamespacePrefix;
                        uri = _parser.NamespaceUri;
                        RemoveNamespace(prefix, uri);
                        break;
                    case ResXMLParser.XmlParserEventCode.START_TAG:
                        MoveToAttribute(null);
                        yield return true;
                        _depth++;
                        break;
                    case ResXMLParser.XmlParserEventCode.END_TAG:
                        string elementNamespace = _parser.ElementNamespace;
                        SetState(
                            nodeType: XmlNodeType.EndElement,
                            localName: _parser.ElementName,
                            namespaceUri: elementNamespace,
                            prefix: LookupPrefix(elementNamespace));
                        yield return true;
                        break;
                    case ResXMLParser.XmlParserEventCode.TEXT:
                        SetState(nodeType: XmlNodeType.Text, value: _parser.CData);
                        yield return true;
                        break;
                    default:
#if !CORECLR
                        Console.WriteLine("Warning: Unexpeted event code: {0} (0x{1:x4})", eventCode, (int) eventCode);
#endif
                        break;
                }
            }

            SetState(readState: ReadState.EndOfFile);
            while (true)
            {
                yield return false;
            }
        }
        
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_readState == ReadState.Closed) return;
            _readState = ReadState.Closed;
            _parser.Close();
        }

        /// <summary>
        /// Resolves a namespace prefix in the current element's scope.
        /// </summary>
        /// <returns>
        /// The namespace URI to which the prefix maps or null if no matching prefix is found.
        /// </returns>
        /// <param name="prefix">The prefix whose namespace URI you want to resolve. To match the default namespace, pass an empty string. </param>
        public override string LookupNamespace(string prefix)
        {
            List<string> definitions;
            _namespaces.TryGetValue(prefix, out definitions);
            if (definitions == null || definitions.Count == 0) return null;
            return definitions.LastOrDefault();
        }

        /// <summary>
        /// Resolves a namespace uri in the current element's scope.
        /// </summary>
        /// <returns>
        /// The namespace prefix to which maps the URI or null if no matching URI is found.
        /// </returns>
        /// <param name="uri">The URI whose namespace prefix you want to resolve.</param>
        protected string LookupPrefix(string uri)
        {
            return _namespaces
                       .Where(pair => pair.Value.Count > 0 && (pair.Value.LastOrDefault() ?? "") == (uri ?? ""))
                       .Select(pair => pair.Key)
                       .LastOrDefault() ?? "";
        }

        /// <summary>
        /// Resolves the entity reference for EntityReference nodes.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The reader is not positioned on an EntityReference node; this implementation of the reader cannot resolve entities (<see cref="P:System.Xml.XmlReader.CanResolveEntity"/> returns false). </exception>
        public override void ResolveEntity()
        {
            throw new InvalidOperationException("Entities not supported");
        }

#region Overriden abstract properties

        /// <summary>
        /// Gets the type of the current node.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Xml.XmlNodeType"/> values representing the type of the current node.
        /// </returns>
        public override XmlNodeType NodeType
        {
            get { return _nodeType; }
        }

        /// <summary>
        /// Gets the local name of the current node.
        /// </summary>
        /// <returns>
        /// The name of the current node with the prefix removed. For example, LocalName is book for the element &lt;bk:book&gt;.For node types that do not have a name (like Text, Comment, and so on), this property returns String.Empty.
        /// </returns>
        public override string LocalName
        {
            get { return _localName; }
        }

        /// <summary>
        /// Gets the namespace URI (as defined in the W3C Namespace specification) of the node on which the reader is positioned.
        /// </summary>
        /// <returns>
        /// The namespace URI of the current node; otherwise an empty string.
        /// </returns>
        public override string NamespaceURI
        {
            get { return _namespaceUri; }
        }

        /// <summary>
        /// Gets the namespace prefix associated with the current node.
        /// </summary>
        /// <returns>
        /// The namespace prefix associated with the current node.
        /// </returns>
        public override string Prefix
        {
            get { return _prefix; }
        }

        /// <summary>
        /// Gets the text value of the current node.
        /// </summary>
        /// <returns>
        /// The value returned depends on the <see cref="P:System.Xml.XmlReader.NodeType"/> of the node. The following table lists node types that have a value to return. All other node types return String.Empty.Node type Value AttributeThe value of the attribute. CDATAThe content of the CDATA section. CommentThe content of the comment. DocumentTypeThe internal subset. ProcessingInstructionThe entire content, excluding the target. SignificantWhitespaceThe white space between markup in a mixed content model. TextThe content of the text node. WhitespaceThe white space between markup. XmlDeclarationThe content of the declaration. 
        /// </returns>
        public override string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the depth of the current node in the XML document.
        /// </summary>
        /// <returns>
        /// The depth of the current node in the XML document.
        /// </returns>
        public override int Depth
        {
            get { return _depth; }
        }

        /// <summary>
        /// Gets the base URI of the current node.
        /// </summary>
        /// <returns>
        /// The base URI of the current node.
        /// </returns>
        public override string BaseURI
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets a value indicating whether the current node is an empty element (for example, &lt;MyElement/&gt;).
        /// </summary>
        /// <returns>
        /// true if the current node is an element (<see cref="P:System.Xml.XmlReader.NodeType"/> equals XmlNodeType.Element) that ends with /&gt;; otherwise, false.
        /// </returns>
        public override bool IsEmptyElement
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of attributes on the current node.
        /// </summary>
        /// <returns>
        /// The number of attributes on the current node.
        /// </returns>
        public override int AttributeCount
        {
            get { return (int) _parser.AttributeCount; }
        }

        /// <summary>
        /// Gets a value indicating whether the reader is positioned at the end of the stream.
        /// </summary>
        /// <returns>
        /// true if the reader is positioned at the end of the stream; otherwise, false.
        /// </returns>
        public override bool EOF
        {
            get { return _readState == ReadState.EndOfFile; }
        }

        /// <summary>
        /// Gets the state of the reader.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Xml.ReadState"/> values.
        /// </returns>
        public override ReadState ReadState
        {
            get { return _readState; }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Xml.XmlNameTable"/> associated with this implementation.
        /// </summary>
        /// <returns>
        /// The XmlNameTable enabling you to get the atomized version of a string within the node.
        /// </returns>
        public override XmlNameTable NameTable
        {
            get { return _nameTable; }
        }

#endregion

#endregion
    }

    internal class NamespaceInfo
    {
        public string Prefix { get; set; }
        public string Uri { get; set; }
    }
}