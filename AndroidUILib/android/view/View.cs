using AndroidInteropLib.android.content;
using AndroidInteropLib.android.util;
using AndroidInteropLib.org.xmlpull.v1;
using AndroidInteropLib.ticomware.interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AndroidInteropLib.android.view
{
    public abstract class View
    {
        private int id;
        public Context mContext { get; private set; }
        private ViewGroup.LayoutParams layparams;
        

        public const int GONE = 8;
        public const int INVISIBLE = 4;
        public const int NO_ID = -1;
        public const int VISIBLE = 0;

        //Used for Microsoft Windows rendering
        public ContentControl WinUI = new ContentControl();

        public View(Context context)
        {
            this.mContext = context;
            CreateWinUI();
        }

        public View(Context context, AttributeSet attrs)
        {
            WinUI.Name = this.GetType().Name;
            WinUI.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            WinUI.VerticalContentAlignment = VerticalAlignment.Stretch;

            this.mContext = context;

            //Get attributes and set this view's options
            setId(Convert.ToInt32(attrs.getIdAttribute()));
            //setId((int)attrs.getAttributeUnsignedIntValue(Util.nspace, "id", 0));

            //FOR IMPLEMENTING GRAVITY: if((grav | top) == grav) - means it contains top param. replace top with others (bottom, center, etc) to determine gravity
            LoadViewAttributeSet(attrs);

            CreateWinUI(context, attrs);
        }

        private void LoadViewAttributeSet(AttributeSet a)
        {
            int grav = a.getAttributeIntValue(XmlPullParser.ANDROID_NAMESPACE, "layout_gravity", -1);
            if (grav != -1)
            {
                if ((grav | Gravity.TOP) == grav)
                    WinUI.VerticalAlignment = VerticalAlignment.Top;
                else if ((grav | Gravity.BOTTOM) == grav)
                    WinUI.VerticalAlignment = VerticalAlignment.Bottom;
                if ((grav | Gravity.LEFT) == grav)
                    WinUI.HorizontalAlignment = HorizontalAlignment.Left;
                else if ((grav | Gravity.RIGHT) == grav)
                    WinUI.HorizontalAlignment = HorizontalAlignment.Right;
            }


            //padLeft returns 4097 instead of 16 for some reason. Hmm. Perhaps there is some conversion algorithim to convert this number to dp.
            /*string padLeft = ResolveResourceString(a.getAttributeValue(XmlPullParser.ANDROID_NAMESPACE, "paddingLeft"));
            if(padLeft != null)
            {
                Debug.WriteLine("padLeft: " + padLeft);
                int leftPad = int.Parse(padLeft, System.Globalization.NumberStyles.HexNumber);
                Thickness leftPadThick = WinUI.Padding;
                leftPadThick.Left = leftPad;
                WinUI.Padding = leftPadThick;
            }*/

        }

        private string ResolveResourceString(string input)
        {
            if (input != null)
            {
                if (input.StartsWith("@"))
                {
                    int id = int.Parse(input.Substring(1), System.Globalization.NumberStyles.HexNumber);
                    return mContext.getResources().getString(id);
                }

                else
                    return input;
            }

            else
                return null;
        }

        public abstract void CreateWinUI(params object[] obj);

        public static implicit operator ContentControl(View v)
        {
            return v.WinUI;
        }

        public int generateViewId()
        {
            //TODO: Make ID that doesnt exist in R.id
            return -1;
        }

        public int getWidth()
        {
            return (int)WinUI.Width;
        }

        public bool isEnabled()
        {
            //return true;
            
            return WinUI.IsEnabled;
        }

        public float getAlpha()
        {
            return (float)WinUI.Opacity;
        }

        public Context getContext()
        {
            return mContext;
        }

        public ViewGroup.LayoutParams getLayoutParams()
        {
            return layparams;
        }

        public void onFinishInflate() { }

        public void setBackgroundColor(int color)
        {
            WinUI.Background = new SolidColorBrush(ticomware.interop.Util.IntToColor(color));
        }

        public void requestFocus()
        {
            WinUI.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        public void setEnabled(bool enabled)
        {
            WinUI.IsEnabled = enabled;
        }

        public void setId(int id)
        {
            this.id = id;
        }

        public void setLayoutParams(ViewGroup.LayoutParams lparams)
        {
            layparams = lparams;

            switch(lparams.width)
            {
                case ViewGroup.LayoutParams.MATCH_PARENT:
                    WinUI.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                    break;
                case ViewGroup.LayoutParams.WRAP_CONTENT:
                    WinUI.Width = double.NaN;
                    break;
                case 0:
                    break;
                default:
                    WinUI.Width = lparams.width;
                    break;
            }

            switch (lparams.height)
            {
                case ViewGroup.LayoutParams.MATCH_PARENT:
                    WinUI.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
                    break;
                case ViewGroup.LayoutParams.WRAP_CONTENT:
                    WinUI.Height = double.NaN;
                    break;
                case 0:
                    break;
                default:
                    WinUI.Height = lparams.height;
                    break;
            }

            if(lparams.GetType().Equals(typeof(ViewGroup.MarginLayoutParams)))
            {
                ViewGroup.MarginLayoutParams lparams2 = (ViewGroup.MarginLayoutParams)lparams;
                WinUI.Margin = new Windows.UI.Xaml.Thickness(lparams2.leftMargin, lparams2.topMargin, lparams2.rightMargin, lparams2.bottomMargin);
            }
        }

        public void setMinimumHeight(int minHeight)
        {
            WinUI.MinHeight = minHeight;
        }

        public void setMinimumWidth(int minWidth)
        {
            WinUI.MinWidth = minWidth;
        }

        public void setVisibility(int visibility)
        {
            switch(visibility)
            {
                case VISIBLE:
                    WinUI.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    break;
                case INVISIBLE:
                    WinUI.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case GONE:
                    WinUI.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
            }
        }
    }
}
