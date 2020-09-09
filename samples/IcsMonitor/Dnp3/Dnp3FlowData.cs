using MessagePack;
using Traffix.Extensions.Decoders.Industrial;

namespace IcsMonitor.Modbus
{
    /// <summary>
    /// The extended flow data type for DNP3 protocol.
    /// </summary>
    [MessagePackObject]
    public class Dnp3FlowData
    {
        [Key("DNP3_CONFIRMATIONS")]
        public int Confirmations { get; internal set; }

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

        [Key("DNP3_OTHER_OPERATION_REQUESTS")]
        public int OtherOperationRequests { get; internal set; }

        [Key("DNP3_FILE_OPERATION_REQUESTS")]
        public int FileOperationRequests { get; internal set; }

        [Key("DNP3_RESPONSES")]
        public int Responses { get; internal set; }

        [Key("DNP3_UNSOLICITED_RESPONSES")]
        public int UnsolicitedResponses { get; internal set; }

        [Key("DNP3_OTHER_RESPONSES")]
        public int OtherResponses { get; internal set; }

        [Key("DNP3_MALFORMED_REQUESTS")]
        public int MalformedRequests { get; internal set; }

        [Key("DNP3_MALFORMED_RESPONSES")]
        public int MalformedResponses { get; internal set; }

        #region Flags in Response messages
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
/*
            Dnp3Confirm = 0,

            Dnp3Read = 1,

            Dnp3Write = 2,

            Dnp3Select = 3,

            OPERATE:
            Dnp3Operate = 4,
            Dnp3DirOperate = 5,
            Dnp3DirOperateNoResp = 6,

            FREEZE:
            Dnp3Freeze = 7,
            Dnp3FreezeNoResp = 8,
            Dnp3FreezeClear = 9,
            Dnp3FreezeClearNoResp = 10,
            Dnp3FreezeAtTime = 11,
            Dnp3FreezeAtTimeNoResp = 12,

            RESTART:
            Dnp3ColdRestart = 13,
            Dnp3WarmRestart = 14,

            INITIALIZE:
            Dnp3InitializeData = 15,
            Dnp3InitializeApplication = 16,

            APPLICATION:
            Dnp3StartApplication = 17,
            Dnp3StopApplication = 18,

            FILE:
            Dnp3OpenFile = 25,
            Dnp3CloseFile = 26,
            Dnp3DeleteFile = 27,
            Dnp3GetFileInformation = 28,
            Dnp3AuthenticateFile = 29,
            Dnp3AbortFile = 30,

            OTHER:
            Dnp3SaveConfiguration = 19,
            Dnp3EnableUnsolicited = 20,
            Dnp3DisableUnsolicited = 21,
            Dnp3AssignClass = 22,
            Dnp3DelayMeasurement = 23,
            Dnp3RecordCurrentTime = 24,

            RESPONSES:
            Dnp3Response = 129,
            Dnp3UnsolicitedResponse = 130,
*/