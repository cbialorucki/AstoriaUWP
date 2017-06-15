using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.utils
{
    //This is a wrapper for interop with the ported java code
    public class ByteBuffer
    {
        private MemoryStream ms;

        public ByteBuffer()
        {
            ms = new MemoryStream();
        }

        private ByteBuffer(byte[] array, long position)
        {
            ms = new MemoryStream(array);
            ms.Position = position;
        }

        private ByteBuffer(byte[] array)
        {
            ms = new MemoryStream(array);
        }

        private ByteBuffer(MemoryStream ms1)
        {
            ms = ms1;
        }

        public long position()
        {
            return ms.Position;
        }

        public ByteBuffer position(int i)
        {
            ms.Position = i;
            return this;
        }

        public static ByteBuffer wrap(byte[] array)
        {
            //Wrap bytearray into buffer
            return new ByteBuffer(array);
        }

        public async Task<ByteBuffer> slice()
        {
            //Take current buffer's position and make a new buffer that starts there and ends at the end of the current buffer.
            MemoryStream ms2 = new MemoryStream();
            //long ln = ms.Length;
            //long pos = ms.Position;
            //long newlength = ln - pos;
            await ms.CopyToAsync(ms2);
            ms2.Position = 0;

            return new ByteBuffer(ms2);

        }

        public int getInt()
        {
            return ms.ReadByte();
        }

        public byte get()
        {
            return (byte)ms.ReadByte();
        }

        public ByteBuffer get(byte[] array)
        {
            byte[] content = ms.ToArray();
            content.CopyTo(array, 0);
            return this;
        }

        public async Task<short> getShort()
        {
            byte[] content = new byte[2];
            await ms.ReadAsync(content, 0, 2);
            return BitConverter.ToInt16(content, 0);
        }

        public async Task<char> getChar()
        {
            byte[] content = new byte[2];
            await ms.ReadAsync(content, 0, 2);
            return BitConverter.ToChar(content, 0);
        }

        public bool hasRemaining()
        {
            ms.Position += 1;
            return (ms.Position < ms.Length - 1);
        }
        
        //All calls to this method only sets order to little-Endian. ignore byte order and ensure it is LE
        public async Task order(ByteOrder bo)
        {
            //if not LE...
            if(!BitConverter.IsLittleEndian)
            {
                //reverse byte order
                byte[] content = ms.ToArray().Reverse().ToArray();
                //Flush current stream, but don't dispose, we'll reuse it
                await ms.FlushAsync();
                ms = new MemoryStream(content);
                //return this;
            }
        }

        public ByteBuffer duplicate()
        {
            //mem leaks:(

            //return this;
            byte[] content = ms.ToArray();
            long position = ms.Position;
            return new ByteBuffer(content, position);
        }

        public void Dispose()
        {
            ms.Dispose();
        }
    }
}
