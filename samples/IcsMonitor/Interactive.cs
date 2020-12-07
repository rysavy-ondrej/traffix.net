using Kaitai;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using Traffix.Hosting.Console;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;

namespace IcsMonitor
{
    /// <summary>
    /// Interactive class exposes various API methods usable in C# interactive sessions. 
    /// </summary>
    public class Interactive
    {
        public FasterConversationTable ReadToConversationTable(string pcapFile, string conversationTablePath, CancellationToken token)
        {
            using var stream = new FileInfo(pcapFile).OpenRead();
            return ReadToConversationTable(stream, conversationTablePath, token);
        }
        public FasterConversationTable ReadToConversationTable(Stream stream, string conversationTablePath, CancellationToken token)
        {
            var flowTable = new FasterConversationTable(conversationTablePath);
            flowTable.LoadFromStream(stream, token, null);
            return flowTable;
        }

        public IEnumerable<T> GetConversations<T> (FasterConversationTable table, IConversationProcessor<T> processor)
        {
            return table.ProcessConversations<T>(table.ConversationKeys, processor);
        }

        /// <summary>
        /// Reads the input file and creates a collection of conversation tables for the intervals of the specified duration.
        /// <para>
        /// This method enables to create mutliple conversation tables for given time interval. For instance if the time intevral is set to X minutes
        /// thenn every X minutes a new conversation table is created. It means that long running conversations can be split to multiple tables. 
        /// </para>
        /// </summary>
        /// <param name="reader">The packet capture reader.</param>
        /// <param name="conversationTablePath">The root folder for storing data of the conversation tables.</param>
        /// <param name="timeInterval">The time interval for capturing packets to a conversation table.</param>
        /// <param name="token">the cancellation token.</param>
        /// <returns>A collection of conversation tables.</returns>
        public IEnumerable<FasterConversationTable> ReadToConversationTables(CaptureFileReader reader, string conversationTablePath, TimeSpan timeInterval, CancellationToken token)
        {
            return PopulateConversationTables(GetNextFrames(reader), conversationTablePath, timeInterval, token);
        }

        public IEnumerable<FasterConversationTable> PopulateConversationTables(IEnumerable<RawFrame> frames, string conversationTablePath, TimeSpan timeInterval, CancellationToken token)
        {
            long? startWindowTicks = null;
            long timeIntervalTicks = timeInterval.Ticks;
            int flowTableNum = 0;
            var flowTable = new FasterConversationTable(Path.Combine(conversationTablePath, flowTableNum.ToString("D4")));

            var loader = flowTable.GetFrameLoader();
            foreach(var frame in frames)
            {
                if (startWindowTicks is null) startWindowTicks = frame.Ticks;
                if (frame.Ticks > startWindowTicks + timeIntervalTicks)
                {
                    yield return flowTable;
                    flowTableNum++;
                    flowTable = new FasterConversationTable(Path.Combine(conversationTablePath, flowTableNum.ToString("D4")));
                    loader.Dispose();
                    loader = flowTable.GetFrameLoader();
                    startWindowTicks += timeIntervalTicks;
                }

                loader.AddFrame(frame, frame.Data, frame.Offset);
                if (token.IsCancellationRequested) break;
            }
            loader.Dispose();
            yield return flowTable;
        }

        public CaptureFileReader CreateCaptureFileReader(string path)
        {
            return new CaptureFileReader(new FileInfo(path).OpenRead());
        }

        public IEnumerable<RawFrame> GetNextFrames(CaptureFileReader reader, int count = Int32.MaxValue)
        { 
            for (int i = 0; i < count; i++)
            {
                if (reader.GetNextFrame(out var frame, true))
                {
                    yield return frame;
                }
                else
                {
                    break;
                }
            }
        }
    }


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
