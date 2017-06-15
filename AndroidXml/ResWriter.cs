// Copyright (c) 2012 Markus Jarderot
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using System.IO;
using System.Text;
using AndroidXml.Res;

namespace AndroidXml
{
    public class ResWriter
    {
        protected readonly BinaryWriter _writer;

        public ResWriter(Stream output, Encoding encoding, bool leaveOpen)
            : this(new BinaryWriter(output, encoding, leaveOpen))
        {
        }

        public ResWriter(Stream output)
            : this(new BinaryWriter(output))
        {
        }

        public ResWriter(BinaryWriter writer)
        {
            _writer = writer;
        }

        /// <summary>
        /// Gets the underlying <c>BinaryWriter</c> to write primitive values.
        /// </summary>
        public BinaryWriter Writer
        {
            get { return _writer; }
        }

        public virtual void Write(Res_value data)
        {
            _writer.Write(data.Size);
            _writer.Write(data.Res0);
            _writer.Write((byte) data.DataType);
            _writer.Write(data.RawData);
        }

        public virtual void Write(ResChunk_header data)
        {
            _writer.Write((ushort) data.Type);
            _writer.Write(data.HeaderSize);
            _writer.Write(data.Size);
        }

        public virtual void Write(ResStringPool_header data)
        {
            Write(data.Header);
            _writer.Write(data.StringCount);
            _writer.Write(data.StyleCount);
            _writer.Write((uint) data.Flags);
            _writer.Write(data.StringStart);
            _writer.Write(data.StylesStart);
        }

        public virtual void Write(ResStringPool_ref data)
        {
            _writer.Write(data.Index ?? 0xFFFFFFFFu);
        }

        public virtual void Write(ResStringPool_span data)
        {
            Write(data.Name);
            _writer.Write(data.FirstChar);
            _writer.Write(data.LastChar);
        }

        public virtual void Write(ResTable_config data)
        {
            _writer.Write(data.Size);

            _writer.Write(data.IMSI_MCC);
            _writer.Write(data.IMSI_MNC);

            _writer.Write(Encoding.ASCII.GetBytes(data.LocaleLanguage));
            _writer.Write(Encoding.ASCII.GetBytes(data.LocaleCountry));

            _writer.Write((byte)data.ScreenTypeOrientation);
            _writer.Write((byte)data.ScreenTypeTouchscreen);
            _writer.Write((ushort)data.ScreenTypeDensity);

            _writer.Write((byte)data.InputKeyboard);
            _writer.Write((byte)data.InputNavigation);
            _writer.Write(data.InputFlags);
            _writer.Write(data.Input_Pad0);

            _writer.Write(data.ScreenSizeWidth);
            _writer.Write(data.ScreenSizeHeight);

            _writer.Write(data.VersionSdk);
            _writer.Write(data.VersionMinor);

            _writer.Write(data.ScreenConfigScreenLayout);
            _writer.Write(data.ScreenConfigUIMode);
            _writer.Write(data.ScreenConfigSmallestScreenWidthDp);

            _writer.Write(Encoding.ASCII.GetBytes(data.LocaleScript));
            _writer.Write(Encoding.ASCII.GetBytes(data.LocaleVariant));

            _writer.Write(data.ScreenLayout2);
            _writer.Write(data.ScreenConfigPad1);
            _writer.Write(data.ScreenConfigPad2);
        }

        public virtual void Write(ResTable_entry data)
        {
            _writer.Write(data.Size);
            _writer.Write((ushort) data.Flags);
            Write(data.Key);
        }

        public virtual void Write(ResTable_header data)
        {
            Write(data.Header);
            _writer.Write(data.PackageCount);
        }

        public virtual void Write(ResTable_map data)
        {
            Write(data.Name);
            Write(data.Value);
        }

        public virtual void Write(ResTable_map_entry data)
        {
            _writer.Write(data.Size);
            _writer.Write((ushort) data.Flags);
            Write(data.Key);
            Write(data.Parent);
            _writer.Write(data.Count);
        }

        public virtual void Write(ResTable_package data)
        {
            Write(data.Header);
            _writer.Write(data.Id);
            var stringData = new byte[256];
            byte[] tempData = Encoding.Unicode.GetBytes(data.Name);
            int length = Math.Min(255, tempData.Length); // last pair of bytes must be 0
            Array.Copy(tempData, stringData, length);
            _writer.Write(stringData);
            _writer.Write(data.TypeStrings);
            _writer.Write(data.LastPublicType);
            _writer.Write(data.KeyStrings);
            _writer.Write(data.LastPublicKey);
        }

        public virtual void Write(ResTable_ref data)
        {
            _writer.Write(data.Ident ?? 0xFFFFFFFFu);
        }

        public virtual void Write(ResTable_type data)
        {
            Write(data.Header);
            _writer.Write(data.RawID);
            _writer.Write(data.EntryCount);
            _writer.Write(data.EntriesStart);
            Write(data.Config);
        }

        public virtual void Write(ResTable_typeSpec data)
        {
            Write(data.Header);
            _writer.Write(data.RawID);
            _writer.Write(data.EntryCount);
        }

        public virtual void Write(ResXMLTree_attrExt data)
        {
            Write(data.Namespace);
            Write(data.Name);
            _writer.Write(data.AttributeStart);
            _writer.Write(data.AttributeSize);
            _writer.Write(data.AttributeCount);
            _writer.Write(data.IdIndex);
            _writer.Write(data.ClassIndex);
            _writer.Write(data.StyleIndex);
        }

        public virtual void Write(ResXMLTree_attribute data)
        {
            Write(data.Namespace);
            Write(data.Name);
            Write(data.RawValue);
            Write(data.TypedValue);
        }

        public virtual void Write(ResXMLTree_cdataExt data)
        {
            Write(data.Data);
            Write(data.TypedData);
        }

        public virtual void Write(ResXMLTree_endElementExt data)
        {
            Write(data.Namespace);
            Write(data.Name);
        }

        public virtual void Write(ResXMLTree_header data)
        {
            Write(data.Header);
        }

        public virtual void Write(ResXMLTree_namespaceExt data)
        {
            Write(data.Prefix);
            Write(data.Uri);
        }

        public virtual void Write(ResXMLTree_node data)
        {
            Write(data.Header);
            _writer.Write(data.LineNumber);
            Write(data.Comment);
        }
    }
}