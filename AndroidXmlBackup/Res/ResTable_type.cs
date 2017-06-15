// Copyright (c) 2012 Markus Jarderot
//
// This software may be modified and distributed under the terms
// of the MIT license.  See the LICENSE file for details.

using System;
using AndroidXml.Utils;

namespace AndroidXml.Res
{
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public class ResTable_type
    {
        public ResChunk_header Header { get; set; }
        public uint RawID { get; set; }
        public uint EntryCount { get; set; }
        public uint EntriesStart { get; set; }
        public ResTable_config Config { get; set; }

        public byte ID
        {
            get { return (byte) Helper.GetBits(RawID, 0xFFu, 24); }
            set { RawID = Helper.SetBits(RawID, value, 0xFFu, 24); }
        }

        public byte Res0
        {
            get { return (byte) Helper.GetBits(RawID, 0xFFu, 16); }
            set { RawID = Helper.SetBits(RawID, value, 0xFFu, 16); }
        }

        public ushort Res1
        {
            get { return (ushort) Helper.GetBits(RawID, 0xFFFFu, 0); }
            set { RawID = Helper.SetBits(RawID, value, 0xFFFFu, 0); }
        }
    }
}