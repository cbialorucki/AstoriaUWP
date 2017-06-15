using AndroidInteropLib.android.content;
using AndroidInteropLib.android.util;
using AndroidInteropLib.android.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AndroidInteropLib.android.support.design.widget
{
    public class CoordinatorLayout : ViewGroup
    {
        private bool isAppBarAdded = false;
        Grid sourceGrid = new Grid();

        public CoordinatorLayout(Context c, AttributeSet a) : base(c, a)
        {
            
        }

        public override void CreateWinUI(params object[] obj)
        {
            //Windows will not render Coordinator layout w/o a background or objects in it.
            int ResID = (int)(mContext.getR().color.get("windowBackground") ?? -1);
            if (ResID != -1)
            {
                int color = mContext.getResources().getColor(ResID);
                sourceGrid.Background = new SolidColorBrush(ticomware.interop.Util.IntToColor(color));
            }
            else
            {
                sourceGrid.Background = new SolidColorBrush(Windows.UI.Colors.White);
            }

            sourceGrid.VerticalAlignment = VerticalAlignment.Stretch;
            sourceGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            WinUI.Content = sourceGrid;
        }

        public override void addView(View view, ViewGroup.LayoutParams param)
        {
            if (view is AppBarLayout && !isAppBarAdded)
            {
                view.WinUI.VerticalAlignment = VerticalAlignment.Top;
                view.WinUI.Height = 56;
                view.WinUI.HorizontalAlignment = HorizontalAlignment.Stretch;
                view.WinUI.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                //view's content is a grid with toolbar
                //appBarHolder.Children.Add(((Grid)view.Content).Children.OfType<v7.widget.Toolbar>().FirstOrDefault());
                sourceGrid.Children.Add(view);
                //view.SetValue(Grid.RowProperty, 0);

                isAppBarAdded = true;
            }

            else if(isAppBarAdded)
            {
                view.WinUI.Margin = new Thickness(0, 56, 0, 0);

                sourceGrid.Children.Add(view);
                //view.SetValue(Grid.RowProperty, 1);
            }


        }

        public override void addView(View view)
        {
            addView(view, null);

        }

        public override void removeView(View view)
        {
            sourceGrid.Children.Remove(view);
        }
    }
}
