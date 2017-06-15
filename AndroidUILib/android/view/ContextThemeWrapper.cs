using AndroidInteropLib.android.content;
using AndroidInteropLib.android.content.res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.view
{
    public class ContextThemeWrapper : ContextWrapper
    {
        private int mThemeResource;
        //private Resources.Theme mTheme;
        private LayoutInflater mInflater;
        //private Configuration mOverrideConfiguration;
        private Resources mResources;

        public ContextThemeWrapper(Context _base) : base(_base)
        {

        }

    }
}
