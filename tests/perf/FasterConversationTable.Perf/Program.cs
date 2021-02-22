using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.IO;

namespace FasterConversationTable.Perf
{
    class Program
    {

        public class IngestPacketTraceBenchmark
        {
            private byte[] data;

            [Params(5)]
            public int N;

            [GlobalSetup]
            public void Setup()
            {
            }


            [Benchmark]
            public void LoadTable1()
            {
                CreateTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.nfx", 300000, @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.pcap");
                DeleteTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.nfx");
            }
            [Benchmark]
            public void LoadTable2()
            {
                CreateTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151021.nfx", 1500000, @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.pcap");
                DeleteTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151021.nfx");
            }
            [Benchmark]
            public void LoadTable3()
            {
                CreateTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.nfx", 2500000, @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.pcap");
                DeleteTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.nfx");
            }

            [Benchmark]
            public void ReadExistingTable1() => OpenTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.nfx");
            [Benchmark]
            public void ReadExistingTable2() => OpenTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151021.nfx");
            [Benchmark]
            public void ReadExistingTable3() => OpenTable(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.nfx");


            public static Traffix.Storage.Faster.FasterConversationTable OpenTable(string tablePath)
            {
                var table = Traffix.Storage.Faster.FasterConversationTable.Open(tablePath);
                return table;
            }
            public static Traffix.Storage.Faster.FasterConversationTable CreateTable(string tablePath, long framesCount, string sourcePcap)
            {
                var table = Traffix.Storage.Faster.FasterConversationTable.Create(tablePath, framesCount);
                return table;
            }
            public static void DeleteTable(string tablePath)
            {
                Directory.Delete(tablePath, true);
            }
        }

        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IngestPacketTraceBenchmark>();
        }
    }
}
