using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    public interface AttributeSet
    {
        bool getAttributeBooleanValue(string nspace, string attribute, bool defaultValue);
        bool getAttributeBooleanValue(int index, bool defaultValue);
        int getAttributeCount();
        float getAttributeFloatValue(int index, float defaultValue);
        float getAttributeFloatValue(string nspace, string attribute, float defaultValue);
        int getAttributeIntValue(string nspace, string attribute, int defaultValue);
        int getAttributeIntValue(int index, int defaultValue);
        int getAttributeListValue(int index, string[] options, int defaultValue);
        int getAttributeListValue(string nspace, string attribute, string[] options, int defaultValue);
        string getAttributeName(int index);
        int getAttributeNameResource(int index);
        int getAttributeResourceValue(string nspace, string attribute, int defaultValue);
        int getAttributeResourceValue(int index, int defaultValue);
        uint getAttributeUnsignedIntValue(string nspace, string attribute, uint defaultValue);
        uint getAttributeUnsignedIntValue(int index, uint defaultValue);
        string getAttributeValue(int index);
        string getAttributeValue(string nspace, string name);
        string getClassAttribute();
        string getIdAttribute();
        int getIdAttributeResourceValue(int defaultValue);
        string getPositionDescription();
        int getStyleAttribute();
    }
}
