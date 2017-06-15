using AndroidInteropLib.android.content;
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
    public class Snackbar : View
    {
        //Would love to use enum instead, but trying to interop with java class :/
        public const int LENGTH_INDEFINITE = -2;
        public const int LENGTH_LONG = 0;
        public const int LENGTH_SHORT = -1;

        private int MSToShow;

        //for WinUI rendering
        //private Grid sourceGrid;

        public Snackbar(Context c):base(c)
        {

        }

        public override void CreateWinUI(params object[] obj)
        {
            //Nothing thus far.
        }

        public Snackbar make(View view, string text, int duration)
        {
            //work up view until find parent, add snackbar to that. CoordinatorLayout has special treatment with snackbar.
            setText(text);

            switch (duration)
            {
                case LENGTH_SHORT:
                    MSToShow = 2000;
                    break;
                case LENGTH_LONG:
                    MSToShow = 3500;
                    break;
                case LENGTH_INDEFINITE:
                    MSToShow = -1;
                    break;
            }

            return this;
        }

        public void setText(string message)
        {
            //DisplayTextBlock.Text = message;
        }

        public async void show()
        {
            WinUI.Visibility = Visibility.Visible;
            //Play Swipe Up animation
            if (MSToShow == -1)
            {
                return;
            }

            else
            {
                await Task.Delay(MSToShow);
                //Play collapse animation
                WinUI.Visibility = Visibility.Collapsed;
                //remove self from viewParent to be garbage collected
            }

        }
    }
}
