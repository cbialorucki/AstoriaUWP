using AndroidInteropLib.android.app;
using AndroidInteropLib.android.content;
using dex.net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Reassembly
{
    public class AstoriaActivity : Activity
    {
        Class thisClass;
        AstoriaContext appContext;

        public AstoriaActivity(AstoriaContext c) : base(c)
        {
            appContext = c;
        }

        public void setActivityClass(dex.net.Class c)
        {
            thisClass = c;
        }

        public object runMethod(string methodName)
        {
            foreach(Method m in thisClass.GetMethods())
            {
                if(m.Name.Equals(methodName))
                {
                    return appContext.getCPU().RunMethod(m, thisClass);
                }
            }

            return null;
        }

        public object runMethod(int id)
        {
            foreach (Method m in thisClass.GetMethods())
            {
                if (m.Id.Equals(id))
                {
                    return appContext.getCPU().RunMethod(m, thisClass);
                }
            }

            return null;
        }
    }
}
