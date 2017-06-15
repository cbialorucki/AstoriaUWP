using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class ApkMeta
    {
        private string packageName;
        private string label;
        private string icon;
        private string versionName;
        private long versionCode;
        private string installLocation;
        private string minSdkVersion;
        private string targetSdkVersion;
        private string maxSdkVersion;
        private GlEsVersion glEsVersion;
        private bool anyDensity;
        private bool smallScreens;
        private bool normalScreens;
        private bool largeScreens;

        private List<string> usesPermissions = new List<string>();
        private List<UseFeature> usesFeatures = new List<UseFeature>();
        private List<Permission> permissions = new List<Permission>();

        public string getPackageName()
        {
            return packageName;
        }

        public void setPackageName(string packageName)
        {
            this.packageName = packageName;
        }

        public string getVersionName()
        {
            return versionName;
        }

        public void setVersionName(string versionName)
        {
            this.versionName = versionName;
        }

        public long getVersionCode()
        {
            return versionCode;
        }

        public void setVersionCode(long versionCode)
        {
            this.versionCode = versionCode;
        }

        public string getMinSdkVersion()
        {
            return minSdkVersion;
        }

        public void setMinSdkVersion(string minSdkVersion)
        {
            this.minSdkVersion = minSdkVersion;
        }

        public string getTargetSdkVersion()
        {
            return targetSdkVersion;
        }

        public void setTargetSdkVersion(string targetSdkVersion)
        {
            this.targetSdkVersion = targetSdkVersion;
        }

        public string getMaxSdkVersion()
        {
            return maxSdkVersion;
        }

        public void setMaxSdkVersion(string maxSdkVersion)
        {
            this.maxSdkVersion = maxSdkVersion;
        }

        public List<string> getUsesPermissions()
        {
            return usesPermissions;
        }

        public void addUsesPermission(string permission)
        {
            this.usesPermissions.Add(permission);
        }

        /**
         * the icon file path in apk
         *
         * @return null if not found
         */
        public string getIcon()
        {
            return icon;
        }

        public void setIcon(string icon)
        {
            this.icon = icon;
        }

        /**
         * alias for getLabel
         */
        public string getName()
        {
            return label;
        }

        /**
         * get the apk's title(name)
         */
        public string getLabel()
        {
            return label;
        }

        public void setLabel(string label)
        {
            this.label = label;
        }

        public bool isAnyDensity()
        {
            return anyDensity;
        }

        public void setAnyDensity(bool anyDensity)
        {
            this.anyDensity = anyDensity;
        }

        public bool isSmallScreens()
        {
            return smallScreens;
        }

        public void setSmallScreens(bool smallScreens)
        {
            this.smallScreens = smallScreens;
        }

        public bool isNormalScreens()
        {
            return normalScreens;
        }

        public void setNormalScreens(bool normalScreens)
        {
            this.normalScreens = normalScreens;
        }

        public bool isLargeScreens()
        {
            return largeScreens;
        }

        public void setLargeScreens(bool largeScreens)
        {
            this.largeScreens = largeScreens;
        }

        public GlEsVersion getGlEsVersion()
        {
            return glEsVersion;
        }

        public void setGlEsVersion(GlEsVersion glEsVersion)
        {
            this.glEsVersion = glEsVersion;
        }

        public List<UseFeature> getUsesFeatures()
        {
            return usesFeatures;
        }

        public void addUseFeatures(UseFeature useFeature)
        {
            this.usesFeatures.Add(useFeature);
        }

        public string getInstallLocation()
        {
            return installLocation;
        }

        public void setInstallLocation(string installLocation)
        {
            this.installLocation = installLocation;
        }

        public void addPermission(Permission permission)
        {
            this.permissions.Add(permission);
        }

        public List<Permission> getPermissions()
        {
            return this.permissions;
        }

        public string tostring()
        {
            return "packageName: \t" + packageName + "\n"
                    + "label: \t" + label + "\n"
                    + "icon: \t" + icon + "\n"
                    + "versionName: \t" + versionName + "\n"
                    + "versionCode: \t" + versionCode + "\n"
                    + "minSdkVersion: \t" + minSdkVersion + "\n"
                    + "targetSdkVersion: \t" + targetSdkVersion + "\n"
                    + "maxSdkVersion: \t" + maxSdkVersion;
        }
    }
}
