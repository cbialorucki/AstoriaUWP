using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace DalvikUWPCSharp.Disassembly.AXMLPort
{
    class IntReader
    {
        /////////////////////////////////// data

        private FileStream m_stream;
        private bool m_bigEndian;
        //private int m_position;

        public IntReader() { }
        public IntReader(FileStream stream, bool bigEndian)
        {
            reset(stream, bigEndian);
        }

        public void reset(FileStream stream, bool bigEndian)
        {
            m_stream = stream;
            m_bigEndian = bigEndian;
            //m_position = 0;
        }

        public void close()
        {
            if (m_stream == null)
            {
                return;
            }
            try
            {
                //m_stream .close();
                m_stream.Dispose();
                //m_stream.FlushAsync();
            }
            catch
            {
                //ignore exception
            }
            reset(null, false);
        }

        public FileStream getStream()
        {
            return m_stream;
        }

        public bool isBigEndian()
        {
            return m_bigEndian;
        }
        public void setBigEndian(bool bigEndian)
        {
            m_bigEndian = bigEndian;
        }

        public int readByte()
        {
		    return readInt(1);
        }
        public int readShort()
        {
		    return readInt(2);
        }
        public int readInt()
        {
		    return readInt(4);
        }

        public int readInt(int length)
        {
		    if (length<0 || length>4)
            {
                throw new ArgumentException();
            }

            int result = 0;
		    if (m_bigEndian)
            {
			    for (int i = (length - 1) * 8; i>=0;i-=8)
                {
                    //int b = m_stream.read();
                    int b = m_stream.ReadByte(); //Read();
				    if (b==-1)
                    {
                        throw new EndOfStreamException();
                    }

                    //m_position +=1;
				    result|=(b<<i);
			    }
		    }
            else
            {
			    length*=8;
			    for (int i = 0; i!=length;i+=8)
                {
                    int b = m_stream.ReadByte();
				    if (b==-1)
                    {
					    throw new EndOfStreamException();
				    }
				    //m_position+=1;
				    result|=(b<<i);
			    }
		    }
		    return result;		  
	    }
	
	    public int[] readIntArray(int length)
        {
		    int[] array = new int[length];

            readIntArray(array,0, length);
		    return array;
	    }
	
	    public void readIntArray(int[] array, int offset, int length)
        {
		    for (; length>0; length-=1) 
            {
                array[offset++] = readInt();
            }
        }

        public byte[] readByteArray(int length)
        {
		    byte[] array = new byte[length];
            //m_stream.Read(arra)
            //Refactorization of code below:
            int read = m_stream.Read(array, (int)m_stream.Position, length); //.read(array);
            /*m_position+=read;
		    if (read!=length)
            {
			    throw new EndOfStreamException();
		    }*/

		    return array;
	    }
	
	    public void skip(int bytes)
        {
		    if (bytes<=0)
            {
                return;
            }
            m_stream.Position = m_stream.Position + bytes;

            //C# handles the below for us, no need to manually check for errors.
            //Java is barbaric.

            //long skipped = m_stream.Position += bytes; //.skip(bytes);
            /*m_position+=(int)skipped;
		    if (skipped!=bytes)
            {
                throw new EndOfStreamException();
            }*/
        }

        public void skipInt()
        {
            skip(4);
        }

        public int available()
        {
            return 1;
            //return m_stream.CanRead && m_stream.CanWrite; //available();
        }

        public int getPosition()
        {
            return (int)m_stream.Position;
        }

    }
}
