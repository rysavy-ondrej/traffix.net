using Kaitai;
using PacketDotNet;
using System;
using Traffix.Core.Flows;
using Traffix.Core.Observable;
using Traffix.Extensions.Decoders.Industrial;
using Traffix.Processors;
using Traffix.Storage.Faster;

namespace IcsMonitor.Tests
{
    class ModbusFlowProcessor : FlowProcessor<PacketRecord, FlowKey, ModbusFlowRecord>
    {
        protected override ModbusFlowRecord Aggregate(ModbusFlowRecord arg1, ModbusFlowRecord arg2)
        {
            return new ModbusFlowRecord
            {
                FlowKey = arg1.FlowKey.SourcePort > arg2.FlowKey.SourcePort ? arg1.FlowKey : arg2.FlowKey,
                Metrics = FlowMetrics.Aggregate(ref arg1.Metrics, ref arg2.Metrics),
                Modbus = ModbusData.Aggregate(ref arg1.Modbus, ref arg2.Modbus)
            };
        }

        protected override void Update(ModbusFlowRecord arg1, PacketRecord arg2)
        {
            arg1.Metrics.Packets++;
            arg1.Metrics.Octets += arg2.Packet.TotalPacketLength;
            arg1.Metrics.Start = new DateTime(Math.Min(arg1.Metrics.Start.Ticks, arg2.Ticks));
            arg1.Metrics.End = new DateTime(Math.Max(arg1.Metrics.Start.Ticks, arg2.Ticks));
            if (arg2.Key.SourcePort > arg2.Key.DestinationPort)
            {
                UpdateRequest(ref arg1.Modbus, arg2);
            }
            else
            {
                UpdateResponse(ref arg1.Modbus, arg2);
            }
        }

        protected override ModbusFlowRecord Create(PacketRecord arg)
        {
            var record = new ModbusFlowRecord();
            record.FlowKey = arg.Key;
            record.Metrics.Packets = 1;
            record.Metrics.Octets = arg.Packet.TotalPacketLength;
            record.Metrics.Start = new DateTime(arg.Ticks);
            record.Metrics.End = new DateTime(arg.Ticks);
            if (arg.Key.SourcePort > arg.Key.DestinationPort)
            {
                UpdateRequest(ref record.Modbus, arg);
            }
            else
            {
                UpdateResponse(ref record.Modbus, arg);
            }
            return record;
        }
       
        private static void UpdateRequest(ref ModbusData request, PacketRecord arg)
        { 
            var tcpPacket = arg.Packet.Extract<TcpPacket>();
            if (tcpPacket?.PayloadData?.Length >= 8)
            {
                var stream = new KaitaiStream(tcpPacket.PayloadData);
                if (TryParseModbusRequestPacket(stream, out var modbusPacket, out var error))
                {
                    switch (modbusPacket.Function)
                    {
                        case ModbusRequestPacket.ModbusFunctionCode.Diagnostic:
                            request.DiagnosticFunctionsRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.GetComEventCounter:
                            request.OtherFunctionsRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.GetComEventLog:
                            request.OtherFunctionsRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadDeviceIdentification:
                            request.OtherFunctionsRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.MaskWriteRegister:
                            request.MaskWriteRegisterRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadCoilStatus:
                            request.ReadCoilsRequests++;
                            request.ReadCoilsBits += (modbusPacket.Data as ModbusRequestPacket.ReadCoilStatusFunction)?.QuantityOfCoils ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadExceptionStatus:
                            request.OtherFunctionsRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadFifoQueue:
                            request.ReadFifoRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadFileRecord:
                            request.ReadFileRecordRequests++;
                            request.ReadFileRecordCount += (modbusPacket.Data as ModbusRequestPacket.ReadFileRecordFunction)?.SubRequests.Count ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadHoldingRegister:
                            request.ReadHoldingRegistersRequests++;
                            request.ReadHoldingRegistersWords += (modbusPacket.Data as ModbusRequestPacket.ReadHoldingRegistersFunction)?.QuantityOfInputs ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadInputRegisters:
                            request.ReadInputRegistersRequests++;
                            request.ReadInputRegistersWords += (modbusPacket.Data as ModbusRequestPacket.ReadInputRegistersFunction)?.QuantityOfInputs ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadInputStatus:
                            request.ReadDiscreteInputsRequests++;
                            request.ReadDiscreteInputsBits += (modbusPacket.Data as ModbusRequestPacket.ReadInputStatusFunction)?.QuantityOfInputs ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReadWriteMultiupleRegisters:
                            request.ReadWriteMultRegistersRequests++;
                            request.ReadWriteMultRegistersReadWords += (modbusPacket.Data as ModbusRequestPacket.ReadWriteMultiupleRegistersFunction)?.QuantityToRead ?? 0;
                            request.ReadWriteMultRegistersWriteWords += (modbusPacket.Data as ModbusRequestPacket.ReadWriteMultiupleRegistersFunction)?.QunatityToWrite ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.ReportSlaveId:
                            request.OtherFunctionsRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.WriteFileRecord:
                            request.WriteFileRecordRequests++;
                            request.WriteFileRecordCount += (modbusPacket.Data as ModbusRequestPacket.WriteFileRecordFunction)?.SubRequests.Count ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.WriteMultipleCoils:
                            request.WriteMultCoilsRequests++;
                            request.WriteMultCoilsBits += (modbusPacket.Data as ModbusRequestPacket.WriteMultipleCoilsFunction)?.QuantityOfOutputs ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.WriteMultipleRegisters:
                            request.WriteMultRegistersRequests++;
                            request.WriteMultRegistersWords += (modbusPacket.Data as ModbusRequestPacket.WriteMultipleRegistersFunction)?.QuantityOfRegisters ?? 0;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.WriteSingleCoil:
                            request.WriteSingleCoilRequests++;
                            break;
                        case ModbusRequestPacket.ModbusFunctionCode.WriteSingleRegister:
                            request.WriteSingleRegisterRequests++;
                            break;
                        default:
                            request.UndefinedFunctionsRequests++;
                            break;
                    }
                }
                else
                {
                    request.MalformedRequests++;
                }
            }
        }
        private static void UpdateResponse(ref ModbusData response, PacketRecord arg)
        {
            var tcpPacket = arg.Packet.Extract<TcpPacket>();
            if (tcpPacket?.PayloadData?.Length >= 8)
            {
                var stream = new KaitaiStream(tcpPacket.PayloadData);
                if (TryParseModbusResponsePacket(stream, out var modbusPacket, out var error))
                {
                    if (modbusPacket.Status == ModbusResponsePacket.ModbusStatusCode.Success)
                    {
                        switch (modbusPacket.Function)
                        {
                            case ModbusResponsePacket.ModbusFunctionCode.Diagnostic:
                                response.DiagnosticFunctionsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.GetComEventCounter:
                                response.OtherFunctionsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.GetComEventLog:
                                response.OtherFunctionsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.MaskWriteRegister:
                                response.MaskWriteRegisterResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadCoilStatus:
                                response.ReadCoilsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadDeviceIdentification:
                                response.OtherFunctionsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadExceptionStatus:
                                response.OtherFunctionsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadFifoQueue:
                                response.ReadFifoResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadFileRecord:
                                response.ReadFileRecordResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadHoldingRegister:
                                response.ReadHoldingRegistersResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadInputRegisters:
                                response.ReadInputRegistersResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadInputStatus:
                                response.ReadDiscreteInputsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadWriteMultiupleRegisters:
                                response.ReadWriteMultRegistersResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReportSlaveId:
                                response.OtherFunctionsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteFileRecord:
                                response.WriteFileRecordResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleCoils:
                                response.WriteMultCoilsResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleRegisters:
                                response.WriteMultRegistersResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteSingleCoil:
                                response.WriteSingleCoilResponsesSuccess++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteSingleRegister:
                                response.WriteSingleRegisterResponsesSuccess++;
                                break;
                            default:
                                response.UndefinedFunctionsResponsesSuccess++;
                                break;
                        }
                    }
                    else
                    {
                        switch (modbusPacket.Function)
                        {
                            case ModbusResponsePacket.ModbusFunctionCode.Diagnostic:
                                response.DiagnosticFunctionsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.GetComEventCounter:
                                response.OtherFunctionsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.GetComEventLog:
                                response.OtherFunctionsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.MaskWriteRegister:
                                response.MaskWriteRegisterResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadCoilStatus:
                                response.ReadCoilsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadDeviceIdentification:
                                response.OtherFunctionsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadExceptionStatus:
                                response.OtherFunctionsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadFifoQueue:
                                response.ReadFifoResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadFileRecord:
                                response.ReadFileRecordResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadHoldingRegister:
                                response.ReadHoldingRegistersResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadInputRegisters:
                                response.ReadInputRegistersResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadInputStatus:
                                response.ReadDiscreteInputsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReadWriteMultiupleRegisters:
                                response.ReadWriteMultRegistersResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.ReportSlaveId:
                                response.OtherFunctionsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteFileRecord:
                                response.WriteFileRecordResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleCoils:
                                response.WriteMultCoilsResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleRegisters:
                                response.WriteMultRegistersResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteSingleCoil:
                                response.WriteSingleCoilResponsesError++;
                                break;
                            case ModbusResponsePacket.ModbusFunctionCode.WriteSingleRegister:
                                response.WriteSingleRegisterResponsesError++;
                                break;
                            default:
                                response.UndefinedFunctionsResponsesError++;
                                break;
                        }
                    }
                }
            }
        }
        private static bool TryParseModbusRequestPacket(KaitaiStream stream, out ModbusRequestPacket packet, out Exception exception)
        {
            try
            {
                packet = new ModbusRequestPacket(stream);
                exception = null;
                return true;
            }
            catch (Exception e)
            {
                packet = null;
                exception = e;
                return false;
            }
        }
        private static bool TryParseModbusResponsePacket(KaitaiStream stream, out ModbusResponsePacket packet, out Exception exception)
        {
            try
            {
                packet = new ModbusResponsePacket(stream);
                exception = null;
                return true;
            }
            catch (Exception e)
            {
                packet = null;
                exception = e;
                return false;
            }
        }

        protected override FlowKey GetFlowKey(PacketRecord source)
        {
            return source.Key;
        }

        public static ConversationKey GetConversationKey(FlowKey flowKey)
        { 
            if (flowKey.SourcePort > flowKey.DestinationPort)
            {
                return new ConversationKey(flowKey);
            }
            else
            {
                return new ConversationKey(flowKey.Reverse());
            }
        }
    }
}
