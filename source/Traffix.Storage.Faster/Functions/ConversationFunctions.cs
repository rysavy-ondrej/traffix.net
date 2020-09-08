using FASTER.core;
using System;
using System.Runtime.CompilerServices;

namespace Traffix.Storage.Faster
{
    internal class ConversationFunctions : IFunctions<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationContext>
    {
        public void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint)
        {
            throw new System.NotImplementedException();
        }

        public void ConcurrentReader(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value, ref ConversationOutput dst)
        {
            dst.Key = key;
            dst.Value = value;
        }

        public bool ConcurrentWriter(ref ConversationKey key, ref ConversationValue src, ref ConversationValue dst)
        {
            dst = src;
            return true;
        }

        public void CopyUpdater(ref ConversationKey key, ref ConversationInput input, ref ConversationValue oldValue, ref ConversationValue newValue)
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

        public void InitialUpdater(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value)
        {
            value = new ConversationValue(16)
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

        ConversationKeyFastComparer comparer = new ConversationKeyFastComparer();
        public bool InPlaceUpdater(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value)
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

        public void ReadCompletionCallback(ref ConversationKey key, ref ConversationInput input, ref ConversationOutput output, ConversationContext ctx, Status status)
        {
            ctx.OutputValues.Add(output);    
        }

        public void RMWCompletionCallback(ref ConversationKey key, ref ConversationInput input, ConversationContext ctx, Status status)
        {
            throw new System.NotImplementedException();
        }

        public void SingleReader(ref ConversationKey key, ref ConversationInput input, ref ConversationValue value, ref ConversationOutput dst)
        {
            dst.Key = key;
            dst.Value = value;
        }

        public void SingleWriter(ref ConversationKey key, ref ConversationValue src, ref ConversationValue dst)
        {
            dst = src;
        }

        public void UpsertCompletionCallback(ref ConversationKey key, ref ConversationValue value, ConversationContext ctx)
        {
            throw new System.NotImplementedException();
        }
                                                                                                         
        public static FasterKV<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationContext, ConversationFunctions> CreateFaster(IDevice log, IDevice objLog)
        {
            return new FasterKV<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationContext, ConversationFunctions>(
                size: 1L << 20,  // about 1M conversations
                functions: new ConversationFunctions(), 
                logSettings: new LogSettings { LogDevice = log, ObjectLogDevice = objLog },
                comparer: new ConversationKeyFastComparer(),
                serializerSettings: new SerializerSettings<ConversationKey, ConversationValue> { keySerializer = () => new ConversationKeySerializer(), valueSerializer = () => new ConversationValueSerializer() }
                
                );
        }

        public void DeleteCompletionCallback(ref ConversationKey key, ConversationContext ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}
