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

namespace AndroidInteropLib.android.widget
{
    public class RelativeLayout : ViewGroup
    {
        Grid content = new Grid();

        public RelativeLayout(Context c, AttributeSet a) : base(c, a)
        {
            
        }

        public override void CreateWinUI(params object[] obj)
        {
            content.HorizontalAlignment = HorizontalAlignment.Stretch;
            content.VerticalAlignment = VerticalAlignment.Stretch;

            WinUI.Content = content;
        }

        public override void addView(View view)
        {
            addView(view, null);
        }

        public override void addView(View view, LayoutParams param)
        {
            content.Children.Add(view);
        }

        public override void removeView(View view)
        {
            content.Children.Remove(view);
        }
    }
}
