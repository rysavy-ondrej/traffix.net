// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Industrial
{

    /// <summary>
    /// A simplified DNP3 parser. It only extracts information up to the application function code 
    /// which needs to be located wihtin the first data chunk. It ignores objects and does not
    /// process other fragments/chunks. 
    /// </summary>
    public partial class Dnp3Packet : KaitaiStruct
    {
        public static Dnp3Packet FromFile(string fileName)
        {
            return new Dnp3Packet(new KaitaiStream(fileName));
        }


        public enum Dnp3Direction
        {
            FromOutstation = 0,
            FromMaster = 1,
        }

        public enum Dnp3Primary
        {
            SecToPri = 0,
            PriToSec = 1,
        }

        public enum Dnp3FunctionCode
        {
            Dnp3Confirm = 0,
            Dnp3Read = 1,
            Dnp3Write = 2,
            Dnp3Select = 3,
            Dnp3Operate = 4,
            Dnp3DirOperate = 5,
            Dnp3DirOperateNoResp = 6,
            Dnp3Freeze = 7,
            Dnp3FreezeNoResp = 8,
            Dnp3FreezeClear = 9,
            Dnp3FreezeClearNoResp = 10,
            Dnp3FreezeAtTime = 11,
            Dnp3FreezeAtTimeNoResp = 12,
            Dnp3ColdRestart = 13,
            Dnp3WarmRestart = 14,
            Dnp3InitializeData = 15,
            Dnp3InitializeApplication = 16,
            Dnp3StartApplication = 17,
            Dnp3StopApplication = 18,
            Dnp3SaveConfiguration = 19,
            Dnp3EnableUnsolicited = 20,
            Dnp3DisableUnsolicited = 21,
            Dnp3AssignClass = 22,
            Dnp3DelayMeasurement = 23,
            Dnp3RecordCurrentTime = 24,
            Dnp3OpenFile = 25,
            Dnp3CloseFile = 26,
            Dnp3DeleteFile = 27,
            Dnp3GetFileInformation = 28,
            Dnp3AuthenticateFile = 29,
            Dnp3AbortFile = 30,
            Dnp3Response = 129,
            Dnp3UnsolicitedResponse = 130,
        }
        public Dnp3Packet(KaitaiStream p__io, KaitaiStruct p__parent = null, Dnp3Packet p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            f_transportControl = false;
            _read();
        }
        private void _read()
        {
            _frameHeader = new Dnp3FrameHeader(m_io, this, m_root);
            if ((TransportControl & 64) == 64) {
                __raw_firstChunk = m_io.ReadBytes(((M_Io.Pos + 18) <= M_Io.Size ? 18 : (M_Io.Size - M_Io.Pos)));
                var io___raw_firstChunk = new KaitaiStream(__raw_firstChunk);
                _firstChunk = new Dnp3FirstChunk(io___raw_firstChunk, this, m_root);
            }
            _nextChunks = new List<Dnp3Chunk>();
            {
                var i = 0;
                while (!m_io.IsEof) {
                    _nextChunks.Add(new Dnp3Chunk(m_io, this, m_root));
                    i++;
                }
            }
        }
        public partial class Dnp3InternalIndications : KaitaiStruct
        {
            public static Dnp3InternalIndications FromFile(string fileName)
            {
                return new Dnp3InternalIndications(new KaitaiStream(fileName));
            }

            public Dnp3InternalIndications(KaitaiStream p__io, Dnp3Packet.Dnp3Application p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _deviceRestart = m_io.ReadBitsInt(1) != 0;
                _deviceTrouble = m_io.ReadBitsInt(1) != 0;
                _localControl = m_io.ReadBitsInt(1) != 0;
                _needTime = m_io.ReadBitsInt(1) != 0;
                _class3Event = m_io.ReadBitsInt(1) != 0;
                _class2Event = m_io.ReadBitsInt(1) != 0;
                _class1Event = m_io.ReadBitsInt(1) != 0;
                _allStations = m_io.ReadBitsInt(1) != 0;
                _reserved = m_io.ReadBitsInt(2);
                _configurationCorrupt = m_io.ReadBitsInt(1) != 0;
                _alreadyExecuting = m_io.ReadBitsInt(1) != 0;
                _eventBufferOverflow = m_io.ReadBitsInt(1) != 0;
                _parameterError = m_io.ReadBitsInt(1) != 0;
                _objectUnknown = m_io.ReadBitsInt(1) != 0;
                _functionNotSupported = m_io.ReadBitsInt(1) != 0;
            }
            private bool _deviceRestart;
            private bool _deviceTrouble;
            private bool _localControl;
            private bool _needTime;
            private bool _class3Event;
            private bool _class2Event;
            private bool _class1Event;
            private bool _allStations;
            private ulong _reserved;
            private bool _configurationCorrupt;
            private bool _alreadyExecuting;
            private bool _eventBufferOverflow;
            private bool _parameterError;
            private bool _objectUnknown;
            private bool _functionNotSupported;
            private Dnp3Packet m_root;
            private Dnp3Packet.Dnp3Application m_parent;
            public bool DeviceRestart { get { return _deviceRestart; } }
            public bool DeviceTrouble { get { return _deviceTrouble; } }
            public bool LocalControl { get { return _localControl; } }
            public bool NeedTime { get { return _needTime; } }
            public bool Class3Event { get { return _class3Event; } }
            public bool Class2Event { get { return _class2Event; } }
            public bool Class1Event { get { return _class1Event; } }
            public bool AllStations { get { return _allStations; } }
            public ulong Reserved { get { return _reserved; } }
            public bool ConfigurationCorrupt { get { return _configurationCorrupt; } }
            public bool AlreadyExecuting { get { return _alreadyExecuting; } }
            public bool EventBufferOverflow { get { return _eventBufferOverflow; } }
            public bool ParameterError { get { return _parameterError; } }
            public bool ObjectUnknown { get { return _objectUnknown; } }
            public bool FunctionNotSupported { get { return _functionNotSupported; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet.Dnp3Application M_Parent { get { return m_parent; } }
        }
        public partial class Dnp3TransportControl : KaitaiStruct
        {
            public static Dnp3TransportControl FromFile(string fileName)
            {
                return new Dnp3TransportControl(new KaitaiStream(fileName));
            }

            public Dnp3TransportControl(KaitaiStream p__io, Dnp3Packet.Dnp3FirstChunk p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _final = m_io.ReadBitsInt(1) != 0;
                _first = m_io.ReadBitsInt(1) != 0;
                _sequence = m_io.ReadBitsInt(6);
            }
            private bool _final;
            private bool _first;
            private ulong _sequence;
            private Dnp3Packet m_root;
            private Dnp3Packet.Dnp3FirstChunk m_parent;
            public bool Final { get { return _final; } }
            public bool First { get { return _first; } }
            public ulong Sequence { get { return _sequence; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet.Dnp3FirstChunk M_Parent { get { return m_parent; } }
        }
        public partial class Dnp3ApplicationControl : KaitaiStruct
        {
            public static Dnp3ApplicationControl FromFile(string fileName)
            {
                return new Dnp3ApplicationControl(new KaitaiStream(fileName));
            }

            public Dnp3ApplicationControl(KaitaiStream p__io, Dnp3Packet.Dnp3Application p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _fir = m_io.ReadBitsInt(1) != 0;
                _fin = m_io.ReadBitsInt(1) != 0;
                _con = m_io.ReadBitsInt(1) != 0;
                _uns = m_io.ReadBitsInt(1) != 0;
                _seq = m_io.ReadBitsInt(4);
            }
            private bool _fir;
            private bool _fin;
            private bool _con;
            private bool _uns;
            private ulong _seq;
            private Dnp3Packet m_root;
            private Dnp3Packet.Dnp3Application m_parent;
            public bool Fir { get { return _fir; } }
            public bool Fin { get { return _fin; } }
            public bool Con { get { return _con; } }
            public bool Uns { get { return _uns; } }
            public ulong Seq { get { return _seq; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet.Dnp3Application M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Contains application control and mainly function code of DNP3 message.
        /// </summary>
        public partial class Dnp3Application : KaitaiStruct
        {
            public static Dnp3Application FromFile(string fileName)
            {
                return new Dnp3Application(new KaitaiStream(fileName));
            }

            public Dnp3Application(KaitaiStream p__io, Dnp3Packet.Dnp3FirstChunk p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _applicationControl = new Dnp3ApplicationControl(m_io, this, m_root);
                _functionCode = ((Dnp3Packet.Dnp3FunctionCode) m_io.ReadU1());
                if ( ((FunctionCode == Dnp3Packet.Dnp3FunctionCode.Dnp3Response) || (FunctionCode == Dnp3Packet.Dnp3FunctionCode.Dnp3UnsolicitedResponse)) ) {
                    _internalIndication = new Dnp3InternalIndications(m_io, this, m_root);
                }
            }
            private Dnp3ApplicationControl _applicationControl;
            private Dnp3FunctionCode _functionCode;
            private Dnp3InternalIndications _internalIndication;
            private Dnp3Packet m_root;
            private Dnp3Packet.Dnp3FirstChunk m_parent;
            public Dnp3ApplicationControl ApplicationControl { get { return _applicationControl; } }
            public Dnp3FunctionCode FunctionCode { get { return _functionCode; } }
            public Dnp3InternalIndications InternalIndication { get { return _internalIndication; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet.Dnp3FirstChunk M_Parent { get { return m_parent; } }
        }
        public partial class Dnp3FrameHeaderControl : KaitaiStruct
        {
            public static Dnp3FrameHeaderControl FromFile(string fileName)
            {
                return new Dnp3FrameHeaderControl(new KaitaiStream(fileName));
            }

            public Dnp3FrameHeaderControl(KaitaiStream p__io, Dnp3Packet.Dnp3FrameHeader p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _direction = ((Dnp3Packet.Dnp3Direction) m_io.ReadBitsInt(1));
                _primary = ((Dnp3Packet.Dnp3Primary) m_io.ReadBitsInt(1));
                _frameCountBit = m_io.ReadBitsInt(1) != 0;
                _frameCountValie = m_io.ReadBitsInt(1) != 0;
                _controlFunctionCode = m_io.ReadBitsInt(4);
            }
            private Dnp3Direction _direction;
            private Dnp3Primary _primary;
            private bool _frameCountBit;
            private bool _frameCountValie;
            private ulong _controlFunctionCode;
            private Dnp3Packet m_root;
            private Dnp3Packet.Dnp3FrameHeader m_parent;
            public Dnp3Direction Direction { get { return _direction; } }
            public Dnp3Primary Primary { get { return _primary; } }
            public bool FrameCountBit { get { return _frameCountBit; } }
            public bool FrameCountValie { get { return _frameCountValie; } }
            public ulong ControlFunctionCode { get { return _controlFunctionCode; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet.Dnp3FrameHeader M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Represents a DNP3 chunk. The chunk always consists of up to 16 data bytes
        /// and checksum value.  
        /// </summary>
        public partial class Dnp3Chunk : KaitaiStruct
        {
            public static Dnp3Chunk FromFile(string fileName)
            {
                return new Dnp3Chunk(new KaitaiStream(fileName));
            }

            public Dnp3Chunk(KaitaiStream p__io, Dnp3Packet p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _data = m_io.ReadBytes(((M_Io.Pos + 18) <= M_Io.Size ? 16 : ((M_Io.Size - M_Io.Pos) - 2)));
                _checksum = m_io.ReadU2be();
            }
            private byte[] _data;
            private ushort _checksum;
            private Dnp3Packet m_root;
            private Dnp3Packet m_parent;
            public byte[] Data { get { return _data; } }
            public ushort Checksum { get { return _checksum; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Represents DNP3 link layer header.
        /// </summary>
        public partial class Dnp3FrameHeader : KaitaiStruct
        {
            public static Dnp3FrameHeader FromFile(string fileName)
            {
                return new Dnp3FrameHeader(new KaitaiStream(fileName));
            }

            public Dnp3FrameHeader(KaitaiStream p__io, Dnp3Packet p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _sync = m_io.EnsureFixedContents(new byte[] { 5, 100 });
                _length = m_io.ReadU1();
                _control = new Dnp3FrameHeaderControl(m_io, this, m_root);
                _destination = m_io.ReadU2be();
                _source = m_io.ReadU2be();
                _checksum = m_io.ReadU2be();
            }
            private byte[] _sync;
            private byte _length;
            private Dnp3FrameHeaderControl _control;
            private ushort _destination;
            private ushort _source;
            private ushort _checksum;
            private Dnp3Packet m_root;
            private Dnp3Packet m_parent;
            public byte[] Sync { get { return _sync; } }
            public byte Length { get { return _length; } }
            public Dnp3FrameHeaderControl Control { get { return _control; } }
            public ushort Destination { get { return _destination; } }
            public ushort Source { get { return _source; } }
            public ushort Checksum { get { return _checksum; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Represents a DNP3 chunk. The chunk always consists of up to 16 data bytes
        /// and checksum value.
        /// </summary>
        public partial class Dnp3FirstChunk : KaitaiStruct
        {
            public static Dnp3FirstChunk FromFile(string fileName)
            {
                return new Dnp3FirstChunk(new KaitaiStream(fileName));
            }

            public Dnp3FirstChunk(KaitaiStream p__io, Dnp3Packet p__parent = null, Dnp3Packet p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _transportControl = new Dnp3TransportControl(m_io, this, m_root);
                _application = new Dnp3Application(m_io, this, m_root);
                _data = m_io.ReadBytes((M_Io.Size - (M_Io.Pos + 2)));
                _checksum = m_io.ReadU2be();
            }
            private Dnp3TransportControl _transportControl;
            private Dnp3Application _application;
            private byte[] _data;
            private ushort _checksum;
            private Dnp3Packet m_root;
            private Dnp3Packet m_parent;
            public Dnp3TransportControl TransportControl { get { return _transportControl; } }
            public Dnp3Application Application { get { return _application; } }
            public byte[] Data { get { return _data; } }
            public ushort Checksum { get { return _checksum; } }
            public Dnp3Packet M_Root { get { return m_root; } }
            public Dnp3Packet M_Parent { get { return m_parent; } }
        }
        private bool f_transportControl;
        private byte _transportControl;

        /// <summary>
        /// Provides transport control byte. We need to determine transport control 
        /// in advance to decide if we have the first chunk or not.
        /// </summary>
        public byte TransportControl
        {
            get
            {
                if (f_transportControl)
                    return _transportControl;
                long _pos = m_io.Pos;
                m_io.Seek(10);
                _transportControl = m_io.ReadU1();
                m_io.Seek(_pos);
                f_transportControl = true;
                return _transportControl;
            }
        }
        private Dnp3FrameHeader _frameHeader;
        private Dnp3FirstChunk _firstChunk;
        private List<Dnp3Chunk> _nextChunks;
        private Dnp3Packet m_root;
        private KaitaiStruct m_parent;
        private byte[] __raw_firstChunk;
        public Dnp3FrameHeader FrameHeader { get { return _frameHeader; } }
        public Dnp3FirstChunk FirstChunk { get { return _firstChunk; } }
        public List<Dnp3Chunk> NextChunks { get { return _nextChunks; } }
        public Dnp3Packet M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
        public byte[] M_RawFirstChunk { get { return __raw_firstChunk; } }
    }
}
