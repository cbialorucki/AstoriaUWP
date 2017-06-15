struct Res_png_9patch
{
    Res_png_9patch() : wasDeserialized(false), xDivs(NULL),
                       yDivs(NULL), colors(NULL) { }
    int8_t wasDeserialized;
    int8_t numXDivs;
    int8_t numYDivs;
    int8_t numColors;
    int32_t* xDivs;
    int32_t* yDivs;
    int32_t paddingLeft, paddingRight;
    int32_t paddingTop, paddingBottom;
    enum {
        NO_COLOR = 0x00000001,
        TRANSPARENT_COLOR = 0x00000000
    };
    uint32_t* colors;
    void deviceToFile();
    void fileToDevice();
    void* serialize();
    void serialize(void* outData);
    static Res_png_9patch* deserialize(const void* data);
    size_t serializedSize();
};
struct ResChunk_header
{
    uint16_t type;
    uint16_t headerSize;
    uint32_t size;
};
enum {
    RES_NULL_TYPE               = 0x0000,
    RES_STRING_POOL_TYPE        = 0x0001,
    RES_TABLE_TYPE              = 0x0002,
    RES_XML_TYPE                = 0x0003,
    RES_XML_FIRST_CHUNK_TYPE    = 0x0100,
    RES_XML_START_NAMESPACE_TYPE= 0x0100,
    RES_XML_END_NAMESPACE_TYPE  = 0x0101,
    RES_XML_START_ELEMENT_TYPE  = 0x0102,
    RES_XML_END_ELEMENT_TYPE    = 0x0103,
    RES_XML_CDATA_TYPE          = 0x0104,
    RES_XML_LAST_CHUNK_TYPE     = 0x017f,
    RES_XML_RESOURCE_MAP_TYPE   = 0x0180,
    RES_TABLE_PACKAGE_TYPE      = 0x0200,
    RES_TABLE_TYPE_TYPE         = 0x0201,
    RES_TABLE_TYPE_SPEC_TYPE    = 0x0202
};
struct Res_value
{
    uint16_t size;
    uint8_t res0;
    enum {
        TYPE_NULL = 0x00,
        TYPE_REFERENCE = 0x01,
        TYPE_ATTRIBUTE = 0x02,
        TYPE_STRING = 0x03,
        TYPE_FLOAT = 0x04,
        TYPE_DIMENSION = 0x05,
        TYPE_FRACTION	 = 0x06,
        TYPE_FIRST_INT = 0x10,
        TYPE_INT_DEC = 0x10,
        TYPE_INT_HEX = 0x11,
        TYPE_INT_BOOLEAN = 0x12,
        TYPE_FIRST_COLOR_INT = 0x1c,
        TYPE_INT_COLOR_ARGB8 = 0x1c,
        TYPE_INT_COLOR_RGB8 = 0x1d,
        TYPE_INT_COLOR_ARGB4 = 0x1e,
        TYPE_INT_COLOR_RGB4 = 0x1f,
        TYPE_LAST_COLOR_INT = 0x1f,
        TYPE_LAST_INT = 0x1f
    };
    uint8_t dataType;
    enum {
        COMPLEX_UNIT_SHIFT = 0,
        COMPLEX_UNIT_MASK = 0xf,
        COMPLEX_UNIT_PX = 0,
        COMPLEX_UNIT_DIP = 1,
        COMPLEX_UNIT_SP = 2,
        COMPLEX_UNIT_PT = 3,
        COMPLEX_UNIT_IN = 4,
        COMPLEX_UNIT_MM = 5,
        COMPLEX_UNIT_FRACTION = 0,
        COMPLEX_UNIT_FRACTION_PARENT = 1,
        COMPLEX_RADIX_SHIFT = 4,
        COMPLEX_RADIX_MASK = 0x3,
        COMPLEX_RADIX_23p0 = 0,
        COMPLEX_RADIX_16p7 = 1,
        COMPLEX_RADIX_8p15 = 2,
        COMPLEX_RADIX_0p23 = 3,
        COMPLEX_MANTISSA_SHIFT = 8,
        COMPLEX_MANTISSA_MASK = 0xffffff
    };
    uint32_t data;
    void copyFrom_dtoh(const Res_value& src);
};
struct ResTable_ref
{
    uint32_t ident;
};
struct ResStringPool_ref
{
    uint32_t index;
};
struct ResStringPool_header
{
    struct ResChunk_header header;
    uint32_t stringCount;
    uint32_t styleCount;
    enum {
        SORTED_FLAG = 1<<0,
        UTF8_FLAG = 1<<8
    };
    uint32_t flags;
    uint32_t stringsStart;
    uint32_t stylesStart;
};
struct ResStringPool_span
{
    enum {
        END = 0xFFFFFFFF
    };
    ResStringPool_ref name;
    uint32_t firstChar, lastChar;
};
struct ResXMLTree_header
{
    struct ResChunk_header header;
};
struct ResXMLTree_node
{
    struct ResChunk_header header;
    uint32_t lineNumber;
    struct ResStringPool_ref comment;
};
struct ResXMLTree_cdataExt
{
    struct ResStringPool_ref data;
    struct Res_value typedData;
};
struct ResXMLTree_namespaceExt
{
    struct ResStringPool_ref prefix;
    struct ResStringPool_ref uri;
};
struct ResXMLTree_endElementExt
{
    struct ResStringPool_ref ns;
    struct ResStringPool_ref name;
};
struct ResXMLTree_attrExt
{
    struct ResStringPool_ref ns;
    struct ResStringPool_ref name;
    uint16_t attributeStart;
    uint16_t attributeSize;
    uint16_t attributeCount;
    uint16_t idIndex;
    uint16_t classIndex;
    uint16_t styleIndex;
};
struct ResXMLTree_attribute
{
    struct ResStringPool_ref ns;
    struct ResStringPool_ref name;
    struct ResStringPool_ref rawValue;
    struct Res_value typedValue;
};
struct ResTable_header
{
    struct ResChunk_header header;
    uint32_t packageCount;
};
struct ResTable_package
{
    struct ResChunk_header header;
    uint32_t id;
    char16_t name[128];
    uint32_t typeStrings;
    uint32_t lastPublicType;
    uint32_t keyStrings;
    uint32_t lastPublicKey;
};
struct ResTable_config
{
    uint32_t size;
    union {
        struct {
            uint16_t mcc;
            uint16_t mnc;
        };
        uint32_t imsi;
    };
    union {
        struct {
            char language[2];
            char country[2];
        };
        uint32_t locale;
    };
    enum {
        ORIENTATION_ANY  = ACONFIGURATION_ORIENTATION_ANY,
        ORIENTATION_PORT = ACONFIGURATION_ORIENTATION_PORT,
        ORIENTATION_LAND = ACONFIGURATION_ORIENTATION_LAND,
        ORIENTATION_SQUARE = ACONFIGURATION_ORIENTATION_SQUARE,
    };
    enum {
        TOUCHSCREEN_ANY  = ACONFIGURATION_TOUCHSCREEN_ANY,
        TOUCHSCREEN_NOTOUCH  = ACONFIGURATION_TOUCHSCREEN_NOTOUCH,
        TOUCHSCREEN_STYLUS  = ACONFIGURATION_TOUCHSCREEN_STYLUS,
        TOUCHSCREEN_FINGER  = ACONFIGURATION_TOUCHSCREEN_FINGER,
    };
    enum {
        DENSITY_DEFAULT = ACONFIGURATION_DENSITY_DEFAULT,
        DENSITY_LOW = ACONFIGURATION_DENSITY_LOW,
        DENSITY_MEDIUM = ACONFIGURATION_DENSITY_MEDIUM,
        DENSITY_TV = ACONFIGURATION_DENSITY_TV,
        DENSITY_HIGH = ACONFIGURATION_DENSITY_HIGH,
        DENSITY_NONE = ACONFIGURATION_DENSITY_NONE
    };
    union {
        struct {
            uint8_t orientation;
            uint8_t touchscreen;
            uint16_t density;
        };
        uint32_t screenType;
    };
    enum {
        KEYBOARD_ANY  = ACONFIGURATION_KEYBOARD_ANY,
        KEYBOARD_NOKEYS  = ACONFIGURATION_KEYBOARD_NOKEYS,
        KEYBOARD_QWERTY  = ACONFIGURATION_KEYBOARD_QWERTY,
        KEYBOARD_12KEY  = ACONFIGURATION_KEYBOARD_12KEY,
    };
    enum {
        NAVIGATION_ANY  = ACONFIGURATION_NAVIGATION_ANY,
        NAVIGATION_NONAV  = ACONFIGURATION_NAVIGATION_NONAV,
        NAVIGATION_DPAD  = ACONFIGURATION_NAVIGATION_DPAD,
        NAVIGATION_TRACKBALL  = ACONFIGURATION_NAVIGATION_TRACKBALL,
        NAVIGATION_WHEEL  = ACONFIGURATION_NAVIGATION_WHEEL,
    };
    enum {
        MASK_KEYSHIDDEN = 0x0003,
        KEYSHIDDEN_ANY = ACONFIGURATION_KEYSHIDDEN_ANY,
        KEYSHIDDEN_NO = ACONFIGURATION_KEYSHIDDEN_NO,
        KEYSHIDDEN_YES = ACONFIGURATION_KEYSHIDDEN_YES,
        KEYSHIDDEN_SOFT = ACONFIGURATION_KEYSHIDDEN_SOFT,
    };
    enum {
        MASK_NAVHIDDEN = 0x000c,
        SHIFT_NAVHIDDEN = 2,
        NAVHIDDEN_ANY = ACONFIGURATION_NAVHIDDEN_ANY << SHIFT_NAVHIDDEN,
        NAVHIDDEN_NO = ACONFIGURATION_NAVHIDDEN_NO << SHIFT_NAVHIDDEN,
        NAVHIDDEN_YES = ACONFIGURATION_NAVHIDDEN_YES << SHIFT_NAVHIDDEN,
    };
    union {
        struct {
            uint8_t keyboard;
            uint8_t navigation;
            uint8_t inputFlags;
            uint8_t inputPad0;
        };
        uint32_t input;
    };
    enum {
        SCREENWIDTH_ANY = 0
    };
    enum {
        SCREENHEIGHT_ANY = 0
    };
    union {
        struct {
            uint16_t screenWidth;
            uint16_t screenHeight;
        };
        uint32_t screenSize;
    };
    enum {
        SDKVERSION_ANY = 0
    };
    enum {
        MINORVERSION_ANY = 0
    };
    union {
        struct {
            uint16_t sdkVersion;
            uint16_t minorVersion;
        };
        uint32_t version;
    };
    enum {
        MASK_SCREENSIZE = 0x0f,
        SCREENSIZE_ANY = ACONFIGURATION_SCREENSIZE_ANY,
        SCREENSIZE_SMALL = ACONFIGURATION_SCREENSIZE_SMALL,
        SCREENSIZE_NORMAL = ACONFIGURATION_SCREENSIZE_NORMAL,
        SCREENSIZE_LARGE = ACONFIGURATION_SCREENSIZE_LARGE,
        SCREENSIZE_XLARGE = ACONFIGURATION_SCREENSIZE_XLARGE,
        MASK_SCREENLONG = 0x30,
        SHIFT_SCREENLONG = 4,
        SCREENLONG_ANY = ACONFIGURATION_SCREENLONG_ANY << SHIFT_SCREENLONG,
        SCREENLONG_NO = ACONFIGURATION_SCREENLONG_NO << SHIFT_SCREENLONG,
        SCREENLONG_YES = ACONFIGURATION_SCREENLONG_YES << SHIFT_SCREENLONG,
    };
    enum {
        MASK_UI_MODE_TYPE = 0x0f,
        UI_MODE_TYPE_ANY = ACONFIGURATION_UI_MODE_TYPE_ANY,
        UI_MODE_TYPE_NORMAL = ACONFIGURATION_UI_MODE_TYPE_NORMAL,
        UI_MODE_TYPE_DESK = ACONFIGURATION_UI_MODE_TYPE_DESK,
        UI_MODE_TYPE_CAR = ACONFIGURATION_UI_MODE_TYPE_CAR,
        UI_MODE_TYPE_TELEVISION = ACONFIGURATION_UI_MODE_TYPE_TELEVISION,
        MASK_UI_MODE_NIGHT = 0x30,
        SHIFT_UI_MODE_NIGHT = 4,
        UI_MODE_NIGHT_ANY = ACONFIGURATION_UI_MODE_NIGHT_ANY << SHIFT_UI_MODE_NIGHT,
        UI_MODE_NIGHT_NO = ACONFIGURATION_UI_MODE_NIGHT_NO << SHIFT_UI_MODE_NIGHT,
        UI_MODE_NIGHT_YES = ACONFIGURATION_UI_MODE_NIGHT_YES << SHIFT_UI_MODE_NIGHT,
    };
    union {
        struct {
            uint8_t screenLayout;
            uint8_t uiMode;
            uint16_t smallestScreenWidthDp;
        };
        uint32_t screenConfig;
    };
    union {
        struct {
            uint16_t screenWidthDp;
            uint16_t screenHeightDp;
        };
        uint32_t screenSizeDp;
    };
    inline void copyFromDeviceNoSwap(const ResTable_config& o) {
        const size_t size = dtohl(o.size);
        if (size >= sizeof(ResTable_config)) {
            *this = o;
        } else {
            memcpy(this, &o, size);
            memset(((uint8_t*)this)+size, 0, sizeof(ResTable_config)-size);
        }
    }
    inline void copyFromDtoH(const ResTable_config& o) {
        copyFromDeviceNoSwap(o);
        size = sizeof(ResTable_config);
        mcc = dtohs(mcc);
        mnc = dtohs(mnc);
        density = dtohs(density);
        screenWidth = dtohs(screenWidth);
        screenHeight = dtohs(screenHeight);
        sdkVersion = dtohs(sdkVersion);
        minorVersion = dtohs(minorVersion);
        smallestScreenWidthDp = dtohs(smallestScreenWidthDp);
        screenWidthDp = dtohs(screenWidthDp);
        screenHeightDp = dtohs(screenHeightDp);
    }
    inline void swapHtoD() {
        size = htodl(size);
        mcc = htods(mcc);
        mnc = htods(mnc);
        density = htods(density);
        screenWidth = htods(screenWidth);
        screenHeight = htods(screenHeight);
        sdkVersion = htods(sdkVersion);
        minorVersion = htods(minorVersion);
        smallestScreenWidthDp = htods(smallestScreenWidthDp);
        screenWidthDp = htods(screenWidthDp);
        screenHeightDp = htods(screenHeightDp);
    }
    inline int compare(const ResTable_config& o) const {
        int32_t diff = (int32_t)(imsi - o.imsi);
        if (diff != 0) return diff;
        diff = (int32_t)(locale - o.locale);
        if (diff != 0) return diff;
        diff = (int32_t)(screenType - o.screenType);
        if (diff != 0) return diff;
        diff = (int32_t)(input - o.input);
        if (diff != 0) return diff;
        diff = (int32_t)(screenSize - o.screenSize);
        if (diff != 0) return diff;
        diff = (int32_t)(version - o.version);
        if (diff != 0) return diff;
        diff = (int32_t)(screenLayout - o.screenLayout);
        if (diff != 0) return diff;
        diff = (int32_t)(uiMode - o.uiMode);
        if (diff != 0) return diff;
        diff = (int32_t)(smallestScreenWidthDp - o.smallestScreenWidthDp);
        if (diff != 0) return diff;
        diff = (int32_t)(screenSizeDp - o.screenSizeDp);
        return (int)diff;
    }
    enum {
        CONFIG_MCC = ACONFIGURATION_MCC,
        CONFIG_MNC = ACONFIGURATION_MCC,
        CONFIG_LOCALE = ACONFIGURATION_LOCALE,
        CONFIG_TOUCHSCREEN = ACONFIGURATION_TOUCHSCREEN,
        CONFIG_KEYBOARD = ACONFIGURATION_KEYBOARD,
        CONFIG_KEYBOARD_HIDDEN = ACONFIGURATION_KEYBOARD_HIDDEN,
        CONFIG_NAVIGATION = ACONFIGURATION_NAVIGATION,
        CONFIG_ORIENTATION = ACONFIGURATION_ORIENTATION,
        CONFIG_DENSITY = ACONFIGURATION_DENSITY,
        CONFIG_SCREEN_SIZE = ACONFIGURATION_SCREEN_SIZE,
        CONFIG_SMALLEST_SCREEN_SIZE = ACONFIGURATION_SMALLEST_SCREEN_SIZE,
        CONFIG_VERSION = ACONFIGURATION_VERSION,
        CONFIG_SCREEN_LAYOUT = ACONFIGURATION_SCREEN_LAYOUT,
        CONFIG_UI_MODE = ACONFIGURATION_UI_MODE
    };
    inline int diff(const ResTable_config& o) const {
        int diffs = 0;
        if (mcc != o.mcc) diffs |= CONFIG_MCC;
        if (mnc != o.mnc) diffs |= CONFIG_MNC;
        if (locale != o.locale) diffs |= CONFIG_LOCALE;
        if (orientation != o.orientation) diffs |= CONFIG_ORIENTATION;
        if (density != o.density) diffs |= CONFIG_DENSITY;
        if (touchscreen != o.touchscreen) diffs |= CONFIG_TOUCHSCREEN;
        if (((inputFlags^o.inputFlags)&(MASK_KEYSHIDDEN|MASK_NAVHIDDEN)) != 0)
                diffs |= CONFIG_KEYBOARD_HIDDEN;
        if (keyboard != o.keyboard) diffs |= CONFIG_KEYBOARD;
        if (navigation != o.navigation) diffs |= CONFIG_NAVIGATION;
        if (screenSize != o.screenSize) diffs |= CONFIG_SCREEN_SIZE;
        if (version != o.version) diffs |= CONFIG_VERSION;
        if (screenLayout != o.screenLayout) diffs |= CONFIG_SCREEN_LAYOUT;
        if (uiMode != o.uiMode) diffs |= CONFIG_UI_MODE;
        if (smallestScreenWidthDp != o.smallestScreenWidthDp) diffs |= CONFIG_SMALLEST_SCREEN_SIZE;
        if (screenSizeDp != o.screenSizeDp) diffs |= CONFIG_SCREEN_SIZE;
        return diffs;
    }
    inline bool
    isMoreSpecificThan(const ResTable_config& o) const {
        if (imsi || o.imsi) {
            if (mcc != o.mcc) {
                if (!mcc) return false;
                if (!o.mcc) return true;
            }
            if (mnc != o.mnc) {
                if (!mnc) return false;
                if (!o.mnc) return true;
            }
        }
        if (locale || o.locale) {
            if (language[0] != o.language[0]) {
                if (!language[0]) return false;
                if (!o.language[0]) return true;
            }
            if (country[0] != o.country[0]) {
                if (!country[0]) return false;
                if (!o.country[0]) return true;
            }
        }
        if (smallestScreenWidthDp || o.smallestScreenWidthDp) {
            if (smallestScreenWidthDp != o.smallestScreenWidthDp) {
                if (!smallestScreenWidthDp) return false;
                if (!o.smallestScreenWidthDp) return true;
            }
        }
        if (screenSizeDp || o.screenSizeDp) {
            if (screenWidthDp != o.screenWidthDp) {
                if (!screenWidthDp) return false;
                if (!o.screenWidthDp) return true;
            }
            if (screenHeightDp != o.screenHeightDp) {
                if (!screenHeightDp) return false;
                if (!o.screenHeightDp) return true;
            }
        }
        if (screenLayout || o.screenLayout) {
            if (((screenLayout^o.screenLayout) & MASK_SCREENSIZE) != 0) {
                if (!(screenLayout & MASK_SCREENSIZE)) return false;
                if (!(o.screenLayout & MASK_SCREENSIZE)) return true;
            }
            if (((screenLayout^o.screenLayout) & MASK_SCREENLONG) != 0) {
                if (!(screenLayout & MASK_SCREENLONG)) return false;
                if (!(o.screenLayout & MASK_SCREENLONG)) return true;
            }
        }
        if (orientation != o.orientation) {
            if (!orientation) return false;
            if (!o.orientation) return true;
        }
        if (uiMode || o.uiMode) {
            if (((uiMode^o.uiMode) & MASK_UI_MODE_TYPE) != 0) {
                if (!(uiMode & MASK_UI_MODE_TYPE)) return false;
                if (!(o.uiMode & MASK_UI_MODE_TYPE)) return true;
            }
            if (((uiMode^o.uiMode) & MASK_UI_MODE_NIGHT) != 0) {
                if (!(uiMode & MASK_UI_MODE_NIGHT)) return false;
                if (!(o.uiMode & MASK_UI_MODE_NIGHT)) return true;
            }
        }
        if (touchscreen != o.touchscreen) {
            if (!touchscreen) return false;
            if (!o.touchscreen) return true;
        }
        if (input || o.input) {
            if (((inputFlags^o.inputFlags) & MASK_KEYSHIDDEN) != 0) {
                if (!(inputFlags & MASK_KEYSHIDDEN)) return false;
                if (!(o.inputFlags & MASK_KEYSHIDDEN)) return true;
            }
            if (((inputFlags^o.inputFlags) & MASK_NAVHIDDEN) != 0) {
                if (!(inputFlags & MASK_NAVHIDDEN)) return false;
                if (!(o.inputFlags & MASK_NAVHIDDEN)) return true;
            }
            if (keyboard != o.keyboard) {
                if (!keyboard) return false;
                if (!o.keyboard) return true;
            }
            if (navigation != o.navigation) {
                if (!navigation) return false;
                if (!o.navigation) return true;
            }
        }
        if (screenSize || o.screenSize) {
            if (screenWidth != o.screenWidth) {
                if (!screenWidth) return false;
                if (!o.screenWidth) return true;
            }
            if (screenHeight != o.screenHeight) {
                if (!screenHeight) return false;
                if (!o.screenHeight) return true;
            }
        }
        if (version || o.version) {
            if (sdkVersion != o.sdkVersion) {
                if (!sdkVersion) return false;
                if (!o.sdkVersion) return true;
            }
            if (minorVersion != o.minorVersion) {
                if (!minorVersion) return false;
                if (!o.minorVersion) return true;
            }
        }
        return false;
    }
    inline bool
    isBetterThan(const ResTable_config& o,
            const ResTable_config* requested) const {
        if (requested) {
            if (imsi || o.imsi) {
                if ((mcc != o.mcc) && requested->mcc) {
                    return (mcc);
                }
                if ((mnc != o.mnc) && requested->mnc) {
                    return (mnc);
                }
            }
            if (locale || o.locale) {
                if ((language[0] != o.language[0]) && requested->language[0]) {
                    return (language[0]);
                }
                if ((country[0] != o.country[0]) && requested->country[0]) {
                    return (country[0]);
                }
            }
            if (smallestScreenWidthDp || o.smallestScreenWidthDp) {
                return smallestScreenWidthDp >= o.smallestScreenWidthDp;
            }
            if (screenSizeDp || o.screenSizeDp) {
                int myDelta = 0, otherDelta = 0;
                if (requested->screenWidthDp) {
                    myDelta += requested->screenWidthDp - screenWidthDp;
                    otherDelta += requested->screenWidthDp - o.screenWidthDp;
                }
                if (requested->screenHeightDp) {
                    myDelta += requested->screenHeightDp - screenHeightDp;
                    otherDelta += requested->screenHeightDp - o.screenHeightDp;
                }
                return (myDelta <= otherDelta);
            }
            if (screenLayout || o.screenLayout) {
                if (((screenLayout^o.screenLayout) & MASK_SCREENSIZE) != 0
                        && (requested->screenLayout & MASK_SCREENSIZE)) {
                    int mySL = (screenLayout & MASK_SCREENSIZE);
                    int oSL = (o.screenLayout & MASK_SCREENSIZE);
                    int fixedMySL = mySL;
                    int fixedOSL = oSL;
                    if ((requested->screenLayout & MASK_SCREENSIZE) >= SCREENSIZE_NORMAL) {
                        if (fixedMySL == 0) fixedMySL = SCREENSIZE_NORMAL;
                        if (fixedOSL == 0) fixedOSL = SCREENSIZE_NORMAL;
                    }
                    if (fixedMySL == fixedOSL) {
                        if (mySL == 0) return false;
                        return true;
                    }
                    return fixedMySL >= fixedOSL;
                }
                if (((screenLayout^o.screenLayout) & MASK_SCREENLONG) != 0
                        && (requested->screenLayout & MASK_SCREENLONG)) {
                    return (screenLayout & MASK_SCREENLONG);
                }
            }
            if ((orientation != o.orientation) && requested->orientation) {
                return (orientation);
            }
            if (uiMode || o.uiMode) {
                if (((uiMode^o.uiMode) & MASK_UI_MODE_TYPE) != 0
                        && (requested->uiMode & MASK_UI_MODE_TYPE)) {
                    return (uiMode & MASK_UI_MODE_TYPE);
                }
                if (((uiMode^o.uiMode) & MASK_UI_MODE_NIGHT) != 0
                        && (requested->uiMode & MASK_UI_MODE_NIGHT)) {
                    return (uiMode & MASK_UI_MODE_NIGHT);
                }
            }
            if (screenType || o.screenType) {
                if (density != o.density) {
                    int h = (density?density:160);
                    int l = (o.density?o.density:160);
                    bool bImBigger = true;
                    if (l > h) {
                        int t = h;
                        h = l;
                        l = t;
                        bImBigger = false;
                    }
                    int reqValue = (requested->density?requested->density:160);
                    if (reqValue >= h) {
                        return bImBigger;
                    }
                    if (l >= reqValue) {
                        return !bImBigger;
                    }
                    if (((2 * l) - reqValue) * h > reqValue * reqValue) {
                        return !bImBigger;
                    } else { 
                        return bImBigger;
                    }
                }
                if ((touchscreen != o.touchscreen) && requested->touchscreen) {
                    return (touchscreen);
                }
            }
            if (input || o.input) {
                const int keysHidden = inputFlags & MASK_KEYSHIDDEN;
                const int oKeysHidden = o.inputFlags & MASK_KEYSHIDDEN;
                if (keysHidden != oKeysHidden) {
                    const int reqKeysHidden =
                            requested->inputFlags & MASK_KEYSHIDDEN;
                    if (reqKeysHidden) {
                        if (!keysHidden) return false;
                        if (!oKeysHidden) return true;
                        if (reqKeysHidden == keysHidden) return true;
                        if (reqKeysHidden == oKeysHidden) return false;
                    }
                }
                const int navHidden = inputFlags & MASK_NAVHIDDEN;
                const int oNavHidden = o.inputFlags & MASK_NAVHIDDEN;
                if (navHidden != oNavHidden) {
                    const int reqNavHidden =
                            requested->inputFlags & MASK_NAVHIDDEN;
                    if (reqNavHidden) {
                        if (!navHidden) return false;
                        if (!oNavHidden) return true;
                    }
                }
                if ((keyboard != o.keyboard) && requested->keyboard) {
                    return (keyboard);
                }
                if ((navigation != o.navigation) && requested->navigation) {
                    return (navigation);
                }
            }
            if (screenSize || o.screenSize) {
                int myDelta = 0, otherDelta = 0;
                if (requested->screenWidth) {
                    myDelta += requested->screenWidth - screenWidth;
                    otherDelta += requested->screenWidth - o.screenWidth;
                }
                if (requested->screenHeight) {
                    myDelta += requested->screenHeight - screenHeight;
                    otherDelta += requested->screenHeight - o.screenHeight;
                }
                return (myDelta <= otherDelta);
            }
            if (version || o.version) {
                if ((sdkVersion != o.sdkVersion) && requested->sdkVersion) {
                    return (sdkVersion > o.sdkVersion);
                }
                if ((minorVersion != o.minorVersion) &&
                        requested->minorVersion) {
                    return (minorVersion);
                }
            }
            return false;
        }
        return isMoreSpecificThan(o);
    }
    inline bool match(const ResTable_config& settings) const {
        if (imsi != 0) {
            if (mcc != 0 && mcc != settings.mcc) {
                return false;
            }
            if (mnc != 0 && mnc != settings.mnc) {
                return false;
            }
        }
        if (locale != 0) {
            if (language[0] != 0
                && (language[0] != settings.language[0]
                    || language[1] != settings.language[1])) {
                return false;
            }
            if (country[0] != 0
                && (country[0] != settings.country[0]
                    || country[1] != settings.country[1])) {
                return false;
            }
        }
        if (screenConfig != 0) {
            const int screenSize = screenLayout&MASK_SCREENSIZE;
            const int setScreenSize = settings.screenLayout&MASK_SCREENSIZE;
            if (screenSize != 0 && screenSize > setScreenSize) {
                return false;
            }
            const int screenLong = screenLayout&MASK_SCREENLONG;
            const int setScreenLong = settings.screenLayout&MASK_SCREENLONG;
            if (screenLong != 0 && screenLong != setScreenLong) {
                return false;
            }
            const int uiModeType = uiMode&MASK_UI_MODE_TYPE;
            const int setUiModeType = settings.uiMode&MASK_UI_MODE_TYPE;
            if (uiModeType != 0 && uiModeType != setUiModeType) {
                return false;
            }
            const int uiModeNight = uiMode&MASK_UI_MODE_NIGHT;
            const int setUiModeNight = settings.uiMode&MASK_UI_MODE_NIGHT;
            if (uiModeNight != 0 && uiModeNight != setUiModeNight) {
                return false;
            }
            if (smallestScreenWidthDp != 0
                    && smallestScreenWidthDp > settings.smallestScreenWidthDp) {
                return false;
            }
        }
        if (screenSizeDp != 0) {
            if (screenWidthDp != 0 && screenWidthDp > settings.screenWidthDp) {
                return false;
            }
            if (screenHeightDp != 0 && screenHeightDp > settings.screenHeightDp) {
                return false;
            }
        }
        if (screenType != 0) {
            if (orientation != 0 && orientation != settings.orientation) {
                return false;
            }
            if (touchscreen != 0 && touchscreen != settings.touchscreen) {
                return false;
            }
        }
        if (input != 0) {
            const int keysHidden = inputFlags&MASK_KEYSHIDDEN;
            const int setKeysHidden = settings.inputFlags&MASK_KEYSHIDDEN;
            if (keysHidden != 0 && keysHidden != setKeysHidden) {
                if (keysHidden != KEYSHIDDEN_NO || setKeysHidden != KEYSHIDDEN_SOFT) {
                    return false;
                }
            }
            const int navHidden = inputFlags&MASK_NAVHIDDEN;
            const int setNavHidden = settings.inputFlags&MASK_NAVHIDDEN;
            if (navHidden != 0 && navHidden != setNavHidden) {
                return false;
            }
            if (keyboard != 0 && keyboard != settings.keyboard) {
                return false;
            }
            if (navigation != 0 && navigation != settings.navigation) {
                return false;
            }
        }
        if (screenSize != 0) {
            if (screenWidth != 0 && screenWidth > settings.screenWidth) {
                return false;
            }
            if (screenHeight != 0 && screenHeight > settings.screenHeight) {
                return false;
            }
        }
        if (version != 0) {
            if (sdkVersion != 0 && sdkVersion > settings.sdkVersion) {
                return false;
            }
            if (minorVersion != 0 && minorVersion != settings.minorVersion) {
                return false;
            }
        }
        return true;
    }
    void getLocale(char str[6]) const {
        memset(str, 0, 6);
        if (language[0]) {
            str[0] = language[0];
            str[1] = language[1];
            if (country[0]) {
                str[2] = '_';
                str[3] = country[0];
                str[4] = country[1];
            }
        }
    }
    String8 toString() const {
        char buf[200];
        sprintf(buf, "imsi=%d/%d lang=%c%c reg=%c%c orient=%d touch=%d dens=%d "
                "kbd=%d nav=%d input=%d ssz=%dx%d sw%ddp w%ddp h%ddp sz=%d long=%d "
                "ui=%d night=%d vers=%d.%d",
                mcc, mnc,
                language[0] ? language[0] : '-', language[1] ? language[1] : '-',
                country[0] ? country[0] : '-', country[1] ? country[1] : '-',
                orientation, touchscreen, density, keyboard, navigation, inputFlags,
                screenWidth, screenHeight, smallestScreenWidthDp, screenWidthDp, screenHeightDp,
                screenLayout&MASK_SCREENSIZE, screenLayout&MASK_SCREENLONG,
                uiMode&MASK_UI_MODE_TYPE, uiMode&MASK_UI_MODE_NIGHT,
                sdkVersion, minorVersion);
        return String8(buf);
    }
};
struct ResTable_typeSpec
{
    struct ResChunk_header header;
    uint8_t id;
    uint8_t res0;
    uint16_t res1;
    uint32_t entryCount;
    enum {
        SPEC_PUBLIC = 0x40000000
    };
};
struct ResTable_type
{
    struct ResChunk_header header;
    enum {
        NO_ENTRY = 0xFFFFFFFF
    };
    uint8_t id;
    uint8_t res0;
    uint16_t res1;
    uint32_t entryCount;
    uint32_t entriesStart;
    ResTable_config config;
};
struct ResTable_entry
{
    uint16_t size;
    enum {
        FLAG_COMPLEX = 0x0001,
        FLAG_PUBLIC = 0x0002
    };
    uint16_t flags;
    struct ResStringPool_ref key;
};
struct ResTable_map_entry : public ResTable_entry
{
    ResTable_ref parent;
    uint32_t count;
};
struct ResTable_map
{
    ResTable_ref name;
    enum {
        ATTR_TYPE = 0x01000000,
        ATTR_MIN = 0x01000001,
        ATTR_MAX = 0x01000002,
        ATTR_L10N = 0x01000003,
        ATTR_OTHER = 0x01000004,
        ATTR_ZERO = 0x01000005,
        ATTR_ONE = 0x01000006,
        ATTR_TWO = 0x01000007,
        ATTR_FEW = 0x01000008,
        ATTR_MANY = 0x01000009
    };
    enum {
        TYPE_ANY = 0x0000FFFF,
        TYPE_REFERENCE = 1<<0,
        TYPE_STRING = 1<<1,
        TYPE_INTEGER = 1<<2,
        TYPE_BOOLEAN = 1<<3,
        TYPE_COLOR = 1<<4,
        TYPE_FLOAT = 1<<5,
        TYPE_DIMENSION = 1<<6,
        TYPE_FRACTION = 1<<7,
        TYPE_ENUM = 1<<16,
        TYPE_FLAGS = 1<<17
    };
    enum {
        L10N_NOT_REQUIRED = 0,
        L10N_SUGGESTED    = 1
    };
    Res_value value;
};
