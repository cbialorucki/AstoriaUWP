using AndroidInteropLib.android.content;
using AndroidInteropLib.android.util;
using AndroidInteropLib.android.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AndroidInteropLib.android.support.design.widget
{
    public class AppBarLayout : ViewGroup
    {
        Grid sourceGrid1;

        public AppBarLayout(Context c, AttributeSet a) : base(c, a)
        {
            
        }

        public override void CreateWinUI(params object[] obj)
        {
            sourceGrid1 = new Grid();
            WinUI.Content = sourceGrid1;
        }

        public override void addView(View view, LayoutParams param)
        {
            sourceGrid1.Children.Add(view);
        }
        public override void addView(View view)
        {
            sourceGrid1.Children.Add(view);
        }
        public override void removeView(View view)
        {
            sourceGrid1.Children.Remove(view);
        }
    }
}
