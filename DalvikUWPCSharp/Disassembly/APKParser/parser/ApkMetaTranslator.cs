using DalvikUWPCSharp.Disassembly.APKParser.bean;
using DalvikUWPCSharp.Disassembly.APKParser.struct_.xml;
using DalvikUWPCSharp.Disassembly.APKParser.utils.xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    class ApkMetaTranslator : XmlStreamer
    {
        private string[] tagStack = new string[100];
        private int depth = 0;
        private ApkMeta apkMeta = new ApkMeta();

        public void onStartTag(XmlNodeStartTag xmlNodeStartTag)
        {
            Attributes attributes = xmlNodeStartTag.getAttributes();
            switch (xmlNodeStartTag.getName())
            {
                case "application":
                    string label = attributes.get("label");
                    if (label != null)
                    {
                        apkMeta.setLabel(label);
                    }
                    string icon = attributes.get("icon");
                    if (icon != null)
                    {
                        apkMeta.setIcon(icon);
                    }
                    break;
                case "manifest":
                    apkMeta.setPackageName(attributes.get("package"));
                    apkMeta.setVersionName(attributes.get("versionName"));
                    apkMeta.setVersionCode(attributes.getLong("versionCode"));
                    string installLocation = attributes.get("installLocation");
                    if (installLocation != null)
                    {
                        apkMeta.setInstallLocation(installLocation);
                    }
                    break;
                case "uses-sdk":
                    apkMeta.setMinSdkVersion(attributes.get("minSdkVersion"));
                    apkMeta.setTargetSdkVersion(attributes.get("targetSdkVersion"));
                    apkMeta.setMaxSdkVersion(attributes.get("maxSdkVersion"));
                    break;
                case "supports-screens":
                    apkMeta.setAnyDensity(attributes.getBoolean("anyDensity", false));
                    apkMeta.setSmallScreens(attributes.getBoolean("smallScreens", false));
                    apkMeta.setNormalScreens(attributes.getBoolean("normalScreens", false));
                    apkMeta.setLargeScreens(attributes.getBoolean("largeScreens", false));
                    break;
                case "uses-feature":
                    string name = attributes.get("name");
                    bool required = attributes.getBoolean("required", false);

                    if (name != null)
                    {
                        UseFeature useFeature = new UseFeature();
                        useFeature.setName(name);
                        useFeature.setRequired(required);
                        apkMeta.addUseFeatures(useFeature);
                    }
                    else
                    {
                        int gl = attributes.getInt("glEsVersion");
                        if (gl != -1)
                        {
                            int v = gl;
                            GlEsVersion glEsVersion = new GlEsVersion();
                            glEsVersion.setMajor(v >> 16);
                            glEsVersion.setMinor(v & 0xffff);
                            glEsVersion.setRequired(required);
                            apkMeta.setGlEsVersion(glEsVersion);
                        }
                    }

                    break;
                case "uses-permission":
                    apkMeta.addUsesPermission(attributes.get("name"));
                    break;
                case "permission":
                    Permission permission = new Permission();
                    permission.setName(attributes.get("name"));
                    permission.setLabel(attributes.get("label"));
                    permission.setIcon(attributes.get("icon"));
                    permission.setGroup(attributes.get("group"));
                    permission.setDescription(attributes.get("description"));
                    string protectionLevel = attributes.get("android:protectionLevel");
                    if (protectionLevel != null)
                    {
                        permission.setProtectionLevel(protectionLevel);
                    }
                    apkMeta.addPermission(permission);
                    break;
            }
            tagStack[depth++] = xmlNodeStartTag.getName();
        }

        public void onEndTag(XmlNodeEndTag xmlNodeEndTag)
        {
            depth--;
        }


        public void onCData(XmlCData xmlCData)
        {

        }

        public void onNamespaceStart(XmlNamespaceStartTag tag)
        {

        }

        public void onNamespaceEnd(XmlNamespaceEndTag tag)
        {

        }

        public ApkMeta getApkMeta()
        {
            return apkMeta;
        }

        private bool matchTagPath(params string[] tags)
        {
            // the root should always be "manifest"
            if (depth != tags.Length + 1)
            {
                return false;
            }
            for (int i = 1; i < depth; i++)
            {
                if (!tagStack[i].Equals(tags[i - 1]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool matchLastTag(string tag)
        {
            // the root should always be "manifest"
            return tagStack[depth - 1].EndsWith(tag);
        }
    }
}
