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
                    dnp3FlowData.Confirmations++;
                    break;
                case Dnp3Packet.Dnp3FunctionCode.Dnp3Response:
                    dnp3FlowData.Responses++;
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
                dnp3FlowData.AllStationsFlag += internalIndication.AllStations ? 1 : 0;
                dnp3FlowData.AlreadyExecutingFlag += internalIndication.AlreadyExecuting ? 1 : 0;
                dnp3FlowData.Class1EventFlag += internalIndication.Class1Event ? 1 : 0;
                dnp3FlowData.Class2EventFlag += internalIndication.Class2Event ? 1 : 0;
                dnp3FlowData.Class3EventFlag += internalIndication.Class3Event ? 1 : 0;
                dnp3FlowData.ConfigurationCorruptFlag += internalIndication.ConfigurationCorrupt ? 1 : 0;
                dnp3FlowData.DeviceRestartFlag += internalIndication.DeviceRestart ? 1 : 0;
                dnp3FlowData.DeviceTroubleFlag += internalIndication.DeviceTrouble ? 1 : 0;
                dnp3FlowData.EventBufferOverflowFlag += internalIndication.EventBufferOverflow ? 1 : 0;
                dnp3FlowData.FunctionNotSupportedFlag += internalIndication.FunctionNotSupported ? 1 : 0;
                dnp3FlowData.LocalControlFlag += internalIndication.LocalControl ? 1 : 0;
                dnp3FlowData.NeedTimeFlag += internalIndication.NeedTime ? 1 : 0;
                dnp3FlowData.ObjectUnknownFlag += internalIndication.ObjectUnknown ? 1 : 0;
                dnp3FlowData.ParameterErrorFlag += internalIndication.ParameterError ? 1 : 0;
            }
        }

        private void UpdateRequestFlowData(Dnp3FlowData dnp3FlowData, Dnp3Packet dnp3Packet)
        {
            switch(dnp3Packet.FirstChunk?.Application.FunctionCode)
            {
                case Dnp3Packet.Dnp3FunctionCode.Dnp3Confirm:
                    dnp3FlowData.Confirmations++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Read:
                    dnp3FlowData.ReadRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Write:
                    dnp3FlowData.WriteRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Select:
                    dnp3FlowData.SelectRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3DirOperate:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3DirOperateNoResp:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3Operate:
                    dnp3FlowData.OperateRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3Freeze:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeAtTime:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeAtTimeNoResp:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeClearNoResp:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeNoResp:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3FreezeClear:
                    dnp3FlowData.FreezeRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3ColdRestart:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3WarmRestart:
                    dnp3FlowData.RestartRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3InitializeApplication:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3InitializeData:
                    dnp3FlowData.InitializeRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3StartApplication:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3StopApplication:
                    dnp3FlowData.ApplicationOperationRequests++;
                    break;

                case Dnp3Packet.Dnp3FunctionCode.Dnp3AbortFile:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3AuthenticateFile:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3CloseFile:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3DeleteFile:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3GetFileInformation:
                case Dnp3Packet.Dnp3FunctionCode.Dnp3OpenFile:
                    dnp3FlowData.FileOperationRequests++;
                    break;

                default:
                    dnp3FlowData.OtherOperationRequests++;
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
