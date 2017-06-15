using AndroidInteropLib.android.support.design.widget;
using AndroidXml;
using DalvikUWPCSharp.Applet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DalvikUWPCSharp.Reassembly.UI
{
    public class Renderer
    {
        public DroidApp CurrentApp;
        //public Frame CurrentFrame; //For statusbar color emulation

        private string p1nspace = "{http://schemas.android.com/apk/res/android}";

        public Renderer(DroidApp da)
        {
            CurrentApp = da;
        }

        public async Task<UIElement> RenderXmlFile(StorageFile sf)
        {
            using (MemoryStream stream = new MemoryStream(await Disassembly.Util.ReadFile(sf)))
            {
                AndroidXmlReader reader = new AndroidXmlReader(stream);
                reader.MoveToContent();
                XDocument document = XDocument.Load(reader);
                //string p1nspace = "{http://schemas.android.com/apk/res/android}";

                foreach(XElement xe in document.Elements())
                {
                    //Should only be 1 element
                    return await RenderObject(xe);
                }

                throw new Exception("Invalid XML File");
                //decoded = document.ToString();
            }
        }

        public async Task<UIElement> RenderObject(XElement xe)
        {
            string xeName = xe.Name.ToString();
            bool nestedObjs = xe.HasElements;
            //AstoriaContext context = new AstoriaContext();

            if (xeName.Equals("android.support.design.widget.AppBarLayout"))
            {
                //This manipulates the appbar. For now, let's just make it a grid. Usually contains toolbar.
                //double height = DPtoEP(56);
                Grid container = new Grid();
                container.VerticalAlignment = VerticalAlignment.Top;
                container.HorizontalAlignment = HorizontalAlignment.Stretch;
                container.Height = attr.actionBarSize; //DPtoEP(56);

                if(nestedObjs)
                {
                    foreach(XElement xe1 in xe.Elements())
                    {
                        container.Children.Add(await RenderObject(xe1));
                    }
                }
                return container;
            }

            else if (xeName.Equals("android.support.design.widget.CoordinatorLayout"))
            {
                CoordinatorLayout cl = new CoordinatorLayout();
                if(nestedObjs)
                {
                    foreach(XElement xe1 in xe.Elements())
                    {
                        cl.Add(await RenderObject(xe1));
                    }
                }

                return cl;
                
            }

            else if(xeName.Equals("android.support.design.widget.FloatingActionButton"))
            {
                //Ignore gravity for now, is typically bottom right
                //Button FAB = new Button();
                //FloatingActionButton FAB = new FloatingActionButton(context, new AstoriaAttrSet(xe));
                //FAB.HorizontalAlignment = HorizontalAlignment.Right;
                //FAB.VerticalAlignment = VerticalAlignment.Bottom;
                //FAB.Width = 52;
                //FAB.Height = 52;
                //xe.Attribute(XName.Get())
                //FAB.Content = "src: " + xe.Attribute(XName.Get(p1nspace + "src")).Value;
                //FAB.Content = "src: " + CurrentApp.metadata.resStrings[xe.Attribute(XName.Get(p1nspace + "src")).Value][0];
                //FAB.Margin = new Thickness(10);
                return null;
                //return FAB;
            }

            else if(xeName.Equals("android.support.v7.widget.Toolbar"))
            {
                //return null;
                //Read attributes on toolbar and return it
                AndroidToolbar at = new AndroidToolbar();
                at.SetTitle(CurrentApp.metadata.label);
                //at.Height = DPtoEP(56);
                return at;
            }

            else if(xeName.Equals("include"))
            {
                //Get layout attribute, render the .xml, and return that
                //return null;
                string relUri = xe.Attribute("layout").Value;
                //Take current app path, pass 
                string path = CurrentApp.resFolder.Path + relUri.Replace('@', '\\').Replace('/', '\\') + ".xml";
                StorageFile sf = await StorageFile.GetFileFromPathAsync(path);
                return await RenderXmlFile(sf);
            }

            else if (xeName.Equals("RelativeLayout"))
            {
                //Return Grid with objects inside
                Grid container = new Grid();
                if (nestedObjs)
                {
                    foreach (XElement xe1 in xe.Elements())
                    {
                        container.Children.Add(await RenderObject(xe1));
                    }
                }

                return container;
            }

            else if (xeName.Equals("TextView"))
            {
                TextBlock tv = new TextBlock();

                string content = xe.Attribute(p1nspace + "text").Value;
                tv.Text = content;
                //Default left padding is 16dp left, 8dp tall
                tv.Margin = new Thickness(14.8, 7.4, 14.8, 7.4);
                //-2 represents "wrap_content", essentially "autosize"
                int width = int.Parse(xe.Attribute(p1nspace + "layout_width").Value);
                int height = int.Parse(xe.Attribute(p1nspace + "layout_height").Value);

                if(width > -1)
                {
                    tv.Width = width;
                }

                if(height > -1)
                {
                    tv.Height = height;
                }

                //Default text color is gray 115
                tv.Foreground = new SolidColorBrush(Color.FromArgb(255, 115, 115, 115));
                return tv;
            }

            else
            {
                throw new NotImplementedException($"UIElement {xe.Name.ToString()} is not currently implemented on this renderer.");
                //return null;
            }
        }

        public static double DPtoEP(int i)
        {
            //TODO: scale android dp sizes to windows dp sizes

            //Android dp size = dp = (width in pixels * 160) / screen density in dpi; 1dp = 1px on a 160dpi screen; px = dp * (dpi / 160)
            //Android DP is equivelent to the current screen size at 160dpi
            //Windows Effictive Pixels (ep) = 146.86 dpi (phone) ~150 (Tablet) ~110 (Desktop)
            //Let's make it 148 dpi for simplicity sake.

            //ep = (dp/160) * 148
            return (i / 160) * 148;
        }
    }
}
