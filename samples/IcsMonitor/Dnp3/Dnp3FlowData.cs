using MessagePack;
using Traffix.Extensions.Decoders.Industrial;

namespace IcsMonitor.Modbus
{
    /// <summary>
    /// The extended flow data type for DNP3 protocol.
    /// It represents both request and response directions of communication.
    /// <para/> 
    /// Because the communication parties have well-defined roles, 
    /// we can distinguish between requests and responses. 
    /// The requests are grouped according to function classes to 
    /// keep the number of fields relatively small.
    /// </summary>
    [MessagePackObject]
    public class Dnp3FlowData
    {
        #region REQUESTS:
        [Key("DNP3_CONFIRMATION_REQUESTS")]
        public int ConfirmationRequests { get; internal set; }

        [Key("DNP3_READ_REQUESTS")]
        public int ReadRequests { get; internal set; }

        [Key("DNP3_WRITE_REQUESTS")]
        public int WriteRequests { get; internal set; }

        [Key("DNP3_SELECT_REQUESTS")]
        public int SelectRequests { get; internal set; }

        [Key("DNP3_OPERATE_REQUESTS")]
        public int OperateRequests { get; internal set; }

        [Key("DNP3_FREEZE_REQUESTS")]
        public int FreezeRequests { get; internal set; }

        [Key("DNP3_RESTART_REQUESTS")]
        public int RestartRequests { get; internal set; }

        [Key("DNP3_INITIALIZE_REQUESTS")]
        public int InitializeRequests { get; internal set; }

        [Key("DNP3_APPLICATION_OPERATION_REQUESTS")]
        public int ApplicationOperationRequests { get; internal set; }

        [Key("DNP3_FILE_OPERATION_REQUESTS")]
        public int FileOperationRequests { get; internal set; }

        [Key("DNP3_OTHER_OPERATION_REQUESTS")]
        public int OtherOperationRequests { get; internal set; }

        [Key("DNP3_MALFORMED_REQUESTS")]
        public int MalformedRequests { get; internal set; }
        #endregion

        #region RESPONSES:

        [Key("DNP3_REGULAR_RESPONSES")]
        public int RegularResponses { get; internal set; }

        [Key("DNP3_UNSOLICITED_RESPONSES")]
        public int UnsolicitedResponses { get; internal set; }

        [Key("DNP3_OTHER_RESPONSES")]
        public int OtherResponses { get; internal set; }

        [Key("DNP3_MALFORMED_RESPONSES")]
        public int MalformedResponses { get; internal set; }

        [Key("DNP3_RESPONSE_DEVICE_RESTART_FLAG")]
        public int DeviceRestartFlag { get; internal set; }

        [Key("DNP3_RESPONSE_DEVICE_TROUBLE_FLAG")]
        public int DeviceTroubleFlag { get; internal set; }

        [Key("DNP3_RESPONSE_LOCAL_CONTROL_FLAG")]
        public int LocalControlFlag { get; internal set; }

        [Key("DNP3_RESPONSE_NEED_TIME_FLAG")]
        public int NeedTimeFlag { get; internal set; }

        [Key("DNP3_RESPONSE_CLASS3_EVENT_FLAG")]
        public int Class3EventFlag { get; internal set; }

        [Key("DNP3_RESPONSE_CLASS2_EVENT_FLAG")]
        public int Class2EventFlag { get; internal set; }

        [Key("DNP3_RESPONSE_CLASS1_EVENT_FLAG")]
        public int Class1EventFlag { get; internal set; }

        [Key("DNP3_RESPONSE_ALL_STATIONS_FLAG")]
        public int AllStationsFlag { get; internal set; }

        [Key("DNP3_RESPONSE_CONFIGURATION_CORRUPT_FLAG")]
        public int ConfigurationCorruptFlag { get; internal set; }

        [Key("DNP3_RESPONSE_ALREADY_EXECUTING_FLAG")]
        public int AlreadyExecutingFlag { get; internal set; }

        [Key("DNP3_RESPONSE_EVENT_BUFFER_OVERFLOW_FLAG")]
        public int EventBufferOverflowFlag { get; internal set; }

        [Key("DNP3_RESPONSE_PARAMETER_ERROR_FLAG")]
        public int ParameterErrorFlag { get; internal set; }

        [Key("DNP3_RESPONSE_OBJECT_UNKNOWN_FLAG")]
        public int ObjectUnknownFlag { get; internal set; }

        [Key("DNP3_RESPONSE_FUNCTION_NOT_SUPPORTED_FLAG")]
        public int FunctionNotSupportedFlag { get; internal set; }

        #endregion
    }
}