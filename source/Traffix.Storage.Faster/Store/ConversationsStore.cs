using System;

namespace Traffix.Storage.Faster
{

    /// <summary>
    /// A conversation store implements a key-value (persistent) database for conversation records.
    /// </summary>
    internal class ConversationsStore : KeyValueStore<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationsStore.ConversationFunctions>
    {
        /// <summary>
        /// Creates a new conversation store.
        /// </summary>
        /// <param name="folder">The folder for creating a persistent data storage.</param>
        /// <param name="capacity">The expected capacity of the store.</param>
        public ConversationsStore(string folder, long capacity) : base(folder, (int)Math.Log(capacity, 2) + 1, new ConversationKeyFastComparer(), new ConversationFunctions(), () => new ConversationKeySerializer(), () => new ConversationValueSerializer())
        {
        }

        /// <summary>
        /// Gets the total number of records in the conversations store.
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount() => ProcessEntries(null);
        internal class ConversationFunctions : StoreFunctions
        {
            public override void ConcurrentReader(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value, ref ConversationOutput dst)
            {
                dst.Key = key;
                dst.Value = value;
            }

            public override bool ConcurrentWriter(ref ConversationKey key, ref ConversationValue src, ref ConversationValue dst)
            {
                dst = src;
                return true;
            }

            public override void CopyUpdater(ref ConversationKey key, ref ConversationInput input, ref ConversationValue oldValue, ref ConversationValue newValue)
            {
                newValue = new ConversationValue(oldValue.FrameAddresses.Length * 2)
                {
                    FrameCount = oldValue.FrameCount,
                    ForwardFlow = oldValue.ForwardFlow,
                    ReverseFlow = oldValue.ReverseFlow,
                };
                Array.Copy(oldValue.FrameAddresses, newValue.FrameAddresses, oldValue.FrameCount);
                UpdateEntry(ref key, ref input, ref newValue);
            }

            public override void InitialUpdater(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value)
            {
                value = new ConversationValue(32)
                {
                    FrameCount = 1,
                    ForwardFlow = new FlowValue
                    {
                        FirstSeen = input.FrameTicks,
                        LastSeen = input.FrameTicks,
                        Packets = 1,
                        Octets = (ulong)input.FrameSize
                    }
                };
                value.FrameAddresses[0] = input.FrameAddress;
            }

            public override bool InPlaceUpdater(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value)
            {
                if (value.FrameCount < value.FrameAddresses.Length)
                {
                    UpdateEntry(ref key, ref input, ref value);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            private void UpdateEntry(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value)
            {
                if (Equals(key.FlowKey, input.FrameKey))
                {
                    UpdateFlow(ref input, ref value.ForwardFlow);
                }
                else
                {
                    UpdateFlow(ref input, ref value.ReverseFlow);
                }
                value.FrameAddresses[value.FrameCount] = input.FrameAddress;
                value.FrameCount++;
            }

            private void UpdateFlow(ref ConversationInput input, ref FlowValue flow)
            {
                flow.FirstSeen = Math.Min(flow.FirstSeen, input.FrameTicks);
                flow.LastSeen = Math.Max(flow.LastSeen, input.FrameTicks);
                flow.Packets++;
                flow.Octets += (ulong)input.FrameSize;
            }
            public override void SingleReader(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value, ref ConversationOutput dst)
            {
                dst.Key = key;
                dst.Value = value;
            }

            public override void SingleWriter(ref ConversationKey key, ref ConversationValue src, ref ConversationValue dst)
            {
                dst = src;
            }
        }
    }
}
