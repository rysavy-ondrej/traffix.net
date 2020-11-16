using FASTER.core;
using System;
using System.Diagnostics.CodeAnalysis;
using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{
    public interface IConversationKey
    {
        FlowKey FlowKey { get; }
        long HashCode64 { get; }
    }

    /// <summary>
    /// Represents a conversation key. Usually, a conversation key is a key of the forward flow.
    /// </summary>
    internal struct ConversationKey : IEquatable<ConversationKey>, IConversationKey
    {
        /// <summary>
        /// The flow key.
        /// </summary>
        internal FlowKey FlowKey;

        /// <summary>
        /// The precomputed 64-bit hash code of the flow key.
        /// </summary>
        internal long HashCode;

        FlowKey IConversationKey.FlowKey => this.FlowKey;

        long IConversationKey.HashCode64 => this.HashCode;

        internal ConversationKey(FlowKey flowKey)
        {
            FlowKey = flowKey;
            HashCode = flowKey.GetHashCode64();
        }

        public bool Equals([AllowNull] ConversationKey other) => this.FlowKey.Equals(other.FlowKey);

        public override int GetHashCode()
        {
            return (int)HashCode ^ ((int)HashCode >> 32);
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
            return k1.HashCode == k2.HashCode && Equals(k1.FlowKey, k2.FlowKey); 
        }

        public long GetHashCode64(ref ConversationKey key)
        {
            return key.HashCode;
        }
    }

    internal class ConversationKeySerializer : BinaryObjectSerializer<ConversationKey>
    {
        public override void Deserialize(ref ConversationKey obj)
        {
            obj.HashCode = reader.ReadInt64();
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
            writer.Write(obj.HashCode);
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
