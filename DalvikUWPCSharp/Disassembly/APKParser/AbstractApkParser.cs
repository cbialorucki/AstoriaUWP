using DalvikUWPCSharp.Disassembly.APKParser.bean;
using DalvikUWPCSharp.Disassembly.APKParser.exception;
using DalvikUWPCSharp.Disassembly.APKParser.parser;
using DalvikUWPCSharp.Disassembly.APKParser.struct_;
using DalvikUWPCSharp.Disassembly.APKParser.struct_.resource;
using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DalvikUWPCSharp.Disassembly.APKParser
{
    public abstract class AbstractApkParser
    {
        private DexClass[] dexClasses;
        private ResourceTable resourceTable;

        private string manifestXml;
        private ApkMeta apkMeta;
        private HashSet<CultureInfo> locales;
        private List<CertificateMeta> certificateMetaList;

        private static CultureInfo DEFAULT_LOCALE = new CultureInfo("en-us");

        /**
         * default use empty locale
         */
        private CultureInfo preferredLocale = CultureInfo.CurrentCulture;

        /**
         * return decoded AndroidManifest.xml
         *
         * @return decoded AndroidManifest.xml
         */
        public async Task<string> getManifestXml()
        {
            if (this.manifestXml == null)
            {
                await parseManifestXml();
            }
            return this.manifestXml;
        }

            /**
             * return decoded AndroidManifest.xml
             *
             * @return decoded AndroidManifest.xml
             */
            public async Task<ApkMeta> getApkMeta()
            {
                if (this.apkMeta == null)
                {
                    await parseApkMeta();
                }
                return this.apkMeta;
            }

            /**
             * get locales supported from resource file
             *
             * @return decoded AndroidManifest.xml
             * @throws IOException
             */
            public async Task<HashSet<CultureInfo>> getLocales()
            {
                if (this.locales == null)
                {
                    await parseResourceTable();
                }

                return this.locales;
            }

            /**
             * get the apk's certificates.
             */
             //Needs Win10 AU.
            /*public List<CertificateMeta> getCertificateMetaList()
            {
                if (this.certificateMetaList == null)
                {
                    parseCertificate();
                }

                return this.certificateMetaList;
            }*/

            protected abstract Task<byte[]> getCertificateData();

            //Needs Aniversary Update to run. Making this on an RTM Win10 machine so i'll just disable it for now.
            /*private async void parseCertificate()
            {
                byte[] data = await getCertificateData();
                if (data == null)
                {
                    throw new Exception("ApkParser certificate not found");
                }

                CertificateParser parser = new CertificateParser(data);
                parser.parse();
                this.certificateMetaList = parser.getCertificateMetas();
            }*/

            /**
             * parse manifest.xml, get apkMeta.
             *
             * @throws IOException
             */
            private async Task parseApkMeta()
            {
                if (this.manifestXml == null) 
                {
                    await parseManifestXml();
                }
            }

            /**
             * parse manifest.xml, get manifestXml as xml text.
             *
             * @throws IOException
             */
            private async Task parseManifestXml()
            {
                XmlTranslator xmlTranslator = new XmlTranslator();
                ApkMetaTranslator translator = new ApkMetaTranslator();
                XmlStreamer xmlStreamer = new CompositeXmlStreamer(xmlTranslator, translator);

                byte[] data = await getFileData(AndroidConstants.MANIFEST_FILE);
                if (data == null) 
                {
                    throw new ParserException("Manifest file not found");
                }
                await transBinaryXml(data, xmlStreamer);
                this.manifestXml = xmlTranslator.getXml();
                this.apkMeta = translator.getApkMeta();
            }

            /**
             * read file in apk into bytes
             */
            public abstract Task<byte[]> getFileData(string path);

            public async Task<string> transBinaryXml(StorageFile sf)
            {
                byte[] data = await Util.ReadFile(sf);
                if (data == null)
                {
                    return null;
                }
                if (this.resourceTable == null)
                {
                    await parseResourceTable();
                }

                XmlTranslator xmlTranslator = new XmlTranslator();
                await transBinaryXml(data, xmlTranslator);
                return xmlTranslator.getXml();
            }

            /**
                * trans binary xml file to text xml file.
                *
                * @param path the xml file path in apk file
                * @return the text. null if file not exists
                * @throws IOException
                */
            public async Task<string> transBinaryXml(string path)
            {
                byte[] data = await getFileData(path);
                if (data == null)
                {
                    return null;
                }
                if (this.resourceTable == null)
                {
                    await parseResourceTable();
                }

                XmlTranslator xmlTranslator = new XmlTranslator();
                await transBinaryXml(data, xmlTranslator);
                return xmlTranslator.getXml();
            }

            private async Task transBinaryXml(byte[] data, XmlStreamer xmlStreamer)
            {
                if (this.resourceTable == null)
                {
                    await parseResourceTable();
                }

                ByteBuffer buffer = ByteBuffer.wrap(data);
                BinaryXmlParser binaryXmlParser = new BinaryXmlParser(buffer, resourceTable);
                binaryXmlParser.setLocale(preferredLocale);
                binaryXmlParser.setXmlStreamer(xmlStreamer);
                await binaryXmlParser.parse();
            }

            /**
             * get the apk icon file as bytes.
             *
             * @return the apk icon data,null if icon not found
             * @throws IOException
             */
            public async Task<Icon> getIconFile()
            {
                ApkMeta apkMeta = await getApkMeta();
                string iconPath = apkMeta.getIcon();
                if (iconPath == null)
                {
                    return null;
                }
                return new Icon(iconPath, await getFileData(iconPath));
            }

            /**
             * get class infos form dex file. currently only class name
             */
            public async Task<DexClass[]> getDexClasses()
            {
                if (this.dexClasses == null)
                {
                    await parseDexFile();
                }

                return this.dexClasses;
            }

            private async Task parseDexFile()
            {
                byte[] data = await getFileData(AndroidConstants.DEX_FILE);
                if (data == null) 
                {
                    throw new Exception("Dex file not found");
                }
                ByteBuffer buffer = ByteBuffer.wrap(data);
                DexParser dexParser = new DexParser(buffer);
                dexParser.parse();
                this.dexClasses = dexParser.getDexClasses();
            }

            /**
             * parse resource table.
             */
            private async Task parseResourceTable()
            {
                byte[] data = await getFileData(AndroidConstants.RESOURCE_FILE);
                //byte[] data = await getFileData(Disassembly.Util.CurrentApp.localAppRoot.Path + "\\" + AndroidConstants.RESOURCE_FILE);
                if (data == null)
                {
                    // if no resource entry has been found, we assume it is not needed by this APK
                    this.resourceTable = new ResourceTable();
                    //this.locales = Collections.emptySet();
                    this.locales = new HashSet<CultureInfo>();
                    return;
                }

                this.resourceTable = new ResourceTable();
                //this.locales = Collections.emptySet();
                this.locales = new HashSet<CultureInfo>();

                ByteBuffer buffer = ByteBuffer.wrap(data);
                ResourceTableParser resourceTableParser = new ResourceTableParser(buffer);
                await resourceTableParser.parse();
                this.resourceTable = resourceTableParser.getResourceTable();
                this.locales = resourceTableParser.getLocales();
                //possible mem leak
                buffer.Dispose();
            }

            /**
             * check apk sign
             *
             * @throws IOException
             */
             //Needs Win10 AU
            //public abstract ApkSignStatus verifyApk();

            public void close()
            {
                this.certificateMetaList = null;
                this.resourceTable = null;
                this.certificateMetaList = null;
            }

            public CultureInfo getPreferredLocale()
            {
                return preferredLocale;
            }


            /**
             * The locale preferred. Will cause getManifestXml / getApkMeta to return different values.
             * The default value is from os default locale setting.
             */
            public void setPreferredLocale(CultureInfo passedLocale)
            {
                if (!Equals(this.preferredLocale, preferredLocale))
                {
                    this.preferredLocale = passedLocale;
                    this.manifestXml = null;
                    this.apkMeta = null;
                }
            }
    }
}
