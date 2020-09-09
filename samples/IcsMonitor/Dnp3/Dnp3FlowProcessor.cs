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
                        UpdateRequestFlowData(dnp3FlowData, dnp3Packet);
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
                        UpdateResponseFlowData(dnp3FlowData, dnp3Packet);
                    }
                    else
                    {
                        dnp3FlowData.MalformedResponses++;
                    }
                }
            }
            return dnp3FlowData;
        }

        private void UpdateResponseFlowData(Dnp3FlowData dnp3FlowData, Dnp3Packet dnp3Packet)
        {
            switch(dnp3Packet.FirstChunk?.Application.FunctionCode)
            {
                case Dnp3Packet.Dnp3FunctionCode.Dnp3Confirm:
                    dnp3FlowData.ConfirmationResponses++;
                    break; 
                case Dnp3Packet.Dnp3FunctionCode.Dnp3Response:
                    dnp3FlowData.RegularResponses++;
                    break;
                case Dnp3Packet.Dnp3FunctionCode.Dnp3UnsolicitedResponse:
                    dnp3FlowData.UnsolicitedResponses++;
                    break;
                default:
                    dnp3FlowData.OtherResponses++;
                    break;
            }
            var internalIndication = dnp3Packet.FirstChunk?.Application.InternalIndication;
            if (internalIndication != null)
            {
                dnp3FlowData.ConfigurationCorruptCount += internalIndication.ConfigurationCorrupt ? 1 : 0;
                dnp3FlowData.DeviceRestartCount += internalIndication.DeviceRestart ? 1 : 0;
                dnp3FlowData.DeviceTroubleCount += internalIndication.DeviceTrouble ? 1 : 0;
                dnp3FlowData.EventBufferOverflowCount += internalIndication.EventBufferOverflow ? 1 : 0;
                dnp3FlowData.FunctionNotSupportedCount += internalIndication.FunctionNotSupported ? 1 : 0;
                dnp3FlowData.ObjectUnknownCount += internalIndication.ObjectUnknown ? 1 : 0;
                dnp3FlowData.ParameterErrorCount += internalIndication.ParameterError ? 1 : 0;
            }
        }

        private void UpdateRequestFlowData(Dnp3FlowData dnp3FlowData, Dnp3Packet dnp3Packet)
        {
            var fc = dnp3Packet.FirstChunk?.Application.FunctionCode;
            switch (fc)
            {
                case Dnp3Packet.Dnp3FunctionCode.Dnp3Confirm:
                    dnp3FlowData.ConfirmationRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Read:
                    dnp3FlowData.ReadRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Write:
                    dnp3FlowData.WriteRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Select:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3Operate:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3DirOperate:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3DirOperateNoResp:
                    dnp3FlowData.ControlRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Freeze:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeAtTime:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeAtTimeNoResp:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeClearNoResp:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeNoResp:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeClear:
                    dnp3FlowData.FreezeRequests++;
                    break;
                case Dnp3Packet.Dnp3FunctionCode.Dnp3DelayMeasurement:
                    dnp3FlowData.TimeSynchronizationRequests++;
                    break;
                case Dnp3Packet.Dnp3FunctionCode.Dnp3ColdRestart:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3WarmRestart:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3InitializeApplication:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3InitializeData:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3StartApplication:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3StopApplication:
                    dnp3FlowData.ApplicationControlRequests++;
                    break;
                default:
                    if ((int?)fc >= 24 && (int?)fc <= 128)
                    {
                        dnp3FlowData.ReservedRequests++;
                    }
                    else
                    {
                        dnp3FlowData.OtherOperationRequests++;
                    }
                    break;
            }
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
