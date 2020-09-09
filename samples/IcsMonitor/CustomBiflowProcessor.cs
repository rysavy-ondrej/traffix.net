using PacketDotNet;
using System;
using System.Collections.Generic;
using Traffix.Core.Flows;
using Traffix.Storage.Faster;

namespace IcsMonitor
{
    public abstract class CustomBiflowProcessor<TData> : BiflowProcessor<ConversationRecord<TData>>
    {
        public override ConversationRecord<TData> Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
        {
            var fwdPackets = new List<Packet>();
            var revPackets = new List<Packet>();
            var forwardKeyHash = flowKey.GetHashCode64();
            var meta = new FrameMetadata();
            FlowMetrics fwdMetrics = new FlowMetrics();
            FlowMetrics revMetrics = new FlowMetrics();
            foreach (var frame in frames)
            {
                var buffer = GetFrame(frame, ref meta);
                var packet = Packet.ParsePacket((LinkLayers)meta.LinkLayer, buffer.ToArray());
                if (meta.FlowKeyHash == forwardKeyHash)
                {
                    AddPacket(fwdPackets, fwdMetrics, meta, packet);

                }
                else
                {
                    AddPacket(revPackets, revMetrics, meta, packet);
                }
            }

            return new ConversationRecord<TData>()
            {
                Key = flowKey,
                ForwardMetrics = fwdMetrics,
                ReverseMetrics = revMetrics,
                CustomData = Invoke(fwdPackets, revPackets)
            };
        }
        static DateTime nullDate = new DateTime();
        private static void AddPacket(List<Packet> packets, FlowMetrics metrics, FrameMetadata meta, Packet packet)
        {
            metrics.Octets += meta.IncludedLength;
            metrics.Packets++;
            var packetTimestamp = new DateTime(meta.Ticks);
            if (metrics.Start == nullDate || packetTimestamp < metrics.Start) metrics.Start = packetTimestamp;
            if (metrics.End == nullDate || packetTimestamp > metrics.End) metrics.End = packetTimestamp;
            packets.Add(packet);
        }

        protected abstract TData Invoke(IReadOnlyCollection<Packet> fwdPackets, IReadOnlyCollection<Packet> revPackets);
    }
}