using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class AttributeValues
    {
        // Activity constants begin. see:
        // http://developer.android.com/reference/android/content/pm/ActivityInfo.html
        // http://developer.android.com/guide/topics/manifest/activity-element.html
        public static string getScreenOrientation(uint value)
        {
            switch (value)
            {
                case 0x00000003:
                    return "behind";
                case 0x0000000a:
                    return "fullSensor";
                case 0x0000000d:
                    return "fullUser";
                case 0x00000000:
                    return "landscape";
                case 0x0000000e:
                    return "locked";
                case 0x00000005:
                    return "nosensor";
                case 0x00000001:
                    return "portrait";
                case 0x00000008:
                    return "reverseLandscape";
                case 0x00000009:
                    return "reversePortrait";
                case 0x00000004:
                    return "sensor";
                case 0x00000006:
                    return "sensorLandscape";
                case 0x00000007:
                    return "sensorPortrait";
                case 0xffffffff:
                    return "unspecified";
                case 0x00000002:
                    return "user";
                case 0x0000000b:
                    return "userLandscape";
                case 0x0000000c:
                    return "userPortrait";
                default:
                    return "ScreenOrientation:" + value.ToString("X");
            }
        }

        public static string getLaunchMode(uint value)
        {
            switch (value)
            {
                case 0x00000000:
                    return "standard";
                case 0x00000001:
                    return "singleTop";
                case 0x00000002:
                    return "singleTask";
                case 0x00000003:
                    return "singleInstance";
                default:
                    return "LaunchMode:" + value.ToString("X");
            }

        }

        public static string getConfigChanges(uint value)
        {
            List<string> list = new List<string>();
            if ((value & 0x00001000) != 0)
            {
                list.Add("density");
            }
            else if ((value & 0x40000000) != 0)
            {
                list.Add("fontScale");
            }
            else if ((value & 0x00000010) != 0)
            {
                list.Add("keyboard");
            }
            else if ((value & 0x00000020) != 0)
            {
                list.Add("keyboardHidden");
            }
            else if ((value & 0x00002000) != 0)
            {
                list.Add("direction");
            }
            else if ((value & 0x00000004) != 0)
            {
                list.Add("locale");
            }
            else if ((value & 0x00000001) != 0)
            {
                list.Add("mcc");
            }
            else if ((value & 0x00000002) != 0)
            {
                list.Add("mnc");
            }
            else if ((value & 0x00000040) != 0)
            {
                list.Add("navigation");
            }
            else if ((value & 0x00000080) != 0)
            {
                list.Add("orientation");
            }
            else if ((value & 0x00000100) != 0)
            {
                list.Add("screenLayout");
            }
            else if ((value & 0x00000400) != 0)
            {
                list.Add("screenSize");
            }
            else if ((value & 0x00000800) != 0)
            {
                list.Add("smallestScreenSize");
            }
            else if ((value & 0x00000008) != 0)
            {
                list.Add("touchscreen");
            }
            else if ((value & 0x00000200) != 0)
            {
                list.Add("uiMode");
            }
            return Utils.join(list, "|");
        }

        public static string getWindowSoftInputMode(uint value)
        {
            uint adjust = value & 0x000000f0;
            uint state = value & 0x0000000f;
            List<string> list = new List<string>(2);
            switch (adjust)
            {
                case 0x00000030:
                    list.Add("adjustNothing");
                    break;
                case 0x00000020:
                    list.Add("adjustPan");
                    break;
                case 0x00000010:
                    list.Add("adjustResize");
                    break;
                case 0x00000000:
                    //levels.Add("adjustUnspecified");
                    break;
                default:
                    list.Add("WindowInputModeAdjust:" + adjust.ToString("X"));
                    break;
            }
            switch (state)
            {
                case 0x00000003:
                    list.Add("stateAlwaysHidden");
                    break;
                case 0x00000005:
                    list.Add("stateAlwaysVisible");
                    break;
                case 0x00000002:
                    list.Add("stateHidden");
                    break;
                case 0x00000001:
                    list.Add("stateUnchanged");
                    break;
                case 0x00000004:
                    list.Add("stateVisible");
                    break;
                case 0x00000000:
                    //levels.Add("stateUnspecified");
                    break;
                default:
                    list.Add("WindowInputModeState:" + state.ToString("X"));
                    break;
            }
            return Utils.join(list, "|");
            //isForwardNavigation(0x00000100),
            //mode_changed(0x00000200),
        }

        //http://developer.android.com/reference/android/content/pm/PermissionInfo.html
        public static string getProtectionLevel(uint value)
        {
            List<string> levels = new List<string>(3);
            if ((value & 0x10) != 0)
            {
                value = value ^ 0x10;
                levels.Add("system");
            }
            if ((value & 0x20) != 0)
            {
                value = value ^ 0x20;
                levels.Add("development");
            }
            switch (value)
            {
                case 0:
                    levels.Add("normal");
                    break;
                case 1:
                    levels.Add("dangerous");
                    break;
                case 2:
                    levels.Add("signature");
                    break;
                case 3:
                    levels.Add("signatureOrSystem");
                    break;
                default:
                    levels.Add("ProtectionLevel:" + value.ToString("X"));
                    break;
            }
            return Utils.join(levels, "|");
        }


        // Activity constants end

        /**
         * get Installation string values from int
         */
        public static string getInstallLocation(uint value)
        {
            switch (value)
            {
                case 0:
                    return "auto";
                case 1:
                    return "internalOnly";
                case 2:
                    return "preferExternal";
                default:
                    return "installLocation:" + value.ToString("X");
            }
        }
    }
}
