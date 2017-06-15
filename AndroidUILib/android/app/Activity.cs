using AndroidInteropLib.android.content;
using AndroidInteropLib.android.support.v7.widget;
using AndroidInteropLib.android.view;
using AndroidInteropLib.com.android._internal.app;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.app
{
    public class Activity : ContextThemeWrapper
    {
        private Intent mIntent;
        private Window mWindow;
        private string mTitle;
        private int mTitleColor;
        Activity mParent;

        private ActionBar mActionBar;

        public Activity(Context context) : base(context)
        {

        }

        public Intent getIntent()
        {
            return mIntent;
        }

        public Window getWindow()
        {
            return mWindow;
        }

        public void setActionBar(Toolbar toolbar)
        {
            //ToolbarActionBar tbab = new ToolbarActionBar(toolbar, getTitle(), this);
            ToolbarActionBar tbab = new ToolbarActionBar(toolbar, getTitle(), null);
            mActionBar = tbab;
            //mWindow.setCallback(tbab.getWrappedWindowCallback());
            mActionBar.invalidateOptionsMenu();
        }

        public bool isChild()
        {
            return mParent != null;
        }

        public void setContentView(int layoutResID)
        {
            getWindow().setContentView(layoutResID);
            initWindowDecorActionBar();
        }

        public void setContentView(View view)
        {
            getWindow().setContentView(view);
            initWindowDecorActionBar();
        }

        public string getTitle()
        {
            return mTitle;
        }

        public int getTitleColor()
        {
            return mTitleColor;
        }

        private void initWindowDecorActionBar()
        {
            Window window = getWindow();

            // Initializing the window decor can change window feature flags.
            // Make sure that we have the correct set before performing the test below.
            window.getDecorView();

            if (isChild() || !window.hasFeature(Window.FEATURE_ACTION_BAR) || mActionBar != null)
            {
                return;
            }

            //mActionBar = new WindowDecorActionBar(this);
            //mActionBar.setDefaultDisplayHomeAsUpEnabled(mEnableDefaultActionBarUp);

            //mWindow.setDefaultIcon(mActivityInfo.getIconResource());
            //mWindow.setDefaultLogo(mActivityInfo.getLogoResource());
        }

        public virtual void setContentView(View view, ViewGroup.LayoutParams _params)
        {
            throw new NotImplementedException();
        }

        public void setIntent(Intent newIntent)
        {
            mIntent = newIntent;
        }

        public void onCreate()
        {

        }
    }
}
