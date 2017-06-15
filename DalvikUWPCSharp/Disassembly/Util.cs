using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Streams;
using DalvikUWPCSharp.Applet;
using Windows.UI.Popups;
//using DalvikUWPCSharp.Disassembly.APKParser;
//using DalvikUWPCSharp.Disassembly.APKParser.bean;

namespace DalvikUWPCSharp.Disassembly
{
    public static class Util
    {
        //public static FileActivatedEventArgs CurrentFileActivatedArgs { get; set; }
        public static StorageFile CurrentFile { get; private set; }
        public static Applet.DroidApp CurrentApp { get; private set; }
        public static InstallApkPage apkpage { get; set; }
        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public static async Task LoadAPK(FileActivatedEventArgs e)
        {
            StorageFile sf = (StorageFile)e.Files[0];
            //Debug.WriteLine("When do I get called?");
            //var appsRoot = await localFolder.CreateFolderAsync("Apps", CreationCollisionOption.OpenIfExists);
            //StorageFile copiedFile = await sf.CopyAsync(appsRoot, sf.Name, NameCollisionOption.GenerateUniqueName);

            //ApkParser parser = await ApkParser.CreateAsync(copiedFile);
            //ApkMeta meta = await parser.getApkMeta();
            //apkpage.SetDisplayName(meta.getName());
            apkpage.SetDisplayName(sf.DisplayName);
            CurrentApp = await DroidApp.CreateAsync(sf);
            apkpage.appletLoaded(CurrentApp, EventArgs.Empty);
            //Debug.WriteLine($"cf is null: {CurrentFile == null}");


        }

        public static async Task<byte[]> ReadFile(StorageFile sf)
        {
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await sf.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }

        public static List<string> GeneratePermissions()
        {
            return CurrentApp.metadata.Permissions;
        } 

        public static async Task PurgeAppsFolder()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var appsRoot = await localFolder.CreateFolderAsync("Apps", CreationCollisionOption.OpenIfExists);

            await appsRoot.DeleteAsync();

            var dialog = new MessageDialog("Apps folder purged.");
            await dialog.ShowAsync();
        }

        public static string ConvertPath(string path)
        {
            return "\\" + path.Replace('/', '\\');
        }

        
    }
}
