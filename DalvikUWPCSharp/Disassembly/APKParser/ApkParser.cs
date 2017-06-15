using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using DalvikUWPCSharp.Disassembly.APKParser.parser;
using System.IO.Compression;
using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System.IO;

namespace DalvikUWPCSharp.Disassembly.APKParser
{
    public class ApkParser : AbstractApkParser
    {
        private ZipArchive zf;
        private StorageFile apkFile;

        private ApkParser(StorageFile apkFile)
        {
            this.apkFile = apkFile;
            // create zip file cost time, use one zip file for apk parser life cycle
            //LoadZip();


        }

        public static async Task<ApkParser> CreateAsync(StorageFile apkFile)
        {
            ApkParser temp = new ApkParser(apkFile);
            await temp.LoadZip();
            return temp;
        }

        public async Task LoadZip()
        {
            byte[] byteArray = await Util.ReadFile(apkFile);
            Stream stream = new MemoryStream(byteArray);
            this.zf = new ZipArchive(stream);
        }

        /*public ApkParser(string filePath)
        {
            //this(new File(filePath));
            ApkParser()
        }*/

        protected override async Task<byte[]> getCertificateData()
        {
            ZipArchiveEntry entry = null;
            //Enumeration<? extends ZipEntry> enu = zf.entries();
            foreach(ZipArchiveEntry ne in zf.Entries)
            {
                if(ne.Name.ToUpper().EndsWith(".RSA") || ne.Name.ToUpper().EndsWith(".DSA"))
                {
                    entry = ne;
                    break;
                }
            }
            /*while (enu.hasMoreElements()) 
            {
                ZipEntry ne = enu.nextElement();
                if (ne.isDirectory())
                {
                    continue;
                }
                if (ne.getName().toUpperCase().endsWith(".RSA") || ne.getName().toUpperCase().endsWith(".DSA"))
                {
                    entry = ne;
                    break;
                }
            }*/

            if (entry == null)
            {
                return null;
            }

            return await Utils.toByteArray(entry.Open().AsInputStream());
        }


        public override async Task<byte[]> getFileData(string path)
        {
            //StorageFile sf = await StorageFile.GetFileFromPathAsync(path);
            //return await Disassembly.Util.ReadFile(apkFile);
            ZipArchiveEntry entry = zf.GetEntry(path);
            if (entry == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                await entry.Open().CopyToAsync(ms);
                return ms.ToArray();
            }
                //InputStream inputStream = zf.getInputStream(entry);
                //return Utils.toByteArray(inputStream);
        }


        //public ApkSignStatus verifyApk()
        //{
            //This relies upon quirks of Java. Ignore for now.

            /*ZipArchiveEntry entry = zf.GetEntry("META-INF/MANIFEST.MF");
            if (entry == null)
            {
                // apk is not signed;
                return ApkSignStatus.notSigned;
            }

            //JarFile jarFile = new JarFile(this.apkFile);
            //Enumeration<JarEntry> entries = jarFile.entries();
            byte[] buffer = new byte[8192];

            foreach(ZipArchiveEntry e in zf.Entries)
            {
                try()
                {

                }
                catch { }
            }

            while (entries.hasMoreElements())
            {
                JarEntry e = entries.nextElement();
                if (e.isDirectory())
                {
                    continue;
                }
                try (InputStream en = jarFile.getInputStream(e)) 
                {
                    // Read in each jar entry. A security exception will be thrown if a signature/digest check fails.
                    int count;
                    while ((count = en.read(buffer, 0, buffer.Length)) != -1) {
                        // Don't care
                    }
                } 
                catch (SecurityException se)
                {
                    return ApkSignStatus.incorrect;
                }
            }*/

            //return ApkSignStatus.signed;
        //}

        public new void close()
        {
            //super.close();
            //zf.close();
            zf.Dispose();
        }
    }
}
