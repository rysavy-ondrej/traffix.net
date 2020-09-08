// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Core
{
    public partial class DhcpPacket : KaitaiStruct
    {
        public static DhcpPacket FromFile(string fileName)
        {
            return new DhcpPacket(new KaitaiStream(fileName));
        }

        public enum DhcpOpcode
        {
            Request = 1,
            Reply = 2,
        }

        public enum DhcpHardwareType
        {
            Ethernet = 1,
            ExperimentalEthernet = 2,
            AmateurRadio = 3,
            ProteonPronetTokenRing = 4,
            Chaos = 5,
            Ieee802 = 6,
            Arcnet = 7,
            Hyperchannel = 8,
            Lanstar = 9,
            AutonetShortAddress = 10,
            Localtalk = 11,
            Localnet = 12,
            UltraLink = 13,
            Smds = 14,
            FrameRelay = 15,
            Atm16 = 16,
            Hdlc = 17,
            FibreChannel = 18,
            Atm19 = 19,
            SerialLine = 20,
            Atm21 = 21,
            MilStd188220 = 22,
            Metricom = 23,
            Ieee1394p1995 = 24,
            Mapos = 25,
            Twinaxial = 26,
            Eui64 = 27,
            Hiparp = 28,
            IpAndArpOverIso = 29,
            Arpsec = 30,
            IpsecTunnel = 31,
            Infiniband = 32,
            Cai = 33,
        }

        public enum DhcpMessageType
        {
            Discover = 1,
            Offer = 2,
            Request = 3,
            Decline = 4,
            Ack = 5,
            Nak = 6,
            Release = 7,
            Inform = 8,
        }

        public enum DhcpOptionCode
        {
            Pad = 0,
            SubnetMask = 1,
            TimeOffset = 2,
            Router = 3,
            TimeServer = 4,
            NameServer = 5,
            DomainNameServer = 6,
            LogServer = 7,
            CookieServer = 8,
            LprServer = 9,
            ImpressServer = 10,
            ResourceLocationServer = 11,
            HostName = 12,
            BootFileSize = 13,
            MeritDumpFile = 14,
            DomainName = 15,
            SwapServer = 16,
            RootPath = 17,
            ExtensionsPath = 18,
            IpForwardingEnableDisable = 19,
            NonLocalSourceRoutingEnableDisable = 20,
            PolicyFilter = 21,
            MaximumDatagramReassemblySize = 22,
            DefaultIpTimeToLive = 23,
            PathMtuAgingTimeout = 24,
            PathMtuPlateauTable = 25,
            InterfaceMtu = 26,
            AllSubnetsAreLocal = 27,
            BroadcastAddress = 28,
            PerformMaskDiscovery = 29,
            MaskSupplier = 30,
            PerformRouterDiscovery = 31,
            RouterSolicitationAddress = 32,
            StaticRoute = 33,
            TrailerEncapsulation = 34,
            ArpCacheTimeout = 35,
            EthernetEncapsulation = 36,
            TcpDefaultTtl = 37,
            TcpKeepaliveInterval = 38,
            TcpKeepaliveGarbage = 39,
            NetworkInformationServiceDomain40 = 40,
            NetworkInformationServers = 41,
            NetworkTimeProtocolServers = 42,
            VendorSpecificInformation = 43,
            NetbiosOverTcpIpNameServer = 44,
            NetbiosOverTcpIpDatagramDistributionServer = 45,
            NetbiosOverTcpIpNodeType = 46,
            NetbiosOverTcpIpScope = 47,
            XWindowSystemFontServer = 48,
            XWindowSystemDisplayManager = 49,
            RequestedIpAddress = 50,
            IpAddressLeaseTime = 51,
            OptionOverload = 52,
            DhcpMessageType = 53,
            ServerIdentifier = 54,
            ParameterRequestList = 55,
            Message = 56,
            MaximumDhcpMessageSize = 57,
            RenewalT1TimeValue = 58,
            RebindingT2TimeValue = 59,
            ClassIdentifier = 60,
            ClientIdentifier = 61,
            NetwareIpDomainName = 62,
            NetwareIpInformation = 63,
            NetworkInformationServiceDomain64 = 64,
            NetworkInformationServiceServers = 65,
            TftpServerName = 66,
            BootfileName = 67,
            MobileIpHomeAgent = 68,
            SimpleMailTransportProtocolServer = 69,
            PostOfficeProtocolServer = 70,
            NetworkNewsTransportProtocolServer = 71,
            DefaultWorldWideWebServer = 72,
            DefaultFingerServer = 73,
            DefaultInternetRelayChatServer = 74,
            StreettalkServer = 75,
            StreettalkDirectoryAssistanceServer = 76,
            UserClassInformation = 77,
            SlpDirectoryAgent = 78,
            SlpServiceScope = 79,
            FullyQualifiedDomainName = 81,
            AgentCircuitId = 82,
            NdsServers = 85,
            NdsTreeName = 86,
            NdsContext = 87,
            BcmcsDomainName = 88,
            BcmcsServerAddress = 89,
            Authentication = 90,
            ClientSystem = 93,
            ClientNetworkDeviceInterface = 94,
            LightweightDirectoryAccessProtocol = 95,
            UuidGuidBasedClientIdentifier = 97,
            OpenGroupsUserAuthentication = 98,
            OverallFormat = 99,
            AutonomousSystemNumber = 109,
            NetinfoParentServerAddress = 112,
            NetinfoParentServerTag = 113,
            Url = 114,
            AutoConfigure = 116,
            NameServiceSearch = 117,
            SubnetSelection = 118,
            DnsDomainSearchList = 119,
            SipServersDhcpOption = 120,
            ClasslessStaticRouteOption = 121,
            CablelabsClientConfiguration = 122,
            Geoconf = 123,
            VendorIdentifyingVendorClass = 124,
            VendorIdentifyingVendorSpecific = 125,
            Extension126 = 126,
            Extension127 = 127,
            ClasslessStaticRoute = 249,
            ContinuationOption = 250,
            WebProxyAutoDetectionWpad = 252,
            EndOfOptions = 255,
        }

        public DhcpPacket(KaitaiStream io, KaitaiStruct parent = null, DhcpPacket root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _opCode = ((DhcpOpcode)m_io.ReadU1());
            _hardwareType = ((DhcpHardwareType)m_io.ReadU1());
            _hardwareAddressLength = m_io.ReadU1();
            _hopCount = m_io.ReadU1();
            _traqnsactionId = m_io.ReadU4be();
            _seconds = m_io.ReadU2be();
            _flags = new DhcpFlags(m_io, this, m_root);
            _clientIp = m_io.ReadBytes(4);
            _yourIp = m_io.ReadBytes(4);
            _serverIp = m_io.ReadBytes(4);
            _relayAgentIp = m_io.ReadBytes(4);
            _clientHardwareAddress = m_io.ReadBytes(16);
            _serverHostName = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(0, false, true, true));
            _bootFileName = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesTerm(0, false, true, true));
            _magicCookie = m_io.ReadBytes(4);
            _options = new List<DhcpOption>();
            while (!m_io.IsEof)
            {
                _options.Add(new DhcpOption(m_io, this, m_root));
            }
        }
        public partial class DhcpFlags : KaitaiStruct
        {
            public static DhcpFlags FromFile(string fileName)
            {
                return new DhcpFlags(new KaitaiStream(fileName));
            }

            public DhcpFlags(KaitaiStream io, DhcpPacket parent = null, DhcpPacket root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _broadcast = m_io.ReadBitsInt(1) != 0;
                _reserved = m_io.ReadBitsInt(15);
            }
            private bool _broadcast;
            private ulong _reserved;
            private DhcpPacket m_root;
            private DhcpPacket m_parent;
            public bool Broadcast { get { return _broadcast; } }
            public ulong Reserved { get { return _reserved; } }
            public DhcpPacket M_Root { get { return m_root; } }
            public DhcpPacket M_Parent { get { return m_parent; } }
        }
        public partial class DhcpOption : KaitaiStruct
        {
            public static DhcpOption FromFile(string fileName)
            {
                return new DhcpOption(new KaitaiStream(fileName));
            }

            public DhcpOption(KaitaiStream io, DhcpPacket parent = null, DhcpPacket root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _code = ((DhcpPacket.DhcpOptionCode)m_io.ReadU1());
                _len = m_io.ReadU1();
                _body = m_io.ReadBytes(Len);
            }
            private DhcpOptionCode _code;
            private byte _len;
            private byte[] _body;
            private DhcpPacket m_root;
            private DhcpPacket m_parent;
            public DhcpOptionCode Code { get { return _code; } }
            public byte Len { get { return _len; } }
            public byte[] Body { get { return _body; } }
            public DhcpPacket M_Root { get { return m_root; } }
            public DhcpPacket M_Parent { get { return m_parent; } }
        }
        private DhcpOpcode _opCode;
        private DhcpHardwareType _hardwareType;
        private byte _hardwareAddressLength;
        private byte _hopCount;
        private uint _traqnsactionId;
        private ushort _seconds;
        private DhcpFlags _flags;
        private byte[] _clientIp;
        private byte[] _yourIp;
        private byte[] _serverIp;
        private byte[] _relayAgentIp;
        private byte[] _clientHardwareAddress;
        private string _serverHostName;
        private string _bootFileName;
        private byte[] _magicCookie;
        private List<DhcpOption> _options;
        private DhcpPacket m_root;
        private KaitaiStruct m_parent;
        public DhcpOpcode OpCode { get { return _opCode; } }
        public DhcpHardwareType HardwareType { get { return _hardwareType; } }
        public byte HardwareAddressLength { get { return _hardwareAddressLength; } }
        public byte HopCount { get { return _hopCount; } }
        public uint TraqnsactionId { get { return _traqnsactionId; } }
        public ushort Seconds { get { return _seconds; } }
        public DhcpFlags Flags { get { return _flags; } }
        public byte[] ClientIp { get { return _clientIp; } }
        public byte[] YourIp { get { return _yourIp; } }
        public byte[] ServerIp { get { return _serverIp; } }
        public byte[] RelayAgentIp { get { return _relayAgentIp; } }
        public byte[] ClientHardwareAddress { get { return _clientHardwareAddress; } }
        public string ServerHostName { get { return _serverHostName; } }
        public string BootFileName { get { return _bootFileName; } }
        public byte[] MagicCookie { get { return _magicCookie; } }
        public List<DhcpOption> Options { get { return _options; } }
        public DhcpPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
