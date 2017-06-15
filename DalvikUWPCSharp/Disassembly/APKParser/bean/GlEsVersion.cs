using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class GlEsVersion
    {
        private int major;
        private int minor;
        private bool required = true;

        public int getMajor()
        {
            return major;
        }

        public void setMajor(int major)
        {
            this.major = major;
        }

        public int getMinor()
        {
            return minor;
        }

        public void setMinor(int minor)
        {
            this.minor = minor;
        }

        public string toString()
        {
            return this.major + "." + this.minor;
        }

        public bool isRequired()
        {
            return required;
        }

        public void setRequired(bool required)
        {
            this.required = required;
        }
    }
}
