using MessagePack;

namespace IcsMonitor.Modbus
{
    [MessagePackObject]
    public class ModbusFlowData
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
        /// <summary>
        /// A MODBUS flow extension.
        /// </summary>
        [MessagePackObject]
        public class Complete
        {
            ModbusFlowData _data;
            public Complete(ModbusFlowData data)
            {
                _data = data;
            }
            #region REQUESTS
            [Key("MODBUS_UNIT_ID")]
            public byte UnitId;
            [Key("MODBUS_READ_COILS_REQUESTS")]
            public int ReadCoilsRequests => _data.ReadCoilsRequests;
            [Key("MODBUS_READ_COILS_BITS")]
            public int ReadCoilsBits => _data.ReadCoilsBits;
            [Key("MODBUS_READ_DISCRETE_INPUTS_REQUESTS")]
            public int ReadDiscreteInputsRequests => _data.ReadDiscreteInputsRequests;
            [Key("MODBUS_READ_DISCRETE_INPUTS_BITS")]
            public int ReadDiscreteInputsBits => _data.ReadDiscreteInputsBits;
            [Key("MODBUS_READ_INPUT_REGISTERS_REQUESTS")]
            public int ReadInputRegistersRequests => _data.ReadInputRegistersRequests;
            [Key("MODBUS_READ_INPUT_REGISTERS_WORDS")]
            public int ReadInputRegistersWords => _data.ReadInputRegistersWords;
            [Key("MODBUS_READ_HOLDING_REGISTERS_REQUESTS")]
            public int ReadHoldingRegistersRequests => _data.ReadHoldingRegistersRequests;
            [Key("MODBUS_READ_HOLDING_REGISTERS_WORDS")]
            public int ReadHoldingRegistersWords => _data.ReadHoldingRegistersWords;
            [Key("MODBUS_WRITE_SINGLE_COIL_REQUESTS")]
            public int WriteSingleCoilRequests => _data.WriteSingleCoilRequests;
            [Key("MODBUS_WRITE_SINGLE_REGISTER_REQUESTS")]
            public int WriteSingleRegisterRequests => _data.WriteSingleRegisterRequests;
            [Key("MODBUS_WRITE_MULT_COILS_REQUESTS")]
            public int WriteMultCoilsRequests => _data.WriteMultCoilsRequests;
            [Key("MODBUS_WRITE_MULT_COILS_BITS")]
            public int WriteMultCoilsBits => _data.WriteMultCoilsBits;
            [Key("MODBUS_WRITE_MULT_REGISTERS_REQUESTS")]
            public int WriteMultRegistersRequests => _data.WriteMultRegistersRequests;
            [Key("MODBUS_WRITE_MULT_REGISTERS_WORDS")]
            public int WriteMultRegistersWords => _data.WriteMultRegistersWords;
            [Key("MODBUS_READ_FILE_RECORD_REQUESTS")]
            public int ReadFileRecordRequests => _data.ReadFileRecordRequests;
            [Key("MODBUS_READ_FILE_RECORD_COUNT")]
            public int ReadFileRecordCount => _data.ReadFileRecordCount;
            [Key("MODBUS_WRITE_FILE_RECORD_REQUESTS")]
            public int WriteFileRecordRequests => _data.WriteFileRecordRequests;
            [Key("MODBUS_WRTIE_FILE_RECORD_COUNT")]
            public int WriteFileRecordCount => _data.WriteFileRecordCount;
            [Key("MODBUS_MASK_WRITE_REGISTER_REQUESTS")]
            public int MaskWriteRegisterRequests => _data.MaskWriteRegisterRequests;
            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_REQUESTS")]
            public int ReadWriteMultRegistersRequests => _data.ReadWriteMultRegistersRequests;
            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_READ_WORDS")]
            public int ReadWriteMultRegistersReadWords => _data.ReadWriteMultRegistersReadWords;
            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_WRITE_WORDS")]
            public int ReadWriteMultRegistersWriteWords => _data.ReadWriteMultRegistersWriteWords;
            [Key("MODBUS_READ_FIFO_REQUESTS")]
            public int ReadFifoRequests => _data.ReadFifoRequests;
            [Key("MODBUS_DIAGNOSTIC_REQUESTS")]
            public int DiagnosticFunctionsRequests => _data.DiagnosticFunctionsRequests;
            [Key("MODBUS_OTHER_REQUESTS")]
            public int OtherFunctionsRequests => _data.OtherFunctionsRequests;
            [Key("MODBUS_UNDEFINED_REQUESTS")]
            public int UndefinedFunctionsRequests => _data.UndefinedFunctionsRequests;
            #endregion

            #region RESPONSES - SUCCESS
            [Key("MODBUS_READ_COILS_RESPONSES_SUCCESS")]
            public int ReadCoilsResponsesSuccess => _data.ReadCoilsResponsesSuccess;

            [Key("MODBUS_READ_DISCRETE_INPUTS_RESPONSES_SUCCESS")]
            public int ReadDiscreteInputsResponsesSuccess => _data.ReadDiscreteInputsResponsesSuccess;

            [Key("MODBUS_READ_INPUT_REGISTERS_RESPONSES_SUCCESS")]
            public int ReadInputRegistersResponsesSuccess => _data.ReadInputRegistersResponsesSuccess;

            [Key("MODBUS_READ_HOLDING_REGISTERS_RESPONSES_SUCCESS")]
            public int ReadHoldingRegistersResponsesSuccess => _data.ReadHoldingRegistersResponsesSuccess;

            [Key("MODBUS_WRITE_SINGLE_COIL_RESPONSES_SUCCESS")]
            public int WriteSingleCoilResponsesSuccess => _data.WriteSingleCoilResponsesSuccess;

            [Key("MODBUS_WRITE_SINGLE_REGISTER_RESPONSES_SUCCESS")]
            public int WriteSingleRegisterResponsesSuccess => _data.WriteSingleRegisterResponsesSuccess;

            [Key("MODBUS_WRITE_MULT_COILS_RESPONSES_SUCCESS")]
            public int WriteMultCoilsResponsesSuccess => _data.WriteMultCoilsResponsesSuccess;

            [Key("MODBUS_WRITE_MULT_REGISTERS_RESPONSES_SUCCESS")]
            public int WriteMultRegistersResponsesSuccess => _data.WriteMultRegistersResponsesSuccess;

            [Key("MODBUS_READ_FILE_RECORD_RESPONSES_SUCCESS")]
            public int ReadFileRecordResponsesSuccess => _data.ReadFileRecordResponsesSuccess;

            [Key("MODBUS_WRITE_FILE_RECORD_RESPONSES_SUCCESS")]
            public int WriteFileRecordResponsesSuccess => _data.WriteFileRecordResponsesSuccess;

            [Key("MODBUS_MASK_WRITE_REGISTER_RESPONSES_SUCCESS")]
            public int MaskWriteRegisterResponsesSuccess => _data.MaskWriteRegisterResponsesSuccess;

            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_RESPONSES_SUCCESS")]
            public int ReadWriteMultRegistersResponsesSuccess => _data.ReadWriteMultRegistersResponsesSuccess;

            [Key("MODBUS_READ_FIFO_RESPONSES_SUCCESS")]
            public int ReadFifoResponsesSuccess => _data.ReadFifoResponsesSuccess;

            [Key("MODBUS_DIAGNOSTIC_RESPONSES_SUCCESS")]
            public int DiagnosticFunctionsResponsesSuccess => _data.DiagnosticFunctionsResponsesSuccess;

            [Key("MODBUS_OTHER_RESPONSES_SUCCESS")]
            public int OtherFunctionsResponsesSuccess => _data.OtherFunctionsResponsesSuccess;

            [Key("MODBUS_UNDEFINED_RESPONSES_SUCCESS")]
            public int UndefinedFunctionsResponsesSuccess => _data.UndefinedFunctionsResponsesSuccess;

            #endregion

            #region RESPONSES - ERROR

            [Key("MODBUS_READ_COILS_RESPONSES_ERROR")]
            public int ReadCoilsResponsesError => _data.ReadCoilsResponsesError;

            [Key("MODBUS_READ_DISCRETE_INPUTS_RESPONSES_ERROR")]
            public int ReadDiscreteInputsResponsesError => _data.ReadDiscreteInputsResponsesError;

            [Key("MODBUS_READ_INPUT_REGISTERS_RESPONSES_ERROR")]
            public int ReadInputRegistersResponsesError => _data.ReadInputRegistersResponsesError;

            [Key("MODBUS_READ_HOLDING_REGISTERS_RESPONSES_ERROR")]
            public int ReadHoldingRegistersResponsesError => _data.ReadHoldingRegistersResponsesError;

            [Key("MODBUS_WRITE_SINGLE_COIL_RESPONSES_ERROR")]
            public int WriteSingleCoilResponsesError => _data.WriteSingleCoilResponsesError;

            [Key("MODBUS_WRITE_SINGLE_REGISTER_RESPONSES_ERROR")]
            public int WriteSingleRegisterResponsesError => _data.WriteSingleRegisterResponsesError;

            [Key("MODBUS_WRITE_MULT_COILS_RESPONSES_ERROR")]
            public int WriteMultCoilsResponsesError => _data.WriteMultCoilsResponsesError;

            [Key("MODBUS_WRITE_MULT_REGISTERS_RESPONSES_ERROR")]
            public int WriteMultRegistersResponsesError => _data.WriteMultRegistersResponsesError;

            [Key("MODBUS_READ_FILE_RECORD_RESPONSES_ERROR")]
            public int ReadFileRecordResponsesError => _data.ReadFileRecordResponsesError;

            [Key("MODBUS_WRITE_FILE_RECORD_RESPONSES_ERROR")]
            public int WriteFileRecordResponsesError => _data.WriteFileRecordResponsesError;

            [Key("MODBUS_MASK_WRITE_REGISTER_RESPONSES_ERROR")]
            public int MaskWriteRegisterResponsesError => _data.MaskWriteRegisterResponsesError;

            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_RESPONSES_ERROR")]
            public int ReadWriteMultRegistersResponsesError => _data.ReadWriteMultRegistersResponsesError;

            [Key("MODBUS_READ_FIFO_RESPONSES_ERROR")]
            public int ReadFifoResponsesError => _data.ReadFifoResponsesError;

            [Key("MODBUS_DIAGNOSTIC_RESPONSES_ERROR")]
            public int DiagnosticFunctionsResponsesError => _data.DiagnosticFunctionsResponsesError;

            [Key("MODBUS_OTHER_RESPONSES_ERROR")]
            public int OtherFunctionsResponsesError => _data.OtherFunctionsResponsesError;

            [Key("MODBUS_UNDEFINED_RESPONSES_ERROR")]
            public int UndefinedFunctionsResponsesError => _data.UndefinedFunctionsResponsesError;
            #endregion


            #region Malformed messages

            [Key("MODBUS_MALFORMED_REQUESTS")]
            public int MalformedRequests => _data.MalformedRequests;

            [Key("MODBUS_MALFORMED_RESPONSES")]
            public int MalformedResponses => _data.MalformedResponses;

            #endregion

        }
        /// <summary>
        /// A MODBUS flow extension that can be extracted from the simple MODBUS parser (parsing individual functions is not necessary).
        /// </summary>
        [MessagePackObject]
        public class Extended
        {
            ModbusFlowData _data;
            public Extended(ModbusFlowData data)
            {
                _data = data;
            }
            #region REQUESTS
            [Key("MODBUS_UNIT_ID")]
            public byte UnitId => _data.UnitId;
            [Key("MODBUS_READ_COILS_REQUESTS")]
            public int ReadCoilsRequests => _data.ReadCoilsRequests;
            [Key("MODBUS_READ_DISCRETE_INPUTS_REQUESTS")]
            public int ReadDiscreteInputsRequests => _data.ReadDiscreteInputsRequests;
            [Key("MODBUS_READ_INPUT_REGISTERS_REQUESTS")]
            public int ReadInputRegistersRequests => _data.ReadInputRegistersRequests;
            [Key("MODBUS_READ_HOLDING_REGISTERS_REQUESTS")]
            public int ReadHoldingRegistersRequests => _data.ReadHoldingRegistersRequests;
            [Key("MODBUS_WRITE_SINGLE_COIL_REQUESTS")]
            public int WriteSingleCoilRequests => _data.WriteSingleCoilRequests;
            [Key("MODBUS_WRITE_SINGLE_REGISTER_REQUESTS")]
            public int WriteSingleRegisterRequests => _data.WriteSingleRegisterRequests;
            [Key("MODBUS_WRITE_MULT_COILS_REQUESTS")]
            public int WriteMultCoilsRequests => _data.WriteMultCoilsRequests;
            [Key("MODBUS_WRITE_MULT_REGISTERS_REQUESTS")]
            public int WriteMultRegistersRequests => _data.WriteMultRegistersRequests;
            [Key("MODBUS_READ_FILE_RECORD_REQUESTS")]
            public int ReadFileRecordRequests => _data.ReadFileRecordRequests;
            [Key("MODBUS_WRITE_FILE_RECORD_REQUESTS")]
            public int WriteFileRecordRequests => _data.WriteFileRecordRequests;
            [Key("MODBUS_MASK_WRITE_REGISTER_REQUESTS")]
            public int MaskWriteRegisterRequests => _data.MaskWriteRegisterRequests;
            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_REQUESTS")]
            public int ReadWriteMultRegistersRequests => _data.ReadWriteMultRegistersRequests;
            [Key("MODBUS_READ_FIFO_REQUESTS")]
            public int ReadFifoRequests => _data.ReadFifoRequests;
            [Key("MODBUS_DIAGNOSTIC_REQUESTS")]
            public int DiagnosticFunctionsRequests => _data.DiagnosticFunctionsRequests;
            [Key("MODBUS_OTHER_REQUESTS")]
            public int OtherFunctionsRequests => _data.OtherFunctionsRequests;
            [Key("MODBUS_UNDEFINED_REQUESTS")]
            public int UndefinedFunctionsRequests => _data.UndefinedFunctionsRequests;
            #endregion

            #region RESPONSES - SUCCESS
            [Key("MODBUS_READ_COILS_RESPONSES_SUCCESS")]
            public int ReadCoilsResponsesSuccess => _data.ReadCoilsResponsesSuccess;

            [Key("MODBUS_READ_DISCRETE_INPUTS_RESPONSES_SUCCESS")]
            public int ReadDiscreteInputsResponsesSuccess => _data.ReadDiscreteInputsResponsesSuccess;

            [Key("MODBUS_READ_INPUT_REGISTERS_RESPONSES_SUCCESS")]
            public int ReadInputRegistersResponsesSuccess => _data.ReadInputRegistersResponsesSuccess;

            [Key("MODBUS_READ_HOLDING_REGISTERS_RESPONSES_SUCCESS")]
            public int ReadHoldingRegistersResponsesSuccess => _data.ReadHoldingRegistersResponsesSuccess;

            [Key("MODBUS_WRITE_SINGLE_COIL_RESPONSES_SUCCESS")]
            public int WriteSingleCoilResponsesSuccess => _data.WriteSingleCoilResponsesSuccess;

            [Key("MODBUS_WRITE_SINGLE_REGISTER_RESPONSES_SUCCESS")]
            public int WriteSingleRegisterResponsesSuccess => _data.WriteSingleRegisterResponsesSuccess;

            [Key("MODBUS_WRITE_MULT_COILS_RESPONSES_SUCCESS")]
            public int WriteMultCoilsResponsesSuccess => _data.WriteMultCoilsResponsesSuccess;

            [Key("MODBUS_WRITE_MULT_REGISTERS_RESPONSES_SUCCESS")]
            public int WriteMultRegistersResponsesSuccess => _data.WriteMultRegistersResponsesSuccess;

            [Key("MODBUS_READ_FILE_RECORD_RESPONSES_SUCCESS")]
            public int ReadFileRecordResponsesSuccess => _data.ReadFileRecordResponsesSuccess;

            [Key("MODBUS_WRITE_FILE_RECORD_RESPONSES_SUCCESS")]
            public int WriteFileRecordResponsesSuccess => _data.WriteFileRecordResponsesSuccess;

            [Key("MODBUS_MASK_WRITE_REGISTER_RESPONSES_SUCCESS")]
            public int MaskWriteRegisterResponsesSuccess => _data.MaskWriteRegisterResponsesSuccess;

            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_RESPONSES_SUCCESS")]
            public int ReadWriteMultRegistersResponsesSuccess => _data.ReadWriteMultRegistersResponsesSuccess;

            [Key("MODBUS_READ_FIFO_RESPONSES_SUCCESS")]
            public int ReadFifoResponsesSuccess => _data.ReadFifoResponsesSuccess;

            [Key("MODBUS_DIAGNOSTIC_RESPONSES_SUCCESS")]
            public int DiagnosticFunctionsResponsesSuccess => _data.DiagnosticFunctionsResponsesSuccess;

            [Key("MODBUS_OTHER_RESPONSES_SUCCESS")]
            public int OtherFunctionsResponsesSuccess => _data.OtherFunctionsResponsesSuccess;

            [Key("MODBUS_UNDEFINED_RESPONSES_SUCCESS")]
            public int UndefinedFunctionsResponsesSuccess => _data.UndefinedFunctionsResponsesSuccess;

            #endregion

            #region RESPONSES - ERROR

            [Key("MODBUS_READ_COILS_RESPONSES_ERROR")]
            public int ReadCoilsResponsesError => _data.ReadCoilsResponsesError;

            [Key("MODBUS_READ_DISCRETE_INPUTS_RESPONSES_ERROR")]
            public int ReadDiscreteInputsResponsesError => _data.ReadDiscreteInputsResponsesError;

            [Key("MODBUS_READ_INPUT_REGISTERS_RESPONSES_ERROR")]
            public int ReadInputRegistersResponsesError => _data.ReadInputRegistersResponsesError;

            [Key("MODBUS_READ_HOLDING_REGISTERS_RESPONSES_ERROR")]
            public int ReadHoldingRegistersResponsesError => _data.ReadHoldingRegistersResponsesError;

            [Key("MODBUS_WRITE_SINGLE_COIL_RESPONSES_ERROR")]
            public int WriteSingleCoilResponsesError => _data.WriteSingleCoilResponsesError;

            [Key("MODBUS_WRITE_SINGLE_REGISTER_RESPONSES_ERROR")]
            public int WriteSingleRegisterResponsesError => _data.WriteSingleRegisterResponsesError;

            [Key("MODBUS_WRITE_MULT_COILS_RESPONSES_ERROR")]
            public int WriteMultCoilsResponsesError => _data.WriteMultCoilsResponsesError;

            [Key("MODBUS_WRITE_MULT_REGISTERS_RESPONSES_ERROR")]
            public int WriteMultRegistersResponsesError => _data.WriteMultRegistersResponsesError;

            [Key("MODBUS_READ_FILE_RECORD_RESPONSES_ERROR")]
            public int ReadFileRecordResponsesError => _data.ReadFileRecordResponsesError;

            [Key("MODBUS_WRITE_FILE_RECORD_RESPONSES_ERROR")]
            public int WriteFileRecordResponsesError => _data.WriteFileRecordResponsesError;

            [Key("MODBUS_MASK_WRITE_REGISTER_RESPONSES_ERROR")]
            public int MaskWriteRegisterResponsesError => _data.MaskWriteRegisterResponsesError;

            [Key("MODBUS_READ_WRITE_MULT_REGISTERS_RESPONSES_ERROR")]
            public int ReadWriteMultRegistersResponsesError => _data.ReadWriteMultRegistersResponsesError;

            [Key("MODBUS_READ_FIFO_RESPONSES_ERROR")]
            public int ReadFifoResponsesError => _data.ReadFifoResponsesError;

            [Key("MODBUS_DIAGNOSTIC_RESPONSES_ERROR")]
            public int DiagnosticFunctionsResponsesError => _data.DiagnosticFunctionsResponsesError;

            [Key("MODBUS_OTHER_RESPONSES_ERROR")]
            public int OtherFunctionsResponsesError => _data.OtherFunctionsResponsesError;

            [Key("MODBUS_UNDEFINED_RESPONSES_ERROR")]
            public int UndefinedFunctionsResponsesError => _data.UndefinedFunctionsResponsesError;
            #endregion

            [Key("MODBUS_MALFORMED_REQUESTS")]
            public int MalformedRequests => _data.MalformedRequests;

            [Key("MODBUS_MALFORMED_RESPONSES")]
            public int MalformedResponses => _data.MalformedResponses;
        }

        /// <summary>
        /// A MODBUS flow extension that can be extracted from the simple MODBUS parser (parsing individual functions is not necessary).
        /// </summary>
        [MessagePackObject]
        public class Compact
        {
            ModbusFlowData _data;
            public Compact(ModbusFlowData data)
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
}