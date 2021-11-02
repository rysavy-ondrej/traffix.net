// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Common
{
    public partial class Snmp : KaitaiStruct
    {
        public static Snmp FromFile(string fileName)
        {
            return new Snmp(new KaitaiStream(fileName));
        }


        public enum AsnTypeTag
        {
            EndOfContent = 0,
            Boolean = 1,
            Integer = 2,
            BitString = 3,
            OctetString = 4,
            NullValue = 5,
            ObjectId = 6,
            ObjectDescriptor = 7,
            External = 8,
            Real = 9,
            Enumerated = 10,
            EmbeddedPdv = 11,
            Utf8string = 12,
            RelativeOid = 13,
            Sequence10 = 16,
            PrintableString = 19,
            Ia5string = 22,
            Sequence30 = 48,
            Set = 49,
            SnmpPduGet = 160,
            SnmpPduGetnext = 161,
            SnmpPduResponse = 162,
            SnmpPduSet = 163,
            SnmpPduTrapv1 = 164,
            SnmpPduTrapv2 = 167,
        }

        public enum SnmpPduType
        {
            SnmpPduGet = 0,
            SnmpPduGetnext = 1,
            SnmpPduResponse = 2,
            SnmpPduSet = 3,
            SnmpPduTrapv1 = 4,
            SnmpPduTrapv2 = 7,
        }

        public enum SnmpErrorStatus
        {
            NoError = 0,
            TooBig = 1,
            NoSuchName = 2,
            BadValue = 3,
            ReadOnly = 4,
            GenErr = 5,
        }
        public Snmp(KaitaiStream p__io, KaitaiStruct p__parent = null, Snmp p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _hdr = new AsnHdr(m_io, this, m_root);
            _version = new AsnObj(m_io, this, m_root);
            _community = new AsnObj(m_io, this, m_root);
            _pduType = new AsnHdr(m_io, this, m_root);
            switch (PduType.Tag) {
            case AsnTypeTag.SnmpPduTrapv1: {
                _data = new Trap1(m_io, this, m_root);
                break;
            }
            case AsnTypeTag.SnmpPduTrapv2: {
                _data = new Trap2(m_io, this, m_root);
                break;
            }
            case AsnTypeTag.SnmpPduSet: {
                _data = new SetRequest(m_io, this, m_root);
                break;
            }
            case AsnTypeTag.SnmpPduResponse: {
                _data = new Response(m_io, this, m_root);
                break;
            }
            case AsnTypeTag.SnmpPduGet: {
                _data = new GetRequest(m_io, this, m_root);
                break;
            }
            case AsnTypeTag.SnmpPduGetnext: {
                _data = new GetNextRequest(m_io, this, m_root);
                break;
            }
            }
        }
        public partial class ErrorStatus : KaitaiStruct
        {
            public static ErrorStatus FromFile(string fileName)
            {
                return new ErrorStatus(new KaitaiStream(fileName));
            }

            public ErrorStatus(KaitaiStream p__io, Snmp.Pdu p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_code = false;
                _read();
            }
            private void _read()
            {
                _hdr = new AsnHdr(m_io, this, m_root);
                __raw_val = m_io.ReadBytes(Hdr.Len.Value);
                var io___raw_val = new KaitaiStream(__raw_val);
                _val = new BodyInteger(io___raw_val, this, m_root);
            }
            private bool f_code;
            private SnmpErrorStatus _code;
            public SnmpErrorStatus Code
            {
                get
                {
                    if (f_code)
                        return _code;
                    _code = (SnmpErrorStatus) (((Snmp.SnmpErrorStatus) Val.Value));
                    f_code = true;
                    return _code;
                }
            }
            private AsnHdr _hdr;
            private BodyInteger _val;
            private Snmp m_root;
            private Snmp.Pdu m_parent;
            private byte[] __raw_val;
            public AsnHdr Hdr { get { return _hdr; } }
            public BodyInteger Val { get { return _val; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp.Pdu M_Parent { get { return m_parent; } }
            public byte[] M_RawVal { get { return __raw_val; } }
        }
        public partial class AsnObj : KaitaiStruct
        {
            public static AsnObj FromFile(string fileName)
            {
                return new AsnObj(new KaitaiStream(fileName));
            }

            public AsnObj(KaitaiStream p__io, KaitaiStruct p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _hdr = new AsnHdr(m_io, this, m_root);
                switch (Hdr.Tag) {
                case Snmp.AsnTypeTag.Set: {
                    __raw_body = m_io.ReadBytes(Hdr.Len.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new BodySequence(io___raw_body, this, m_root);
                    break;
                }
                case Snmp.AsnTypeTag.OctetString: {
                    __raw_body = m_io.ReadBytes(Hdr.Len.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new BodyPrintableString(io___raw_body, this, m_root);
                    break;
                }
                case Snmp.AsnTypeTag.Utf8string: {
                    __raw_body = m_io.ReadBytes(Hdr.Len.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new BodyUtf8string(io___raw_body, this, m_root);
                    break;
                }
                case Snmp.AsnTypeTag.Sequence30: {
                    __raw_body = m_io.ReadBytes(Hdr.Len.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new BodySequence(io___raw_body, this, m_root);
                    break;
                }
                case Snmp.AsnTypeTag.Integer: {
                    __raw_body = m_io.ReadBytes(Hdr.Len.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new BodyInteger(io___raw_body, this, m_root);
                    break;
                }
                case Snmp.AsnTypeTag.Sequence10: {
                    __raw_body = m_io.ReadBytes(Hdr.Len.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new BodySequence(io___raw_body, this, m_root);
                    break;
                }
                case Snmp.AsnTypeTag.PrintableString: {
                    __raw_body = m_io.ReadBytes(Hdr.Len.Value);
                    var io___raw_body = new KaitaiStream(__raw_body);
                    _body = new BodyPrintableString(io___raw_body, this, m_root);
                    break;
                }
                default: {
                    _body = m_io.ReadBytes(Hdr.Len.Value);
                    break;
                }
                }
            }
            private AsnHdr _hdr;
            private object _body;
            private Snmp m_root;
            private KaitaiStruct m_parent;
            private byte[] __raw_body;
            public AsnHdr Hdr { get { return _hdr; } }
            public object Body { get { return _body; } }
            public Snmp M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
            public byte[] M_RawBody { get { return __raw_body; } }
        }
        public partial class VariableBindings : KaitaiStruct
        {
            public static VariableBindings FromFile(string fileName)
            {
                return new VariableBindings(new KaitaiStream(fileName));
            }

            public VariableBindings(KaitaiStream p__io, Snmp.Pdu p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _seqTypeTag = m_io.EnsureFixedContents(new byte[] { 48 });
                _len = new LenEncoded(m_io, this, m_root);
                _entries = new List<VariableBinding>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _entries.Add(new VariableBinding(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private byte[] _seqTypeTag;
            private LenEncoded _len;
            private List<VariableBinding> _entries;
            private Snmp m_root;
            private Snmp.Pdu m_parent;
            public byte[] SeqTypeTag { get { return _seqTypeTag; } }
            public LenEncoded Len { get { return _len; } }
            public List<VariableBinding> Entries { get { return _entries; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp.Pdu M_Parent { get { return m_parent; } }
        }
        public partial class BodySequence : KaitaiStruct
        {
            public static BodySequence FromFile(string fileName)
            {
                return new BodySequence(new KaitaiStream(fileName));
            }

            public BodySequence(KaitaiStream p__io, Snmp.AsnObj p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _entries = new List<AsnObj>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _entries.Add(new AsnObj(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private List<AsnObj> _entries;
            private Snmp m_root;
            private Snmp.AsnObj m_parent;
            public List<AsnObj> Entries { get { return _entries; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp.AsnObj M_Parent { get { return m_parent; } }
        }
        public partial class Trap1 : KaitaiStruct
        {
            public static Trap1 FromFile(string fileName)
            {
                return new Trap1(new KaitaiStream(fileName));
            }

            public Trap1(KaitaiStream p__io, Snmp p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _items = new List<AsnObj>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _items.Add(new AsnObj(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private List<AsnObj> _items;
            private Snmp m_root;
            private Snmp m_parent;
            public List<AsnObj> Items { get { return _items; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp M_Parent { get { return m_parent; } }
        }
        public partial class AsnHdr : KaitaiStruct
        {
            public static AsnHdr FromFile(string fileName)
            {
                return new AsnHdr(new KaitaiStream(fileName));
            }

            public AsnHdr(KaitaiStream p__io, KaitaiStruct p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _tag = ((Snmp.AsnTypeTag) m_io.ReadU1());
                _len = new LenEncoded(m_io, this, m_root);
            }
            private AsnTypeTag _tag;
            private LenEncoded _len;
            private Snmp m_root;
            private KaitaiStruct m_parent;
            public AsnTypeTag Tag { get { return _tag; } }
            public LenEncoded Len { get { return _len; } }
            public Snmp M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class BodyUtf8string : KaitaiStruct
        {
            public static BodyUtf8string FromFile(string fileName)
            {
                return new BodyUtf8string(new KaitaiStream(fileName));
            }

            public BodyUtf8string(KaitaiStream p__io, Snmp.AsnObj p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _value = System.Text.Encoding.GetEncoding("UTF-8").GetString(m_io.ReadBytesFull());
            }
            private string _value;
            private Snmp m_root;
            private Snmp.AsnObj m_parent;
            public string Value { get { return _value; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp.AsnObj M_Parent { get { return m_parent; } }
        }
        public partial class GetRequest : KaitaiStruct
        {
            public static GetRequest FromFile(string fileName)
            {
                return new GetRequest(new KaitaiStream(fileName));
            }

            public GetRequest(KaitaiStream p__io, Snmp p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _pdu = new Pdu(m_io, this, m_root);
            }
            private Pdu _pdu;
            private Snmp m_root;
            private Snmp m_parent;
            public Pdu Pdu { get { return _pdu; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp M_Parent { get { return m_parent; } }
        }
        public partial class Trap2 : KaitaiStruct
        {
            public static Trap2 FromFile(string fileName)
            {
                return new Trap2(new KaitaiStream(fileName));
            }

            public Trap2(KaitaiStream p__io, Snmp p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _requestId = new Pdu(m_io, this, m_root);
            }
            private Pdu _requestId;
            private Snmp m_root;
            private Snmp m_parent;
            public Pdu RequestId { get { return _requestId; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp M_Parent { get { return m_parent; } }
        }
        public partial class VariableBinding : KaitaiStruct
        {
            public static VariableBinding FromFile(string fileName)
            {
                return new VariableBinding(new KaitaiStream(fileName));
            }

            public VariableBinding(KaitaiStream p__io, Snmp.VariableBindings p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _seqTypeTag = m_io.EnsureFixedContents(new byte[] { 48 });
                _len = new LenEncoded(m_io, this, m_root);
                _name = new AsnObj(m_io, this, m_root);
                _value = new AsnObj(m_io, this, m_root);
            }
            private byte[] _seqTypeTag;
            private LenEncoded _len;
            private AsnObj _name;
            private AsnObj _value;
            private Snmp m_root;
            private Snmp.VariableBindings m_parent;
            public byte[] SeqTypeTag { get { return _seqTypeTag; } }
            public LenEncoded Len { get { return _len; } }
            public AsnObj Name { get { return _name; } }
            public AsnObj Value { get { return _value; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp.VariableBindings M_Parent { get { return m_parent; } }
        }
        public partial class BodyInteger : KaitaiStruct
        {
            public static BodyInteger FromFile(string fileName)
            {
                return new BodyInteger(new KaitaiStream(fileName));
            }

            public BodyInteger(KaitaiStream p__io, KaitaiStruct p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_value = false;
                _read();
            }
            private void _read()
            {
                _bytes = new List<byte>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _bytes.Add(m_io.ReadU1());
                        i++;
                    }
                }
            }
            private bool f_value;
            private int _value;

            /// <summary>
            /// Resulting value as normal integer
            /// </summary>
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int) ((((((((Bytes[(Bytes.Count - 1)] + ((Bytes.Count - 1) >= 2 ? (Bytes[((Bytes.Count - 1) - 1)] << 8) : 0)) + ((Bytes.Count - 1) >= 3 ? (Bytes[((Bytes.Count - 1) - 2)] << 16) : 0)) + ((Bytes.Count - 1) >= 4 ? (Bytes[((Bytes.Count - 1) - 3)] << 24) : 0)) + ((Bytes.Count - 1) >= 5 ? (Bytes[((Bytes.Count - 1) - 4)] << 32) : 0)) + ((Bytes.Count - 1) >= 6 ? (Bytes[((Bytes.Count - 1) - 5)] << 40) : 0)) + ((Bytes.Count - 1) >= 7 ? (Bytes[((Bytes.Count - 1) - 6)] << 48) : 0)) + ((Bytes.Count - 1) >= 8 ? (Bytes[((Bytes.Count - 1) - 7)] << 56) : 0)));
                    f_value = true;
                    return _value;
                }
            }
            private List<byte> _bytes;
            private Snmp m_root;
            private KaitaiStruct m_parent;
            public List<byte> Bytes { get { return _bytes; } }
            public Snmp M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class Response : KaitaiStruct
        {
            public static Response FromFile(string fileName)
            {
                return new Response(new KaitaiStream(fileName));
            }

            public Response(KaitaiStream p__io, Snmp p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _pdu = new Pdu(m_io, this, m_root);
            }
            private Pdu _pdu;
            private Snmp m_root;
            private Snmp m_parent;
            public Pdu Pdu { get { return _pdu; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp M_Parent { get { return m_parent; } }
        }
        public partial class Pdu : KaitaiStruct
        {
            public static Pdu FromFile(string fileName)
            {
                return new Pdu(new KaitaiStream(fileName));
            }

            public Pdu(KaitaiStream p__io, KaitaiStruct p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _requestId = new AsnObj(m_io, this, m_root);
                _errorStatus = new ErrorStatus(m_io, this, m_root);
                _errorIndex = new AsnObj(m_io, this, m_root);
                _variableBindings = new VariableBindings(m_io, this, m_root);
            }
            private AsnObj _requestId;
            private ErrorStatus _errorStatus;
            private AsnObj _errorIndex;
            private VariableBindings _variableBindings;
            private Snmp m_root;
            private KaitaiStruct m_parent;
            public AsnObj RequestId { get { return _requestId; } }
            public ErrorStatus ErrorStatus { get { return _errorStatus; } }
            public AsnObj ErrorIndex { get { return _errorIndex; } }
            public VariableBindings VariableBindings { get { return _variableBindings; } }
            public Snmp M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class GetNextRequest : KaitaiStruct
        {
            public static GetNextRequest FromFile(string fileName)
            {
                return new GetNextRequest(new KaitaiStream(fileName));
            }

            public GetNextRequest(KaitaiStream p__io, Snmp p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _pdu = new Pdu(m_io, this, m_root);
            }
            private Pdu _pdu;
            private Snmp m_root;
            private Snmp m_parent;
            public Pdu Pdu { get { return _pdu; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp M_Parent { get { return m_parent; } }
        }
        public partial class SetRequest : KaitaiStruct
        {
            public static SetRequest FromFile(string fileName)
            {
                return new SetRequest(new KaitaiStream(fileName));
            }

            public SetRequest(KaitaiStream p__io, Snmp p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _pdu = new Pdu(m_io, this, m_root);
            }
            private Pdu _pdu;
            private Snmp m_root;
            private Snmp m_parent;
            public Pdu Pdu { get { return _pdu; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp M_Parent { get { return m_parent; } }
        }
        public partial class LenEncoded : KaitaiStruct
        {
            public static LenEncoded FromFile(string fileName)
            {
                return new LenEncoded(new KaitaiStream(fileName));
            }

            public LenEncoded(KaitaiStream p__io, KaitaiStruct p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_value = false;
                _read();
            }
            private void _read()
            {
                _b1 = m_io.ReadU1();
                __raw_b2 = m_io.ReadBytes((B1 < 128 ? 0 : (B1 & 127)));
                var io___raw_b2 = new KaitaiStream(__raw_b2);
                _b2 = new BodyInteger(io___raw_b2, this, m_root);
            }
            private bool f_value;
            private int _value;
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int) ((B1 < 128 ? B1 : B2.Value));
                    f_value = true;
                    return _value;
                }
            }
            private byte _b1;
            private BodyInteger _b2;
            private Snmp m_root;
            private KaitaiStruct m_parent;
            private byte[] __raw_b2;
            public byte B1 { get { return _b1; } }
            public BodyInteger B2 { get { return _b2; } }
            public Snmp M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
            public byte[] M_RawB2 { get { return __raw_b2; } }
        }
        public partial class BodyPrintableString : KaitaiStruct
        {
            public static BodyPrintableString FromFile(string fileName)
            {
                return new BodyPrintableString(new KaitaiStream(fileName));
            }

            public BodyPrintableString(KaitaiStream p__io, Snmp.AsnObj p__parent = null, Snmp p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _value = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytesFull());
            }
            private string _value;
            private Snmp m_root;
            private Snmp.AsnObj m_parent;
            public string Value { get { return _value; } }
            public Snmp M_Root { get { return m_root; } }
            public Snmp.AsnObj M_Parent { get { return m_parent; } }
        }
        private AsnHdr _hdr;
        private AsnObj _version;
        private AsnObj _community;
        private AsnHdr _pduType;
        private KaitaiStruct _data;
        private Snmp m_root;
        private KaitaiStruct m_parent;
        public AsnHdr Hdr { get { return _hdr; } }
        public AsnObj Version { get { return _version; } }
        public AsnObj Community { get { return _community; } }
        public AsnHdr PduType { get { return _pduType; } }
        public KaitaiStruct Data { get { return _data; } }
        public Snmp M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
