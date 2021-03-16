using PacketDotNet;
using System;
using Traffix.Core;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster.Tests
{
    public static class TestHelperFunctions
    { 
        public static (long Ticks, Packet Packet) GetPacket(RawFrame arg)
        {
            return (arg.Ticks, Packet.ParsePacket(arg.LinkLayer, arg.Data));
        }

        public static Packet FrameProcessor(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
        {
            return Packet.ParsePacket((LinkLayers)frameMetadata.LinkLayer, frameBytes.ToArray());
        }
    }
}
