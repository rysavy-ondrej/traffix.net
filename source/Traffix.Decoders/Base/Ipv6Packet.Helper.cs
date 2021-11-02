using System;
using System.Buffers.Binary;

namespace Traffix.Extensions.Decoders.Base
{
    public partial class Ipv6Packet
    {
        public static class IPv6Fields
        {
            /// <summary>
            /// The IP Version, Traffic Class, and Flow Label field length. These must be in one
            /// field due to boundary crossings.
            /// </summary>
            public static readonly Int32 VersionTrafficClassFlowLabelLength = 4;

            /// <summary>
            /// The payload length field length.
            /// </summary>
            public static readonly Int32 PayloadLengthLength = 2;

            /// <summary>
            /// The next header field length, identifies protocol encapsulated by the packet
            /// </summary>
            public static readonly Int32 NextHeaderLength = 1;

            /// <summary>
            /// The hop limit field length.
            /// </summary>
            public static readonly Int32 HopLimitLength = 1;

            /// <summary>
            /// Address field length
            /// </summary>
            public static readonly Int32 AddressLength = 16;

            /// <summary>
            /// The byte position of the field line in the IPv6 header.
            /// This is where the IP version, Traffic Class, and Flow Label fields are.
            /// </summary>
            public static readonly Int32 VersionTrafficClassFlowLabelPosition = 0;

            /// <summary>
            /// The byte position of the payload length field.
            /// </summary>
            public static readonly Int32 PayloadLengthPosition;

            /// <summary>
            /// The byte position of the next header field. (Replaces the ipv4 protocol field)
            /// </summary>
            public static readonly Int32 NextHeaderPosition;

            /// <summary>
            /// The byte position of the hop limit field.
            /// </summary>
            public static readonly Int32 HopLimitPosition;

            /// <summary>
            /// The byte position of the source address field.
            /// </summary>
            public static readonly Int32 SourceAddressPosition;

            /// <summary>
            /// The byte position of the destination address field.
            /// </summary>
            public static readonly Int32 DestinationAddressPosition;

            /// <summary>
            /// The byte length of the IPv6 Header
            /// </summary>
            public static readonly Int32 HeaderLength; // == 40

            /// <summary>
            /// Commutes the field positions.
            /// </summary>
            static IPv6Fields()
            {
                PayloadLengthPosition = VersionTrafficClassFlowLabelPosition + VersionTrafficClassFlowLabelLength;
                NextHeaderPosition = PayloadLengthPosition + PayloadLengthLength;
                HopLimitPosition = NextHeaderPosition + NextHeaderLength;
                SourceAddressPosition = HopLimitPosition + HopLimitLength;
                DestinationAddressPosition = SourceAddressPosition + AddressLength;
                HeaderLength = DestinationAddressPosition + AddressLength;
            }
        }
        public static Span<byte> GetSourceAddress(Span<Byte> ipBytes)
        {
            return ipBytes.Slice(IPv6Fields.SourceAddressPosition, IPv6Fields.AddressLength);
        }

        public static Span<byte> GetDestinationAddress(Span<Byte> ipBytes)
        {
            return ipBytes.Slice(IPv6Fields.DestinationAddressPosition, IPv6Fields.AddressLength);
        }

        public static UInt16 GetPayloadLength(Span<Byte> ipBytes)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(ipBytes.Slice(IPv6Fields.PayloadLengthPosition));
        }

        public static Byte GetProtocol(Span<Byte> ipBytes)
        {
            return ipBytes[IPv6Fields.NextHeaderPosition];
        }

        public static Span<Byte> GetPayloadBytes(Span<Byte> ipBytes)
        {
            return ipBytes.Slice(IPv6Fields.HeaderLength, GetPayloadLength(ipBytes));
        }

    }
}
