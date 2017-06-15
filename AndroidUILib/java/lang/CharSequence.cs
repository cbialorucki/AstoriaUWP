using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.java.lang
{
    public interface CharSequence
    {
        int length();
        char charAt(int index);
        CharSequence subSequence(int begin, int end);
        string toString();
    }
}
