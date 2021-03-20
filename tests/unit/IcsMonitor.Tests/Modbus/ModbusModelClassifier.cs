using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IcsMonitor.Tests
{
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
    
}
