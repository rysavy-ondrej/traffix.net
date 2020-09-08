using Kaitai;
using PacketDotNet;
using System;
using System.Collections.Generic;
using Traffix.Extensions.Decoders.Industrial;

namespace IcsMonitor.Modbus
{
    public class ModbusBiflowProcessor : CustomBiflowProcessor<ModbusFlowData>
    {
        protected override ModbusFlowData Invoke(IReadOnlyCollection<Packet> fwdPackets, IReadOnlyCollection<Packet> revPackets)
        {
            var modbusFlowData = new ModbusFlowData();
            foreach (var packet in fwdPackets)
            {
                var tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket.PayloadData?.Length != 0)
                {
                    var stream = new KaitaiStream(tcpPacket.PayloadData);
                    if (TryParseModbusRequestPacket(stream, out var modbusPacket, out var error))
                    {
                        UpdateRequests(modbusFlowData, modbusPacket);
                    }
                    else
                    {
                        modbusFlowData.MalformedRequests++;
                    }
                }
            }
            foreach (var packet in revPackets)
            {
                var tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket.PayloadData?.Length != 0)
                {
                    var stream = new KaitaiStream(tcpPacket.PayloadData);
                    if (TryParseModbusResponsePacket(stream, out var modbusPacket, out var error))
                    {
                        UpdateResponses(modbusFlowData, modbusPacket);
                    }
                    else
                    {
                        modbusFlowData.MalformedResponses++;
                    }
                }
            }
            return modbusFlowData;
        }

        private static bool TryParseModbusRequestPacket(KaitaiStream stream, out ModbusRequestPacket packet, out Exception exception)
        {
            try
            {
                packet = new ModbusRequestPacket(stream);
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

        private void UpdateResponses(ModbusFlowData responses, ModbusResponsePacket modbusPacket)
        {
            if (modbusPacket.Status == ModbusResponsePacket.ModbusStatusCode.Success)
            {
                switch(modbusPacket.Function)
                {
                    case ModbusResponsePacket.ModbusFunctionCode.Diagnostic:
                        responses.DiagnosticFunctionsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.GetComEventCounter:
                        responses.OtherFunctionsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.GetComEventLog:
                        responses.OtherFunctionsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.MaskWriteRegister:
                        responses.MaskWriteRegisterResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadCoilStatus:
                        responses.ReadCoilsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadDeviceIdentification:
                        responses.OtherFunctionsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadExceptionStatus:
                        responses.OtherFunctionsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadFifoQueue:
                        responses.ReadFifoResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadFileRecord:
                        responses.ReadFileRecordResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadHoldingRegister:
                        responses.ReadHoldingRegistersResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadInputRegisters:
                        responses.ReadInputRegistersResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadInputStatus:
                        responses.ReadDiscreteInputsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadWriteMultiupleRegisters:
                        responses.ReadWriteMultRegistersResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReportSlaveId:
                        responses.OtherFunctionsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteFileRecord:
                        responses.WriteFileRecordResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleCoils:
                        responses.WriteMultCoilsResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleRegisters:
                        responses.WriteMultRegistersResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteSingleCoil:
                        responses.WriteSingleCoilResponsesSuccess++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteSingleRegister:
                        responses.WriteSingleRegisterResponsesSuccess++;
                        break;
                    default:
                        responses.UndefinedFunctionsResponsesSuccess++;
                        break;
                }
            }
            else
            {
                switch (modbusPacket.Function)
                {
                    case ModbusResponsePacket.ModbusFunctionCode.Diagnostic:
                        responses.DiagnosticFunctionsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.GetComEventCounter:
                        responses.OtherFunctionsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.GetComEventLog:
                        responses.OtherFunctionsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.MaskWriteRegister:
                        responses.MaskWriteRegisterResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadCoilStatus:
                        responses.ReadCoilsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadDeviceIdentification:
                        responses.OtherFunctionsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadExceptionStatus:
                        responses.OtherFunctionsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadFifoQueue:
                        responses.ReadFifoResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadFileRecord:
                        responses.ReadFileRecordResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadHoldingRegister:
                        responses.ReadHoldingRegistersResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadInputRegisters:
                        responses.ReadInputRegistersResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadInputStatus:
                        responses.ReadDiscreteInputsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReadWriteMultiupleRegisters:
                        responses.ReadWriteMultRegistersResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.ReportSlaveId:
                        responses.OtherFunctionsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteFileRecord:
                        responses.WriteFileRecordResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleCoils:
                        responses.WriteMultCoilsResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteMultipleRegisters:
                        responses.WriteMultRegistersResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteSingleCoil:
                        responses.WriteSingleCoilResponsesError++;
                        break;
                    case ModbusResponsePacket.ModbusFunctionCode.WriteSingleRegister:
                        responses.WriteSingleRegisterResponsesError++;
                        break;
                    default:
                        responses.UndefinedFunctionsResponsesError++;
                        break;
                }
            }
        }

        private void UpdateRequests(ModbusFlowData requests, ModbusRequestPacket modbusPacket)
        {
            switch (modbusPacket.Function)
            {
                case ModbusRequestPacket.ModbusFunctionCode.Diagnostic:
                    requests.DiagnosticFunctionsRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.GetComEventCounter:
                    requests.OtherFunctionsRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.GetComEventLog:
                    requests.OtherFunctionsRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadDeviceIdentification:
                    requests.OtherFunctionsRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.MaskWriteRegister:
                    requests.MaskWriteRegisterRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadCoilStatus:
                    requests.ReadCoilsRequests++;
                    requests.ReadCoilsBits += (modbusPacket.Data as ModbusRequestPacket.ReadCoilStatusFunction)?.QuantityOfCoils ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadExceptionStatus:
                    requests.OtherFunctionsRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadFifoQueue:
                    requests.ReadFifoRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadFileRecord:
                    requests.ReadFileRecordRequests++;
                    requests.ReadFileRecordCount+=(modbusPacket.Data as ModbusRequestPacket.ReadFileRecordFunction)?.SubRequests.Count ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadHoldingRegister:
                    requests.ReadHoldingRegistersRequests++;
                    requests.ReadHoldingRegistersWords += (modbusPacket.Data as ModbusRequestPacket.ReadHoldingRegistersFunction)?.QuantityOfInputs ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadInputRegisters:
                    requests.ReadInputRegistersRequests++;
                    requests.ReadInputRegistersWords += (modbusPacket.Data as ModbusRequestPacket.ReadInputRegistersFunction)?.QuantityOfInputs ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadInputStatus:
                    requests.ReadDiscreteInputsRequests++;
                    requests.ReadDiscreteInputsBits += (modbusPacket.Data as ModbusRequestPacket.ReadInputStatusFunction)?.QuantityOfInputs ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReadWriteMultiupleRegisters:
                    requests.ReadWriteMultRegistersRequests++;
                    requests.ReadWriteMultRegistersReadWords += (modbusPacket.Data as ModbusRequestPacket.ReadWriteMultiupleRegistersFunction)?.QuantityToRead ?? 0;
                    requests.ReadWriteMultRegistersWriteWords += (modbusPacket.Data as ModbusRequestPacket.ReadWriteMultiupleRegistersFunction)?.QunatityToWrite ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.ReportSlaveId:
                    requests.OtherFunctionsRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.WriteFileRecord:
                    requests.WriteFileRecordRequests++;
                    requests.WriteFileRecordCount += (modbusPacket.Data as ModbusRequestPacket.WriteFileRecordFunction)?.SubRequests.Count ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.WriteMultipleCoils:
                    requests.WriteMultCoilsRequests++;
                    requests.WriteMultCoilsBits += (modbusPacket.Data as ModbusRequestPacket.WriteMultipleCoilsFunction)?.QuantityOfOutputs ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.WriteMultipleRegisters:
                    requests.WriteMultRegistersRequests++;
                    requests.WriteMultRegistersWords += (modbusPacket.Data as ModbusRequestPacket.WriteMultipleRegistersFunction)?.QuantityOfRegisters ?? 0;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.WriteSingleCoil:
                    requests.WriteSingleCoilRequests++;
                    break;
                case ModbusRequestPacket.ModbusFunctionCode.WriteSingleRegister:
                    requests.WriteSingleRegisterRequests++;
                    break;
                default:
                    requests.UndefinedFunctionsRequests++;
                    break;
            }
        }


    }
}
