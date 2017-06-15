using AndroidInteropLib.android.content;
using AndroidInteropLib.android.util;
using AndroidInteropLib.android.view;
using AndroidInteropLib.org.xmlpull.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AndroidInteropLib.android.widget
{
    public class TextView : View
    {
        TextBlock content = new TextBlock();

        public TextView(Context c, AttributeSet a) : base(c, a)
        {
            
        }

        public override void CreateWinUI(params object[] obj)
        {
            content.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 115, 115, 115));
            content.Margin = new Thickness(16);
            WinUI.Content = content;
            setText("null");

            if(obj[1] != null && obj[1] is AttributeSet)
            {
                AttributeSet a = (AttributeSet)obj[1];
                setText(a.getAttributeValue(XmlPullParser.ANDROID_NAMESPACE, "text"));
            }

            
        }

        public void setText(int resid)
        {
            //not implemented yet.
            //setText(mContext.getResources().);
        }

        public void setText(string text)
        {
            if (text != null)
            {
                content.Text = text;
            }
        }
    }
}
