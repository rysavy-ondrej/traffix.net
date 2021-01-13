using FASTER.core;
using System;
namespace Traffix.Storage.Faster
{
    internal class ConversationFunctions : KeyValueStore<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationFunctions>.StoreFunctions
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
            InPlaceUpdater(ref key, ref input, ref newValue);
        }

        public override void InitialUpdater(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value)
        {
            value = new ConversationValue(32)
            {
                FrameCount = 1,
                ForwardFlow = new FlowMetrics
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
            // cannot update in-place
            if (value.FrameCount == value.FrameAddresses.Length) return false;
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
            return true;  
        }

        private void UpdateFlow(ref ConversationInput input, ref FlowMetrics flow)
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

    internal class ConversationsStore : KeyValueStore<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationFunctions>
    {
        public ConversationsStore(string folder, long capacity) : base(folder, (int)Math.Log(capacity,2)+1, new ConversationKeyFastComparer(), new ConversationFunctions(), () => new ConversationKeySerializer(), () => new ConversationValueSerializer())
        {
        }
    }
}
