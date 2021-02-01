using PacketDotNet;
using System.Net;
using System.Net.Sockets;

namespace Traffix.Core.Flows
{

    public class PacketKeyProvider : IFlowKeyProvider<FlowKey, Packet>
    {
        public FlowKey GetKey(Packet packet)
        {
            return GetFlowKeyFromPacket(packet);
        }

        public int GetKeyHash(Packet packet)
        {
            return GetFlowKeyFromPacket(packet).GetHashCode();
        }

        public static FlowKey GetFlowKeyFromPacket(Packet packet)
        {
            AddressFamily GetAddressFamily(IPVersion version) => version == IPVersion.IPv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
            FlowKey GetUdpFlowKey(UdpPacket udp)
            {
                var ip = udp.ParentPacket as IPPacket;
                return FlowKey.Create(
                    GetAddressFamily(ip.Version),
                    System.Net.Sockets.ProtocolType.Udp,
                    ip.SourceAddress.GetAddressBytes(),
                    udp.SourcePort,
                    ip.DestinationAddress.GetAddressBytes(),
                    udp.DestinationPort);
            }
            FlowKey GetTcpFlowKey(TcpPacket tcp)
            {
                var ip = tcp.ParentPacket as IPPacket;
                return FlowKey.Create(
                    GetAddressFamily(ip.Version),
                    System.Net.Sockets.ProtocolType.Tcp,
                    ip.SourceAddress.GetAddressBytes(),
                    tcp.SourcePort,
                    ip.DestinationAddress.GetAddressBytes(),
                    tcp.DestinationPort);
            }
            FlowKey GetIpFlowKey(IPPacket ip)
            {
                return FlowKey.Create(
                    GetAddressFamily(ip.Version),
                    (System.Net.Sockets.ProtocolType)ip.Protocol,
                    ip.SourceAddress.GetAddressBytes(),
                    0,
                    ip.DestinationAddress.GetAddressBytes(),
                    0
                );
            }

            if (packet is NullPacket)
            {
                return NullFlowKey;
            }
            switch (packet.Extract<TransportPacket>())
            {
                case UdpPacket udp: return GetUdpFlowKey(udp);
                case TcpPacket tcp: return GetTcpFlowKey(tcp);
                default:
                    switch (packet.Extract<InternetPacket>())
                    {
                        case IPPacket ip: return GetIpFlowKey(ip);
                        default:
                            return NullFlowKey;
                    }
            }
        }
        static FlowKey NullFlowKey => FlowKey.Create(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.ProtocolType.Unknown,
                                IPAddress.None.GetAddressBytes(), 0, IPAddress.None.GetAddressBytes(), 0);
    }
}
