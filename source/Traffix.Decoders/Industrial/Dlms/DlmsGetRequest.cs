// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsGetRequest : KaitaiStruct
    {
        public static DlmsGetRequest FromFile(string fileName)
        {
            return new DlmsGetRequest(new KaitaiStream(fileName));
        }

        public enum GetRequestType
        {
            GetRequestNormal = 1,
            GetRequestNext = 2,
            GetRequestWithList = 3,
        }

        public DlmsGetRequest(KaitaiStream io, KaitaiStruct parent = null, DlmsGetRequest root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _requestType = ((GetRequestType)m_io.ReadU1());
            switch (RequestType)
            {
                case GetRequestType.GetRequestNormal:
                    {
                        _request = new GetRequestNormal(m_io, this, m_root);
                        break;
                    }
                case GetRequestType.GetRequestNext:
                    {
                        _request = new GetRequestNext(m_io, this, m_root);
                        break;
                    }
                case GetRequestType.GetRequestWithList:
                    {
                        _request = new GetRequestWithList(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class GetRequestNormal : KaitaiStruct
        {
            public static GetRequestNormal FromFile(string fileName)
            {
                return new GetRequestNormal(new KaitaiStream(fileName));
            }

            public GetRequestNormal(KaitaiStream io, DlmsGetRequest parent = null, DlmsGetRequest root = null) : base(io)
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
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.CosemAttributeDescriptor _cosemAttributeDescriptor;
            private DlmsStruct.SelectiveAccessDescriptorOptional _accessSelection;
            private DlmsGetRequest m_root;
            private DlmsGetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.CosemAttributeDescriptor CosemAttributeDescriptor { get { return _cosemAttributeDescriptor; } }
            public DlmsStruct.SelectiveAccessDescriptorOptional AccessSelection { get { return _accessSelection; } }
            public DlmsGetRequest M_Root { get { return m_root; } }
            public DlmsGetRequest M_Parent { get { return m_parent; } }
        }
        public partial class GetRequestNext : KaitaiStruct
        {
            public static GetRequestNext FromFile(string fileName)
            {
                return new GetRequestNext(new KaitaiStream(fileName));
            }

            public GetRequestNext(KaitaiStream io, DlmsGetRequest parent = null, DlmsGetRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _blockNumber = m_io.ReadU4be();
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private uint _blockNumber;
            private DlmsGetRequest m_root;
            private DlmsGetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public uint BlockNumber { get { return _blockNumber; } }
            public DlmsGetRequest M_Root { get { return m_root; } }
            public DlmsGetRequest M_Parent { get { return m_parent; } }
        }
        public partial class GetRequestWithList : KaitaiStruct
        {
            public static GetRequestWithList FromFile(string fileName)
            {
                return new GetRequestWithList(new KaitaiStream(fileName));
            }

            public GetRequestWithList(KaitaiStream io, DlmsGetRequest parent = null, DlmsGetRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _attributeDescriptorList = new DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection _attributeDescriptorList;
            private DlmsGetRequest m_root;
            private DlmsGetRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.SequenceOfCosemAttributeDescriptorWithSelection AttributeDescriptorList { get { return _attributeDescriptorList; } }
            public DlmsGetRequest M_Root { get { return m_root; } }
            public DlmsGetRequest M_Parent { get { return m_parent; } }
        }
        private GetRequestType _requestType;
        private KaitaiStruct _request;
        private DlmsGetRequest m_root;
        private KaitaiStruct m_parent;
        public GetRequestType RequestType { get { return _requestType; } }
        public KaitaiStruct Request { get { return _request; } }
        public DlmsGetRequest M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
