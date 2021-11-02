// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class S7commPacket : KaitaiStruct
    {
        public static S7commPacket FromFile(string fileName)
        {
            return new S7commPacket(new KaitaiStream(fileName));
        }


        public enum S7MessageType
        {
            JobRequest = 1,
            Ack = 2,
            AckData = 3,
            UserData = 7,
            ServerControl = 8,
        }

        public enum ErrorClass
        {
            NoError = 0,
            Apprel = 129,
            Objdef = 130,
            Resource = 131,
            Service = 132,
            Supplies = 133,
            Access = 135,
        }

        public enum AreaCode
        {
            Flags = 131,
            DataBlocks = 132,
        }

        public enum S7FunctionCode
        {
            ReadVariable = 4,
            WriteVariable = 5,
            RequestDownload = 26,
            DownloadBlock = 27,
            DownloadEnded = 28,
            StartUpload = 29,
            Upload = 30,
            EndUpload = 31,
            PlcControl = 40,
            PlcStop = 41,
            SetupCommunication = 240,
        }

        public enum ReturnCode
        {
            Reserved = 0,
            DataHwFault = 1,
            DataAccessFault = 3,
            DataOutOfRange = 5,
            DataTypeNorSupported = 6,
            DataSizeMismatch = 7,
            DataNotAvailable = 10,
            Success = 255,
        }

        public enum TransportSizeType
        {
            Nul = 0,
            Byte = 2,
            Integer = 5,
        }
        public S7commPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, S7commPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _protcolId = m_io.ReadU1();
            _messageType = ((S7MessageType) m_io.ReadU1());
            _reserved = m_io.ReadU2be();
            _pduReference = m_io.ReadU2be();
            _parametersLength = m_io.ReadU2be();
            _dataLength = m_io.ReadU2be();
            if (MessageType == S7MessageType.AckData) {
                _error = new ResponseError(m_io, this, m_root);
            }
            switch (MessageType) {
            case S7MessageType.JobRequest: {
                __raw_message = m_io.ReadBytes((ParametersLength + DataLength));
                var io___raw_message = new KaitaiStream(__raw_message);
                _message = new JobRequestMessage(io___raw_message, this, m_root);
                break;
            }
            case S7MessageType.AckData: {
                __raw_message = m_io.ReadBytes((ParametersLength + DataLength));
                var io___raw_message = new KaitaiStream(__raw_message);
                _message = new AckDataMessage(io___raw_message, this, m_root);
                break;
            }
            default: {
                __raw_message = m_io.ReadBytes((ParametersLength + DataLength));
                var io___raw_message = new KaitaiStream(__raw_message);
                _message = new MessageOther(io___raw_message, this, m_root);
                break;
            }
            }
        }
        public partial class JobSetupCommunication : KaitaiStruct
        {
            public static JobSetupCommunication FromFile(string fileName)
            {
                return new JobSetupCommunication(new KaitaiStream(fileName));
            }

            public JobSetupCommunication(KaitaiStream p__io, KaitaiStruct p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _reserved = m_io.ReadU1();
                _maxAmqCaller = m_io.ReadU2be();
                _maxAmqCallee = m_io.ReadU2be();
                _maxPduLength = m_io.ReadU2be();
            }
            private byte _reserved;
            private ushort _maxAmqCaller;
            private ushort _maxAmqCallee;
            private ushort _maxPduLength;
            private S7commPacket m_root;
            private KaitaiStruct m_parent;
            public byte Reserved { get { return _reserved; } }
            public ushort MaxAmqCaller { get { return _maxAmqCaller; } }
            public ushort MaxAmqCallee { get { return _maxAmqCallee; } }
            public ushort MaxPduLength { get { return _maxPduLength; } }
            public S7commPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class AckDataOther : KaitaiStruct
        {
            public static AckDataOther FromFile(string fileName)
            {
                return new AckDataOther(new KaitaiStream(fileName));
            }

            public AckDataOther(KaitaiStream p__io, S7commPacket.AckDataMessage p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _parameters = m_io.ReadBytes((M_Root.ParametersLength - 1));
                _data = m_io.ReadBytes(M_Root.DataLength);
            }
            private byte[] _parameters;
            private byte[] _data;
            private S7commPacket m_root;
            private S7commPacket.AckDataMessage m_parent;
            public byte[] Parameters { get { return _parameters; } }
            public byte[] Data { get { return _data; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket.AckDataMessage M_Parent { get { return m_parent; } }
        }
        public partial class MessageOther : KaitaiStruct
        {
            public static MessageOther FromFile(string fileName)
            {
                return new MessageOther(new KaitaiStream(fileName));
            }

            public MessageOther(KaitaiStream p__io, S7commPacket p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _parameters = m_io.ReadBytes(M_Root.ParametersLength);
                _data = m_io.ReadBytes(M_Root.DataLength);
            }
            private byte[] _parameters;
            private byte[] _data;
            private S7commPacket m_root;
            private S7commPacket m_parent;
            public byte[] Parameters { get { return _parameters; } }
            public byte[] Data { get { return _data; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket M_Parent { get { return m_parent; } }
        }
        public partial class ResponseError : KaitaiStruct
        {
            public static ResponseError FromFile(string fileName)
            {
                return new ResponseError(new KaitaiStream(fileName));
            }

            public ResponseError(KaitaiStream p__io, S7commPacket p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _errorClass = m_io.ReadU1();
                _errorCode = m_io.ReadU1();
            }
            private byte _errorClass;
            private byte _errorCode;
            private S7commPacket m_root;
            private S7commPacket m_parent;
            public byte ErrorClass { get { return _errorClass; } }
            public byte ErrorCode { get { return _errorCode; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket M_Parent { get { return m_parent; } }
        }
        public partial class JobRequestMessage : KaitaiStruct
        {
            public static JobRequestMessage FromFile(string fileName)
            {
                return new JobRequestMessage(new KaitaiStream(fileName));
            }

            public JobRequestMessage(KaitaiStream p__io, S7commPacket p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _functionCode = ((S7commPacket.S7FunctionCode) m_io.ReadU1());
                switch (FunctionCode) {
                case S7commPacket.S7FunctionCode.SetupCommunication: {
                    _function = new JobSetupCommunication(m_io, this, m_root);
                    break;
                }
                case S7commPacket.S7FunctionCode.ReadVariable: {
                    _function = new JobReadVariable(m_io, this, m_root);
                    break;
                }
                case S7commPacket.S7FunctionCode.WriteVariable: {
                    _function = new JobWriteVariable(m_io, this, m_root);
                    break;
                }
                default: {
                    _function = new JobOther(m_io, this, m_root);
                    break;
                }
                }
            }
            private S7FunctionCode _functionCode;
            private KaitaiStruct _function;
            private S7commPacket m_root;
            private S7commPacket m_parent;
            public S7FunctionCode FunctionCode { get { return _functionCode; } }
            public KaitaiStruct Function { get { return _function; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket M_Parent { get { return m_parent; } }
        }
        public partial class AckDataMessage : KaitaiStruct
        {
            public static AckDataMessage FromFile(string fileName)
            {
                return new AckDataMessage(new KaitaiStream(fileName));
            }

            public AckDataMessage(KaitaiStream p__io, S7commPacket p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _functionCode = ((S7commPacket.S7FunctionCode) m_io.ReadU1());
                switch (FunctionCode) {
                case S7commPacket.S7FunctionCode.SetupCommunication: {
                    _function = new JobSetupCommunication(m_io, this, m_root);
                    break;
                }
                case S7commPacket.S7FunctionCode.ReadVariable: {
                    _function = new AckDataReadVariable(m_io, this, m_root);
                    break;
                }
                case S7commPacket.S7FunctionCode.WriteVariable: {
                    _function = new AckDataWriteVariable(m_io, this, m_root);
                    break;
                }
                default: {
                    _function = new AckDataOther(m_io, this, m_root);
                    break;
                }
                }
            }
            private S7FunctionCode _functionCode;
            private KaitaiStruct _function;
            private S7commPacket m_root;
            private S7commPacket m_parent;
            public S7FunctionCode FunctionCode { get { return _functionCode; } }
            public KaitaiStruct Function { get { return _function; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket M_Parent { get { return m_parent; } }
        }
        public partial class AckDataWriteVariable : KaitaiStruct
        {
            public static AckDataWriteVariable FromFile(string fileName)
            {
                return new AckDataWriteVariable(new KaitaiStream(fileName));
            }

            public AckDataWriteVariable(KaitaiStream p__io, S7commPacket.AckDataMessage p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _itemCount = m_io.ReadU1();
                _items = new List<ReturnCode>((int) (ItemCount));
                for (var i = 0; i < ItemCount; i++)
                {
                    _items.Add(((S7commPacket.ReturnCode) m_io.ReadU1()));
                }
            }
            private byte _itemCount;
            private List<ReturnCode> _items;
            private S7commPacket m_root;
            private S7commPacket.AckDataMessage m_parent;
            public byte ItemCount { get { return _itemCount; } }
            public List<ReturnCode> Items { get { return _items; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket.AckDataMessage M_Parent { get { return m_parent; } }
        }
        public partial class JobWriteVariable : KaitaiStruct
        {
            public static JobWriteVariable FromFile(string fileName)
            {
                return new JobWriteVariable(new KaitaiStream(fileName));
            }

            public JobWriteVariable(KaitaiStream p__io, S7commPacket.JobRequestMessage p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _itemCount = m_io.ReadU1();
                _items = new List<ParameterItem>((int) (ItemCount));
                for (var i = 0; i < ItemCount; i++)
                {
                    _items.Add(new ParameterItem(m_io, this, m_root));
                }
                _data = new List<DataItem>((int) (ItemCount));
                for (var i = 0; i < ItemCount; i++)
                {
                    _data.Add(new DataItem(((ItemCount - i) - 1), m_io, this, m_root));
                }
            }
            private byte _itemCount;
            private List<ParameterItem> _items;
            private List<DataItem> _data;
            private S7commPacket m_root;
            private S7commPacket.JobRequestMessage m_parent;
            public byte ItemCount { get { return _itemCount; } }
            public List<ParameterItem> Items { get { return _items; } }
            public List<DataItem> Data { get { return _data; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket.JobRequestMessage M_Parent { get { return m_parent; } }
        }
        public partial class AckDataReadVariable : KaitaiStruct
        {
            public static AckDataReadVariable FromFile(string fileName)
            {
                return new AckDataReadVariable(new KaitaiStream(fileName));
            }

            public AckDataReadVariable(KaitaiStream p__io, S7commPacket.AckDataMessage p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _itemCount = m_io.ReadU1();
                _data = new List<DataItem>((int) (ItemCount));
                for (var i = 0; i < ItemCount; i++)
                {
                    _data.Add(new DataItem(((ItemCount - i) - 1), m_io, this, m_root));
                }
            }
            private byte _itemCount;
            private List<DataItem> _data;
            private S7commPacket m_root;
            private S7commPacket.AckDataMessage m_parent;
            public byte ItemCount { get { return _itemCount; } }
            public List<DataItem> Data { get { return _data; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket.AckDataMessage M_Parent { get { return m_parent; } }
        }
        public partial class DataItem : KaitaiStruct
        {
            public DataItem(int p_remaining, KaitaiStream p__io, KaitaiStruct p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _remaining = p_remaining;
                f_dataLength = false;
                _read();
            }
            private void _read()
            {
                _returnCode = ((S7commPacket.ReturnCode) m_io.ReadU1());
                _dataType = m_io.ReadU1();
                _transportSize = m_io.ReadU2be();
                _data = m_io.ReadBytes(DataLength);
                if ( ((KaitaiStream.Mod(DataLength, 2) == 1) && (Remaining > 0)) ) {
                    _fillByte = m_io.ReadBytes(1);
                }
            }
            private bool f_dataLength;
            private int _dataLength;

            /// <summary>
            /// If data type is 3,4,5 then length gives number of bits. As #3 can represent unaligned bits we need to round it to to bytes.
            /// </summary>
            public int DataLength
            {
                get
                {
                    if (f_dataLength)
                        return _dataLength;
                    _dataLength = (int) (( ((DataType == 3) || (DataType == 4) || (DataType == 5))  ? ((TransportSize + 7) / 8) : TransportSize));
                    f_dataLength = true;
                    return _dataLength;
                }
            }
            private ReturnCode _returnCode;
            private byte _dataType;
            private ushort _transportSize;
            private byte[] _data;
            private byte[] _fillByte;
            private int _remaining;
            private S7commPacket m_root;
            private KaitaiStruct m_parent;
            public ReturnCode ReturnCode { get { return _returnCode; } }
            public byte DataType { get { return _dataType; } }
            public ushort TransportSize { get { return _transportSize; } }
            public byte[] Data { get { return _data; } }
            public byte[] FillByte { get { return _fillByte; } }
            public int Remaining { get { return _remaining; } }
            public S7commPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class JobOther : KaitaiStruct
        {
            public static JobOther FromFile(string fileName)
            {
                return new JobOther(new KaitaiStream(fileName));
            }

            public JobOther(KaitaiStream p__io, S7commPacket.JobRequestMessage p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _parameters = m_io.ReadBytes((M_Root.ParametersLength - 1));
                _data = m_io.ReadBytes(M_Root.DataLength);
            }
            private byte[] _parameters;
            private byte[] _data;
            private S7commPacket m_root;
            private S7commPacket.JobRequestMessage m_parent;
            public byte[] Parameters { get { return _parameters; } }
            public byte[] Data { get { return _data; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket.JobRequestMessage M_Parent { get { return m_parent; } }
        }
        public partial class JobReadVariable : KaitaiStruct
        {
            public static JobReadVariable FromFile(string fileName)
            {
                return new JobReadVariable(new KaitaiStream(fileName));
            }

            public JobReadVariable(KaitaiStream p__io, S7commPacket.JobRequestMessage p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _itemCount = m_io.ReadU1();
                _items = new List<ParameterItem>((int) (ItemCount));
                for (var i = 0; i < ItemCount; i++)
                {
                    _items.Add(new ParameterItem(m_io, this, m_root));
                }
            }
            private byte _itemCount;
            private List<ParameterItem> _items;
            private S7commPacket m_root;
            private S7commPacket.JobRequestMessage m_parent;
            public byte ItemCount { get { return _itemCount; } }
            public List<ParameterItem> Items { get { return _items; } }
            public S7commPacket M_Root { get { return m_root; } }
            public S7commPacket.JobRequestMessage M_Parent { get { return m_parent; } }
        }
        public partial class ParameterItem : KaitaiStruct
        {
            public static ParameterItem FromFile(string fileName)
            {
                return new ParameterItem(new KaitaiStream(fileName));
            }

            public ParameterItem(KaitaiStream p__io, KaitaiStruct p__parent = null, S7commPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _parameterType = m_io.ReadU1();
                _parameterLength = m_io.ReadU1();
                _syntaxId = m_io.ReadU1();
                _transportSize = ((S7commPacket.TransportSizeType) m_io.ReadU1());
                _length = m_io.ReadU2be();
                _dbNumber = m_io.ReadU2be();
                _area = ((S7commPacket.AreaCode) m_io.ReadU1());
                _address = m_io.ReadBytes(3);
            }
            private byte _parameterType;
            private byte _parameterLength;
            private byte _syntaxId;
            private TransportSizeType _transportSize;
            private ushort _length;
            private ushort _dbNumber;
            private AreaCode _area;
            private byte[] _address;
            private S7commPacket m_root;
            private KaitaiStruct m_parent;
            public byte ParameterType { get { return _parameterType; } }
            public byte ParameterLength { get { return _parameterLength; } }
            public byte SyntaxId { get { return _syntaxId; } }
            public TransportSizeType TransportSize { get { return _transportSize; } }
            public ushort Length { get { return _length; } }
            public ushort DbNumber { get { return _dbNumber; } }
            public AreaCode Area { get { return _area; } }
            public byte[] Address { get { return _address; } }
            public S7commPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        private byte _protcolId;
        private S7MessageType _messageType;
        private ushort _reserved;
        private ushort _pduReference;
        private ushort _parametersLength;
        private ushort _dataLength;
        private ResponseError _error;
        private KaitaiStruct _message;
        private S7commPacket m_root;
        private KaitaiStruct m_parent;
        private byte[] __raw_message;
        public byte ProtcolId { get { return _protcolId; } }
        public S7MessageType MessageType { get { return _messageType; } }
        public ushort Reserved { get { return _reserved; } }
        public ushort PduReference { get { return _pduReference; } }
        public ushort ParametersLength { get { return _parametersLength; } }
        public ushort DataLength { get { return _dataLength; } }
        public ResponseError Error { get { return _error; } }
        public KaitaiStruct Message { get { return _message; } }
        public S7commPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
        public byte[] M_RawMessage { get { return __raw_message; } }
    }
}
