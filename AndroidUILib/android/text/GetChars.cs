using AndroidInteropLib.java.lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.text
{
    public interface GetChars : CharSequence
    {
        void getChars(int start, int end, char[] dest, int destoff);
    }
}
