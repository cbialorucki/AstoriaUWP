using DalvikUWPCSharp.Applet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Reassembly
{
    public class PreloadedDroidApp
    {
        DroidApp runningApp;
        AstoriaContext currentContext;
        AstoriaR currentRes;

        public PreloadedDroidApp(DroidApp da)
        {
            runningApp = da;
            currentContext = new AstoriaContext(runningApp);
            currentRes = new AstoriaR(da);
        }




    }
}
