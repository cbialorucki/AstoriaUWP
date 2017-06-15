using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DalvikUWPCSharp.Disassembly.APKParser
{
    public class StaticApkParser : AbstractApkParser
    {
        StorageFolder localAppRoot;

        public StaticApkParser(StorageFolder appRoot)
        {
            localAppRoot = appRoot;
        }

        public override async Task<byte[]> getFileData(string path)
        {
            return await Disassembly.Util.ReadFile(await localAppRoot.GetFileAsync(path));
        }

        protected override Task<byte[]> getCertificateData()
        {
            return null;
        }
    }
}
