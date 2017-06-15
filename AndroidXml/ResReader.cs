// Copyright (c) 2012 Markus Jarderot
// Copyright (c) 2016 Quamotion
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using AndroidXml.Res;
using AndroidXml.Utils;
using BitConverter = System.BitConverter;

namespace AndroidXml
{
    public class ResReader : BinaryReader
    {
        public ResReader(Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
        }

        public ResReader(Stream input)
            : base(input)
        {
        }

        public virtual Res_value ReadRes_value()
        {
            return new Res_value
            {
                Size = ReadUInt16(),
                Res0 = ReadByte(),
                DataType = (ValueType)ReadByte(),
                RawData = ReadUInt32()
            };
        }

        public virtual ResChunk_header ReadResChunk_header()
        {
            return new ResChunk_header
            {
                Type = (ResourceType)ReadUInt16(),
                HeaderSize = ReadUInt16(),
                Size = ReadUInt32(),
            };
        }

        public virtual ResStringPool_header ReadResStringPool_header(ResChunk_header header)
        {
            return new ResStringPool_header
            {
                Header = header,
                StringCount = ReadUInt32(),
                StyleCount = ReadUInt32(),
                Flags = (StringPoolFlags)ReadUInt32(),
                StringStart = ReadUInt32(),
                StylesStart = ReadUInt32(),
            };
        }

        public virtual ResStringPool_ref ReadResStringPool_ref()
        {
            uint index = ReadUInt32();
            return new ResStringPool_ref
            {
                Index = index == 0xFFFFFFFFu ? (uint?)null : index,
            };
        }

        public virtual ResTable_config ReadResTable_config(uint size)
        {
            // There are different versions of this table, each differing in length.
            // Depending on the size, we should not read all data.

            var value = new ResTable_config
            {
                Size = ReadUInt32(),
                IMSI_MCC = ReadUInt16(),
                IMSI_MNC = ReadUInt16(),
                LocaleLanguage = new char[] { (char)ReadByte(), (char)ReadByte() },
                LocaleCountry = new char[] { (char)ReadByte(), (char)ReadByte() },
                ScreenTypeOrientation = (ConfigOrientation)ReadByte(),
                ScreenTypeTouchscreen = (ConfigTouchscreen)ReadByte(),
                ScreenTypeDensity = (ConfigDensity)ReadUInt16(),
                InputKeyboard = (ConfigKeyboard)ReadByte(),
                InputNavigation = (ConfigNavigation)ReadByte(),
                InputFlags = ReadByte(),
                Input_Pad0 = ReadByte(),
                ScreenSizeWidth = ReadUInt16(),
                ScreenSizeHeight = ReadUInt16(),
                VersionSdk = ReadUInt16(),
                VersionMinor = ReadUInt16()
            };

            // Read 7 uints, which is 7 * 4 = 28 bytes worth of data. This is
            // also the minimal size of this table; so really old file formats
            // will stop reading here (i.e. they don't have values for ScreenConfig
            // and ScreenSizeDp)
            if (size <= 28)
            {
                return value;
            }

            value.ScreenConfigScreenLayout = ReadByte();
            value.ScreenConfigUIMode = ReadByte();
            value.ScreenConfigSmallestScreenWidthDp = ReadUInt16();

            // The screen size was another addition, so not all files may
            // have this value
            if (size <= 32)
            {
                return value;
            }

            value.ScreenSizeDpWidth = ReadUInt16();
            value.ScreenSizeDpHeight = ReadUInt16();

            if (size <= 36)
            {
                return value;
            }

            value.LocaleScript = Encoding.ASCII.GetString(ReadBytes(4)).ToCharArray();
            value.LocaleVariant = Encoding.ASCII.GetString(ReadBytes(8)).ToCharArray();

            if (size <= 48)
            {
                return value;
            }

            value.ScreenLayout2 = ReadByte();
            value.ScreenConfigPad1 = ReadByte();
            value.ScreenConfigPad2 = ReadUInt16();

            if (size > 52)
            {
                // New fields have been added that we don't know about;
                // padding.
                ReadBytes((int)size - 52);
            }

            return value;
        }

        public virtual ResTable_entry ReadResTable_entry()
        {
            return new ResTable_entry
            {
                Size = ReadUInt16(),
                Flags = (EntryFlags)ReadUInt16(),
                Key = ReadResStringPool_ref(),
            };
        }

        public virtual ResTable_header ReadResTable_header(ResChunk_header header)
        {
            return new ResTable_header
            {
                Header = header,
                PackageCount = ReadUInt32(),
            };
        }

        public virtual ResTable_map ReadResTable_map()
        {
            return new ResTable_map
            {
                Name = ReadResTable_ref(),
                Value = ReadRes_value(),
            };
        }

        public virtual ResTable_map_entry ReadResTable_map_entry()
        {
            return new ResTable_map_entry
            {
                Size = ReadUInt16(),
                Flags = (EntryFlags)ReadUInt16(),
                Key = ReadResStringPool_ref(),
                Parent = ReadResTable_ref(),
                Count = ReadUInt32(),
            };
        }

        public virtual ResTable_package ReadResTable_package(ResChunk_header header)
        {
            var value = new ResTable_package
            {
                Header = header,
                Id = ReadUInt32(),
                Name = Encoding.Unicode.GetString(ReadBytes(256)),
                TypeStrings = ReadUInt32(),
                LastPublicType = ReadUInt32(),
                KeyStrings = ReadUInt32(),
                LastPublicKey = ReadUInt32(),
            };

            if (header.HeaderSize > 284)
            {
                value.TypeIdOffset = ReadUInt32();
            }

            if (header.HeaderSize > 292)
            {
                // New fields have been added, which we don't know about.
                ReadBytes((int)header.HeaderSize - 292);
            }

            return value;
        }

        public virtual ResTable_ref ReadResTable_ref()
        {
            uint ident = ReadUInt32();
            return new ResTable_ref
            {
                Ident = ident == 0xFFFFFFFFu ? (uint?)null : ident,
            };
        }

        public virtual ResTable_type ReadResTable_type(ResChunk_header header)
        {
            // The config data is versioned using the "size"; newer versions are
            // larger and more data. So we need to let the ReadResTable_config
            // method know how much data it can read to prevent it from being
            // too greedy.
            ushort configSize = header.HeaderSize;
            configSize -= ResChunk_header.DataSize;
            configSize -= 12; // RawId, EntryCount, EntriesStart

            return new ResTable_type
            {
                Header = header,
                RawID = ReadUInt32(),
                EntryCount = ReadUInt32(),
                EntriesStart = ReadUInt32(),
                Config = ReadResTable_config(configSize),
            };
        }

        public virtual ResTable_typeSpec ReadResTable_typeSpec(ResChunk_header header)
        {
            return new ResTable_typeSpec
            {
                Header = header,
                RawID = ReadUInt32(),
                EntryCount = ReadUInt32(),
            };
        }

        public virtual ResXMLTree_attrExt ReadResXMLTree_attrExt()
        {
            return new ResXMLTree_attrExt
            {
                Namespace = ReadResStringPool_ref(),
                Name = ReadResStringPool_ref(),
                AttributeStart = ReadUInt16(),
                AttributeSize = ReadUInt16(),
                AttributeCount = ReadUInt16(),
                IdIndex = ReadUInt16(),
                ClassIndex = ReadUInt16(),
                StyleIndex = ReadUInt16(),
            };
        }

        public virtual ResXMLTree_attribute ReadResXMLTree_attribute()
        {
            return new ResXMLTree_attribute
            {
                Namespace = ReadResStringPool_ref(),
                Name = ReadResStringPool_ref(),
                RawValue = ReadResStringPool_ref(),
                TypedValue = ReadRes_value()
            };
        }

        public virtual ResXMLTree_cdataExt ReadResXMLTree_cdataExt()
        {
            return new ResXMLTree_cdataExt
            {
                Data = ReadResStringPool_ref(),
                TypedData = ReadRes_value(),
            };
        }

        public virtual ResXMLTree_endElementExt ReadResXMLTree_endElementExt()
        {
            return new ResXMLTree_endElementExt
            {
                Namespace = ReadResStringPool_ref(),
                Name = ReadResStringPool_ref(),
            };
        }

        public virtual ResXMLTree_header ReadResXMLTree_header(ResChunk_header header)
        {
            return new ResXMLTree_header
            {
                Header = header,
            };
        }


        public virtual ResXMLTree_namespaceExt ReadResXMLTree_namespaceExt()
        {
            return new ResXMLTree_namespaceExt
            {
                Prefix = ReadResStringPool_ref(),
                Uri = ReadResStringPool_ref(),
            };
        }

        public virtual ResXMLTree_node ReadResXMLTree_node(ResChunk_header header)
        {
            return new ResXMLTree_node
            {
                Header = header,
                LineNumber = ReadUInt32(),
                Comment = ReadResStringPool_ref(),
            };
        }

        public virtual ResStringPool ReadResStringPool(ResStringPool_header header)
        {
            var pool = new ResStringPool
            {
                Header = header,
                StringData = new List<string>(),
                StyleData = new List<List<ResStringPool_span>>()
            };

            // Offsets of the string data, relative to header.StringStart
            var stringIndices = new List<uint>();

            for (int i = 0; i < header.StringCount; i++)
            {
                stringIndices.Add(ReadUInt32());
            }

            // Offset of the style data, relative to header.StylesStart
            var styleIndices = new List<uint>();
            for (int i = 0; i < header.StyleCount; i++)
            {
                styleIndices.Add(ReadUInt32());
            }

            // Keep track of how many bytes are left, to prevent us
            // from reading invalid data.
            long bytesLeft = header.Header.Size;
            bytesLeft -= header.Header.HeaderSize;
            bytesLeft -= 4 * header.StringCount;
            bytesLeft -= 4 * header.StyleCount;

            // Fetch the block which contains the string. If a styles section is
            // present, the strings block ends there; otherwise, it runs to the end
            // of this entry.
            uint stringsEnd = header.StyleCount > 0 ? header.StylesStart : header.Header.Size;
            byte[] rawStringData = ReadBytes((int)stringsEnd - (int)header.StringStart);

            bytesLeft -= rawStringData.Length;

            bool isUtf8 = (header.Flags & StringPoolFlags.UTF8_FLAG) == StringPoolFlags.UTF8_FLAG;

            foreach (uint startingIndex in stringIndices)
            {
                // The starting index specifies where the string starts.
                // We can now read the string in either UTF8 or UTF16 format.
                uint pos = startingIndex;
                if (isUtf8)
                {
                    uint charLen = Helper.DecodeLengthUtf8(rawStringData, ref pos);
                    uint byteLen = Helper.DecodeLengthUtf8(rawStringData, ref pos);
                    string item = Encoding.UTF8.GetString(rawStringData, (int)pos, (int)byteLen);
                    if (item.Length != charLen)
                    {
#if !CORECLR
                        Debug.WriteLine("Warning: UTF-8 string length ({0}) not matching specified length ({1}).",
                                        item.Length, charLen);
#endif
                    }
                    pool.StringData.Add(item);
                }
                else
                {
                    uint charLen = Helper.DecodeLengthUtf16(rawStringData, ref pos);
                    uint byteLen = charLen * 2;
                    string item = Encoding.Unicode.GetString(rawStringData, (int)pos, (int)byteLen);
                    pool.StringData.Add(item);
                }
            }

            // If styles are present, we should read them, too.
            if (header.StyleCount > 0)
            {
                byte[] rawStyleData = ReadBytes((int)header.Header.Size - (int)header.StylesStart);

                foreach (uint startingIndex in styleIndices)
                {
                    // At startingIndex, there are N entries defining the individual tags (b, i,...)
                    // that style the string at index i
                    // They are terminated by a value with value END (0xFFFFFFFF)
                    List<ResStringPool_span> styleData = new List<ResStringPool_span>();

                    int pos = (int)startingIndex;

                    while (true)
                    {
                        var index = BitConverter.ToUInt32(rawStyleData, pos);
                        var firstChar = BitConverter.ToUInt32(rawStyleData, pos + 4);
                        var lastChar = BitConverter.ToUInt32(rawStringData, pos + 8);

                        var span = new ResStringPool_span
                        {
                            Name = new ResStringPool_ref()
                            {
                                Index = index == 0xFFFFFFFFu ? (uint?)null : index
                            },
                            FirstChar = firstChar,
                            LastChar = lastChar,
                        };

                        styleData.Add(span);
                        if (span.IsEnd)
                        {
                            break;
                        }

                        pos += 12;
                    }

                    pool.StyleData.Add(styleData);
                }

                bytesLeft -= rawStyleData.Length;
            }

            // Make sure we didn't go out of bounds.
            if (bytesLeft < 0)
            {
                throw new InvalidDataException("The length of the content exceeds the ResStringPool block boundary.");
            }
            if (bytesLeft > 0)
            {
                // Padding: data is always aligned to 4 bytes.
#if !CORECLR
                Debug.WriteLine("Warning: Garbage at the end of the StringPool block. Padding?");
#endif
                ReadBytes((int)bytesLeft);
            }

            return pool;
        }

        public virtual ResXMLTree_startelement ReadResXMLTree_startelement(ResXMLTree_node node,
                                                                           ResXMLTree_attrExt attrExt)
        {
            var element = new ResXMLTree_startelement
            {
                Node = node,
                AttrExt = attrExt,
                Attributes = new List<ResXMLTree_attribute>()
            };

            uint bytesLeft = node.Header.Size - 0x24u;

            for (int i = 0; i < attrExt.AttributeCount; i++)
            {
                element.Attributes.Add(ReadResXMLTree_attribute());
                bytesLeft -= 0x14u;
            }

            if (bytesLeft < 0)
            {
                throw new InvalidDataException("The length of the content exceeds the ResStringPool block boundary.");
            }
            if (bytesLeft > 0)
            {
#if !CORECLR
                Debug.WriteLine("Warning: Garbage at the end of the StringPool block. Padding?");
#endif
                ReadBytes((int)bytesLeft);
            }

            return element;
        }

        public virtual ResResourceMap ReadResResourceMap(ResChunk_header header)
        {
            var result = new ResResourceMap
            {
                Header = header,
                ResouceIds = new List<uint>()
            };
            for (int pos = 8; pos < header.Size; pos += 4)
            {
                result.ResouceIds.Add(ReadUInt32());
            }
            return result;
        }
    }
}