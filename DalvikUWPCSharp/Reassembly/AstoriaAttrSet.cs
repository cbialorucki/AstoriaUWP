using AndroidInteropLib.android.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DalvikUWPCSharp.Reassembly
{
    public class AstoriaAttrSet : AttributeSet
    {
        public readonly XAttribute[] attributes;
        public readonly XElement xe1;
        //private readonly XElement element;
        //ANDROID: namespace = string p1nspace = "{http://schemas.android.com/apk/res/android}";


        public AstoriaAttrSet(XElement xe)
        {
            xe1 = xe;
            attributes = xe.Attributes().ToArray();
        }

        public bool getAttributeBooleanValue(string nspace, string attribute, bool defaultValue)
        {
            try { return bool.Parse(FindAttributeVal(nspace, attribute)); }
            catch { return defaultValue; }
        }

        public bool getAttributeBooleanValue(int index, bool defaultValue)
        {
            try { return bool.Parse(attributes[index].Value); }
            catch { return defaultValue; }
        }

        public int getAttributeCount()
        {
            return attributes.Length;
        }

        public float getAttributeFloatValue(int index, float defaultValue)
        {
            try { return float.Parse(attributes[index].Value); }
            catch { return defaultValue; }
        }

        public float getAttributeFloatValue(string nspace, string attribute, float defaultValue)
        {
            try { return float.Parse(FindAttributeVal(nspace, attribute)); }
            catch { return defaultValue; }
        }

        public int getAttributeIntValue(string nspace, string attribute, int defaultValue)
        {
            try { return int.Parse(FindAttributeVal(nspace, attribute)); }
            catch { return defaultValue; }
        }

        public int getAttributeIntValue(int index, int defaultValue)
        {
            try { return int.Parse(attributes[index].Value); }
            catch { return defaultValue; }
        }

        public int getAttributeListValue(int index, string[] options, int defaultValue)
        {
            throw new NotImplementedException();
        }

        public int getAttributeListValue(string nspace, string attribute, string[] options, int defaultValue)
        {
            throw new NotImplementedException();
        }

        public string getAttributeName(int index)
        {
            try { return attributes[index].Name.ToString(); }
            catch { return null; }
        }

        public int getAttributeNameResource(int index)
        {
            //TODO: Return res ID for attribute
            throw new NotImplementedException();
        }

        public int getAttributeResourceValue(string nspace, string attribute, int defaultValue)
        {
            //TODO: return res ID for attribute
            throw new NotImplementedException();
        }

        public int getAttributeResourceValue(int index, int defaultValue)
        {
            //TODO: return res ID for attribute
            throw new NotImplementedException();
        }

        public uint getAttributeUnsignedIntValue(string nspace, string attribute, uint defaultValue)
        {
            try { return uint.Parse(FindAttributeVal(nspace, attribute)); }
            catch { return defaultValue; }
        }

        public uint getAttributeUnsignedIntValue(int index, uint defaultValue)
        {
            try { return uint.Parse(attributes[index].Value); }
            catch { return defaultValue; }
        }

        public string getAttributeValue(int index)
        {
            return attributes[index].Value;
        }

        public string getAttributeValue(string nspace, string name)
        {
            return FindAttributeVal(nspace, name);
        }

        public string getClassAttribute()
        {
            return getAttributeValue(null, "class");
        }

        public string getIdAttribute()
        {
            return getAttributeValue(null, "id");
        }

        public int getIdAttributeResourceValue(int defaultValue)
        {
            return getAttributeResourceValue(null, "id", defaultValue);
        }

        public string getPositionDescription()
        {
            throw new NotImplementedException();
        }

        public int getStyleAttribute()
        {
            throw new NotImplementedException();
            //return getAttributeResourceValue(null, "style");
        }

        private string FindAttributeVal(string nspace, string attribute)
        {
            //Wrap nspace with curly brackets if not already
            if(!nspace.StartsWith("{") && !nspace.EndsWith("}") && (!nspace.Equals(string.Empty) || nspace == null))
            {
                nspace = "{" + nspace + "}";
            }

            string expandedName = (nspace ?? "") + attribute;

            foreach(XAttribute xa in attributes)
            {
                if(xa.Name.ToString().Equals(expandedName))
                {
                    return xa.Value;
                }
            }

            return null;
        }
    }
}
