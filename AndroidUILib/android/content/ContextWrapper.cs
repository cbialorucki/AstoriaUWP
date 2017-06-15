using AndroidInteropLib.android.content.res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidInteropLib.com.android._internal;

namespace AndroidInteropLib.android.content
{
    public class ContextWrapper : Context
    {
        Context mBase;

        public ContextWrapper(Context _base)
        {
            mBase = _base;
        }

        protected void attachBaseContext(Context _base)
        {
            if (mBase != null)
            {
                throw new InvalidOperationException("Base context already set");
            }

            mBase = _base;
        }

        public Context getBaseContext()
        {
            return mBase;
        }

        /*public override AssetManager getAssets()
        {
            return mBase.getAssets();
        }*/

        public override Resources getResources()
        {
            return mBase.getResources();
        }

        public override void startActivity(Intent intent)
        {
            mBase.startActivity(intent);
        }

        public override object getSystemService(string name)
        {
            return mBase.getSystemService(name);
        }

        public override R getR()
        {
            return mBase.getR();
        }

        public override void CallBack(object _params)
        {
            mBase.CallBack(_params);
        }
    }
}
