// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsGetResponse : KaitaiStruct
    {
        public static DlmsGetResponse FromFile(string fileName)
        {
            return new DlmsGetResponse(new KaitaiStream(fileName));
        }

        public enum GetResponseType
        {
            GetResponseNormal = 1,
            GetResponseNext = 2,
            GetResponseWithList = 3,
        }

        public DlmsGetResponse(KaitaiStream io, KaitaiStruct parent = null, DlmsGetResponse root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _responseType = ((GetResponseType)m_io.ReadU1());
            switch (ResponseType)
            {
                case GetResponseType.GetResponseNormal:
                    {
                        _response = new GetResponseNormal(m_io, this, m_root);
                        break;
                    }
                case GetResponseType.GetResponseNext:
                    {
                        _response = new GetResponseWithDatablock(m_io, this, m_root);
                        break;
                    }
                case GetResponseType.GetResponseWithList:
                    {
                        _response = new GetResponseWithList(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class GetResponseNormal : KaitaiStruct
        {
            public static GetResponseNormal FromFile(string fileName)
            {
                return new GetResponseNormal(new KaitaiStream(fileName));
            }

            public GetResponseNormal(KaitaiStream io, DlmsGetResponse parent = null, DlmsGetResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _result = new DlmsStruct.GetDataResult(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.GetDataResult _result;
            private DlmsGetResponse m_root;
            private DlmsGetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.GetDataResult Result { get { return _result; } }
            public DlmsGetResponse M_Root { get { return m_root; } }
            public DlmsGetResponse M_Parent { get { return m_parent; } }
        }
        public partial class GetResponseWithDatablock : KaitaiStruct
        {
            public static GetResponseWithDatablock FromFile(string fileName)
            {
                return new GetResponseWithDatablock(new KaitaiStream(fileName));
            }

            public GetResponseWithDatablock(KaitaiStream io, DlmsGetResponse parent = null, DlmsGetResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _result = new DlmsStruct.DatablockG(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.DatablockG _result;
            private DlmsGetResponse m_root;
            private DlmsGetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.DatablockG Result { get { return _result; } }
            public DlmsGetResponse M_Root { get { return m_root; } }
            public DlmsGetResponse M_Parent { get { return m_parent; } }
        }
        public partial class GetResponseWithList : KaitaiStruct
        {
            public static GetResponseWithList FromFile(string fileName)
            {
                return new GetResponseWithList(new KaitaiStream(fileName));
            }

            public GetResponseWithList(KaitaiStream io, DlmsGetResponse parent = null, DlmsGetResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _result = new DlmsStruct.SequenceOfGetDataResult(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.SequenceOfGetDataResult _result;
            private DlmsGetResponse m_root;
            private DlmsGetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.SequenceOfGetDataResult Result { get { return _result; } }
            public DlmsGetResponse M_Root { get { return m_root; } }
            public DlmsGetResponse M_Parent { get { return m_parent; } }
        }
        private GetResponseType _responseType;
        private KaitaiStruct _response;
        private DlmsGetResponse m_root;
        private KaitaiStruct m_parent;
        public GetResponseType ResponseType { get { return _responseType; } }
        public KaitaiStruct Response { get { return _response; } }
        public DlmsGetResponse M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
