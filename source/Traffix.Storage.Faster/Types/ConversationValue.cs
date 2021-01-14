using FASTER.core;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Buffers;
using System.ComponentModel;

namespace Traffix.Storage.Faster
{
    public interface IConversationValue
    {
        FlowMetrics ForwardFlowMetrics { get; }

        FlowMetrics ReverseFlowMetrics { get; }
        long FirstSeen { get; }
        long LastSeen { get; }
        ulong Octets { get; }
        uint Packets { get; }
    }

    /// <summary>
    /// Provides the conversation value, which consists of pair of flow meta data and 
    /// a collection of frames.
    /// </summary>
    public class ConversationValue : IConversationValue
    {
        /// <summary>
        /// Provides meta information about the forward flow.
        /// </summary>
        public FlowMetrics ForwardFlow;

        /// <summary>
        /// Provides meta information about the reverse flow.
        /// </summary>
        public FlowMetrics ReverseFlow;
        /// <summary>
        /// The actual number of flows. 
        /// <para/>
        /// <see cref="FrameAddresses"/> can be larger than <see cref="FrameCount"/>.
        /// It enables to use In-place update which is faster for most of the RMW operations.
        /// </summary>
        public int FrameCount;
        /// <summary>
        /// An array of frame numbers.
        /// </summary>
        public long[] FrameAddresses;

        /// <summary>
        /// Default parameterless constructor.
        /// </summary>
        public ConversationValue() 
        {
            FrameAddresses = Array.Empty<long>();
        }

        /// <summary>
        /// Creates a new object and allocates the <see cref="FrameAddresses"/> to the specified size.
        /// </summary>
        /// <param name="initialAddresses">The requested size of <see cref="FrameAddresses"/> array.</param>
        internal ConversationValue(int initialAddresses)
        {
            FrameAddresses = new long[initialAddresses];
        }
                                               
        #region Public interface - IConversationValue
        public ulong Octets => ForwardFlow.Octets + ReverseFlow.Octets;

        public uint Packets => ForwardFlow.Packets + ReverseFlow.Packets;

        public long FirstSeen => ReverseFlow.Packets != 0 ? Math.Min(ForwardFlow.FirstSeen, ReverseFlow.FirstSeen) : ForwardFlow.FirstSeen;

        public long LastSeen => Math.Max(ForwardFlow.LastSeen, ReverseFlow.LastSeen);

        public FlowMetrics ForwardFlowMetrics => this.ForwardFlow;

        public FlowMetrics ReverseFlowMetrics => this.ReverseFlow;
        #endregion
    }
    internal class ConversationValueSerializer : BinaryObjectSerializer<ConversationValue>
    {
        public override void Serialize(ref ConversationValue value)
        {
            writer.Write(value.ForwardFlow.FirstSeen);
            writer.Write(value.ForwardFlow.LastSeen);
            writer.Write(value.ForwardFlow.Octets);
            writer.Write(value.ForwardFlow.Packets);
            writer.Write(value.ReverseFlow.FirstSeen);
            writer.Write(value.ReverseFlow.LastSeen);
            writer.Write(value.ReverseFlow.Octets);
            writer.Write(value.ReverseFlow.Packets);
            writer.Write(value.FrameCount);
            writer.Write(value.FrameAddresses.Length);
            for(int i=0; i < value.FrameAddresses.Length; i++)
            {
                writer.Write(value.FrameAddresses[i]);
            }
        }

        public override void Deserialize(out ConversationValue value)
        {
            value = new ConversationValue();
            value.ForwardFlow.FirstSeen = reader.ReadInt64();
            value.ForwardFlow.LastSeen = reader.ReadInt64();
            value.ForwardFlow.Octets = reader.ReadUInt64();
            value.ForwardFlow.Packets = reader.ReadUInt32();
            value.ReverseFlow.FirstSeen = reader.ReadInt64();
            value.ReverseFlow.LastSeen = reader.ReadInt64();
            value.ReverseFlow.Octets = reader.ReadUInt64();
            value.ReverseFlow.Packets = reader.ReadUInt32();
            value.FrameCount = reader.ReadInt32();
            var size = reader.ReadInt32();
            value.FrameAddresses = new long[size];
            for (int i = 0; i < size; i++)
            {
                value.FrameAddresses[i] = reader.ReadInt64();
            }
        }
    }

}
