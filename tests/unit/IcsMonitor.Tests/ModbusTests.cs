using Kaitai;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketDotNet;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Traffix.Core.Flows;
using Traffix.Core.Observable;
using Traffix.Extensions.Decoders.Industrial;
using Traffix.Processors;
using Traffix.Providers.PcapFile;

namespace IcsMonitor.Tests
{
    [TestClass]
    public class ModbusTests
    {
        const int ModbusPort = 502;
        [TestMethod]
        public async Task TestPrepareModbusData()
        {
            // load from file
            var observable = SharpPcapReader.CreateObservable(@"data\PCAP\run1_3rtu_2s.cap").Select(PacketRecord.FromFrame).Where(p=>p.Key.SourcePort == ModbusPort || p.Key.DestinationPort == ModbusPort);
            var windows = observable.TimeSpanWindow(t => t.Ticks, TimeSpan.FromMinutes(30));
            
            // apply modbus flow processor
            await windows.Select((packets, index) => (packets,index)).ForEachAsync(async window =>
            {
                var totalPackets = 0;
                var flowProcessor = new ModbusFlowProcessor();
                await window.packets.Do(_=>totalPackets++).ForEachAsync(p => flowProcessor.OnNext(p.Key, p));
                Console.WriteLine($"# Window {window.index} []");
                Console.WriteLine($"Flows = {flowProcessor.Count},  Packets = {totalPackets}");
                Console.WriteLine();
                Console.WriteLine("| Date first seen | Duration | Proto | Src IP Addr:Port | Dst IP Addr:Port | Packets | Bytes | UnitId | ReadRequests | WriteRequests | DiagnosticRequests | OtherRequests | UndefinedRequests | ResponsesSuccess | ResponsesError | MalformedRequests | MalformedResponses |");
                Console.WriteLine("| --------------- | -------- | ----- | ---------------- | ---------------- | ------- | ----- | ------ | ------------ | ------------- | ------------------ | ------------- | ----------------- | ---------------- | -------------- | ----------------- | ------------------ |");
                foreach (var flow in flowProcessor.AggregateFlows(f => (f.ProtocolType, f.SourceIpAddress, f.DestinationIpAddress) ))
                {
                    Console.WriteLine($"| {flow.Value.Metrics.Start} |  {flow.Value.Metrics.Duration} | {flow.Key.ProtocolType} | {flow.Key.SourceIpAddress} | {flow.Key.DestinationIpAddress} | {flow.Value.Metrics.Packets} | {flow.Value.Metrics.Octets} | {flow.Value.Compact.UnitId} | {flow.Value.Compact.ReadRequests} | {flow.Value.Compact.WriteRequests} | {flow.Value.Compact.DiagnosticRequests} | {flow.Value.Compact.OtherRequests} | {flow.Value.Compact.UndefinedRequests} | {flow.Value.Compact.ResponsesSuccess} | {flow.Value.Compact.ResponsesError} | {flow.Value.Compact.MalformedRequests} | {flow.Value.Compact.MalformedResponses} |");
                }
                Console.WriteLine();
            });
            // export ml.net dataframes

        }
    }
    class PacketRecord
    {
        public long Ticks;
        public FlowKey Key;
        public Packet Packet;

        public static PacketRecord FromFrame(RawFrame arg)
        {
            var packet = Packet.ParsePacket(arg.LinkLayer, arg.Data);
            return new PacketRecord { Ticks = arg.Ticks, Packet = packet, Key = packet.GetFlowKey() };
        }
    }
    class ModbusFlowRecord
    {
        public FlowMetrics Metrics;
        public ModbusData Modbus;
        public ModbusCompact Compact => new ModbusCompact(Modbus); 
    }                           

    class ModbusFlowProcessor : FlowProcessor<PacketRecord, FlowKey, ModbusFlowRecord>
    {
        public ModbusFlowProcessor() : base(Create, Update, Aggregate) {   }

        private static ModbusFlowRecord Aggregate(ModbusFlowRecord arg1, ModbusFlowRecord arg2)
        {
            return new ModbusFlowRecord
            {
                Metrics = FlowMetrics.Combine(ref arg1.Metrics, ref arg2.Metrics),
                Modbus = ModbusData.Combine(ref arg1.Modbus, ref arg2.Modbus)
            };
        }

        private static void Update(ModbusFlowRecord arg1, PacketRecord arg2)
        {
            if (arg2.Key.SourcePort > arg2.Key.DestinationPort)
            {
                UpdateRequest(ref arg1.Modbus, arg2);
            }
            else
            {
                UpdateResponse(ref arg1.Modbus, arg2);
            }
        }

        private static ModbusFlowRecord Create(PacketRecord arg)
        {
            var record = new ModbusFlowRecord();
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
    }
}
