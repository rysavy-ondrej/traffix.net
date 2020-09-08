// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Base
{
    public partial class LlcFrame : KaitaiStruct
    {
        public static LlcFrame FromFile(string fileName)
        {
            return new LlcFrame(new KaitaiStream(fileName));
        }

        public enum LlcSframeCommand
        {
            ReceiveReady = 0,
            Reject = 1,
            ReceiveNotReady = 2,
            UnknownCommand = 3,
        }

        public enum LlcAddressTable
        {
            NullLsap = 0,
            LlcSubLayerManagement = 2,
            IbmSnaPathControl = 4,
            TcpIpdodInternetProtocol = 6,
            IbmSna8 = 8,
            IbmSnaC = 12,
            ProwayIec955NetworkManagementAndInitialization = 14,
            Netware = 16,
            IsoNetworkLayerOslan1 = 20,
            TexasInstruments = 24,
            IsoNetworkLayer20 = 32,
            DgX25 = 50,
            IsoNetworkLayer34 = 52,
            IbmSna40 = 64,
            SpanningTreeBpdu = 66,
            EiaRs511ManufacturingMessageService = 78,
            IsoNetworkLayerOslan2 = 84,
            IsiIp = 94,
            Iso8208X25Over8022 = 126,
            Xns = 128,
            Bacnet = 130,
            Nestar = 134,
            ProwayIec955ActiveStationListMaintenance = 142,
            Arp = 152,
            SnapsubNetworkAccessProtocol = 170,
            BanyanVinesBa = 186,
            BanyanVinesBc = 188,
            IbmResourceManagement = 212,
            IbmDynamicAddressResolutionnameManagement = 220,
            IpxNovellNetware = 224,
            IbmNetbios = 240,
            IbmNetManagement = 244,
            IrplibmRemoteProgramLoadF8 = 248,
            UngermannBass = 250,
            IrplibmRemoteProgramLoadFc = 252,
            IsoNetworkLayerProtocols = 254,
        }

        public enum ControlFrameType
        {
            Iframe = 0,
            Sframe = 1,
            Uframe = 2,
        }

        public enum AddressTypeEnum
        {
            IndividualAddress = 0,
            GroupAddress = 1,
        }

        public enum FrameTypeEnum
        {
            CommandFrame = 0,
            ResponseFrame = 1,
        }

        public LlcFrame(KaitaiStream io, KaitaiStruct parent = null, LlcFrame root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _dsapAddress = m_io.ReadBitsInt(7);
            _dsapType = ((AddressTypeEnum)m_io.ReadBitsInt(1));
            _ssapAddress = m_io.ReadBitsInt(7);
            _frameType = ((FrameTypeEnum)m_io.ReadBitsInt(1));
            m_io.AlignToByte();
            _control = new ControlType(m_io, this, m_root);
            _data = m_io.ReadBytesFull();
        }
        public partial class ControlType : KaitaiStruct
        {
            public static ControlType FromFile(string fileName)
            {
                return new ControlType(new KaitaiStream(fileName));
            }

            public ControlType(KaitaiStream io, LlcFrame parent = null, LlcFrame root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_controlByte = false;
                f_type = false;
                switch (Type)
                {
                    case LlcFrame.ControlFrameType.Iframe:
                        {
                            _frameContent = new Information(m_io, this, m_root);
                            break;
                        }
                    case LlcFrame.ControlFrameType.Sframe:
                        {
                            _frameContent = new Supervisory(m_io, this, m_root);
                            break;
                        }
                    case LlcFrame.ControlFrameType.Uframe:
                        {
                            _frameContent = new Unnumbered(m_io, this, m_root);
                            break;
                        }
                }
            }
            private bool f_controlByte;
            private byte _controlByte;
            public byte ControlByte
            {
                get
                {
                    if (f_controlByte)
                        return _controlByte;
                    long _pos = m_io.Pos;
                    m_io.Seek(0);
                    _controlByte = m_io.ReadU1();
                    m_io.Seek(_pos);
                    f_controlByte = true;
                    return _controlByte;
                }
            }
            private bool f_type;
            private ControlFrameType _type;
            public ControlFrameType Type
            {
                get
                {
                    if (f_type)
                        return _type;
                    _type = (ControlFrameType)(((LlcFrame.ControlFrameType)((ControlByte & 1) == 0 ? 0 : (ControlByte & 3))));
                    f_type = true;
                    return _type;
                }
            }
            private KaitaiStruct _frameContent;
            private LlcFrame m_root;
            private LlcFrame m_parent;
            public KaitaiStruct FrameContent { get { return _frameContent; } }
            public LlcFrame M_Root { get { return m_root; } }
            public LlcFrame M_Parent { get { return m_parent; } }
        }
        public partial class Information : KaitaiStruct
        {
            public static Information FromFile(string fileName)
            {
                return new Information(new KaitaiStream(fileName));
            }

            public Information(KaitaiStream io, ControlType parent = null, LlcFrame root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _tsSequenceNumber = m_io.ReadBitsInt(7);
                _type = m_io.ReadBitsInt(1) != 0;
                _trSequenceNumber = m_io.ReadBitsInt(7);
                _pollFinal = m_io.ReadBitsInt(1) != 0;
            }
            private ulong _tsSequenceNumber;
            private bool _type;
            private ulong _trSequenceNumber;
            private bool _pollFinal;
            private LlcFrame m_root;
            private LlcFrame.ControlType m_parent;
            public ulong TsSequenceNumber { get { return _tsSequenceNumber; } }
            public bool Type { get { return _type; } }
            public ulong TrSequenceNumber { get { return _trSequenceNumber; } }
            public bool PollFinal { get { return _pollFinal; } }
            public LlcFrame M_Root { get { return m_root; } }
            public LlcFrame.ControlType M_Parent { get { return m_parent; } }
        }
        public partial class Supervisory : KaitaiStruct
        {
            public static Supervisory FromFile(string fileName)
            {
                return new Supervisory(new KaitaiStream(fileName));
            }

            public Supervisory(KaitaiStream io, ControlType parent = null, LlcFrame root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _reserved = m_io.ReadBitsInt(4);
                _command = ((LlcFrame.LlcSframeCommand)m_io.ReadBitsInt(2));
                _type = m_io.ReadBitsInt(2);
                _trSequenceNumber = m_io.ReadBitsInt(7);
                _pollFinal = m_io.ReadBitsInt(1) != 0;
            }
            private ulong _reserved;
            private LlcSframeCommand _command;
            private ulong _type;
            private ulong _trSequenceNumber;
            private bool _pollFinal;
            private LlcFrame m_root;
            private LlcFrame.ControlType m_parent;
            public ulong Reserved { get { return _reserved; } }
            public LlcSframeCommand Command { get { return _command; } }
            public ulong Type { get { return _type; } }
            public ulong TrSequenceNumber { get { return _trSequenceNumber; } }
            public bool PollFinal { get { return _pollFinal; } }
            public LlcFrame M_Root { get { return m_root; } }
            public LlcFrame.ControlType M_Parent { get { return m_parent; } }
        }
        public partial class Unnumbered : KaitaiStruct
        {
            public static Unnumbered FromFile(string fileName)
            {
                return new Unnumbered(new KaitaiStream(fileName));
            }

            public Unnumbered(KaitaiStream io, ControlType parent = null, LlcFrame root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _mmm = m_io.ReadBitsInt(3);
                _pollFinal = m_io.ReadBitsInt(1) != 0;
                _mm = m_io.ReadBitsInt(2);
                _type = m_io.ReadBitsInt(2);
            }
            private ulong _mmm;
            private bool _pollFinal;
            private ulong _mm;
            private ulong _type;
            private LlcFrame m_root;
            private LlcFrame.ControlType m_parent;
            public ulong Mmm { get { return _mmm; } }
            public bool PollFinal { get { return _pollFinal; } }
            public ulong Mm { get { return _mm; } }
            public ulong Type { get { return _type; } }
            public LlcFrame M_Root { get { return m_root; } }
            public LlcFrame.ControlType M_Parent { get { return m_parent; } }
        }
        private ulong _dsapAddress;
        private AddressTypeEnum _dsapType;
        private ulong _ssapAddress;
        private FrameTypeEnum _frameType;
        private ControlType _control;
        private byte[] _data;
        private LlcFrame m_root;
        private KaitaiStruct m_parent;
        public ulong DsapAddress { get { return _dsapAddress; } }
        public AddressTypeEnum DsapType { get { return _dsapType; } }
        public ulong SsapAddress { get { return _ssapAddress; } }
        public FrameTypeEnum FrameType { get { return _frameType; } }
        public ControlType Control { get { return _control; } }
        public byte[] Data { get { return _data; } }
        public LlcFrame M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
