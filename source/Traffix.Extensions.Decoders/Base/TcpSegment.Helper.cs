using System;
using System.Buffers.Binary;

namespace Traffix.Extensions.Decoders.Base
{
    public partial class TcpSegment
    {
        /// <summary>
        /// IP protocol field encoding information from PacketDotNet (https://github.com/chmorgan/packetnet/blob/master/PacketDotNet/TcpFields.cs)
        /// </summary>
        static class TcpFields
        {
#pragma warning disable 1591
            // flag bitmasks
            public static readonly Int32 TCPNsMask = 0x0100;
            public static readonly Int32 TCPCwrMask = 0x0080;
            public static readonly Int32 TCPEcnMask = 0x0040;
            public static readonly Int32 TCPUrgMask = 0x0020;
            public static readonly Int32 TCPAckMask = 0x0010;
            public static readonly Int32 TCPPshMask = 0x0008;
            public static readonly Int32 TCPRstMask = 0x0004;
            public static readonly Int32 TCPSynMask = 0x0002;
            public static readonly Int32 TCPFinMask = 0x0001;
#pragma warning restore 1591

            /// <summary> Length of a TCP port in bytes.</summary>
            public static readonly Int32 PortLength = 2;

            /// <summary> Length of the sequence number in bytes.</summary>
            public static readonly Int32 SequenceNumberLength = 4;

            /// <summary> Length of the acknowledgment number in bytes.</summary>
            public static readonly Int32 AckNumberLength = 4;

            /// <summary> Length of the data offset and flags field in bytes.</summary>
            public static readonly Int32 DataOffsetAndFlagsLength = 2;

            /// <summary> Length of the window size field in bytes.</summary>
            public static readonly Int32 WindowSizeLength = 2;

            /// <summary> Length of the checksum field in bytes.</summary>
            public static readonly Int32 ChecksumLength = 2;

            /// <summary> Length of the urgent field in bytes.</summary>
            public static readonly Int32 UrgentPointerLength = 2;

            /// <summary> Position of the source port field.</summary>
            public static readonly Int32 SourcePortPosition = 0;

            /// <summary> Position of the destination port field.</summary>
            public static readonly Int32 DestinationPortPosition;

            /// <summary> Position of the sequence number field.</summary>
            public static readonly Int32 SequenceNumberPosition;

            /// <summary> Position of the acknowledgment number field.</summary>
            public static readonly Int32 AckNumberPosition;

            /// <summary> Position of the data offset </summary>
            public static readonly Int32 DataOffsetAndFlagsPosition;

            /// <summary> Position of the window size field.</summary>
            public static readonly Int32 WindowSizePosition;

            /// <summary> Position of the checksum field.</summary>
            public static readonly Int32 ChecksumPosition;

            /// <summary> Position of the urgent pointer field.</summary>
            public static readonly Int32 UrgentPointerPosition;

            /// <summary> Length in bytes of a TCP header.</summary>
            public static readonly Int32 HeaderLength; // == 20

            static TcpFields()
            {
                DestinationPortPosition = PortLength;
                SequenceNumberPosition = DestinationPortPosition + PortLength;
                AckNumberPosition = SequenceNumberPosition + SequenceNumberLength;
                DataOffsetAndFlagsPosition = AckNumberPosition + AckNumberLength;
                WindowSizePosition = DataOffsetAndFlagsPosition + DataOffsetAndFlagsLength;
                ChecksumPosition = WindowSizePosition + WindowSizeLength;
                UrgentPointerPosition = ChecksumPosition + ChecksumLength;
                HeaderLength = UrgentPointerPosition + UrgentPointerLength;
            }
        }
        public static UInt16 GetSourcePort(Span<Byte> tcpBytes)
        {
            var port = tcpBytes.Slice(TcpFields.SourcePortPosition);
            return BinaryPrimitives.ReadUInt16BigEndian(port);
        }
        public static UInt16 GetDestinationPort(Span<Byte> tcpBytes)
        {
            var port = tcpBytes.Slice(TcpFields.DestinationPortPosition);
            return BinaryPrimitives.ReadUInt16BigEndian(port);
        }
        public static Span<Byte> GetSourcePortBytes(Span<Byte> udpBytes)
        {
            return udpBytes.Slice(TcpFields.SourcePortPosition, 2);
        }
        public static Span<Byte> GetDestinationPortBytes(Span<Byte> udpBytes)
        {
            return udpBytes.Slice(TcpFields.DestinationPortPosition, 2);
        }
        public static UInt32 GetSequenceNumber(Span<Byte> tcpBytes)
        {
            var sequenceNumber = tcpBytes.Slice(TcpFields.SequenceNumberPosition);
            return BinaryPrimitives.ReadUInt32BigEndian(sequenceNumber);
        }
        public static UInt32 GetAcknowledgmentNumber(Span<Byte> tcpBytes)
        {
            var sequenceNumber = tcpBytes.Slice(TcpFields.AckNumberPosition);
            return BinaryPrimitives.ReadUInt32BigEndian(sequenceNumber);
        }
        public static Byte GetDataOffset(Span<Byte> tcpBytes)
        {
            var dataOffsetAndFlags = BinaryPrimitives.ReadUInt16BigEndian(tcpBytes.Slice(TcpFields.DataOffsetAndFlagsPosition));
            return (Byte)((dataOffsetAndFlags >> 12) & 0xF);
        }
        public static Span<Byte> GetPayloadBytes(Span<Byte> tcpBytes)
        {
            var headerLength = GetDataOffset(tcpBytes) * 4;
            return tcpBytes.Slice(headerLength);
        }
    }
}
