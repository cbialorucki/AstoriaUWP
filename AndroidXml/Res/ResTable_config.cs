// Copyright (c) 2012 Markus Jarderot
// Copyright (c) 2015 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using AndroidXml.Utils;

namespace AndroidXml.Res
{
#if !NETSTANDARD1_3
    //[Serializable]
#endif
    public class ResTable_config
    {
        // Original properties
        public uint Size { get; set; }
        /// Mobile country code (from SIM). 0 means "any"
        public ushort IMSI_MCC { get; set; }
        /// Mobile network code (from SIM). 0 means "any"
        public ushort IMSI_MNC { get; set; }
        public char[] LocaleLanguage { get; set; }
        public char[] LocaleCountry { get; set; }
        public ConfigOrientation ScreenTypeOrientation { get; set; }
        public ConfigTouchscreen ScreenTypeTouchscreen { get; set; }
        public ConfigDensity ScreenTypeDensity { get; set; }
        public ConfigKeyboard InputKeyboard { get; set; }
        public ConfigNavigation InputNavigation { get; set; }
        public byte InputFlags { get; set; }
        public byte Input_Pad0 { get; set; }
        public ushort ScreenSizeWidth { get; set; }
        public ushort ScreenSizeHeight { get; set; }
        public ushort VersionSdk { get; set; }
        public ushort VersionMinor { get; set; }
        public byte ScreenConfigScreenLayout { get; set; }
        public byte ScreenConfigUIMode { get; set; }
        public ushort ScreenConfigSmallestScreenWidthDp { get; set; }
        public ushort ScreenSizeDpWidth { get; set; }
        public ushort ScreenSizeDpHeight { get; set; }
        public char[] LocaleScript { get; set; }
        public char[] LocaleVariant { get; set; }
        public byte ScreenLayout2 { get; set; }
        public byte ScreenConfigPad1 { get; set; }
        public ushort ScreenConfigPad2 { get; set; }
        #region Derived properties

        #region Input derived properties

        public ConfigKeysHidden InputKeysHidden
        {
            get { return (ConfigKeysHidden)Helper.GetBits(InputFlags, (byte)0x3u, (byte)0); }
            set { InputFlags = Helper.SetBits(InputFlags, (byte) value, (byte)0x3u, (byte)0); }
        }

        public ConfigNavHidden InputNavHidden
        {
            get { return (ConfigNavHidden)Helper.GetBits(InputFlags, (byte)0xc, (byte)0x2); }
            set { InputFlags = Helper.SetBits(InputFlags, (byte)value, (byte)0xc, (byte)0x2); }
        }

        #endregion

        #region ScreenConfig derived properties

        public ConfigScreenSize ScreenConfigScreenSize
        {
            get { return (ConfigScreenSize)Helper.GetBits(ScreenConfigScreenLayout, (byte)0x0f, (byte)0); }
            set { ScreenConfigScreenLayout = Helper.SetBits(ScreenConfigScreenLayout, (byte)value, (byte)0x0f, (byte)0); }
        }

        public ConfigScreenLong ScreenConfigScreenLong
        {
            get { return (ConfigScreenLong)Helper.GetBits(ScreenConfigScreenLayout, (byte)0x30, (byte)0x04); }
            set { ScreenConfigScreenLayout = Helper.SetBits(ScreenConfigScreenLayout, (byte)value, (byte)0x30, (byte)0x04); }
        }

        public ConfigScreenLayoutDirection ScreenConfigLayoutDirection
        {
            get { return (ConfigScreenLayoutDirection)Helper.GetBits(ScreenConfigScreenLayout, (byte)0xc0, (byte)0x6); }
            set { ScreenConfigScreenLayout = Helper.SetBits(ScreenConfigScreenLayout, (byte)value, (byte)0xc0, (byte)0x6); }
        }

        public ConfigUIModeType ScreenConfigUIModeType
        {
            get { return (ConfigUIModeType)Helper.GetBits(ScreenConfigUIMode, (byte)0x0f, (byte)0x00); }
            set { ScreenConfigUIMode = Helper.SetBits(ScreenConfigUIMode, (byte)value, (byte)0xFu, (byte)0x00); }
        }

        public ConfigUIModeNight ScreenConfigUIModeNight
        {
            get { return (ConfigUIModeNight)Helper.GetBits(ScreenConfigUIMode, (byte)0x3u, (byte)0x04); }
            set { ScreenConfigUIMode = Helper.SetBits(ScreenConfigUIMode, (byte)value, (byte)0x3u, (byte)0x04); }
        }

        #endregion

        #endregion
    }

    #region Enums

    public enum ConfigOrientation : byte
    {
        ORIENTATION_ANY = 0x0000,
        ORIENTATION_PORT = 0x0001,
        ORIENTATION_LAND = 0x0002,
        ORIENTATION_SQUARE = 0x0003,
    }

    public enum ConfigTouchscreen : byte
    {
        TOUCHSCREEN_ANY = 0x0000,
        TOUCHSCREEN_NOTOUCH = 0x0001,
        TOUCHSCREEN_STYLUS = 0x0002,
        TOUCHSCREEN_FINGER = 0x0003,
    }

    public enum ConfigDensity : ushort
    {
        DENSITY_DEFAULT = 0,
        DENSITY_LOW = 120,
        DENSITY_MEDIUM = 160,
        DENSITY_TV = 213,
        DENSITY_HIGH = 240,
        DENSITY_XHIGH = 320,
        DENSITY_XXHIGH = 480,
        DENSITY_XXXHIGH = 640,
        DENSITY_NONE = 0xffff,
    }

    public enum ConfigKeyboard : byte
    {
        KEYBOARD_ANY = 0x0000,
        KEYBOARD_NOKEYS = 0x0001,
        KEYBOARD_QWERTY = 0x0002,
        KEYBOARD_12KEY = 0x0003,
    }

    public enum ConfigNavigation : byte
    {
        NAVIGATION_ANY = 0x0000,
        NAVIGATION_NONAV = 0x0001,
        NAVIGATION_DPAD = 0x0002,
        NAVIGATION_TRACKBALL = 0x0003,
        NAVIGATION_WHEEL = 0x0004,
    }

    public enum ConfigKeysHidden
    {
        KEYSHIDDEN_ANY = 0x0000,
        KEYSHIDDEN_NO = 0x0001,
        KEYSHIDDEN_YES = 0x0002,
        KEYSHIDDEN_SOFT = 0x0003,
    }

    public enum ConfigNavHidden
    {
        NAVHIDDEN_ANY = 0x0000,
        NAVHIDDEN_NO = 0x0001,
        NAVHIDDEN_YES = 0x0002,
    }

    public enum ConfigScreenLayoutDirection
    {
        LAYOUTDIR_ANY = 0x00,
        LAYOUTDIR_LTR = 0x01,
        LAYOUTDIR_RTL = 0x02
    }

    public enum ConfigScreenSize
    {
        SCREENSIZE_ANY = 0x00,
        SCREENSIZE_SMALL = 0x01,
        SCREENSIZE_NORMAL = 0x02,
        SCREENSIZE_LARGE = 0x03,
        SCREENSIZE_XLARGE = 0x04,
    }

    public enum ConfigScreenLong
    {
        SCREENLONG_ANY = 0x00,
        SCREENLONG_NO = 0x01,
        SCREENLONG_YES = 0x02,
    }

    public enum ConfigUIModeType
    {
        UI_MODE_TYPE_ANY = 0x00,
        UI_MODE_TYPE_NORMAL = 0x01,
        UI_MODE_TYPE_DESK = 0x02,
        UI_MODE_TYPE_CAR = 0x03,
        UI_MODE_TYPE_TELEVISION = 0x04,
    }

    public enum ConfigUIModeNight
    {
        UI_MODE_NIGHT_ANY = 0x00,
        UI_MODE_NIGHT_NO = 0x01,
        UI_MODE_NIGHT_YES = 0x02,
    }

    #endregion
}