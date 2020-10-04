using CsvHelper.Configuration.Attributes;
using IcsMonitor.Commands;
using IcsMonitor.Modbus;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Traffix.Core.Flows;

namespace IcsMonitor
{
    class ModbusDataModel
    {
        public void PrintCentroids(TransformerChain<ClusteringPredictionTransformer<KMeansModelParameters>> model, float[] variances, TextWriter writer)
        {
            VBuffer<float>[] centroids = default;
            model.LastTransformer.Model.GetClusterCentroids(ref centroids, out int k);
            writer.WriteLine($"ClusterId," + string.Join(", ", GetFeatureNames()) + ", Variance");
            for (var i = 0; i < k; i++)
            {
                writer.WriteLine(
                    $"{i+1}, " + string.Join(", ", centroids[i].GetValues().ToArray()) + $",{variances[i].ToString("F6")}");
            }
        }

        public IEnumerable<DataPoint> GetDataPoints(IEnumerable<ConversationRecord<ModbusFlowData>> records)
        {
            var compactRecords = records.Select(ConversationRecord<ModbusFlowData>.TransformTo(x => new ModbusFlowData.Compact(x)));
            foreach (var record in compactRecords)
            {
                yield return new DataPoint { FlowKey = record.Key.ToString(), Features = GetFeatures(record) };
            }
        }

        public float[] GetFeatures(ConversationRecord<ModbusFlowData.Compact> record)
        {
            return new float[]
                {
                    (float)record.ForwardMetrics.Duration.TotalSeconds,
                    (float)record.ReverseMetrics.Duration.TotalSeconds,
                    record.ForwardMetrics.Packets,
                    record.ReverseMetrics.Packets,
                    record.ForwardMetrics.Octets,
                    record.ReverseMetrics.Octets,
                    record.Data.ReadRequests,
                    record.Data.WriteRequests,
                    record.Data.DiagnosticRequests,
                    record.Data.OtherRequests,
                    record.Data.UndefinedRequests,
                    record.Data.MalformedRequests,
                    record.Data.ResponsesSuccess,
                    record.Data.ResponsesError,
                    record.Data.MalformedResponses,
                };
        }
        public string[] GetFeatureNames()
        {
            return new string[]
            {
                    "FwdDuration",
                    "RevDuration",
                    "FwdPackets", 
                    "RevPackets",
                    "FwdOctets", 
                    "RevOctets",
                    
                    "ReadRequests",
                    "WriteRequests",
                    "DiagnosticRequests",
                    "OtherRequests",
                    "UndefinedRequests",
                    "MalformedRequests",
                    "ResponsesSuccess",
                    "ResponsesError",
                    "MalformedResponses"
            };
        }
        /// <summary>
        /// Represents a single data point usable as an input for ML tasks. 
        /// </summary>
        public class DataPoint
        {

            /// <summary>
            /// Vector of features used by ML algorithms.
            /// </summary>
            [VectorType(15)]
            public float[] Features; 

            /// <summary>
            /// The flow key used for referencing the original flow.
            /// </summary>
            public string FlowKey; 
        }
        /// <summary>
        /// The output of the ML evaluation. 
        /// </summary>
        public class Prediction
        {
            /// <summary>
            /// The flow key used for referencing the original flow.
            /// </summary>
            [CsvHelper.Configuration.Attributes.Name("FlowKey")]
            public string FlowKey { get; set; }
            /// <summary>
            /// Contains the ID of the predicted cluster.
            /// </summary>
            [ColumnName("PredictedLabel")]
            [CsvHelper.Configuration.Attributes.Name("ClusterId")]
            public uint PredictedClusterId { get; set; }


            /// <summary>
            /// Gets the distance to the precicted cluster centroid. 
            /// </summary>
            [CsvHelper.Configuration.Attributes.Name("Distance")]
            [CsvHelper.Configuration.Attributes.Format("F6")]
            public float Distance => Distances.Min();
            /// <summary>
            /// Contains an array with squared Euclidean distances to the cluster centroids. The array length is equal to the number of clusters.
            /// </summary>
            [ColumnName("Score")]
            [CsvHelper.Configuration.Attributes.Ignore]
            public float[] Distances { get; set; }
        }
    }
}
