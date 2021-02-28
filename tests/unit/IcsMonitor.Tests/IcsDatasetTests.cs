using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PacketDotNet;
using System;
using System.IO;
using System.Linq;
using Traffix.Core;
using Traffix.Data;
using Traffix.Interactive;
using Traffix.Processors;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Data;

namespace IcsMonitor.Tests
{
    [TestClass]
    public class IcsDatasetTests
    {
        public void PrepareDataForOCKMeans(int numberOfClusters, string sourceFile, DateTime start, TimeSpan duration)
        {
            // PART 1: Data ingestion
            using var conversationTable = FasterConversationTable.Create($"{sourceFile}.db", 3000000);
            using var loader = conversationTable.GetStreamer();
            using var pcapReader = new SharpPcapReader(sourceFile);
            while (pcapReader.GetNextFrame(out var rawFrame)) { loader.AddFrame(rawFrame); }
            loader.Close();

            // PART 2: Data preparation
            var processor = new Modbus.ModbusBiflowProcessor();
            var windows = conversationTable.Conversations.GroupByWindow(start, duration);
            var records = windows.SelectMany(conversation => 
                conversationTable.ProcessConversations(conversation, new Modbus.ModbusBiflowProcessor().ApplyToWindow(start, duration)));
            
            // PART 3: Model Training
            var trainingData = records.AsDataView();       
            var mlContext = new MLContext();
            var normalize = mlContext.Transforms.NormalizeMinMax("Features");
            var options = new KMeansTrainer.Options { NumberOfClusters = numberOfClusters };
            var pipeline = normalize.Append(mlContext.Clustering.Trainers.KMeans(options));
            var model = pipeline.Fit(trainingData);
        }

        public class PacketCountData
        {
            public long Timestamp;

            public double Value;
        }
        public class PacketCountPrediction
        {
            [VectorType(7)]
            public double[] Prediction { get; set; }
        }

        public void PrepareDataForTimeSeriesAD(FasterConversationTable table, TimeSpan timeInterval)
        {
            // PART 1: DATA PREPARATION
            var frames = table.ProcessFrames(table.FrameKeys, new TimedFrames());
            var intervals = frames.GroupBy(x => (int)(x.Ticks / timeInterval.Ticks));
            var framesValues = intervals.Select(x => new PacketCountData { Timestamp = x.Key * timeInterval.Ticks, Value = x.Count() });

            // PART 2: TIME SERIES AD
            var mlContext = new MLContext();
            var dataview = mlContext.Data.LoadFromEnumerable(framesValues);
            int period = mlContext.AnomalyDetection.DetectSeasonality(dataview, "Value");
            var options = new SrCnnEntireAnomalyDetectorOptions() { Threshold = 0.3, Sensitivity = 80.0,
                DetectMode = SrCnnDetectMode.AnomalyAndMargin, Period = period };

            //PART 3: Invoke SrCnn algorithm to detect anomaly
            var outputDataView = mlContext.AnomalyDetection.DetectEntireAnomalyBySrCnn(dataview, "Prediction", "Value", options);
            var predictions = mlContext.Data.CreateEnumerable<PacketCountPrediction>(outputDataView, reuseRowObject: false);
        }

        class TimedFrames : IFrameProcessor<(long Ticks, Packet Packet)>
        {
            public (long Ticks, Packet Packet) Invoke(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
            {
                return (frameMetadata.Ticks, Packet.ParsePacket((LinkLayers)frameMetadata.LinkLayer, frameBytes.ToArray()));
            }
        }
    }
}
