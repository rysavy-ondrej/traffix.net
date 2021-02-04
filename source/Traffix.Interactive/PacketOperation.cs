using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using Traffix.Core;
using Traffix.Core.Flows;
using Traffix.Data;
using Traffix.Storage.Faster;

namespace Traffix.Interactive
{
    public sealed class PacketOperation
    {
        private Interactive _interactive;

        public PacketOperation(Interactive interactive)
        {
            this._interactive = interactive;
        }

        /// <summary>
        /// Reads all packets available in conversation <paramref name="table"/>.
        /// </summary>
        /// <param name="table">The conversation table.</param>
        /// <returns>A collection of packets.</returns>
        public IEnumerable<(long Ticks, Packet Packet)> ReadAllPackets(FasterConversationTable table)
        {
            return table.ProcessFrames<(long, Packet)>(table.FrameKeys, new PacketProcessor());
        }

        class PacketProcessor : IFrameProcessor<(long Ticks, Packet Packet)>
        {
            public (long Ticks, Packet Packet) Invoke(FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
            {
                return (frameMetadata.Ticks, Packet.ParsePacket((LinkLayers)frameMetadata.LinkLayer, frameBytes.ToArray()));
            }
        }

        /// <summary>
        /// Tests if <paramref name="packet"/> belongs to the given conversation specified by its <paramref name="conversationKey"/>.
        /// </summary>
        /// <param name="table">The conversation table providing <see cref="FasterConversationTable.GetFlowKey(Packet)"/> operation.</param>
        /// <param name="conversationKey">The conversation key.</param>
        /// <param name="packet">The packet.</param>
        /// <returns>true if the packet belongs to the conversation; false otherwise</returns>
        public bool ContainsPacket(FasterConversationTable table, FlowKey conversationKey, Packet packet)
        {
            var packetKey = table.GetFlowKey(packet);
            return conversationKey.EqualsOrReverse(packetKey);
        }
    }
}