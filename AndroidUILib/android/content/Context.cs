using AndroidInteropLib.android.content.res;
using AndroidInteropLib.com.android._internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.content
{
    public abstract class Context
    {
        public const string ACCESSIBILITY_SERVICE = "accessibility";
        public const string ACCOUNT_SERVICE = "account";
        public const string ACTIVITY_SERVICE = "activity";
        public const string ALARM_SERVICE = "alarm";
        public const string APPWIDGET_SERVICE = "appwidget";
        public const string APP_OPS_SERVICE = "appops";
        public const string AUDIO_SERVICE = "audio";
        public const string BATTERY_SERVICE = "batterymanager";
        public const int BIND_ABOVE_CLIENT = 8;
        public const int BIND_ADJUST_WITH_ACTIVITY = 128;
        public const int BIND_ALLOW_OOM_MANAGEMENT = 16;
        public const int BIND_AUTO_CREATE = 1;
        public const int BIND_DEBUG_UNBIND = 2;
        public const int BIND_EXTERNAL_SERVICE = -2147483648;
        public const int BIND_IMPORTANT = 64;
        public const int BIND_NOT_FOREGROUND = 4;
        public const int BIND_WAIVE_PRIORITY = 32;
        public const string BLUETOOTH_SERVICE = "bluetooth";
        public const string CAMERA_SERVICE = "camera";
        public const string LAYOUT_INFLATER_SERVICE = "layout_inflater";

        public abstract Resources getResources();
        public abstract void startActivity(Intent intent);
        public abstract object getSystemService(string name);

        //This is a hack for Astoria. I know on Android R is typically static but we want Astoria to be able to run multiple apps at once one day so I decided to make it an object.
        public abstract R getR();

        public abstract void CallBack(object _params);

    }
}
