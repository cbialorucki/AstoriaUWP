using AndroidInteropLib.android.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidInteropLib.android.content;
using AndroidInteropLib.android.util;
using AndroidInteropLib.android.support.design.widget;
using System.Xml.Linq;

namespace DalvikUWPCSharp.Reassembly
{
    public class AstoriaLayoutInflator : LayoutInflater
    {
        public AstoriaLayoutInflator(Context c) : base(c)
        {

        }

        public override LayoutInflater cloneInContext(Context newContext)
        {
            throw new NotImplementedException();
        }

        /*public override View createView(string name, string prefix, AttributeSet attrs)
        {
            string viewName = attrs.getAttributeValue(prefix, name);

            //Decided to use this instead of reflection
            switch(viewName)
            {
                case "android.support.design.widget.FloatingActionButton":
                    return new FloatingActionButton(mContext, attrs);
                case "1":
                    break;
            }

            return null;
        }*/
    }
}
