using Microsoft.StreamProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Traffix.Core.Flows;
using Traffix.Core.Observable;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster.Tests
{
    [TestClass]
    public class ObservablePacketTraceTests
    {
        /// <summary>
        /// Check if we have test data in the expected location.
        /// If this test fail, please execute fetch-testdata.cmd or fetch-testdata.sh 
        /// in the test project testdata folder.
        /// </summary>
        [TestMethod]
        public void CheckDataAvailable()
        {
            Assert.IsTrue(File.Exists(Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap")));
        }

        /// <summary>
        /// Loads the table from the given pcap file.
        /// </summary>
        [TestMethod]
        public async Task TestCreateFrameTable()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            var dbPath = Path.GetFullPath(@"c:\temp\0001\");
            if (Directory.Exists(dbPath)) Directory.Delete(dbPath, true);

            using var flowTable = FasterFrameTable.Create(dbPath, framesCapacity: 1700000);
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
            flowTable.SaveChanges();
            Console.WriteLine($"--- COMMITED --- [{sw.Elapsed}]");
            sw.Restart();
            var observable = flowTable.GetObservable(TestHelperFunctions.FrameProcessor);
            var packetCount = 0;
            var flows = new Dictionary<FlowKey, int>();

            void UpdatePacket(Packet p)
            {
                packetCount++;
                var fk = p.GetFlowKey();
                flows.TryGetValue(fk, out var count);
                flows[fk] = count++;
            }

            await observable.ForEachAsync(UpdatePacket);

            Console.WriteLine($"- Packets = {packetCount} [{sw.Elapsed}]");
            Console.WriteLine($"- Flows = {flows.Count}");
        }

        [TestMethod]
        public async Task TestObservableFromReader()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
            var packetCount = 0;
            var flows = new Dictionary<FlowKey, int>();

            void UpdatePacket(Packet p)
            {
                packetCount++;
                var fk = p.GetFlowKey();
                flows.TryGetValue(fk, out var count);
                flows[fk] = count++;
            }

            await observable.ForEachAsync(f => UpdatePacket(f.Packet));

            Console.WriteLine($"- Packets = {packetCount} [{sw.Elapsed}]");
            Console.WriteLine($"- Flows = {flows.Count}");
        }
        [TestMethod]
        public async Task TestCountWindowOperator()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
            var wins = observable.Window(100).Select(o => o.GroupBy(f => f.Packet.GetFlowKey()));
            var windowNumber = 0;
            await wins.ForEachAsync(async win =>
            {
                Console.WriteLine($"Window {++windowNumber}");
                await win.ForEachAsync(async flow =>
                {
                    var packets = 0;
                    var octets = 0;
                    var firstSeen = long.MaxValue;
                    var lastSeen = long.MinValue;
                    await flow.ForEachAsync(packet =>
                    {
                        packets++;
                        octets += packet.Packet.TotalPacketLength;
                        firstSeen = Math.Min(firstSeen, packet.Ticks);
                        lastSeen = Math.Max(lastSeen, packet.Ticks);
                    });
                    Console.WriteLine($"  {flow.Key}:firstSeen={firstSeen}, lastSeen={lastSeen}, packets={packets}, octets={octets}");
                });
            });
        }

        [TestMethod]
        public async Task TestTimeWindowOperator()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
            var wins = observable.TimeSpanWindow(t => t.Ticks, TimeSpan.FromSeconds(60)).Select(o => o.GroupBy(f => f.Packet.GetFlowKey()));
            var windowNumber = 0;
            var totalPackets = 0;
            await wins.ForEachAsync(async win =>
            {
                Console.Write($"Window {++windowNumber}:  ");
                await win.ForEachAsync(async flow =>
                {
                    var packets = 0;
                    var octets = 0;
                    var firstSeen = long.MaxValue;
                    var lastSeen = long.MinValue;
                    await flow.ForEachAsync(packet =>
                    {
                        totalPackets++;
                        packets++;
                        octets += packet.Packet.TotalPacketLength;
                        firstSeen = Math.Min(firstSeen, packet.Ticks);
                        lastSeen = Math.Max(lastSeen, packet.Ticks);
                    });
                    Console.Write(".");
                    //Console.WriteLine($"  Flow {flow.Key}: firstSeen={new DateTime(firstSeen)}, duration={new TimeSpan(lastSeen - firstSeen)}, packets={packets}, octets={octets}");
                });
                Console.WriteLine($"Packets = {totalPackets}");
            });
        }

        [TestMethod]
        public async Task TestApplyFlowProcessor()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
            var windows = observable
                .TimeSpanWindow(t => t.Ticks, TimeSpan.FromSeconds(60))
                .Select(packets => packets.GroupFlows(packet => packet.Packet.GetFlowKey()));
            var totalFlows = 0;
            var windowNumber = 0;
            await windows.ForEachAsync(async win =>
            {
                Console.WriteLine($"Window {++windowNumber}:  ");
                await win.ApplyFlowProcessor(FlowProcessorFunc).Do(_ => totalFlows++).ForEachAsync(flowStr =>
                {
                    Console.WriteLine(flowStr);
                });
            });
            Console.WriteLine($"All done, flows = {totalFlows} [{sw.Elapsed}]");
        }
        [TestMethod]
        public async Task TestApplyFlowProcessor2()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
            var windows = observable
                .TimeSpanWindow(t => t.Ticks, TimeSpan.FromSeconds(60))
                .Select(packets => packets.GroupFlowsDictionary(packet => packet.Packet.GetFlowKey()));
            var windowNumber = 0;
            var totalFlows = 0;
            await windows.ForEachAsync(async win =>
            {
                Console.WriteLine($"Window {++windowNumber}:  ");
                await win.ApplyFlowProcessor(FlowProcessorFunc).Do(_ => totalFlows++).ForEachAsync(flowStr =>
                {
                    Console.WriteLine(flowStr);
                });
            });
            Console.WriteLine($"All done, flows = {totalFlows} [{sw.Elapsed}]");
        }
        [TestMethod]
        public async Task TestCreateConversations()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var source = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
            // get windows of flows:
            var windows = source
                            .TimeSpanWindow(packet => packet.Ticks, TimeSpan.FromSeconds(60))
                            .Select(window => window.GroupConversations(packet => packet.Packet.GetFlowKey(), flowKey => GetConversationKey(flowKey)));

            var windowNumber = 0;
            await windows.ForEachAsync(async win =>
            {
                Console.WriteLine($"Window {++windowNumber}:  ");
                await win.ApplyFlowProcessor(ConversationProcessor).ForEachAsync(flowStr =>
                {
                    Console.WriteLine(flowStr);
                });
            });
            Console.WriteLine($"All done [{sw.Elapsed}]");
        }


        [TestMethod]
        public async Task TestTimeWindowFlowProcessorOperator()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacketAndKey);
            var windows = observable.TimeSpanWindow(t => t.Ticks, TimeSpan.FromSeconds(60));
            var windowCount = 0;
            var totalPackets = 0;
            await windows.ForEachAsync(async window =>
            {
                var flowProcessor = new NetFlowProcessor();
                
                // The flow processor is a sink for the observable:
                // Method 1:
                // window.Do(_=>totalPackets++).Subscribe(flowProcessor);
                // await flowProcessor.Completed;
                // Method 2:
                await window.Do(_ => totalPackets++).ForEachAsync(p => flowProcessor.OnNext(p));

                // Get the results:
                Console.WriteLine($"# Window {windowCount}");
                Console.WriteLine($"Flows = {flowProcessor.Count},  Packets = {totalPackets}");
                Console.WriteLine();
                Console.WriteLine("| Date first seen | Duration | Proto | Src IP Addr:Port | Dst IP Addr:Port | Packets | Bytes |");
                Console.WriteLine("| --------------- | -------- | ----- | ---------------- | ---------------- | ------- | ----- |");
                
                foreach (var flow in flowProcessor.Flows)
                {
                    Console.WriteLine($"| {new DateTime(flow.Value.Value.FirstSeen)} |  {new TimeSpan(flow.Value.Value.LastSeen - flow.Value.Value.FirstSeen)} | {flow.Key.ProtocolType} | {flow.Key.SourceIpAddress}:{flow.Key.SourcePort} | {flow.Key.DestinationIpAddress}:{flow.Key.DestinationPort} | {flow.Value.Value.Packets} | {flow.Value.Value.Octets} |");
                }
                Console.WriteLine();
            });
        }
        [TestMethod]
        public async Task TestTimeWindowFlowProcessorAggregate()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacketAndKey);
            var windows = observable.TimeSpanWindow(t => t.Ticks, TimeSpan.FromSeconds(60));
            var windowCount = 0;
            var totalPackets = 0;
            await windows.Do(_ => windowCount++).ForEachAsync(async window =>
            {
                var flowProcessor = new NetFlowProcessor();
                await window.Do(_ => totalPackets++).ForEachAsync(p => flowProcessor.OnNext(p));
                Console.WriteLine($"# Window {windowCount}");
                Console.WriteLine($"Flows = {flowProcessor.Count},  Packets = {totalPackets}");
                Console.WriteLine();
                Console.WriteLine("| Date first seen | Duration | Proto | Src IP Addr | Dst IP Addr | Packets | Bytes |");
                Console.WriteLine("| --------------- | -------- | ----- | ----------- | ----------- | ------- | ----- |");
                foreach (var flow in flowProcessor.AggregateFlows(f=>(f.ProtocolType, f.SourceIpAddress, f.DestinationIpAddress)))
                {
                    Console.WriteLine($"| {new DateTime(flow.Value.Value.FirstSeen)} |  {new TimeSpan(flow.Value.Value.LastSeen - flow.Value.Value.FirstSeen)} | {flow.Key.ProtocolType} | {flow.Key.SourceIpAddress} | {flow.Key.DestinationIpAddress} | {flow.Value.Value.Packets} | {flow.Value.Value.Octets} |");
                }
                Console.WriteLine();
            });
        }


        [TestMethod]
        public async Task TestTimeWindowConversationProcessor()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-32.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacketAndKey);
            var windows = observable.TimeSpanWindow(t => t.Ticks, TimeSpan.FromSeconds(60));
            var windowCount = 0;
            var totalPackets = 0;
            await windows.Do(_ => windowCount++).ForEachAsync(async window =>
            {
                var flowProcessor = new NetFlowProcessor();

                await window.Do(_ => totalPackets++).ForEachAsync(p => flowProcessor.OnNext(p));

                // Get the results:
                Console.WriteLine($"# Window {windowCount}");
                Console.WriteLine($"Flows = {flowProcessor.Count},  Packets = {totalPackets}");
                Console.WriteLine();
                Console.WriteLine("| Date first seen | Duration | Proto | Src IP Addr:Port | Dst IP Addr:Port | Packets | Bytes |");
                Console.WriteLine("| --------------- | -------- | ----- | ---------------- | ---------------- | ------- | ----- |");

                foreach (var flow in flowProcessor.GetConversations(flowProcessor.GetConversationKey))
                {
                    Console.WriteLine($"| {new DateTime(flow.Value.Value.FirstSeen)} |  {new TimeSpan(flow.Value.Value.LastSeen - flow.Value.Value.FirstSeen)} | {flow.Key.FlowKey.ProtocolType} | {flow.Key.FlowKey.SourceIpAddress}:{flow.Key.FlowKey.SourcePort} | {flow.Key.FlowKey.DestinationIpAddress}:{flow.Key.FlowKey.DestinationPort} | {flow.Value.Value.Packets} | {flow.Value.Value.Octets} |");
                }
                Console.WriteLine();
            });
        }

        #region Helper methods

        private FlowKey GetConversationKey(FlowKey key)
        {
            var revKey = key.Reverse();
            var sp1 = key.SourcePort;
            var sp2 = revKey.SourcePort;
            return sp1 > sp2 ? key : revKey;
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


        class NetFlowProcessor : FlowProcessor<(long Ticks, FlowKey Key, Packet Packet), FlowKey, NetFlowProcessor.NetFlowRecord>
        {
            protected override NetFlowRecord Aggregate(NetFlowRecord arg1, NetFlowRecord arg2)
            {
               return  NetFlowRecord.Aggregate(arg1, arg2);
            }

            protected override NetFlowRecord Create((long, FlowKey, Packet) source)
            {
                return NetFlowRecord.Create(source);
            }

            protected override FlowKey GetFlowKey((long, FlowKey, Packet) source)
            {
                return source.Item2;
            }
            public ConversationKey GetConversationKey(FlowKey flowKey)
            {
                if (flowKey.SourcePort > flowKey.DestinationPort)
                {
                    return new ConversationKey(flowKey);
                }
                else
                {
                    return new ConversationKey(flowKey.Reverse());
                }
            }

            protected override void Update(NetFlowRecord record, (long, FlowKey, Packet) source)
            {
                NetFlowRecord.Update(record, source);
            }

            public class NetFlowRecord
            {
                public readonly struct _
                {
                    public readonly long Octets;
                    public readonly int Packets;
                    public readonly long FirstSeen;
                    public readonly long LastSeen;

                    public _(int packets, long octets, long firstSeen, long lastSeen)
                    {
                        Packets = packets;
                        Octets = octets;
                        FirstSeen = firstSeen;
                        LastSeen = lastSeen;
                    }

                    public static _ Aggregate(_ x, _ y)
                    {
                        return new _(x.Packets + x.Packets, x.Octets + y.Octets, Math.Min(x.FirstSeen, y.FirstSeen), Math.Max(x.LastSeen, y.LastSeen)); 
                    }
                }

                public _ Value { get; private set; }

                public static NetFlowRecord Create((long, FlowKey, Packet) packet)
                {
                    return new NetFlowRecord { Value = new _(packet.Item3.TotalPacketLength, 1, packet.Item1, packet.Item1) };
                }

                public static void Update(NetFlowRecord record, (long, FlowKey, Packet) packet)
                {
                    record.Value = _.Aggregate(record.Value, new _(packet.Item3.TotalPacketLength, 1, packet.Item1, packet.Item1));
                }

                public static NetFlowRecord Aggregate(NetFlowRecord arg1, NetFlowRecord arg2)
                {
                    return new NetFlowRecord { Value = _.Aggregate(arg1.Value, arg2.Value) };
                }
            }
        }
        #endregion
    }
}
