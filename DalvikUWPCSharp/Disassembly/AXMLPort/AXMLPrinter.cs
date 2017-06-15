using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DalvikUWPCSharp.Disassembly.AXMLPort
{
    class AXMLPrinter
    {
        readonly string indentStep = "	";

        public void main(StorageFile arg)
        {
            if (arg == null)
            {
                log("Usage: AXMLPrinter <binary xml file>");
                return;
            }

            try
            {
                AXmlResourceParser parser = new AXmlResourceParser();
                //Create Stream
                //var stream = await arg.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                FileStream stream = new FileStream(arg.Path, FileMode.Open);
                parser.open(stream);
                StringBuilder indent = new StringBuilder(10);
                
                while (true)
                {
                    int type = parser.next();
                    if (type == XmlPullParser.END_DOCUMENT)
                    {
                        break;
                    }
                    switch (type)
                    {
                        //case XmlPullParser.START_DOCUMENT:
                        case 0:
                            {
                                log("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                                break;
                            }
                        //case XmlPullParser.START_TAG:
                        case 2:
                            {
                                log("{0}<{1}{2}", indent,
                                    getNamespacePrefix(parser.getPrefix()), parser.getName());
                                indent.Append(indentStep);

                                int namespaceCountBefore = parser.getNamespaceCount(parser.getDepth() - 1);
                                int namespaceCount = parser.getNamespaceCount(parser.getDepth());
                                for (int i = namespaceCountBefore; i != namespaceCount; ++i)
                                {
                                    log("{0}xmlns:{1}=\"{2}\"",
                                        indent,
                                        parser.getNamespacePrefix(i),
                                        parser.getNamespaceUri(i));
                                }

                                for (int i = 0; i != parser.getAttributeCount(); ++i)
                                {
                                    log("{0}{1}{2}=\"{3}\"", indent,
                                        getNamespacePrefix(parser.getAttributePrefix(i)),
                                        parser.getAttributeName(i),
                                        getAttributeValue(parser, i));
                                }
                                log("{0}>", indent);
                                break;
                            }
                        //case XmlPullParser.END_TAG:
                        case 3:
                            {
                                indent.Length = indent.Length - indentStep.Length;
                                log("{0}</{1}{2}>", indent,
                                    getNamespacePrefix(parser.getPrefix()),
                                    parser.getName());
                                break;
                            }
                        //case XmlPullParser.TEXT:
                        case 4:
                            {
                                log("{0}{1}", indent, parser.getText());
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                log(e.StackTrace);
                //e.printStackTrace();
            }
        }

        private static string getNamespacePrefix(string prefix)
        {
            if (prefix == null || prefix.Length == 0)
            {
                return "";
            }
            return prefix + ":";
        }

        private static string getAttributeValue(AXmlResourceParser parser, int index)
        {
            int type = parser.getAttributeValueType(index);
            int data = parser.getAttributeValueData(index);
            if (type == TypedValue.TYPE_STRING)
            {
                return parser.getAttributeValue(index);
            }
            if (type == TypedValue.TYPE_ATTRIBUTE)
            {
                //%08x is info represented in hex with 8 traling zeros
                //return string.Format("?%s%08X", getPackage(data), data);
                return string.Format("?{0}0x{1}", getPackage(data), data.ToString("X8"));
            }
            if (type == TypedValue.TYPE_REFERENCE)
            {
                return string.Format("@{0}0x{1}", getPackage(data), data.ToString("X8"));
            }
            if (type == TypedValue.TYPE_FLOAT)
            {
                //return String.valueOf(Float.intBitsToFloat(data));
                byte[] intBytes = BitConverter.GetBytes(data);
                float f = BitConverter.ToSingle(intBytes, 0);
                return f.ToString();
            }
            if (type == TypedValue.TYPE_INT_HEX)
            {
                return string.Format("0x%08X", data);
            }
            if (type == TypedValue.TYPE_INT_BOOLEAN)
            {
                return data != 0 ? "true" : "false";
            }
            if (type == TypedValue.TYPE_DIMENSION)
            {
                //Check later...
                return complexToFloat(data).ToString() +
                    DIMENSION_UNITS[data & TypedValue.COMPLEX_UNIT_MASK];
            }
            if (type == TypedValue.TYPE_FRACTION)
            {
                return complexToFloat(data).ToString() +
                    FRACTION_UNITS[data & TypedValue.COMPLEX_UNIT_MASK];
            }
            if (type >= TypedValue.TYPE_FIRST_COLOR_INT && type <= TypedValue.TYPE_LAST_COLOR_INT)
            {
                return string.Format("#%08X", data);
            }
            if (type >= TypedValue.TYPE_FIRST_INT && type <= TypedValue.TYPE_LAST_INT)
            {
                return data.ToString();
            }
            return string.Format("<0x{0}, type 0x{1}>", string.Format("X", data), string.Format("X2", type));
        }

        private static string getPackage(int id)
        {
            //if(id >>> 24 == 1)
            if (unchecked((int) (uint)id >> 24) == 1)
            {
                return "android:";
            }
            return "";
        }

        private static void log(string format, params object[] obj)
        {
            Debug.WriteLine(string.Format(format, obj));
            //System.out.printf(format, arguments);
            //System.out.println();
        }

        private static void log(string msg)
        {
            Debug.WriteLine(msg);
            //System.out.printf(format, arguments);
            //System.out.println();
        }

        /////////////////////////////////// ILLEGAL STUFF, DONT LOOK :)

        public static float complexToFloat(int complex)
        {
            return (float)(complex & 0xFFFFFF00) * RADIX_MULTS[(complex >> 4) & 3];
        }

        private static readonly float[] RADIX_MULTS = {0.00390625F,3.051758E-005F,1.192093E-007F,4.656613E-010F};
        private static readonly string[] DIMENSION_UNITS = {"px","dip","sp","pt","in","mm","",""};
	    private static readonly string[] FRACTION_UNITS = {"%","%p","","","","","",""};

    }
}
