// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsActionRequest : KaitaiStruct
    {
        public static DlmsActionRequest FromFile(string fileName)
        {
            return new DlmsActionRequest(new KaitaiStream(fileName));
        }

        public enum ActionRequestType
        {
            ActionRequestNormal = 1,
            ActionRequestNextPblock = 2,
            ActionRequestWithList = 3,
            ActionRequestWithFirstPblock = 4,
            ActionRequestWithListAndFirstPblock = 5,
            ActionRequestWithPblock = 6,
        }

        public DlmsActionRequest(KaitaiStream io, KaitaiStruct parent = null, DlmsActionRequest root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _requestType = ((ActionRequestType)m_io.ReadU1());
            switch (RequestType)
            {
                case ActionRequestType.ActionRequestWithList:
                    {
                        _request = new ActionRequestWithList(m_io, this, m_root);
                        break;
                    }
                case ActionRequestType.ActionRequestNormal:
                    {
                        _request = new ActionRequestNormal(m_io, this, m_root);
                        break;
                    }
                case ActionRequestType.ActionRequestWithListAndFirstPblock:
                    {
                        _request = new ActionRequestWithListAndFirstPblock(m_io, this, m_root);
                        break;
                    }
                case ActionRequestType.ActionRequestWithFirstPblock:
                    {
                        _request = new ActionRequestWithFirstPblock(m_io, this, m_root);
                        break;
                    }
                case ActionRequestType.ActionRequestNextPblock:
                    {
                        _request = new ActionRequestNextPblock(m_io, this, m_root);
                        break;
                    }
                case ActionRequestType.ActionRequestWithPblock:
                    {
                        _request = new ActionRequestWithPblock(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class ActionRequestWithFirstPblock : KaitaiStruct
        {
            public static ActionRequestWithFirstPblock FromFile(string fileName)
            {
                return new ActionRequestWithFirstPblock(new KaitaiStream(fileName));
            }

            public ActionRequestWithFirstPblock(KaitaiStream io, DlmsActionRequest parent = null, DlmsActionRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsActionRequest m_root;
            private DlmsActionRequest m_parent;
            public DlmsActionRequest M_Root { get { return m_root; } }
            public DlmsActionRequest M_Parent { get { return m_parent; } }
        }
        public partial class ActionRequestWithListAndFirstPblock : KaitaiStruct
        {
            public static ActionRequestWithListAndFirstPblock FromFile(string fileName)
            {
                return new ActionRequestWithListAndFirstPblock(new KaitaiStream(fileName));
            }

            public ActionRequestWithListAndFirstPblock(KaitaiStream io, DlmsActionRequest parent = null, DlmsActionRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsActionRequest m_root;
            private DlmsActionRequest m_parent;
            public DlmsActionRequest M_Root { get { return m_root; } }
            public DlmsActionRequest M_Parent { get { return m_parent; } }
        }
        public partial class ActionRequestNormal : KaitaiStruct
        {
            public static ActionRequestNormal FromFile(string fileName)
            {
                return new ActionRequestNormal(new KaitaiStream(fileName));
            }

            public ActionRequestNormal(KaitaiStream io, DlmsActionRequest parent = null, DlmsActionRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _cosemMethodDescriptor = new DlmsStruct.CosemMethodDescriptor(m_io);
                _methodInvocationParameters = new DlmsStruct.DataOptional(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.CosemMethodDescriptor _cosemMethodDescriptor;
            private DlmsStruct.DataOptional _methodInvocationParameters;
            private DlmsActionRequest m_root;
            private DlmsActionRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.CosemMethodDescriptor CosemMethodDescriptor { get { return _cosemMethodDescriptor; } }
            public DlmsStruct.DataOptional MethodInvocationParameters { get { return _methodInvocationParameters; } }
            public DlmsActionRequest M_Root { get { return m_root; } }
            public DlmsActionRequest M_Parent { get { return m_parent; } }
        }
        public partial class ActionRequestNextPblock : KaitaiStruct
        {
            public static ActionRequestNextPblock FromFile(string fileName)
            {
                return new ActionRequestNextPblock(new KaitaiStream(fileName));
            }

            public ActionRequestNextPblock(KaitaiStream io, DlmsActionRequest parent = null, DlmsActionRequest root = null) : base(io)
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
            private DlmsActionRequest m_root;
            private DlmsActionRequest m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public uint BlockNumber { get { return _blockNumber; } }
            public DlmsActionRequest M_Root { get { return m_root; } }
            public DlmsActionRequest M_Parent { get { return m_parent; } }
        }
        public partial class ActionRequestWithPblock : KaitaiStruct
        {
            public static ActionRequestWithPblock FromFile(string fileName)
            {
                return new ActionRequestWithPblock(new KaitaiStream(fileName));
            }

            public ActionRequestWithPblock(KaitaiStream io, DlmsActionRequest parent = null, DlmsActionRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsActionRequest m_root;
            private DlmsActionRequest m_parent;
            public DlmsActionRequest M_Root { get { return m_root; } }
            public DlmsActionRequest M_Parent { get { return m_parent; } }
        }
        public partial class ActionRequestWithList : KaitaiStruct
        {
            public static ActionRequestWithList FromFile(string fileName)
            {
                return new ActionRequestWithList(new KaitaiStream(fileName));
            }

            public ActionRequestWithList(KaitaiStream io, DlmsActionRequest parent = null, DlmsActionRequest root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsActionRequest m_root;
            private DlmsActionRequest m_parent;
            public DlmsActionRequest M_Root { get { return m_root; } }
            public DlmsActionRequest M_Parent { get { return m_parent; } }
        }
        private ActionRequestType _requestType;
        private KaitaiStruct _request;
        private DlmsActionRequest m_root;
        private KaitaiStruct m_parent;
        public ActionRequestType RequestType { get { return _requestType; } }
        public KaitaiStruct Request { get { return _request; } }
        public DlmsActionRequest M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
