using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using Traffix.Providers.PcapFile;

namespace FasterConversationTable.Perf
{
    [SimpleJob(RunStrategy.Monitoring, targetCount: 10)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class IngestPacketTraceBenchmark
    {

        [Params(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151021.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.pcap")]
        public string dataset;

        [GlobalSetup]
        public void Setup()
        {
        }

        int loadTableRun = 0;
        [Benchmark]
        public void LoadTable()
        {
            loadTableRun++;
            using var flowTable = Traffix.Storage.Faster.FasterConversationTable.Create($"{dataset}.{loadTableRun}", 3000000);
            using (var loader = flowTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(dataset))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame);
                }
                loader.Close();
            }
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
