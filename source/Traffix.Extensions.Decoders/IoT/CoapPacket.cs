// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.IoT
{

    /// <summary>
    /// The Constrained Application Protocol (CoAP) is a specialized web transfer protocol for use with constrained 
    /// nodes and constrained networks in the Internet of Things. 
    /// The protocol is designed for machine-to-machine (M2M) applications such as smart energy and building automation.
    /// 
    /// More information can be found at: http://coap.technology/
    /// and https://tools.ietf.org/html/rfc7252
    /// </summary>
    public partial class CoapPacket : KaitaiStruct
    {
        public static CoapPacket FromFile(string fileName)
        {
            return new CoapPacket(new KaitaiStream(fileName));
        }


        public enum CoapMessageType
        {
            Confirmable = 0,
            NonComfirmanble = 1,
            Acknowledgement = 2,
            Reset = 3,
        }

        public enum CoapCode
        {
            Empty = 0,
            Get = 1,
            Post = 2,
            Put = 3,
            Delete = 4,
            Created = 65,
            Deleted = 66,
            Valid = 67,
            Changed = 68,
            Content = 69,
            BadRequest = 128,
            Unathorized = 129,
            BadOption = 130,
            Forbidden = 131,
            NotFound = 132,
            MethodNotAllowed = 133,
            NotAcceptable = 134,
            PreconditionFailed = 140,
            RequestEntityTooLarge = 141,
            UnsupportedContentFormat = 143,
            InternalServerError = 160,
            NotImplemented = 161,
            BadGateway = 162,
            ServiceUnavailable = 163,
            GatewayTimeout = 164,
            ProxyingNotSupported = 165,
        }

        public enum CoapOptions
        {
            IfMatch = 1,
            UriHost = 3,
            Etag = 4,
            IfNoneMatch = 5,
            UriPort = 7,
            LocationPath = 8,
            UriPath = 11,
            ContentFormat = 12,
            MaxAge = 14,
            UriQuery = 15,
            Accept = 17,
            LocationQuery = 20,
            ProxyUri = 35,
            ProxyScheme = 39,
            Size1 = 60,
        }
        public CoapPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, CoapPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _version = m_io.ReadBitsInt(2);
            _type = ((CoapMessageType)m_io.ReadBitsInt(2));
            _tkl = m_io.ReadBitsInt(4);
            m_io.AlignToByte();
            _code = ((CoapCode)m_io.ReadU1());
            _messageId = m_io.ReadU2be();
            _token = m_io.ReadBytes(Tkl);
            if (M_Io.IsEof == false)
            {
                _options = new List<Option>();
                {
                    var i = 0;
                    Option M_;
                    do
                    {
                        M_ = new Option(m_io, this, m_root);
                        _options.Add(M_);
                        i++;
                    } while (!(((M_.IsPayloadMarker) || (M_Io.IsEof))));
                }
            }
            _body = m_io.ReadBytesFull();
        }

        /// <summary>
        /// Each option instance in a message specifies the Option Number of the
        /// defined CoAP option, the length of the Option Value, and the Option
        /// Value itself. Option nunber is expressed as delta.
        /// Both option length and delta values uses packing. Option is represented as 
        /// 4 bits for regular values from 0-12. Values 13 and 14 informs that 
        /// option length is provided in extra bytes. The same holds for delta. 
        /// </summary>
        public partial class Option : KaitaiStruct
        {
            public static Option FromFile(string fileName)
            {
                return new Option(new KaitaiStream(fileName));
            }

            public Option(KaitaiStream p__io, CoapPacket p__parent = null, CoapPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_delta = false;
                f_length = false;
                f_isPayloadMarker = false;
                _read();
            }
            private void _read()
            {
                _optDelta = m_io.ReadBitsInt(4);
                _optLen = m_io.ReadBitsInt(4);
                m_io.AlignToByte();
                if (OptDelta == 13)
                {
                    _optDeltaExtraU1 = m_io.ReadU1();
                }
                if (OptDelta == 14)
                {
                    _optDeltaExtraU2 = m_io.ReadU2be();
                }
                if (OptLen == 13)
                {
                    _optLenExtraU1 = m_io.ReadU1();
                }
                if (OptLen == 14)
                {
                    _optLenExtraU2 = m_io.ReadU2be();
                }
                _value = m_io.ReadBytes(Length);
            }
            private bool f_delta;
            private int _delta;
            public int Delta
            {
                get
                {
                    if (f_delta)
                        return _delta;
                    _delta = (int)((OptDelta == 13 ? (OptDeltaExtraU1 + 13) : (OptDelta == 14 ? (OptDeltaExtraU2 + 269) : (OptDelta == 15 ? 0 : OptDelta))));
                    f_delta = true;
                    return _delta;
                }
            }
            private bool f_length;
            private int _length;
            public int Length
            {
                get
                {
                    if (f_length)
                        return _length;
                    _length = (int)((OptLen == 13 ? (OptLenExtraU1 + 13) : (OptLen == 14 ? (OptLenExtraU2 + 269) : (OptLen == 15 ? 0 : OptLen))));
                    f_length = true;
                    return _length;
                }
            }
            private bool f_isPayloadMarker;
            private bool _isPayloadMarker;
            public bool IsPayloadMarker
            {
                get
                {
                    if (f_isPayloadMarker)
                        return _isPayloadMarker;
                    _isPayloadMarker = (bool)(((OptLen == 15) && (OptDelta == 15)));
                    f_isPayloadMarker = true;
                    return _isPayloadMarker;
                }
            }
            private ulong _optDelta;
            private ulong _optLen;
            private byte? _optDeltaExtraU1;
            private ushort? _optDeltaExtraU2;
            private byte? _optLenExtraU1;
            private ushort? _optLenExtraU2;
            private byte[] _value;
            private CoapPacket m_root;
            private CoapPacket m_parent;
            public int OptDelta { get { return (int)_optDelta; } }
            public int OptLen { get { return (int)_optLen; } }
            public byte? OptDeltaExtraU1 { get { return _optDeltaExtraU1; } }
            public ushort? OptDeltaExtraU2 { get { return _optDeltaExtraU2; } }
            public byte? OptLenExtraU1 { get { return _optLenExtraU1; } }
            public ushort? OptLenExtraU2 { get { return _optLenExtraU2; } }
            public byte[] Value { get { return _value; } }
            public CoapPacket M_Root { get { return m_root; } }
            public CoapPacket M_Parent { get { return m_parent; } }
        }
        private ulong _version;
        private CoapMessageType _type;
        private ulong _tkl;
        private CoapCode _code;
        private ushort _messageId;
        private byte[] _token;
        private List<Option> _options;
        private byte[] _body;
        private CoapPacket m_root;
        private KaitaiStruct m_parent;
        public ulong Version { get { return _version; } }
        public CoapMessageType Type { get { return _type; } }
        public ulong Tkl { get { return _tkl; } }
        public CoapCode Code { get { return _code; } }
        public ushort MessageId { get { return _messageId; } }
        public byte[] Token { get { return _token; } }
        public List<Option> Options { get { return _options; } }
        public byte[] Body { get { return _body; } }
        public CoapPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
