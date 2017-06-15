using AndroidInteropLib.com.android._internal;
using DalvikUWPCSharp.Applet;
using dex.net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Reassembly
{
    public class AstoriaR : R
    {
        private EnhancedDexWriter edw;
        private DroidApp runningApp;

        public AstoriaR(DroidApp app)
        {
            runningApp = app;
            edw = new EnhancedDexWriter();

            foreach(Class c in app.dex.GetClasses())
            {
                if(c.Name.StartsWith(app.metadata.packageName + ".R") && c.Name.Contains("$"))
                {
                    string[] name = c.Name.Split('$');
                    try
                    {
                        switch (name[1])
                        {
                            case "color":
                                SetSubClass(color, c);
                                break;
                            case "layout":
                                SetSubClass(layout, c);
                                break;
                        }
                    }
                    catch { }
                    
                }
            }
        }

        public void SetSubClass(RSubClass subclass, Class dexClass)
        {
            int i = 0;
            foreach (var field in dexClass.GetFields())
            {
                if (i < dexClass.StaticFieldsValues.Length)
                {
                    //Has value associated with it
                    /*int i2 = (int)ConvertField(dexClass.StaticFieldsValues[i], dexClass);
                    //i2 is a reference to res entry. Find it.
                    List<string> res = runningApp.metadata.resStrings["0x" + i2.ToString("X")];
                    if(res.Count == 1)
                    {
                        int i3;
                        var hackprovider = CultureInfo.InvariantCulture;

                        if(int.TryParse(res[0], System.Globalization.NumberStyles.HexNumber, hackprovider, out i3))
                        {
                            subclass.add(field.Name, Convert.ToInt32(ConvertField(dexClass.StaticFieldsValues[i], dexClass)), i3);
                        }

                        else
                        {
                            subclass.add(field.Name, Convert.ToInt32(ConvertField(dexClass.StaticFieldsValues[i], dexClass)), res[0]);
                        }
                    }

                    else
                    {
                        subclass.add(field.Name, Convert.ToInt32(ConvertField(dexClass.StaticFieldsValues[i], dexClass)), res);
                    }*/

                    subclass.add(field.Name, ConvertField(dexClass.StaticFieldsValues[i], dexClass));
                }

                else
                {
                    //Value not initilized yet
                    subclass.add(field.Name, null);
                }

                i++;
            }
        }

        private object ConvertField(EncodedValue value, Class currentClass)
        {
            switch (value.EncodedType)
            {
                case EncodedValueType.VALUE_BYTE:
                    return ((EncodedNumber)value).AsByte();

                case EncodedValueType.VALUE_NULL:
                    return null;

                case EncodedValueType.VALUE_BOOLEAN:
                    return ((EncodedNumber)value).AsBoolean();

                case EncodedValueType.VALUE_SHORT:
                    return ((EncodedNumber)value).AsShort();

                case EncodedValueType.VALUE_CHAR:
                    return ((EncodedNumber)value).AsChar();

                case EncodedValueType.VALUE_INT:
                    return ((EncodedNumber)value).AsInt();

                case EncodedValueType.VALUE_LONG:
                    return ((EncodedNumber)value).AsLong();

                case EncodedValueType.VALUE_FLOAT:
                    return ((EncodedNumber)value).AsFloat();

                case EncodedValueType.VALUE_DOUBLE:
                    return ((EncodedNumber)value).AsDouble();

                case EncodedValueType.VALUE_STRING:
                    return String.Format("\"{0}\"", runningApp.dex.GetString(((EncodedNumber)value).AsId()).Replace("\n", "\\n"));

                case EncodedValueType.VALUE_TYPE:
                    return runningApp.dex.GetTypeName(((EncodedNumber)value).AsId());

                case EncodedValueType.VALUE_FIELD:
                case EncodedValueType.VALUE_ENUM:
                    return edw.GetFieldName(((EncodedNumber)value).AsId(), currentClass, true);

                case EncodedValueType.VALUE_METHOD:
                    return runningApp.dex.GetMethod(((EncodedNumber)value).AsId());
                    //return string.Format("{0}.{1}", runningApp.dex.GetTypeName(method.ClassIndex), method.Name);

                case EncodedValueType.VALUE_ARRAY:
                    var encodedArray = (EncodedArray)value;
                    object[] array = new object[(int)encodedArray.Count];

                    int i = 0;
                    foreach (var arrayValue in encodedArray.GetValues())
                    {
                        //array.Add(ConvertField(arrayValue, currentClass));
                        array[i] = ConvertField(arrayValue, currentClass);
                        i++;
                    }
                    return array;
                    //return string.Format("[{0}]", string.Join(",", array));

                case EncodedValueType.VALUE_ANNOTATION:
                    //Not sure when/if this is used
                    var stringAnnotation = new StringWriter();
                    edw.AnnotationToString(stringAnnotation, (EncodedAnnotation)value, currentClass, new Indentation());
                    return stringAnnotation;
            }

            return "Unknown";
        }


    }
}
