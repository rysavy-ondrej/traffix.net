using ConsoleAppFramework;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Traffix.Hosting.Console;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;

namespace IcsMonitor
{
    public partial class Program : TraffixConsoleApp
    {

        /// <summary>
        /// Export the payload of tcp segments. Each payload is put in a single binary file created in the target zip archive.  
        /// </summary>
        /// <param name="inputFile">The name of the input pcap file.</param>
        /// <param name="port">The port number of the TCP packet to be included.</param>
        /// <param name="outputFile">The name of the output zip file. If not specified the name is the same as the inpout name but with zip extension.</param>
        [Command("Export-TcpPayload")]
        public void ExportTcpPayload(
            string inputFile,
            int? port = null,
            string outputFile = null)
        {
            using var stream = new FileInfo(inputFile).OpenRead();
            using var captureReader = new CaptureFileReader(stream);
            using var payloadArchive = ZipFile.Open(outputFile, ZipArchiveMode.Update);
            int packetNumber = 0;
            while (captureReader.GetNextFrame(out var frame))
            {
                packetNumber++;
                var packet = Packet.ParsePacket(frame.LinkLayer, frame.Data);
                var tcpPacket = packet.Extract<TcpPacket>();
                if (tcpPacket != null)
                {
                    if (port == null || tcpPacket.SourcePort == port || tcpPacket.DestinationPort == port )
                    {
                        WriteNewFile(payloadArchive, packetNumber, tcpPacket.PayloadData);
                    }
                }
            }
        }

        private void WriteNewFile(ZipArchive payloadArchive, int packetNumber, ReadOnlySpan<byte> payloadData)
        {
            var entry = payloadArchive.CreateEntry($"{packetNumber.ToString("D8")}.bin");
            using var writer = new BinaryWriter(entry.Open());
            writer.Write(payloadData);
        }


        [Command("Copy-Pcap")]
        public void CopyPcap(string inputFile, string outputFile)
        {
            var sw = new Stopwatch();
            var ctx = new Interactive();
            sw.Start();
            Console.Write("Reading frames to conversation table...");
            var reader = ctx.CreateCaptureFileReader(inputFile, false);
            var table = ctx.CreateConversationTable(ctx.GetNextFrames(reader), ctx.TempDirectory.FullName);
            Console.WriteLine($"done [{sw.Elapsed}].");
            sw.Restart();
            Console.Write($"Writing frames to '{outputFile}' file...");
            var regularFrames = ctx.GetPackets(table);
            ctx.WriteToFile(regularFrames.Select(p => p.Packet.GetRawFrame(p.Ticks)), outputFile);
            Console.WriteLine($"done [{sw.Elapsed}].");
        }


        // Try to find a conversation that corresponds to the Factory meta conversation:
        class FactoryMetaProcessor : CustomConversationProcessor<bool>
        {
            bool MatchPattern(byte[] segmentPayload)
            {
                return segmentPayload.Length == 12
                    && segmentPayload[7] == 02
                    && segmentPayload[10] == 00
                    && segmentPayload[11] == 01;

            }
            protected override bool Invoke(IReadOnlyCollection<(FrameMetadata Meta, Packet Packet)> forward, IReadOnlyCollection<(FrameMetadata Meta, Packet Packet)> reverse)
            {
                return forward.Select(x => x.Packet).Segments().Take(5).All(p => MatchPattern(p.PayloadData));
            }
        }

        [Command("Redact-FactoryCapture")]
        public void RedactFactoryCapture(string time, string inputFile, string outputFile)
        {

            var timeOrigin = DateTime.Parse(time).Ticks;
            if (!File.Exists(inputFile)) throw new FileNotFoundException($"Input file '{inputFile}' not found.");
            var sw = new Stopwatch();
            var ctx = new Interactive();
            sw.Start();

            // Load capture file:
            Console.Write($"Loading capture file '{inputFile}' to internal db '{ctx.TempDirectory}'...");
            var reader = ctx.CreateCaptureFileReader(inputFile, false);
            var table = ctx.CreateConversationTable(ctx.GetNextFrames(reader), ctx.TempDirectory.FullName);
            Console.WriteLine("done.");
            var firstFrame = ctx.GetPackets(table).First();
            var timeBaseTicks = firstFrame.Ticks;
            var frameCount = ctx.GetPackets(table).Count();
            Console.WriteLine($"Frames count={frameCount}, first frame={new DateTime(timeBaseTicks)}");

            var processor = new FactoryMetaProcessor();
            var conversations = ctx.GetConversations(table, new FactoryMetaProcessor()).ToList();
            var factoryConversation = conversations.FirstOrDefault(c => c.Data == true);

            if (factoryConversation != null)        
                Console.WriteLine($"Found factory conversation {factoryConversation.Key}");
            else
                Console.WriteLine($"Factory conversation not found.");

            // we need to find the first non-meta frame:
            var regularFrames = ctx.GetPackets(table).Where(p => factoryConversation != null ? !ctx.ContainsPacket(table, factoryConversation?.Key, p.Packet) : true);

        
            var firstRegularFrameTicks = regularFrames.First().Ticks;
            var regularFramesCount = regularFrames.Count();
            var offset = firstRegularFrameTicks - timeOrigin;

            Console.WriteLine($"Regular frames count={regularFramesCount}, first regular frame={new DateTime(firstRegularFrameTicks)}");
            // write output frames:
            // remove factory conversation and shift the time if necessary
            Console.Write($"Writing frames to '{outputFile}', time shift {offset} ticks....");
            ctx.WriteToFile(regularFrames.Select(p => p.Packet.GetRawFrame(p.Ticks - offset)), outputFile);
            Console.WriteLine("done.");
            table.Dispose();
            ctx.Dispose();
        }
    }
}
