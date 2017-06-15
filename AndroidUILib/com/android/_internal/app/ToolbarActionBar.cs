using AndroidInteropLib.android.app;
using AndroidInteropLib.android.support.v7.widget;
using AndroidInteropLib.android.view;
using AndroidInteropLib.com.android._internal.widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.com.android._internal.app
{
    public class ToolbarActionBar : ActionBar
    {
        private DecorToolbar mDecorToolbar;
        private bool mToolbarMenuPrepared;
        private Window.Callback mWindowCallback;
        private bool mMenuCallbackSet;
        private bool mLastMenuVisibility;
        private List<OnMenuVisibilityListener> mMenuVisibilityListeners = new List<OnMenuVisibilityListener>();

        public ToolbarActionBar(Toolbar toolbar, string title, Window.Callback windowCallback)
        {
            //mDecorToolbar = new ToolbarWidgetWrapper(toolbar, false);
            //mWindowCallback = new ToolbarCallbackWrapper(windowCallback);
            //mDecorToolbar.setWindowCallback(mWindowCallback);
            //toolbar.setOnMenuItemClickListener(mMenuClicker);
            //mDecorToolbar.setWindowTitle(title);
        }



    }
}
