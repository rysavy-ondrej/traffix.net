using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Traffix.Core.Flows;
using Traffix.Core.Observable;
using Traffix.Data;
using Traffix.Providers.PcapFile;
using Microsoft.StreamProcessing;
using Traffix.Processors;

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

            var windowNumber = 0;
            await windows.ForEachAsync(async win =>
            {
                Console.WriteLine($"Window {++windowNumber}:  ");
                await win.ApplyFlowProcessor(FlowProcessor).ForEachAsync(flowStr =>
                {
                    Console.WriteLine(flowStr);
                });
            });
            Console.WriteLine($"All done [{sw.Elapsed}]");
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
        #endregion
    }
}
