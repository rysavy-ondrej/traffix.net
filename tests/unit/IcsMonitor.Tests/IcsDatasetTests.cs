using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PacketDotNet;
using System;
using System.IO;
using System.Linq;
using Traffix.Interactive;
using Traffix.Processors;

namespace IcsMonitor.Tests
{
    [TestClass]
    public class IcsDatasetTests
    {



        [TestMethod]
        public void CreateDatasetTest()
        {
            var ctx = new Interactive();
            var dataset = ctx.Datasets.Prepare(@"C:\Users\user\Captures\sorting_station_ver2.pcap", TimeSpan.FromSeconds(120));
            var stat = dataset.Statistics;
            Console.WriteLine("--STATS:");
            Console.WriteLine($"  Frames={stat.FramesCount}");
            Console.WriteLine($"  Tables={stat.TablesCount}");
        }
        [TestMethod]
        public void GetDataFrameTest()
        {
            var ml = new MLContext();
            var ctx = new Monitor();
            var dataset = ctx.ComputeModbusDataset(@"C:\Users\user\Captures\sorting_station_ver2.pcap", TimeSpan.FromSeconds(180));
            var records = dataset.ConversationTables.SelectMany(x => x.AsEnumerable());
            var dataframe = records.ToDataFrame();
            var schema = (dataframe as IDataView).Schema;
            Console.WriteLine($"Schema={schema}");
            Console.WriteLine($"DataView rows = {dataframe.Rows.Count()}");
            Console.WriteLine($"{dataframe.ToString()}");
        }
        [TestMethod]
        public void SaveDatasetTest()
        {
            var ml = new MLContext();
            var ctx = new Monitor();
            var dataset = ctx.ComputeModbusDataset(@"C:\Users\user\Captures\sorting_station_ver2.pcap", TimeSpan.FromSeconds(180));
            var records = dataset.ConversationTables.SelectMany(x => x.AsEnumerable());
            var dataframe = records.ToDataFrame();
            dataframe.WriteCsv(@"C:\Users\user\Captures\sorting_station_ver2.csv");
        }
        [TestMethod]
        public void LoadDatasetTest()
        {
            var dataframe = DataFrame.LoadCsv(@"C:\Users\user\Captures\sorting_station_ver2.csv");
            Console.WriteLine($"DataView rows = {dataframe.Rows.Count()}");
            Console.WriteLine($"{dataframe.ToString()}");
        }

        [TestMethod]
        public void ConversationsDataViewTest()
        {
            var pcapPath = Path.GetFullPath(@"data\PCAP\modbus.pcap");
            var ml = new MLContext();
            var ctx = new Monitor();
            var dataset = ctx.ComputeModbusDataset(pcapPath, TimeSpan.FromDays(1000));
            var records = dataset.ConversationTables.SelectMany(x => x.AsEnumerable());
            var dataview = records.AsDataView(); 
            foreach (var col in dataview.Schema)
            {
                Console.WriteLine($"{col.Index} {col.Name} : {col.Type}");
            }
            // Use preview during debugging...
            var preview = dataview.Preview(10);
            // and we can also print it to console
            ml.Data.SaveAsMd(preview, Console.Out);
            // or write to md file
            using var writer = File.CreateText(@"data\PCAP\modbus.md");
            ml.Data.SaveAsMd(preview, writer);
            // save to TSV file
            using var stream = File.Create(@"data\PCAP\modbus.csv");
            ml.Data.SaveAsText(dataview, stream);
        }
        [TestMethod]
        public void FrameRecordsDataViewTest()
        {
            var pcapPath = Path.GetFullPath(@"data\PCAP\modbus.pcap");
            var ml = new MLContext();
            var ctx = new Monitor();
            var dataset = ctx.ComputeModbusDataset(pcapPath, TimeSpan.FromDays(1000));
            var records = dataset.Frames.Select(x => new FrameRecord<IPPacket> { Timestamp = new DateTime(x.Ticks), FrameLength = x.OriginalLength, Data = Packet.ParsePacket(x.LinkLayer, x.Data).Extract<IPPacket>() });
            var dataview = records.AsDataView();
            var preview = dataview.Preview(10);
            ml.Data.SaveAsMd(preview, Console.Out);
        }

    }
}
