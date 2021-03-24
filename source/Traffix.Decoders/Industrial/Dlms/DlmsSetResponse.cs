// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class DlmsSetResponse : KaitaiStruct
    {
        public static DlmsSetResponse FromFile(string fileName)
        {
            return new DlmsSetResponse(new KaitaiStream(fileName));
        }

        public enum SetResponseType
        {
            SetResponseNormal = 1,
            SetResponseDatablock = 2,
            SetResponseLastDatablock = 3,
            SetResponseLastDatablockWithList = 4,
            SetResponseWithList = 5,
        }

        public DlmsSetResponse(KaitaiStream io, KaitaiStruct parent = null, DlmsSetResponse root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _responseType = ((SetResponseType)m_io.ReadU1());
            switch (ResponseType)
            {
                case SetResponseType.SetResponseWithList:
                    {
                        _response = new SetResponseWithList(m_io, this, m_root);
                        break;
                    }
                case SetResponseType.SetResponseNormal:
                    {
                        _response = new SetResponseNormal(m_io, this, m_root);
                        break;
                    }
                case SetResponseType.SetResponseLastDatablock:
                    {
                        _response = new SetResponseLastDatablock(m_io, this, m_root);
                        break;
                    }
                case SetResponseType.SetResponseDatablock:
                    {
                        _response = new SetResponseDatablock(m_io, this, m_root);
                        break;
                    }
                case SetResponseType.SetResponseLastDatablockWithList:
                    {
                        _response = new SetResponseLastDatablockWithList(m_io, this, m_root);
                        break;
                    }
            }
        }
        public partial class SetResponseLastDatablock : KaitaiStruct
        {
            public static SetResponseLastDatablock FromFile(string fileName)
            {
                return new SetResponseLastDatablock(new KaitaiStream(fileName));
            }

            public SetResponseLastDatablock(KaitaiStream io, DlmsSetResponse parent = null, DlmsSetResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _result = new DlmsStruct.DataAccessResult(m_io);
                _blockNumber = m_io.ReadU4be();
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.DataAccessResult _result;
            private uint _blockNumber;
            private DlmsSetResponse m_root;
            private DlmsSetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.DataAccessResult Result { get { return _result; } }
            public uint BlockNumber { get { return _blockNumber; } }
            public DlmsSetResponse M_Root { get { return m_root; } }
            public DlmsSetResponse M_Parent { get { return m_parent; } }
        }
        public partial class SetResponseDatablock : KaitaiStruct
        {
            public static SetResponseDatablock FromFile(string fileName)
            {
                return new SetResponseDatablock(new KaitaiStream(fileName));
            }

            public SetResponseDatablock(KaitaiStream io, DlmsSetResponse parent = null, DlmsSetResponse root = null) : base(io)
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
            private DlmsSetResponse m_root;
            private DlmsSetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public uint BlockNumber { get { return _blockNumber; } }
            public DlmsSetResponse M_Root { get { return m_root; } }
            public DlmsSetResponse M_Parent { get { return m_parent; } }
        }
        public partial class SetResponseLastDatablockWithList : KaitaiStruct
        {
            public static SetResponseLastDatablockWithList FromFile(string fileName)
            {
                return new SetResponseLastDatablockWithList(new KaitaiStream(fileName));
            }

            public SetResponseLastDatablockWithList(KaitaiStream io, DlmsSetResponse parent = null, DlmsSetResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _result = new DlmsStruct.SequenceOfDataAccessResult(m_io);
                _blockNumber = m_io.ReadU4be();
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.SequenceOfDataAccessResult _result;
            private uint _blockNumber;
            private DlmsSetResponse m_root;
            private DlmsSetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.SequenceOfDataAccessResult Result { get { return _result; } }
            public uint BlockNumber { get { return _blockNumber; } }
            public DlmsSetResponse M_Root { get { return m_root; } }
            public DlmsSetResponse M_Parent { get { return m_parent; } }
        }
        public partial class SetResponseWithList : KaitaiStruct
        {
            public static SetResponseWithList FromFile(string fileName)
            {
                return new SetResponseWithList(new KaitaiStream(fileName));
            }

            public SetResponseWithList(KaitaiStream io, DlmsSetResponse parent = null, DlmsSetResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _result = new DlmsStruct.SequenceOfDataAccessResult(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.SequenceOfDataAccessResult _result;
            private DlmsSetResponse m_root;
            private DlmsSetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.SequenceOfDataAccessResult Result { get { return _result; } }
            public DlmsSetResponse M_Root { get { return m_root; } }
            public DlmsSetResponse M_Parent { get { return m_parent; } }
        }
        public partial class SetResponseNormal : KaitaiStruct
        {
            public static SetResponseNormal FromFile(string fileName)
            {
                return new SetResponseNormal(new KaitaiStream(fileName));
            }

            public SetResponseNormal(KaitaiStream io, DlmsSetResponse parent = null, DlmsSetResponse root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _invokeIdAndPriority = new DlmsStruct.InvokeIdAndPriority(m_io);
                _result = new DlmsStruct.DataAccessResult(m_io);
            }
            private DlmsStruct.InvokeIdAndPriority _invokeIdAndPriority;
            private DlmsStruct.DataAccessResult _result;
            private DlmsSetResponse m_root;
            private DlmsSetResponse m_parent;
            public DlmsStruct.InvokeIdAndPriority InvokeIdAndPriority { get { return _invokeIdAndPriority; } }
            public DlmsStruct.DataAccessResult Result { get { return _result; } }
            public DlmsSetResponse M_Root { get { return m_root; } }
            public DlmsSetResponse M_Parent { get { return m_parent; } }
        }
        private SetResponseType _responseType;
        private KaitaiStruct _response;
        private DlmsSetResponse m_root;
        private KaitaiStruct m_parent;
        public SetResponseType ResponseType { get { return _responseType; } }
        public KaitaiStruct Response { get { return _response; } }
        public DlmsSetResponse M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
