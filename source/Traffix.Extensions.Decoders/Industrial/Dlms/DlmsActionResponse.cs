// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsActionResponse : KaitaiStruct
    {
        public static DlmsActionResponse FromFile(string fileName)
        {
            return new DlmsActionResponse(new KaitaiStream(fileName));
        }

        public enum ActionResponseType
        {
            ActionResponseNormal = 1,
            ActionResponseWithPblock = 2,
            ActionResponseWithList = 3,
            ActionResponseNextPblock = 4,
        }

        public enum ActionResult
        {
            Success = 0,
            HardwareFault = 1,
            TemporaryFailure = 2,
            ReadWriteDenied = 3,
            ObjectUndefined = 4,
            ObjectClassInconsistent = 5,
            ObjectUnavailable = 11,
            TypeUnmatched = 12,
            ScopeOfAccessViolated = 13,
            DataBlockUnavailable = 14,
            LongActionAborted = 15,
            NoLongActionInProgress = 16,
            OtherReason = 250,
        }

        public DlmsActionResponse(KaitaiStream io, KaitaiStruct parent = null, DlmsActionResponse root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _responseType = ((ActionResponseType)m_io.ReadU1());
            switch (ResponseType)
            {
                case ActionResponseType.ActionResponseNormal:
                    {
                        _response = new ActionResponseNormal(m_io, this, m_root);
                        break;
                    }
                case ActionResponseType.ActionResponseWithPblock:
                    {
                        _response = new ActionResponseWithPblock(m_io, this, m_root);
                        break;
                    }
                case ActionResponseType.ActionResponseWithList:
                    {
                        _response = new ActionResponseWithList(m_io, this, m_root);
                        break;
                    }
                case ActionResponseType.ActionResponseNextPblock:
                    {
                        _response = new ActionResponseNextPblock(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class ActionResponseWithPblock : KaitaiStruct
        {
            public static ActionResponseWithPblock FromFile(string fileName)
            {
                return new ActionResponseWithPblock(new KaitaiStream(fileName));
            }

            public ActionResponseWithPblock(KaitaiStream io, DlmsActionResponse parent = null, DlmsActionResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsActionResponse m_root;
            private DlmsActionResponse m_parent;
            public DlmsActionResponse M_Root { get { return m_root; } }
            public DlmsActionResponse M_Parent { get { return m_parent; } }
        }
        public partial class ActionResponseWithOptionalData : KaitaiStruct
        {
            public static ActionResponseWithOptionalData FromFile(string fileName)
            {
                return new ActionResponseWithOptionalData(new KaitaiStream(fileName));
            }

            public ActionResponseWithOptionalData(KaitaiStream io, ActionResponseNormal parent = null, DlmsActionResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _result = ((DlmsActionResponse.ActionResult)m_io.ReadU1());
                _returnParameters = new DlmsStruct.GetDataResultOptional(m_io);
            }
            private ActionResult _result;
            private DlmsStruct.GetDataResultOptional _returnParameters;
            private DlmsActionResponse m_root;
            private DlmsActionResponse.ActionResponseNormal m_parent;
            public ActionResult Result { get { return _result; } }
            public DlmsStruct.GetDataResultOptional ReturnParameters { get { return _returnParameters; } }
            public DlmsActionResponse M_Root { get { return m_root; } }
            public DlmsActionResponse.ActionResponseNormal M_Parent { get { return m_parent; } }
        }
        public partial class ActionResponseNormal : KaitaiStruct
        {
            public static ActionResponseNormal FromFile(string fileName)
            {
                return new ActionResponseNormal(new KaitaiStream(fileName));
            }

            public ActionResponseNormal(KaitaiStream io, DlmsActionResponse parent = null, DlmsActionResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _singleResponse = new ActionResponseWithOptionalData(m_io, this, m_root);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private ActionResponseWithOptionalData _singleResponse;
            private DlmsActionResponse m_root;
            private DlmsActionResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public ActionResponseWithOptionalData SingleResponse { get { return _singleResponse; } }
            public DlmsActionResponse M_Root { get { return m_root; } }
            public DlmsActionResponse M_Parent { get { return m_parent; } }
        }
        public partial class ActionResponseNextPblock : KaitaiStruct
        {
            public static ActionResponseNextPblock FromFile(string fileName)
            {
                return new ActionResponseNextPblock(new KaitaiStream(fileName));
            }

            public ActionResponseNextPblock(KaitaiStream io, DlmsActionResponse parent = null, DlmsActionResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsActionResponse m_root;
            private DlmsActionResponse m_parent;
            public DlmsActionResponse M_Root { get { return m_root; } }
            public DlmsActionResponse M_Parent { get { return m_parent; } }
        }
        public partial class ActionResponseWithList : KaitaiStruct
        {
            public static ActionResponseWithList FromFile(string fileName)
            {
                return new ActionResponseWithList(new KaitaiStream(fileName));
            }

            public ActionResponseWithList(KaitaiStream io, DlmsActionResponse parent = null, DlmsActionResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
            }
            private DlmsActionResponse m_root;
            private DlmsActionResponse m_parent;
            public DlmsActionResponse M_Root { get { return m_root; } }
            public DlmsActionResponse M_Parent { get { return m_parent; } }
        }
        private ActionResponseType _responseType;
        private KaitaiStruct _response;
        private DlmsActionResponse m_root;
        private KaitaiStruct m_parent;
        public ActionResponseType ResponseType { get { return _responseType; } }
        public KaitaiStruct Response { get { return _response; } }
        public DlmsActionResponse M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
