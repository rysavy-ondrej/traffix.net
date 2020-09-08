// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.IoT
{

    /// <summary>
    /// The binary TLV (Type-Length-Value) format is used to represent an array of values  or a singular value using a compact binary representation, which is easy to process  on simple embedded devices. The format has a minimum overhead per value of just 2 bytes  and a maximum overhead of 5 bytes depending on the type of Identifier and length of the value.  The maximum size of an Object Instance or Resource in this format is 16.7 MB.  The format is self- describing, thus a parser can skip TLVs for which the Resource is not known. This data format has a Media Type of application/vnd.oma.lwm2m+tlv.
    /// </summary>
    public partial class Lwm2mTlv : KaitaiStruct
    {
        public static Lwm2mTlv FromFile(string fileName)
        {
            return new Lwm2mTlv(new KaitaiStream(fileName));
        }


        public enum Lwm2mTlvIdentifierType
        {
            ObjectInstance = 0,
            ResourceInstance = 1,
            MultipleResource = 2,
            ResourceWithValue = 3,
        }
        public Lwm2mTlv(KaitaiStream p__io, KaitaiStruct p__parent = null, Lwm2mTlv p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _type = new TlvType(m_io, this, m_root);
            _identifier = new TlvIdentifier(m_io, this, m_root);
            _length = new TlvLength(m_io, this, m_root);
            _value = m_io.ReadBytes(Length.Value);
        }
        public partial class TlvIdentifier : KaitaiStruct
        {
            public static TlvIdentifier FromFile(string fileName)
            {
                return new TlvIdentifier(new KaitaiStream(fileName));
            }

            public TlvIdentifier(KaitaiStream p__io, Lwm2mTlv p__parent = null, Lwm2mTlv p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_value = false;
                _read();
            }
            private void _read()
            {
                if (M_Parent.Type.IdentifierWideLength == false)
                {
                    _tlvId1 = m_io.ReadU1();
                }
                if (M_Parent.Type.IdentifierWideLength == true)
                {
                    _tlvId2 = m_io.ReadU2be();
                }
            }
            private bool f_value;
            private int _value;
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int)((TlvId1 | TlvId2));
                    f_value = true;
                    return _value;
                }
            }
            private byte? _tlvId1;
            private ushort? _tlvId2;
            private Lwm2mTlv m_root;
            private Lwm2mTlv m_parent;
            public byte? TlvId1 { get { return _tlvId1; } }
            public ushort? TlvId2 { get { return _tlvId2; } }
            public Lwm2mTlv M_Root { get { return m_root; } }
            public Lwm2mTlv M_Parent { get { return m_parent; } }
        }
        public partial class TlvLength : KaitaiStruct
        {
            public static TlvLength FromFile(string fileName)
            {
                return new TlvLength(new KaitaiStream(fileName));
            }

            public TlvLength(KaitaiStream p__io, Lwm2mTlv p__parent = null, Lwm2mTlv p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_value = false;
                _read();
            }
            private void _read()
            {
                if (M_Parent.Type.LengthType == 1)
                {
                    _tlvLen1 = m_io.ReadU1();
                }
                if (M_Parent.Type.LengthType == 2)
                {
                    _tlvLen2 = m_io.ReadBitsInt(16);
                }
                if (M_Parent.Type.LengthType == 3)
                {
                    _tlvLen3 = m_io.ReadBitsInt(24);
                }
            }
            private bool f_value;
            private int _value;
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int)((((M_Parent.Type.ValueLength | TlvLen1) | TlvLen2) | TlvLen3));
                    f_value = true;
                    return _value;
                }
            }
            private byte? _tlvLen1;
            private ulong? _tlvLen2;
            private ulong? _tlvLen3;
            private Lwm2mTlv m_root;
            private Lwm2mTlv m_parent;
            public byte? TlvLen1 { get { return _tlvLen1; } }
            public ulong? TlvLen2 { get { return _tlvLen2; } }
            public ulong? TlvLen3 { get { return _tlvLen3; } }
            public Lwm2mTlv M_Root { get { return m_root; } }
            public Lwm2mTlv M_Parent { get { return m_parent; } }
        }
        public partial class TlvType : KaitaiStruct
        {
            public static TlvType FromFile(string fileName)
            {
                return new TlvType(new KaitaiStream(fileName));
            }

            public TlvType(KaitaiStream p__io, Lwm2mTlv p__parent = null, Lwm2mTlv p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _identifierType = ((Lwm2mTlv.Lwm2mTlvIdentifierType)m_io.ReadBitsInt(2));
                _identifierWideLength = m_io.ReadBitsInt(1) != 0;
                _lengthType = m_io.ReadBitsInt(2);
                _valueLength = m_io.ReadBitsInt(3);
            }
            private Lwm2mTlvIdentifierType _identifierType;
            private bool _identifierWideLength;
            private ulong _lengthType;
            private ulong _valueLength;
            private Lwm2mTlv m_root;
            private Lwm2mTlv m_parent;

            /// <summary>
            /// Bits 7-6: Indicates the type of Identifier:
            ///   00= Object Instance in which case the Value contains one or more Resource TLVs
            ///   01= Resource Instance with Value for use within a multiple Resource TLV
            ///   10= multiple Resource, in which case the Value contains one or more Resource Instance TLVs
            ///   11= Resource with Value
            /// </summary>
            public Lwm2mTlvIdentifierType IdentifierType { get { return _identifierType; } }

            /// <summary>
            /// Bit 5: Indicates the Length of the Identifier. 
            ///   0=The Identifier field of this TLV is 8 bits long
            ///   1=The Identifier field of this TLV is 16 bits long
            /// </summary>
            public bool IdentifierWideLength { get { return _identifierWideLength; } }

            /// <summary>
            /// Bit 4-3: Indicates the type of Length.
            ///   00 = No length field, the value immediately follows the Identifier field in is of the length indicated by Bits 2-0 of this field
            ///   01 = The Length field is 8-bits and Bits 2-0 MUST be ignored
            ///   10 = The Length field is 16-bits and Bits 2-0 MUST be ignored
            ///   11 = The Length field is 24-bits and Bits 2-0 MUST be ignored
            /// </summary>
            public ulong LengthType { get { return _lengthType; } }

            /// <summary>
            /// Bits 2-0: A 3-bit unsigned integer indicating the Length of the Value.
            /// </summary>
            public ulong ValueLength { get { return _valueLength; } }
            public Lwm2mTlv M_Root { get { return m_root; } }
            public Lwm2mTlv M_Parent { get { return m_parent; } }
        }
        private TlvType _type;
        private TlvIdentifier _identifier;
        private TlvLength _length;
        private byte[] _value;
        private Lwm2mTlv m_root;
        private KaitaiStruct m_parent;

        /// <summary>
        /// 8-bits masked field
        /// </summary>
        public TlvType Type { get { return _type; } }

        /// <summary>
        /// The Object Instance, Resource, or Resource Instance ID as indicated by the Type field.
        /// </summary>
        public TlvIdentifier Identifier { get { return _identifier; } }

        /// <summary>
        /// The Length of the following field in bytes.
        /// </summary>
        public TlvLength Length { get { return _length; } }

        /// <summary>
        /// Value of the tag. The format of the value depends on the Resource‟s data type.
        /// </summary>
        public byte[] Value { get { return _value; } }
        public Lwm2mTlv M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
