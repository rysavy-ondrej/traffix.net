using PacketDotNet;
using System.Collections.Generic;
using Traffix.Core.Flows;

namespace IcsMonitor.Modbus
{
    public class ModbusFlowFilter
    {
        public bool Invoke(FlowKey flowKey, IReadOnlyCollection<Packet> frames)
        {
            return flowKey.ProtocolType == System.Net.Sockets.ProtocolType.Tcp &&
                (flowKey.SourcePort == 502 || flowKey.DestinationPort == 502);
        }
    }
}
