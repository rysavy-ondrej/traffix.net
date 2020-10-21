using MessagePack;

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

        /// <summary>
        /// Aggregates function codes 3-6.
        /// <para/>
        /// Functions: Dnp3Select = 3, Dnp3Operate = 4,
        /// Dnp3DirOperate = 5, and Dnp3DirOperateNoResp = 6.
        /// </summary>
        [Key("DNP3_CONTROL_REQUESTS")]
        public int ControlRequests { get; internal set; }

        /// <summary>
        /// Aggregates all freeze functions (7-12).
        /// </summary>
        [Key("DNP3_FREEZE_REQUESTS")]
        public int FreezeRequests { get; internal set; }

        /// <summary>
        /// Aggregates functions 13-18.
        /// </summary>
        [Key("DNP3_APPLICATION_CONTROL_REQUESTS")]
        public int ApplicationControlRequests { get; internal set; }

        /// <summary>
        /// Aggregates configuratino related functions (19-22).
        /// </summary>
        [Key("DNP3_CONFIGURATION_REQUESTS")]
        public int ConfigurationRequests { get; internal set; }

        /// <summary>
        /// Represents time sync requests, i.e., fc=23.
        /// </summary>
        [Key("DNP3_TIME_SYNCHRONIZATION_REQUESTS")]
        public int TimeSynchronizationRequests { get; internal set; }

        /// <summary>
        /// Aggregates functions in range 24-128.
        /// </summary>
        [Key("DNP3_RESERVED_REQUESTS")]
        public int ReservedRequests { get; internal set; }

        /// <summary>
        /// Aggregates all other functions > 128.
        /// </summary>
        [Key("DNP3_OTHER_OPERATION_REQUESTS")]
        public int OtherOperationRequests { get; internal set; }

        /// <summary>
        /// Represents count of invalid DNP3 packets.
        /// </summary>
        [Key("DNP3_MALFORMED_REQUESTS")]
        public int MalformedRequests { get; internal set; }
        #endregion

        #region RESPONSES:
        [Key("DNP3_CONFIRMATION_RESPONSES")]
        public int ConfirmationResponses { get; internal set; }

        [Key("DNP3_REGULAR_RESPONSES")]
        public int RegularResponses { get; internal set; }

        [Key("DNP3_UNSOLICITED_RESPONSES")]
        public int UnsolicitedResponses { get; internal set; }

        /// <summary>
        /// Represents count of DNP3 response, which function code is 
        /// not 0, 129, or 130.
        /// </summary>
        [Key("DNP3_OTHER_RESPONSES")]
        public int OtherResponses { get; internal set; }

        /// <summary>
        /// Represents count of invalid DNP3 packets.
        /// </summary>
        [Key("DNP3_MALFORMED_RESPONSES")]
        public int MalformedResponses { get; internal set; }

        [Key("DNP3_RESPONSE_DEVICE_RESTART_COUNT")]
        public int DeviceRestartCount { get; internal set; }

        [Key("DNP3_RESPONSE_DEVICE_TROUBLE_COUNT")]
        public int DeviceTroubleCount { get; internal set; }

        [Key("DNP3_RESPONSE_CONFIGURATION_CORRUPT_COUNT")]
        public int ConfigurationCorruptCount { get; internal set; }


        [Key("DNP3_RESPONSE_EVENT_BUFFER_OVERFLOW_COUNT")]
        public int EventBufferOverflowCount { get; internal set; }

        [Key("DNP3_RESPONSE_PARAMETER_ERROR_COUNT")]
        public int ParameterErrorCount { get; internal set; }

        [Key("DNP3_RESPONSE_OBJECT_UNKNOWN_COUNT")]
        public int ObjectUnknownCount { get; internal set; }

        [Key("DNP3_RESPONSE_FUNCTION_NOT_SUPPORTED_COUNT")]
        public int FunctionNotSupportedCount { get; internal set; }


        #endregion
    }
}