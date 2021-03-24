// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsData : KaitaiStruct
    {
        public static DlmsData FromFile(string fileName)
        {
            return new DlmsData(new KaitaiStream(fileName));
        }

        public enum DataType
        {
            NullData = 0,
            Array = 1,
            Structure = 2,
            Boolean = 3,
            BitString = 4,
            DoubleLong = 5,
            DoubleLongUnsigned = 6,
            OctetString = 9,
            VisibleString = 10,
            Bcd = 13,
            Integer = 15,
            Long = 16,
            Unsigned = 17,
            LongUnsigned = 18,
            CompactArray = 19,
            Long64 = 20,
            Long64Unsigned = 21,
            Enum = 22,
            Float32 = 23,
            Float64 = 24,
            DateTime = 25,
            Date = 26,
            Time = 27,
            DoNotCare = 255,
        }

        public DlmsData(KaitaiStream io, KaitaiStruct parent = null, DlmsData root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _datatype = ((DataType)m_io.ReadU1());
            switch (Datatype)
            {
                case DataType.Integer:
                    {
                        _dataValue = new Integer(m_io, this, m_root);
                        break;
                    }
                case DataType.Unsigned:
                    {
                        _dataValue = new Unsigned(m_io, this, m_root);
                        break;
                    }
                case DataType.Long:
                    {
                        _dataValue = new Long(m_io, this, m_root);
                        break;
                    }
                case DataType.Boolean:
                    {
                        _dataValue = new Boolean(m_io, this, m_root);
                        break;
                    }
                case DataType.Structure:
                    {
                        _dataValue = new Structure(m_io, this, m_root);
                        break;
                    }
                case DataType.Array:
                    {
                        _dataValue = new Array(m_io, this, m_root);
                        break;
                    }
                case DataType.Float64:
                    {
                        _dataValue = new Float64(m_io, this, m_root);
                        break;
                    }
                case DataType.DoNotCare:
                    {
                        _dataValue = new DoNotCare(m_io, this, m_root);
                        break;
                    }
                case DataType.LongUnsigned:
                    {
                        _dataValue = new LongUnsigned(m_io, this, m_root);
                        break;
                    }
                case DataType.Time:
                    {
                        _dataValue = new Time(m_io, this, m_root);
                        break;
                    }
                case DataType.OctetString:
                    {
                        _dataValue = new OctetString(m_io, this, m_root);
                        break;
                    }
                case DataType.NullData:
                    {
                        _dataValue = new NullData(m_io, this, m_root);
                        break;
                    }
                case DataType.CompactArray:
                    {
                        _dataValue = new CompactArray(m_io, this, m_root);
                        break;
                    }
                case DataType.DateTime:
                    {
                        _dataValue = new DateTime(m_io, this, m_root);
                        break;
                    }
                case DataType.DoubleLongUnsigned:
                    {
                        _dataValue = new DoubleLongUnsigned(m_io, this, m_root);
                        break;
                    }
                case DataType.Float32:
                    {
                        _dataValue = new Float32(m_io, this, m_root);
                        break;
                    }
                case DataType.Long64Unsigned:
                    {
                        _dataValue = new Long64Unsigned(m_io, this, m_root);
                        break;
                    }
                case DataType.DoubleLong:
                    {
                        _dataValue = new DoubleLong(m_io, this, m_root);
                        break;
                    }
                case DataType.Long64:
                    {
                        _dataValue = new Long64(m_io, this, m_root);
                        break;
                    }
                case DataType.Date:
                    {
                        _dataValue = new Date(m_io, this, m_root);
                        break;
                    }
                case DataType.Enum:
                    {
                        _dataValue = new Enum(m_io, this, m_root);
                        break;
                    }
                case DataType.Bcd:
                    {
                        _dataValue = new Bcd(m_io, this, m_root);
                        break;
                    }
                case DataType.VisibleString:
                    {
                        _dataValue = new VisibleString(m_io, this, m_root);
                        break;
                    }
                case DataType.BitString:
                    {
                        _dataValue = new BitString(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class DoubleLong : KaitaiStruct
        {
            public static DoubleLong FromFile(string fileName)
            {
                return new DoubleLong(new KaitaiStream(fileName));
            }

            public DoubleLong(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadS4be();
            }
            private int _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public int Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class OctetString : KaitaiStruct
        {
            public static OctetString FromFile(string fileName)
            {
                return new OctetString(new KaitaiStream(fileName));
            }

            public OctetString(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _length = new LengthEncoded(m_io, this, m_root);
                _value = m_io.ReadBytes(Length.Value);
            }
            private LengthEncoded _length;
            private byte[] _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public LengthEncoded Length { get { return _length; } }
            public byte[] Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class NullData : KaitaiStruct
        {
            public static NullData FromFile(string fileName)
            {
                return new NullData(new KaitaiStream(fileName));
            }

            public NullData(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _nothing = m_io.ReadBytes(0);
            }
            private byte[] _nothing;
            private DlmsData m_root;
            private DlmsData m_parent;
            public byte[] Nothing { get { return _nothing; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class OctetStringOptional : KaitaiStruct
        {
            public static OctetStringOptional FromFile(string fileName)
            {
                return new OctetStringOptional(new KaitaiStream(fileName));
            }

            public OctetStringOptional(KaitaiStream io, KaitaiStruct parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _present = m_io.ReadU1();
                if (Present != 0)
                {
                    _value = new OctetString(m_io, null, m_root);
                }
            }
            private byte _present;
            private OctetString _value;
            private DlmsData m_root;
            private KaitaiStruct m_parent;
            public byte Present { get { return _present; } }
            public OctetString Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class Float64 : KaitaiStruct
        {
            public static Float64 FromFile(string fileName)
            {
                return new Float64(new KaitaiStream(fileName));
            }

            public Float64(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadF8be();
            }
            private double _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public double Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class LengthEncoded : KaitaiStruct
        {
            public static LengthEncoded FromFile(string fileName)
            {
                return new LengthEncoded(new KaitaiStream(fileName));
            }

            public LengthEncoded(KaitaiStream io, KaitaiStruct parent = null, DlmsData root = null) : base(io)
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
            private DlmsData m_root;
            private KaitaiStruct m_parent;
            public byte B1 { get { return _b1; } }
            public ushort Int2 { get { return _int2; } }
            public DlmsData M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class CompactArray : KaitaiStruct
        {
            public static CompactArray FromFile(string fileName)
            {
                return new CompactArray(new KaitaiStream(fileName));
            }

            public CompactArray(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsData m_root;
            private DlmsData m_parent;
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class DoNotCare : KaitaiStruct
        {
            public static DoNotCare FromFile(string fileName)
            {
                return new DoNotCare(new KaitaiStream(fileName));
            }

            public DoNotCare(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsData m_root;
            private DlmsData m_parent;
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Array : KaitaiStruct
        {
            public static Array FromFile(string fileName)
            {
                return new Array(new KaitaiStream(fileName));
            }

            public Array(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _length = new LengthEncoded(m_io, this, m_root);
                _value = m_io.ReadBytes(Length.Value);
            }
            private LengthEncoded _length;
            private byte[] _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public LengthEncoded Length { get { return _length; } }
            public byte[] Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Float32 : KaitaiStruct
        {
            public static Float32 FromFile(string fileName)
            {
                return new Float32(new KaitaiStream(fileName));
            }

            public Float32(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadF4be();
            }
            private float _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public float Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Long : KaitaiStruct
        {
            public static Long FromFile(string fileName)
            {
                return new Long(new KaitaiStream(fileName));
            }

            public Long(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadS2be();
            }
            private short _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public short Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Boolean : KaitaiStruct
        {
            public static Boolean FromFile(string fileName)
            {
                return new Boolean(new KaitaiStream(fileName));
            }

            public Boolean(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadU1();
            }
            private byte _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public byte Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Structure : KaitaiStruct
        {
            public static Structure FromFile(string fileName)
            {
                return new Structure(new KaitaiStream(fileName));
            }

            public Structure(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsData m_root;
            private DlmsData m_parent;
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Date : KaitaiStruct
        {
            public static Date FromFile(string fileName)
            {
                return new Date(new KaitaiStream(fileName));
            }

            public Date(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadBytes(5);
            }
            private byte[] _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public byte[] Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class DoubleLongUnsigned : KaitaiStruct
        {
            public static DoubleLongUnsigned FromFile(string fileName)
            {
                return new DoubleLongUnsigned(new KaitaiStream(fileName));
            }

            public DoubleLongUnsigned(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadU4be();
            }
            private uint _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public uint Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class VisibleString : KaitaiStruct
        {
            public static VisibleString FromFile(string fileName)
            {
                return new VisibleString(new KaitaiStream(fileName));
            }

            public VisibleString(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _length = new LengthEncoded(m_io, this, m_root);
                _value = System.Text.Encoding.GetEncoding("ascii").GetString(m_io.ReadBytes(Length.Value));
            }
            private LengthEncoded _length;
            private string _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public LengthEncoded Length { get { return _length; } }
            public string Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Enum : KaitaiStruct
        {
            public static Enum FromFile(string fileName)
            {
                return new Enum(new KaitaiStream(fileName));
            }

            public Enum(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadU1();
            }
            private byte _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public byte Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Unsigned : KaitaiStruct
        {
            public static Unsigned FromFile(string fileName)
            {
                return new Unsigned(new KaitaiStream(fileName));
            }

            public Unsigned(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadU1();
            }
            private byte _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public byte Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class DateTime : KaitaiStruct
        {
            public static DateTime FromFile(string fileName)
            {
                return new DateTime(new KaitaiStream(fileName));
            }

            public DateTime(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadBytes(12);
            }
            private byte[] _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public byte[] Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Long64Unsigned : KaitaiStruct
        {
            public static Long64Unsigned FromFile(string fileName)
            {
                return new Long64Unsigned(new KaitaiStream(fileName));
            }

            public Long64Unsigned(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadU8be();
            }
            private ulong _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public ulong Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Integer : KaitaiStruct
        {
            public static Integer FromFile(string fileName)
            {
                return new Integer(new KaitaiStream(fileName));
            }

            public Integer(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadS1();
            }
            private sbyte _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public sbyte Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Time : KaitaiStruct
        {
            public static Time FromFile(string fileName)
            {
                return new Time(new KaitaiStream(fileName));
            }

            public Time(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadBytes(4);
            }
            private byte[] _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public byte[] Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Bcd : KaitaiStruct
        {
            public static Bcd FromFile(string fileName)
            {
                return new Bcd(new KaitaiStream(fileName));
            }

            public Bcd(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsData m_root;
            private DlmsData m_parent;
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class Long64 : KaitaiStruct
        {
            public static Long64 FromFile(string fileName)
            {
                return new Long64(new KaitaiStream(fileName));
            }

            public Long64(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadS8be();
            }
            private long _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public long Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class BitString : KaitaiStruct
        {
            public static BitString FromFile(string fileName)
            {
                return new BitString(new KaitaiStream(fileName));
            }

            public BitString(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _length = new LengthEncoded(m_io, this, m_root);
                _value = new List<byte>((int)(Length.Value));
                for (var i = 0; i < Length.Value; i++)
                {
                    _value.Add(m_io.ReadU1());
                }
            }
            private LengthEncoded _length;
            private List<byte> _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public LengthEncoded Length { get { return _length; } }
            public List<byte> Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        public partial class BooleanOptional : KaitaiStruct
        {
            public static BooleanOptional FromFile(string fileName)
            {
                return new BooleanOptional(new KaitaiStream(fileName));
            }

            public BooleanOptional(KaitaiStream io, KaitaiStruct parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _present = m_io.ReadU1();
                if (Present != 0)
                {
                    _value = new Boolean(m_io, null, m_root);
                }
            }
            private byte _present;
            private Boolean _value;
            private DlmsData m_root;
            private KaitaiStruct m_parent;
            public byte Present { get { return _present; } }
            public Boolean Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class LongUnsigned : KaitaiStruct
        {
            public static LongUnsigned FromFile(string fileName)
            {
                return new LongUnsigned(new KaitaiStream(fileName));
            }

            public LongUnsigned(KaitaiStream io, DlmsData parent = null, DlmsData root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = m_io.ReadU2be();
            }
            private ushort _value;
            private DlmsData m_root;
            private DlmsData m_parent;
            public ushort Value { get { return _value; } }
            public DlmsData M_Root { get { return m_root; } }
            public DlmsData M_Parent { get { return m_parent; } }
        }
        private DataType _datatype;
        private KaitaiStruct _dataValue;
        private DlmsData m_root;
        private KaitaiStruct m_parent;
        public DataType Datatype { get { return _datatype; } }
        public KaitaiStruct DataValue { get { return _dataValue; } }
        public DlmsData M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
