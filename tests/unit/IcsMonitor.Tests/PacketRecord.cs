using PacketDotNet;
using SharpPcap;
using Traffix.Core.Flows;
using Traffix.Providers.PcapFile;

namespace IcsMonitor.Tests
{
    class PacketRecord
    {
        public long Ticks;
        public FlowKey Key;
        public Packet Packet;

        public static PacketRecord FromFrame(RawCapture arg)
        {
            var packet = Packet.ParsePacket(arg.LinkLayerType, arg.Data);
            return new PacketRecord { Ticks = arg.Timeval.Date.Ticks, Packet = packet, Key = packet.GetFlowKey() };
        }
    }
}
