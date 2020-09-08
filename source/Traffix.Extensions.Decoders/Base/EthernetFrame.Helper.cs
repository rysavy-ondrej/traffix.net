using System;
using System.Buffers.Binary;

namespace Traffix.Extensions.Decoders.Base
{
    public partial class EthernetFrame
    {
        /// <summary>
        /// Ethernet protocol field encoding information.
        /// </summary>
        public static class EthernetFields
        {
            /// <summary> Position of the destination MAC address within the ethernet header.</summary>
            public static readonly Int32 DestinationMacPosition = 0;

            /// <summary> Total length of an ethernet header in bytes.</summary>
            public static readonly Int32 HeaderLength; // == 14

            /// <summary>
            /// size of an ethernet mac address in bytes
            /// </summary>
            public static readonly Int32 MacAddressLength = 6;

            /// <summary> Position of the source MAC address within the ethernet header.</summary>
            public static readonly Int32 SourceMacPosition;

            /// <summary> Width of the ethernet type code in bytes.</summary>
            public static readonly Int32 TypeLength = 2;

            /// <summary> Position of the ethernet type field within the ethernet header.</summary>
            public static readonly Int32 TypePosition;

            static EthernetFields()
            {
                SourceMacPosition = MacAddressLength;
                TypePosition = MacAddressLength * 2;
                HeaderLength = TypePosition + TypeLength;
            }
        }
        public static Span<Byte> GetPayloadBytes(Span<Byte> etherBytes)
        {
            return etherBytes.Slice(EthernetFields.HeaderLength);
        }
        public static UInt16 GetEtherType(Span<Byte> etherBytes)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(etherBytes.Slice(EthernetFields.TypePosition));
        }
        public static Span<Byte> GetSourceMacAddress(Span<Byte> etherBytes)
        {
            return etherBytes.Slice(EthernetFields.SourceMacPosition);
        }
        public static Span<Byte> GetDestinationMacAddress(Span<Byte> etherBytes)
        {
            return etherBytes.Slice(EthernetFields.DestinationMacPosition);
        }
    }
}
