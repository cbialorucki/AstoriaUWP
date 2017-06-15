using AndroidInteropLib;
using AndroidInteropLib.android.content;
using AndroidInteropLib.android.view;
using DalvikUWPCSharp.Applet;
using DalvikUWPCSharp.Reassembly;
using dex.net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Classes
{
    public class DalvikCPU
    {
        //List<object> Registers = new List<object>();
        object[] Registers = new object[16];
        object result;
        public Dex dex;
        string packageName;
        public EmuPage hostPage;
        DroidApp da;
        //int LastRegisterModified;

        private Context appContext;
        private Window droidWindow;

        public DalvikCPU(Dex d, string pName, EmuPage hostPg)
        {
            dex = d;
            packageName = pName;
            hostPage = hostPg;
            da = hostPage.RunningApp;
            da.cpu = this;

            hostPage.setPreloadStatusText("Setting up app environment...");
        }

        public async void Start()
        {
            if (appContext == null)
            {
                appContext = new AstoriaContext(da, await AstoriaResources.CreateAsync(da));
                droidWindow = new AstoriaWindow(appContext, hostPage);
            }

            foreach(Class c in dex.GetClasses())
            {
                if(c.Name.Equals(packageName + ".MainActivity"))
                {
                    foreach(Method m in c.GetMethods())
                    {
                        if(m.Name.Equals("onCreate"))
                        {
                            RunMethod(m, c);
                        }
                    }
                }
            }

            hostPage.preloadDone();
        }

        public async void GoBack()
        {
            var dialog = new Windows.UI.Popups.MessageDialog("Back event initiated.", "Dalvik CPU");
            await dialog.ShowAsync();
        }

        public object RunMethod(Method m, Class c, params object[] obj)
        {
            if(!TryNativeMethod(m, c, obj))
            {
                foreach (OpCode o in m.GetInstructions())
                {
                    ExecuteInstruction(o, c);
                }
            }

            return result;
            //dynamic MyD = new DynamicObject()
            
        }

        public void ExecuteInstruction(OpCode op, Class c)
        {
            var opType = op.Instruction;

            switch(op.Instruction)
            {
                case Instructions.Const:
                    ConstOpCode ConstOP = (ConstOpCode)op;
                    Registers[ConstOP.Destination] = ConstOP.Value;
                    break;
                case Instructions.InvokeSuper:
                    InvokeSuperOpCode op2 = (InvokeSuperOpCode)op;
                    RunMethod(dex.GetMethod(op2.MethodIndex), c);
                    break;
                case Instructions.InvokeVirtual:
                    InvokeVirtualOpCode ivop = (InvokeVirtualOpCode)op;
                    RunMethod(dex.GetMethod(ivop.MethodIndex), c, Registers[ivop.ArgumentRegisters[1]]);
                    break;
                case Instructions.MoveResult:
                    MoveResultOpCode movR = (MoveResultOpCode)op;
                    Registers[movR.Destination] = result;
                    break;
                case Instructions.ReturnVoid:
                    //Clear result
                    result = null;
                    break;
                default:
                    Debug.WriteLine("Unhandled Instruction");
                    break;
            }

            Debug.WriteLine("Executed: " + op.ToString());
        }

        private bool TryNativeMethod(Method m, Class c, params object[] obj)
        {
            if(m.Name.Contains("setContentView"))
            {
                droidWindow.setContentView((int)obj[0]);
                return true;
            }

            string className = ConvertClassName(c.Name);
            if (className.StartsWith(packageName))
                return false;
            
            Type myType = Type.GetType("AndroidInteropLib." + className);
            if(myType != null)
            {
                TypeInfo info = myType.GetTypeInfo();
                MethodInfo mi = info.GetDeclaredMethod(m.Name);
                if (mi != null)
                {
                    try
                    {
                        mi.Invoke(this, obj);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }  
                }
            }

            return false;
        }

        private string ConvertClassName(string s)
        {
            return s.Replace("internal", "_internal");
        }
    }

    public class DalvikClass
    {
        Type super;
        //object super;
        Class c;
        DalvikCPU cpu;

        public DalvikClass(Class c, DalvikCPU dc)
        {
            this.c = c;
            cpu = dc;

            if("AndroidInteropLib" + c.SuperClass == "")
            {
                //set super to native class
            }
        }

        public void SetInheritence(Type t)
        {
            super = t;
        }

        public object RunMethod(string name, params object[] obj)
        {
            //Check if current class has method. If not, check super.
            var meth = c.GetMethods().FirstOrDefault(x => x.Name.Equals(name));
            if (meth != null)
                return cpu.RunMethod(meth, c);

            if (super != null)
            {
                TypeInfo info = super.GetTypeInfo();
                MethodInfo mi = info.GetDeclaredMethod(name);
                if (mi != null)
                {
                    try
                    {
                        return mi.Invoke(this, obj);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        private Type GetSuperType()
        {
            return super;
        }
    }
}
