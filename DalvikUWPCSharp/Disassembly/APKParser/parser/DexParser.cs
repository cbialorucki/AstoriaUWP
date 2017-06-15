using DalvikUWPCSharp.Disassembly.APKParser.bean;
using DalvikUWPCSharp.Disassembly.APKParser.struct_;
using DalvikUWPCSharp.Disassembly.APKParser.struct_.dex;
using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    class DexParser
    {
        private ByteBuffer buffer;
        private ByteOrder byteOrder = ByteOrder.LITTLE_ENDIAN;

        private static uint NO_INDEX = 0xffffffff;

        private DexClass[] dexClasses;

        public DexParser(ByteBuffer buffer)
        {
            this.buffer = buffer.duplicate();
            //this.buffer.order(byteOrder).RunSynchronously(); //slow :(
        }

        public void parse()
        {
            // read magic
            string magic = Encoding.UTF8.GetString(Buffers.readBytes(buffer, 8)); //new string(Buffers.readBytes(buffer, 8));
            if (!magic.StartsWith("dex\n"))
            {
                return;
            }
            int version = int.Parse(magic.Substring(4, 7));
            // now the version is 035
            if (version < 35)
            {
                // version 009 was used for the M3 releases of the Android platform (November–December 2007),
                // and version 013 was used for the M5 releases of the Android platform (February–March 2008)
                throw new Exception("Dex file version: " + version + " is not supported");
            }

            // read header
            DexHeader header = readDexHeader();
            header.setVersion(version);

            // read string pool
            long[] stringOffsets = readStringPool(header.getStringIdsOff(), header.getStringIdsSize());

            // read types
            int[] typeIds = readTypes(header.getTypeIdsOff(), header.getTypeIdsSize());

            // read classes
            DexClassStruct[] dexClassStructs = readClass(header.getClassDefsOff(),
                    header.getClassDefsSize());

            StringPool stringpool = readStrings(stringOffsets);

            string[] types = new string[typeIds.Length];
            for (int i = 0; i < typeIds.Length; i++)
            {
                types[i] = stringpool.get(typeIds[i]);
            }

            dexClasses = new DexClass[dexClassStructs.Length];
            for (int i = 0; i < dexClasses.Length; i++)
            {
                dexClasses[i] = new DexClass();
            }
            for (int i = 0; i < dexClassStructs.Length; i++)
            {
                DexClassStruct dexClassStruct = dexClassStructs[i];
                DexClass dexClass = dexClasses[i];
                dexClass.setClassType(types[dexClassStruct.getClassIdx()]);
                if (dexClassStruct.getSuperclassIdx() != NO_INDEX)
                {
                    dexClass.setSuperClass(types[dexClassStruct.getSuperclassIdx()]);
                }
                dexClass.setAccessFlags(dexClassStruct.getAccessFlags());
            }
        }

        /**
         * read class info.
         */
        private DexClassStruct[] readClass(long classDefsOff, int classDefsSize)
        {
            buffer.position((int)classDefsOff);

            DexClassStruct[] dexClassStructs = new DexClassStruct[classDefsSize];
            for (int i = 0; i < classDefsSize; i++)
            {
                DexClassStruct dexClassStruct = new DexClassStruct();
                dexClassStruct.setClassIdx(buffer.getInt());

                dexClassStruct.setAccessFlags(buffer.getInt());
                dexClassStruct.setSuperclassIdx(buffer.getInt());

                dexClassStruct.setInterfacesOff(Buffers.readUInt(buffer));
                dexClassStruct.setSourceFileIdx(buffer.getInt());
                dexClassStruct.setAnnotationsOff(Buffers.readUInt(buffer));
                dexClassStruct.setClassDataOff(Buffers.readUInt(buffer));
                dexClassStruct.setStaticValuesOff(Buffers.readUInt(buffer));
                dexClassStructs[i] = dexClassStruct;
            }

            return dexClassStructs;
        }

        /**
         * read types.
         */
        private int[] readTypes(long typeIdsOff, int typeIdsSize)
        {
            buffer.position((int)typeIdsOff);
            int[] typeIds = new int[typeIdsSize];
            for (int i = 0; i < typeIdsSize; i++)
            {
                typeIds[i] = (int)Buffers.readUInt(buffer);
            }
            return typeIds;
        }

        /**
         * read string pool for dex file.
         * dex file string pool diff a bit with binary xml file or resource table.
         *
         * @param offsets
         * @return
         * @throws IOException
         */
        private StringPool readStrings(long[] offsets)
        {
            // read strings.
            // buffer some apk, the strings' offsets may not well ordered. we sort it first

            StringPoolEntry[] entries = new StringPoolEntry[offsets.Length];
            for (int i = 0; i < offsets.Length; i++)
            {
                entries[i] = new StringPoolEntry(i, offsets[i]);
            }

            string lastStr = null;
            long lastOffset = -1;
            StringPool stringpool = new StringPool(offsets.Length);
            foreach (StringPoolEntry entry in entries)
            {
                if (entry.getOffset() == lastOffset)
                {
                    stringpool.set(entry.getIdx(), lastStr);
                    continue;
                }
                buffer.position((int)entry.getOffset());
                lastOffset = entry.getOffset();
                string str = readString();
                lastStr = str;
                stringpool.set(entry.getIdx(), str);
            }
            return stringpool;
        }

        /*
         * read string identifiers list.
         */
        private long[] readStringPool(long stringIdsOff, int stringIdsSize)
        {
            buffer.position((int)stringIdsOff);
            long[] offsets = new long[stringIdsSize];
            for (int i = 0; i < stringIdsSize; i++)
            {
                offsets[i] = Buffers.readUInt(buffer);
            }

            return offsets;
        }

        /**
         * read dex encoding string.
         */
        private string readString()
        {
            // the length is char len, not byte len
            int strLen = readVarInts();
            return readString(strLen);
        }

        /**
         * read Modified UTF-8 encoding str.
         *
         * @param strLen the java-utf16-char len, not strLen nor bytes len.
         */
        private string readString(int strLen)
        {
            char[] chars = new char[strLen];

            for (int i = 0; i < strLen; i++)
            {
                short a = Buffers.readUByte(buffer);
                if ((a & 0x80) == 0)
                {
                    // ascii char
                    chars[i] = (char)a;
                }
                else if ((a & 0xe0) == 0xc0)
                {
                    // read one more
                    short b = Buffers.readUByte(buffer);
                    chars[i] = (char)(((a & 0x1F) << 6) | (b & 0x3F));
                }
                else if ((a & 0xf0) == 0xe0)
                {
                    short b = Buffers.readUByte(buffer);
                    short c = Buffers.readUByte(buffer);
                    chars[i] = (char)(((a & 0x0F) << 12) | ((b & 0x3F) << 6) | (c & 0x3F));
                }
                else if ((a & 0xf0) == 0xf0)
                {
                    //throw new UTFDataFormatException();

                }
                else {
                    //throw new UTFDataFormatException();
                }
                if (chars[i] == 0)
                {
                    // the end of string.
                }
            }

            return new string(chars);
        }


        /**
         * read varints.
         *
         * @return
         * @throws IOException
         */
        private int readVarInts()
        {
            int i = 0;
            int count = 0;
            short s;
            do
            {
                if (count > 4)
                {
                    throw new Exception("read varints error.");
                }
                s = Buffers.readUByte(buffer);
                i |= (s & 0x7f) << (count * 7);
                count++;
            } while ((s & 0x80) != 0);

            return i;
        }

        private DexHeader readDexHeader()
        {

            // check sum. skip
            buffer.getInt();

            // signature skip
            Buffers.readBytes(buffer, DexHeader.kSHA1DigestLen);

            DexHeader header = new DexHeader();
            header.setFileSize(Buffers.readUInt(buffer));
            header.setHeaderSize(Buffers.readUInt(buffer));

            // skip?
            Buffers.readUInt(buffer);

            // static link data
            header.setLinkSize(Buffers.readUInt(buffer));
            header.setLinkOff(Buffers.readUInt(buffer));

            // the map data is just the same as dex header.
            header.setMapOff(Buffers.readUInt(buffer));

            header.setStringIdsSize(buffer.getInt());
            header.setStringIdsOff(Buffers.readUInt(buffer));

            header.setTypeIdsSize(buffer.getInt());
            header.setTypeIdsOff(Buffers.readUInt(buffer));

            header.setProtoIdsSize(buffer.getInt());
            header.setProtoIdsOff(Buffers.readUInt(buffer));

            header.setFieldIdsSize(buffer.getInt());
            header.setFieldIdsOff(Buffers.readUInt(buffer));

            header.setMethodIdsSize(buffer.getInt());
            header.setMethodIdsOff(Buffers.readUInt(buffer));

            header.setClassDefsSize(buffer.getInt());
            header.setClassDefsOff(Buffers.readUInt(buffer));

            header.setDataSize(buffer.getInt());
            header.setDataOff(Buffers.readUInt(buffer));

            buffer.position((int)header.getHeaderSize());

            return header;
        }

        public DexClass[] getDexClasses()
        {
            return dexClasses;
        }
    }
}
