using MessagePack;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Traffix.Core.Flows
{
    [Union(NullFlowKey.FlowKeyType, typeof(NullFlowKey))]
    [Union(FlowKeyInternetwork.FlowKeyType, typeof(FlowKeyInternetwork))]
    [Union(FlowKeyInternetworkV6.FlowKeyType, typeof(FlowKeyInternetworkV6))]
    [MessagePackObject]
    public abstract class FlowKey : IEquatable<FlowKey>
    {

        [IgnoreMember]
        public abstract AddressFamily AddressFamily { get; }

        [IgnoreMember]
        public abstract ProtocolType ProtocolType { get; }

        [IgnoreMember]
        public abstract IPAddress SourceIpAddress { get; }

        [IgnoreMember]
        public abstract IPAddress DestinationIpAddress { get; }

        [IgnoreMember]
        public abstract ushort SourcePort { get; }

        [IgnoreMember]
        public abstract ushort DestinationPort { get; }

        public static FlowKey Create(AddressFamily addressFamily, ProtocolType protocolType, ReadOnlySpan<byte> sourceAddress, ushort sourcePort, ReadOnlySpan<byte> destinationAddress, ushort destinationPort)
        {
            switch (addressFamily)
            {
                case AddressFamily.InterNetwork:
                    return new FlowKeyInternetwork(protocolType, sourceAddress, sourcePort, destinationAddress, destinationPort);
                case AddressFamily.InterNetworkV6:
                    return new FlowKeyInternetworkV6(protocolType, sourceAddress, sourcePort, destinationAddress, destinationPort);
                default:
                    return NullFlowKey.Instance;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = GetHashCode64();
            return ((int)hashCode) ^ (int)(hashCode >> 32);
        }

        public override bool Equals(object obj)
        {
            var other = (FlowKey)obj;
            if (other == null)
                return false;
            else 
                return Equals((FlowKey)obj);
        }

        public bool EqualsOrReverse(FlowKey other)
        {
            return Equals(this, other) || Equals(this, other.Reverse());
        }

        public abstract long GetHashCode64();

        public abstract bool Equals(FlowKey other);

        /// <summary>
        /// Gets the flow key for the opposite flow.
        /// </summary>
        /// <returns>The flow key for the opposite flow.</returns>
        public abstract FlowKey Reverse();

        public abstract Span<byte> GetBytes();

        /// <summary>
        /// Parses the flow key string created by ToString methods.
        /// </summary>
        /// <param name="flowString">The flow key string.</param>
        /// <param name="flowKey">The flow key object.</param>
        /// <returns>True on success. False if the input string cannot be parsed to a valid flow key.</returns>
        public static bool TryParse(string flowString, out FlowKey? flowKey)
        {
            var m1 = ipv4FlowRegex.Match(flowString);
            if (m1.Success)
            {
                if (Enum.TryParse<ProtocolType>(m1.Groups[1].Value, out var protocolType)
                    && IPAddress.TryParse(m1.Groups[2].Value, out var srcAddress)
                    && ushort.TryParse(m1.Groups[3].Value, out var srcPort)
                    && IPAddress.TryParse(m1.Groups[4].Value, out var dstAddress)
                    && ushort.TryParse(m1.Groups[5].Value, out var dstPort))
                {
                    flowKey = FlowKey.Create(AddressFamily.InterNetwork, protocolType, srcAddress.GetAddressBytes(), srcPort, dstAddress.GetAddressBytes(), dstPort);
                    return true;
                }
                else
                {
                    flowKey = null;
                    return false;
                }
            }
            var m2 = ipv6FlowRegex.Match(flowString);
            if (m2.Success)
            {
                if (Enum.TryParse<ProtocolType>(m2.Groups[1].Value, out var protocolType)
                    && IPAddress.TryParse(m2.Groups[2].Value, out var srcAddress)
                    && ushort.TryParse(m2.Groups[3].Value, out var srcPort)
                    && IPAddress.TryParse(m2.Groups[4].Value, out var dstAddress)
                    && ushort.TryParse(m2.Groups[5].Value, out var dstPort))
                {
                    flowKey = FlowKey.Create(AddressFamily.InterNetworkV6, protocolType, srcAddress.GetAddressBytes(), srcPort, dstAddress.GetAddressBytes(), dstPort);
                    return true;
                }
                else
                {
                    flowKey = null;
                    return false;
                }
            }
            flowKey = null;
            return false;
        }
        static Regex ipv4FlowRegex = new Regex(@"([A-Z]+)\$([0-9.]+):([0-9]+)->([0-9.]+):([0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex ipv6FlowRegex = new Regex(@"([A-Z]+)\$\[([0-9a-z:]+)\]:([0-9]+)->\[([0-9a-z:]+)\]:([0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
