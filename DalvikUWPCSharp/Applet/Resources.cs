using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace DalvikUWPCSharp.Applet
{
    /*public class ResourcesOLD
    {
        public string fullText { get; private set; }

        public ResourcesOLD(StorageFile sf)
        {
            if(sf != null)
            {
                LoadAsync(sf);

                Debug.WriteLine("res loaded.");
            }
        }

        public async Task LoadAsync(StorageFile res)
        {
            var dialog = new MessageDialog("Resources found!\nContents:\n\n" + res.ToString());
            await dialog.ShowAsync();

            byte[] resBytes = await Disassembly.Util.ReadFile(res);
            //string decoded = Disassembly.Manifest.ManifestDecompressor.DecompressAXML(resBytes);

            //var dialog2 = new MessageDialog("Resources decoded!\nContents:\n\n" + decoded);
            //await dialog2.ShowAsync();

            //fullText = decoded;
            Debug.WriteLine("res output:");
            //Debug.WriteLine(decoded);
        }
    }*/
}
