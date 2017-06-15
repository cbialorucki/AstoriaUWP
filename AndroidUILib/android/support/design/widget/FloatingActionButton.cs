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
    public class FloatingActionButton : View
    {
        //private int id;
        //private Gravity grav;

        Button source = new Button();

        public FloatingActionButton(Context context) : base(context)
        {
            
        }

        public FloatingActionButton(Context context, AttributeSet attrs) : base(context, attrs)
        {
            
        }

        public override void CreateWinUI(params object[] obj)
        {
            int toolBarRef = (int)(mContext.getR().color.get("colorAccent") ?? -1);
            if (toolBarRef != -1)
            {
                int color = mContext.getResources().getColor(toolBarRef);
                source.Background = new Windows.UI.Xaml.Media.SolidColorBrush(ticomware.interop.Util.IntToColor(color));
            }

            else
            {
                source.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 64, 129));
            }

            source.Height = 56;
            source.Content = "button";
            //default
            
            source.Margin = new Thickness(16);
            source.Click += Source_Click;

            WinUI.Content = source;
        }

        private async void Source_Click(object sender, RoutedEventArgs e)
        {
            mContext.CallBack(this);
            //var result = await dialog.ShowAsync();

        }

        //public Gravity GetGravity() { return grav; }

        //public void SetGravity(Gravity g) { grav = g; }

        //public string GetTypeString() { return "android.support.design.widget.FloatingActionButton"; }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //Execute onClick() event code
        }
    }
}
