using PacketDotNet;
using System;
using Traffix.Core;
using Traffix.Providers.PcapFile;
using Traffix.Core.Flows;
using SharpPcap;

namespace Traffix.Storage.Faster.Tests
{
    public static class TestHelperFunctions
    { 
        public static (long Ticks, Packet Packet) GetPacket(RawCapture arg)
        {
            return (arg.Timeval.Date.Ticks, arg.GetPacket());
        }

        public static Packet FrameProcessor(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
        {
            return Packet.ParsePacket((LinkLayers)frameMetadata.LinkLayer, frameBytes.ToArray());
        }

        public static (long Ticks, FlowKey Key, Packet Packet) GetPacketAndKey(RawCapture arg)
        {
            var packet = arg.GetPacket();
            return (arg.Timeval.Date.Ticks, packet.GetFlowKey(), packet);
        }
    }
}
