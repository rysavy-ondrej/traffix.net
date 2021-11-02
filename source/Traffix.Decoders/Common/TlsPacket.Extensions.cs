using System;
using System.Collections.Generic;
using System.Text;

namespace Traffix.Extensions.Decoders.Common
{

    public static class TlsPacketExtensions

    {
        public static bool TryGetHandshakeMessage<T>(this TlsPacket.TlsHandshakeProtocol handshake, out T protocol) where T : class
        {
            foreach (var msg in handshake.HandshakeMessages)
            {
                if (msg.Body is T x)
                {
                    protocol = x;
                    return true;
                }
            }
            protocol = default;
            return false;
        }
    }

    public partial class TlsPacket
    {
        public static bool TryParsePacket(Kaitai.KaitaiStream stream, out TlsPacket packet)
        {
            try
            {
                var packetStartPos = stream.Pos;
                // first we check that this is really TlsRecord to be parsed
                // 1 byte  - content_type
                // 2 bytes - version
                // 2 bytes - record length
                if (stream.Size > 5 && CheckSignature(stream.ReadBytes(5)) == 0)
                {
                    packet = null;
                    return false;
                }
                stream.Seek(packetStartPos);
                // Try to parse the record.
                packet = new TlsPacket(stream);

                // this code is necessary to properly handle change cipher + finished records in TLS1.2
                // the TLS1.3 does not necessary use finished record after change cipher 
                if (packet.ContentType == TlsContentType.ChangeCipherSpec && stream.PeekByte() == (byte)TlsContentType.Handshake)
                {   // consume finished
                    var finished = new TlsPacket.TlsFinished(stream);    
                }
                return true;
            }
            catch(Exception e)
            {
                packet = null;
                return false;
            }
        }
        /// <summary>
        /// Checks if the byte can represent TLS header. 
        /// <para/>
        /// It check first 5 bytes of possible TLS record. The method has non-zero false positive.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>The length of the TLS record, or 0 ifit is not the start of TLS record.</returns>
        public static ushort CheckSignature(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length < 5) return 0;
            var magic = (bytes[0] >= 0x14 && bytes[0] <= 0x17)
             && (bytes[1] == 0x3)
             && (bytes[2] <= 0x3);
            var length = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(3, 2));
            return magic ? length : (ushort)0;
        }
    }
}
