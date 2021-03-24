// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Industrial
{

    /// <summary>
    /// The ACSE APDUs are part of DLMS/COSEM communication during opening and  closing phase. ACSE APDUs are standardized by IEC 8650-1 standard or X.227 The APDUs are encoded by BER for transmission. The user-information field  contains a COSEM pdu encoded by A-XDR.
    /// ACSE contains three pairs (request and response) of messages: * Application Association messages for creating association between cleint and server application. * Application Association Release Messages for finishing association.  * Application Association Abort Messages for aborting association in case of unexpected situation.
    /// References:
    /// * Distribution automation using distribution line carrier systems - Part 6: A-XDR encoding rule, IEC 61334-6:2000, 2006, Edition 1. * ITU-T Recommendation X.227: Data Networks and Open System Communications: Connection-oriented Protocol for the Association Control Service Element: Protocol Specification, ITU-T X.227, 1995.
    /// 
    /// Limitations: 
    /// We parse only top level structure. Most of the fields uses BER encoding for their values. But there are some fields that used X-ADR encoding, e.g. User Information. 
    /// </summary>
    public partial class DlmsAcse : KaitaiStruct
    {
        public static DlmsAcse FromFile(string fileName)
        {
            return new DlmsAcse(new KaitaiStream(fileName));
        }

        public enum AcsePduType
        {
            Aarq = 96,
            Aare = 97,
            Rlrq = 98,
            Rlre = 99,
            Abrt = 100,
            Adt = 101,
        }

        public enum AarqPduFields
        {
            SenderAcseRequirements = 138,
            MechanismName = 139,
            ProtocolVersion = 160,
            ApplicationContextName = 161,
            CalledApTitle = 162,
            CalledAeQuantifier = 163,
            CalledApInvocationId = 164,
            CalledAeInvocationIde = 165,
            CallingApTitle = 166,
            CallingAeQuantifier = 167,
            CallingApInvocationId = 168,
            CallingAeInvocationId = 169,
            CallingAuthenticationValue = 172,
            ImplementationInformation = 189,
            UserInformation = 190,
        }

        public enum AarePduFields
        {
            RespondingAcseRequirements = 136,
            MechanismName = 137,
            ApplicationContextNameList = 139,
            ProtocolVersion = 160,
            ApplicationContextName = 161,
            Result = 162,
            ResultSourceDiagnostic = 163,
            RespondingApTitle = 164,
            RespondingAeQualifier = 165,
            RespondingApInvocationId = 166,
            RespondingAeInvocationId = 167,
            RespondingAuthenticationValue = 170,
            ImplementationInformation = 189,
            UserInformation = 190,
        }

        public DlmsAcse(KaitaiStream io, KaitaiStruct parent = null, DlmsAcse root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _pduType = ((AcsePduType)m_io.ReadU1());
            switch (PduType)
            {
                case AcsePduType.Abrt:
                    {
                        _pdu = new AbrtPdu(m_io, this, m_root);
                        break;
                    }
                case AcsePduType.Aarq:
                    {
                        _pdu = new AarqPdu(m_io, this, m_root);
                        break;
                    }
                case AcsePduType.Rlrq:
                    {
                        _pdu = new RlrqPdu(m_io, this, m_root);
                        break;
                    }
                case AcsePduType.Aare:
                    {
                        _pdu = new AarePdu(m_io, this, m_root);
                        break;
                    }
                case AcsePduType.Adt:
                    {
                        _pdu = new AdtPdu(m_io, this, m_root);
                        break;
                    }
                case AcsePduType.Rlre:
                    {
                        _pdu = new RlrePdu(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class RlrqPdu : KaitaiStruct
        {
            public static RlrqPdu FromFile(string fileName)
            {
                return new RlrqPdu(new KaitaiStream(fileName));
            }

            public RlrqPdu(KaitaiStream io, DlmsAcse parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsAcse m_root;
            private DlmsAcse m_parent;
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse M_Parent { get { return m_parent; } }
        }
        public partial class AbrtPdu : KaitaiStruct
        {
            public static AbrtPdu FromFile(string fileName)
            {
                return new AbrtPdu(new KaitaiStream(fileName));
            }

            public AbrtPdu(KaitaiStream io, DlmsAcse parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsAcse m_root;
            private DlmsAcse m_parent;
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse M_Parent { get { return m_parent; } }
        }
        public partial class AarePduField : KaitaiStruct
        {
            public static AarePduField FromFile(string fileName)
            {
                return new AarePduField(new KaitaiStream(fileName));
            }

            public AarePduField(KaitaiStream io, AarePdu parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _tag = ((DlmsAcse.AarePduFields)m_io.ReadU1());
                _length = new BerLength(m_io, this, m_root);
                _value = m_io.ReadBytes(Length.Value);
            }
            private AarePduFields _tag;
            private BerLength _length;
            private byte[] _value;
            private DlmsAcse m_root;
            private DlmsAcse.AarePdu m_parent;
            public AarePduFields Tag { get { return _tag; } }
            public BerLength Length { get { return _length; } }
            public byte[] Value { get { return _value; } }
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse.AarePdu M_Parent { get { return m_parent; } }
        }
        public partial class AarqPduField : KaitaiStruct
        {
            public static AarqPduField FromFile(string fileName)
            {
                return new AarqPduField(new KaitaiStream(fileName));
            }

            public AarqPduField(KaitaiStream io, AarqPdu parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _tag = ((DlmsAcse.AarqPduFields)m_io.ReadU1());
                _length = new BerLength(m_io, this, m_root);
                _value = m_io.ReadBytes(Length.Value);
            }
            private AarqPduFields _tag;
            private BerLength _length;
            private byte[] _value;
            private DlmsAcse m_root;
            private DlmsAcse.AarqPdu m_parent;
            public AarqPduFields Tag { get { return _tag; } }
            public BerLength Length { get { return _length; } }
            public byte[] Value { get { return _value; } }
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse.AarqPdu M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Application Association Request PDU. It uses BER encoding 
        /// according to the the following ASN.1 schema:
        /// protocol-version [0] 
        /// application-context-name [1] 
        /// called-AP-title [2] 
        /// called-AE-qualifier [3] 
        /// called-AP-invocation-id [4] 
        /// called-AE-invocation-ide [5] 
        /// calling-AP-title [6] 
        /// calling-AE-quantifier [7] 
        /// calling-AP-invocation-id [8] 
        /// calling-AE-invocation-id [9] 
        /// sender-acse-requirements [10] 
        /// mechanism-name [11] 
        /// calling-authentication-value [12] 
        /// implementation-information [29] 
        /// user-information [30]
        /// 
        /// Values are either application specific or context specific identifier, i.e., 
        /// they have 0xa or 0x8 prefix. 
        /// 
        /// Only top-level information is parsed. To parse 
        /// user-information field use corresponding dlms parser.
        /// </summary>
        public partial class AarqPdu : KaitaiStruct
        {
            public static AarqPdu FromFile(string fileName)
            {
                return new AarqPdu(new KaitaiStream(fileName));
            }

            public AarqPdu(KaitaiStream io, DlmsAcse parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _length = new BerLength(m_io, this, m_root);
                _fields = new List<AarqPduField>();
                while (!m_io.IsEof)
                {
                    _fields.Add(new AarqPduField(m_io, this, m_root));
                }
            }
            private BerLength _length;
            private List<AarqPduField> _fields;
            private DlmsAcse m_root;
            private DlmsAcse m_parent;
            public BerLength Length { get { return _length; } }
            public List<AarqPduField> Fields { get { return _fields; } }
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// Application Association Response PDU. It uses BER encoding 
        /// according to the the following ASN.1 schema:
        ///  protocol-version                [0]
        ///  application-context-name        [1]
        ///  result                          [2]
        ///  result-source-diagnostic        [3]
        ///  responding-AP-title             [4]
        ///  responding-AE-qualifier         [5] 
        ///  responding-AP-invocation-id     [6]
        ///  responding-AE-invocation-id     [7]
        ///  responder-acse-requirements     [8]
        ///  mechanism-name                  [9]
        ///  responding-authentication-value [10] 
        ///  application-context-name-list   [11] 
        ///  implementation-information      [29] 
        ///  user-information                [30] 
        /// 
        /// Values are either application specific or context specific identifier, i.e., 
        /// they have 0xa or 0x8 prefix. 
        /// 
        /// Only top-level information is parsed. To parse 
        /// user-information field use corresponding dlms parser.
        /// </summary>
        public partial class AarePdu : KaitaiStruct
        {
            public static AarePdu FromFile(string fileName)
            {
                return new AarePdu(new KaitaiStream(fileName));
            }

            public AarePdu(KaitaiStream io, DlmsAcse parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _length = new BerLength(m_io, this, m_root);
                _fields = new List<AarePduField>();
                while (!m_io.IsEof)
                {
                    _fields.Add(new AarePduField(m_io, this, m_root));
                }
            }
            private BerLength _length;
            private List<AarePduField> _fields;
            private DlmsAcse m_root;
            private DlmsAcse m_parent;
            public BerLength Length { get { return _length; } }
            public List<AarePduField> Fields { get { return _fields; } }
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse M_Parent { get { return m_parent; } }
        }
        public partial class BerLength : KaitaiStruct
        {
            public static BerLength FromFile(string fileName)
            {
                return new BerLength(new KaitaiStream(fileName));
            }

            public BerLength(KaitaiStream io, KaitaiStruct parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_value = false;
                _b1 = m_io.ReadU1();
                if (B1 == 130)
                {
                    _int2 = m_io.ReadU2be();
                }
            }
            private bool f_value;
            private ushort _value;
            public ushort Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (ushort)(((B1 & 128) == 0 ? B1 : Int2));
                    f_value = true;
                    return _value;
                }
            }
            private byte _b1;
            private ushort _int2;
            private DlmsAcse m_root;
            private KaitaiStruct m_parent;
            public byte B1 { get { return _b1; } }
            public ushort Int2 { get { return _int2; } }
            public DlmsAcse M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class RlrePdu : KaitaiStruct
        {
            public static RlrePdu FromFile(string fileName)
            {
                return new RlrePdu(new KaitaiStream(fileName));
            }

            public RlrePdu(KaitaiStream io, DlmsAcse parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsAcse m_root;
            private DlmsAcse m_parent;
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse M_Parent { get { return m_parent; } }
        }
        public partial class AdtPdu : KaitaiStruct
        {
            public static AdtPdu FromFile(string fileName)
            {
                return new AdtPdu(new KaitaiStream(fileName));
            }

            public AdtPdu(KaitaiStream io, DlmsAcse parent = null, DlmsAcse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsAcse m_root;
            private DlmsAcse m_parent;
            public DlmsAcse M_Root { get { return m_root; } }
            public DlmsAcse M_Parent { get { return m_parent; } }
        }
        private AcsePduType _pduType;
        private KaitaiStruct _pdu;
        private DlmsAcse m_root;
        private KaitaiStruct m_parent;
        public AcsePduType PduType { get { return _pduType; } }
        public KaitaiStruct Pdu { get { return _pdu; } }
        public DlmsAcse M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
