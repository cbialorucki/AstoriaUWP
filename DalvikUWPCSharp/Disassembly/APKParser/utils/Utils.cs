using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils
{
    public static class Utils
    {

        public async static Task<byte[]> toByteArray(IInputStream iis)
        {
            using (MemoryStream msft = new MemoryStream())
            {
                Stream str = iis.AsStreamForRead();
                await str.CopyToAsync(msft);
                return msft.ToArray();
            }

        }

        public async static Task<byte[]> toByteArray(StorageFile input)
        {
            return await Disassembly.Util.ReadFile(input);

            /*try
            {
                byte[] buf = new byte[1024 * 8];
                try (ByteArrayOutputStream bos = new ByteArrayOutputStream()) 
                {
                    int len;
                    while ((len = input.read(buf)) != -1)
                    {
                        bos.write(buf, 0, len);
                    }
                    return bos.toByteArray();
                } }
                catch
                {

                }
            }
            finally
            {
                input.close();
            }*/
        }


        /**
         * Copied fom commons StringUtils
         * <p>Joins the elements of the provided {@code Iterable} into
         * a single String containing the provided elements.</p>
         */
        /*public static string join(Iterable<?> iterable, string separator)
        {
            if (iterable == null)
            {
                return null;
            }
            return join(iterable.iterator(), separator);
        }*/

        /**
         * Copied fom commons StringUtils
         */
        public static string join(IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }

            //The below code does the same thing 1 line in C# can do.

            // handle null, zero and one elements before building a buffer
            /*if (iterator == null)
            {
                return null;
            }
            if (!iterator.hasNext())
            {
                return "";
            }

            object first = iterator.next();

            if (!iterator.hasNext())
            {
                return first == null ? null : first.toString();
            }

            // two or more elements
            StringBuilder buf = new StringBuilder(256); // Java default is 16, probably too small
            if (first != null)
            {
                buf.append(first);
            }

            while (iterator.hasNext())
            {
                if (separator != null)
                {
                    buf.append(separator);
                }
                final object obj = iterator.next();
                if (obj != null)
                {
                    buf.append(obj);
                }
            }
            return buf.toString();
        }*/

        public static bool isNumeric(string cs)
        {
            if (isEmpty(cs))
            {
                return false;
            }
            int sz = cs.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsDigit(cs.ToCharArray()[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool isEmpty(string cs)
        {
            return cs == null || cs.Length == 0;
        }

        public static string substringBefore(string str, string separator)
        {
            if (Utils.isEmpty(str) || separator == null)
            {
                return str;
            }
            if (separator == string.Empty)
            {
                return "";
            }
            int pos = str.IndexOf(separator);
            if (pos == -1)
            {
                return str;
            }
            return str.Substring(0, pos);
        }
    }
}
