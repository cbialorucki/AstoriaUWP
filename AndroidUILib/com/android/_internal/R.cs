using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidInteropLib.com.android._internal
{
    public abstract class R
    {
        public RSubClass anim = new RSubClass();
        public RSubClass animator = new RSubClass();
        public RSubClass array = new RSubClass();
        public RSubClass attr = new RSubClass();
        public RSubClass _bool = new RSubClass();
        public RSubClass color = new RSubClass();
        public RSubClass dimen = new RSubClass();
        public RSubClass drawable = new RSubClass();
        public RSubClass fraction = new RSubClass();
        public RSubClass id = new RSubClass();
        public RSubClass integer = new RSubClass();
        public RSubClass interpolator = new RSubClass();
        public RSubClass layout = new RSubClass();
        public RSubClass menu = new RSubClass();
        public RSubClass mipmap = new RSubClass();
        public RSubClass plurals = new RSubClass();
        public RSubClass raw = new RSubClass();
        public RSubClass _string = new RSubClass();
        public RSubClass style = new RSubClass();
        public RSubClass styleable = new RSubClass();
        public RSubClass transition = new RSubClass();
        public RSubClass xml = new RSubClass();


        public class RSubClass
        {
            //private Dictionary<RKey, object> source = new Dictionary<RKey, object>();
            private Dictionary<string, object> source = new Dictionary<string, object>();
            //private Dictionary<KeyValuePair<string, int>, object> source = new Dictionary<KeyValuePair<string, int>, object>();

            //Tuple<string, int> t2 = new Tuple<string, int>("hello", 119);



            public void add(string name, object value)
            {
                //source[name] = value;

                source.Add(name, value);
            }

            /*public object get(string name)
            {
                //Sorry, I know this search is expensive :(
                foreach(RKey key in source.Keys)
                {
                    if(key.Equals(name))
                    {
                        return source[key];
                    }

                }

                //Not found
                return null;
            }*/

            /*public object get(int id)
            {
                //Again yes, I know this search is expensive :(
                //Astoria is just a proof of concept at this point.

                foreach (RKey key in source.Keys)
                {
                    if (key.Equals(id))
                    {
                        return source[key];
                    }

                }

                return null;
            }*/

            public object get(string key)
            {
                if (source.ContainsKey(key))
                {
                    return source[key];
                }
                else
                {
                    return null;
                }
            }


            /*public object get(object o, bool isName = true)
            {
                if (isName && o.GetType().Equals(typeof(string)))
                {
                    string key = (string)o;

                    if (source.ContainsKey(key))
                    {
                        return source[key];
                    }
                    else
                    {
                        return null;
                    }

                }

                else
                {
                    //search by value, return name
                    //return source.FirstOrDefault(x => x.Value.Equals(o)).Key;
                }
            }*/
        }

        /*public struct RKey : IEquatable<string>, IEquatable<int>
        {
            public string Name { get; internal set; }
            public int ResID { get; internal set; }

            public RKey(string n, int id)
            {
                Name = n;
                ResID = id;
            }

            public override bool Equals(object obj)
            {
                if(obj is int)
                {
                    return Equals((int)obj);
                }
                else if(obj is string)
                {
                    return Equals((string)obj);
                }

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public bool Equals(string other)
            {
                return Name.Equals(other);
            }

            public bool Equals(int other)
            {
                return ResID.Equals(other);
            }
        }*/
    }
}
