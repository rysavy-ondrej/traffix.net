using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcsMonitor.Tests
{
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
    
}
