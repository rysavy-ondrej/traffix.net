using PacketDotNet;
using System;

namespace Traffix.Providers.PcapFile
{

    public class RawFrame
    {
        public RawFrame(LinkLayers linkLayer, int number, long ticks, long offset, int includedLength, int originalLength)
        {
            LinkLayer = linkLayer;
            Number = number;
            Ticks = ticks;
            Offset = offset;
            IncludedLength = includedLength;
            OriginalLength = originalLength;
        }

        public int OriginalLength { get; }
        public int IncludedLength { get; }

        public long Ticks { get; }

        public int Number { get; }

        public long Offset { get;  }

        public PacketDotNet.LinkLayers LinkLayer { get;  }

        public byte[] Data { get; internal set; }
    }
}
