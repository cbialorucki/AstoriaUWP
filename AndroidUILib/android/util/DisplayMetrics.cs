using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.util
{
    public class DisplayMetrics
    {
        public const int DENSITY_LOW = 120;
        public const int DENSITY_MEDIUM = 160;
        public const int DENSITY_TV = 213;
        public const int DENSITY_HIGH = 240;
        public const int DENSITY_280 = 280;
        public const int DENSITY_XHIGH = 320;
        public const int DENSITY_400 = 400;
        public const int DENSITY_XXHIGH = 480;
        public const int DENSITY_560 = 560;
        public const int DENSITY_XXXHIGH = 640;
        public const int DENSITY_DEFAULT = DENSITY_MEDIUM;
        public const float DENSITY_DEFAULT_SCALE = 1.0f / DENSITY_DEFAULT;
        public static int DENSITY_DEVICE = getDeviceDensity();
        public int widthPixels;
        public int heightPixels;
        public float density;
        public int densityDpi;
        public float scaledDensity;
        public float xdpi;
        public float ydpi;
        public int noncompatWidthPixels;
        public int noncompatHeightPixels;
        public float noncompatDensity;
        public int noncompatDensityDpi;
        public float noncompatScaledDensity;
        public float noncompatXdpi;
        public float noncompatYdpi;

        public DisplayMetrics() { }

        public void setTo(DisplayMetrics o)
        {
            widthPixels = o.widthPixels;
            heightPixels = o.heightPixels;
            density = o.density;
            densityDpi = o.densityDpi;
            scaledDensity = o.scaledDensity;
            xdpi = o.xdpi;
            ydpi = o.ydpi;
            noncompatWidthPixels = o.noncompatWidthPixels;
            noncompatHeightPixels = o.noncompatHeightPixels;
            noncompatDensity = o.noncompatDensity;
            noncompatDensityDpi = o.noncompatDensityDpi;
            noncompatScaledDensity = o.noncompatScaledDensity;
            noncompatXdpi = o.noncompatXdpi;
            noncompatYdpi = o.noncompatYdpi;
        }

        public void setToDefaults()
        {
            widthPixels = 0;
            heightPixels = 0;
            density = DENSITY_DEVICE / (float)DENSITY_DEFAULT;
            densityDpi = DENSITY_DEVICE;
            scaledDensity = density;
            xdpi = DENSITY_DEVICE;
            ydpi = DENSITY_DEVICE;
            noncompatWidthPixels = widthPixels;
            noncompatHeightPixels = heightPixels;
            noncompatDensity = density;
            noncompatDensityDpi = densityDpi;
            noncompatScaledDensity = scaledDensity;
            noncompatXdpi = xdpi;
            noncompatYdpi = ydpi;
        }

        public bool equals(Object o)
        {
            return o.GetType().Equals(typeof(DisplayMetrics)) && equals((DisplayMetrics)o);
        }


        public bool equals(DisplayMetrics other)
        {
            return equalsPhysical(other) && scaledDensity == other.scaledDensity && noncompatScaledDensity == other.noncompatScaledDensity;
        }


        public bool equalsPhysical(DisplayMetrics other)
        {
            return other != null
                    && widthPixels == other.widthPixels
                    && heightPixels == other.heightPixels
                    && density == other.density
                    && densityDpi == other.densityDpi
                    && xdpi == other.xdpi
                    && ydpi == other.ydpi
                    && noncompatWidthPixels == other.noncompatWidthPixels
                    && noncompatHeightPixels == other.noncompatHeightPixels
                    && noncompatDensity == other.noncompatDensity
                    && noncompatDensityDpi == other.noncompatDensityDpi
                    && noncompatXdpi == other.noncompatXdpi
                    && noncompatYdpi == other.noncompatYdpi;
        }

        public int hashCode()
        {
            return widthPixels * heightPixels * densityDpi;
        }

        public string toString()
        {
            return "DisplayMetrics{density=" + density + ", width=" + widthPixels +
                ", height=" + heightPixels + ", scaledDensity=" + scaledDensity +
                ", xdpi=" + xdpi + ", ydpi=" + ydpi + "}";
        }

        private static int getDeviceDensity()
        {
            // qemu.sf.lcd_density can be used to override ro.sf.lcd_density
            // when running in the emulator, allowing for dynamic configurations.
            // The reason for this is that ro.sf.lcd_density is write-once and is
            // set by the init process when it parses build.prop before anything else.
            //return SystemProperties.getInt("qemu.sf.lcd_density", SystemProperties.getInt("ro.sf.lcd_density", DENSITY_DEFAULT));
            return -1;
        }
    }
}
