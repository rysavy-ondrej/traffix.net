using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Linq;
using Traffix.Providers.PcapFile;
using Traffix.Core.Observable;
using Traffix.Core.Flows;
using PacketDotNet;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FasterConversationTablePerf
{
    [SimpleJob(RunStrategy.Monitoring, targetCount: 5)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class IngestPacketTraceBenchmarkObservable
    {
        [Params(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151021.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.pcap")]
        public string dataset;


        private (long Ticks, Packet Packet) GetPacket(RawFrame arg)
        {
            return (arg.Ticks, Packet.ParsePacket(arg.LinkLayer, arg.Data));
        }
        private async Task<string> FlowProcessor(FlowKey flowKey, IObservable<(long Ticks, Packet Packet)> packets)
        {
            var packetCount = 0;
            var firstSeen = long.MaxValue;
            var lastSeen = long.MinValue;
            var octets = 0;
            await packets.ForEachAsync(packet =>
            {
                packetCount++;
                octets += packet.Packet.TotalPacketLength;
                firstSeen = Math.Min(firstSeen, packet.Ticks);
                lastSeen = Math.Max(lastSeen, packet.Ticks);
            });
            return $"  Flow {flowKey}: firstSeen={new DateTime(firstSeen)}, duration={new TimeSpan(lastSeen - firstSeen)}, packets={packetCount}, octets={octets}";
        }

        [Benchmark]
        public async Task ExportWindowedConversationsObservable()
        {
            var observable = SharpPcapReader.CreateObservable(dataset).Select(GetPacket);
            // get windows of flows:
            var wins = observable.TimeWindow(t => t.Ticks, TimeSpan.FromMinutes(5)).Select(o => o.GroupBy(f => f.Packet.GetFlowKey()));
            var windowNumber = 0;
            await wins.ForEachAsync(async win =>
            {
                Console.Write($"Window {++windowNumber}:  ");
                await win.ApplyFlowProcessor(FlowProcessor).ForEachAsync(flowStr =>
                {
                    Console.Write('.');
                });
                Console.WriteLine();
            });
        }
    }
}
