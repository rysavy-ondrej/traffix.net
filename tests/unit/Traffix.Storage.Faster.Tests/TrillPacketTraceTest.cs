using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Traffix.Providers.PcapFile;
using Microsoft.StreamProcessing;

namespace Traffix.Storage.Faster.Tests
{
    public class TrillPacketTraceTest
    {

        [TestMethod]
        public async Task TestBasicTrill()
        {
            var pcapPath = Path.Combine(TestEnvironment.DataPath, "testbed-16.pcap");
            var sw = new Stopwatch();
            sw.Start();
            var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
            var streamable = observable.ToTemporalStreamable(f => f.Ticks);
        }
    }
}
