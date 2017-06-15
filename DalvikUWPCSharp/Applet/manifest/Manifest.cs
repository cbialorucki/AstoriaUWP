using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;
using Windows.UI.Popups;

namespace DalvikUWPCSharp.Applet
{
    public class Manifest
    {
        /*public string fullText { get; private set; }

        public XDocument LINQData { get; private set; }

        public string Label { get; private set; } = "No label";
        public string IconPath { get; private set; } = "No label";

        public Manifest(StorageFile sf)
        {
            if(sf != null)
            {
                AsyncLoad(sf);

                Debug.WriteLine("manifest loaded");

            }
            

        }

        public Manifest(StorageFile sf, bool isDecompiled)
        {
            if(!isDecompiled)
            {
                AsyncLoad(sf);
            }
            else
            {
                DecompiledAsyncLoad(sf);
            }
        }

        public async Task AsyncLoad(StorageFile manifest)
        {
            try
            {
                var dialog = new MessageDialog("Manifest found!\nContents:\n\n" + manifest.ToString());
                await dialog.ShowAsync();

                byte[] manifestBytes = await Disassembly.Util.ReadFile(manifest);
                string decoded = Disassembly.Manifest.ManifestDecompressor.DecompressAXML(manifestBytes);

                var dialog2 = new MessageDialog("Manifest decoded!\nContents:\n\n" + decoded);
                await dialog2.ShowAsync();

                fullText = decoded;
                LINQData = XDocument.Parse(decoded);
                Debug.WriteLine("linqdata parsed");
                Debug.WriteLine("Label: " + LINQData.Element("manifest").Element("application").Attribute("label").Value);

                Label = LINQData.Element("manifest").Element("application").Attribute("label").Value;
                IconPath = LINQData.Element("manifest").Element("application").Attribute("icon").Value;

                //var a = LINQData.Element("manifest").Elements("")
                DroidApp.InvokeLoadEvent();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog($"Manifest translation failed. Please try a different file.\n{ex.Message}\n{ex.InnerException}");
                await dialog.ShowAsync();
            }
        }

        public async Task DecompiledAsyncLoad(StorageFile man)
        {
            try
            {
                var dialog = new MessageDialog("Manifest found!\nContents:\n\n" + man.ToString());
                await dialog.ShowAsync();

                //byte[] manifestBytes = await Disassembly.Util.ReadFile(man);
                string decoded = await Windows.Storage.FileIO.ReadTextAsync(man);

                var dialog2 = new MessageDialog("Manifest decoded!\nContents:\n\n" + decoded);
                await dialog2.ShowAsync();

                fullText = decoded;
                decoded.Replace("android:", "android");
                LINQData = XDocument.Parse(decoded);
                Debug.WriteLine("linqdata parsed");
                Debug.WriteLine("Label: " + LINQData.Element("manifest").Element("application").Attribute("androidlabel").Value);

                Label = LINQData.Element("manifest").Element("application").Attribute("androidlabel").Value;
                IconPath = LINQData.Element("manifest").Element("application").Attribute("androidicon").Value;

                //var a = LINQData.Element("manifest").Elements("")
                DroidApp.InvokeLoadEvent();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog($"Manifest translation failed. Please try a different file.\n{ex.Message}\n{ex.InnerException}");
                await dialog.ShowAsync();
            }
        }*/
    }
}
