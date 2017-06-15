using AndroidInteropLib.android.app;
using AndroidInteropLib.android.content;
using AndroidInteropLib.android.content.res;
using AndroidXml;
using DalvikUWPCSharp.Applet;
using DalvikUWPCSharp.Classes;
using DalvikUWPCSharp.Disassembly.APKReader;
using dex.net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;
using AndroidInteropLib.com.android._internal;
using AndroidInteropLib.android.support.design.widget;

namespace DalvikUWPCSharp.Reassembly
{
    public class AstoriaContext : Context
    {
        public DroidApp runningApp { get; private set; }
        List<AstoriaActivity> Activities = new List<AstoriaActivity>();
        AstoriaR RClass;
        AstoriaResources resObj;

        public AstoriaContext(DroidApp da, AstoriaResources res)
        {
            runningApp = da;
            RClass = new AstoriaR(runningApp);
            resObj = res;
            da.context = this;
        }

        public AstoriaContext(DroidApp da)
        {
            runningApp = da;
            RClass = new AstoriaR(runningApp);
            //resObj = new AstoriaResources(runningApp);

            //Inflate manifest, inflate RClass using dex.net
            /*string layoutAbsoluteUri = runningApp.localAppRoot.Path + "\\AndroidManifest.xml";

            IAsyncOperation<StorageFile> task1 = StorageFile.GetFileFromPathAsync(layoutAbsoluteUri);
            StorageFile sf = task1.GetResults();

            Task<byte[]> task = DalvikUWPCSharp.Disassembly.Util.ReadFile(sf);
            byte[] xmlfile = task.Result;

            using (MemoryStream ms = new MemoryStream(xmlfile))
            {
                AndroidXmlReader man = new AndroidXmlReader(ms);
                man.MoveToContent();
                XDocument LINQData = new XDocument(man);

                foreach (XElement xe in LINQData.Elements(XName.Get("activity")))
                {
                    AstoriaActivity a = new AstoriaActivity(this);

                    foreach (XElement xe2 in xe.Descendants())
                    {
                        if (xe2.Name.Equals("intent-filter"))
                        {
                            Intent i = new Intent();

                            foreach (XElement xe3 in xe2.Descendants())
                            {
                                if (xe3.Name.Equals("action"))
                                {
                                    string act = xe3.Attribute(XName.Get("{http://schemas.android.com/apk/res/android}name")).Value;
                                    i.setAction(act);
                                }

                                if (xe3.Name.Equals("category"))
                                {
                                    string cat = xe3.Attribute(XName.Get("{http://schemas.android.com/apk/res/android}name")).Value;
                                    i.addCategory(cat);
                                }
                            }

                            a.setIntent(i);
                        }
                    }

                    Activities.Add(a);
                }
            }*/

        }

        public DalvikCPU getCPU()
        {
            return runningApp.cpu;
        }

        public override Resources getResources()
        {
            //Create new AstoriaResources, pass that
            return resObj;
        }



        public override void startActivity(Intent intent)
        {
            //Find Activity with specified intent, and run it.
            foreach(AstoriaActivity a in Activities)
            {
                if(a.getIntent().Equals(intent))
                {
                    //run this activity
                    a.runMethod("onCreate");
                }
            }
        }

        public void startMainActivity()
        {
            foreach(Activity a in Activities)
            {
                Intent i = a.getIntent();
                if(i.getAction().Equals("android.intent.action.MAIN"))
                {
                    startActivity(i);
                    return;
                }

            }
        }

        public override object getSystemService(string name)
        {
            switch(name)
            {
                case LAYOUT_INFLATER_SERVICE:
                    return new AstoriaLayoutInflator(this);
                default:
                    return null;
            }

        }

        public override R getR()
        {
            return RClass;
        }

        public async override void CallBack(object _params)
        {
            if(_params is FloatingActionButton)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Callback initiated.", "Context");
                await dialog.ShowAsync();
            }
        }
    }
}
