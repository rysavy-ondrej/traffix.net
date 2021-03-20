using AutoMapper;
using CsvHelper;
using IcsMonitor.Modbus;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Traffix.Processors;

namespace IcsMonitor.Tests
{
    public class ModbusDataModel
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

        public IEnumerable<DataPoint> GetDataPoints(IEnumerable<ModbusFlowRecord> records)
        {
            foreach (var record in records)
            {
                yield return new DataPoint { FlowKey = record.FlowKey.ToString(), Features = GetFeatures(record) };
            }
        }

        public float[] GetFeatures(ModbusFlowRecord record)
        {
            return new float[]
                {
                    (float)record.ForwardMetrics.Duration.TotalSeconds,
                    (float)record.ReverseMetrics.Duration.TotalSeconds,
                    record.ForwardMetrics.Packets,
                    record.ReverseMetrics.Packets,
                    record.ForwardMetrics.Octets,
                    record.ReverseMetrics.Octets,
                    record.Compact.ReadRequests,
                    record.Compact.WriteRequests,
                    record.Compact.DiagnosticRequests,
                    record.Compact.OtherRequests,
                    record.Compact.UndefinedRequests,
                    record.Compact.MalformedRequests,
                    record.Compact.ResponsesSuccess,
                    record.Compact.ResponsesError,
                    record.Compact.MalformedResponses,
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
            /// Gets the distance to the predicted cluster centroid. 
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
    /// <summary>
    /// Represents a MODBUS model trainer.
    /// <para/>
    /// It can be used to train the model for <see cref="ModbusModelClassifier"/>.
    /// </summary>
    public class ModbusModelTrainer
    {
        private readonly int _numberOfClusters;
        private ModbusDataModel _modbusDataModel;
        private TransformerChain<ClusteringPredictionTransformer<KMeansModelParameters>> _model;
        private float[] _variance;
        private DataViewSchema _schema;

        /// <summary>
        /// Creates a new trainer with the given options.
        /// </summary>
        /// <param name="numberOfClusters"></param>
        public ModbusModelTrainer(int numberOfClusters = 8)
        {
            _numberOfClusters = numberOfClusters;
        }

        /// <summary>
        /// Creates a new model by training it on the given collection of input data.
        /// </summary>
        /// <param name="modbusFlows"></param>
        public void Train(IEnumerable<ModbusFlowRecord> modbusFlows)
        {
            _modbusDataModel = new ModbusDataModel(new MLContext(seed: 0));
            IEnumerable<ModbusDataModel.DataPoint> datapoints = _modbusDataModel.GetDataPoints(modbusFlows).ToList();
            var trainingData = _modbusDataModel.MlContext.Data.LoadFromEnumerable(datapoints);

            // Normalize data:
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.normalizationcatalog?view=ml-dotnet
            var normalizeFixZero = _modbusDataModel.MlContext.Transforms.NormalizeMinMax("Features", fixZero: true);

            var options = new KMeansTrainer.Options
            {
                NumberOfClusters = _numberOfClusters,
                OptimizationTolerance = 1e-6f,
                NumberOfThreads = 1
            };
            // Define the trainer.
            var pipeline = normalizeFixZero.Append(_modbusDataModel.MlContext.Clustering.Trainers.KMeans(options));

            // Train the model.
            _model = pipeline.Fit(trainingData);
            // we also need to compute variance for the data
            // to do so, we c reate a predictor and evaluate all points
            var predictor = _modbusDataModel.MlContext.Model.CreatePredictionEngine<ModbusDataModel.DataPoint, ModbusDataModel.Prediction>(_model);
            var predictions = datapoints.Select(p => predictor.Predict(p)).ToList();
            _variance = predictions.GroupBy(x => x.ClusterId).Select(p => (Key: p.Key, Variance: ComputeVariance(p))).OrderBy(p => p.Key).Select(p => p.Variance).ToArray();
            _schema = trainingData.Schema;


            /// <summary>
            /// Variance is computed as v = 1/n \sum_{i=1}^{n}(x_i - x_{mean})^2 .
            /// </summary>
            /// <param name="p">The collection of predictions.</param>
            /// <returns>The variance value.</returns>
            static float ComputeVariance(IEnumerable<ModbusDataModel.Prediction> p)
            {
                return (float)(p.Sum(s => Math.Pow(s.Distance, 2)) / p.Count());
            }
        }

        /// <summary>
        /// Saves the model to the file. 
        /// <para/> 
        /// The model file can be loaded to the classifier using <see cref="ModbusModelClassifier.Load(string)"/> method.
        /// </summary>
        /// <param name="modelPath"></param>
        public void Save(string modelPath)
        {
            if (_modbusDataModel == null) throw new InvalidOperationException("Cannot save model: Model has not been trained yet.");
            _modbusDataModel.SaveModel(_model, _variance, _schema, modelPath);
        }

        /// <summary>
        /// Gets the classifier based on the current model.
        /// </summary>
        /// <returns></returns>
        public ModbusModelClassifier GetClassifier()
        {
            if (_modbusDataModel == null) throw new InvalidOperationException("Cannot get classifier: Model has not been trained yet.");
            return new ModbusModelClassifier(_modbusDataModel, _model, _schema, _modbusDataModel.GetCentroids(_model, _variance));
        }
    }
    /// <summary>
    /// The MODBUS flow classifier that uses trained model.
    /// <para/>
    /// The model can be created by <see cref="ModbusModelTrainer"/>.
    /// </summary>
    public class ModbusModelClassifier
    {
        private readonly ModbusDataModel _modbusDataModel;
        private readonly ITransformer _transformer;
        private readonly DataViewSchema _dataViewSchema;
        private readonly ModbusDataModel.Centroids[] _centroids;

        public static ModbusModelClassifier Load(string modelFile)
        {
            var modbusDataModel = new ModbusDataModel(new MLContext(seed: 0));
            var model = modbusDataModel.LoadModel(modelFile, out var inputSchema, out var centroids);
            return new ModbusModelClassifier(modbusDataModel, model, inputSchema, centroids);
        }

        internal ModbusModelClassifier(ModbusDataModel modbusDataModel, ITransformer transformer, DataViewSchema schema, IEnumerable<ModbusDataModel.Centroids> centroids)
        {
            _modbusDataModel = modbusDataModel;
            _transformer = transformer;
            _dataViewSchema = schema;
            _centroids = centroids.ToArray();
        }

        /// <summary>
        /// Evaluates a collection of input MODBUS conversations. For each conversation the prediction is computed. 
        /// </summary>
        /// <param name="modbusFlows">The input collection of MODBUS conversations.</param>
        /// <param name="acceptance">The acceptance multipler used to decide on each conversation.</param>
        /// <returns>A collection of predictions.</returns>
        public IEnumerable<ModbusDataModel.Prediction> Evaluate(
            IEnumerable<ModbusFlowRecord> modbusFlows,
            float acceptance = 1.0F
            )
        {
            var predictor = _modbusDataModel.MlContext.Model.CreatePredictionEngine<ModbusDataModel.DataPoint, ModbusDataModel.Prediction>(_transformer);
            var datapoints = _modbusDataModel.GetDataPoints(modbusFlows);

            // Extends the prediction with other information.
            ModbusDataModel.Prediction Decide(ModbusDataModel.Prediction pred)
            {
                var variance = _centroids[pred.ClusterId - 1].Variance;
                pred.Variance = variance;
                pred.Threshold = (float)Math.Sqrt(variance) * acceptance;
                pred.Threshold2 = (float)Math.Sqrt(variance) * acceptance * 2;
                pred.Threshold3 = (float)Math.Sqrt(variance) * acceptance * 3;
                return pred;
            }

            var predictions = datapoints.Select(p => Decide(predictor.Predict(p)));
            return predictions;
        }
    }
    public static class ModbusDataModelPredictions
    {
        /// <summary>
        /// Saves the collection of predictions to CSV file.
        /// </summary>
        /// <param name="predictions">The collection of predictions.</param>
        /// <param name="csvFile">The outpu CSV file.</param>
        public static void SaveAsCsv(this IEnumerable<ModbusDataModel.Prediction> predictions, string csvFile)
        {
            using var csv = new CsvWriter(new StreamWriter(new FileInfo(csvFile).Open(FileMode.Create)), CultureInfo.InvariantCulture);
            csv.WriteRecords(predictions);
        }
    }
    
}
