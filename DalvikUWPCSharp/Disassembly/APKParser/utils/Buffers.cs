using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils
{
    public class Buffers
    {
        /**
        * get one unsigned byte as short type
        */
        public static short readUByte(ByteBuffer buffer)
        {
            byte b = buffer.get();
            return (short)(b & 0xff);
        }

        /**
         * get one unsigned short as int type
         */
        public static async Task<int> readUShort(ByteBuffer buffer)
        {
            short s = await buffer.getShort();
            return s & 0xffff;
        }

        /**
         * get one unsigned int as long type
         */
        public static long readUInt(ByteBuffer buffer)
        {
            int i = buffer.getInt();
            return i & 0xffffffffL;
        }

        /**
         * get bytes
         */
        public static byte[] readBytes(ByteBuffer buffer, int size)
        {
            byte[] bytes = new byte[size];
            buffer.get(bytes);
            return bytes;
        }


        /**
         * read utf16 strings, use strLen, not ending 0 char.
         */
        public static string readString(ByteBuffer buffer, int strLen)
        {
            StringBuilder sb = new StringBuilder(strLen);
            for (int i = 0; i < strLen; i++)
            {
                sb.Append(buffer.getChar());
            }
            return sb.ToString();
        }

        /**
         * read utf16 strings, ending with 0 char.
         */
        public static async Task<string> readZeroTerminatedString(ByteBuffer buffer, int strLen)
        {
            StringBuilder sb = new StringBuilder(strLen);
            for (int i = 0; i < strLen; i++)
            {
                char c = await buffer.getChar();
                if (c == '\0')
                {
                    skip(buffer, (strLen - i - 1) * 2);
                    break;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /**
         * skip count bytes
         */
        public static void skip(ByteBuffer buffer, int count)
        {
            buffer.position((int)(buffer.position() + count));
        }
    }
}
