using PacketDotNet;
using System;
using System.Collections.Generic;
using Traffix.Core;
using Traffix.Core.Flows;
using Traffix.Core.Processors;

namespace Traffix.Processors
{
    public readonly struct MetaPacket
    {
        public readonly FrameMetadata Metadata;
        public readonly Packet Packet;

        public MetaPacket(ref FrameMetadata metadata, Packet packet)
        {
            Metadata = metadata;
            Packet = packet;
        }
    }
    /// <summary>
    /// Implements base class for custom conversation processors. Custom conversation processors 
    /// work on <see cref="Packet"/> objects instead of raw frame data. Additionally, 
    /// the base implementation also computes flow metrics. The data type of this processor is   <see cref="ConversationRecord{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">The user data type. Used to parametrize <see cref="ConversationRecord{TData}"/> to define the target processor data type.</typeparam>
    abstract public class ConversationProcessorBase<TData> : IConversationProcessor<ConversationRecord<TData>>
    {
        public ConversationRecord<TData> Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
        {
            var fwdPackets = new List<MetaPacket>();
            var revPackets = new List<MetaPacket>();
            var forwardKeyHash = flowKey.GetHashCode64();
            var meta = new FrameMetadata();
            FlowMetrics fwdMetrics = new FlowMetrics();
            FlowMetrics revMetrics = new FlowMetrics();
            DateTime? firstTimestamp = null; 
            foreach (var frame in frames)
            {
                var buffer = FrameMetadata.FromBytes(frame.Span, ref meta);

                if (firstTimestamp == null) firstTimestamp = new DateTime(meta.Ticks);

                var packet = Packet.ParsePacket((LinkLayers)meta.LinkLayer, buffer.ToArray());
                if (meta.FlowKeyHash == forwardKeyHash)
                {
                    AddPacket(fwdPackets, ref fwdMetrics, ref meta, packet);

                }
                else
                {
                    AddPacket(revPackets, ref revMetrics, ref meta, packet);
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
                Data = Invoke(ref flowKey, ref fwdMetrics, ref revMetrics, fwdPackets, revPackets)
            };
        }
        static DateTime nullDate = new DateTime();
        private static void AddPacket(List<MetaPacket> packets, ref FlowMetrics metrics, ref FrameMetadata meta, Packet packet)
        {
            metrics.Octets += meta.OriginalLength;
            metrics.Packets++;
            var packetTimestamp = new DateTime(meta.Ticks);
            if (metrics.Start == nullDate || packetTimestamp < metrics.Start) metrics.Start = packetTimestamp;
            if (metrics.End == nullDate || packetTimestamp > metrics.End) metrics.End = packetTimestamp;
            packets.Add(new MetaPacket(ref meta, packet));
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
        protected abstract TData Invoke(ref FlowKey flowKey, ref FlowMetrics fwdMetrics, ref FlowMetrics revMetrics, IReadOnlyCollection<MetaPacket> fwdPackets, IReadOnlyCollection<MetaPacket> revPackets);
    }
      internal class PacketConversationProcessor<TData> : ConversationProcessorBase<TData>
    {
        ProcessConversationCallback<TData> _function;

        public PacketConversationProcessor(ProcessConversationCallback<TData> function)
        {
            this._function = function;
        }

        protected override TData Invoke(ref FlowKey flowKey, ref FlowMetrics fwdMetrics, ref FlowMetrics revMetrics, IReadOnlyCollection<MetaPacket> fwdPackets, IReadOnlyCollection<MetaPacket> revPackets)
        {
            return _function.Invoke(ref flowKey, ref fwdMetrics, ref revMetrics, fwdPackets, revPackets);
        }
    }
}