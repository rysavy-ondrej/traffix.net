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
                await window.ForEachAsync(_ =>
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
            await flows.ApplyFlowProcessor(FlowProcessorFunc).ForEachAsync(_ =>
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
                await window.ApplyFlowProcessor(FlowProcessorFunc).ForEachAsync(_ =>
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

        /// <summary>
        /// Creates flows using flow processor applied in the consumer phase.
        /// </summary>
        [Benchmark]
        public async Task ExportWindowedFlowsByProcessor()
        {
            var packets = SharpPcapReader.CreateObservable(dataset).Select(GetPacketAndKey);
            var windows = packets.TimeSpanWindow(t => t.Ticks, TimeSpan.FromMinutes(5));
            var windowCount = 0;
            var totalPackets = 0;
            await windows.ForEachAsync(async window =>
            {
                windowCount++;
                var flowProcessor = new NetFlowProcessor();
                await window.Do(_ => totalPackets++).ForEachAsync(p => flowProcessor.OnNext(p));
                Console.WriteLine($"Window = {windowCount},  Flows = {flowProcessor.Count},  Packets = {totalPackets}");
            });
        }

        #region Helper methods
        private (long Ticks, FlowKey Key, Packet Packet) GetPacketAndKey(RawFrame arg)
        {
            var packet = GetPacket(arg);
            return (packet.Ticks, packet.Packet.GetFlowKey(), packet.Packet);
        }
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
        private async Task<string> FlowProcessorFunc(FlowKey flowKey, IObservable<(long Ticks, Packet Packet)> packets)
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

        class NetFlowProcessor : FlowProcessor<(long, FlowKey, Packet), FlowKey, NetFlowProcessor.NetFlowRecord>
        {
            protected override NetFlowRecord Aggregate(NetFlowRecord arg1, NetFlowRecord arg2)
            {
                return NetFlowRecord.Aggregate(arg1, arg2);
            }

            protected override NetFlowRecord Create((long, FlowKey, Packet) source)
            {
                return NetFlowRecord.Create(source);
            }

            protected override FlowKey GetKey((long, FlowKey, Packet) source)
            {
                return source.Item2;
            }

            protected override void Update(NetFlowRecord record, (long, FlowKey, Packet) source)
            {
                NetFlowRecord.Update(record, source);
            }

            public class NetFlowRecord
            {
                public long Octets { get; set; }
                public int Packets { get; set; }
                public long FirstSeen { get; set; }
                public long LastSeen { get; set; }

                public static NetFlowRecord Create((long, FlowKey, Packet) c)
                {
                    return new NetFlowRecord { FirstSeen = c.Item1, LastSeen = c.Item1, Octets = c.Item3.TotalPacketLength, Packets = 1 };
                }

                public static void Update(NetFlowRecord record, (long, FlowKey, Packet) packet)
                {
                    record.Packets++;
                    record.Octets += packet.Item3.TotalPacketLength;
                    record.FirstSeen = Math.Min(record.FirstSeen, packet.Item1);
                    record.LastSeen = Math.Max(record.FirstSeen, packet.Item1);
                }

                public static NetFlowRecord Aggregate(NetFlowRecord arg1, NetFlowRecord arg2)
                {
                    return new NetFlowRecord
                    {
                        Octets = arg1.Octets + arg2.Octets,
                        Packets = arg1.Packets + arg2.Packets,
                        FirstSeen = Math.Min(arg1.FirstSeen, arg2.FirstSeen),
                        LastSeen = Math.Max(arg1.LastSeen, arg2.LastSeen)
                    };
                }

            }
        }
        #endregion
    }
}
