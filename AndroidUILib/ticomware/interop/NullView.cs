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
using Windows.UI.Xaml.Media;

namespace AndroidInteropLib.ticomware.interop
{
    public class NullView : View
    {
        TextBlock tb = new TextBlock();

        public NullView(Context c, AttributeSet a):base(c,a)
        {
            
        }

        public override void CreateWinUI(params object[] obj)
        {
            WinUI.Background = new SolidColorBrush(Windows.UI.Colors.SkyBlue);
            tb.Text = "NULL";
            WinUI.Content = tb;
        }

        public void setText(string message)
        {
            tb.Text = message;
        }
    }
}
