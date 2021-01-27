using Microsoft.VisualStudio.TestTools.UnitTesting;
using IcsMonitor;
using System;
using Microsoft.ML;
using System.Linq;
using Microsoft.Data.Analysis;

namespace IcsMonitor.Tests
{
    [TestClass]
    public class IcsDatasetTests
    {
        [TestMethod]
        public void CreateDatasetTest()
        {
            var ctx = new Interactive();
            var dataset = ctx.ComputeModbusDataset(@"C:\Users\user\Captures\sorting_station_ver2.pcap", TimeSpan.FromSeconds(120));
            var stat = dataset.Statistics;
            Console.WriteLine("--STATS:");
            Console.WriteLine($"  Frames={stat.FramesCount}");
            Console.WriteLine($"  Tables={stat.TablesCount}");
        }
        [TestMethod]
        public void GetDataFrameTest()
        {
            var ml = new MLContext();
            var ctx = new Interactive();
            var dataset = ctx.ComputeModbusDataset(@"C:\Users\user\Captures\sorting_station_ver2.pcap", TimeSpan.FromSeconds(180));
            var records = dataset.ConversationTables.SelectMany(x => x.AsEnumerable());
            var dataframe =records.ToDataFrame();
            var schema = dataframe.Schema;
            Console.WriteLine($"Schema={schema}");
            Console.WriteLine($"DataView rows = {dataframe.Rows.Count()}");
            Console.WriteLine($"{dataframe.ToString()}");
        }
        [TestMethod]
        public void SaveDatasetTest()
        {
            var ml = new MLContext();
            var ctx = new Interactive();
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
    }
}
