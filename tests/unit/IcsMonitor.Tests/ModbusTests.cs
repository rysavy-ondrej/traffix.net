using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Traffix.Core.Observable;
using Traffix.Providers.PcapFile;
using Traffix.DataView;
using Microsoft.ML;

namespace IcsMonitor.Tests
{
    [TestClass]
    public class ModbusTests
    {
        const int ModbusPort = 502;
        [TestMethod]
        public async Task TestPrepareModbusData()
        {
            // load from file
            var observable = SharpPcapReader.CreateObservable(@"data\PCAP\run1_3rtu_2s.cap").Select(PacketRecord.FromFrame).Where(p=>p.Key.SourcePort == ModbusPort || p.Key.DestinationPort == ModbusPort);
            var windows = observable.TimeSpanWindow(t => t.Ticks, TimeSpan.FromMinutes(30));
            
            // apply modbus flow processor
            await windows.Select((packets, index) => (packets,index)).ForEachAsync(async window =>
            {
                var totalPackets = 0;
                var flowProcessor = new ModbusFlowProcessor();
                await window.packets.Do(_=>totalPackets++).ForEachAsync(p => flowProcessor.OnNext(p));
                Console.WriteLine($"# Window {window.index}");
                Console.WriteLine();
                Console.WriteLine($"Flows = {flowProcessor.Count},  Packets = {totalPackets}");
                Console.WriteLine();
                Console.WriteLine("| Date first seen | Duration | Proto | Src IP Addr:Port | Dst IP Addr:Port | Packets | Bytes | UnitId | ReadRequests | WriteRequests | DiagnosticRequests | OtherRequests | UndefinedRequests | ResponsesSuccess | ResponsesError | MalformedRequests | MalformedResponses |");
                Console.WriteLine("| --------------- | -------- | ----- | ---------------- | ---------------- | ------- | ----- | ------ | ------------ | ------------- | ------------------ | ------------- | ----------------- | ---------------- | -------------- | ----------------- | ------------------ |");
                var aggregatedFlows = flowProcessor.AggregateFlows(f => (f.ProtocolType, f.SourceIpAddress, f.DestinationIpAddress));
                foreach (var flow in aggregatedFlows )
                {
                    Console.WriteLine($"| {flow.Value.Metrics.Start} |  {flow.Value.Metrics.Duration} | {flow.Key.ProtocolType} | {flow.Key.SourceIpAddress} | {flow.Key.DestinationIpAddress} | {flow.Value.Metrics.Packets} | {flow.Value.Metrics.Octets} | {flow.Value.Compact.UnitId} | {flow.Value.Compact.ReadRequests} | {flow.Value.Compact.WriteRequests} | {flow.Value.Compact.DiagnosticRequests} | {flow.Value.Compact.OtherRequests} | {flow.Value.Compact.UndefinedRequests} | {flow.Value.Compact.ResponsesSuccess} | {flow.Value.Compact.ResponsesError} | {flow.Value.Compact.MalformedRequests} | {flow.Value.Compact.MalformedResponses} |");
                }
                Console.WriteLine();

                var dataview = aggregatedFlows.Select(x => x.Value).AsDataView(new DataViewTypeResolver(new ModbusFlowRecordDataViewType()));
                Preview(dataview);
            });
            // export ml.net dataframes

        }

        private void Preview(Microsoft.ML.IDataView dataview)
        {
            foreach (var row in dataview.Preview(100).RowView)
            {
                foreach (var val in row.Values)
                {
                    Console.Write($"{val}\t");
                }
                Console.WriteLine();
            }
        }
    }
}
