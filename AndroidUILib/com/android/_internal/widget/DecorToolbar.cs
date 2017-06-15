using AndroidInteropLib.android.content;
using AndroidInteropLib.android.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.com.android._internal.widget
{
    public interface DecorToolbar
    {
        ViewGroup getViewGroup();
        Context getContext();
        bool isSplit();
        bool hasExpandedActionView();
        void collapseActionView();
        void setWindowCallback(Window.Callback cb);
        void setWindowTitle(string title);
        string getTitle();
        void setTitle(string title);
        string getSubtitle();
        void setSubtitle(string subtitle);
        void initProgress();
        void initIndeterminateProgress();
        bool canSplit();
        void setSplitView(ViewGroup splitView);
        void setSplitToolbar(bool split);
        void setSplitWhenNarrow(bool splitWhenNarrow);
        bool hasIcon();
        bool hasLogo();
        void setIcon(int resId);
        //void setIcon(Drawable d);
        void setLogo(int resId);
        //void setLogo(Drawable d);
        bool canShowOverflowMenu();
        bool isOverflowMenuShowing();
        bool isOverflowMenuShowPending();
        bool showOverflowMenu();
        bool hideOverflowMenu();
        void setMenuPrepared();
        //void setMenu(Menu menu, MenuPresenter.Callback cb);
        void dismissPopupMenus();

        int getDisplayOptions();
        void setDisplayOptions(int opts);
        //void setEmbeddedTabView(ScrollingTabContainerView tabView);
        bool hasEmbeddedTabs();
        bool isTitleTruncated();
        void setCollapsible(bool collapsible);
        void setHomeButtonEnabled(bool enable);
        int getNavigationMode();
        void setNavigationMode(int mode);
        //void setDropdownParams(SpinnerAdapter adapter, AdapterView.OnItemSelectedListener listener);
        void setDropdownSelectedPosition(int position);
        int getDropdownSelectedPosition();
        int getDropdownItemCount();
        void setCustomView(View view);
        View getCustomView();
        void animateToVisibility(int visibility);
        //void setNavigationIcon(Drawable icon);
        void setNavigationIcon(int resId);
        void setNavigationContentDescription(string description);
        void setNavigationContentDescription(int resId);
        void setDefaultNavigationContentDescription(int defaultNavigationContentDescription);
        //void setDefaultNavigationIcon(Drawable icon);
        //void saveHierarchyState(SparseArray<Parcelable> toolbarStates);
        //void restoreHierarchyState(SparseArray<Parcelable> toolbarStates);
        //void setBackgroundDrawable(Drawable d);
        int getHeight();
        void setVisibility(int visible);
        int getVisibility();
        //void setMenuCallbacks(MenuPresenter.Callback presenterCallback, MenuBuilder.Callback menuBuilderCallback);
        Menu getMenu();

    }
}
