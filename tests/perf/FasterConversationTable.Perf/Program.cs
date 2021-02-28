using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Linq;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;
using Traffix.Processors;
namespace FasterConversationTablePerf
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
            _conversationTable = FasterConversationTable.Create($"{dataset}.{loadTableRun}", 3000000);
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
            using var conversationTable = FasterConversationTable.Create($"{dataset}.{loadTableRun}", 3000000);
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
            var packets = _conversationTable.ProcessFrames(_conversationTable.FrameKeys, new FasterConversationTable.PacketFrameProcessor());
            packets.Count();
        }
        [Benchmark]
        public void ExportConversations()
        {
            var conversations = _conversationTable.ProcessConversations(_conversationTable.ConversationKeys, new FasterConversationTable.PacketConversationProcessor());
            conversations.Count();
        }
        [Benchmark]
        public void GroupWindowedConversations()
        {
            var start = DateTimeOffset.FromUnixTimeSeconds(_conversationTable.FrameKeys.First().Epoch);
            var end = DateTimeOffset.FromUnixTimeSeconds(_conversationTable.FrameKeys.Last().Epoch);
            var span = (end - start) / 100;
            var windows = _conversationTable.Conversations.GroupByWindow(start.DateTime, span);
            windows.Count();
        }
        [Benchmark]
        public void ExportWindowedConversations()
        {
            var start = DateTimeOffset.FromUnixTimeSeconds(_conversationTable.FrameKeys.First().Epoch);
            var end = DateTimeOffset.FromUnixTimeSeconds(_conversationTable.FrameKeys.Last().Epoch);
            var span = (end - start) / 100;
            var windows = _conversationTable.Conversations.GroupByWindow(start.DateTime, span);
            var conversations = windows.SelectMany(conversationKeys =>
                _conversationTable.ProcessConversations(conversationKeys, new FasterConversationTable.PacketConversationProcessor().ApplyToWindow(start.DateTime, span)));
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
