using DalvikUWPCSharp.Disassembly.APKParser.struct_.dex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.bean
{
    public class DexClass
    {
        /**
     * the class name
     */
        private string classType;

        private string superClass;

        private int accessFlags;

        public string getPackageName()
        {
            string packageName = classType;
            if (packageName.Length > 0)
            {
                if (packageName.ToCharArray()[0] == 'L') // .charAt(0)
                {
                    packageName = packageName.Substring(1);
                }
            }
            if (packageName.Length > 0)
            {
                int idx = classType.LastIndexOf('/');
                if (idx > 0)
                {
                    packageName = packageName.Substring(0, classType.LastIndexOf('/') - 1);
                }
                else if (packageName.ToCharArray()[packageName.Length - 1] == ';')
                {
                    packageName = packageName.Substring(0, packageName.Length - 1);
                }
            }
            return packageName.Replace('/', '.');
        }

        public string getClassType()
        {
            return classType;
        }

        public void setClassType(string classType)
        {
            this.classType = classType;
        }

        public string getSuperClass()
        {
            return superClass;
        }

        public void setSuperClass(string superClass)
        {
            this.superClass = superClass;
        }

        public void setAccessFlags(int accessFlags)
        {
            this.accessFlags = accessFlags;
        }

        public bool isInterface()
        {
            return (this.accessFlags & DexClassStruct.ACC_INTERFACE) != 0;
        }

        public bool isEnum()
        {
            return (this.accessFlags & DexClassStruct.ACC_ENUM) != 0;
        }

        public bool isAnnotation()
        {
            return (this.accessFlags & DexClassStruct.ACC_ANNOTATION) != 0;
        }

        public bool isPublic()
        {
            return (this.accessFlags & DexClassStruct.ACC_PUBLIC) != 0;
        }

        public bool isProtected()
        {
            return (this.accessFlags & DexClassStruct.ACC_PROTECTED) != 0;
        }

        public bool isStatic()
        {
            return (this.accessFlags & DexClassStruct.ACC_STATIC) != 0;
        }

        public string tostring()
        {
            return classType;
        }

        public override string ToString()
        {
            return tostring();
        }
    }
}
