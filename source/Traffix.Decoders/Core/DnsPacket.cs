// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Core
{

    /// <summary>
    /// Implements DNS packet decoder.
    /// </summary>
    public partial class DnsPacket : KaitaiStruct
    {
        public static DnsPacket FromFile(string fileName)
        {
            return new DnsPacket(new KaitaiStream(fileName));
        }


        public enum ClassType
        {
            InClass = 1,
            Cs = 2,
            Ch = 3,
            Hs = 4,
        }

        public enum RecordType
        {
            A = 1,
            Ns = 2,
            Md = 3,
            Mf = 4,
            Cname = 5,
            Soe = 6,
            Mb = 7,
            Mg = 8,
            Mr = 9,
            Null = 10,
            Wks = 11,
            Ptr = 12,
            Hinfo = 13,
            Minfo = 14,
            Mx = 15,
            Txt = 16,
            Aaaa = 28,
        }
        public DnsPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, DnsPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _transactionId = m_io.ReadU2be();
            _flags = new PacketFlags(m_io, this, m_root);
            _qdcount = m_io.ReadU2be();
            _ancount = m_io.ReadU2be();
            _nscount = m_io.ReadU2be();
            _arcount = m_io.ReadU2be();
            _queries = new List<Query>((int) (Qdcount));
            for (var i = 0; i < Qdcount; i++)
            {
                _queries.Add(new Query(m_io, this, m_root));
            }
            _answers = new List<Answer>((int) (Ancount));
            for (var i = 0; i < Ancount; i++)
            {
                _answers.Add(new Answer(m_io, this, m_root));
            }
        }
        public partial class PointerStruct : KaitaiStruct
        {
            public static PointerStruct FromFile(string fileName)
            {
                return new PointerStruct(new KaitaiStream(fileName));
            }

            public PointerStruct(KaitaiStream p__io, DnsPacket.Label p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_contents = false;
                _read();
            }
            private void _read()
            {
                _offset = m_io.ReadU1();
            }
            private bool f_contents;
            private DomainName _contents;
            public DomainName Contents
            {
                get
                {
                    if (f_contents)
                        return _contents;
                    KaitaiStream io = M_Root.M_Io;
                    long _pos = io.Pos;
                    io.Seek(Offset);
                    _contents = new DomainName(io, this, m_root);
                    io.Seek(_pos);
                    f_contents = true;
                    return _contents;
                }
            }
            private byte _offset;
            private DnsPacket m_root;
            private DnsPacket.Label m_parent;

            /// <summary>
            /// Read one byte, then offset to that position, read one domain-name and return
            /// </summary>
            public byte Offset { get { return _offset; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.Label M_Parent { get { return m_parent; } }
        }
        public partial class AaaaRecord : KaitaiStruct
        {
            public static AaaaRecord FromFile(string fileName)
            {
                return new AaaaRecord(new KaitaiStream(fileName));
            }

            public AaaaRecord(KaitaiStream p__io, DnsPacket.Answer p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _address = m_io.ReadBytes(16);
            }
            private byte[] _address;
            private DnsPacket m_root;
            private DnsPacket.Answer m_parent;
            public byte[] Address { get { return _address; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.Answer M_Parent { get { return m_parent; } }
        }
        public partial class ARecord : KaitaiStruct
        {
            public static ARecord FromFile(string fileName)
            {
                return new ARecord(new KaitaiStream(fileName));
            }

            public ARecord(KaitaiStream p__io, DnsPacket.Answer p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _address = m_io.ReadBytes(4);
            }
            private byte[] _address;
            private DnsPacket m_root;
            private DnsPacket.Answer m_parent;
            public byte[] Address { get { return _address; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.Answer M_Parent { get { return m_parent; } }
        }
        public partial class CnameRecord : KaitaiStruct
        {
            public static CnameRecord FromFile(string fileName)
            {
                return new CnameRecord(new KaitaiStream(fileName));
            }

            public CnameRecord(KaitaiStream p__io, DnsPacket.Answer p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _hostname = new DomainName(m_io, this, m_root);
            }
            private DomainName _hostname;
            private DnsPacket m_root;
            private DnsPacket.Answer m_parent;
            public DomainName Hostname { get { return _hostname; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.Answer M_Parent { get { return m_parent; } }
        }
        public partial class Label : KaitaiStruct
        {
            public static Label FromFile(string fileName)
            {
                return new Label(new KaitaiStream(fileName));
            }

            public Label(KaitaiStream p__io, DnsPacket.DomainName p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_isPointer = false;
                _read();
            }
            private void _read()
            {
                _length = m_io.ReadU1();
                if (IsPointer) {
                    _pointer = new PointerStruct(m_io, this, m_root);
                }
                if (!(IsPointer)) {
                    _name = System.Text.Encoding.GetEncoding("ASCII").GetString(m_io.ReadBytes(Length));
                }
            }
            private bool f_isPointer;
            private bool _isPointer;
            public bool IsPointer
            {
                get
                {
                    if (f_isPointer)
                        return _isPointer;
                    _isPointer = (bool) (Length == 192);
                    f_isPointer = true;
                    return _isPointer;
                }
            }
            private byte _length;
            private PointerStruct _pointer;
            private string _name;
            private DnsPacket m_root;
            private DnsPacket.DomainName m_parent;

            /// <summary>
            /// RFC1035 4.1.4: If the first two bits are raised it's a pointer-offset to a previously defined name
            /// </summary>
            public byte Length { get { return _length; } }
            public PointerStruct Pointer { get { return _pointer; } }

            /// <summary>
            /// Otherwise its a string the length of the length value
            /// </summary>
            public string Name { get { return _name; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.DomainName M_Parent { get { return m_parent; } }
        }
        public partial class Query : KaitaiStruct
        {
            public static Query FromFile(string fileName)
            {
                return new Query(new KaitaiStream(fileName));
            }

            public Query(KaitaiStream p__io, DnsPacket p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _name = new DomainName(m_io, this, m_root);
                _type = ((DnsPacket.RecordType) m_io.ReadU2be());
                _queryClass = ((DnsPacket.ClassType) m_io.ReadU2be());
            }
            private DomainName _name;
            private RecordType _type;
            private ClassType _queryClass;
            private DnsPacket m_root;
            private DnsPacket m_parent;
            public DomainName Name { get { return _name; } }
            public RecordType Type { get { return _type; } }
            public ClassType QueryClass { get { return _queryClass; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket M_Parent { get { return m_parent; } }
        }
        public partial class DomainName : KaitaiStruct
        {
            public static DomainName FromFile(string fileName)
            {
                return new DomainName(new KaitaiStream(fileName));
            }

            public DomainName(KaitaiStream p__io, KaitaiStruct p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _labels = new List<Label>();
                {
                    var i = 0;
                    Label M_;
                    do {
                        M_ = new Label(m_io, this, m_root);
                        _labels.Add(M_);
                        i++;
                    } while (!( ((M_.Length == 0) || (M_.Length == 192)) ));
                }
            }
            private List<Label> _labels;
            private DnsPacket m_root;
            private KaitaiStruct m_parent;

            /// <summary>
            /// Repeat until the length is 0 or it is a pointer (bit-hack to get around lack of OR operator)
            /// </summary>
            public List<Label> Labels { get { return _labels; } }
            public DnsPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class PtrRecord : KaitaiStruct
        {
            public static PtrRecord FromFile(string fileName)
            {
                return new PtrRecord(new KaitaiStream(fileName));
            }

            public PtrRecord(KaitaiStream p__io, DnsPacket.Answer p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _hostname = new DomainName(m_io, this, m_root);
            }
            private DomainName _hostname;
            private DnsPacket m_root;
            private DnsPacket.Answer m_parent;
            public DomainName Hostname { get { return _hostname; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.Answer M_Parent { get { return m_parent; } }
        }
        public partial class MxRecord : KaitaiStruct
        {
            public static MxRecord FromFile(string fileName)
            {
                return new MxRecord(new KaitaiStream(fileName));
            }

            public MxRecord(KaitaiStream p__io, DnsPacket.Answer p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _priority = m_io.ReadS2be();
                _hostname = new DomainName(m_io, this, m_root);
            }
            private short _priority;
            private DomainName _hostname;
            private DnsPacket m_root;
            private DnsPacket.Answer m_parent;
            public short Priority { get { return _priority; } }
            public DomainName Hostname { get { return _hostname; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.Answer M_Parent { get { return m_parent; } }
        }
        public partial class NsRecord : KaitaiStruct
        {
            public static NsRecord FromFile(string fileName)
            {
                return new NsRecord(new KaitaiStream(fileName));
            }

            public NsRecord(KaitaiStream p__io, DnsPacket.Answer p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _hostname = new DomainName(m_io, this, m_root);
            }
            private DomainName _hostname;
            private DnsPacket m_root;
            private DnsPacket.Answer m_parent;
            public DomainName Hostname { get { return _hostname; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket.Answer M_Parent { get { return m_parent; } }
        }
        public partial class Answer : KaitaiStruct
        {
            public static Answer FromFile(string fileName)
            {
                return new Answer(new KaitaiStream(fileName));
            }

            public Answer(KaitaiStream p__io, DnsPacket p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _name = new DomainName(m_io, this, m_root);
                _type = ((DnsPacket.RecordType) m_io.ReadU2be());
                _answerClass = ((DnsPacket.ClassType) m_io.ReadU2be());
                _ttl = m_io.ReadS4be();
                _rdlength = m_io.ReadU2be();
                switch (Type) {
                case DnsPacket.RecordType.Aaaa: {
                    _rdata = new AaaaRecord(m_io, this, m_root);
                    break;
                }
                case DnsPacket.RecordType.A: {
                    _rdata = new ARecord(m_io, this, m_root);
                    break;
                }
                case DnsPacket.RecordType.Mx: {
                    _rdata = new MxRecord(m_io, this, m_root);
                    break;
                }
                case DnsPacket.RecordType.Cname: {
                    _rdata = new CnameRecord(m_io, this, m_root);
                    break;
                }
                case DnsPacket.RecordType.Ns: {
                    _rdata = new NsRecord(m_io, this, m_root);
                    break;
                }
                case DnsPacket.RecordType.Ptr: {
                    _rdata = new PtrRecord(m_io, this, m_root);
                    break;
                }
                }
            }
            private DomainName _name;
            private RecordType _type;
            private ClassType _answerClass;
            private int _ttl;
            private ushort _rdlength;
            private KaitaiStruct _rdata;
            private DnsPacket m_root;
            private DnsPacket m_parent;
            public DomainName Name { get { return _name; } }
            public RecordType Type { get { return _type; } }
            public ClassType AnswerClass { get { return _answerClass; } }

            /// <summary>
            /// Time to live (in seconds)
            /// </summary>
            public int Ttl { get { return _ttl; } }

            /// <summary>
            /// Length in octets of the following payload
            /// </summary>
            public ushort Rdlength { get { return _rdlength; } }
            public KaitaiStruct Rdata { get { return _rdata; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket M_Parent { get { return m_parent; } }
        }
        public partial class PacketFlags : KaitaiStruct
        {
            public static PacketFlags FromFile(string fileName)
            {
                return new PacketFlags(new KaitaiStream(fileName));
            }

            public PacketFlags(KaitaiStream p__io, DnsPacket p__parent = null, DnsPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_qr = false;
                f_ra = false;
                f_tc = false;
                f_rcode = false;
                f_opcode = false;
                f_aa = false;
                f_z = false;
                f_rd = false;
                f_cd = false;
                f_ad = false;
                _read();
            }
            private void _read()
            {
                _flag = m_io.ReadU2be();
            }
            private bool f_qr;
            private int _qr;
            public int Qr
            {
                get
                {
                    if (f_qr)
                        return _qr;
                    _qr = (int) (((Flag & 32768) >> 15));
                    f_qr = true;
                    return _qr;
                }
            }
            private bool f_ra;
            private int _ra;
            public int Ra
            {
                get
                {
                    if (f_ra)
                        return _ra;
                    _ra = (int) (((Flag & 128) >> 7));
                    f_ra = true;
                    return _ra;
                }
            }
            private bool f_tc;
            private int _tc;
            public int Tc
            {
                get
                {
                    if (f_tc)
                        return _tc;
                    _tc = (int) (((Flag & 512) >> 9));
                    f_tc = true;
                    return _tc;
                }
            }
            private bool f_rcode;
            private int _rcode;
            public int Rcode
            {
                get
                {
                    if (f_rcode)
                        return _rcode;
                    _rcode = (int) (((Flag & 15) >> 0));
                    f_rcode = true;
                    return _rcode;
                }
            }
            private bool f_opcode;
            private int _opcode;
            public int Opcode
            {
                get
                {
                    if (f_opcode)
                        return _opcode;
                    _opcode = (int) (((Flag & 30720) >> 11));
                    f_opcode = true;
                    return _opcode;
                }
            }
            private bool f_aa;
            private int _aa;
            public int Aa
            {
                get
                {
                    if (f_aa)
                        return _aa;
                    _aa = (int) (((Flag & 1024) >> 10));
                    f_aa = true;
                    return _aa;
                }
            }
            private bool f_z;
            private int _z;
            public int Z
            {
                get
                {
                    if (f_z)
                        return _z;
                    _z = (int) (((Flag & 64) >> 6));
                    f_z = true;
                    return _z;
                }
            }
            private bool f_rd;
            private int _rd;
            public int Rd
            {
                get
                {
                    if (f_rd)
                        return _rd;
                    _rd = (int) (((Flag & 256) >> 8));
                    f_rd = true;
                    return _rd;
                }
            }
            private bool f_cd;
            private int _cd;
            public int Cd
            {
                get
                {
                    if (f_cd)
                        return _cd;
                    _cd = (int) (((Flag & 16) >> 4));
                    f_cd = true;
                    return _cd;
                }
            }
            private bool f_ad;
            private int _ad;
            public int Ad
            {
                get
                {
                    if (f_ad)
                        return _ad;
                    _ad = (int) (((Flag & 32) >> 5));
                    f_ad = true;
                    return _ad;
                }
            }
            private ushort _flag;
            private DnsPacket m_root;
            private DnsPacket m_parent;
            public ushort Flag { get { return _flag; } }
            public DnsPacket M_Root { get { return m_root; } }
            public DnsPacket M_Parent { get { return m_parent; } }
        }
        private ushort _transactionId;
        private PacketFlags _flags;
        private ushort _qdcount;
        private ushort _ancount;
        private ushort _nscount;
        private ushort _arcount;
        private List<Query> _queries;
        private List<Answer> _answers;
        private DnsPacket m_root;
        private KaitaiStruct m_parent;

        /// <summary>
        /// ID to keep track of request/responces
        /// </summary>
        public ushort TransactionId { get { return _transactionId; } }
        public PacketFlags Flags { get { return _flags; } }

        /// <summary>
        /// How many questions are there
        /// </summary>
        public ushort Qdcount { get { return _qdcount; } }

        /// <summary>
        /// Number of resource records answering the question
        /// </summary>
        public ushort Ancount { get { return _ancount; } }

        /// <summary>
        /// Number of resource records pointing toward an authority
        /// </summary>
        public ushort Nscount { get { return _nscount; } }

        /// <summary>
        /// Number of resource records holding additional information
        /// </summary>
        public ushort Arcount { get { return _arcount; } }
        public List<Query> Queries { get { return _queries; } }
        public List<Answer> Answers { get { return _answers; } }
        public DnsPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
