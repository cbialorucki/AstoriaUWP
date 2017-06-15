using DalvikUWPCSharp.Disassembly.APKParser.struct_.xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    class XmlNamespaces
    {
        private List<XmlNamespace> nspaces;

        private List<XmlNamespace> newNamespaces;

        public XmlNamespaces()
        {
            this.nspaces = new List<XmlNamespace>();
            this.newNamespaces = new List<XmlNamespace>();
        }

        public void addNamespace(XmlNamespaceStartTag tag)
        {
            XmlNamespace nspace = new XmlNamespace(tag.getPrefix(), tag.getUri());
            nspaces.Add(nspace);
            newNamespaces.Add(nspace);
        }

        public void removeNamespace(XmlNamespaceEndTag tag)
        {
            XmlNamespace nspace = new XmlNamespace(tag.getPrefix(), tag.getUri());
            nspaces.Remove(nspace);
            newNamespaces.Remove(nspace);
        }

        public string getPrefixViaUri(string uri)
        {
            if (uri == null)
            {
                return null;
            }
            foreach (XmlNamespace nspace in nspaces)
            {
                if (nspace.uri.Equals(uri)) {
                    return nspace.prefix;
                }
            }

            return null;
        }

        public List<XmlNamespace> consumeNameSpaces()
        {
            if (newNamespaces.Count != 0) //.isEmpty())
            {
                List<XmlNamespace> xmlNamespaces = new List<XmlNamespace>();
                xmlNamespaces.AddRange(newNamespaces); //addAll(newNamespaces);
                newNamespaces.Clear(); //.clear();
                return xmlNamespaces;
            }
            else
            {
                return new List<XmlNamespace>(); //Collections.emptyList();
            }
        }

        /**
         * one nspace
         */
        public class XmlNamespace
        {
            public string prefix;
            public string uri;

            public XmlNamespace(string prefix, string uri)
            {
                this.prefix = prefix;
                this.uri = uri;
            }

            public string getPrefix()
            {
                return prefix;
            }

            public string getUri()
            {
                return uri;
            }

            public override bool Equals(object obj)
            {
                return equals(obj);
            }

            public bool equals(object o)
            {
                if (this == o) return true;
                //if (o == null || getClass() != o.getClass()) return false;
                if (o == null || this.GetType() != o.GetType()) return false;

                XmlNamespace nspace = (XmlNamespace) o;

                if (prefix == null && nspace.prefix != null) return false;
                if (uri == null && nspace.uri != null) return false;
                if (prefix != null && !prefix.Equals(nspace.prefix)) return false;
                if (uri != null && !uri.Equals(nspace.uri)) return false;

                return true;
            }

            public override int GetHashCode()
            {
                return hashCode();
            }

            public int hashCode()
            {
                int result = prefix.GetHashCode();
                result = 31 * result + uri.GetHashCode();
                return result;
            }
        }
    }
}
