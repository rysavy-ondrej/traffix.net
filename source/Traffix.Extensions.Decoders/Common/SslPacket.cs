// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Common
{
    public partial class SslPacket : KaitaiStruct
    {
        public static SslPacket FromFile(string fileName)
        {
            return new SslPacket(new KaitaiStream(fileName));
        }


        public enum SslMessageType
        {
            Error = 0,
            ClientHello = 1,
            ClientMasterKey = 2,
            ClientFinished = 3,
            ServerHello = 4,
            ServerVerify = 5,
            ServerFinished = 6,
            RequestCertificate = 7,
            ClientCertificate = 8,
        }
        public SslPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, SslPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _length = new SslLength(m_io, this, m_root);
            __raw_record = m_io.ReadBytes(Length.Record);
            var io___raw_record = new KaitaiStream(__raw_record);
            _record = new SslRecord(io___raw_record, this, m_root);
            _padding = m_io.ReadBytes(Length.Padding);
        }
        public partial class SslEncryptedMessage : KaitaiStruct
        {
            public static SslEncryptedMessage FromFile(string fileName)
            {
                return new SslEncryptedMessage(new KaitaiStream(fileName));
            }

            public SslEncryptedMessage(KaitaiStream p__io, SslPacket.SslRecord p__parent = null, SslPacket p__root = null) : base(p__io)
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
            private SslPacket m_root;
            private SslPacket.SslRecord m_parent;
            public byte[] Bytes { get { return _bytes; } }
            public SslPacket M_Root { get { return m_root; } }
            public SslPacket.SslRecord M_Parent { get { return m_parent; } }
        }
        public partial class SslCipherSpec : KaitaiStruct
        {
            public static SslCipherSpec FromFile(string fileName)
            {
                return new SslCipherSpec(new KaitaiStream(fileName));
            }

            public SslCipherSpec(KaitaiStream p__io, SslPacket.SslCipherList p__parent = null, SslPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _cipherBytes = m_io.ReadBytes(3);
            }
            private byte[] _cipherBytes;
            private SslPacket m_root;
            private SslPacket.SslCipherList m_parent;
            public byte[] CipherBytes { get { return _cipherBytes; } }
            public SslPacket M_Root { get { return m_root; } }
            public SslPacket.SslCipherList M_Parent { get { return m_parent; } }
        }
        public partial class SslRecord : KaitaiStruct
        {
            public static SslRecord FromFile(string fileName)
            {
                return new SslRecord(new KaitaiStream(fileName));
            }

            public SslRecord(KaitaiStream p__io, SslPacket p__parent = null, SslPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageType = ((SslPacket.SslMessageType) m_io.ReadU1());
                switch (MessageType) {
                case SslPacket.SslMessageType.ClientHello: {
                    _message = new SslClientHello(m_io, this, m_root);
                    break;
                }
                default: {
                    _message = new SslEncryptedMessage(m_io, this, m_root);
                    break;
                }
                }
            }
            private SslMessageType _messageType;
            private KaitaiStruct _message;
            private SslPacket m_root;
            private SslPacket m_parent;
            public SslMessageType MessageType { get { return _messageType; } }
            public KaitaiStruct Message { get { return _message; } }
            public SslPacket M_Root { get { return m_root; } }
            public SslPacket M_Parent { get { return m_parent; } }
        }
        public partial class SslVersion : KaitaiStruct
        {
            public static SslVersion FromFile(string fileName)
            {
                return new SslVersion(new KaitaiStream(fileName));
            }

            public SslVersion(KaitaiStream p__io, SslPacket.SslClientHello p__parent = null, SslPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _major = m_io.ReadU1();
                _minor = m_io.ReadU1();
            }
            private byte _major;
            private byte _minor;
            private SslPacket m_root;
            private SslPacket.SslClientHello m_parent;
            public byte Major { get { return _major; } }
            public byte Minor { get { return _minor; } }
            public SslPacket M_Root { get { return m_root; } }
            public SslPacket.SslClientHello M_Parent { get { return m_parent; } }
        }
        public partial class SslLength : KaitaiStruct
        {
            public static SslLength FromFile(string fileName)
            {
                return new SslLength(new KaitaiStream(fileName));
            }

            public SslLength(KaitaiStream p__io, SslPacket p__parent = null, SslPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_hasPadding = false;
                f_isEscape = false;
                f_record = false;
                f_padding = false;
                _read();
            }
            private void _read()
            {
                _len1 = m_io.ReadU1();
                _len2 = m_io.ReadU1();
                if (HasPadding) {
                    _len3 = m_io.ReadU1();
                }
            }
            private bool f_hasPadding;
            private bool _hasPadding;
            public bool HasPadding
            {
                get
                {
                    if (f_hasPadding)
                        return _hasPadding;
                    _hasPadding = (bool) ((Len1 & 128) == 0);
                    f_hasPadding = true;
                    return _hasPadding;
                }
            }
            private bool f_isEscape;
            private int _isEscape;
            public int IsEscape
            {
                get
                {
                    if (f_isEscape)
                        return _isEscape;
                    _isEscape = (int) ((Len1 & 64));
                    f_isEscape = true;
                    return _isEscape;
                }
            }
            private bool f_record;
            private int _record;
            public int Record
            {
                get
                {
                    if (f_record)
                        return _record;
                    _record = (int) ((((Len1 & 127) << 8) + Len2));
                    f_record = true;
                    return _record;
                }
            }
            private bool f_padding;
            private byte _padding;
            public byte Padding
            {
                get
                {
                    if (f_padding)
                        return _padding;
                    _padding = (byte) ((HasPadding ? Len3 : 0));
                    f_padding = true;
                    return _padding;
                }
            }
            private byte _len1;
            private byte _len2;
            private byte? _len3;
            private SslPacket m_root;
            private SslPacket m_parent;
            public byte Len1 { get { return _len1; } }
            public byte Len2 { get { return _len2; } }
            public byte? Len3 { get { return _len3; } }
            public SslPacket M_Root { get { return m_root; } }
            public SslPacket M_Parent { get { return m_parent; } }
        }
        public partial class SslClientHello : KaitaiStruct
        {
            public static SslClientHello FromFile(string fileName)
            {
                return new SslClientHello(new KaitaiStream(fileName));
            }

            public SslClientHello(KaitaiStream p__io, SslPacket.SslRecord p__parent = null, SslPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _version = new SslVersion(m_io, this, m_root);
                _cipherSpecLength = m_io.ReadU2be();
                _sessionIdLength = m_io.ReadU2be();
                _challengeLength = m_io.ReadU2be();
                __raw_cipherSpecs = m_io.ReadBytes(CipherSpecLength);
                var io___raw_cipherSpecs = new KaitaiStream(__raw_cipherSpecs);
                _cipherSpecs = new SslCipherList(io___raw_cipherSpecs, this, m_root);
                _sessionId = m_io.ReadBytes(SessionIdLength);
                _challenge = m_io.ReadBytes(ChallengeLength);
            }
            private SslVersion _version;
            private ushort _cipherSpecLength;
            private ushort _sessionIdLength;
            private ushort _challengeLength;
            private SslCipherList _cipherSpecs;
            private byte[] _sessionId;
            private byte[] _challenge;
            private SslPacket m_root;
            private SslPacket.SslRecord m_parent;
            private byte[] __raw_cipherSpecs;
            public SslVersion Version { get { return _version; } }
            public ushort CipherSpecLength { get { return _cipherSpecLength; } }
            public ushort SessionIdLength { get { return _sessionIdLength; } }
            public ushort ChallengeLength { get { return _challengeLength; } }
            public SslCipherList CipherSpecs { get { return _cipherSpecs; } }
            public byte[] SessionId { get { return _sessionId; } }
            public byte[] Challenge { get { return _challenge; } }
            public SslPacket M_Root { get { return m_root; } }
            public SslPacket.SslRecord M_Parent { get { return m_parent; } }
            public byte[] M_RawCipherSpecs { get { return __raw_cipherSpecs; } }
        }
        public partial class SslCipherList : KaitaiStruct
        {
            public static SslCipherList FromFile(string fileName)
            {
                return new SslCipherList(new KaitaiStream(fileName));
            }

            public SslCipherList(KaitaiStream p__io, SslPacket.SslClientHello p__parent = null, SslPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _entries = new List<SslCipherSpec>();
                {
                    var i = 0;
                    while (!m_io.IsEof) {
                        _entries.Add(new SslCipherSpec(m_io, this, m_root));
                        i++;
                    }
                }
            }
            private List<SslCipherSpec> _entries;
            private SslPacket m_root;
            private SslPacket.SslClientHello m_parent;
            public List<SslCipherSpec> Entries { get { return _entries; } }
            public SslPacket M_Root { get { return m_root; } }
            public SslPacket.SslClientHello M_Parent { get { return m_parent; } }
        }
        private SslLength _length;
        private SslRecord _record;
        private byte[] _padding;
        private SslPacket m_root;
        private KaitaiStruct m_parent;
        private byte[] __raw_record;
        public SslLength Length { get { return _length; } }
        public SslRecord Record { get { return _record; } }
        public byte[] Padding { get { return _padding; } }
        public SslPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
        public byte[] M_RawRecord { get { return __raw_record; } }
    }
}
