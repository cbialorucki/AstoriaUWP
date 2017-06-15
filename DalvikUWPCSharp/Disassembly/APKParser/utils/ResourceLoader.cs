using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils
{
    public class ResourceLoader
    {
        /**
        * load system attr ids for parse binary xml.
        */
        public static async Task<Dictionary<int, string>> loadSystemAttrIds()
        {
            try 
            {
                using (StreamReader reader = await toReader("r_values.txt"))
                {
                    Dictionary<int, string> map = new Dictionary<int, string>();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] items = line.Trim().Split('=');
                        if (items.Length != 2)
                        {
                            continue;
                        }
                        string name = items[0].Trim();
                        int id = int.Parse(items[1].Trim()); //.valueOf(...)
                                                             //map.put(id, name);
                        map[id] = name;
                    }

                    return map;
                }
            }
             
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public static async Task<Dictionary<int, string>> loadSystemStyles()
        {
            Dictionary<int, string> map = new Dictionary<int, string>();
            try 
            {
                using (StreamReader reader = await toReader("r_styles.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string[] items = line.Split('=');
                        if (items.Length != 2)
                        {
                            continue;
                        }
                        int id = int.Parse(items[1].Trim()); //.valueOf(...)
                        string name = items[0].Trim();
                        //map.put(id, name);
                        map[id] = name;
                    }
                }
            }
             
            catch (Exception e)
            {
                throw e;
            }
            return map;
        }

        //Added methods for interop
        private async static Task<StreamReader> toReader(string s)
        {
            //convert filename to path
            StorageFile sf = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Disassembly\APKParser\" + s);
            return await toReader(sf);
        }

        private async static Task<StreamReader> toReader(StorageFile ini)
        {
            byte[] content = await Disassembly.Util.ReadFile(ini);
            return new StreamReader(new MemoryStream(content));
        }
    }
}
