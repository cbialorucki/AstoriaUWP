using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AndroidInteropLib.android.content
{
    public class Intent
    {
        private const string ATTR_ACTION = "action";
        private const string TAG_CATEGORIES = "categories";
        private const string ATTR_CATEGORY = "category";
        private const string TAG_EXTRA = "extra";
        private const string ATTR_TYPE = "type";
        private const string ATTR_COMPONENT = "component";
        private const string ATTR_DATA = "data";
        private const string ATTR_FLAGS = "flags";
        // ---------------------------------------------------------------------
        // ---------------------------------------------------------------------
        // Standard intent activity actions (see action variable).
        public const string ACTION_MAIN = "android.intent.action.MAIN";
        public const string ACTION_VIEW = "android.intent.action.VIEW";
        public const string ACTION_DEFAULT = ACTION_VIEW;
        public const string ACTION_ATTACH_DATA = "android.intent.action.ATTACH_DATA";
        public const string ACTION_EDIT = "android.intent.action.EDIT";
        public const string ACTION_INSERT_OR_EDIT = "android.intent.action.INSERT_OR_EDIT";
        public const string ACTION_PICK = "android.intent.action.PICK";
        public const string ACTION_CREATE_SHORTCUT = "android.intent.action.CREATE_SHORTCUT";
        public const string EXTRA_SHORTCUT_INTENT = "android.intent.extra.shortcut.INTENT";
        public const string EXTRA_SHORTCUT_NAME = "android.intent.extra.shortcut.NAME";  
        public const string EXTRA_SHORTCUT_ICON = "android.intent.extra.shortcut.ICON";
        public const string EXTRA_SHORTCUT_ICON_RESOURCE = "android.intent.extra.shortcut.ICON_RESOURCE";
        public const string ACTION_CHOOSER = "android.intent.action.CHOOSER";
        public const string ACTION_GET_CONTENT = "android.intent.action.GET_CONTENT";
        public const string ACTION_DIAL = "android.intent.action.DIAL";
        public const string ACTION_CALL = "android.intent.action.CALL";
        public const string ACTION_CALL_EMERGENCY = "android.intent.action.CALL_EMERGENCY";
        public const string ACTION_CALL_PRIVILEGED = "android.intent.action.CALL_PRIVILEGED";
        public const string ACTION_SENDTO = "android.intent.action.SENDTO";
        public const string ACTION_SEND = "android.intent.action.SEND";
        public const string ACTION_SEND_MULTIPLE = "android.intent.action.SEND_MULTIPLE";
        public const string ACTION_ANSWER = "android.intent.action.ANSWER";
        public const string ACTION_INSERT = "android.intent.action.INSERT";
        public const string ACTION_PASTE = "android.intent.action.PASTE";
        public const string ACTION_DELETE = "android.intent.action.DELETE";
        public const string ACTION_RUN = "android.intent.action.RUN";
        public const string ACTION_SYNC = "android.intent.action.SYNC";
        public const string ACTION_PICK_ACTIVITY = "android.intent.action.PICK_ACTIVITY";
        public const string ACTION_SEARCH = "android.intent.action.SEARCH";
        public const string ACTION_SYSTEM_TUTORIAL = "android.intent.action.SYSTEM_TUTORIAL";
        public const string ACTION_WEB_SEARCH = "android.intent.action.WEB_SEARCH";
        public const string ACTION_ASSIST = "android.intent.action.ASSIST";
        public const string ACTION_VOICE_ASSIST = "android.intent.action.VOICE_ASSIST";
        public const string EXTRA_ASSIST_PACKAGE = "android.intent.extra.ASSIST_PACKAGE";
        public const string EXTRA_ASSIST_CONTEXT = "android.intent.extra.ASSIST_CONTEXT";
        public const string EXTRA_ASSIST_INPUT_HINT_KEYBOARD = "android.intent.extra.ASSIST_INPUT_HINT_KEYBOARD";
        public const string ACTION_ALL_APPS = "android.intent.action.ALL_APPS";
        public const string ACTION_SET_WALLPAPER = "android.intent.action.SET_WALLPAPER";
        public const string ACTION_BUG_REPORT = "android.intent.action.BUG_REPORT";
        public const string ACTION_FACTORY_TEST = "android.intent.action.FACTORY_TEST";
        public const string ACTION_CALL_BUTTON = "android.intent.action.CALL_BUTTON";
        public const string ACTION_VOICE_COMMAND = "android.intent.action.VOICE_COMMAND";
        public const string ACTION_SEARCH_LONG_PRESS = "android.intent.action.SEARCH_LONG_PRESS";
        public const string ACTION_APP_ERROR = "android.intent.action.APP_ERROR";
        public const string ACTION_POWER_USAGE_SUMMARY = "android.intent.action.POWER_USAGE_SUMMARY";
        public const string ACTION_UPGRADE_SETUP = "android.intent.action.UPGRADE_SETUP";
        public const string ACTION_MANAGE_NETWORK_USAGE = "android.intent.action.MANAGE_NETWORK_USAGE";
        public const string ACTION_INSTALL_PACKAGE = "android.intent.action.INSTALL_PACKAGE";
        public const string EXTRA_INSTALLER_PACKAGE_NAME = "android.intent.extra.INSTALLER_PACKAGE_NAME";
        public const string EXTRA_NOT_UNKNOWN_SOURCE = "android.intent.extra.NOT_UNKNOWN_SOURCE";
        public const string EXTRA_ORIGINATING_URI = "android.intent.extra.ORIGINATING_URI";
        public const string EXTRA_REFERRER = "android.intent.extra.REFERRER";
        public const string EXTRA_REFERRER_NAME = "android.intent.extra.REFERRER_NAME";
        public const string EXTRA_ORIGINATING_UID = "android.intent.extra.ORIGINATING_UID";
        public const string EXTRA_ALLOW_REPLACE = "android.intent.extra.ALLOW_REPLACE";
        public const string EXTRA_RETURN_RESULT = "android.intent.extra.RETURN_RESULT";
        public const string EXTRA_INSTALL_RESULT = "android.intent.extra.INSTALL_RESULT";
        public const string ACTION_UNINSTALL_PACKAGE = "android.intent.action.UNINSTALL_PACKAGE";
        public const string EXTRA_UNINSTALL_ALL_USERS = "android.intent.extra.UNINSTALL_ALL_USERS";
        public const string METADATA_SETUP_VERSION = "android.SETUP_VERSION";
        // ---------------------------------------------------------------------
        // ---------------------------------------------------------------------
        // Standard intent broadcast actions (see action variable).
        public const string ACTION_SCREEN_OFF = "android.intent.action.SCREEN_OFF";
        public const string ACTION_SCREEN_ON = "android.intent.action.SCREEN_ON";
        public const string ACTION_DREAMING_STOPPED = "android.intent.action.DREAMING_STOPPED";
        public const string ACTION_DREAMING_STARTED = "android.intent.action.DREAMING_STARTED";
        public const string ACTION_USER_PRESENT = "android.intent.action.USER_PRESENT";
        public const string ACTION_TIME_TICK = "android.intent.action.TIME_TICK";
        public const string ACTION_TIME_CHANGED = "android.intent.action.TIME_SET";
        public const string ACTION_DATE_CHANGED = "android.intent.action.DATE_CHANGED";
        public const string ACTION_TIMEZONE_CHANGED = "android.intent.action.TIMEZONE_CHANGED";
        public const string ACTION_CLEAR_DNS_CACHE = "android.intent.action.CLEAR_DNS_CACHE";
        public const string ACTION_ALARM_CHANGED = "android.intent.action.ALARM_CHANGED";
        public const string ACTION_SYNC_STATE_CHANGED = "android.intent.action.SYNC_STATE_CHANGED";
        public const string ACTION_BOOT_COMPLETED = "android.intent.action.BOOT_COMPLETED";
        public const string ACTION_CLOSE_SYSTEM_DIALOGS = "android.intent.action.CLOSE_SYSTEM_DIALOGS";
        public const string ACTION_PACKAGE_INSTALL = "android.intent.action.PACKAGE_INSTALL";
        public const string ACTION_PACKAGE_ADDED = "android.intent.action.PACKAGE_ADDED";
        public const string ACTION_PACKAGE_REPLACED = "android.intent.action.PACKAGE_REPLACED";
        public const string ACTION_MY_PACKAGE_REPLACED = "android.intent.action.MY_PACKAGE_REPLACED";
        public const string ACTION_PACKAGE_REMOVED = "android.intent.action.PACKAGE_REMOVED";
        public const string ACTION_PACKAGE_FULLY_REMOVED = "android.intent.action.PACKAGE_FULLY_REMOVED";
        public const string ACTION_PACKAGE_CHANGED = "android.intent.action.PACKAGE_CHANGED";
        public const string ACTION_QUERY_PACKAGE_RESTART = "android.intent.action.QUERY_PACKAGE_RESTART";
        public const string ACTION_PACKAGE_RESTARTED = "android.intent.action.PACKAGE_RESTARTED";
        public const string ACTION_PACKAGE_DATA_CLEARED = "android.intent.action.PACKAGE_DATA_CLEARED";
        public const string ACTION_UID_REMOVED = "android.intent.action.UID_REMOVED";
        public const string ACTION_PACKAGE_FIRST_LAUNCH = "android.intent.action.PACKAGE_FIRST_LAUNCH";
        public const string ACTION_PACKAGE_NEEDS_VERIFICATION = "android.intent.action.PACKAGE_NEEDS_VERIFICATION";
        public const string ACTION_PACKAGE_VERIFIED = "android.intent.action.PACKAGE_VERIFIED";
        public const string ACTION_EXTERNAL_APPLICATIONS_AVAILABLE = "android.intent.action.EXTERNAL_APPLICATIONS_AVAILABLE";
        public const string ACTION_EXTERNAL_APPLICATIONS_UNAVAILABLE = "android.intent.action.EXTERNAL_APPLICATIONS_UNAVAILABLE";
        public const string ACTION_WALLPAPER_CHANGED = "android.intent.action.WALLPAPER_CHANGED";
        public const string ACTION_CONFIGURATION_CHANGED = "android.intent.action.CONFIGURATION_CHANGED";
        public const string ACTION_LOCALE_CHANGED = "android.intent.action.LOCALE_CHANGED";
        public const string ACTION_BATTERY_CHANGED = "android.intent.action.BATTERY_CHANGED";
        public const string ACTION_BATTERY_LOW = "android.intent.action.BATTERY_LOW";
        public const string ACTION_BATTERY_OKAY = "android.intent.action.BATTERY_OKAY";
        public const string ACTION_POWER_CONNECTED = "android.intent.action.ACTION_POWER_CONNECTED";
        public const string ACTION_POWER_DISCONNECTED = "android.intent.action.ACTION_POWER_DISCONNECTED";
        public const string ACTION_SHUTDOWN = "android.intent.action.ACTION_SHUTDOWN";
        public const string ACTION_REQUEST_SHUTDOWN = "android.intent.action.ACTION_REQUEST_SHUTDOWN";
        public const string ACTION_DEVICE_STORAGE_LOW = "android.intent.action.DEVICE_STORAGE_LOW";
        public const string ACTION_DEVICE_STORAGE_OK = "android.intent.action.DEVICE_STORAGE_OK";
        public const string ACTION_DEVICE_STORAGE_FULL = "android.intent.action.DEVICE_STORAGE_FULL";
        public const string ACTION_DEVICE_STORAGE_NOT_FULL = "android.intent.action.DEVICE_STORAGE_NOT_FULL";
        public const string ACTION_MANAGE_PACKAGE_STORAGE = "android.intent.action.MANAGE_PACKAGE_STORAGE";
        public const string ACTION_UMS_CONNECTED = "android.intent.action.UMS_CONNECTED";
        public const string ACTION_UMS_DISCONNECTED = "android.intent.action.UMS_DISCONNECTED";
        public const string ACTION_MEDIA_REMOVED = "android.intent.action.MEDIA_REMOVED";
        public const string ACTION_MEDIA_UNMOUNTED = "android.intent.action.MEDIA_UNMOUNTED";
        public const string ACTION_MEDIA_CHECKING = "android.intent.action.MEDIA_CHECKING";
        public const string ACTION_MEDIA_NOFS = "android.intent.action.MEDIA_NOFS";
        public const string ACTION_MEDIA_MOUNTED = "android.intent.action.MEDIA_MOUNTED";
        public const string ACTION_MEDIA_SHARED = "android.intent.action.MEDIA_SHARED";
        public const string ACTION_MEDIA_UNSHARED = "android.intent.action.MEDIA_UNSHARED";
        public const string ACTION_MEDIA_BAD_REMOVAL = "android.intent.action.MEDIA_BAD_REMOVAL";
        public const string ACTION_MEDIA_UNMOUNTABLE = "android.intent.action.MEDIA_UNMOUNTABLE";
        public const string ACTION_MEDIA_EJECT = "android.intent.action.MEDIA_EJECT";
        public const string ACTION_MEDIA_SCANNER_STARTED = "android.intent.action.MEDIA_SCANNER_STARTED";
        public const string ACTION_MEDIA_SCANNER_FINISHED = "android.intent.action.MEDIA_SCANNER_FINISHED";
        public const string ACTION_MEDIA_SCANNER_SCAN_FILE = "android.intent.action.MEDIA_SCANNER_SCAN_FILE";
        public const string ACTION_MEDIA_BUTTON = "android.intent.action.MEDIA_BUTTON";
        public const string ACTION_CAMERA_BUTTON = "android.intent.action.CAMERA_BUTTON";
        // *** NOTE: 
        // location; they are not general-purpose actions.
        public const string ACTION_GTALK_SERVICE_CONNECTED = "android.intent.action.GTALK_CONNECTED";
        public const string ACTION_GTALK_SERVICE_DISCONNECTED = "android.intent.action.GTALK_DISCONNECTED";
        public const string ACTION_INPUT_METHOD_CHANGED = "android.intent.action.INPUT_METHOD_CHANGED";
        public const string ACTION_AIRPLANE_MODE_CHANGED = "android.intent.action.AIRPLANE_MODE";
        public const string ACTION_PROVIDER_CHANGED = "android.intent.action.PROVIDER_CHANGED";
        //public const string ACTION_HEADSET_PLUG = android.media.AudioManager.ACTION_HEADSET_PLUG;
        public const string ACTION_ADVANCED_SETTINGS_CHANGED = "android.intent.action.ADVANCED_SETTINGS";
        public const string ACTION_APPLICATION_RESTRICTIONS_CHANGED = "android.intent.action.APPLICATION_RESTRICTIONS_CHANGED";
        public const string ACTION_NEW_OUTGOING_CALL = "android.intent.action.NEW_OUTGOING_CALL";
        public const string ACTION_REBOOT = "android.intent.action.REBOOT";
        public const string ACTION_DOCK_EVENT = "android.intent.action.DOCK_EVENT";
        public const string ACTION_IDLE_MAINTENANCE_START = "android.intent.action.ACTION_IDLE_MAINTENANCE_START";
        public const string ACTION_IDLE_MAINTENANCE_END = "android.intent.action.ACTION_IDLE_MAINTENANCE_END";
        public const string ACTION_REMOTE_INTENT = "com.google.android.c2dm.intent.RECEIVE";
        public const string ACTION_PRE_BOOT_COMPLETED = "android.intent.action.PRE_BOOT_COMPLETED";
        public const string ACTION_GET_RESTRICTION_ENTRIES = "android.intent.action.GET_RESTRICTION_ENTRIES";
        public const string ACTION_RESTRICTIONS_CHALLENGE = "android.intent.action.RESTRICTIONS_CHALLENGE";
        public const string ACTION_USER_INITIALIZE = "android.intent.action.USER_INITIALIZE";
        public const string ACTION_USER_FOREGROUND = "android.intent.action.USER_FOREGROUND";
        public const string ACTION_USER_BACKGROUND = "android.intent.action.USER_BACKGROUND";
        public const string ACTION_USER_ADDED = "android.intent.action.USER_ADDED";
        public const string ACTION_USER_STARTED = "android.intent.action.USER_STARTED";
        public const string ACTION_USER_STARTING = "android.intent.action.USER_STARTING";
        public const string ACTION_USER_STOPPING = "android.intent.action.USER_STOPPING";
        public const string ACTION_USER_STOPPED = "android.intent.action.USER_STOPPED";
        public const string ACTION_USER_REMOVED = "android.intent.action.USER_REMOVED";
        public const string ACTION_USER_SWITCHED = "android.intent.action.USER_SWITCHED";
        public const string ACTION_USER_INFO_CHANGED = "android.intent.action.USER_INFO_CHANGED";
        public const string ACTION_MANAGED_PROFILE_ADDED = "android.intent.action.MANAGED_PROFILE_ADDED";
        public const string ACTION_MANAGED_PROFILE_REMOVED = "android.intent.action.MANAGED_PROFILE_REMOVED";
        public const string ACTION_QUICK_CLOCK = "android.intent.action.QUICK_CLOCK";
        public const string ACTION_SHOW_BRIGHTNESS_DIALOG = "android.intent.action.SHOW_BRIGHTNESS_DIALOG";
        public const string ACTION_GLOBAL_BUTTON = "android.intent.action.GLOBAL_BUTTON";
        public const string ACTION_OPEN_DOCUMENT = "android.intent.action.OPEN_DOCUMENT";
        public const string ACTION_CREATE_DOCUMENT = "android.intent.action.CREATE_DOCUMENT";
        public const string ACTION_OPEN_DOCUMENT_TREE = "android.intent.action.OPEN_DOCUMENT_TREE";
        public const string ACTION_MASTER_CLEAR = "android.intent.action.MASTER_CLEAR";
        // ---------------------------------------------------------------------
        // ---------------------------------------------------------------------
        // Standard intent categories (see addCategory()).
        public const string CATEGORY_DEFAULT = "android.intent.category.DEFAULT";
        public const string CATEGORY_BROWSABLE = "android.intent.category.BROWSABLE";
        public const string CATEGORY_VOICE = "android.intent.category.VOICE";
        public const string CATEGORY_ALTERNATIVE = "android.intent.category.ALTERNATIVE";
        public const string CATEGORY_SELECTED_ALTERNATIVE = "android.intent.category.SELECTED_ALTERNATIVE";
        public const string CATEGORY_TAB = "android.intent.category.TAB";
        public const string CATEGORY_LAUNCHER = "android.intent.category.LAUNCHER";
        public const string CATEGORY_LEANBACK_LAUNCHER = "android.intent.category.LEANBACK_LAUNCHER";
        public const string CATEGORY_LEANBACK_SETTINGS = "android.intent.category.LEANBACK_SETTINGS";
        public const string CATEGORY_INFO = "android.intent.category.INFO";
        public const string CATEGORY_HOME = "android.intent.category.HOME";
        public const string CATEGORY_PREFERENCE = "android.intent.category.PREFERENCE";
        public const string CATEGORY_DEVELOPMENT_PREFERENCE = "android.intent.category.DEVELOPMENT_PREFERENCE";
        public const string CATEGORY_EMBED = "android.intent.category.EMBED";
        public const string CATEGORY_APP_MARKET = "android.intent.category.APP_MARKET";
        public const string CATEGORY_MONKEY = "android.intent.category.MONKEY";
        public const string CATEGORY_TEST = "android.intent.category.TEST";
        public const string CATEGORY_UNIT_TEST = "android.intent.category.UNIT_TEST";
        public const string CATEGORY_SAMPLE_CODE = "android.intent.category.SAMPLE_CODE";
        public const string CATEGORY_OPENABLE = "android.intent.category.OPENABLE";
        public const string CATEGORY_FRAMEWORK_INSTRUMENTATION_TEST = "android.intent.category.FRAMEWORK_INSTRUMENTATION_TEST";
        public const string CATEGORY_CAR_DOCK = "android.intent.category.CAR_DOCK";
        public const string CATEGORY_DESK_DOCK = "android.intent.category.DESK_DOCK";
        public const string CATEGORY_LE_DESK_DOCK = "android.intent.category.LE_DESK_DOCK";
        public const string CATEGORY_HE_DESK_DOCK = "android.intent.category.HE_DESK_DOCK";
        public const string CATEGORY_CAR_MODE = "android.intent.category.CAR_MODE";
        // ---------------------------------------------------------------------
        // ---------------------------------------------------------------------
        // Application launch intent categories (see addCategory()).
        public const string CATEGORY_APP_BROWSER = "android.intent.category.APP_BROWSER";
        public const string CATEGORY_APP_CALCULATOR = "android.intent.category.APP_CALCULATOR";
        public const string CATEGORY_APP_CALENDAR = "android.intent.category.APP_CALENDAR";
        public const string CATEGORY_APP_CONTACTS = "android.intent.category.APP_CONTACTS";
        public const string CATEGORY_APP_EMAIL = "android.intent.category.APP_EMAIL";
        public const string CATEGORY_APP_GALLERY = "android.intent.category.APP_GALLERY";
        public const string CATEGORY_APP_MAPS = "android.intent.category.APP_MAPS";
        public const string CATEGORY_APP_MESSAGING = "android.intent.category.APP_MESSAGING";
        public const string CATEGORY_APP_MUSIC = "android.intent.category.APP_MUSIC";
        // ---------------------------------------------------------------------
        // ---------------------------------------------------------------------
        // Standard extra data keys.
        public const string EXTRA_TEMPLATE = "android.intent.extra.TEMPLATE";
        public const string EXTRA_TEXT = "android.intent.extra.TEXT";
        public const string EXTRA_HTML_TEXT = "android.intent.extra.HTML_TEXT";
        public const string EXTRA_STREAM = "android.intent.extra.STREAM";
        public const string EXTRA_EMAIL       = "android.intent.extra.EMAIL";
        public const string EXTRA_CC       = "android.intent.extra.CC";
        public const string EXTRA_BCC      = "android.intent.extra.BCC";
        public const string EXTRA_SUBJECT  = "android.intent.extra.SUBJECT";
        public const string EXTRA_INTENT = "android.intent.extra.INTENT";
        public const string EXTRA_TITLE = "android.intent.extra.TITLE";
        public const string EXTRA_INITIAL_INTENTS = "android.intent.extra.INITIAL_INTENTS";
        public const string EXTRA_REPLACEMENT_EXTRAS = "android.intent.extra.REPLACEMENT_EXTRAS";
        public const string EXTRA_CHOSEN_COMPONENT_INTENT_SENDER = "android.intent.extra.CHOSEN_COMPONENT_INTENT_SENDER";
        public const string EXTRA_CHOSEN_COMPONENT = "android.intent.extra.CHOSEN_COMPONENT";
        public const string EXTRA_KEY_EVENT = "android.intent.extra.KEY_EVENT";
        public const string EXTRA_KEY_CONFIRM = "android.intent.extra.KEY_CONFIRM";
        public const string EXTRA_DONT_KILL_APP = "android.intent.extra.DONT_KILL_APP";
        public const string EXTRA_PHONE_NUMBER = "android.intent.extra.PHONE_NUMBER";
        public const string EXTRA_UID = "android.intent.extra.UID";
        public const string EXTRA_PACKAGES = "android.intent.extra.PACKAGES";
        public const string EXTRA_DATA_REMOVED = "android.intent.extra.DATA_REMOVED";
        public const string EXTRA_REMOVED_FOR_ALL_USERS = "android.intent.extra.REMOVED_FOR_ALL_USERS";
        public const string EXTRA_REPLACING = "android.intent.extra.REPLACING";
        public const string EXTRA_ALARM_COUNT = "android.intent.extra.ALARM_COUNT";
        public const string EXTRA_DOCK_STATE = "android.intent.extra.DOCK_STATE";
        public const int EXTRA_DOCK_STATE_UNDOCKED = 0;
        public const int EXTRA_DOCK_STATE_DESK = 1;
        public const int EXTRA_DOCK_STATE_CAR = 2;
        public const int EXTRA_DOCK_STATE_LE_DESK = 3;
        public const int EXTRA_DOCK_STATE_HE_DESK = 4;
        public const string METADATA_DOCK_HOME = "android.dock_home";
        public const string EXTRA_BUG_REPORT = "android.intent.extra.BUG_REPORT";
        public const string EXTRA_REMOTE_INTENT_TOKEN = "android.intent.extra.remote_intent_token";
        public const string EXTRA_CHANGED_COMPONENT_NAME = "android.intent.extra.changed_component_name";
        public const string EXTRA_CHANGED_COMPONENT_NAME_LIST = "android.intent.extra.changed_component_name_list";
        public const string EXTRA_CHANGED_PACKAGE_LIST = "android.intent.extra.changed_package_list";
        public const string EXTRA_CHANGED_UID_LIST = "android.intent.extra.changed_uid_list";
        public const string EXTRA_CLIENT_LABEL = "android.intent.extra.client_label";
        public const string EXTRA_CLIENT_INTENT = "android.intent.extra.client_intent";
        public const string EXTRA_LOCAL_ONLY = "android.intent.extra.LOCAL_ONLY";
        public const string EXTRA_ALLOW_MULTIPLE = "android.intent.extra.ALLOW_MULTIPLE";
        public const string EXTRA_USER_HANDLE = "android.intent.extra.user_handle";
        public const string EXTRA_USER = "android.intent.extra.USER";
        public const string EXTRA_RESTRICTIONS_LIST = "android.intent.extra.restrictions_list";
        public const string EXTRA_RESTRICTIONS_BUNDLE = "android.intent.extra.restrictions_bundle";
        public const string EXTRA_RESTRICTIONS_INTENT = "android.intent.extra.restrictions_intent";
        public const string EXTRA_MIME_TYPES = "android.intent.extra.MIME_TYPES";
        public const string EXTRA_SHUTDOWN_USERSPACE_ONLY = "android.intent.extra.SHUTDOWN_USERSPACE_ONLY";
        public const string EXTRA_TIME_PREF_24_HOUR_FORMAT = "android.intent.extra.TIME_PREF_24_HOUR_FORMAT";
        public const string EXTRA_REASON = "android.intent.extra.REASON";

        private List<string> cats = new List<string>();
        private string action;

        public Intent() { }

        public Intent(string action)
        {
            this.action = action;
        }

        public Intent addCategory(string category)
        {
            cats.Add(category);
            return this;
        }

        public string getAction()
        {
            return action;
        }

        public List<string> getCategories()
        {
            return cats;
        }

        public Intent setAction(string action)
        {
            this.action = action;
            return this;
        }

        public override bool Equals(object obj)
        {
            if(obj != null && obj.GetType().Equals(typeof(Intent)))
            {
                Intent obj1 = (Intent)obj;
                if(obj1.getAction().Equals(this.action) && obj1.getCategories().Equals(this.cats))
                {
                    return true;
                }
            }

            return false;
        }


    }
}
