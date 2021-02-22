using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Traffix.Core.Flows;
using Traffix.Data;
using Traffix.Processors;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster.Tests
{

    public class FasterConversationTableBenchmarks
    {

    }

    [TestClass]
    public  class FasterConversationTableTests
    {
        /// <summary>
        /// Loads the table from the given pcap file.
        /// </summary>
        [TestMethod]
        public void CreateTable()
        {
            var sw = new Stopwatch();
            var pcapPath = Path.GetFullPath(@"data\PCAP\modbus.pcap");
            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            if (Directory.Exists(dbPath)) Directory.Delete(dbPath, true);

            var flowTable = FasterConversationTable.Create(dbPath, framesCapacity: 1700000);
            var frameNumber = 0;
            sw.Restart();
            using (var loader = flowTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(pcapPath))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    frameNumber++;
                    loader.AddFrame(rawFrame);
                }
                loader.Close();
            }

            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Convs= {flowTable.ConversationsCount} [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount} /{frameNumber} [{sw.Elapsed}]");
            sw.Restart();
            flowTable.SaveChanges();
            Console.WriteLine($"--- COMMITED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount} / {frameNumber} [{sw.Elapsed}]");

            flowTable.Dispose();
        }

        private void PrintConversations(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ConversationKey, ConversationValue>> conversations)
        {
            foreach (var c in conversations)
            {
                Console.WriteLine(c.Key);
            }
        }

        /// <summary>
        /// Open existing table.
        /// </summary>
        [TestMethod]
        public void ReadExistingTable()
        {
            var sw = new Stopwatch();
            sw.Start();
            var flowTable = OpenTable();
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Convs= {flowTable.ConversationsCount} [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount}  [{sw.Elapsed}]");
        }

        [TestMethod]
        public void ReadRawFrames()
        {
            var sw = new Stopwatch();
            sw.Start();
            var flowTable = OpenTable();
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            var frames = flowTable.ProcessFrames<RawFrame>(flowTable.FrameKeys, new FasterConversationTable.RawFrameProcessor());
            var allFrames = 0;
            var otherPackets = 0;
            var ethernetPackets = 0;
            var ipPackets = 0;
            var tcpPackets = 0;
            var udpPackets = 0;

            foreach (var frame in frames)
            {
                allFrames++;
                if (frame.LinkLayer != PacketDotNet.LinkLayers.Ethernet) otherPackets++;
                else
                {
                    ethernetPackets++;
                    var packet = PacketDotNet.Packet.ParsePacket(frame.LinkLayer, frame.Data);
                    if (packet.Extract<PacketDotNet.InternetPacket>() != null) ipPackets++;
                    if (packet.Extract<PacketDotNet.UdpPacket>() != null) udpPackets++;
                    if (packet.Extract<PacketDotNet.TcpPacket>() != null) tcpPackets++;
                }
            }
            Console.WriteLine($"--- CHECKED --- [{sw.Elapsed}]");
            Console.WriteLine($"Frames={allFrames}]");
            Console.WriteLine($"Ethernet={ethernetPackets}]");
            Console.WriteLine($"Packets={ipPackets}]");
            Console.WriteLine($"TCP={tcpPackets}]");
            Console.WriteLine($"UDP={udpPackets}]");
        }

        /// <summary>
        /// Opens the existing table.
        /// </summary>
        /// <returns></returns>
        public static FasterConversationTable OpenTable()
        {
            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            var table = FasterConversationTable.Open(dbPath);
            return table;
        }

        [TestMethod]
        public void ReadAllFrames()
        {
            var sw = new Stopwatch();
            sw.Start();
            var flowTable = OpenTable();
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            int frames = 0;
            long octets = 0;
            var rawFrames = flowTable.ProcessFrames<RawFrame>(flowTable.FrameKeys, new FasterConversationTable.RawFrameProcessor());
            foreach (var f in rawFrames)
            {
                frames++;
                octets += f.OriginalLength;
            }
            Console.WriteLine($"Frames={frames}, Octets={octets}  [{sw.Elapsed}]");
        }

        /// <summary>
        /// Opens the existing table and counts all frames using frame processor of all conversations.
        /// </summary>
        [TestMethod]
        public void ReadAllFramesOfConversations()
        {
            var sw = new Stopwatch();
            sw.Start();
            var table = OpenTable();
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            foreach (var c in table.ProcessConversations(table.ConversationKeys, ConversationProcessor.FromFunction<(string key, int frames, int octets, int ip, int tcp, int udp)>(CountFrames)))
            {
                Console.WriteLine($"Conversatiom={c.key}, Frames={c.frames}, IP={c.ip}, TCP={c.tcp}, UDP={c.udp}, Octets={c.octets}  [{sw.Elapsed}]");
            }
        }

        /// <summary>
        /// Implements a function to be used in the Count Frame conversation processor.
        /// </summary>
        /// <param name="flowKey"></param>
        /// <param name="frames"></param>
        /// <returns></returns>
        public static (string key, int frames, int octets, int ip, int tcp, int udp) CountFrames(FlowKey flowKey, System.Collections.Generic.IEnumerable<Memory<byte>> frames)
        {
            (int octets, int ip, int tcp, int udp) GetFrameSize(Memory<byte> memory)
            {
                var meta = default(FrameMetadata);
                var bytes = FrameMetadata.GetFrameFromMemory(memory, ref meta);
                var packet = PacketDotNet.Packet.ParsePacket((PacketDotNet.LinkLayers)meta.LinkLayer, bytes.ToArray());
                return (meta.OriginalLength,
                    packet.Extract<PacketDotNet.InternetPacket>() != null ? 1 : 0,
                    packet.Extract<PacketDotNet.TcpPacket>() != null ? 1 : 0,
                    packet.Extract<PacketDotNet.UdpPacket>() != null ? 1 : 0
                    );
            }
            var framesList = frames.ToList();
            // intentionally it is computed in this inefficient way to test 
            // the implementated iteration over the input collection
            return (flowKey.ToString(), framesList.Count,
                 framesList.Sum(x => GetFrameSize(x).octets),
                 framesList.Sum(x => GetFrameSize(x).ip),
                 framesList.Sum(x => GetFrameSize(x).tcp),
                 framesList.Sum(x => GetFrameSize(x).udp)
                );
        }

        [TestMethod]
        public void ConversationsInWindow()
        {
            var sw = new Stopwatch();
            sw.Start();
            var table = OpenTable();
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            var firstPacketTime = DateTimeOffset.FromUnixTimeSeconds(table.FrameKeys.First().Epoch);
            var lastPacketTime = DateTimeOffset.FromUnixTimeSeconds(table.FrameKeys.Last().Epoch);
            // create 10 windows
            var windowSpan = (lastPacketTime - firstPacketTime) / 10;
            var windows = table.Conversations.GroupByWindow(firstPacketTime.DateTime, windowSpan);
            var processor = ConversationProcessor.FromFunction((key, frames) => $"{key} : {frames.Count()}");
            foreach (var win in windows)
            {
                Console.WriteLine($"{win.Key} - {win.Key + windowSpan}:");
                var conversations = table.ProcessConversations(win, processor.ApplyToWindow(win.Key, windowSpan));
                foreach (var c in conversations)
                {
                    Console.WriteLine($"  {c}");
                }
            }
        }
    }
}
