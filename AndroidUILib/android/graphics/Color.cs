using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.graphics
{
    public class Color
    {
        public static readonly int BLACK = Convert.ToInt32(0xFF000000);
        public static readonly int DKGRAY = Convert.ToInt32(0xFF444444);
        public static readonly int GRAY = Convert.ToInt32(0xFF888888);
        public static readonly int LTGRAY = Convert.ToInt32(0xFFCCCCCC);
        public static readonly int WHITE = Convert.ToInt32(0xFFFFFFFF);
        public static readonly int RED = Convert.ToInt32(0xFFFF0000);
        public static readonly int GREEN = Convert.ToInt32(0xFF00FF00);
        public static readonly int BLUE = Convert.ToInt32(0xFF0000FF);
        public static readonly int YELLOW = Convert.ToInt32(0xFFFFFF00);
        public static readonly int CYAN = Convert.ToInt32(0xFF00FFFF);
        public static readonly int MAGENTA = Convert.ToInt32(0xFFFF00FF);
        public static readonly int TRANSPARENT = 0;

        public static int alpha(int color)
        {
            //return color >>> 24;
            return unchecked(color >> 24);
        }

        public static int red(int color)
        {
            return (color >> 16) & 0xFF;
        }

        public static int green(int color)
        {
            return (color >> 8) & 0xFF;
        }

        public static int blue(int color)
        {
            return color & 0xFF;
        }

        public static int rgb(int red, int green, int blue)
        {
            return (0xFF << 24) | (red << 16) | (green << 8) | blue;
        }

        public static int argb(int alpha, int red, int green, int blue)
        {
            return (alpha << 24) | (red << 16) | (green << 8) | blue;
        }

        public static float hue(int color)
        {
            int r = (color >> 16) & 0xFF;
            int g = (color >> 8) & 0xFF;
            int b = color & 0xFF;

            int V = Math.Max(b, Math.Max(r, g));
            int temp = Math.Min(b, Math.Min(r, g));

            float H;

            if (V == temp)
            {
                H = 0;
            }
            else {
                float vtemp = (float)(V - temp);
                float cr = (V - r) / vtemp;
                float cg = (V - g) / vtemp;
                float cb = (V - b) / vtemp;

                if (r == V)
                {
                    H = cb - cg;
                }
                else if (g == V)
                {
                    H = 2 + cr - cb;
                }
                else {
                    H = 4 + cg - cr;
                }

                H /= 6f;
                if (H < 0)
                {
                    H++;
                }
            }

            return H;
        }

        public static float saturation(int color)
        {
            int r = (color >> 16) & 0xFF;
            int g = (color >> 8) & 0xFF;
            int b = color & 0xFF;


            int V = Math.Max(b, Math.Max(r, g));
            int temp = Math.Min(b, Math.Min(r, g));

            float S;

            if (V == temp)
            {
                S = 0;
            }
            else {
                S = (V - temp) / (float)V;
            }

            return S;
        }

        public static float brightness(int color)
        {
            int r = (color >> 16) & 0xFF;
            int g = (color >> 8) & 0xFF;
            int b = color & 0xFF;

            int V = Math.Max(b, Math.Max(r, g));

            return (V / 255f);
        }

        public static int parseColor(string colorString)
        {
            if (colorString.ToCharArray()[0] == '#')
            {
                // Use a long to avoid rollovers on #ffXXXXXX
                long color = long.Parse(colorString.Substring(1, 16));
                if (colorString.Length == 7)
                {
                    // Set the alpha value
                    color |= 0x00000000ff000000;
                }
                else if (colorString.Length != 9)
                {
                    throw new ArgumentException("Unknown color");
                }
                return (int)color;
            }
            else
            {
                //int color = sColorNameMap[colorString.toLowerCase(Locale.ROOT)];
                //if (color != null)
                //{
                    //return color;
            }
            throw new ArgumentException("Unknown color");
        }


    }

}
