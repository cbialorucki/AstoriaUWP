using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.view
{
    public interface ViewManager
    {
        void addView(View view, ViewGroup.LayoutParams param);
        void removeView(View view);
        void updateViewLayout(View view, ViewGroup.LayoutParams param);
    }
}
