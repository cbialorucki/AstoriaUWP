using DalvikUWPCSharp.Applet;
using DalvikUWPCSharp.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DalvikUWPCSharp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetTitleBarColor();

            var appsRoot = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Apps", CreationCollisionOption.OpenIfExists);

            foreach(StorageFolder sf in await appsRoot.GetFoldersAsync())
            {
                AppListBox.Items.Add(sf.Name);
            }

            
            //DalvikCPU MainCPU = new DalvikCPU();
            //MainCPU.Code = Util.GetSampleCode();
            //MainCPU.RunVM();


        }

        public void SetTitleBarColor()
        {
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            var titleBar = appView.TitleBar;
            titleBar.BackgroundColor = null;
            titleBar.ButtonBackgroundColor = null;
            titleBar.InactiveBackgroundColor = null;
            titleBar.ButtonInactiveBackgroundColor = null;
            titleBar.ButtonPressedBackgroundColor = null;
            titleBar.ButtonHoverBackgroundColor = null;
            

            appView.Title = string.Empty;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            //appView.Title = UIRenderer.CurrentApp.metadata.label;
            //titleBar.ButtonHoverBackgroundColor = Color.FromArgb(10, 255, 255, 255);

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundColor = null;
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundOpacity = 0;
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = null;
            }

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await Disassembly.Util.PurgeAppsFolder();
        }

        private async void AppListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is string)
            {

                bgGrid.Children.Add(new DroidAppLoadingPopup());

                var appsRoot = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Apps", CreationCollisionOption.OpenIfExists);

                string lbi = (string)e.AddedItems[0];
                DroidApp da = await DroidApp.CreateAsync(await appsRoot.GetFolderAsync(lbi));
                da.Run(Frame);
            }

            else
            {
                Frame.Navigate(typeof(SettingsPage));
            }
        }
    }
}
