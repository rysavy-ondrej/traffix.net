using FASTER.core;
using System;
using System.Diagnostics.CodeAnalysis;
using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// Represents a conversation key. Usually, a conversation key is a key of the forward flow.
    /// </summary>
    public struct ConversationKey : IEquatable<ConversationKey>
    {
        /// <summary>
        /// The precomputed 64-bit hash code of the flow key.
        /// </summary>
        public long HashCode64;

        /// <summary>
        /// The conversation initiator flow key.
        /// </summary>
        public FlowKey FlowKey; 


        public ConversationKey(FlowKey flowKey)
        {
            FlowKey = flowKey;
            HashCode64 = flowKey.GetHashCode64();
        }

        public bool Equals([AllowNull] ConversationKey other) => this.FlowKey.Equals(other.FlowKey);

        public override int GetHashCode()
        {
            return (int)HashCode64 ^ ((int)HashCode64 >> 32);
        }

        public override bool Equals(object other) => other is ConversationKey l && Equals(l);

        public override string ToString()
        {
            return FlowKey.ToString();
        }

        public static bool operator ==(ConversationKey left, ConversationKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConversationKey left, ConversationKey right)
        {
            return !(left == right);
        }
    }

    internal class ConversationKeyFastComparer : IFasterEqualityComparer<ConversationKey>
    {
        public bool Equals(ref ConversationKey k1, ref ConversationKey k2)
        {
            return k1.HashCode64 == k2.HashCode64 && Equals(k1.FlowKey, k2.FlowKey); 
        }

        public long GetHashCode64(ref ConversationKey key)
        {
            return key.HashCode64;
        }
    }

    internal class ConversationKeySerializer : BinaryObjectSerializer<ConversationKey>
    {
        public override void Deserialize(out ConversationKey obj)
        {
            obj.HashCode64 = reader.ReadInt64();
            var flowType = reader.ReadInt32();
            switch(flowType)
            {
                case FlowKeyInternetwork.FlowKeyType:
                    obj.FlowKey = new FlowKeyInternetwork();
                    break;
                case FlowKeyInternetworkV6.FlowKeyType:
                    obj.FlowKey = new FlowKeyInternetworkV6();
                    break;
                case NullFlowKey.FlowKeyType:
                    obj.FlowKey = new NullFlowKey();
                    break;
                default:
                    throw new NotSupportedException();
            }

            reader.Read(obj.FlowKey.GetBytes());
        }

        public override void Serialize(ref ConversationKey obj)
        {
            writer.Write(obj.HashCode64);
            switch(obj.FlowKey)
            {
                case FlowKeyInternetwork _:
                    writer.Write(FlowKeyInternetwork.FlowKeyType);
                    break;
                case FlowKeyInternetworkV6 _:
                    writer.Write(FlowKeyInternetworkV6.FlowKeyType);
                    break;
                case NullFlowKey _:
                    writer.Write(NullFlowKey.FlowKeyType);
                    break;
                default:
                    throw new NotSupportedException();
            }
            writer.Write(obj.FlowKey.GetBytes());
        }
    }
}
