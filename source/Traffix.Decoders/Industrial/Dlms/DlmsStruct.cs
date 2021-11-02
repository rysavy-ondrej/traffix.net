// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsStruct : KaitaiStruct
    {
        public static DlmsStruct FromFile(string fileName)
        {
            return new DlmsStruct(new KaitaiStream(fileName));
        }

        public enum AccessResult
        {
            Success = 0,
            HardwareFault = 1,
            TemporaryFailure = 2,
            ReadWriteDenied = 3,
            ObjectUndefined = 4,
            ObjectClassInconsistent = 9,
            ObjectUnavailable = 11,
            TypeUnmatched = 12,
            ScopeOfAccessViolated = 13,
            DataBlockUnavailable = 14,
            LongGetAborted = 15,
            NoLongGetInProgress = 16,
            LongSetAborted = 17,
            NoLongSetInProgress = 18,
            OtherReason = 250,
        }

        public DlmsStruct(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
        }
        public partial class SequenceOfData : KaitaiStruct
        {
            public static SequenceOfData FromFile(string fileName)
            {
                return new SequenceOfData(new KaitaiStream(fileName));
            }

            public SequenceOfData(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _itemCount = new LengthEncoded(m_io, this, m_root);
                _items = new List<DlmsData>((int)(ItemCount.Value));
                for (var i = 0; i < ItemCount.Value; i++)
                {
                    _items.Add(new DlmsData(m_io));
                }
            }
            private LengthEncoded _itemCount;
            private List<DlmsData> _items;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public LengthEncoded ItemCount { get { return _itemCount; } }
            public List<DlmsData> Items { get { return _items; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class SequenceOfCosemAttributeDescriptorWithSelection : KaitaiStruct
        {
            public static SequenceOfCosemAttributeDescriptorWithSelection FromFile(string fileName)
            {
                return new SequenceOfCosemAttributeDescriptorWithSelection(new KaitaiStream(fileName));
            }

            public SequenceOfCosemAttributeDescriptorWithSelection(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _itemCount = new LengthEncoded(m_io, this, m_root);
                _items = new List<CosemAttributeDescriptorWithSelection>((int)(ItemCount.Value));
                for (var i = 0; i < ItemCount.Value; i++)
                {
                    _items.Add(new CosemAttributeDescriptorWithSelection(m_io, this, m_root));
                }
            }
            private LengthEncoded _itemCount;
            private List<CosemAttributeDescriptorWithSelection> _items;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public LengthEncoded ItemCount { get { return _itemCount; } }
            public List<CosemAttributeDescriptorWithSelection> Items { get { return _items; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class CosemDateTime : KaitaiStruct
        {
            public static CosemDateTime FromFile(string fileName)
            {
                return new CosemDateTime(new KaitaiStream(fileName));
            }

            public CosemDateTime(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _bytes = m_io.ReadBytes(12);
            }
            private byte[] _bytes;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte[] Bytes { get { return _bytes; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class DataOptional : KaitaiStruct
        {
            public static DataOptional FromFile(string fileName)
            {
                return new DataOptional(new KaitaiStream(fileName));
            }

            public DataOptional(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
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
                    _value = new DlmsData(m_io);
                }
            }
            private byte _present;
            private DlmsData _value;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte Present { get { return _present; } }
            public DlmsData Value { get { return _value; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class GetDataResult : KaitaiStruct
        {
            public static GetDataResult FromFile(string fileName)
            {
                return new GetDataResult(new KaitaiStream(fileName));
            }

            public GetDataResult(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _dataResultType = m_io.ReadU1();
                switch (DataResultType)
                {
                    case 0:
                        {
                            _dataResultValue = new DlmsData(m_io);
                            break;
                        }
                    case 1:
                        {
                            _dataResultValue = new DataAccessResult(m_io, this, m_root);
                            break;
                        }
                }
            }
            private byte _dataResultType;
            private KaitaiStruct _dataResultValue;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte DataResultType { get { return _dataResultType; } }
            public KaitaiStruct DataResultValue { get { return _dataResultValue; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class CosemObjectInstanceId : KaitaiStruct
        {
            public static CosemObjectInstanceId FromFile(string fileName)
            {
                return new CosemObjectInstanceId(new KaitaiStream(fileName));
            }

            public CosemObjectInstanceId(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _oid = m_io.ReadBytes(6);
            }
            private byte[] _oid;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte[] Oid { get { return _oid; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class CosemMethodDescriptor : KaitaiStruct
        {
            public static CosemMethodDescriptor FromFile(string fileName)
            {
                return new CosemMethodDescriptor(new KaitaiStream(fileName));
            }

            public CosemMethodDescriptor(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _classId = m_io.ReadU2be();
                _instanceId = new CosemObjectInstanceId(m_io, this, m_root);
                _methodId = m_io.ReadU1();
            }
            private ushort _classId;
            private CosemObjectInstanceId _instanceId;
            private byte _methodId;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public ushort ClassId { get { return _classId; } }
            public CosemObjectInstanceId InstanceId { get { return _instanceId; } }
            public byte MethodId { get { return _methodId; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class DatablockSa : KaitaiStruct
        {
            public static DatablockSa FromFile(string fileName)
            {
                return new DatablockSa(new KaitaiStream(fileName));
            }

            public DatablockSa(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _data = m_io.ReadBytesFull();
            }
            private byte[] _data;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte[] Data { get { return _data; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class CosemAttributeDescriptorWithSelection : KaitaiStruct
        {
            public static CosemAttributeDescriptorWithSelection FromFile(string fileName)
            {
                return new CosemAttributeDescriptorWithSelection(new KaitaiStream(fileName));
            }

            public CosemAttributeDescriptorWithSelection(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _cosemAttributeDescriptor = new CosemAttributeDescriptor(m_io, this, m_root);
                _accessSelection = new SelectiveAccessDescriptorOptional(m_io, this, m_root);
            }
            private CosemAttributeDescriptor _cosemAttributeDescriptor;
            private SelectiveAccessDescriptorOptional _accessSelection;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public CosemAttributeDescriptor CosemAttributeDescriptor { get { return _cosemAttributeDescriptor; } }
            public SelectiveAccessDescriptorOptional AccessSelection { get { return _accessSelection; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class DatablockG : KaitaiStruct
        {
            public static DatablockG FromFile(string fileName)
            {
                return new DatablockG(new KaitaiStream(fileName));
            }

            public DatablockG(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _lastBlock = m_io.ReadU1();
                _blockNumber = m_io.ReadU4be();
                _resultChoice = m_io.ReadU1();
                switch (ResultChoice)
                {
                    case 0:
                        {
                            _result = new DlmsData.OctetString(m_io);
                            break;
                        }
                    case 1:
                        {
                            _result = new DataAccessResult(m_io, this, m_root);
                            break;
                        }
                }
            }
            private byte _lastBlock;
            private uint _blockNumber;
            private byte _resultChoice;
            private KaitaiStruct _result;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte LastBlock { get { return _lastBlock; } }
            public uint BlockNumber { get { return _blockNumber; } }
            public byte ResultChoice { get { return _resultChoice; } }
            public KaitaiStruct Result { get { return _result; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class LengthEncoded : KaitaiStruct
        {
            public static LengthEncoded FromFile(string fileName)
            {
                return new LengthEncoded(new KaitaiStream(fileName));
            }

            public LengthEncoded(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
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
                if (B1 == 132)
                {
                    _int4 = m_io.ReadU4be();
                }
            }
            private bool f_value;
            private uint _value;
            public uint Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (uint)((B1 == 132 ? Int4 : (B1 == 130 ? Int2 : B1)));
                    f_value = true;
                    return _value;
                }
            }
            private byte _b1;
            private ushort _int2;
            private uint _int4;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte B1 { get { return _b1; } }
            public ushort Int2 { get { return _int2; } }
            public uint Int4 { get { return _int4; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class SequenceOfGetDataResult : KaitaiStruct
        {
            public static SequenceOfGetDataResult FromFile(string fileName)
            {
                return new SequenceOfGetDataResult(new KaitaiStream(fileName));
            }

            public SequenceOfGetDataResult(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _itemCount = new LengthEncoded(m_io, this, m_root);
                _items = new List<GetDataResult>((int)(ItemCount.Value));
                for (var i = 0; i < ItemCount.Value; i++)
                {
                    _items.Add(new GetDataResult(m_io, this, m_root));
                }
            }
            private LengthEncoded _itemCount;
            private List<GetDataResult> _items;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public LengthEncoded ItemCount { get { return _itemCount; } }
            public List<GetDataResult> Items { get { return _items; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class InvokeIdAndPriority : KaitaiStruct
        {
            public static InvokeIdAndPriority FromFile(string fileName)
            {
                return new InvokeIdAndPriority(new KaitaiStream(fileName));
            }

            public InvokeIdAndPriority(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _priority = m_io.ReadBitsInt(1) != 0;
                _serviceClass = m_io.ReadBitsInt(1) != 0;
                _invokeId = m_io.ReadBitsInt(6);
            }
            private bool _priority;
            private bool _serviceClass;
            private ulong _invokeId;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public bool Priority { get { return _priority; } }
            public bool ServiceClass { get { return _serviceClass; } }
            public ulong InvokeId { get { return _invokeId; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class SequenceOfDataAccessResult : KaitaiStruct
        {
            public static SequenceOfDataAccessResult FromFile(string fileName)
            {
                return new SequenceOfDataAccessResult(new KaitaiStream(fileName));
            }

            public SequenceOfDataAccessResult(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _itemCount = new LengthEncoded(m_io, this, m_root);
                _items = new List<DataAccessResult>((int)(ItemCount.Value));
                for (var i = 0; i < ItemCount.Value; i++)
                {
                    _items.Add(new DataAccessResult(m_io, this, m_root));
                }
            }
            private LengthEncoded _itemCount;
            private List<DataAccessResult> _items;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public LengthEncoded ItemCount { get { return _itemCount; } }
            public List<DataAccessResult> Items { get { return _items; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class SelectiveAccessDescriptor : KaitaiStruct
        {
            public static SelectiveAccessDescriptor FromFile(string fileName)
            {
                return new SelectiveAccessDescriptor(new KaitaiStream(fileName));
            }

            public SelectiveAccessDescriptor(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _accessSelector = m_io.ReadU1();
                _accessParameters = new DlmsData(m_io);
            }
            private byte _accessSelector;
            private DlmsData _accessParameters;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte AccessSelector { get { return _accessSelector; } }
            public DlmsData AccessParameters { get { return _accessParameters; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class CosemDateTimeOptional : KaitaiStruct
        {
            public static CosemDateTimeOptional FromFile(string fileName)
            {
                return new CosemDateTimeOptional(new KaitaiStream(fileName));
            }

            public CosemDateTimeOptional(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
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
                    _value = new CosemDateTime(m_io, this, m_root);
                }
            }
            private byte _present;
            private CosemDateTime _value;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte Present { get { return _present; } }
            public CosemDateTime Value { get { return _value; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class DataAccessResult : KaitaiStruct
        {
            public static DataAccessResult FromFile(string fileName)
            {
                return new DataAccessResult(new KaitaiStream(fileName));
            }

            public DataAccessResult(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _value = ((DlmsStruct.AccessResult)m_io.ReadU1());
            }
            private AccessResult _value;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public AccessResult Value { get { return _value; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class SelectiveAccessDescriptorOptional : KaitaiStruct
        {
            public static SelectiveAccessDescriptorOptional FromFile(string fileName)
            {
                return new SelectiveAccessDescriptorOptional(new KaitaiStream(fileName));
            }

            public SelectiveAccessDescriptorOptional(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
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
                    _value = new SelectiveAccessDescriptor(m_io, this, m_root);
                }
            }
            private byte _present;
            private SelectiveAccessDescriptor _value;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte Present { get { return _present; } }
            public SelectiveAccessDescriptor Value { get { return _value; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class GetDataResultOptional : KaitaiStruct
        {
            public static GetDataResultOptional FromFile(string fileName)
            {
                return new GetDataResultOptional(new KaitaiStream(fileName));
            }

            public GetDataResultOptional(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
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
                    _value = new GetDataResult(m_io, this, m_root);
                }
            }
            private byte _present;
            private GetDataResult _value;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public byte Present { get { return _present; } }
            public GetDataResult Value { get { return _value; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class CosemAttributeDescriptor : KaitaiStruct
        {
            public static CosemAttributeDescriptor FromFile(string fileName)
            {
                return new CosemAttributeDescriptor(new KaitaiStream(fileName));
            }

            public CosemAttributeDescriptor(KaitaiStream io, KaitaiStruct parent = null, DlmsStruct root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _classId = m_io.ReadU2be();
                _instanceId = new CosemObjectInstanceId(m_io, this, m_root);
                _attributeId = m_io.ReadU1();
            }
            private ushort _classId;
            private CosemObjectInstanceId _instanceId;
            private byte _attributeId;
            private DlmsStruct m_root;
            private KaitaiStruct m_parent;
            public ushort ClassId { get { return _classId; } }
            public CosemObjectInstanceId InstanceId { get { return _instanceId; } }
            public byte AttributeId { get { return _attributeId; } }
            public DlmsStruct M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        private DlmsStruct m_root;
        private KaitaiStruct m_parent;
        public DlmsStruct M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
