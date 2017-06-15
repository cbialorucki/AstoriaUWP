using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class UseFeature
    {
        private string name;
        private bool required = true;

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public bool isRequired()
        {
            return required;
        }

        public void setRequired(bool required)
        {
            this.required = required;
        }

        public string tostring()
        {
            return this.name;
        }
    }
}
