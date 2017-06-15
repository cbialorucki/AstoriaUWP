using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.AXMLPort
{
    class StringBlock
    {
        /**
	 * Reads whole (including chunk type) string block from stream.
	 * Stream must be at the chunk type.
	 */
        public static StringBlock read(IntReader reader)
        {
            ChunkUtil.readCheckType(reader,CHUNK_TYPE);

            int chunkSize = reader.readInt();
            int stringCount = reader.readInt();
            int styleOffsetCount = reader.readInt();
            /*?*/ reader.readInt();
		    int stringsOffset = reader.readInt();
            int stylesOffset = reader.readInt();

            StringBlock block = new StringBlock();
            block.m_stringOffsets=reader.readIntArray(stringCount);
		    if (styleOffsetCount!=0)
            {
			    block.m_styleOffsets=reader.readIntArray(styleOffsetCount);
		    }

		    {
			    int size = ((stylesOffset == 0) ? chunkSize : stylesOffset) - stringsOffset;
			    if ((size%4)!=0)
                {
				    throw new IOException("String data size is not multiple of 4 ("+size+").");
                }

                block.m_strings=reader.readIntArray(size/4);
		    }

		    if (stylesOffset!=0)
            {
			    int size = (chunkSize - stylesOffset);
			    if ((size%4)!=0)
                {
				    throw new IOException("Style data size is not multiple of 4 ("+size+").");
			    }
			    block.m_styles=reader.readIntArray(size/4);
		    }

            return block;
	    }
	
	    /**
	     * Returns number of strings in block. 
	     */
	    public int getCount()
        {
            return m_stringOffsets != null ? m_stringOffsets.Length : 0;
        }

        /**
        * Returns raw string (without any styling information) at specified index.
        * Buggy :(
        */
        public string getString(int index)
        {
            if (index < 0 || m_stringOffsets == null || index >= m_stringOffsets.Length)
            {
                return null;
            }
            int offset = m_stringOffsets[index];
            int length = getShort(m_strings, offset);
            StringBuilder result = new StringBuilder(length);
            //for (; length != 0; length -= 1)
            for(; length != 0; length -= 1)
            {
                offset += 2;
                result.Append((char)getShort(m_strings, offset));
            }
            return result.ToString();
        }

        public string getstring(int index)
        {
            //This hack is so I don't have to mess with the source code too much
            return getString(index);
        }

        /**
        * Not yet implemented. 
        * 
        * Returns string with style information (if any).
        */
        /*public CharSequence get(int index)
        {
            return getString(index);
        }*/

        /**
        * Returns string with style tags (html-like). 
        */
        public string getHTML(int index)
        {
            string raw = getString(index);
            if (raw == null)
            {
                return raw;
            }
            int[] style = getStyle(index);
            if (style == null)
            {
                return raw;
            }
            StringBuilder html = new StringBuilder(raw.Length + 32);
            int offset = 0;
            while (true)
            {
                int i = -1;
                for (int j = 0; j != style.Length; j += 3)
                {
                    if (style[j + 1] == -1)
                    {
                        continue;
                    }
                    if (i == -1 || style[i + 1] > style[j + 1])
                    {
                        i = j;
                    }
                }
                int start = ((i != -1) ? style[i + 1] : raw.Length);
                for (int j = 0; j != style.Length; j += 3)
                {
                    int end = style[j + 2];
                    if (end == -1 || end >= start)
                    {
                        continue;
                    }
                    if (offset <= end)
                    {
                        html.Append(raw, offset, end + 1);
                        offset = end + 1;
                    }
                    style[j + 2] = -1;
                    html.Append('<');
                    html.Append('/');
                    html.Append(getString(style[j]));
                    html.Append('>');
                }
                if (offset < start)
                {
                    html.Append(raw, offset, start);
                    offset = start;
                }
                if (i == -1)
                {
                    break;
                }
                html.Append('<');
                html.Append(getString(style[i]));
                html.Append('>');
                style[i + 1] = -1;
            }
            return html.ToString();
        }

        /**
        * Finds index of the string.
        * Returns -1 if the string was not found.
        */
        public int find(string str)
        {
            if (str == null)
            {
                return -1;
            }
            for (int i = 0; i != m_stringOffsets.Length; ++i)
            {
                int offset = m_stringOffsets[i];
                int length = getShort(m_strings, offset);
                if (length != str.Length)
                {
                    continue;
                }
                int j = 0;
                for (; j != length; ++j)
                {
                    offset += 2;
                    if (str.ToCharArray()[j] != getShort(m_strings, offset))
                    {
                        break;
                    }
                }
                if (j == length)
                {
                    return i;
                }
            }

            return -1;
        }

            ///////////////////////////////////////////// implementation

            private StringBlock() { }

        /**
         * Returns style information - array of int triplets,
         * where in each triplet:
         * 	* first int is index of tag name ('b','i', etc.)
         * 	* second int is tag start index in string
         * 	* third int is tag end index in string
         */
        private int[] getStyle(int index)
        {
            if (m_styleOffsets == null || m_styles == null ||
                index >= m_styleOffsets.Length)
            {
                return null;
            }
            int offset = m_styleOffsets[index] / 4;
            int[] style;
            {
                int count = 0;
                for (int i = offset; i < m_styles.Length; ++i)
                {
                    if (m_styles[i] == -1)
                    {
                        break;
                    }
                    count += 1;
                }
                if (count == 0 || (count % 3) != 0)
                {
                    return null;
                }
                style = new int[count];
            }
            for (int i = offset, j = 0; i < m_styles.Length;)
            {
                if (m_styles[i] == -1)
                {
                    break;
                }
                style[j++] = m_styles[i++];
            }
            return style;
        }

        //BUGGY!
        private static int getShort(int[] array, int offset)
        {
            //Debug.WriteLine("getShort(): Offset=" + offset + " array length:" + array.Length);
            if ((offset / 4) >= array.Length)
            {
                //Error...Return last two bits?
                //return 0;
                throw new IndexOutOfRangeException();
            }
            else
            {
                int value = array[offset / 4];
                if ((offset % 4) / 2 == 0)
                {
                    //Debug.WriteLine("getShort(): Value: " + (char)(value & 0xFFFF));
                    return (value & 0xFFFF);
                }
                else
                {
                    //Debug.WriteLine("getShort(): Value: " + (char)(value >> 16));
                    return value >> 16;
                }
            }
        }

        private int[] m_stringOffsets;
        private int[] m_strings;
        private int[] m_styleOffsets;
        private int[] m_styles;

        private static int CHUNK_TYPE = 0x001C0001;
    }
}
