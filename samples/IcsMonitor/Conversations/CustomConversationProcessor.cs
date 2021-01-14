using PacketDotNet;
using System;
using System.Collections.Generic;
using Traffix.Core.Flows;
using Traffix.Storage.Faster;

namespace IcsMonitor
{
    public abstract class CustomConversationProcessor<TData> : ConversationProcessor<ConversationRecord<TData>>
    {
        public override ConversationRecord<TData> Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
        {
            var fwdPackets = new List<(FrameMetadata, Packet)>();
            var revPackets = new List<(FrameMetadata, Packet)>();
            var forwardKeyHash = flowKey.GetHashCode64();
            var meta = new FrameMetadata();
            FlowMetrics fwdMetrics = new FlowMetrics();
            FlowMetrics revMetrics = new FlowMetrics();
            DateTime? firstTimestamp = null; 
            foreach (var frame in frames)
            {
                var buffer = GetFrame(frame, ref meta);

                if (firstTimestamp == null) firstTimestamp = new DateTime(meta.Ticks);

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
            if (firstTimestamp != null)
            {
                // adjust metrics:
                AdjustMetrics(ref fwdMetrics, firstTimestamp.Value);
                AdjustMetrics(ref revMetrics, firstTimestamp.Value);
            }
            return new ConversationRecord<TData>()
            {
                Key = flowKey,
                ForwardMetrics = fwdMetrics,
                ReverseMetrics = revMetrics,
                Data = Invoke(fwdPackets, revPackets)
            };
        }
        static DateTime nullDate = new DateTime();
        private static void AddPacket(List<(FrameMetadata,Packet)> packets, FlowMetrics metrics, FrameMetadata meta, Packet packet)
        {
            metrics.Octets += meta.OriginalLength;
            metrics.Packets++;
            var packetTimestamp = new DateTime(meta.Ticks);
            if (metrics.Start == nullDate || packetTimestamp < metrics.Start) metrics.Start = packetTimestamp;
            if (metrics.End == nullDate || packetTimestamp > metrics.End) metrics.End = packetTimestamp;
            packets.Add((meta, packet));
        }
        private void AdjustMetrics(ref FlowMetrics metrics, DateTime timestamp)
        {
            if (metrics.Start == DateTime.MinValue)
            {
                metrics.Start = timestamp;
            }
            if (metrics.End == DateTime.MinValue)
            {
                metrics.End = timestamp;
            }
        }

        protected abstract TData Invoke(IReadOnlyCollection<(FrameMetadata Meta,Packet Packet)> fwdPackets, IReadOnlyCollection<(FrameMetadata Meta,Packet Packet)> revPackets);
    }
}