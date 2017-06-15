using AndroidInteropLib.android.content.res;
using DalvikUWPCSharp.Applet;
using DalvikUWPCSharp.Disassembly.APKReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;

namespace DalvikUWPCSharp.Reassembly
{
    public class AstoriaResources : Resources
    {
        DroidApp currentApp;
        Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

        public static async Task<AstoriaResources> CreateAsync(DroidApp da)
        {
            AstoriaResources temp = new AstoriaResources(da);
            await temp.EnumFiles();
            return temp;
        }

        private AstoriaResources(DroidApp infoWars) : base()
        {
            currentApp = infoWars;
        }

        public async Task EnumFiles()
        {
            currentApp.cpu.hostPage.setPreloadStatusText("Enumerating layout files...");

            StorageFolder layoutFolder = await StorageFolder.GetFolderFromPathAsync(currentApp.resFolder.Path + @"\layout");
            foreach(StorageFile sf in await layoutFolder.GetFilesAsync())
            {
                int rootPathLength = currentApp.localAppRoot.Path.Length;
                string relPathKey = sf.Path.Remove(0, rootPathLength + 1).Replace('\\', '/');

                byte[] file = await Disassembly.Util.ReadFile(sf);

                files[relPathKey] = file;
                currentApp.cpu.hostPage.setPreloadStatusText($"Enumerated {sf.Name}");
            }

        }

        public override int getColor(int id)
        {
            List<string> res = currentApp.metadata.resStrings["@" + id.ToString("X")];
            return int.Parse(res[0]);
        }

        public override string getString(int id)
        {
            List<string> res = currentApp.metadata.resStrings["@" + id.ToString("X")];
            return res[0];
        }

        public override string[] getStringArray(int id)
        {
            List<string> res = currentApp.metadata.resStrings["@" + id.ToString("X")];
            return res.ToArray();
        }

        public override XmlResourceParser getLayout(int id)
        {
            //res should be layout filepath string
            List<string> res = currentApp.metadata.resStrings["@" + id.ToString("X")];

            string fileName = res[0];
            //string fileName = currentApp.context.getR().layout.get(id).ToString();
            byte[] xmlfile = files[fileName];
            //byte[] xmlfile = files[((currentApp.metadata.resStrings["@" + id.ToString("X")])[0])];

            using (MemoryStream ms = new MemoryStream(xmlfile))
            {
                //AndroidXml.AndroidXmlReader axr = new AndroidXml.AndroidXmlReader(ms);
                //XDocument axr = XDocument.Parse(System.Text.Encoding.UTF8.GetString(xmlfile));
                XmlReader axr = XmlReader.Create(ms);
                return new AstoriaXmlParser(axr);
            }
        }

        /*private void task1Loaded(IAsyncOperation<StorageFile> sender, AsyncStatus e)
        {
            //StorageFile sf = sender.GetResults();


        }*/
    }
}
