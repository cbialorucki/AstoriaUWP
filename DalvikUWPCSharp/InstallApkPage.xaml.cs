using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DalvikUWPCSharp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InstallApkPage : Page
    {
        public InstallApkPage()
        {
            this.InitializeComponent();
            Disassembly.Util.apkpage = this;
            //AddDebugMessage($"File Name: {Disassembly.Util.CurrentFile.Name}");
            AddDebugMessage("Loading...");
            //appletLoaded();
            //Applet.Applet.appletLoaded += Applet_appletLoaded;
            //Applet_appletLoaded(null, EventArgs.Empty);
            Applet.DroidApp.appletLoaded += appletLoaded;

        }

        public void SetDisplayName(string s)
        {
            displayTitle.Text = s;
        }

        public void appletLoaded(object sender, EventArgs e)
        {
            installProgbar.Visibility = Visibility.Collapsed;
            installBarChrome.Visibility = Visibility.Visible;

            //displayTitle.Text = Disassembly.Util.CurrentApp.metadata.label;
            //AddDebugMessage("Icon Path: " + Disassembly.Util.CurrentApp.metadata.iconFileName[0]);

            app_image.Source = Disassembly.Util.CurrentApp.appIcon;

            AddDebugMessage("page loaded.");



            foreach (string s in Disassembly.Util.CurrentApp.metadata.Permissions)
            {
                Debug.WriteLine(s);
                AddDebugMessage(s);
            }

        }

        private void AddDebugMessage(string text)
        {
            Description_Textblock.Text += $"\n{text}";
        }

        private void cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Disassembly.Util.CurrentApp.Purge();

            Frame.Navigate(typeof(MainPage));
        }

        private void forcerlbutton_Click(object sender, RoutedEventArgs e)
        {
            appletLoaded("", EventArgs.Empty);
        }

        private async void install_Click(object sender, RoutedEventArgs e)
        {
            //Put up loading screen
            Description_Textblock.Text = "Installing...";
            installBarChrome.Visibility = Visibility.Collapsed;
            installProgbar.Visibility = Visibility.Visible;

            //Install app
            await Disassembly.Util.CurrentApp.Install();

            //Stop install elements and show "Done"/"Open" screen
            installProgbar.Visibility = Visibility.Collapsed;
            installBarChrome.Visibility = Visibility.Visible;
            Description_Textblock.Text = "App installed";
            install_Button.Content = "Open";
            cancel_Button.Content = "Done";

            //remove old button click event handlers and use new ones
            install_Button.Click -= install_Click;
            cancel_Button.Click -= cancel_Button_Click;

            install_Button.Click += Done_Click;
            cancel_Button.Click += Open_Click;
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            //Go to home screen
            Frame.Navigate(typeof(MainPage));
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            //Start app
            Frame.Navigate(typeof(EmuPage));
        }
    }
}
