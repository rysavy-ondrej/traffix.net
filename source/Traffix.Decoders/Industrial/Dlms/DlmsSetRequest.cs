// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsSetRequest : KaitaiStruct
    {
        public static DlmsSetRequest FromFile(string fileName)
        {
            return new DlmsSetRequest(new KaitaiStream(fileName));
        }

        public enum SetRequestType
        {
            SetRequestNormal = 1,
            SetRequestWithFirstDatablock = 2,
            SetRequestWithDatablock = 3,
            SetRequestWithList = 4,
            SetRequestWithListAndFirstDatablock = 5,
        }

        public DlmsSetRequest(KaitaiStream io, KaitaiStruct parent = null, DlmsSetRequest root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _requestType = ((SetRequestType)m_io.ReadU1());
            switch (RequestType)
            {
                case SetRequestType.SetRequestWithListAndFirstDatablock:
                    {
                        _request = new SetRequestWithListAndFirstDatablock(m_io, this, m_root);
                        break;
                    }
                case SetRequestType.SetRequestWithFirstDatablock:
                    {
                        _request = new SetRequestWithFirstDatablock(m_io, this, m_root);
                        break;
                    }
                case SetRequestType.SetRequestWithList:
                    {
                        _request = new SetRequestWithList(m_io, this, m_root);
                        break;
                    }
                case SetRequestType.SetRequestNormal:
                    {
                        _request = new SetRequestNormal(m_io, this, m_root);
                        break;
                    }
                case SetRequestType.SetRequestWithDatablock:
                    {
                        _request = new SetRequestWithDatablock(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class SetRequestWithListAndFirstDatablock : KaitaiStruct
        {
            public static SetRequestWithListAndFirstDatablock FromFile(string fileName)
            {
                return new SetRequestWithListAndFirstDatablock(new KaitaiStream(fileName));
            }

            public SetRequestWithListAndFirstDatablock(KaitaiStream io, DlmsSetRequest parent = null, DlmsSetRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _attributeDescriptorList = new DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection(m_io);
                _datablock = new DlmsStruct.DatablockSa(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection _attributeDescriptorList;
            private DlmsStruct.DatablockSa _datablock;
            private DlmsSetRequest m_root;
            private DlmsSetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection AttributeDescriptorList { get { return _attributeDescriptorList; } }
            public DlmsStruct.DatablockSa Datablock { get { return _datablock; } }
            public DlmsSetRequest M_Root { get { return m_root; } }
            public DlmsSetRequest M_Parent { get { return m_parent; } }
        }
        public partial class SetRequestWithFirstDatablock : KaitaiStruct
        {
            public static SetRequestWithFirstDatablock FromFile(string fileName)
            {
                return new SetRequestWithFirstDatablock(new KaitaiStream(fileName));
            }

            public SetRequestWithFirstDatablock(KaitaiStream io, DlmsSetRequest parent = null, DlmsSetRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _datablock = new DlmsStruct.DatablockSa(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.DatablockSa _datablock;
            private DlmsSetRequest m_root;
            private DlmsSetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.DatablockSa Datablock { get { return _datablock; } }
            public DlmsSetRequest M_Root { get { return m_root; } }
            public DlmsSetRequest M_Parent { get { return m_parent; } }
        }
        public partial class SetRequestWithDatablock : KaitaiStruct
        {
            public static SetRequestWithDatablock FromFile(string fileName)
            {
                return new SetRequestWithDatablock(new KaitaiStream(fileName));
            }

            public SetRequestWithDatablock(KaitaiStream io, DlmsSetRequest parent = null, DlmsSetRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _cosemAttributeDescriptor = new DlmsStruct.CosemAttributeDescriptor(m_io);
                _accessSelection = new DlmsStruct.SelectiveAccessDescriptorOptional(m_io);
                _datablock = new DlmsStruct.DatablockSa(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.CosemAttributeDescriptor _cosemAttributeDescriptor;
            private DlmsStruct.SelectiveAccessDescriptorOptional _accessSelection;
            private DlmsStruct.DatablockSa _datablock;
            private DlmsSetRequest m_root;
            private DlmsSetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.CosemAttributeDescriptor CosemAttributeDescriptor { get { return _cosemAttributeDescriptor; } }
            public DlmsStruct.SelectiveAccessDescriptorOptional AccessSelection { get { return _accessSelection; } }
            public DlmsStruct.DatablockSa Datablock { get { return _datablock; } }
            public DlmsSetRequest M_Root { get { return m_root; } }
            public DlmsSetRequest M_Parent { get { return m_parent; } }
        }
        public partial class SetRequestWithList : KaitaiStruct
        {
            public static SetRequestWithList FromFile(string fileName)
            {
                return new SetRequestWithList(new KaitaiStream(fileName));
            }

            public SetRequestWithList(KaitaiStream io, DlmsSetRequest parent = null, DlmsSetRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _attributeDescriptorList = new DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection(m_io);
                _valueList = new DlmsStruct.SequenceOfData(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection _attributeDescriptorList;
            private DlmsStruct.SequenceOfData _valueList;
            private DlmsSetRequest m_root;
            private DlmsSetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection AttributeDescriptorList { get { return _attributeDescriptorList; } }
            public DlmsStruct.SequenceOfData ValueList { get { return _valueList; } }
            public DlmsSetRequest M_Root { get { return m_root; } }
            public DlmsSetRequest M_Parent { get { return m_parent; } }
        }
        public partial class SetRequestNormal : KaitaiStruct
        {
            public static SetRequestNormal FromFile(string fileName)
            {
                return new SetRequestNormal(new KaitaiStream(fileName));
            }

            public SetRequestNormal(KaitaiStream io, DlmsSetRequest parent = null, DlmsSetRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _cosemAttributeDescriptor = new DlmsStruct.CosemAttributeDescriptor(m_io);
                _accessSelection = new DlmsStruct.SelectiveAccessDescriptorOptional(m_io);
                _value = new DlmsData(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.CosemAttributeDescriptor _cosemAttributeDescriptor;
            private DlmsStruct.SelectiveAccessDescriptorOptional _accessSelection;
            private DlmsData _value;
            private DlmsSetRequest m_root;
            private DlmsSetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.CosemAttributeDescriptor CosemAttributeDescriptor { get { return _cosemAttributeDescriptor; } }
            public DlmsStruct.SelectiveAccessDescriptorOptional AccessSelection { get { return _accessSelection; } }
            public DlmsData Value { get { return _value; } }
            public DlmsSetRequest M_Root { get { return m_root; } }
            public DlmsSetRequest M_Parent { get { return m_parent; } }
        }
        private SetRequestType _requestType;
        private KaitaiStruct _request;
        private DlmsSetRequest m_root;
        private KaitaiStruct m_parent;
        public SetRequestType RequestType { get { return _requestType; } }
        public KaitaiStruct Request { get { return _request; } }
        public DlmsSetRequest M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
