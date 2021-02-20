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

    /// <summary>
    /// Provides the conversation value, which consists of pair of flow meta data and 
    /// a collection of frames.
    /// </summary>
    public class ConversationValue
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
        public ulong[] FrameAddresses;

        /// <summary>
        /// Default parameterless constructor.
        /// </summary>
        public ConversationValue() 
        {
            FrameAddresses = Array.Empty<ulong>();
        }

        /// <summary>
        /// Creates a new object and allocates the <see cref="FrameAddresses"/> to the specified size.
        /// </summary>
        /// <param name="initialAddresses">The requested size of <see cref="FrameAddresses"/> array.</param>
        internal ConversationValue(int initialAddresses)
        {
            FrameAddresses = new ulong[initialAddresses];
        }
                                               
        public ulong Octets => ForwardFlow.Octets + ReverseFlow.Octets;

        public uint Packets => ForwardFlow.Packets + ReverseFlow.Packets;

        public long FirstSeen => Ticks.Min(ForwardFlow.FirstSeen, ReverseFlow.FirstSeen);

        public long LastSeen => Ticks.Max(ForwardFlow.LastSeen, ReverseFlow.LastSeen);
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
            value.FrameAddresses = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                value.FrameAddresses[i] = reader.ReadUInt64();
            }
        }
    }
    static class Ticks
    {
        // 6,93792e16
        // 9,223372036854776e18
        static readonly long TicksBase = new DateTime(1970, 1,1).Ticks; 
        public static long Min(long ticks1, long ticks2)
        {
            if (ticks1 != 0 && ticks2 != 0) return Math.Min(ticks1, ticks2);
            return ticks1 == 0 ? ticks2 : ticks1;
        }
        public static long Max(long ticks1, long ticks2)
        {
            if (ticks1 != 0 && ticks2 != 0) return Math.Max(ticks1, ticks2);
            return ticks1 == 0 ? ticks2 : ticks1;
        }

        internal static long GetEpochTicks(long ticks)
        {
            return ticks - TicksBase;
        }
    }
}
