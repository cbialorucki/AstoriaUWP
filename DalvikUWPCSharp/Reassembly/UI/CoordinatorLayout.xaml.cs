using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DalvikUWPCSharp.Reassembly.UI
{
    public sealed partial class CoordinatorLayout : UserControl
    {

        public CoordinatorLayout()
        {
            this.InitializeComponent();
        }

        public void Add(UIElement element)
        {
            if (element != null)
            {
                if (element.GetType().Equals(typeof(Grid)))
                {
                    if (((Grid)element).Children.OfType<AndroidToolbar>().FirstOrDefault() != null)
                    {
                        //Toolbar added. Shrink container and add toolbar to top.
                        subContainer.Margin = new Thickness(0, ((Grid)element).Height, 0, 0);
                        ((Grid)element).VerticalAlignment = VerticalAlignment.Top;
                        container.Children.Add(element);
                        return;
                    }
                }

                subContainer.Children.Add(element);
            }
            
        }
    }
}
