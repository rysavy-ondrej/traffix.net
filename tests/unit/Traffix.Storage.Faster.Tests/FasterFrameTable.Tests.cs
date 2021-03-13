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
using Traffix.Core;
using Traffix.Core.Flows;
using Traffix.Data;
using Traffix.Providers.PcapFile;
using Microsoft.StreamProcessing;
namespace Traffix.Storage.Faster.Tests
{
    [TestClass]
    public class FasterFrameTableTests
    {
        string dataFolderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\data"));
        /// <summary>
        /// Check if we have test data in the expected location.
        /// If this test fail, please execute fetch-testdata.cmd or fetch-testdata.sh 
        /// in the test project testdata folder.
        /// </summary>
        [TestMethod]
        public void CheckDataAvailable()
        {
            Assert.IsTrue(File.Exists(Path.Combine(dataFolderPath, "testbed-64.pcap")));
        }

        /// <summary>
        /// Loads the table from the given pcap file.
        /// </summary>
        [TestMethod]
        public async Task CreateTable()
        {
            var pcapPath = Path.Combine(dataFolderPath, "testbed-16.pcap");
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
            var observable = flowTable.GetObservable(frameProcessor);
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
        public async Task ObservableFromFile()
        {
            var pcapPath = Path.Combine(dataFolderPath, "testbed-16.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(GetPacket);
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
        public async Task TrillTest()
        {
            var pcapPath = Path.Combine(dataFolderPath, "testbed-16.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(GetPacket);
            var streamable = observable.ToTemporalStreamable(f => f.Ticks);
            var query = streamable.Select(p => new { Key = p.Packet.GetFlowKey() }).GroupApply(f => f.Key, g => g.SessionTimeoutWindow(TimeSpan.FromMinutes(5).Ticks).Select(s=>s.Key));
            await query.ToTemporalObservable((s,e,c)=> c).ForEachAsync(f => Console.WriteLine($"count = {f}"));
        }


        private (long Ticks, Packet Packet) GetPacket(RawFrame arg)
        {
            return (arg.Ticks, Packet.ParsePacket(arg.LinkLayer, arg.Data));
        }

        private Packet frameProcessor(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
        {
            return Packet.ParsePacket((LinkLayers)frameMetadata.LinkLayer, frameBytes.ToArray());
        }
    }
        
}
