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
    [TestClass]
    public class FasterConversationTableTests
    {
        /// <summary>
        /// Loads the table from the given pcap file.
        /// </summary>
        [TestMethod]
        public void CreateTable()
        {
            var sw = new Stopwatch();
            var pcapPath = Path.GetFullPath(@"data\PCAP\modbus.pcap");
            //var pcapPath = Path.GetFullPath(@"C:\Users\user\Captures\sorting_station_ver2.pcap");

            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            if (Directory.Exists(dbPath)) Directory.Delete(dbPath, true);
            sw.Start();
            var flowTable = FasterConversationTable.Create(dbPath, framesCapacity:1700000);
            var frameNumber = 0;
            using (var loader = flowTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(pcapPath))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame, ++frameNumber);
                }
                loader.Close();
            }
                        
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Convs= {flowTable.Conversations.Count()} [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount} /{frameNumber} [{sw.Elapsed}]");
            sw.Restart();
            flowTable.SaveChanges();
            Console.WriteLine($"--- COMMITED --- [{sw.Elapsed}]");
            sw.Restart();
            Console.WriteLine($"Frames= {flowTable.FramesCount} / {frameNumber} [{sw.Elapsed}]");

            flowTable.Dispose();
        }

        private void PrintConversations(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ConversationKey, IConversationValue>> conversations)
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
            var frames = flowTable.GetRawFrames();
            var allFrames = 0;
            var otherPackets = 0;
            var ethernetPackets = 0;
            var ipPackets = 0;
            var tcpPackets = 0;
            var udpPackets = 0;

            foreach(var frame in frames)
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
            var table = OpenTable();
            Console.WriteLine($"--- LOADED --- [{sw.Elapsed}]");
            sw.Restart();
            int frames = 0;
            long octets = 0;
            foreach (var f in table.GetRawFrames())
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
            foreach (var c in table.ProcessConversations(table.ConversationKeys, new FuncConversationProcessor<(string key, int frames, int octets, int ip, int tcp, int udp)>(CountFrames)))
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
        public static (string key, int frames, int octets, int ip, int tcp, int udp) CountFrames(FlowKey flowKey, System.Collections.Generic.ICollection<Memory<byte>> frames)
        {
            (int octets, int ip,int tcp,int udp) GetFrameSize(Memory<byte> memory)
            {
                var meta = default(FrameMetadata);
                var bytes = ConversationProcessor.GetFrameFromMemory(memory, ref meta);
                var packet = PacketDotNet.Packet.ParsePacket((PacketDotNet.LinkLayers)meta.LinkLayer, bytes.ToArray());
                return (meta.OriginalLength, 
                    packet.Extract<PacketDotNet.InternetPacket>() != null ? 1 : 0,
                    packet.Extract<PacketDotNet.TcpPacket>() != null ? 1 : 0,
                    packet.Extract<PacketDotNet.UdpPacket>() != null ? 1 : 0
                    );
            }
            // intentionally it is computed in this inefficient way to test 
            // the implementated iteration over the input collection
            return (flowKey.ToString(), frames.Count, 
                 frames.Sum(x=> GetFrameSize(x).octets),
                 frames.Sum(x => GetFrameSize(x).ip),
                 frames.Sum(x => GetFrameSize(x).tcp),
                 frames.Sum(x => GetFrameSize(x).udp)
                );
        }
    }
}
