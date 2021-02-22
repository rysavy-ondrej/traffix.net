using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PacketDotNet;
using System;
using System.IO;
using System.Linq;
using Traffix.Interactive;
using Traffix.Processors;
using Traffix.Providers.PcapFile;

namespace IcsMonitor.Tests
{
    [TestClass]
    public class IcsDatasetTests
    {
        public void PrepareData(int numberOfClusters, string sourceFile, DateTime start, TimeSpan duration)
        {
            // PART 1: Data preparation
            using var conversationTable = Traffix.Storage.Faster.FasterConversationTable.Create($"{sourceFile}.db", 3000000);
            using (var loader = conversationTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(sourceFile))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame);
                }
                loader.Close();
            }
            var records = conversationTable.ProcessConversations(conversationTable.ConversationKeys, 
                new Modbus.ModbusBiflowProcessor().ApplyToWindow(start, duration));
            
            // PART 2: Model Training
            var trainingData = records.AsDataView();
        
            var mlContext = new MLContext();
            var normalizeFixZero = mlContext.Transforms.NormalizeMinMax("Features");
            var options = new KMeansTrainer.Options { NumberOfClusters = numberOfClusters };
            var pipeline = normalizeFixZero.Append(mlContext.Clustering.Trainers.KMeans(options));
            var model = pipeline.Fit(trainingData);
        }
    }
}
