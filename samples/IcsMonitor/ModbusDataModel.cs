using AutoMapper;
using CsvHelper;
using IcsMonitor.Modbus;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Traffix.Processors;

namespace IcsMonitor
{
    class ModbusDataModel
    {
        private readonly Mapper _mapper;

        public MLContext MlContext { get; }

        public ModbusDataModel(MLContext mlContext)
        {
            MlContext = mlContext;
            var configuration = new MapperConfiguration(cfg => { });
            _mapper = new Mapper(configuration);
        }

        public IEnumerable<Centroids> GetCentroids(TransformerChain<ClusteringPredictionTransformer<KMeansModelParameters>> model, float[] variances)
        {
            var kmeansModel = model.LastTransformer.Model;
            VBuffer<float>[] centroids = default;
            kmeansModel.GetClusterCentroids(ref centroids, out int k);
            for (var i = 0; i < k; i++)
            {
                var featureNames = GetFeatureNames();
                var record = new Dictionary<string, object>
                {
                    [nameof(Centroids.ClusterId)] = i + 1,
                    [nameof(Centroids.Variance)] = variances[i]
                };
                var vals = centroids[i].GetValues().ToArray();
                for(var j = 0; j < vals.Length; j++)
                {
                    record[featureNames[j]] = vals[j];
                }
                var result = _mapper.Map<Centroids>(record);
                yield return result;
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
            public uint ClusterId { get; set; }


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

            /// <summary>
            /// The computed variance for the cluster.
            /// </summary>
            [ColumnName("Variance")]
            public float Variance { get; set; }

            /// <summary>
            /// The treshold value, which is SQRT(variance) * T
            /// </summary>
            [ColumnName("Threshold")]

            public float Threshold { get; set; }
            /// <summary>
            /// The double treshold value, which is 2 * T * SQRT(variance)
            /// </summary>
            [ColumnName("Threshold2")]
            public float Threshold2 { get; set; }
            /// <summary>
            /// The triple treshold value, which is 3 * T * SQRT(variance)
            /// </summary>
            [ColumnName("Threshold3")]
            public float Threshold3 { get; set; }


        }

        internal void SaveModel(TransformerChain<ClusteringPredictionTransformer<KMeansModelParameters>> model, float[] variances, DataViewSchema schema, string outputFile)
        {
            // create model file - it is a zip file.
            MlContext.Model.Save(model, schema, outputFile);
            using (var modelArchive = ZipFile.Open(outputFile, ZipArchiveMode.Update))
            {
                var entry = modelArchive.CreateEntry("ClusterCentroids.csv");

                using (var writer = new StreamWriter(entry.Open()))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(GetCentroids(model, variances));

                }
            }
            
        }

        public ITransformer LoadModel(string modelFile, out DataViewSchema inputSchema, out Centroids[] centroids)
        {
            // Load previously trained model.
            var model = MlContext.Model.Load(modelFile, out inputSchema);

            using (var modelArchive = ZipFile.Open(modelFile, ZipArchiveMode.Read))
            {
                var entry = modelArchive.GetEntry("ClusterCentroids.csv");

                using (var reader = new StreamReader(entry.Open()))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                         centroids = csv.GetRecords<Centroids>().ToArray();
                    }
                }
            }


            return model;
        }


        public class Centroids
        {
            public int ClusterId { get; set; }
            public float FwdDuration { get; set; }
            public float RevDuration { get; set; }
            public float FwdPackets { get; set; }
            public float RevPackets { get; set; }
            public float FwdOctets { get; set; }
            public float RevOctets { get; set; }
            public float ReadRequests { get; set; }
            public float WriteRequests { get; set; }
            public float DiagnosticRequests { get; set; }
            public float OtherRequests { get; set; }
            public float UndefinedRequests { get; set; }
            public float MalformedRequests { get; set; }
            public float ResponsesSuccess { get; set; }
            public float ResponsesError { get; set; }
            public float MalformedResponses { get; set; }
            public float Variance { get; set; }
        }
    }
}
