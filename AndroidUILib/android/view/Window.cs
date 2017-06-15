using AndroidInteropLib.android.content;
using AndroidInteropLib.android.content.res;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.view
{
    public abstract class Window
    {
        public const int FEATURE_OPTIONS_PANEL = 0;
        public const int FEATURE_NO_TITLE = 1;
        public const int FEATURE_PROGRESS = 2;
        public const int FEATURE_LEFT_ICON = 3;
        public const int FEATURE_RIGHT_ICON = 4;
        public const int FEATURE_INDETERMINATE_PROGRESS = 5;
        public const int FEATURE_CONTEXT_MENU = 6;
        public const int FEATURE_CUSTOM_TITLE = 7;
        public const int FEATURE_ACTION_BAR = 8;
        public const int FEATURE_ACTION_BAR_OVERLAY = 9;
        public const int FEATURE_ACTION_MODE_OVERLAY = 10;
        public const int FEATURE_SWIPE_TO_DISMISS = 11;

        /** Flag for setting the progress bar's visibility to VISIBLE */
        public const int PROGRESS_VISIBILITY_ON = -1;
        /** Flag for setting the progress bar's visibility to GONE */
        public const int PROGRESS_VISIBILITY_OFF = -2;
        /** Flag for setting the progress bar's indeterminate mode on */
        public const int PROGRESS_INDETERMINATE_ON = -3;
        /** Flag for setting the progress bar's indeterminate mode off */
        public const int PROGRESS_INDETERMINATE_OFF = -4;
        /** Starting value for the (primary) progress */
        public const int PROGRESS_START = 0;
        /** Ending value for the (primary) progress */
        public const int PROGRESS_END = 10000;
        /** Lowest possible value for the secondary progress */
        public const int PROGRESS_SECONDARY_START = 20000;
        /** Highest possible value for the secondary progress */
        public const int PROGRESS_SECONDARY_END = 30000;

        /**
         * The transitionName for the status bar background View when a custom background is used.
         * @see android.view.Window#setStatusBarColor(int)
         */
        public const string STATUS_BAR_BACKGROUND_TRANSITION_NAME = "android:status:background";

        /**
         * The transitionName for the navigation bar background View when a custom background is used.
         * @see android.view.Window#setNavigationBarColor(int)
         */
        public const string NAVIGATION_BAR_BACKGROUND_TRANSITION_NAME = "android:navigation:background";

        protected const int DEFAULT_FEATURES = (1 << FEATURE_OPTIONS_PANEL) | (1 << FEATURE_CONTEXT_MENU);

        /**
        * The ID that the main layout in the XML layout file should have.
        */
        //public readonly int ID_ANDROID_CONTENT; //com.android._internal.R.id.content;

        private const string PROPERTY_HARDWARE_UI = "persist.sys.ui.hw";

        protected Context mContext;

        private TypedArray mWindowStyle;
        private Callback mCallback;
        //private OnWindowDismissedCallback mOnWindowDismissedCallback;
        //private WindowManager mWindowManager;
        //private IBinder mAppToken;
        private string mAppName;
        private bool mHardwareAccelerated;
        private Window mContainer;
        private Window mActiveChild;
        private bool mIsActive = false;
        private bool mHasChildren = false;
        private bool mCloseOnTouchOutside = false;
        private bool mSetCloseOnTouchOutside = false;
        private int mForcedWindowFlags = 0;

        private int mFeatures;
        private int mLocalFeatures;

        private bool mHaveWindowFormat = false;
        private bool mHaveDimAmount = false;
        //private int mDefaultWindowFormat = PixelFormat.OPAQUE;

        private bool mHasSoftInputMode = false;

        private bool mDestroyed;

        public Window(Context context)
        {
            mContext = context;
            //mFeatures = mLocalFeatures = getDefaultFeatures(context);
        }

        public Context getContext()
        {
            return mContext;
        }

        public void setContainer(Window container)
        {
            mContainer = container;
            if (container != null)
            {
                // Embedded screens never have a title.
                mFeatures |= 1 << FEATURE_NO_TITLE;
                mLocalFeatures |= 1 << FEATURE_NO_TITLE;
                container.mHasChildren = true;
            }
        }

        public Window getContainer()
        {
            return mContainer;
        }

        public abstract View getDecorView();

        public virtual void setDefaultIcon(int resId) { }

        public virtual void setLogo(int resId) { }

        public virtual void setDefaultLogo(int resId) { }


        public bool hasChildren()
        {
            return mHasChildren;
        }

        public void destroy()
        {
            mDestroyed = true;
        }

        public bool isDestroyed()
        {
            return mDestroyed;
        }

        public void setCallback(Callback callback)
        {
            mCallback = callback;
        }

        public Callback getCallback()
        {
            return mCallback;
        }

        public bool hasFeature(int feature)
        {
            return (getFeatures() & (1 << feature)) != 0;
        }

        protected int getLocalFeatures()
        {
            return mLocalFeatures;
        }

        protected int getFeatures()
        {
            return mFeatures;
        }

        public bool requestFeature(int featureId)
        {
            int flag = 1 << featureId;
            mFeatures |= flag;
            mLocalFeatures |= mContainer != null ? (flag & ~mContainer.mFeatures) : flag;
            return (mFeatures & flag) != 0;
        }

        protected void removeFeature(int featureId)
        {
            int flag = 1 << featureId;
            mFeatures &= ~flag;
            mLocalFeatures &= ~(mContainer != null ? (flag & ~mContainer.mFeatures) : flag);
        }


        public abstract void setContentView(int layoutResID);

        public abstract void setContentView(View view);

        public abstract void setContentView(View view, ViewGroup.LayoutParams _params);

        public abstract bool isFloating();

        public abstract int getStatusBarColor();

        public abstract void setStatusBarColor(int color);

        public abstract int getNavigationBarColor();

        public abstract void setNavigationBarColor(int color);
        /**
         * API from a Window back to its caller.  This allows the client to
         * intercept key dispatching, panels and menus, etc.
         */
        public interface Callback
        {
            //bool dispatchKeyEvent(KeyEvent _event);
            //bool dispatchKeyShortcutEvent(KeyEvent _event);
            //bool dispatchTouchEvent(MotionEvent _event);
            //bool dispatchTrackballEvent(MotionEvent _event);
            //bool dispatchGenericMotionEvent(MotionEvent _event);
            //bool dispatchPopulateAccessibilityEvent(AccessibilityEvent _event);
            View onCreatePanelView(int featureId);
            bool onCreatePanelMenu(int featureId, Menu menu);
            bool onPreparePanel(int featureId, View view, Menu menu);
            bool onMenuOpened(int featureId, Menu menu);
            bool onMenuItemSelected(int featureId, MenuItem item);
            //void onWindowAttributesChanged(WindowManager.LayoutParams attrs);
            void onContentChanged();
            void onWindowFocusChanged(bool hasFocus);
            void onAttachedToWindow();
            void onDetachedFromWindow();
            void onPanelClosed(int featureId, Menu menu);
            bool onSearchRequested();
            //ActionMode onWindowStartingActionMode(ActionMode.Callback callback);
            //void onActionModeStarted(ActionMode mode);
            //void onActionModeFinished(ActionMode mode);
        }





    }
}
