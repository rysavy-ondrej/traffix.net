using Kaitai;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using System;
using System.Collections.Generic;
using System.Linq;
using Traffix.Providers.PcapFile;

namespace IcsMonitor
{
#if LogSupport
    
    class LoggerOptions : Microsoft.Extensions.Options.IOptionsMonitor<ConsoleLoggerOptions>
    {
        ConsoleLoggerOptions options = new ConsoleLoggerOptions()
        {

        };
        public ConsoleLoggerOptions CurrentValue => options;
        public ConsoleLoggerOptions Get(string name) => options;

        Action<ConsoleLoggerOptions, string> _listener;
        public IDisposable OnChange(Action<ConsoleLoggerOptions, string> listener)
        {
            _listener = listener;
            return new ListenerDisposable(this); 
        }
        class ListenerDisposable : IDisposable
        {
            private readonly LoggerOptions _parent;

            public ListenerDisposable(LoggerOptions parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                _parent._listener = null;
            }
        }
    }
    class LoggerFormatter : Microsoft.Extensions.Logging.Console.ConsoleFormatter
    {
        public LoggerFormatter() : base("simple")
        {
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {                                                                 
            textWriter.WriteLine($"{logEntry.LogLevel}: {logEntry.State}");
        }
    }
#endif
    public static class PacketHelper
    {
        public static UdpPacket GetUdpPacket(this RawFrame frame)
        {
            var packet = Packet.ParsePacket(frame.LinkLayer, frame.Data);
            return packet.Extract<UdpPacket>();
        }
        public static TcpPacket GetTcpPacket(this RawFrame frame)
        {
            var packet = Packet.ParsePacket(frame.LinkLayer, frame.Data);
            return packet.Extract<TcpPacket>();
        }
        public static Packet GetPacket(this RawFrame frame)
        {
            var packet = Packet.ParsePacket(frame.LinkLayer, frame.Data);
            return packet;
        }

        /// <summary>
        /// Tries to get a non-empty Tcp segment out of the provided <see cref="Packet"/>.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="tcp"></param>
        /// <returns></returns>
        public static bool TryGetSegment(this Packet packet, out TcpPacket tcp)
        {
            tcp = packet.Extract<TcpPacket>();
            // this should a way to get the payload length, see: 
            // https://github.com/chmorgan/packetnet/blob/master/Test/PacketType/TcpPacketTest.cs
            var result = tcp?.PayloadData.Length > 0;
            return result;
        }


        /// <summary>
        /// Gets all non-empty <see cref="TcpPacket"/> segments for the given collection of <see cref="Packet"/> instances.
        /// </summary>
        /// <param name="packets">A sequence of packets.</param>
        /// <returns>The sequence of non-empty Tcp segments.</returns>
        public static IEnumerable<TcpPacket> Segments(this IEnumerable<Packet> packets)
        {
            return packets.Select(packet => packet.TryGetSegment(out var tcp) ? tcp : null).Where(packet => packet != null);
        }
        /// <summary>
        /// Gets all non-empty <see cref="TcpPacket"/> segments for the given collection of <see cref="Packet"/> instances.
        /// </summary>
        /// <param name="packets">A sequence of packets.</param>
        /// <returns>The sequence of non-empty Tcp segments.</returns>
        public static IEnumerable<(TcpPacket, T)> Segments<T>(this IEnumerable<(Packet Packet,T Data)> packets)
        {
            return packets.Select(packet => packet.Packet.TryGetSegment(out var tcp) ? (tcp,packet.Data) : (null, packet.Data)).Where(packet => packet.tcp != null);
        }

        public static bool TryDecode<T>(this TcpPacket packet, Func<byte[],T> decoder, out T pdu)
        {
            try
            {
                if (packet.HasPayloadData)
                {
                    pdu = decoder(packet.PayloadData);
                    return true;
                }
                else
                {
                    pdu = default;
                    return false;
                }
            }
            catch(Exception)
            {
                pdu = default;
                return false;
            }
        }

        /// <summary>
        /// Converts <see cref="Packet"/> object to <see cref="RawFrame"/>.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="ticks">Time information on the packet given as the number of Ticks.</param>
        /// <returns>The <see cref="RawFrame"/> representation of the packet.</returns>
        public static RawFrame GetRawFrame(this Packet packet, long ticks)
        {
            switch(packet)
            {
                case EthernetPacket ethernetPacket:
                    return new RawFrame(LinkLayers.Ethernet, 0, ticks, 0, ethernetPacket.TotalPacketLength, ethernetPacket.Bytes);
                case LinuxSllPacket linuxSllPacket:
                    return new RawFrame(LinkLayers.LinuxSll, 0, ticks, 0, linuxSllPacket.TotalPacketLength, linuxSllPacket.Bytes);
                case NullPacket nullPacket:
                    return new RawFrame(LinkLayers.Null, 0, ticks, 0, nullPacket.TotalPacketLength, nullPacket.Bytes);
                case PppPacket pppPacket:
                    return new RawFrame(LinkLayers.Ppp, 0, ticks, 0, pppPacket.TotalPacketLength, pppPacket.Bytes);
                case MacFrame macFrame:
                    return new RawFrame(LinkLayers.Ieee80211, 0, ticks, 0, macFrame.TotalPacketLength, macFrame.Bytes);
                case RadioPacket radioPacket:
                    return new RawFrame(LinkLayers.Ieee80211Radio, 0, ticks, 0, radioPacket.TotalPacketLength, radioPacket.Bytes);
                case PpiPacket ppiPacket:
                    return new RawFrame(LinkLayers.Ppi, 0, ticks, 0, ppiPacket.TotalPacketLength, ppiPacket.Bytes);
                case RawIPPacket rawIPPacket:
                    return new RawFrame(LinkLayers.Raw, 0, ticks, 0, rawIPPacket.TotalPacketLength, rawIPPacket.Bytes);
            }
            throw new NotImplementedException($"The type {packet.GetType()} is not supported.");
        }

        public static T DecodeOrDefault<T>(this TcpPacket packet, Func<byte[], T> decoder)
        {
            return TryDecode<T>(packet, decoder, out var pdu) ? pdu : default;
        }
        public static bool TryDecode<T>(this TcpPacket packet, Func<KaitaiStream, T> decoder, out T pdu)
        {
            try
            {
                if (packet.HasPayloadData)
                {
                    pdu = decoder(new KaitaiStream(packet.PayloadData));
                    return true;
                }
                else
                {
                    pdu = default;
                    return false;
                }
            }
            catch (Exception)
            {
                pdu = default;
                return false;
            }
        }
        public static T DecodeOrDefault<T>(this TcpPacket packet, Func<KaitaiStream, T> decoder)
        {
            return TryDecode<T>(packet,decoder, out var pdu) ? pdu : default;
        }
    }
}
