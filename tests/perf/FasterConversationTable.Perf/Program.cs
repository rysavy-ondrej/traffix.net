using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Linq;
using Traffix.Providers.PcapFile;

namespace FasterConversationTable.Perf
{
    [SimpleJob(RunStrategy.Monitoring, targetCount: 5)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class IngestPacketTraceBenchmark
    {

        [Params(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151021.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.pcap")]
        public string dataset;

        [GlobalSetup]
        public void Setup()
        {
            _conversationTable = Traffix.Storage.Faster.FasterConversationTable.Create($"{dataset}.{loadTableRun}", 3000000);
            using (var loader = _conversationTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(dataset))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame);
                }
                loader.Close();
            }
        }
        Traffix.Storage.Faster.FasterConversationTable _conversationTable;

        int loadTableRun = 0;
        [Benchmark]
        public void LoadTable()
        {
            loadTableRun++;
            using var conversationTable = Traffix.Storage.Faster.FasterConversationTable.Create($"{dataset}.{loadTableRun}", 3000000);
            using (var loader = conversationTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(dataset))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame);
                }
                loader.Close();
            }
        }

        [Benchmark]
        public void ExportPackets()
        {
            var packets = _conversationTable.ProcessFrames(_conversationTable.FrameKeys, new Traffix.Storage.Faster.FasterConversationTable.PacketFrameProcessor());
            packets.Count();
        }
        [Benchmark]
        public void ExportConversations()
        {
            var conversations = _conversationTable.ProcessConversations(_conversationTable.ConversationKeys, new Traffix.Storage.Faster.FasterConversationTable.PacketConversationProcessor());
            conversations.Count();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IngestPacketTraceBenchmark>();
        }
    }
}
