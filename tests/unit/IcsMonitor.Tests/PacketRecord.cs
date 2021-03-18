using PacketDotNet;
using Traffix.Core.Flows;
using Traffix.Providers.PcapFile;

namespace IcsMonitor.Tests
{
    class PacketRecord
    {
        public long Ticks;
        public FlowKey Key;
        public Packet Packet;

        public static PacketRecord FromFrame(RawFrame arg)
        {
            var packet = Packet.ParsePacket(arg.LinkLayer, arg.Data);
            return new PacketRecord { Ticks = arg.Ticks, Packet = packet, Key = packet.GetFlowKey() };
        }
    }
}
