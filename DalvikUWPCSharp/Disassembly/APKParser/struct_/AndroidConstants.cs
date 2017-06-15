using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.struct_
{
    public class AndroidConstants
    {
        public static readonly string RESOURCE_FILE = "resources.arsc";

        public static readonly string MANIFEST_FILE = "AndroidManifest.xml";

        public static readonly string DEX_FILE = "classes.dex";

        public static readonly string RES_PREFIX = "res/";

        public static readonly string ASSETS_PREFIX = "assets/";

        public static readonly string LIB_PREFIX = "lib/";

        public static readonly string META_PREFIX = "META-INF/";

        public static readonly string ARCH_ARMEABI = "";
        /**
         * the binary xml file used system attr id.
         */
        public static readonly int ATTR_ID_START = 0x01010000;

        /**
         * start offset for system android.R.style
         */
        public static readonly int SYS_STYLE_ID_START = 0x01030000;

        /**
         * end offset for system android.R.style
         */
        public static readonly int SYS_STYLE_ID_END = 0x01031000;
    }
}
