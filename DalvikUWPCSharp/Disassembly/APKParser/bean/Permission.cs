using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class Permission
    {
        private string name;
        private string label;
        private string icon;
        private string description;
        private string group;
        private string protectionLevel;

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string getLabel()
        {
            return label;
        }

        public void setLabel(string label)
        {
            this.label = label;
        }

        public string getIcon()
        {
            return icon;
        }

        public void setIcon(string icon)
        {
            this.icon = icon;
        }

        public string getDescription()
        {
            return description;
        }

        public void setDescription(string description)
        {
            this.description = description;
        }

        public string getGroup()
        {
            return group;
        }

        public void setGroup(string group)
        {
            this.group = group;
        }

        public string getProtectionLevel()
        {
            return protectionLevel;
        }

        public void setProtectionLevel(string protectionLevel)
        {
            this.protectionLevel = protectionLevel;
        }
    }
}
