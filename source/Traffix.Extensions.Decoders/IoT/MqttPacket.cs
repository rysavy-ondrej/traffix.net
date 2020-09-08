// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.IoT
{
    public partial class MqttPacket : KaitaiStruct
    {
        public static MqttPacket FromFile(string fileName)
        {
            return new MqttPacket(new KaitaiStream(fileName));
        }


        public enum MqttMessageType
        {
            Reserved0 = 0,
            Connect = 1,
            Connack = 2,
            Publish = 3,
            PublishAck = 4,
            PublishRec = 5,
            PublishRel = 6,
            PublishComp = 7,
            Subscribe = 8,
            SubscribeAck = 9,
            Unsubscribe = 10,
            UnsubscribeAck = 11,
            PingRequest = 12,
            PingResponse = 13,
            Disconnect = 14,
            Reserved15 = 15,
        }

        public enum MqttConnectReturnCode
        {
            ConnectionAccepted = 0,
            ConnectionRefusedUnacceptableProtocolVersion = 1,
            ConnectionRefusedIdentifierRejected = 2,
            ConnectionRefusedServerUnavailable = 3,
            ConnectionRefusedBadUsernameOrPassword = 4,
            ConnectionRefusedNotAuthorized = 5,
        }

        public enum MqttQos
        {
            AtMostOnce = 0,
            AtLeastOnce = 1,
            ExactlyOnce = 2,
            Reserved = 3,
        }
        public MqttPacket(KaitaiStream p__io, KaitaiStruct p__parent = null, MqttPacket p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _header = new MqttFixedHeader(m_io, this, m_root);
            switch (Header.MessageType)
            {
                case MqttMessageType.PingRequest:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessagePingRequest(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.PublishAck:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessagePublishAck(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Disconnect:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageDisconnect(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.PublishComp:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessagePublishComp(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.PublishRel:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessagePublishRel(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.SubscribeAck:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageSubscribeAck(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Connect:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageConnect(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Publish:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessagePublish(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Connack:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageConnack(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.UnsubscribeAck:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageUnsubscribeAck(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Unsubscribe:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageUnsubscribe(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Reserved0:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageReserved0(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Subscribe:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageSubscribe(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.PublishRec:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessagePublishRec(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.Reserved15:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageReserved15(io___raw_body, this, m_root);
                        break;
                    }
                case MqttMessageType.PingResponse:
                    {
                        __raw_body = m_io.ReadBytes(Header.Length.Value);
                        var io___raw_body = new KaitaiStream(__raw_body);
                        _body = new MqttMessageResponse(io___raw_body, this, m_root);
                        break;
                    }
                default:
                    {
                        _body = m_io.ReadBytes(Header.Length.Value);
                        break;
                    }
            }
        }
        public partial class MqttMessageReserved0 : KaitaiStruct
        {
            public static MqttMessageReserved0 FromFile(string fileName)
            {
                return new MqttMessageReserved0(new KaitaiStream(fileName));
            }

            public MqttMessageReserved0(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _contentOfMessage = m_io.ReadBytesFull();
            }
            private byte[] _contentOfMessage;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public byte[] ContentOfMessage { get { return _contentOfMessage; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageUnsubscribeAck : KaitaiStruct
        {
            public static MqttMessageUnsubscribeAck FromFile(string fileName)
            {
                return new MqttMessageUnsubscribeAck(new KaitaiStream(fileName));
            }

            public MqttMessageUnsubscribeAck(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
            }
            private ushort _messageId;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessagePublishAck : KaitaiStruct
        {
            public static MqttMessagePublishAck FromFile(string fileName)
            {
                return new MqttMessagePublishAck(new KaitaiStream(fileName));
            }

            public MqttMessagePublishAck(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
            }
            private ushort _messageId;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttFixedHeader : KaitaiStruct
        {
            public static MqttFixedHeader FromFile(string fileName)
            {
                return new MqttFixedHeader(new KaitaiStream(fileName));
            }

            public MqttFixedHeader(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageType = ((MqttPacket.MqttMessageType)m_io.ReadBitsInt(4));
                _dup = m_io.ReadBitsInt(1) != 0;
                _qos = ((MqttPacket.MqttQos)m_io.ReadBitsInt(2));
                _retain = m_io.ReadBitsInt(1) != 0;
                m_io.AlignToByte();
                _length = new MqttLength(m_io, this, m_root);
            }
            private MqttMessageType _messageType;
            private bool _dup;
            private MqttQos _qos;
            private bool _retain;
            private MqttLength _length;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public MqttMessageType MessageType { get { return _messageType; } }
            public bool Dup { get { return _dup; } }
            public MqttQos Qos { get { return _qos; } }
            public bool Retain { get { return _retain; } }
            public MqttLength Length { get { return _length; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageSubscribeAck : KaitaiStruct
        {
            public static MqttMessageSubscribeAck FromFile(string fileName)
            {
                return new MqttMessageSubscribeAck(new KaitaiStream(fileName));
            }

            public MqttMessageSubscribeAck(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
                _garantedQos = new MqttSubscribeQos(m_io, this, m_root);
            }
            private ushort _messageId;
            private MqttSubscribeQos _garantedQos;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttSubscribeQos GarantedQos { get { return _garantedQos; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessagePublishComp : KaitaiStruct
        {
            public static MqttMessagePublishComp FromFile(string fileName)
            {
                return new MqttMessagePublishComp(new KaitaiStream(fileName));
            }

            public MqttMessagePublishComp(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
            }
            private ushort _messageId;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageSubscribe : KaitaiStruct
        {
            public static MqttMessageSubscribe FromFile(string fileName)
            {
                return new MqttMessageSubscribe(new KaitaiStream(fileName));
            }

            public MqttMessageSubscribe(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
                _topics = new MqttSubscribeTopic(m_io, this, m_root);
            }
            private ushort _messageId;
            private MqttSubscribeTopic _topics;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttSubscribeTopic Topics { get { return _topics; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttSubscribeQos : KaitaiStruct
        {
            public static MqttSubscribeQos FromFile(string fileName)
            {
                return new MqttSubscribeQos(new KaitaiStream(fileName));
            }

            public MqttSubscribeQos(KaitaiStream p__io, MqttPacket.MqttMessageSubscribeAck p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _reserved = m_io.ReadBitsInt(6);
                _qos = ((MqttPacket.MqttQos)m_io.ReadBitsInt(2));
            }
            private ulong _reserved;
            private MqttQos _qos;
            private MqttPacket m_root;
            private MqttPacket.MqttMessageSubscribeAck m_parent;
            public ulong Reserved { get { return _reserved; } }
            public MqttQos Qos { get { return _qos; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket.MqttMessageSubscribeAck M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageDisconnect : KaitaiStruct
        {
            public static MqttMessageDisconnect FromFile(string fileName)
            {
                return new MqttMessageDisconnect(new KaitaiStream(fileName));
            }

            public MqttMessageDisconnect(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _payload = m_io.ReadBytesFull();
            }
            private byte[] _payload;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public byte[] Payload { get { return _payload; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageResponse : KaitaiStruct
        {
            public static MqttMessageResponse FromFile(string fileName)
            {
                return new MqttMessageResponse(new KaitaiStream(fileName));
            }

            public MqttMessageResponse(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _payload = m_io.ReadBytesFull();
            }
            private byte[] _payload;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public byte[] Payload { get { return _payload; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttSubscribeTopic : KaitaiStruct
        {
            public static MqttSubscribeTopic FromFile(string fileName)
            {
                return new MqttSubscribeTopic(new KaitaiStream(fileName));
            }

            public MqttSubscribeTopic(KaitaiStream p__io, MqttPacket.MqttMessageSubscribe p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _topicName = new MqttString(m_io, this, m_root);
                _reserved = m_io.ReadBitsInt(6);
                _requestedQos = ((MqttPacket.MqttQos)m_io.ReadBitsInt(2));
            }
            private MqttString _topicName;
            private ulong _reserved;
            private MqttQos _requestedQos;
            private MqttPacket m_root;
            private MqttPacket.MqttMessageSubscribe m_parent;
            public MqttString TopicName { get { return _topicName; } }
            public ulong Reserved { get { return _reserved; } }
            public MqttQos RequestedQos { get { return _requestedQos; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket.MqttMessageSubscribe M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageConnect : KaitaiStruct
        {
            public static MqttMessageConnect FromFile(string fileName)
            {
                return new MqttMessageConnect(new KaitaiStream(fileName));
            }

            public MqttMessageConnect(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _protocolName = new MqttString(m_io, this, m_root);
                _protocolVersionNumber = m_io.ReadU1();
                _connectFlags = new MqttConnectFlags(m_io, this, m_root);
                _keepAliveTimer = m_io.ReadU2be();
                _clientId = new MqttString(m_io, this, m_root);
                if (ConnectFlags.Will)
                {
                    _willTopic = new MqttString(m_io, this, m_root);
                }
                if (ConnectFlags.Will)
                {
                    _willMessage = new MqttString(m_io, this, m_root);
                }
                if (ConnectFlags.Username)
                {
                    _username = new MqttString(m_io, this, m_root);
                }
                if (ConnectFlags.Password)
                {
                    _password = new MqttString(m_io, this, m_root);
                }
            }
            private MqttString _protocolName;
            private byte _protocolVersionNumber;
            private MqttConnectFlags _connectFlags;
            private ushort _keepAliveTimer;
            private MqttString _clientId;
            private MqttString _willTopic;
            private MqttString _willMessage;
            private MqttString _username;
            private MqttString _password;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public MqttString ProtocolName { get { return _protocolName; } }
            public byte ProtocolVersionNumber { get { return _protocolVersionNumber; } }
            public MqttConnectFlags ConnectFlags { get { return _connectFlags; } }
            public ushort KeepAliveTimer { get { return _keepAliveTimer; } }
            public MqttString ClientId { get { return _clientId; } }
            public MqttString WillTopic { get { return _willTopic; } }
            public MqttString WillMessage { get { return _willMessage; } }
            public MqttString Username { get { return _username; } }
            public MqttString Password { get { return _password; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessagePublish : KaitaiStruct
        {
            public static MqttMessagePublish FromFile(string fileName)
            {
                return new MqttMessagePublish(new KaitaiStream(fileName));
            }

            public MqttMessagePublish(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _topic = new MqttString(m_io, this, m_root);
                if (((M_Parent.Header.Qos == MqttPacket.MqttQos.AtLeastOnce) || (M_Parent.Header.Qos == MqttPacket.MqttQos.ExactlyOnce)))
                {
                    _messageId = m_io.ReadU2be();
                }
                _payload = m_io.ReadBytesFull();
            }
            private MqttString _topic;
            private ushort? _messageId;
            private byte[] _payload;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public MqttString Topic { get { return _topic; } }
            public ushort? MessageId { get { return _messageId; } }
            public byte[] Payload { get { return _payload; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttConnectFlags : KaitaiStruct
        {
            public static MqttConnectFlags FromFile(string fileName)
            {
                return new MqttConnectFlags(new KaitaiStream(fileName));
            }

            public MqttConnectFlags(KaitaiStream p__io, MqttPacket.MqttMessageConnect p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _username = m_io.ReadBitsInt(1) != 0;
                _password = m_io.ReadBitsInt(1) != 0;
                _willRetain = m_io.ReadBitsInt(1) != 0;
                _willQos = ((MqttPacket.MqttQos)m_io.ReadBitsInt(2));
                _will = m_io.ReadBitsInt(1) != 0;
                _cleanSession = m_io.ReadBitsInt(1) != 0;
                _reserved = m_io.ReadBitsInt(1) != 0;
            }
            private bool _username;
            private bool _password;
            private bool _willRetain;
            private MqttQos _willQos;
            private bool _will;
            private bool _cleanSession;
            private bool _reserved;
            private MqttPacket m_root;
            private MqttPacket.MqttMessageConnect m_parent;
            public bool Username { get { return _username; } }
            public bool Password { get { return _password; } }
            public bool WillRetain { get { return _willRetain; } }
            public MqttQos WillQos { get { return _willQos; } }
            public bool Will { get { return _will; } }
            public bool CleanSession { get { return _cleanSession; } }
            public bool Reserved { get { return _reserved; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket.MqttMessageConnect M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageUnsubscribe : KaitaiStruct
        {
            public static MqttMessageUnsubscribe FromFile(string fileName)
            {
                return new MqttMessageUnsubscribe(new KaitaiStream(fileName));
            }

            public MqttMessageUnsubscribe(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
            }
            private ushort _messageId;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessagePublishRec : KaitaiStruct
        {
            public static MqttMessagePublishRec FromFile(string fileName)
            {
                return new MqttMessagePublishRec(new KaitaiStream(fileName));
            }

            public MqttMessagePublishRec(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
            }
            private ushort _messageId;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessagePingRequest : KaitaiStruct
        {
            public static MqttMessagePingRequest FromFile(string fileName)
            {
                return new MqttMessagePingRequest(new KaitaiStream(fileName));
            }

            public MqttMessagePingRequest(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _payload = m_io.ReadBytesFull();
            }
            private byte[] _payload;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public byte[] Payload { get { return _payload; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessagePublishRel : KaitaiStruct
        {
            public static MqttMessagePublishRel FromFile(string fileName)
            {
                return new MqttMessagePublishRel(new KaitaiStream(fileName));
            }

            public MqttMessagePublishRel(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _messageId = m_io.ReadU2be();
            }
            private ushort _messageId;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public ushort MessageId { get { return _messageId; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttString : KaitaiStruct
        {
            public static MqttString FromFile(string fileName)
            {
                return new MqttString(new KaitaiStream(fileName));
            }

            public MqttString(KaitaiStream p__io, KaitaiStruct p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _length = m_io.ReadU2be();
                _value = System.Text.Encoding.GetEncoding("ascii").GetString(m_io.ReadBytes(Length));
            }
            private ushort _length;
            private string _value;
            private MqttPacket m_root;
            private KaitaiStruct m_parent;
            public ushort Length { get { return _length; } }
            public string Value { get { return _value; } }
            public MqttPacket M_Root { get { return m_root; } }
            public KaitaiStruct M_Parent { get { return m_parent; } }
        }
        public partial class MqttLength : KaitaiStruct
        {
            public static MqttLength FromFile(string fileName)
            {
                return new MqttLength(new KaitaiStream(fileName));
            }

            public MqttLength(KaitaiStream p__io, MqttPacket.MqttFixedHeader p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_value = false;
                _read();
            }
            private void _read()
            {
                _bytes = new List<byte>();
                {
                    var i = 0;
                    byte M_;
                    do
                    {
                        M_ = m_io.ReadU1();
                        _bytes.Add(M_);
                        i++;
                    } while (!((M_ & 128) == 0));
                }
            }
            private bool f_value;
            private int _value;
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int)(((((Bytes[0] & 127) + (Bytes.Count > 1 ? ((Bytes[1] & 127) * 128) : 0)) + (Bytes.Count > 2 ? (((Bytes[2] & 127) * 128) * 128) : 0)) + (Bytes.Count > 3 ? ((((Bytes[3] & 127) * 128) * 128) * 128) : 0)));
                    f_value = true;
                    return _value;
                }
            }
            private List<byte> _bytes;
            private MqttPacket m_root;
            private MqttPacket.MqttFixedHeader m_parent;
            public List<byte> Bytes { get { return _bytes; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket.MqttFixedHeader M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageConnack : KaitaiStruct
        {
            public static MqttMessageConnack FromFile(string fileName)
            {
                return new MqttMessageConnack(new KaitaiStream(fileName));
            }

            public MqttMessageConnack(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _topicNameCompressionResponse = m_io.ReadU1();
                _connectReturnCode = ((MqttPacket.MqttConnectReturnCode)m_io.ReadU1());
            }
            private byte _topicNameCompressionResponse;
            private MqttConnectReturnCode _connectReturnCode;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public byte TopicNameCompressionResponse { get { return _topicNameCompressionResponse; } }
            public MqttConnectReturnCode ConnectReturnCode { get { return _connectReturnCode; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        public partial class MqttMessageReserved15 : KaitaiStruct
        {
            public static MqttMessageReserved15 FromFile(string fileName)
            {
                return new MqttMessageReserved15(new KaitaiStream(fileName));
            }

            public MqttMessageReserved15(KaitaiStream p__io, MqttPacket p__parent = null, MqttPacket p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _contentOfMessage = m_io.ReadBytesFull();
            }
            private byte[] _contentOfMessage;
            private MqttPacket m_root;
            private MqttPacket m_parent;
            public byte[] ContentOfMessage { get { return _contentOfMessage; } }
            public MqttPacket M_Root { get { return m_root; } }
            public MqttPacket M_Parent { get { return m_parent; } }
        }
        private MqttFixedHeader _header;
        private object _body;
        private MqttPacket m_root;
        private KaitaiStruct m_parent;
        private byte[] __raw_body;
        public MqttFixedHeader Header { get { return _header; } }
        public object Body { get { return _body; } }
        public MqttPacket M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
        public byte[] M_RawBody { get { return __raw_body; } }
    }
}
