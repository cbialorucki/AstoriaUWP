using AndroidInteropLib.android.content;
using AndroidInteropLib.android.content.res;
using AndroidInteropLib.android.util;
using AndroidInteropLib.org.xmlpull.v1;
using AndroidInteropLib.ticomware.interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.view
{
    public abstract class LayoutInflater
    {
        bool DEBUG;
        public readonly Context mContext;
        Factory mFactory;
        Factory2 mFactory2;
        private Factory2 mPrivateFactory;

        private object[] mConstructorArgs = new object[2];
        private const string TAG_MERGE = "merge";
        private const string TAG_INCLUDE = "include";
        private const string TAG_1995 = "blink";
        private const string TAG_REQUEST_FOCUS = "requestFocus";
        private const string TAG_TAG = "tag";

        public LayoutInflater(Context context)
        {
            mContext = context;
        }

        protected LayoutInflater(LayoutInflater original, Context newContext)
        {
            #if DEBUG
            DEBUG = true;
            #endif
            mContext = newContext;
            //Clone og layoutinflator
        }

        public abstract LayoutInflater cloneInContext(Context newContext);

        //public abstract View createView(string name, string prefix, AttributeSet attrs);

        public Context getContext()
        {
            return mContext;
        }

        public Factory getFactory()
        {
            return mFactory;
        }

        public Factory getFactory2()
        {
            return mFactory2;
        }

        public View inflate(int resource, ViewGroup root)
        {
            return inflate(resource, root, root != null);
        }

        public View inflate(int resource, ViewGroup root, bool attachToRoot)
        {
            Resources res = getContext().getResources();

            if(DEBUG)
            {
                //Debug.WriteLine("INFLATING from resource: \"" + res.getResourceName(resource) + "\" (" + resource.ToString("X") + ")");
            }

            XmlResourceParser parser = res.getLayout(resource);

            try
            {
                return inflate(parser, root, attachToRoot);
            }

            finally
            {
                parser.close();
            }

        }

        public virtual View inflate(XmlPullParser parser, ViewGroup root, bool attachToRoot)
        {
            //Trace.traceBegin(Trace.TRACE_TAG_VIEW, "inflate");

            AttributeSet attrs = Xml.asAttributeSet(parser, mContext);
            Context lastContext = (Context)mConstructorArgs[0];
            mConstructorArgs[0] = mContext;
            View result = root;

            try
            {
                // Look for the root node.
                int type;
                while ((type = parser.next()) != XmlPullParser.START_TAG && type != XmlPullParser.END_DOCUMENT)
                {
                    // Empty
                    //"WHAT A MISTAKE!" - DJT; but this is how Android does it so we'll do it here :(
                }

                if (type != XmlPullParser.START_TAG)
                {
                    throw new Exception(parser.getPositionDescription() + ": No start tag found!");
                }

                string name = parser.getName();

                if (DEBUG)
                {
                    Debug.WriteLine("**************************");
                    Debug.WriteLine("Creating root view: " + name);
                    Debug.WriteLine("**************************");
                }

                if (TAG_MERGE.Equals(name))
                {
                    if (root == null || !attachToRoot)
                    {
                        throw new Exception("<merge /> can be used only with a valid " + "ViewGroup root and attachToRoot=true");
                    }

                    rInflate(parser, root, attrs, false, false);
                }

                else
                {
                    // Temp is the root view that was found in the xml
                    View temp = createViewFromTag(root, name, attrs, false);

                    ViewGroup.LayoutParams lparams = null;

                    if (root != null)
                    {
                        if (DEBUG)
                        {
                            Debug.WriteLine("Creating params from root: " + root);
                        }

                        // Create layout params that match root, if supplied
                        lparams = root.generateLayoutParams(attrs);

                        if (!attachToRoot)
                        {
                            // Set the layout params for temp if we are not
                            // attaching. (If we are, we use addView, below)
                            temp.setLayoutParams(lparams);
                        }
                    }

                    if (DEBUG)
                    {
                        Debug.WriteLine("-----> start inflating children");
                    }

                    // Inflate all children under temp
                    rInflate(parser, temp, attrs, true, true);

                    if (DEBUG)
                    {
                        Debug.WriteLine("-----> done inflating children");
                    }

                    // We are supposed to attach all the views we found (int temp)
                    // to root. Do that now.
                    if (root != null && attachToRoot)
                    {
                        root.addView(temp, lparams);
                    }

                    // Decide whether to return the root that was passed in or the
                    // top view found in xml.
                    if (root == null || !attachToRoot)
                    {
                        result = temp;
                    }
                }

            }
            catch
            {
                throw;
            }

            finally
            {
                // Don't retain static reference on context.
                mConstructorArgs[0] = lastContext;
                mConstructorArgs[1] = null;
            }

            //Trace.traceEnd(Trace.TRACE_TAG_VIEW);

            return result;

        }

        private View createViewFromTag(View parent, string name, AttributeSet attrs, bool inheritContext)
        {
            if (name.Equals("view"))
            {
                name = attrs.getAttributeValue(null, "class");
            }

            //Custom hack for now
            //if it isn't a blank view and doesn't have dot operators, we'll assume it's coming from "android.widget" namespace
            //This is probably implemented elsewhere in the original Android java code and will probably get fixed at some point but it's here for now.

            else if (!name.Contains('.'))
            {
                name = "android.widget." + name;
            }

            Context viewContext;

            if (parent != null && inheritContext)
            {
                viewContext = parent.getContext();
            }
            else
            {
                viewContext = mContext;
            }

            // Apply a theme wrapper, if requested.
            /*TypedArray ta = viewContext.obtainStyledAttributes(attrs, ATTRS_THEME);
            int themeResId = ta.getResourceId(0, 0);

            if (themeResId != 0)
            {
                viewContext = new ContextThemeWrapper(viewContext, themeResId);
            }

            ta.recycle();*/

            /*if (name.Equals(TAG_1995))
            {
                // Let's party like it's 1995!
                return new BlinkLayout(viewContext, attrs);
            }*/

            if (DEBUG)
                Debug.WriteLine("******** Creating view: " + name);

            try
            {
                View view;

                if (mFactory2 != null)
                {
                    view = mFactory2.onCreateView(parent, name, viewContext, attrs);
                }

                else if (mFactory != null)
                {
                    view = mFactory.onCreateView(name, viewContext, attrs);
                }

                else
                {
                    view = null;
                }

                if (view == null && mPrivateFactory != null)
                {
                    view = mPrivateFactory.onCreateView(parent, name, viewContext, attrs);
                }

                if (view == null)
                {
                    object lastContext = mConstructorArgs[0];
                    mConstructorArgs[0] = viewContext;
                    try
                    {
                        if (-1 == name.IndexOf('.'))
                        {
                            view = onCreateView(parent, name, attrs);
                        }
                        else
                        {
                            view = createView(name, null, attrs);
                        }
                    }
                    finally
                    {
                        mConstructorArgs[0] = lastContext;
                    }
                }

                if (DEBUG)
                    Debug.WriteLine("Created view is: " + view);

                return view;

            }
            catch
            {
                Exception ie = new Exception(attrs.getPositionDescription() + ": Error inflating class " + name);
                throw ie;
            }
        }

        void rInflate(XmlPullParser parser, View parent, AttributeSet attrs, bool finishInflate, bool inheritContext) 
        {
            int depth = parser.getDepth();
            int type;

            while (((type = parser.next()) != XmlPullParser.END_TAG || parser.getDepth() > depth) && type != XmlPullParser.END_DOCUMENT)
            {

                if (type != XmlPullParser.START_TAG)
                {
                    continue;
                }

                string name = parser.getName();
            
                if (TAG_REQUEST_FOCUS.Equals(name))
                {
                    parseRequestFocus(parser, parent);
                }
                else if (TAG_TAG.Equals(name))
                {
                    parseViewTag(parser, parent, attrs);
                }

                if (TAG_INCLUDE.Equals(name))
                {
                    if (parser.getDepth() == 0)
                    {
                        throw new Exception("<include /> cannot be the root element");
                    }

                    parseInclude(parser, parent, attrs, inheritContext);
                }
                else if (TAG_MERGE.Equals(name))
                {
                    throw new Exception("<merge /> must be the root element");
                }

                else
                {
                    View view = createViewFromTag(parent, name, attrs, inheritContext);
                    ViewGroup viewGroup = (ViewGroup) parent;
                    ViewGroup.LayoutParams lparams = viewGroup.generateLayoutParams(attrs);
                    rInflate(parser, view, attrs, true, true);
                    viewGroup.addView(view, lparams);
                }
            }

            if (finishInflate) parent.onFinishInflate();
        }

        private void parseRequestFocus(XmlPullParser parser, View view)
        {
            int type;
            view.requestFocus();
            int currentDepth = parser.getDepth();
            while (((type = parser.next()) != XmlPullParser.END_TAG || parser.getDepth() > currentDepth) && type != XmlPullParser.END_DOCUMENT)
            {
                // Empty
            }
        }

        private void parseViewTag(XmlPullParser parser, View view, AttributeSet attrs)
        {
            int type;

            //The below will be activated in the final version of Astoria.
            //TypedArray ta = mContext.obtainStyledAttributes(attrs, com.android.internal.R.styleable.ViewTag);
            //int key = ta.getResourceId(com.android.internal.R.styleable.ViewTag_id, 0);
            //string value = ta.getText(com.android.internal.R.styleable.ViewTag_value);
            //view.setTag(key, value);
            //ta.recycle();

            int currentDepth = parser.getDepth();

            while (((type = parser.next()) != XmlPullParser.END_TAG || parser.getDepth() > currentDepth) && type != XmlPullParser.END_DOCUMENT)
            {
                // Empty
            }
        }

        private void parseInclude(XmlPullParser parser, View parent, AttributeSet attrs, bool inheritContext)
        {
            int type;

            //if (parent.GetType().Equals(typeof(ViewGroup)))
            if(parent is ViewGroup)
            {
                int layout = attrs.getAttributeResourceValue(null, "layout", 0); //This should return resource value, not name
                if (layout == 0)
                {
                    string value = attrs.getAttributeValue(null, "layout");
                    if (value == null)
                    {
                        throw new Exception("You must specifiy a layout in the" + " include tag: <include layout=\"@layout/layoutID\" />");
                    }
                    else
                    {
                        throw new Exception("You must specifiy a valid layout " + "reference. The layout ID " + value + " is not valid.");
                    }
                }

                else
                {
                    XmlResourceParser childParser = getContext().getResources().getLayout(layout);

                    try
                    {
                        AttributeSet childAttrs = Xml.asAttributeSet(childParser, mContext);

                        while ((type = childParser.next()) != XmlPullParser.START_TAG && type != XmlPullParser.END_DOCUMENT)
                        {
                            // Empty.
                        }

                        if (type != XmlPullParser.START_TAG)
                        {
                            throw new Exception(childParser.getPositionDescription() + ": No start tag found!");
                        }

                        string childName = childParser.getName();

                        if (TAG_MERGE.Equals(childName))
                        {
                            // Inflate all children.
                            rInflate(childParser, parent, childAttrs, false, inheritContext);
                        }
                        else
                        {
                            View view = createViewFromTag(parent, childName, childAttrs, inheritContext);
                            ViewGroup group = (ViewGroup)parent;

                            // We try to load the layout params set in the <include /> tag. If
                            // they don't exist, we will rely on the layout params set in the
                            // included XML file.
                            // During a layoutparams generation, a runtime exception is thrown
                            // if either layout_width or layout_height is missing. We catch
                            // this exception and set localParams accordingly: true means we
                            // successfully loaded layout params from the <include /> tag,
                            // false means we need to rely on the included layout params.
                            ViewGroup.LayoutParams lparams = null;
                            try
                            {
                                lparams = group.generateLayoutParams(attrs);
                            }
                            catch
                            {
                                lparams = group.generateLayoutParams(childAttrs);
                            }
                            finally
                            {
                                if (lparams != null)
                                {
                                    view.setLayoutParams(lparams);
                                }
                            }

                            // Inflate all children.
                            rInflate(childParser, view, childAttrs, true, true);

                            /*
                            // Attempt to override the included layout's android:id with the
                            // one set on the <include /> tag itself.
                            TypedArray a = mContext.obtainStyledAttributes(attrs, com.android.inner.R.styleable.View, 0, 0);
                            int id = a.getResourceId(com.android.inner.R.styleable.View_id, View.NO_ID);
                            // While we're at it, let's try to override android:visibility.
                            int visibility = a.getInt(com.android.inner.R.styleable.View_visibility, -1);
                            a.recycle();

                            if (id != View.NO_ID)
                            {
                                view.setId(id);
                            }

                            switch (visibility)
                            {
                                case 0:
                                    view.setVisibility(View.VISIBLE);
                                    break;
                                case 1:
                                    view.setVisibility(View.INVISIBLE);
                                    break;
                                case 2:
                                    view.setVisibility(View.GONE);
                                    break;
                            }
                            */

                            group.addView(view);
                        }

                    }

                    finally { }
                }
            }
            else
            {
                throw new Exception("<include /> can only be used inside of a ViewGroup");
            }

            int currentDepth = parser.getDepth();
            while (((type = parser.next()) != XmlPullParser.END_TAG || parser.getDepth() > currentDepth) && type != XmlPullParser.END_DOCUMENT)
            {
                // Empty
            }
        }

        public View createView(string name, string prefix, AttributeSet attrs)
        {
            //PoC
            try
            {
                string fullname = "AndroidInteropLib." + (prefix != null ? (prefix + name) : name);
                Type viewType = Type.GetType(fullname);
                if(viewType != null)
                {
                    View view = (View)Activator.CreateInstance(viewType, mContext, attrs);
                    return view;
                }

                else
                {
                    NullView nv = new NullView(mContext, attrs);
                    nv.setText("View not found: " + name);
                    return nv;
                }
                //TODO: Check w/ filter if view is allowed
            }
            catch (Exception ex)
            {
                //Maybe in the future return a black box with layout info saying "loading failed?"
                NullView nv = new NullView(mContext, attrs);
                nv.setText("View not found: " + name + $"\n\n{ex.Message}");
                return nv;
                //throw ex;
            }
        }


        public void setFactory(Factory factory)
        {
            if(this.mFactory == null)
            {
                this.mFactory = factory;
            }
        }

        public void setFactory2(Factory2 factory2)
        {
            if (this.mFactory2 == null)
            {
                this.mFactory2 = factory2;
            }
        }

        protected View onCreateView(View parent, string name, AttributeSet attrs)
        {
            return null;
            //return createViewFromTag(parent, name, attrs, true);
            //return onCreateView(name, attrs);
        }

        protected View onCreateView(string name, AttributeSet attrs)
        {
            return null;
            //return createViewFromTag(null, name, attrs, true);
            //throw new NotImplementedException();
        }

        public interface Factory
        {
            View onCreateView(string name, Context context, AttributeSet attrs);
        }

        public interface Factory2 : Factory
        {
            View onCreateView(View parent, string name, Context context, AttributeSet attrs);
        }

    }
}
