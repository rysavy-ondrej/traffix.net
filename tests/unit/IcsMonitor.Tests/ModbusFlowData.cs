using MessagePack;

namespace IcsMonitor.Tests
{
    [MessagePackObject]
    public struct ModbusData
    {
        #region REQUESTS
        [Key("MODBUS_UNIT_ID")]
        public byte UnitId;
        [Key("MODBUS_READ_COILS_REQUESTS")]
        public int ReadCoilsRequests;
        [Key("MODBUS_READ_COILS_BITS")]
        public int ReadCoilsBits;
        [Key("MODBUS_READ_DISCRETE_INPUTS_REQUESTS")]
        public int ReadDiscreteInputsRequests;
        [Key("MODBUS_READ_DISCRETE_INPUTS_BITS")]
        public int ReadDiscreteInputsBits;
        [Key("MODBUS_READ_INPUT_REGISTERS_REQUESTS")]
        public int ReadInputRegistersRequests;
        [Key("MODBUS_READ_INPUT_REGISTERS_WORDS")]
        public int ReadInputRegistersWords;
        [Key("MODBUS_READ_HOLDING_REGISTERS_REQUESTS")]
        public int ReadHoldingRegistersRequests;
        [Key("MODBUS_READ_HOLDING_REGISTERS_WORDS")]
        public int ReadHoldingRegistersWords;
        [Key("MODBUS_WRITE_SINGLE_COIL_REQUESTS")]
        public int WriteSingleCoilRequests;
        [Key("MODBUS_WRITE_SINGLE_REGISTER_REQUESTS")]
        public int WriteSingleRegisterRequests;
        [Key("MODBUS_WRITE_MULT_COILS_REQUESTS")]
        public int WriteMultCoilsRequests;
        [Key("MODBUS_WRITE_MULT_COILS_BITS")]
        public int WriteMultCoilsBits;
        [Key("MODBUS_WRITE_MULT_REGISTERS_REQUESTS")]
        public int WriteMultRegistersRequests;
        [Key("MODBUS_WRITE_MULT_REGISTERS_WORDS")]
        public int WriteMultRegistersWords;
        [Key("MODBUS_READ_FILE_RECORD_REQUESTS")]
        public int ReadFileRecordRequests;
        [Key("MODBUS_READ_FILE_RECORD_COUNT")]
        public int ReadFileRecordCount;
        [Key("MODBUS_WRITE_FILE_RECORD_REQUESTS")]
        public int WriteFileRecordRequests;
        [Key("MODBUS_WRTIE_FILE_RECORD_COUNT")]
        public int WriteFileRecordCount;
        [Key("MODBUS_MASK_WRITE_REGISTER_REQUESTS")]
        public int MaskWriteRegisterRequests;
        [Key("MODBUS_READ_WRITE_MULT_REGISTERS_REQUESTS")]
        public int ReadWriteMultRegistersRequests;
        [Key("MODBUS_READ_WRITE_MULT_REGISTERS_READ_WORDS")]
        public int ReadWriteMultRegistersReadWords;
        [Key("MODBUS_READ_WRITE_MULT_REGISTERS_WRITE_WORDS")]
        public int ReadWriteMultRegistersWriteWords;
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

        #region Malformed messages

        [Key("MODBUS_MALFORMED_REQUESTS")]
        public int MalformedRequests;

        [Key("MODBUS_MALFORMED_RESPONSES")]
        public int MalformedResponses;

        #endregion

        #region Combinator function
        public static ModbusData Combine(ref ModbusData x, ref ModbusData y)
        {
            return new ModbusData
            {
                UnitId = x.UnitId,
                DiagnosticFunctionsRequests = x.DiagnosticFunctionsRequests + y.DiagnosticFunctionsRequests,
                DiagnosticFunctionsResponsesError = x.DiagnosticFunctionsResponsesError + y.DiagnosticFunctionsResponsesError,
                DiagnosticFunctionsResponsesSuccess = x.DiagnosticFunctionsResponsesSuccess + y.DiagnosticFunctionsResponsesSuccess,
                MalformedRequests = x.MalformedRequests + y.MalformedRequests,
                MalformedResponses = x.MalformedResponses + y.MalformedResponses,
                MaskWriteRegisterRequests = x.MaskWriteRegisterRequests + y.MaskWriteRegisterRequests,
                MaskWriteRegisterResponsesError = x.MaskWriteRegisterResponsesError + y.MaskWriteRegisterResponsesError,
                MaskWriteRegisterResponsesSuccess = x.MaskWriteRegisterResponsesSuccess + y.MaskWriteRegisterResponsesSuccess,
                OtherFunctionsRequests = x.OtherFunctionsRequests + y.OtherFunctionsRequests,
                OtherFunctionsResponsesError = x.OtherFunctionsResponsesError + y.OtherFunctionsResponsesError,
                OtherFunctionsResponsesSuccess = x.OtherFunctionsResponsesSuccess + y.OtherFunctionsResponsesSuccess,
                ReadCoilsBits = x.ReadCoilsBits + y.ReadCoilsBits,
                ReadCoilsRequests = x.ReadCoilsRequests + y.ReadCoilsRequests,
                ReadCoilsResponsesError = x.ReadCoilsResponsesError + y.ReadCoilsResponsesError,
                ReadCoilsResponsesSuccess = x.ReadCoilsResponsesSuccess + y.ReadCoilsResponsesSuccess,
                ReadDiscreteInputsBits = x.ReadDiscreteInputsBits + y.ReadDiscreteInputsBits,
                ReadDiscreteInputsRequests = x.ReadDiscreteInputsRequests + y.ReadDiscreteInputsRequests,
                ReadDiscreteInputsResponsesError = x.ReadDiscreteInputsResponsesError + y.ReadDiscreteInputsResponsesError,
                ReadDiscreteInputsResponsesSuccess = x.ReadDiscreteInputsResponsesSuccess + y.ReadDiscreteInputsResponsesSuccess,
                ReadFifoRequests = x.ReadFifoRequests + y.ReadFifoRequests,
                ReadFifoResponsesError = x.ReadFifoResponsesError + y.ReadFifoResponsesError,
                ReadFifoResponsesSuccess = x.ReadFifoResponsesSuccess + y.ReadFifoResponsesSuccess,
                ReadFileRecordCount = x.ReadFileRecordCount + y.ReadFileRecordCount,
                ReadFileRecordRequests = x.ReadFileRecordRequests + y.ReadFileRecordRequests,
                ReadFileRecordResponsesError = x.ReadFileRecordResponsesError + y.ReadFileRecordResponsesError,
                ReadFileRecordResponsesSuccess = x.ReadFileRecordResponsesSuccess + y.ReadFileRecordResponsesSuccess,
                ReadHoldingRegistersRequests = x.ReadHoldingRegistersRequests + y.ReadHoldingRegistersRequests,
                ReadHoldingRegistersResponsesError = x.ReadHoldingRegistersResponsesError + y.ReadHoldingRegistersResponsesError,
                ReadHoldingRegistersResponsesSuccess = x.ReadHoldingRegistersResponsesSuccess + y.ReadHoldingRegistersResponsesSuccess,
                ReadHoldingRegistersWords = x.ReadHoldingRegistersWords + y.ReadHoldingRegistersWords,
                ReadInputRegistersRequests = x.ReadInputRegistersRequests + y.ReadInputRegistersRequests,
                ReadInputRegistersResponsesError = x.ReadInputRegistersResponsesError + y.ReadInputRegistersResponsesError,
                ReadInputRegistersResponsesSuccess = x.ReadInputRegistersResponsesSuccess + y.ReadInputRegistersResponsesSuccess,
                ReadInputRegistersWords = x.ReadInputRegistersWords + y.ReadInputRegistersWords,
                ReadWriteMultRegistersReadWords = x.ReadWriteMultRegistersReadWords + y.ReadWriteMultRegistersReadWords,
                ReadWriteMultRegistersRequests = x.ReadWriteMultRegistersRequests + y.ReadWriteMultRegistersRequests,
                ReadWriteMultRegistersResponsesError = x.ReadWriteMultRegistersResponsesError + y.ReadWriteMultRegistersResponsesError,
                ReadWriteMultRegistersResponsesSuccess = x.ReadWriteMultRegistersResponsesSuccess + y.ReadWriteMultRegistersResponsesSuccess,
                ReadWriteMultRegistersWriteWords = x.ReadWriteMultRegistersWriteWords + y.ReadWriteMultRegistersWriteWords,
                UndefinedFunctionsRequests = x.UndefinedFunctionsRequests + y.UndefinedFunctionsRequests,
                UndefinedFunctionsResponsesError = x.UndefinedFunctionsResponsesError + y.UndefinedFunctionsResponsesError,
                UndefinedFunctionsResponsesSuccess = x.UndefinedFunctionsResponsesSuccess + y.UndefinedFunctionsResponsesSuccess,
                WriteFileRecordCount = x.WriteFileRecordCount + y.WriteFileRecordCount,
                WriteFileRecordRequests = x.WriteFileRecordRequests + y.WriteFileRecordRequests,
                WriteFileRecordResponsesError = x.WriteFileRecordResponsesError + y.WriteFileRecordResponsesError,
                WriteFileRecordResponsesSuccess = x.WriteFileRecordResponsesSuccess + y.WriteFileRecordResponsesSuccess,
                WriteMultCoilsBits = x.WriteMultCoilsBits + y.WriteMultCoilsBits,
                WriteMultCoilsRequests = x.WriteMultCoilsRequests + y.WriteMultCoilsRequests,
                WriteMultCoilsResponsesError = x.WriteMultCoilsResponsesError + y.WriteMultCoilsResponsesError,
                WriteMultCoilsResponsesSuccess = x.WriteMultCoilsResponsesSuccess + y.WriteMultCoilsResponsesSuccess,
                WriteMultRegistersRequests = x.WriteMultRegistersRequests + y.WriteMultRegistersRequests,
                WriteMultRegistersResponsesError = x.WriteMultRegistersResponsesError + y.WriteMultRegistersResponsesError,
                WriteMultRegistersResponsesSuccess = x.WriteMultRegistersResponsesSuccess + y.WriteMultRegistersResponsesSuccess,
                WriteMultRegistersWords = x.WriteMultRegistersWords + y.WriteMultRegistersWords,
                WriteSingleCoilRequests = x.WriteSingleCoilRequests + y.WriteSingleCoilRequests,
                WriteSingleCoilResponsesError = x.WriteSingleCoilResponsesError + y.WriteSingleCoilResponsesError,
                WriteSingleCoilResponsesSuccess = x.WriteSingleCoilResponsesSuccess + y.WriteSingleCoilResponsesSuccess,
                WriteSingleRegisterRequests = x.WriteSingleRegisterRequests + y.WriteSingleRegisterRequests,
                WriteSingleRegisterResponsesError = x.WriteSingleRegisterResponsesError + y.WriteSingleRegisterResponsesError,
                WriteSingleRegisterResponsesSuccess = x.WriteSingleRegisterResponsesSuccess + y.WriteSingleRegisterResponsesSuccess
            };
        }

        #endregion

    }
    /// <summary>
    /// A MODBUS flow extension that can be extracted from the simple MODBUS parser (parsing individual functions is not necessary).
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