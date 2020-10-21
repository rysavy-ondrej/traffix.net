// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Industrial
{
    public partial class TpktPacket : KaitaiStruct
    {
        public static TpktPacket FromFile(string fileName)
        {
            return new TpktPacket(new KaitaiStream(fileName));
        }


        public enum CotpType
        {
            DisconnectRequest = 128,
            DiconnectConfirm = 192,
            ConnectionConfirm = 208,
            ConnectionRequest = 224,
            DataTransfer = 240,
        }
        public TpktPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, TpktPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _version = m_io.ReadU1();
            _reserved = m_io.ReadU1();
            _length = m_io.ReadU2be();
            _cotp = new CotpPacket(m_io, this, m_root);
            _payload = m_io.ReadBytes((Length - M_Io.Pos));
        }
        public partial class CotpPacket : KaitaiStruct
        {
            public static CotpPacket FromFile(string fileName)
            {
                return new CotpPacket(new KaitaiStream(fileName));
            }

            public CotpPacket(KaitaiStream p__io, TpktPacket p__parent = null, TpktPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _length = m_io.ReadU1();
                _pduType = ((TpktPacket.CotpType) m_io.ReadU1());
                switch (PduType) {
                case TpktPacket.CotpType.DataTransfer: {
                    __raw_options = m_io.ReadBytes((Length - 1));
                    var io___raw_options = new KaitaiStream(__raw_options);
                    _options = new CotpDataTransfer(io___raw_options, this, m_root);
                    break;
                }
                default: {
                    __raw_options = m_io.ReadBytes((Length - 1));
                    var io___raw_options = new KaitaiStream(__raw_options);
                    _options = new CotpOptions(io___raw_options, this, m_root);
                    break;
                }
                }
            }
            private byte _length;
            private CotpType _pduType;
            private KaitaiStruct _options;
            private TpktPacket m_root;
            private TpktPacket m_parent;
            private byte[] __raw_options;
            public byte Length { get { return _length; } }
            public CotpType PduType { get { return _pduType; } }
            public KaitaiStruct Options { get { return _options; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket M_Parent { get { return m_parent; } }
            public byte[] M_RawOptions { get { return __raw_options; } }
        }
        public partial class CotpOptions : KaitaiStruct
        {
            public static CotpOptions FromFile(string fileName)
            {
                return new CotpOptions(new KaitaiStream(fileName));
            }

            public CotpOptions(KaitaiStream p__io, TpktPacket.CotpPacket p__parent = null, TpktPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _bytes = m_io.ReadBytesFull();
            }
            private byte[] _bytes;
            private TpktPacket m_root;
            private TpktPacket.CotpPacket m_parent;
            public byte[] Bytes { get { return _bytes; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket.CotpPacket M_Parent { get { return m_parent; } }
        }
        public partial class CotpDataTransfer : KaitaiStruct
        {
            public static CotpDataTransfer FromFile(string fileName)
            {
                return new CotpDataTransfer(new KaitaiStream(fileName));
            }

            public CotpDataTransfer(KaitaiStream p__io, TpktPacket.CotpPacket p__parent = null, TpktPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _lastData = m_io.ReadBitsInt(1) != 0;
                _tpduNumber = m_io.ReadBitsInt(7);
                m_io.AlignToByte();
                _data = m_io.ReadBytesFull();
            }
            private bool _lastData;
            private ulong _tpduNumber;
            private byte[] _data;
            private TpktPacket m_root;
            private TpktPacket.CotpPacket m_parent;
            public bool LastData { get { return _lastData; } }
            public ulong TpduNumber { get { return _tpduNumber; } }
            public byte[] Data { get { return _data; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket.CotpPacket M_Parent { get { return m_parent; } }
        }
        private byte _version;
        private byte _reserved;
        private ushort _length;
        private CotpPacket _cotp;
        private byte[] _payload;
        private TpktPacket m_root;
        private KaitaiStruct m_parent;
        public byte Version { get { return _version; } }
        public byte Reserved { get { return _reserved; } }
        public ushort Length { get { return _length; } }
        public CotpPacket Cotp { get { return _cotp; } }
        public byte[] Payload { get { return _payload; } }
        public TpktPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
