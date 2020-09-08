// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using Kaitai;
using System.Collections.Generic;

namespace Traffix.Extensions.Decoders.Industrial
{

    /// <summary>
    /// This is parser for the data link layer for the 3-layer, connection-oriented, HDLC-based,
    /// asous communication COSEM profile. 
    /// 
    /// DLMS information is carried in the information field. To parse the informaiton 
    /// field use dlms_acse or dlms_apdu. What to use depends on the first bbyte of information octet string:
    /// 
    /// * 91-101: dlms_acse
    /// * 192-199: dlms_apdu
    /// 
    /// In order to ensure a coherent data link layer service specification for both connection-oriented and
    /// connectionless operation modes, the data link layer is divided into two sub-layers: the Logical Link
    /// Control (LLC) sub-layer and the Medium Access Control (MAC) sub-layer.
    /// 
    /// The LLC sub-layer is based on ISO/IEC 8802-2. 
    /// 
    /// The MAC sub-layer – the major part of this data link layer specification – is based on ISO/IEC13239 (HDLC). 
    /// 
    /// HDLC Addressing:
    /// 
    /// The HDLC frame format type 3 contains two address fields: a destination and a
    /// source HDLC address. Depending on the direction of the data transfer, both the client and the
    /// server addresses can be destination or source addresses.
    /// The client address shall always be expressed on one byte.
    /// The server address – to enable addressing more than one logical device within a single physical
    /// device and to support the multi-drop configuration – may be divided into two parts:
    /// - the upper HDLC address is used to address a Logical Device (a separately addressable entity within a physical device);
    /// - the lower HDLC address is used to address a Physical Device (a physical device on the multi-drop).
    ///   The upper HDLC address shall always be present. The lower HDLC address may be omitted if it is
    ///   not required. 
    /// The HDLC address extension mechanism applies to both parts. This mechanism specifies variable
    /// length address fields, but for the purpose of this protocol, the length of a complete server address 
    /// field is restricted to be one, two or four bytes long.
    /// </summary>
    public partial class DlmsHdlc : KaitaiStruct
    {
        public static DlmsHdlc FromFile(string fileName)
        {
            return new DlmsHdlc(new KaitaiStream(fileName));
        }

        public enum LlcPacketType
        {
            LlcCommand = 230,
            LlcResponse = 231,
        }

        public enum HdlcPfBit
        {
            HdlcPoll = 0,
            HdlcFinal = 1,
        }

        public enum FrameTypeEnum
        {
            SFrame = 1,
            IFrame = 2,
            UFrame = 3,
        }

        public enum SFrameType
        {
            ReceiverReady = 0,
            ReceiverNotReady = 1,
            Reject = 2,
            SelectiveReject = 3,
        }

        public enum HdlcCr
        {
            HdlcCommand = 0,
            HdclResponse = 1,
        }

        public DlmsHdlc(KaitaiStream io, KaitaiStruct parent = null, DlmsHdlc root = null) : base(io)
        {
            m_parent = parent;
            m_root = root ?? this;
            _parse();
        }

        private void _parse()
        {
            _startFlag = m_io.EnsureFixedContents(new byte[] { 126 });
            _hdlcHeader = new HdlcHeaderFields(m_io, this, m_root);
            if ((HdlcHeader.Control.FrameType & 1) == 0)
            {
                _llcHeader = new LlcHeaderFields(m_io, this, m_root);
            }
            _information = m_io.ReadBytes((HdlcHeader.Format.FrameLength - ((HdlcHeader.Size + ((HdlcHeader.Control.FrameType & 1) == 0 ? LlcHeader.Size : 0)) + 2)));
            _fsc = m_io.ReadU2be();
            _stopFlag = m_io.EnsureFixedContents(new byte[] { 126 });
        }
        public partial class UFrameControlByte : KaitaiStruct
        {
            public static UFrameControlByte FromFile(string fileName)
            {
                return new UFrameControlByte(new KaitaiStream(fileName));
            }

            public UFrameControlByte(KaitaiStream io, ControlType parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _rType = m_io.ReadBitsInt(3);
                _pfBit = ((DlmsHdlc.HdlcPfBit)m_io.ReadBitsInt(1));
                _sType = m_io.ReadBitsInt(2);
            }
            private ulong _rType;
            private HdlcPfBit _pfBit;
            private ulong _sType;
            private DlmsHdlc m_root;
            private DlmsHdlc.ControlType m_parent;
            public ulong RType { get { return _rType; } }
            public HdlcPfBit PfBit { get { return _pfBit; } }
            public ulong SType { get { return _sType; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc.ControlType M_Parent { get { return m_parent; } }
        }

        /// <summary>
        /// One byte group, clearly divided into 7-bit &quot;value&quot; and 1-bit &quot;has continuation
        /// in the next byte&quot; flag. contiune with next byte = 0, final byte = 1.
        /// </summary>
        public partial class HdlcAddressByte : KaitaiStruct
        {
            public static HdlcAddressByte FromFile(string fileName)
            {
                return new HdlcAddressByte(new KaitaiStream(fileName));
            }

            public HdlcAddressByte(KaitaiStream io, HdlcAddress parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_hasNext = false;
                f_value = false;
                _b = m_io.ReadU1();
            }
            private bool f_hasNext;
            private bool _hasNext;

            /// <summary>
            /// If true, then we have more bytes to read
            /// </summary>
            public bool HasNext
            {
                get
                {
                    if (f_hasNext)
                        return _hasNext;
                    _hasNext = (bool)((B & 1) == 0);
                    f_hasNext = true;
                    return _hasNext;
                }
            }
            private bool f_value;
            private int _value;

            /// <summary>
            /// The 7-bit (base128) numeric value of this group
            /// </summary>
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int)(((B & 254) >> 1));
                    f_value = true;
                    return _value;
                }
            }
            private byte _b;
            private DlmsHdlc m_root;
            private DlmsHdlc.HdlcAddress m_parent;
            public byte B { get { return _b; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc.HdlcAddress M_Parent { get { return m_parent; } }
        }
        public partial class HdlcHeaderFields : KaitaiStruct
        {
            public static HdlcHeaderFields FromFile(string fileName)
            {
                return new HdlcHeaderFields(new KaitaiStream(fileName));
            }

            public HdlcHeaderFields(KaitaiStream io, DlmsHdlc parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_size = false;
                __raw_format = m_io.ReadBytes(2);
                var io___raw_format = new KaitaiStream(__raw_format);
                _format = new FormatType(io___raw_format, this, m_root);
                _dstAddress = new HdlcAddress(m_io, this, m_root);
                _srcAddress = new HdlcAddress(m_io, this, m_root);
                __raw_control = m_io.ReadBytes(1);
                var io___raw_control = new KaitaiStream(__raw_control);
                _control = new ControlType(io___raw_control, this, m_root);
                if ((Control.FrameType & 1) == 0)
                {
                    _hcs = m_io.ReadU2be();
                }
            }
            private bool f_size;
            private int _size;
            public int Size
            {
                get
                {
                    if (f_size)
                        return _size;
                    _size = (int)(((((2 + DstAddress.Size) + SrcAddress.Size) + 1) + ((Control.FrameType & 1) == 0 ? 2 : 0)));
                    f_size = true;
                    return _size;
                }
            }
            private FormatType _format;
            private HdlcAddress _dstAddress;
            private HdlcAddress _srcAddress;
            private ControlType _control;
            private ushort _hcs;
            private DlmsHdlc m_root;
            private DlmsHdlc m_parent;
            private byte[] __raw_format;
            private byte[] __raw_control;

            /// <summary>
            /// The length of the frame format field is two bytes. It consists of three sub-fields referred to as the
            /// Format type sub-field (4 bits), the Segmentation bit (S, 1 bit) and the frame length sub-field (11 bits).
            /// </summary>
            public FormatType Format { get { return _format; } }

            /// <summary>
            /// Depending on the direction of the data transfer, both the client and the
            /// server addresses can be destination or source addresses.
            /// The HDLC address extension mechanism applies to address representation.
            /// The client address is only 1 byte. Server address can be 1,2, 4 bytes.          
            /// </summary>
            public HdlcAddress DstAddress { get { return _dstAddress; } }

            /// <summary>
            /// Depending on the direction of the data transfer, both the client and the
            /// server addresses can be destination or source addresses. 
            /// The HDLC address extension mechanism applies to address representation.
            /// The client address is only 1 byte. Server address can be 1,2, 4 bytes.
            /// </summary>
            public HdlcAddress SrcAddress { get { return _srcAddress; } }

            /// <summary>
            /// It indicates the type of commands or responses, and
            ///  contains sequence numbers, where appropriate.
            /// </summary>
            public ControlType Control { get { return _control; } }

            /// <summary>
            /// This check sequence is applied to only the header, i.e.,
            /// the bits between the opening flag sequence and the header check sequence.
            /// Frames that do not have an information field or have an empty information field, e.g., as with some supervisory frames,
            /// do not contain an HCS. 
            /// </summary>
            public ushort Hcs { get { return _hcs; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc M_Parent { get { return m_parent; } }
            public byte[] M_RawFormat { get { return __raw_format; } }
            public byte[] M_RawControl { get { return __raw_control; } }
        }

        /// <summary>
        /// This mechanism specifies variable
        /// length address fields, but for the purpose of this protocol, the length of a complete server address
        /// field is restricted to be one, two or four bytes long. 
        /// 
        /// The address field range can be extended by
        /// reserving the first transmitted bit (low-order) of each address octet which would then be set to
        /// binary zero to indicate that the following octet is an extension of the address field. 
        /// 
        /// The format of the extended octet(s) shall be the same as that of the first octet. Thus, the address field may be
        /// recursively extended. The last octet of an address field is indicted by setting the low-order bit to
        /// binary one.
        /// 
        /// When extension is used, the presence of a binary &quot;1&quot; in the first transmitted bit of the first address
        /// octet indicates that only one address octet is being used. The use of address extension thus
        /// restricts the range of single octet addresses to 0x7F and for two octet addresses to 0…0x3FFF.
        /// 
        /// Single bytes address:
        /// | address-7bits | 1 | 
        /// 
        /// Two bytes address: 
        /// | address-7bits | 0 |  | address-7bits | 1 | 
        /// 
        /// Four bytes address:
        /// | address-7bits | 0 |   | address-7bits | 0 |   | address-7bits | 0 |  | address-7bits | 1 | 
        /// </summary>
        public partial class HdlcAddress : KaitaiStruct
        {
            public static HdlcAddress FromFile(string fileName)
            {
                return new HdlcAddress(new KaitaiStream(fileName));
            }

            public HdlcAddress(KaitaiStream io, HdlcHeaderFields parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_size = false;
                f_last = false;
                f_value = false;
                _bytes = new List<HdlcAddressByte>();
                {
                    HdlcAddressByte M_;
                    do
                    {
                        M_ = new HdlcAddressByte(m_io, this, m_root);
                        _bytes.Add(M_);
                    } while (!(!M_.HasNext));
                }
            }
            private bool f_size;
            private int _size;
            public int Size
            {
                get
                {
                    if (f_size)
                        return _size;
                    _size = (int)(Bytes.Count);
                    f_size = true;
                    return _size;
                }
            }
            private bool f_last;
            private int _last;
            public int Last
            {
                get
                {
                    if (f_last)
                        return _last;
                    _last = (int)((Bytes.Count - 1));
                    f_last = true;
                    return _last;
                }
            }
            private bool f_value;
            private int _value;

            /// <summary>
            /// Resulting value as normal integer
            /// </summary>
            public int Value
            {
                get
                {
                    if (f_value)
                        return _value;
                    _value = (int)((((Bytes[Last].Value + (Last >= 1 ? (Bytes[(Last - 1)].Value << 7) : 0)) + (Last >= 2 ? (Bytes[(Last - 2)].Value << 14) : 0)) + (Last >= 3 ? (Bytes[(Last - 3)].Value << 21) : 0)));
                    f_value = true;
                    return _value;
                }
            }
            private List<HdlcAddressByte> _bytes;
            private DlmsHdlc m_root;
            private DlmsHdlc.HdlcHeaderFields m_parent;
            public List<HdlcAddressByte> Bytes { get { return _bytes; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc.HdlcHeaderFields M_Parent { get { return m_parent; } }
        }
        public partial class ControlType : KaitaiStruct
        {
            public static ControlType FromFile(string fileName)
            {
                return new ControlType(new KaitaiStream(fileName));
            }

            public ControlType(KaitaiStream io, HdlcHeaderFields parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_controlByte = false;
                f_frameType = false;
                if ((FrameType & 1) == 0)
                {
                    _iFrame = new IFrameControlByte(m_io, this, m_root);
                }
                if (FrameType == 1)
                {
                    _sFrame = new SFrameControlByte(m_io, this, m_root);
                }
                if (FrameType == 3)
                {
                    _uFrame = new UFrameControlByte(m_io, this, m_root);
                }
            }
            private bool f_controlByte;
            private byte _controlByte;
            public byte ControlByte
            {
                get
                {
                    if (f_controlByte)
                        return _controlByte;
                    long _pos = m_io.Pos;
                    m_io.Seek(0);
                    _controlByte = m_io.ReadU1();
                    m_io.Seek(_pos);
                    f_controlByte = true;
                    return _controlByte;
                }
            }
            private bool f_frameType;
            private int _frameType;
            public int FrameType
            {
                get
                {
                    if (f_frameType)
                        return _frameType;
                    _frameType = (int)((ControlByte & 3));
                    f_frameType = true;
                    return _frameType;
                }
            }
            private IFrameControlByte _iFrame;
            private SFrameControlByte _sFrame;
            private UFrameControlByte _uFrame;
            private DlmsHdlc m_root;
            private DlmsHdlc.HdlcHeaderFields m_parent;
            public IFrameControlByte IFrame { get { return _iFrame; } }
            public SFrameControlByte SFrame { get { return _sFrame; } }
            public UFrameControlByte UFrame { get { return _uFrame; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc.HdlcHeaderFields M_Parent { get { return m_parent; } }
        }
        public partial class LlcHeaderFields : KaitaiStruct
        {
            public static LlcHeaderFields FromFile(string fileName)
            {
                return new LlcHeaderFields(new KaitaiStream(fileName));
            }

            public LlcHeaderFields(KaitaiStream io, DlmsHdlc parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_size = false;
                f_llcStartByte = false;
                if (LlcStartByte == 230)
                {
                    _remoteLsap = m_io.EnsureFixedContents(new byte[] { 230 });
                }
                if (LlcStartByte == 230)
                {
                    _localLsap = ((DlmsHdlc.LlcPacketType)m_io.ReadU1());
                }
                if (LlcStartByte == 230)
                {
                    _llcQuality = m_io.EnsureFixedContents(new byte[] { 0 });
                }
            }
            private bool f_size;
            private sbyte _size;
            public sbyte Size
            {
                get
                {
                    if (f_size)
                        return _size;
                    _size = (sbyte)((LlcStartByte == 230 ? 3 : 0));
                    f_size = true;
                    return _size;
                }
            }
            private bool f_llcStartByte;
            private byte _llcStartByte;
            public byte LlcStartByte
            {
                get
                {
                    if (f_llcStartByte)
                        return _llcStartByte;
                    long _pos = m_io.Pos;
                    m_io.Seek(M_Io.Pos);
                    _llcStartByte = m_io.ReadU1();
                    m_io.Seek(_pos);
                    f_llcStartByte = true;
                    return _llcStartByte;
                }
            }
            private byte[] _remoteLsap;
            private LlcPacketType _localLsap;
            private byte[] _llcQuality;
            private DlmsHdlc m_root;
            private DlmsHdlc m_parent;

            /// <summary>
            /// Destination_LSAP is always 0xE6.
            /// </summary>
            public byte[] RemoteLsap { get { return _remoteLsap; } }

            /// <summary>
            /// The value of the Source_LSAP is 0xE6 or 0xE7. The last bit is used as a command/response identifier:
            /// 0xE6 ‘command’ and 0xE7 “response�?. 
            /// </summary>
            public LlcPacketType LocalLsap { get { return _localLsap; } }

            /// <summary>
            /// The quality value is reserved for future use and must be 0.
            /// </summary>
            public byte[] LlcQuality { get { return _llcQuality; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc M_Parent { get { return m_parent; } }
        }
        public partial class SFrameControlByte : KaitaiStruct
        {
            public static SFrameControlByte FromFile(string fileName)
            {
                return new SFrameControlByte(new KaitaiStream(fileName));
            }

            public SFrameControlByte(KaitaiStream io, ControlType parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _recvSequenceNumber = m_io.ReadBitsInt(3);
                _pfBit = ((DlmsHdlc.HdlcPfBit)m_io.ReadBitsInt(1));
                _sFrameType = ((DlmsHdlc.SFrameType)m_io.ReadBitsInt(2));
            }
            private ulong _recvSequenceNumber;
            private HdlcPfBit _pfBit;
            private SFrameType _sFrameType;
            private DlmsHdlc m_root;
            private DlmsHdlc.ControlType m_parent;
            public ulong RecvSequenceNumber { get { return _recvSequenceNumber; } }
            public HdlcPfBit PfBit { get { return _pfBit; } }
            public SFrameType SFrameType { get { return _sFrameType; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc.ControlType M_Parent { get { return m_parent; } }
        }
        public partial class IFrameControlByte : KaitaiStruct
        {
            public static IFrameControlByte FromFile(string fileName)
            {
                return new IFrameControlByte(new KaitaiStream(fileName));
            }

            public IFrameControlByte(KaitaiStream io, ControlType parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                _recvSequenceNumber = m_io.ReadBitsInt(3);
                _pfBit = ((DlmsHdlc.HdlcPfBit)m_io.ReadBitsInt(1));
                _sendSequenceNumber = m_io.ReadBitsInt(3);
            }
            private ulong _recvSequenceNumber;
            private HdlcPfBit _pfBit;
            private ulong _sendSequenceNumber;
            private DlmsHdlc m_root;
            private DlmsHdlc.ControlType m_parent;
            public ulong RecvSequenceNumber { get { return _recvSequenceNumber; } }
            public HdlcPfBit PfBit { get { return _pfBit; } }
            public ulong SendSequenceNumber { get { return _sendSequenceNumber; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc.ControlType M_Parent { get { return m_parent; } }
        }
        public partial class FormatType : KaitaiStruct
        {
            public static FormatType FromFile(string fileName)
            {
                return new FormatType(new KaitaiStream(fileName));
            }

            public FormatType(KaitaiStream io, HdlcHeaderFields parent = null, DlmsHdlc root = null) : base(io)
            {
                m_parent = parent;
                m_root = root;
                _parse();
            }

            private void _parse()
            {
                f_frameLength = false;
                _frameFormatType = m_io.ReadBitsInt(4);
                _segmentationFlag = m_io.ReadBitsInt(1) != 0;
                _length = m_io.ReadBitsInt(11);
            }
            private bool f_frameLength;
            private int _frameLength;
            public int FrameLength
            {
                get
                {
                    if (f_frameLength)
                        return _frameLength;
                    _frameLength = (int)((Length + 0));
                    f_frameLength = true;
                    return _frameLength;
                }
            }
            private ulong _frameFormatType;
            private bool _segmentationFlag;
            private ulong _length;
            private DlmsHdlc m_root;
            private DlmsHdlc.HdlcHeaderFields m_parent;

            /// <summary>
            /// The value of the format type sub-field is 1010 (binary), which identifies a frame format type 3.
            /// </summary>
            public ulong FrameFormatType { get { return _frameFormatType; } }

            /// <summary>
            /// Rules of using the segmentation bit are defined in the complete Green Book (?).
            /// </summary>
            public bool SegmentationFlag { get { return _segmentationFlag; } }

            /// <summary>
            /// The value of the frame length subfield is the count of octets in the frame excluding the opening and
            /// closing frame flag sequences. 
            /// </summary>
            public ulong Length { get { return _length; } }
            public DlmsHdlc M_Root { get { return m_root; } }
            public DlmsHdlc.HdlcHeaderFields M_Parent { get { return m_parent; } }
        }
        private byte[] _startFlag;
        private HdlcHeaderFields _hdlcHeader;
        private LlcHeaderFields _llcHeader;
        private byte[] _information;
        private ushort _fsc;
        private byte[] _stopFlag;
        private DlmsHdlc m_root;
        private KaitaiStruct m_parent;

        /// <summary>
        /// The flag field is one byte and its value is 7E.
        /// </summary>
        public byte[] StartFlag { get { return _startFlag; } }

        /// <summary>
        /// The MAC sub-layer uses the HDLC frame format type 3 as defined in Annex H.4 of ISO/IEC 13239. 
        /// </summary>
        public HdlcHeaderFields HdlcHeader { get { return _hdlcHeader; } }

        /// <summary>
        /// The LLC sub-layer transmits LSDUs transparently between its service user layer and the MAC sublayer.
        /// LLC is used only with I-frames.
        /// </summary>
        public LlcHeaderFields LlcHeader { get { return _llcHeader; } }

        /// <summary>
        /// The information field may be any sequence of bytes. In the case of data frames (I and UI frames), it carries the MSDU. 
        /// </summary>
        public byte[] Information { get { return _information; } }

        /// <summary>
        /// Unless otherwise noted, the frame checking sequence is
        /// calculated for the entire length of the frame, excluding the opening flag, the FCS and any start and
        /// stop elements (start/stop transmission).
        /// </summary>
        public ushort Fsc { get { return _fsc; } }

        /// <summary>
        /// The flag field is one byte and its value is 7E. 
        /// When two or more frames are
        /// transmitted continuously, a single flag is used as both the closing flag of one frame and the
        /// opening flag of the next frame.
        /// </summary>
        public byte[] StopFlag { get { return _stopFlag; } }
        public DlmsHdlc M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
