using PacketDotNet;
using System;

namespace Traffix.Providers.PcapFile
{

    /// <summary>
    /// Represents a raw frame capture. 
    /// <para/>
    /// It consist of metadata and frame bytes.
    /// </summary>
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
        public RawFrame(LinkLayers linkLayer, int number, long ticks, long offset, int originalLength, byte[] bytes)
        {
            LinkLayer = linkLayer;
            Number = number;
            Ticks = ticks;
            Offset = offset;
            IncludedLength = bytes.Length;
            OriginalLength = originalLength;
            Data = bytes;
        }

        public int OriginalLength { get; }
        public int IncludedLength { get; }

        public long Ticks { get; }

        public int Number { get; }

        public long Offset { get;  }

        public LinkLayers LinkLayer { get;  }

        public byte[] Data { get; internal set; }
    }
}
