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
    [SimpleJob(RunStrategy.Monitoring, targetCount: 2)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class IngestPacketTraceBenchmarkObservable
    {
        [Params(@"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151020.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151021.pcap", @"D:\Captures\ecbs21paper.datasets\4SICS-GeekLounge-151022.pcap")]
        public string dataset;

        /// <summary>
        /// Reads all frames from the source file and converts them to Packet objects. 
        /// </summary>
        [Benchmark]
        public async Task ExportPackets()
        {
            var packetCount = 0;
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacket);
            await packets.ForEachAsync(packet =>
            {
                packetCount++;
            });
        }
        /// <summary>
        /// Reads all frames from the source file and converts them to Packet objects. 
        /// </summary>
        [Benchmark]
        public async Task ExportPacketsWithFlowKey()
        {
            var packetCount = 0;
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacket).Select(x=>(x.Ticks,x.Packet,x.Packet.GetFlowKey()));
            await packets.ForEachAsync(packet =>
            {
                packetCount++;
            });
        }
        /// <summary>
        /// Reads all frames from the source file, converts them to Packet objects, and groups them in windows (5 duration).  
        /// </summary>
        /// <remarks>This does not run correctly. Why?</remarks>
        [Benchmark]
        public async Task ExportWindowedPackets()
        {
            var packetCount = 0;
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacket);
            var windows = packets.TimeSpanWindow(t => t.Ticks, TimeSpan.FromMinutes(5));
            await windows.ForEachAsync(async window =>
            {
                await packets.ForEachAsync(_ =>
                {
                    packetCount++;
                });
            });
        }

        /// <summary>
        /// Reads all frames and groups them to flows.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task ExportFlows()
        {
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacket);
            var flows = packets.GroupFlows(f => f.Packet.GetFlowKey());
            var flowCount = 0;
            await flows.ApplyFlowProcessor(FlowProcessor).ForEachAsync(_ =>
            {
                flowCount++;
            });
        }
        /// <summary>
        /// Creates flows from source packets grouped in windows.
        /// </summary>
        [Benchmark]
        public async Task ExportWindowedFlows()
        {
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacket);
            var windows = packets.TimeSpanWindow(t => t.Ticks, TimeSpan.FromMinutes(5)).Select(packets => packets.GroupFlows(f => f.Packet.GetFlowKey()));
            var windowCount = 0;
            var flowCount = 0;
            await windows.ForEachAsync(async window =>
            {
                windowCount++;
                await window.ApplyFlowProcessor(FlowProcessor).ForEachAsync(_ =>
                {
                    flowCount++;
                });
            });
        }

        /// <summary>
        /// Creates conversations from source packets.
        /// </summary>
        [Benchmark]
        public async Task ExportConversations()
        {
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacket);
            var conversations = packets.GroupConversations(f => f.Packet.GetFlowKey(), f => GetConversationKey(f));
            var conversationCount = 0;
            await conversations.ApplyFlowProcessor(ConversationProcessor).ForEachAsync(_ =>
            {
                conversationCount++;
            });
        }

        /// <summary>
        /// Creates conversations from source packets grouped in windows.
        /// </summary>
        [Benchmark]
        public async Task ExportWindowedConversations()
        {
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacket);
            var windows = packets.TimeSpanWindow(t => t.Ticks, TimeSpan.FromMinutes(5)).Select(packets => packets.GroupConversations(f => f.Packet.GetFlowKey(), f => GetConversationKey(f)));
            var windowCount = 0;
            var conversationCount = 0;
            await windows.ForEachAsync(async window =>
            {
                windowCount++;
                await window.ApplyFlowProcessor(ConversationProcessor).ForEachAsync(_ =>
                {
                    conversationCount++;
                });
            });
        }

        #region Helper methods
        private (long Ticks, Packet Packet) GetPacket(RawFrame arg)
        {
            return (arg.Ticks, Packet.ParsePacket(arg.LinkLayer, arg.Data));
        }
        private FlowKey GetConversationKey(FlowKey key)
        {
            var revKey = key.Reverse();
            var sp1 = key.SourcePort;
            var sp2 = revKey.SourcePort;
            return sp1 > sp2 ? key : revKey;
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
        private async Task<string> ConversationProcessor(FlowKey conversationKey, IObservable<IGroupedObservable<FlowKey, (long Ticks, Packet Packet)>> flows)
        {
            FlowKey flowKey = null;
            var packetCount = 0;
            var firstSeen = long.MaxValue;
            var lastSeen = long.MinValue;
            var octets = 0;
            var flowCount = 0;
            await flows.ForEachAsync(async flow =>
            {
                flowKey ??= flow.Key;
                flowCount++;
                await flow.ForEachAsync(packet =>
                {
                    packetCount++;
                    octets += packet.Packet.TotalPacketLength;
                    firstSeen = Math.Min(firstSeen, packet.Ticks);
                    lastSeen = Math.Max(lastSeen, packet.Ticks);
                });
            });
            return $"  Conv ({conversationKey}) {flowKey}: flows={flowCount}, firstSeen={new DateTime(firstSeen)}, duration={new TimeSpan(lastSeen - firstSeen)}, packets={packetCount}, octets={octets}";
        }
        #endregion
    }
}
