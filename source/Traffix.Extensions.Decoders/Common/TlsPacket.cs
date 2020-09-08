// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Common
{
    public partial class TlsPacket : KaitaiStruct
    {
        public static TlsPacket FromFile(string fileName)
        {
            return new TlsPacket(new KaitaiStream(fileName));
        }


        public enum TlsContentType
        {
            ChangeCipherSpec = 20,
            Alert = 21,
            Handshake = 22,
            ApplicationData = 23,
        }

        public enum TlsHandshakeType
        {
            HelloRequest = 0,
            ClientHello = 1,
            ServerHello = 2,
            NewSessionTicket = 4,
            Certificate = 11,
            ServerKeyExchange = 12,
            CertificateRequest = 13,
            ServerHelloDone = 14,
            CertificateVerify = 15,
            ClientKeyExchange = 16,
            Finished = 20,
            KeyUpdate = 24,
        }

        public enum CompressionMethods
        {
            NullCompression = 0,
            Deflate = 1,
        }
        public TlsPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _contentType = ((TlsContentType) m_io.ReadU1());
            _version = m_io.ReadU2be();
            _length = m_io.ReadU2be();
            switch (ContentType) {
            case TlsContentType.Handshake: {
                __raw_record = m_io.ReadBytes(Length);
                var io___raw_record = new KaitaiStream(__raw_record);
                _record = new TlsHandshakeProtocol(io___raw_record, this, m_root);
                break;
            }
            case TlsContentType.ApplicationData: {
                __raw_record = m_io.ReadBytes(Length);
                var io___raw_record = new KaitaiStream(__raw_record);
                _record = new TlsApplicationData(io___raw_record, this, m_root);
                break;
            }
            case TlsContentType.ChangeCipherSpec: {
                __raw_record = m_io.ReadBytes(Length);
                var io___raw_record = new KaitaiStream(__raw_record);
                _record = new TlsChangeCipherSpec(io___raw_record, this, m_root);
                break;
            }
            case TlsContentType.Alert: {
                __raw_record = m_io.ReadBytes(Length);
                var io___raw_record = new KaitaiStream(__raw_record);
                _record = new TlsEncryptedMessage(io___raw_record, this, m_root);
                break;
            }
            default: {
                __raw_record = m_io.ReadBytes(Length);
                var io___raw_record = new KaitaiStream(__raw_record);
                _record = new TlsEncryptedMessage(io___raw_record, this, m_root);
                break;
            }
            }
        }
        public partial class ServerName : KaitaiStruct
        {
            public static ServerName FromFile(string fileName)
            {
                return new ServerName(new KaitaiStream(fileName));
            }

            public ServerName(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _nameType = m_io.ReadU1();
                _length = m_io.ReadU2be();
                _hostName = m_io.ReadBytes(Length);
            }
            private byte _nameType;
            private ushort _length;
            private byte[] _hostName;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public byte NameType { get { return _nameType; } }
            public ushort Length { get { return _length; } }
            public byte[] HostName { get { return _hostName; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class Random : KaitaiStruct
        {
            public static Random FromFile(string fileName)
            {
                return new Random(new KaitaiStream(fileName));
            }

            public Random(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _randomTime = m_io.ReadBytes(4);
                _randomBytes = m_io.ReadBytes(28);
            }
            private byte[] _randomTime;
            private byte[] _randomBytes;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public byte[] RandomTime { get { return _randomTime; } }
            public byte[] RandomBytes { get { return _randomBytes; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class TlsCertificateRequest : KaitaiStruct
        {
            public static TlsCertificateRequest FromFile(string fileName)
            {
                return new TlsCertificateRequest(new KaitaiStream(fileName));
            }

            public TlsCertificateRequest(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _request = m_io.ReadBytesFull();
            }
            private byte[] _request;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public byte[] Request { get { return _request; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class TlsCertificate : KaitaiStruct
        {
            public static TlsCertificate FromFile(string fileName)
            {
                return new TlsCertificate(new KaitaiStream(fileName));
            }

            public TlsCertificate(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _certLength = new TlsLength(m_io, this, m_root);
                __raw_certificates = new List<byte[]>();
                _certificates = new List<Certificate>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        __raw_certificates.Add(m_io.ReadBytes(CertLength.Value));
                        var io___raw_certificates = new KaitaiStream(__raw_certificates[__raw_certificates.Count - 1]);
                        _certificates.Add(new Certificate(io___raw_certificates, this, m_root));
                        i++;
                    }
                }
            }
            private TlsLength _certLength;
            private List<Certificate> _certificates;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            private List<byte[]> __raw_certificates;
            public TlsLength CertLength { get { return _certLength; } }
            public List<Certificate> Certificates { get { return _certificates; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
            public List<byte[]> M_RawCertificates { get { return __raw_certificates; } }
        }
        public partial class Certificate : KaitaiStruct
        {
            public static Certificate FromFile(string fileName)
            {
                return new Certificate(new KaitaiStream(fileName));
            }

            public Certificate(KaitaiStream p__io, TlsPacket.TlsCertificate p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _certLength = new TlsLength(m_io, this, m_root);
                _body = m_io.ReadBytes(CertLength.Value);
            }
            private TlsLength _certLength;
            private byte[] _body;
            private TlsPacket m_root;
            private TlsPacket.TlsCertificate m_parent;
            public TlsLength CertLength { get { return _certLength; } }
            public byte[] Body { get { return _body; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsCertificate M_Parent { get { return m_parent; } }
        }
        public partial class SessionId : KaitaiStruct
        {
            public static SessionId FromFile(string fileName)
            {
                return new SessionId(new KaitaiStream(fileName));
            }

            public SessionId(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _len = m_io.ReadU1();
                _sid = m_io.ReadBytes(Len);
            }
            private byte _len;
            private byte[] _sid;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public byte Len { get { return _len; } }
            public byte[] Sid { get { return _sid; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class Sni : KaitaiStruct
        {
            public static Sni FromFile(string fileName)
            {
                return new Sni(new KaitaiStream(fileName));
            }

            public Sni(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _listLength = m_io.ReadU2be();
                _serverNames = new List<ServerName>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _serverNames.Add(new ServerName(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private ushort _listLength;
            private List<ServerName> _serverNames;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public ushort ListLength { get { return _listLength; } }
            public List<ServerName> ServerNames { get { return _serverNames; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class TlsServerHello : KaitaiStruct
        {
            public static TlsServerHello FromFile(string fileName)
            {
                return new TlsServerHello(new KaitaiStream(fileName));
            }

            public TlsServerHello(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _version = m_io.ReadU2be();
                _random = new Random(m_io, this, m_root);
                _sessionId = new SessionId(m_io, this, m_root);
                _cipherSuite = new CipherSuite(m_io, this, m_root);
                _compressionMethod = ((TlsPacket.CompressionMethods) m_io.ReadU1());
                if (M_Io.Size > M_Io.Pos) {
                    _extensions = new TlsExtensions(m_io, this, m_root);
                }
            }
            private ushort _version;
            private Random _random;
            private SessionId _sessionId;
            private CipherSuite _cipherSuite;
            private CompressionMethods _compressionMethod;
            private TlsExtensions _extensions;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public ushort Version { get { return _version; } }
            public Random Random { get { return _random; } }
            public SessionId SessionId { get { return _sessionId; } }
            public CipherSuite CipherSuite { get { return _cipherSuite; } }
            public CompressionMethods CompressionMethod { get { return _compressionMethod; } }
            public TlsExtensions Extensions { get { return _extensions; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class CipherSuites : KaitaiStruct
        {
            public static CipherSuites FromFile(string fileName)
            {
                return new CipherSuites(new KaitaiStream(fileName));
            }

            public CipherSuites(KaitaiStream p__io, TlsPacket.TlsClientHello p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _length = m_io.ReadU2be();
                _items = new List<ushort>((int) ((Length / 2)));
                for (var i = 0; i < (Length / 2); i++)
                {
                    _items.Add(m_io.ReadU2be());
                }
            }
            private ushort _length;
            private List<ushort> _items;
            private TlsPacket m_root;
            private TlsPacket.TlsClientHello m_parent;
            public ushort Length { get { return _length; } }
            public List<ushort> Items { get { return _items; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsClientHello M_Parent { get { return m_parent; } }
        }
        public partial class TlsExtension : KaitaiStruct
        {
            public static TlsExtension FromFile(string fileName)
            {
                return new TlsExtension(new KaitaiStream(fileName));
            }

            public TlsExtension(KaitaiStream p__io, TlsPacket.TlsExtensionsList p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _type = m_io.ReadU2be();
                _length = m_io.ReadU2be();
                _body = m_io.ReadBytes(Length);
            }
            private ushort _type;
            private ushort _length;
            private byte[] _body;
            private TlsPacket m_root;
            private TlsPacket.TlsExtensionsList m_parent;
            public ushort Type { get { return _type; } }
            public ushort Length { get { return _length; } }
            public byte[] Body { get { return _body; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsExtensionsList M_Parent { get { return m_parent; } }
        }
        public partial class TlsExtensions : KaitaiStruct
        {
            public static TlsExtensions FromFile(string fileName)
            {
                return new TlsExtensions(new KaitaiStream(fileName));
            }

            public TlsExtensions(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _extensionsLength = m_io.ReadU2be();
                __raw_extensionsList = m_io.ReadBytes(ExtensionsLength);
                var io___raw_extensionsList = new KaitaiStream(__raw_extensionsList);
                _extensionsList = new TlsExtensionsList(io___raw_extensionsList, this, m_root);
            }
            private ushort _extensionsLength;
            private TlsExtensionsList _extensionsList;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            private byte[] __raw_extensionsList;
            public ushort ExtensionsLength { get { return _extensionsLength; } }
            public TlsExtensionsList ExtensionsList { get { return _extensionsList; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
            public byte[] M_RawExtensionsList { get { return __raw_extensionsList; } }
        }
        public partial class TlsClientKeyExchange : KaitaiStruct
        {
            public static TlsClientKeyExchange FromFile(string fileName)
            {
                return new TlsClientKeyExchange(new KaitaiStream(fileName));
            }

            public TlsClientKeyExchange(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _exchangeKeys = m_io.ReadBytesFull();
            }
            private byte[] _exchangeKeys;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public byte[] ExchangeKeys { get { return _exchangeKeys; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class TlsChangeCipherSpec : KaitaiStruct
        {
            public static TlsChangeCipherSpec FromFile(string fileName)
            {
                return new TlsChangeCipherSpec(new KaitaiStream(fileName));
            }

            public TlsChangeCipherSpec(KaitaiStream p__io, TlsPacket p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _changeMessage = m_io.ReadBytesFull();
            }
            private byte[] _changeMessage;
            private TlsPacket m_root;
            private TlsPacket m_parent;
            public byte[] ChangeMessage { get { return _changeMessage; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket M_Parent { get { return m_parent; } }
        }
        public partial class TlsCertificateVerify : KaitaiStruct
        {
            public static TlsCertificateVerify FromFile(string fileName)
            {
                return new TlsCertificateVerify(new KaitaiStream(fileName));
            }

            public TlsCertificateVerify(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _signedHandshakeMessage = m_io.ReadBytesFull();
            }
            private byte[] _signedHandshakeMessage;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public byte[] SignedHandshakeMessage { get { return _signedHandshakeMessage; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class Alpn : KaitaiStruct
        {
            public static Alpn FromFile(string fileName)
            {
                return new Alpn(new KaitaiStream(fileName));
            }

            public Alpn(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _extLen = m_io.ReadU2be();
                _alpnProtocols = new List<Protocol>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _alpnProtocols.Add(new Protocol(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private ushort _extLen;
            private List<Protocol> _alpnProtocols;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public ushort ExtLen { get { return _extLen; } }
            public List<Protocol> AlpnProtocols { get { return _alpnProtocols; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class TlsPreMasterSecret : KaitaiStruct
        {
            public static TlsPreMasterSecret FromFile(string fileName)
            {
                return new TlsPreMasterSecret(new KaitaiStream(fileName));
            }

            public TlsPreMasterSecret(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _secretLength = m_io.ReadU2be();
                _secret = m_io.ReadBytes(SecretLength);
            }
            private ushort _secretLength;
            private byte[] _secret;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public ushort SecretLength { get { return _secretLength; } }
            public byte[] Secret { get { return _secret; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class TlsServerKeyExchange : KaitaiStruct
        {
            public static TlsServerKeyExchange FromFile(string fileName)
            {
                return new TlsServerKeyExchange(new KaitaiStream(fileName));
            }

            public TlsServerKeyExchange(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _serverKeyExchange = m_io.ReadBytesFull();
            }
            private byte[] _serverKeyExchange;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public byte[] ServerKeyExchange { get { return _serverKeyExchange; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class TlsApplicationData : KaitaiStruct
        {
            public static TlsApplicationData FromFile(string fileName)
            {
                return new TlsApplicationData(new KaitaiStream(fileName));
            }

            public TlsApplicationData(KaitaiStream p__io, TlsPacket p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _body = m_io.ReadBytesFull();
            }
            private byte[] _body;
            private TlsPacket m_root;
            private TlsPacket m_parent;
            public byte[] Body { get { return _body; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket M_Parent { get { return m_parent; } }
        }
        public partial class TlsClientHello : KaitaiStruct
        {
            public static TlsClientHello FromFile(string fileName)
            {
                return new TlsClientHello(new KaitaiStream(fileName));
            }

            public TlsClientHello(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _version = m_io.ReadU2be();
                _random = new Random(m_io, this, m_root);
                _sessionId = new SessionId(m_io, this, m_root);
                _cipherSuites = new CipherSuites(m_io, this, m_root);
                _compressionMethodsLength = m_io.ReadU1();
                _compressionMethods = new List<CompressionMethods>((int) (CompressionMethodsLength));
                for (var i = 0; i < CompressionMethodsLength; i++)
                {
                    _compressionMethods.Add(((TlsPacket.CompressionMethods) m_io.ReadU1()));
                }
                if (M_Io.Size > M_Io.Pos) {
                    _extensions = new TlsExtensions(m_io, this, m_root);
                }
            }
            private ushort _version;
            private Random _random;
            private SessionId _sessionId;
            private CipherSuites _cipherSuites;
            private byte _compressionMethodsLength;
            private List<CompressionMethods> _compressionMethods;
            private TlsExtensions _extensions;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public ushort Version { get { return _version; } }
            public Random Random { get { return _random; } }
            public SessionId SessionId { get { return _sessionId; } }
            public CipherSuites CipherSuites { get { return _cipherSuites; } }
            public byte CompressionMethodsLength { get { return _compressionMethodsLength; } }
            public List<CompressionMethods> CompressionMethods { get { return _compressionMethods; } }
            public TlsExtensions Extensions { get { return _extensions; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class TlsServerHelloDone : KaitaiStruct
        {
            public static TlsServerHelloDone FromFile(string fileName)
            {
                return new TlsServerHelloDone(new KaitaiStream(fileName));
            }

            public TlsServerHelloDone(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _empty = m_io.ReadBytes(0);
            }
            private byte[] _empty;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public byte[] Empty { get { return _empty; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class TlsHelloRequest : KaitaiStruct
        {
            public static TlsHelloRequest FromFile(string fileName)
            {
                return new TlsHelloRequest(new KaitaiStream(fileName));
            }

            public TlsHelloRequest(KaitaiStream p__io, TlsPacket.TlsHandshake p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _empty = m_io.ReadBytes(0);
            }
            private byte[] _empty;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshake m_parent;
            public byte[] Empty { get { return _empty; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshake M_Parent { get { return m_parent; } }
        }
        public partial class TlsEncryptedMessage : KaitaiStruct
        {
            public static TlsEncryptedMessage FromFile(string fileName)
            {
                return new TlsEncryptedMessage(new KaitaiStream(fileName));
            }

            public TlsEncryptedMessage(KaitaiStream p__io, TlsPacket p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _encryptedMessage = m_io.ReadBytesFull();
            }
            private byte[] _encryptedMessage;
            private TlsPacket m_root;
            private TlsPacket m_parent;
            public byte[] EncryptedMessage { get { return _encryptedMessage; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket M_Parent { get { return m_parent; } }
        }
        public partial class TlsHandshakeProtocol : KaitaiStruct
        {
            public static TlsHandshakeProtocol FromFile(string fileName)
            {
                return new TlsHandshakeProtocol(new KaitaiStream(fileName));
            }

            public TlsHandshakeProtocol(KaitaiStream p__io, TlsPacket p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _handshakeMessages = new List<TlsHandshake>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _handshakeMessages.Add(new TlsHandshake(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private List<TlsHandshake> _handshakeMessages;
            private TlsPacket m_root;
            private TlsPacket m_parent;
            public List<TlsHandshake> HandshakeMessages { get { return _handshakeMessages; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket M_Parent { get { return m_parent; } }
        }
        public partial class CipherSuite : KaitaiStruct
        {
            public static CipherSuite FromFile(string fileName)
            {
                return new CipherSuite(new KaitaiStream(fileName));
            }

            public CipherSuite(KaitaiStream p__io, TlsPacket.TlsServerHello p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _cipherId = m_io.ReadU2be();
            }
            private ushort _cipherId;
            private TlsPacket m_root;
            private TlsPacket.TlsServerHello m_parent;
            public ushort CipherId { get { return _cipherId; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsServerHello M_Parent { get { return m_parent; } }
        }
        public partial class TlsHandshake : KaitaiStruct
        {
            public static TlsHandshake FromFile(string fileName)
            {
                return new TlsHandshake(new KaitaiStream(fileName));
            }

            public TlsHandshake(KaitaiStream p__io, TlsPacket.TlsHandshakeProtocol p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _msgType = ((TlsPacket.TlsHandshakeType) m_io.ReadU1());
                _length = new TlsLength(m_io, this, m_root);
                switch (MsgType) {
                case TlsPacket.TlsHandshakeType.HelloRequest: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsHelloRequest(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.Certificate: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsCertificate(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.CertificateVerify: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsCertificateVerify(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.ServerKeyExchange: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsServerKeyExchange(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.ClientHello: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsClientHello(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.ClientKeyExchange: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsClientKeyExchange(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.ServerHello: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsServerHello(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.CertificateRequest: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsCertificateRequest(io___raw_body, this, m_root);
                    break;
                }
                case TlsPacket.TlsHandshakeType.ServerHelloDone: {
                    __raw_body = m_io.ReadBytes(Length.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new TlsServerHelloDone(io___raw_body, this, m_root);
                    break;
                }
                default: {
                    _body = m_io.ReadBytes(Length.Value);
                    break;
                }
                }
            }
            private TlsHandshakeType _msgType;
            private TlsLength _length;
            private object _body;
            private TlsPacket m_root;
            private TlsPacket.TlsHandshakeProtocol m_parent;
            private byte[] __raw_body;
            public TlsHandshakeType MsgType { get { return _msgType; } }
            public TlsLength Length { get { return _length; } }
            public object Body { get { return _body; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsHandshakeProtocol M_Parent { get { return m_parent; } }
            public byte[] M_RawBody { get { return __raw_body; } }
        }
        public partial class Protocol : KaitaiStruct
        {
            public static Protocol FromFile(string fileName)
            {
                return new Protocol(new KaitaiStream(fileName));
            }

            public Protocol(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _strlen = m_io.ReadU1();
                _name = m_io.ReadBytes(Strlen);
            }
            private byte _strlen;
            private byte[] _name;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public byte Strlen { get { return _strlen; } }
            public byte[] Name { get { return _name; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class TlsLength : KaitaiStruct
        {
            public static TlsLength FromFile(string fileName)
            {
                return new TlsLength(new KaitaiStream(fileName));
            }

            public TlsLength(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_value = false;
                _read();
            }
            private void _read()
            {
                _hlen = m_io.ReadU1();
                _llen = m_io.ReadU2be();
            }
            private bool f_value;
            private int _value;
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int) ((Llen + (Hlen << 16)));
                    f_value = true;
                    return _value;
                }
            }
            private byte _hlen;
            private ushort _llen;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public byte Hlen { get { return _hlen; } }
            public ushort Llen { get { return _llen; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class TlsExtensionsList : KaitaiStruct
        {
            public static TlsExtensionsList FromFile(string fileName)
            {
                return new TlsExtensionsList(new KaitaiStream(fileName));
            }

            public TlsExtensionsList(KaitaiStream p__io, TlsPacket.TlsExtensions p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _items = new List<TlsExtension>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _items.Add(new TlsExtension(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private List<TlsExtension> _items;
            private TlsPacket m_root;
            private TlsPacket.TlsExtensions m_parent;
            public List<TlsExtension> Items { get { return _items; } }
            public TlsPacket M_Root { get { return m_root; } }
            public TlsPacket.TlsExtensions M_Parent { get { return m_parent; } }
        }
        public partial class TlsFinished : KaitaiStruct
        {
            public static TlsFinished FromFile(string fileName)
            {
                return new TlsFinished(new KaitaiStream(fileName));
            }

            public TlsFinished(KaitaiStream p__io, KaitaiStruct p__parent = null, TlsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _handshakeMsgType = m_io.EnsureFixedContents(new byte[] { 22 });
                _version = m_io.ReadU2be();
                _length = m_io.ReadU2be();
                _finishedBytes = m_io.ReadBytes(Length);
            }
            private byte[] _handshakeMsgType;
            private ushort _version;
            private ushort _length;
            private byte[] _finishedBytes;
            private TlsPacket m_root;
            private KaitaiStruct m_parent;
            public byte[] HandshakeMsgType { get { return _handshakeMsgType; } }
            public ushort Version { get { return _version; } }
            public ushort Length { get { return _length; } }
            public byte[] FinishedBytes { get { return _finishedBytes; } }
            public TlsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        private TlsContentType _contentType;
        private ushort _version;
        private ushort _length;
        private KaitaiStruct _record;
        private TlsPacket m_root;
        private KaitaiStruct m_parent;
        private byte[] __raw_record;
        public TlsContentType ContentType { get { return _contentType; } }
        public ushort Version { get { return _version; } }
        public ushort Length { get { return _length; } }
        public KaitaiStruct Record { get { return _record; } }
        public TlsPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
        public byte[] M_RawRecord { get { return __raw_record; } }
    }
}
