using DalvikUWPCSharp.Disassembly.APKParser.exception;
using DalvikUWPCSharp.Disassembly.APKParser.struct_;
using DalvikUWPCSharp.Disassembly.APKParser.struct_.resource;
using DalvikUWPCSharp.Disassembly.APKParser.utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    public class ResourceTableParser
    {
        /**
        * By default the data buffer Chunks is buffer little-endian byte order both at runtime and when stored buffer files.
        */
        private ByteOrder byteOrder = ByteOrder.LITTLE_ENDIAN;
        private StringPool stringPool;
        private ByteBuffer buffer;
        // the resource table file size
        private ResourceTable resourceTable;

        private HashSet<CultureInfo> locales;

        public ResourceTableParser(ByteBuffer buffer)
        {
            this.buffer = buffer.duplicate();
            //this.buffer.order(byteOrder).RunSynchronously(); //slow :(
            this.locales = new HashSet<CultureInfo>();
        }

        /**
         * parse resource table file.
         */
        public async Task parse()
        {
            // read resource file header.
            ResourceTableHeader resourceTableHeader = (ResourceTableHeader)await readChunkHeader();

            // read string pool chunk
            stringPool = await ParseUtils.readStringPool(buffer, (StringPoolHeader)await readChunkHeader());

            resourceTable = new ResourceTable();
            resourceTable.setStringPool(stringPool);

            PackageHeader packageHeader = (PackageHeader) await readChunkHeader();
            for (int i = 0; i < resourceTableHeader.getPackageCount(); i++)
            {
                Pair<ResourcePackage, PackageHeader> pair = await readPackage(packageHeader);
                resourceTable.addPackage(pair.getLeft()); //.getLeft());
                //This maybe null, may cause a bug.
                packageHeader = pair.getRight(); //.getRight();
            }
        }

        // read one package
        private async Task<Pair<ResourcePackage, PackageHeader>> readPackage(PackageHeader packageHeader)
        {
            Pair<ResourcePackage, PackageHeader> pair = new Pair<ResourcePackage, PackageHeader>();
            //read packageHeader
            ResourcePackage resourcePackage = new ResourcePackage(packageHeader);
            pair.setLeft(resourcePackage); //= new Pair<ResourcePackage, PackageHeader>(resourcePackage, null);
            //pair. = resourcePackage; //.setLeft(resourcePackage);

            long beginPos = buffer.position();
            // read type string pool
            if (packageHeader.getTypeStrings() > 0)
            {
                buffer.position((int)(beginPos + packageHeader.getTypeStrings()
                        - packageHeader.getHeaderSize()));
                resourcePackage.setTypeStringPool(await ParseUtils.readStringPool(buffer,
                        (StringPoolHeader)await readChunkHeader()));
            }

            //read key string pool
            if (packageHeader.getKeyStrings() > 0)
            {
                buffer.position((int)(beginPos + packageHeader.getKeyStrings()
                        - packageHeader.getHeaderSize()));
                resourcePackage.setKeyStringPool(await ParseUtils.readStringPool(buffer,
                        (StringPoolHeader)await readChunkHeader()));
            }


            //outer:
            while (buffer.hasRemaining())
            {
                ChunkHeader chunkHeader = await readChunkHeader();
                long chunkBegin = buffer.position();
                switch (chunkHeader.getChunkType())
                {
                    case ChunkType.TABLE_TYPE_SPEC:
                        TypeSpecHeader typeSpecHeader = (TypeSpecHeader)chunkHeader;
                        long[] entryFlags = new long[(int)typeSpecHeader.getEntryCount()];
                        for (int i = 0; i < typeSpecHeader.getEntryCount(); i++)
                        {
                            entryFlags[i] = Buffers.readUInt(buffer);
                        }

                        TypeSpec typeSpec = new TypeSpec(typeSpecHeader);


                        typeSpec.setEntryFlags(entryFlags);
                        //id start from 1
                        typeSpec.setName(resourcePackage.getTypeStringPool()
                                .get(typeSpecHeader.getId() - 1));

                        resourcePackage.addTypeSpec(typeSpec);
                        buffer.position((int)(chunkBegin + typeSpecHeader.getBodySize()));
                        break;
                    case ChunkType.TABLE_TYPE:
                        TypeHeader typeHeader = (TypeHeader)chunkHeader;
                        // read offsets table
                        long[] offsets = new long[(int)typeHeader.getEntryCount()];
                        for (int i = 0; i < typeHeader.getEntryCount(); i++)
                        {
                            offsets[i] = Buffers.readUInt(buffer);
                        }

                        RType type = new RType(typeHeader);
                        type.setName(resourcePackage.getTypeStringPool().get(typeHeader.getId() - 1));
                        long entryPos = chunkBegin + typeHeader.getEntriesStart() - typeHeader.getHeaderSize();
                        buffer.position((int)entryPos);
                        ByteBuffer b = await buffer.slice();
                        await b.order(byteOrder);
                        type.setBuffer(b);
                        type.setKeyStringPool(resourcePackage.getKeyStringPool());
                        type.setOffsets(offsets);
                        type.setStringPool(stringPool);
                        resourcePackage.addType(type);
                        locales.Add(type.getLocale());
                        buffer.position((int)(chunkBegin + typeHeader.getBodySize()));
                        break;
                    case ChunkType.TABLE_PACKAGE:
                        // another package. we should read next package here
                        //pair = new Pair<ResourcePackage, PackageHeader>(pair.Key, (PackageHeader)chunkHeader);
                        pair.setRight((PackageHeader)chunkHeader);
                        //break outer;
                        return pair; 
                    case ChunkType.TABLE_LIBRARY:
                        // read entries
                        LibraryHeader libraryHeader = (LibraryHeader)chunkHeader;
                        for (long i = 0; i < libraryHeader.getCount(); i++)
                        {
                            int packageId = buffer.getInt();
                            string name = await Buffers.readZeroTerminatedString(buffer, 128);
                            LibraryEntry entry = new LibraryEntry(packageId, name);
                            //TODO: now just skip it..
                        }
                        buffer.position((int)(chunkBegin + chunkHeader.getBodySize()));
                        break;
                    default:
                        throw new Exception("unexpected chunk type: 0x" + chunkHeader.getChunkType());
                }
            }

            return pair;

        }

        private async Task<ChunkHeader> readChunkHeader()
        {
            long begin = buffer.position();

            int chunkType = await Buffers.readUShort(buffer);
            int headerSize = await Buffers.readUShort(buffer);
            long chunkSize = Buffers.readUInt(buffer);

            switch (chunkType)
            {
                case ChunkType.TABLE:
                    ResourceTableHeader resourceTableHeader = new ResourceTableHeader(chunkType,
                            headerSize, chunkSize);
                    resourceTableHeader.setPackageCount(Buffers.readUInt(buffer));
                    buffer.position((int)(begin + headerSize));
                    return resourceTableHeader;
                case ChunkType.STRING_POOL:
                    StringPoolHeader stringPoolHeader = new StringPoolHeader(chunkType, headerSize,
                            chunkSize);
                    stringPoolHeader.setStringCount(Buffers.readUInt(buffer));
                    stringPoolHeader.setStyleCount(Buffers.readUInt(buffer));
                    stringPoolHeader.setFlags(Buffers.readUInt(buffer));
                    stringPoolHeader.setStringsStart(Buffers.readUInt(buffer));
                    stringPoolHeader.setStylesStart(Buffers.readUInt(buffer));
                    buffer.position((int)(begin + headerSize));
                    return stringPoolHeader;
                case ChunkType.TABLE_PACKAGE:
                    PackageHeader packageHeader = new PackageHeader(chunkType, headerSize, chunkSize);
                    packageHeader.setId(Buffers.readUInt(buffer));
                    packageHeader.setName(ParseUtils.readStringUTF16(buffer, 128));
                    packageHeader.setTypeStrings(Buffers.readUInt(buffer));
                    packageHeader.setLastPublicType(Buffers.readUInt(buffer));
                    packageHeader.setKeyStrings(Buffers.readUInt(buffer));
                    packageHeader.setLastPublicKey(Buffers.readUInt(buffer));
                    buffer.position((int)(begin + headerSize));
                    return packageHeader;
                case ChunkType.TABLE_TYPE_SPEC:
                    TypeSpecHeader typeSpecHeader = new TypeSpecHeader(chunkType, headerSize, chunkSize);
                    typeSpecHeader.setId(Buffers.readUByte(buffer));
                    typeSpecHeader.setRes0(Buffers.readUByte(buffer));
                    typeSpecHeader.setRes1(await Buffers.readUShort(buffer));
                    typeSpecHeader.setEntryCount(Buffers.readUInt(buffer));
                    buffer.position((int)(begin + headerSize));
                    return typeSpecHeader;
                case ChunkType.TABLE_TYPE:
                    TypeHeader typeHeader = new TypeHeader(chunkType, headerSize, chunkSize);
                    typeHeader.setId(Buffers.readUByte(buffer));
                    typeHeader.setRes0(Buffers.readUByte(buffer));
                    typeHeader.setRes1(await Buffers.readUShort(buffer));
                    typeHeader.setEntryCount(Buffers.readUInt(buffer));
                    typeHeader.setEntriesStart(Buffers.readUInt(buffer));
                    typeHeader.setConfig(readResTableConfig());
                    buffer.position((int)(begin + headerSize));
                    return typeHeader;
                case ChunkType.TABLE_LIBRARY:
                    //DynamicRefTable
                    LibraryHeader libraryHeader = new LibraryHeader(chunkType, headerSize, chunkSize);
                    libraryHeader.setCount(Buffers.readUInt(buffer));
                    buffer.position((int)(begin + headerSize));
                    return libraryHeader;

                case ChunkType.NULL:
                //buffer.skip((int) (chunkSize - headerSize));
                default:
                    throw new ParserException("Unexpected chunk Type: 0x" + chunkType.ToString("X"));
            }
        }

        private ResTableConfig readResTableConfig()
        {
            long beginPos = buffer.position();
            ResTableConfig config = new ResTableConfig();
            long size = Buffers.readUInt(buffer);
            Buffers.skip(buffer, 4);
            //read locale
            //Not sure which encoding to use; i'm guessing unicode? probably? i dunno. I heard UTF-16 somewhere but not sure.
            config.setLanguage(Encoding.Unicode.GetString(Buffers.readBytes(buffer, 2)).Replace("\0", ""));
            config.setCountry(Encoding.Unicode.GetString(Buffers.readBytes(buffer, 2)).Replace("\0", ""));
            //config.setLanguage(new string(Buffers.readBytes(buffer, 2)).Replace("\0", ""));
            //config.setCountry(new string(Buffers.readBytes(buffer, 2)).replace("\0", ""));

            long endPos = buffer.position();
            Buffers.skip(buffer, (int)(size - (endPos - beginPos)));
            return config;
        }

        public ResourceTable getResourceTable()
        {
            return resourceTable;
        }

        public HashSet<CultureInfo> getLocales()
        {
            return this.locales;
        }
    }
}
