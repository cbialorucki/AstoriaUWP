using AndroidInteropLib.android.app;
using AndroidInteropLib.android.content;
using AndroidInteropLib.android.util;
using AndroidInteropLib.android.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AndroidInteropLib.android.support.v7.widget
{
    public class Toolbar : ViewGroup
    {

        Grid SourceGrid = new Grid();
        MenuFlyout contextMenu = new MenuFlyout();

        public Toolbar(Context c, AttributeSet a) : base(c, a)
        {
            
        }

        public override void CreateWinUI(params object[] obj)
        {
            int toolBarRef = (int)(mContext.getR().color.get("colorPrimary") ?? -1);
            if (toolBarRef != -1)
            {
                int color = mContext.getResources().getColor(toolBarRef);
                SourceGrid.Background = new SolidColorBrush(ticomware.interop.Util.IntToColor(color));
            }

            TextBlock titleText = new TextBlock();
            titleText.Text = "Test";
            titleText.VerticalAlignment = VerticalAlignment.Center;
            titleText.Padding = new Thickness(16, 0, 0, 0);

            SourceGrid.Children.Add(titleText);

            Button bttn = new Button();
            bttn.Content = ":";
            bttn.HorizontalAlignment = HorizontalAlignment.Right;
            bttn.VerticalAlignment = VerticalAlignment.Center;
            bttn.Margin = new Thickness(16, 0, 16, 0);

            bttn.Click += Bttn_Click;

            SourceGrid.Children.Add(bttn);

            WinUI.Content = SourceGrid;

            /*MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "First item" };
            MenuFlyoutItem secondItem = new MenuFlyoutItem { Text = "Second item" };
            MenuFlyoutSubItem subItem = new MenuFlyoutSubItem { Text = "Other items" };
            MenuFlyoutItem item1 = new MenuFlyoutItem { Text = "First sub item" };
            MenuFlyoutItem item2 = new MenuFlyoutItem { Text = "Second sub item" };
            subItem.Items.Add(item1);
            subItem.Items.Add(item2);
            contextMenu.Items.Add(firstItem);
            contextMenu.Items.Add(secondItem);
            contextMenu.Items.Add(subItem);*/
        }

        private void Bttn_Click(object sender, RoutedEventArgs e)
        {            
            FrameworkElement senderElement = sender as FrameworkElement;
            contextMenu.ShowAt(senderElement);
        }

        public void setTitle(string title)
        {
            //TitleBlock.Text = title;
        }

        public override void addView(View view, ViewGroup.LayoutParams param)
        {

        }

        public override void addView(View view)
        {

        }

        public override void removeView(View view)
        {

        }

        public new class LayoutParams : ActionBar.LayoutParams
        {

        }
    }
}
