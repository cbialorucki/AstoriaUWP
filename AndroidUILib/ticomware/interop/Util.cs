
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.ticomware.interop
{
    public static class Util
    {
        public const string nspace = "{http://schemas.android.com/apk/res/android}";

        public static Windows.UI.Color IntToColor(int color)
        {
            //android.graphics.Color.
            Debug.WriteLine("== Color int: " + color + " ==");
            Debug.WriteLine("Alpha: " + android.graphics.Color.alpha(color));
            Debug.WriteLine("Red: " + android.graphics.Color.red(color));
            Debug.WriteLine("Blue: " + android.graphics.Color.blue(color));
            Debug.WriteLine("Green: " + android.graphics.Color.green(color) + "\n");

            return Windows.UI.Color.FromArgb((byte)android.graphics.Color.alpha(color), (byte)android.graphics.Color.red(color), (byte)android.graphics.Color.green(color), (byte)android.graphics.Color.blue(color));

            //return Windows.UI.Color.FromArgb(Convert.ToByte(android.graphics.Color.alpha(color)), Convert.ToByte(android.graphics.Color.red(color)), Convert.ToByte(android.graphics.Color.green(color)), Convert.ToByte(android.graphics.Color.blue(color)));
        }

        public static float intBitsToFloat(int i)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
        }
    }
}
