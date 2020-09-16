// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;

namespace Traffix.Extensions.Decoders.Core
{
    public partial class ArpPacket : KaitaiStruct
    {
        public static ArpPacket FromFile(string fileName)
        {
            return new ArpPacket(new KaitaiStream(fileName));
        }


        public enum ArpOpCode
        {
            Request = 1,
            Response = 2,
            RequestReverse = 3,
            ReplyReverse = 4,
            DrarpRequest = 5,
            DrarpReply = 6,
            DrarpError = 7,
            InarpRequest = 8,
            InarpReply = 9,
            ArpNak = 10,
            MarsRequest = 11,
            MarsMulti = 12,
            MarsMserv = 13,
            MarsJoin = 14,
            MarsLeave = 15,
            MarsNak = 16,
            MarsUnserv = 17,
            MarsSjoin = 18,
            MarsSleave = 19,
            MarsGrouplistRequest = 20,
            MarsGrouplistReply = 21,
            MarsRedirectMap = 22,
            MaposUnarp = 23,
        }
        public ArpPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, ArpPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _hardwareType = m_io.ReadU2be();
            _protocolType = m_io.ReadU2be();
            _hardwareAddresslen = m_io.ReadU1();
            _protocolAddresslen = m_io.ReadU1();
            _opCode = ((ArpOpCode) m_io.ReadU2be());
            _senderHardwareAddress = m_io.ReadBytes(HardwareAddresslen);
            _senderProtocolAddress = m_io.ReadBytes(ProtocolAddresslen);
            _targetHardwareAddress = m_io.ReadBytes(HardwareAddresslen);
            _targetProtocolAddress = m_io.ReadBytes(ProtocolAddresslen);
        }
        private ushort _hardwareType;
        private ushort _protocolType;
        private byte _hardwareAddresslen;
        private byte _protocolAddresslen;
        private ArpOpCode _opCode;
        private byte[] _senderHardwareAddress;
        private byte[] _senderProtocolAddress;
        private byte[] _targetHardwareAddress;
        private byte[] _targetProtocolAddress;
        private ArpPacket m_root;
        private KaitaiStruct m_parent;
        public ushort HardwareType { get { return _hardwareType; } }
        public ushort ProtocolType { get { return _protocolType; } }
        public byte HardwareAddresslen { get { return _hardwareAddresslen; } }
        public byte ProtocolAddresslen { get { return _protocolAddresslen; } }
        public ArpOpCode OpCode { get { return _opCode; } }
        public byte[] SenderHardwareAddress { get { return _senderHardwareAddress; } }
        public byte[] SenderProtocolAddress { get { return _senderProtocolAddress; } }
        public byte[] TargetHardwareAddress { get { return _targetHardwareAddress; } }
        public byte[] TargetProtocolAddress { get { return _targetProtocolAddress; } }
        public ArpPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
