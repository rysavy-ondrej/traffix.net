using MessagePack;

namespace IcsMonitor.Tests
{
    /// <summary>
    /// A compact version of MODBUS IPFIX record extension.
    /// </summary>
    [MessagePackObject]
    public struct ModbusCompact
    {
        ModbusData _data;
        public ModbusCompact(ModbusData data)
        {
            _data = data;
        }
        [Key("MODBUS_UNIT_ID")]
        public byte UnitId => _data.UnitId;

        [Key("MODBUS_READ_REQUESTS")]
        public int ReadRequests =>
              _data.ReadCoilsRequests
            + _data.ReadDiscreteInputsRequests
            + _data.ReadFifoRequests
            + _data.ReadFileRecordRequests
            + _data.ReadHoldingRegistersRequests
            + _data.ReadInputRegistersRequests;

        [Key("MODBUS_WRITE_REQUESTS")]
        public int WriteRequests =>
              _data.WriteFileRecordRequests
            + _data.WriteMultCoilsRequests
            + _data.WriteMultRegistersRequests
            + _data.WriteSingleCoilRequests
            + _data.WriteSingleRegisterRequests
            + _data.MaskWriteRegisterRequests
            + _data.ReadWriteMultRegistersRequests;

        [Key("MODBUS_DIAGNOSTIC_REQUESTS")]
        public int DiagnosticRequests => _data.DiagnosticFunctionsRequests;

        [Key("MODBUS_OTHER_REQUESTS")]
        public int OtherRequests => _data.OtherFunctionsRequests;

        [Key("MODBUS_UNDEFINED_REQUESTS")]
        public int UndefinedRequests => _data.UndefinedFunctionsRequests;

        [Key("MODBUS_RESPONSES_SUCCESS")]
        public int ResponsesSuccess =>
               _data.DiagnosticFunctionsResponsesSuccess
            + _data.MaskWriteRegisterResponsesSuccess
            + _data.OtherFunctionsResponsesSuccess
            + _data.ReadCoilsResponsesSuccess
            + _data.ReadDiscreteInputsResponsesSuccess
            + _data.ReadFifoResponsesSuccess
            + _data.ReadFileRecordResponsesSuccess
            + _data.ReadHoldingRegistersResponsesSuccess
            + _data.ReadInputRegistersResponsesSuccess
            + _data.ReadWriteMultRegistersResponsesSuccess
            + _data.UndefinedFunctionsResponsesSuccess
            + _data.WriteFileRecordResponsesSuccess
            + _data.WriteMultCoilsResponsesSuccess
            + _data.WriteMultRegistersResponsesSuccess
            + _data.WriteSingleCoilResponsesSuccess
            + _data.WriteSingleRegisterResponsesSuccess;

        [Key("MODBUS_RESPONSES_ERROR")]
        public int ResponsesError =>
              _data.DiagnosticFunctionsResponsesError
            + _data.MaskWriteRegisterResponsesError
            + _data.OtherFunctionsResponsesError
            + _data.ReadCoilsResponsesError
            + _data.ReadDiscreteInputsResponsesError
            + _data.ReadFifoResponsesError
            + _data.ReadFileRecordResponsesError
            + _data.ReadHoldingRegistersResponsesError
            + _data.ReadInputRegistersResponsesError
            + _data.ReadWriteMultRegistersResponsesError
            + _data.UndefinedFunctionsResponsesError
            + _data.WriteFileRecordResponsesError
            + _data.WriteMultCoilsResponsesError
            + _data.WriteMultRegistersResponsesError
            + _data.WriteSingleCoilResponsesError
            + _data.WriteSingleRegisterResponsesError;

        [Key("MODBUS_MALFORMED_REQUESTS")]
        public int MalformedRequests => _data.MalformedRequests;

        [Key("MODBUS_MALFORMED_RESPONSES")]
        public int MalformedResponses => _data.MalformedResponses;
    }
}