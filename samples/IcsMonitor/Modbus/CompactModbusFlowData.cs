using MessagePack;

namespace IcsMonitor.Modbus
{
    /// <summary>
    /// A MODBUS flow extension that can be extracted from the simple MODBUS parser (parsing individual functions is not necessary).
    /// </summary>
    [MessagePackObject]
    public class ExtendedModbusFlowData
    {
        #region REQUESTS
        [Key("MODBUS_UNIT_ID")]
        public byte UnitId;
        [Key("MODBUS_READ_COILS_REQUESTS")]
        public int ReadCoilsRequests;
        [Key("MODBUS_READ_DISCRETE_INPUTS_REQUESTS")]
        public int ReadDiscreteInputsRequests;
        [Key("MODBUS_READ_INPUT_REGISTERS_REQUESTS")]
        public int ReadInputRegistersRequests;
        [Key("MODBUS_READ_HOLDING_REGISTERS_REQUESTS")]
        public int ReadHoldingRegistersRequests;
        [Key("MODBUS_WRITE_SINGLE_COIL_REQUESTS")]
        public int WriteSingleCoilRequests;
        [Key("MODBUS_WRITE_SINGLE_REGISTER_REQUESTS")]
        public int WriteSingleRegisterRequests;
        [Key("MODBUS_WRITE_MULT_COILS_REQUESTS")]
        public int WriteMultCoilsRequests;
        [Key("MODBUS_WRITE_MULT_REGISTERS_REQUESTS")]
        public int WriteMultRegistersRequests;
        [Key("MODBUS_READ_FILE_RECORD_REQUESTS")]
        public int ReadFileRecordRequests;
        [Key("MODBUS_WRITE_FILE_RECORD_REQUESTS")]
        public int WriteFileRecordRequests;
        [Key("MODBUS_MASK_WRITE_REGISTER_REQUESTS")]
        public int MaskWriteRegisterRequests;
        [Key("MODBUS_READ_WRITE_MULT_REGISTERS_REQUESTS")]
        public int ReadWriteMultRegistersRequests;
        [Key("MODBUS_READ_FIFO_REQUESTS")]
        public int ReadFifoRequests;
        [Key("MODBUS_DIAGNOSTIC_REQUESTS")]
        public int DiagnosticFunctionsRequests;
        [Key("MODBUS_OTHER_REQUESTS")]
        public int OtherFunctionsRequests;
        [Key("MODBUS_UNDEFINED_REQUESTS")]
        public int UndefinedFunctionsRequests;
        #endregion 

        #region RESPONSES - SUCCESS
        [Key("MODBUS_READ_COILS_RESPONSES_SUCCESS")]
        public int ReadCoilsResponsesSuccess;

        [Key("MODBUS_READ_DISCRETE_INPUTS_RESPONSES_SUCCESS")]
        public int ReadDiscreteInputsResponsesSuccess;

        [Key("MODBUS_READ_INPUT_REGISTERS_RESPONSES_SUCCESS")]
        public int ReadInputRegistersResponsesSuccess;

        [Key("MODBUS_READ_HOLDING_REGISTERS_RESPONSES_SUCCESS")]
        public int ReadHoldingRegistersResponsesSuccess;

        [Key("MODBUS_WRITE_SINGLE_COIL_RESPONSES_SUCCESS")]
        public int WriteSingleCoilResponsesSuccess;

        [Key("MODBUS_WRITE_SINGLE_REGISTER_RESPONSES_SUCCESS")]
        public int WriteSingleRegisterResponsesSuccess;

        [Key("MODBUS_WRITE_MULT_COILS_RESPONSES_SUCCESS")]
        public int WriteMultCoilsResponsesSuccess;

        [Key("MODBUS_WRITE_MULT_REGISTERS_RESPONSES_SUCCESS")]
        public int WriteMultRegistersResponsesSuccess;

        [Key("MODBUS_READ_FILE_RECORD_RESPONSES_SUCCESS")]
        public int ReadFileRecordResponsesSuccess;

        [Key("MODBUS_WRITE_FILE_RECORD_RESPONSES_SUCCESS")]
        public int WriteFileRecordResponsesSuccess;

        [Key("MODBUS_MASK_WRITE_REGISTER_RESPONSES_SUCCESS")]
        public int MaskWriteRegisterResponsesSuccess;

        [Key("MODBUS_READ_WRITE_MULT_REGISTERS_RESPONSES_SUCCESS")]
        public int ReadWriteMultRegistersResponsesSuccess;

        [Key("MODBUS_READ_FIFO_RESPONSES_SUCCESS")]
        public int ReadFifoResponsesSuccess;

        [Key("MODBUS_DIAGNOSTIC_RESPONSES_SUCCESS")]
        public int DiagnosticFunctionsResponsesSuccess;

        [Key("MODBUS_OTHER_RESPONSES_SUCCESS")]
        public int OtherFunctionsResponsesSuccess;

        [Key("MODBUS_UNDEFINED_RESPONSES_SUCCESS")]
        public int UndefinedFunctionsResponsesSuccess;

        #endregion

        #region RESPONSES - ERROR

        [Key("MODBUS_READ_COILS_RESPONSES_ERROR")]
        public int ReadCoilsResponsesError;

        [Key("MODBUS_READ_DISCRETE_INPUTS_RESPONSES_ERROR")]
        public int ReadDiscreteInputsResponsesError;

        [Key("MODBUS_READ_INPUT_REGISTERS_RESPONSES_ERROR")]
        public int ReadInputRegistersResponsesError;

        [Key("MODBUS_READ_HOLDING_REGISTERS_RESPONSES_ERROR")]
        public int ReadHoldingRegistersResponsesError;

        [Key("MODBUS_WRITE_SINGLE_COIL_RESPONSES_ERROR")]
        public int WriteSingleCoilResponsesError;

        [Key("MODBUS_WRITE_SINGLE_REGISTER_RESPONSES_ERROR")]
        public int WriteSingleRegisterResponsesError;

        [Key("MODBUS_WRITE_MULT_COILS_RESPONSES_ERROR")]
        public int WriteMultCoilsResponsesError;

        [Key("MODBUS_WRITE_MULT_REGISTERS_RESPONSES_ERROR")]
        public int WriteMultRegistersResponsesError;

        [Key("MODBUS_READ_FILE_RECORD_RESPONSES_ERROR")]
        public int ReadFileRecordResponsesError;

        [Key("MODBUS_WRITE_FILE_RECORD_RESPONSES_ERROR")]
        public int WriteFileRecordResponsesError;

        [Key("MODBUS_MASK_WRITE_REGISTER_RESPONSES_ERROR")]
        public int MaskWriteRegisterResponsesError;

        [Key("MODBUS_READ_WRITE_MULT_REGISTERS_RESPONSES_ERROR")]
        public int ReadWriteMultRegistersResponsesError;

        [Key("MODBUS_READ_FIFO_RESPONSES_ERROR")]
        public int ReadFifoResponsesError;

        [Key("MODBUS_DIAGNOSTIC_RESPONSES_ERROR")]
        public int DiagnosticFunctionsResponsesError;

        [Key("MODBUS_OTHER_RESPONSES_ERROR")]
        public int OtherFunctionsResponsesError;

        [Key("MODBUS_UNDEFINED_RESPONSES_ERROR")]
        public int UndefinedFunctionsResponsesError;
        #endregion

        [Key("MODBUS_MALFORMED_REQUESTS")]
        public int MalformedRequests;

        [Key("MODBUS_MALFORMED_RESPONSES")]
        public int MalformedResponses;
    }

    /// <summary>
    /// A MODBUS flow extension that can be extracted from the simple MODBUS parser (parsing individual functions is not necessary).
    /// </summary>
    [MessagePackObject]
    public class CompactModbusFlowData
    {
        [Key("MODBUS_UNIT_ID")]
        public byte UnitId;

        [Key("MODBUS_READ_REQUESTS")]
        public int ReadRequests;

        [Key("MODBUS_WRITE_REQUESTS")]
        public int WriteRequests;

        [Key("MODBUS_DIAGNOSTIC_REQUESTS")]
        public int DiagnosticRequests;

        [Key("MODBUS_OTHER_REQUESTS")]
        public int OtherRequests;

        [Key("MODBUS_UNDEFINED_REQUESTS")]
        public int UndefinedRequests;

        [Key("MODBUS_RESPONSES_SUCCESS")]
        public int ResponsesSuccess;

        [Key("MODBUS_RESPONSES_ERROR")]
        public int ResponsesError;

        [Key("MODBUS_MALFORMED_REQUESTS")]
        public int MalformedRequests;

        [Key("MODBUS_MALFORMED_RESPONSES")]
        public int MalformedResponses;
    }
}