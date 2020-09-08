using Kaitai;
using PacketDotNet;
using System;
using System.Collections.Generic;
using Traffix.Extensions.Decoders.Industrial;

namespace IcsMonitor.Modbus
{
    public class Dnp3BiflowProcessor : CustomBiflowProcessor<Dnp3FlowData>
    {
        protected override Dnp3FlowData Invoke(IReadOnlyCollection<Packet> fwdPackets, IReadOnlyCollection<Packet> revPackets)
        {
            var dnp3FlowData = new Dnp3FlowData();
            foreach (var packet in fwdPackets)
            {
                var tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket.PayloadData?.Length != 0)
                {
                    var stream = new KaitaiStream(tcpPacket.PayloadData);
                    if (TryParseDnp3Packet(stream, out var dnp3Packet, out _))
                    {
                        UpdateFlowData(dnp3FlowData, dnp3Packet);
                    }
                    else
                    {
                        dnp3FlowData.MalformedRequests++;
                    }
                }
            }
            foreach (var packet in revPackets)
            {
                var tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket.PayloadData?.Length != 0)
                {
                    var stream = new KaitaiStream(tcpPacket.PayloadData);
                    if (TryParseDnp3Packet(stream, out var dnp3Packet, out _))
                    {
                        UpdateFlowData(dnp3FlowData, dnp3Packet);
                    }
                    else
                    {
                        dnp3FlowData.MalformedResponses++;
                    }
                }
            }
            return dnp3FlowData;
        }

        private void UpdateFlowData(Dnp3FlowData dnp3FlowData, Dnp3Packet dnp3Packet)
        {
            throw new NotImplementedException();
        }

        private static bool TryParseDnp3Packet(KaitaiStream stream, out Dnp3Packet packet, out Exception exception)
        {
            try
            {
                packet = new Dnp3Packet(stream);
                exception = null;
                return true;
            }
            catch(Exception e)
            {
                packet = null;
                exception = e;
                return false;
            }
        }



    }
}
