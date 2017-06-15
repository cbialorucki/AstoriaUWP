using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace DalvikUWPCSharp.Reassembly.UI
{
    public static class ColorUtil
    {
        public static Color HoverColor(Color c)
        {
            byte max = Max(c.R, c.G, c.B);

            if(max > 128)
            {
                c.R = Convert.ToByte(c.R - (0.15 * c.R));
                c.G = Convert.ToByte(c.G - (0.15 * c.G));
                c.B = Convert.ToByte(c.B - (0.15 * c.B));
            }

            else
            {
                c.R = Convert.ToByte(1.15 * c.R);
                c.G = Convert.ToByte(1.15 * c.G);
                c.B = Convert.ToByte(1.15 * c.B);
            }
            

            return c;
        }

        public static Color PressedColor(Color c)
        {
            byte max = Max(c.R, c.G, c.B);

            if (max > 128)
            {
                c.R = Convert.ToByte(c.R - (0.3 * c.R));
                c.G = Convert.ToByte(c.G - (0.3 * c.G));
                c.B = Convert.ToByte(c.B - (0.3 * c.B));
            }

            else
            {
                c.R = Convert.ToByte(1.3 * c.R);
                c.G = Convert.ToByte(1.3 * c.G);
                c.B = Convert.ToByte(1.3 * c.B);
            }

            return c;
        }

        private static byte Max(byte x, byte y, byte z)
        {
            return Math.Max(x, Math.Max(y, z));
        }

        /*public static Color HoverColor(Color c)
        {
            HSLColor hColor = new HSLColor(c);
            if(hColor.ShouldLighten())
            {
                hColor.Adjust(0.01);
            }
            else
            {
                hColor.Adjust(-0.01);
            }

            return hColor.ToColor();
        }

        public static Color PressedColor(Color c)
        {
            HSLColor hColor = new HSLColor(c);
            if (hColor.ShouldLighten())
            {
                hColor.Adjust(0.02);
            }
            else
            {
                hColor.Adjust(-0.02);
            }

            return hColor.ToColor();
        }*/

    }

    public class HSLColor
    {
        HslColor hslColor;

        public HSLColor(Color c)
        {
            hslColor = Microsoft.Toolkit.Uwp.ColorHelper.ToHsl(c);
        }

        public void Adjust(double ammt)
        {
            hslColor.L += ammt;
        }

        public bool ShouldLighten()
        {
            if (hslColor.L < 0.5)
                return true;

            return false;
        }

        public Color ToColor()
        {
            return Microsoft.Toolkit.Uwp.ColorHelper.FromHsl(hslColor.H, hslColor.S, hslColor.L, hslColor.A);
        }


    }

    /*public class HSLColor
    {
        //Hue: 0-239
        //Sat/Lum: 0-240

        byte Hue;
        byte Sat;
        byte Lum;


        public HSLColor(Color c)
        {
            double R = (double)c.R / 255d;
            double G = (double)c.G / 255d;
            double B = (double)c.B / 255d;

            double RGBMax = Max(R, G, B) / 255d;
            double RGBMin = Min(R, G, B) / 255d;

            //Get L
            double L = (RGBMax + RGBMin) / 2d;

            //Get H and S
            double H;
            double S;

            if(c.R == c.G && c.G == c.B)
            {
                H = 0;
                S = 0;
            }

            else
            {
                if (L < 0.5)
                    S = (RGBMax - RGBMin) / (RGBMax + RGBMin);
                else
                    S = (RGBMax - RGBMin) / (2d - RGBMax - RGBMin);

                switch(MaxPosition(c.R, c.G, c.B))
                {
                    case 0:
                        H = (G - B) / (RGBMax - RGBMin);
                        break;
                    case 1:
                        H = 2d + (B - R) / (RGBMax - RGBMin);
                        break;
                    case 2:
                        H = 4d + (R - G) / (RGBMax - RGBMin);
                        break;
                    default:
                        H = 0;
                        break;
                }

                //H *= 60;

                if(H < 0)
                {
                    H += 360;
                }


            }

            //Numbers are made to corespond to Win32's Color dialog box. 
            Hue = Convert.ToByte((H / 360d) * 239);
            Sat = Convert.ToByte(S * 240);
            Lum = Convert.ToByte(L * 240);
        }

        private int MaxPosition(byte x, byte y, byte z)
        {
            return x < y ? (y < z ? 2 : 1) : (x < z ? 2 : 0);
        }

        private double Max(double x, double y, double z)
        {
            return Math.Max(x, Math.Max(y, z));
        }

        private double Min(double x, double y, double z)
        {
            return Math.Min(x, Math.Min(y, z));
        }

        public bool ShouldLighten()
        {
            if (Lum <= 120)
                return true;
            return false;
        }

        public void Lighten(byte Val)
        {
            Lum += Val;
        }

        public void Darken(byte Val)
        {
            Lum -= Val;
        }

        public Color ToColor()
        {
            if(Sat == 0)
            {
                byte val = Convert.ToByte((Lum / 240) * 255);
                return Color.FromArgb(255, val, val, val);
            }

            else
            {
                double H = (double)Hue / 239; //H is a value between 0 and 1
                double S = (double)Sat / 240;
                double L = (double)Lum / 240;

                double temp1;
                double temp2;

                if(L < 0.5)
                {
                    temp1 = L * (1d + S);
                }
                else
                {
                    temp1 = L + S - L * S;
                }

                temp2 = 2d * L - temp1;

                double tempX, tempY, tempZ;

                double R1, G1, B1;

                tempX = H + (1d / 3d); //TempR
                tempY = H; //TempG
                tempZ = H - (1d / 3d); //TempB

                if (tempX > 1)
                    tempX -= 1;
                if (tempZ < 0)
                    tempZ += 1;

                //Set R1
                R1 = ComputeRGBColorChannel(temp1, temp2, tempX);

                //Set G1
                G1 = ComputeRGBColorChannel(temp1, temp2, tempY);

                //Set B1
                B1 = ComputeRGBColorChannel(temp1, temp2, tempZ);

                byte R = Convert.ToByte(R1 * 255);
                byte G = Convert.ToByte(G1 * 255);
                byte B = Convert.ToByte(B1 * 255);

                return Color.FromArgb(255, R, G, B);
            }
        }

        private double ComputeRGBColorChannel(double temp1, double temp2, double tempA)
        {
            double val;

            if (6 * tempA < 1)
                val = temp2 + (temp1 - temp2) * 6 * tempA;
            else if (2 * tempA < 1)
                val = temp1;
            else if (3 * tempA < 2)
                val = temp2 + (temp1 - temp2) * ((2d / 6d) - tempA) * 6;
            else
                val = temp2;

            if (val < 0)
                val += 1;
            else if (val > 1)
                val -= 1;

            return val;
        }
    }*/

}
