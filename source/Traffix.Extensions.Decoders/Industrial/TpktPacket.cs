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
            _tptk = new TptkHeader(m_io, this, m_root);
            _cotp = new CotpHeader(m_io, this, m_root);
            __raw_opts = m_io.ReadBytes((Cotp.Length - 1));
            var io___raw_opts = new KaitaiStream(__raw_opts);
            _opts = new CotpOptions(Cotp.PduType, io___raw_opts, this, m_root);
            _payload = m_io.ReadBytes((Tptk.Length - M_Io.Pos));
        }
        public partial class CotpHeader : KaitaiStruct
        {
            public static CotpHeader FromFile(string fileName)
            {
                return new CotpHeader(new KaitaiStream(fileName));
            }

            public CotpHeader(KaitaiStream p__io, TpktPacket p__parent = null, TpktPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _length = m_io.ReadU1();
                _pduType = m_io.ReadU1();
            }
            private byte _length;
            private byte _pduType;
            private TpktPacket m_root;
            private TpktPacket m_parent;
            public byte Length { get { return _length; } }
            public byte PduType { get { return _pduType; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket M_Parent { get { return m_parent; } }
        }
        public partial class CotpOptions : KaitaiStruct
        {
            public CotpOptions(byte p_pduType, KaitaiStream p__io, TpktPacket p__parent = null, TpktPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _pduType = p_pduType;
                _read();
            }
            private void _read()
            {
                switch (PduType) {
                case 240: {
                    __raw_options = m_io.ReadBytesFull();
                    var io___raw_options = new KaitaiStream(__raw_options);
                    _options = new CotpDataTransferOptions(io___raw_options, this, m_root);
                    break;
                }
                default: {
                    __raw_options = m_io.ReadBytesFull();
                    var io___raw_options = new KaitaiStream(__raw_options);
                    _options = new CotpOtherOptions(io___raw_options, this, m_root);
                    break;
                }
                }
            }
            private KaitaiStruct _options;
            private byte _pduType;
            private TpktPacket m_root;
            private TpktPacket m_parent;
            private byte[] __raw_options;
            public KaitaiStruct Options { get { return _options; } }
            public byte PduType { get { return _pduType; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket M_Parent { get { return m_parent; } }
            public byte[] M_RawOptions { get { return __raw_options; } }
        }
        public partial class CotpOtherOptions : KaitaiStruct
        {
            public static CotpOtherOptions FromFile(string fileName)
            {
                return new CotpOtherOptions(new KaitaiStream(fileName));
            }

            public CotpOtherOptions(KaitaiStream p__io, TpktPacket.CotpOptions p__parent = null, TpktPacket p__root = null) : base(p__io)
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
            private TpktPacket.CotpOptions m_parent;
            public byte[] Bytes { get { return _bytes; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket.CotpOptions M_Parent { get { return m_parent; } }
        }
        public partial class CotpDataTransferOptions : KaitaiStruct
        {
            public static CotpDataTransferOptions FromFile(string fileName)
            {
                return new CotpDataTransferOptions(new KaitaiStream(fileName));
            }

            public CotpDataTransferOptions(KaitaiStream p__io, TpktPacket.CotpOptions p__parent = null, TpktPacket p__root = null) : base(p__io)
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
            private TpktPacket.CotpOptions m_parent;
            public bool LastData { get { return _lastData; } }
            public ulong TpduNumber { get { return _tpduNumber; } }
            public byte[] Data { get { return _data; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket.CotpOptions M_Parent { get { return m_parent; } }
        }
        public partial class TptkHeader : KaitaiStruct
        {
            public static TptkHeader FromFile(string fileName)
            {
                return new TptkHeader(new KaitaiStream(fileName));
            }

            public TptkHeader(KaitaiStream p__io, TpktPacket p__parent = null, TpktPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _version = m_io.ReadU1();
                _reserved = m_io.ReadU1();
                _length = m_io.ReadU2be();
            }
            private byte _version;
            private byte _reserved;
            private ushort _length;
            private TpktPacket m_root;
            private TpktPacket m_parent;
            public byte Version { get { return _version; } }
            public byte Reserved { get { return _reserved; } }
            public ushort Length { get { return _length; } }
            public TpktPacket M_Root { get { return m_root; } }
            public TpktPacket M_Parent { get { return m_parent; } }
        }
        private TptkHeader _tptk;
        private CotpHeader _cotp;
        private CotpOptions _opts;
        private byte[] _payload;
        private TpktPacket m_root;
        private KaitaiStruct m_parent;
        private byte[] __raw_opts;
        public TptkHeader Tptk { get { return _tptk; } }
        public CotpHeader Cotp { get { return _cotp; } }
        public CotpOptions Opts { get { return _opts; } }
        public byte[] Payload { get { return _payload; } }
        public TpktPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
        public byte[] M_RawOpts { get { return __raw_opts; } }
    }
}
