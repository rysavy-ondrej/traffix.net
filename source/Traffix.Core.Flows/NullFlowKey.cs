using MessagePack;
using System;
using System.Net;
using System.Net.Sockets;

namespace Traffix.Core.Flows
{
    [MessagePackObject]
    public sealed class NullFlowKey : FlowKey
    {
        public const int FlowKeyType = 0;

        [IgnoreMember]
        public override ProtocolType ProtocolType => ProtocolType.Unspecified;


        [IgnoreMember]
        public override AddressFamily AddressFamily => AddressFamily.Unspecified;


        [IgnoreMember]
        public override IPAddress SourceIpAddress => IPAddress.None;


        [IgnoreMember]
        public override IPAddress DestinationIpAddress => IPAddress.None;


        [IgnoreMember]
        public override ushort SourcePort => 0;


        [IgnoreMember]
        public override ushort DestinationPort => 0;

        public override bool Equals(FlowKey other)
        {
            return other is NullFlowKey;
        }
        public static FlowKey Instance { get; } = new NullFlowKey();

        public override FlowKey Reverse() => Instance;

        public override long GetHashCode64()
        {
            return 0L;
        }

        public override Span<byte> GetBytes()
        {
            return Span<byte>.Empty;
        }
    }
}
