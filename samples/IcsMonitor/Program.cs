using ConsoleAppFramework;
using CsvHelper;
using IcsMonitor.Commands;
using IcsMonitor.Modbus;
using IcsMonitor.S7Comm;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Traffix.Hosting.Console;

namespace IcsMonitor
{
    public partial class Program : TraffixConsoleApp
    {
        public static async Task Main(string[] args)
        {
            await RunApplicationAsync(args).ConfigureAwait(false);
        }

        [Command("Extract-ModbusFlows")]
        public async Task ExtractModbusFlows(
            string inputFile,
            DetailLevel detailLevel = DetailLevel.Compact,
            OutputFormat outFormat = OutputFormat.Yaml,
            string outFile = null)
        {

            using var outWriter = OutputWriter.Create(outFile != null ? new FileInfo(outFile) : null);
            using var cmd = new ExtractModbusFlowsCommand
            {
                InputFile = new FileInfo(inputFile),
            };
            var records = ExecuteCommandAsync(cmd).Cast<ConversationRecord<ModbusFlowData>>();
            switch (detailLevel)
            {
                case DetailLevel.Compact:
                    await outWriter.WriteOutputAsync(outFormat, records.Select(ConversationRecord<ModbusFlowData>.TransformTo(x => new ModbusFlowData.Compact(x))));
                    break;
                case DetailLevel.Extended:
                    await outWriter.WriteOutputAsync(outFormat, records.Select(ConversationRecord<ModbusFlowData>.TransformTo(x => new ModbusFlowData.Extended(x))));
                    break;
                case DetailLevel.Full:
                    await outWriter.WriteOutputAsync(outFormat, records.Select(ConversationRecord<ModbusFlowData>.TransformTo(x => new ModbusFlowData.Complete(x))));
                    break;
            }
        }

        [Command("Extract-Dnp3Flows")]
        public async Task ExtractDnp3Flows(string inputFile, OutputFormat outFormat = OutputFormat.Yaml, string outFile = null)
        {
            using var cmd = new ExtractDnp3FlowsCommand
            {
                InputFile = new FileInfo(inputFile),
            };
            using var outWriter = OutputWriter.Create(outFile != null ? new FileInfo(outFile) : null);
            var records = ExecuteCommandAsync(cmd).Cast<ConversationRecord<Dnp3FlowData>>();
            await outWriter.WriteOutputAsync<Dnp3FlowData>(outFormat, records);
        }

        [Command("Extract-S7Conversations")]
        public async Task ExtractS7CommFlows(string inputFile, OutputFormat outFormat = OutputFormat.Yaml, string outFile = null)
        {
            using var cmd = new ExtractS7CommConversationsCommand
            {
                InputFile = new FileInfo(inputFile),
            };
            using var outWriter = OutputWriter.Create(outFile != null ? new FileInfo(outFile) : null);
            var records = ExecuteCommandAsync(cmd).Cast<ConversationRecord<S7CommConversationData>>();
            await outWriter.WriteOutputAsync<S7CommConversationData>(outFormat, records);
        }



        [Command("Train-ModbusModel")]
        public void TrainModbusModel(
            string inputFile,
            string outputFile,
            int numberOfClusters = 8
            )
        {
            var sw = new Stopwatch();
            var modbusDataModel = new ModbusDataModel(new MLContext(seed: 0));
            sw.Start();
            var inputFileInfo = new FileInfo(inputFile);
            Console.Write($"Processing input {inputFileInfo.Name}, {inputFileInfo.Length} bytes...");
            using var cmd = new ExtractModbusFlowsCommand
            {
                InputFile = inputFileInfo,
            };
            var records = ExecuteCommandAsync(cmd).Cast<ConversationRecord<ModbusFlowData>>();
            var datapoints = modbusDataModel.GetDataPoints(records.ToEnumerable()).ToList();
            Console.WriteLine($"Done, {datapoints.Count} records [{sw.Elapsed}]");
            
            var trainingData = modbusDataModel.MlContext.Data.LoadFromEnumerable(datapoints);

            // Normalize data:
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.normalizationcatalog?view=ml-dotnet
            var normalizeFixZero = modbusDataModel.MlContext.Transforms.NormalizeMinMax("Features", fixZero: true);

            var options = new KMeansTrainer.Options
            {
                NumberOfClusters = numberOfClusters,
                OptimizationTolerance = 1e-6f,
                NumberOfThreads = 1
            };
            Console.Write("Traning predictor...");
            sw.Restart();
            // Define the trainer.
            var pipeline = normalizeFixZero.Append(modbusDataModel.MlContext.Clustering.Trainers.KMeans(options));

            // Train the model.
            var model = pipeline.Fit(trainingData);
            // we also need to compute variance for the data
            // to do so, we c reate a predictor and evaluate all points
            var predictor = modbusDataModel.MlContext.Model.CreatePredictionEngine<ModbusDataModel.DataPoint, ModbusDataModel.Prediction>(model);
            var predictions = datapoints.Select(p => predictor.Predict(p)).ToList();
            var variances = predictions.GroupBy(x => x.ClusterId).Select(p => (Key: p.Key, Variance: ComputeVariance(p))).OrderBy(p => p.Key).Select(p => p.Variance).ToArray();

            Console.WriteLine($"Done. [{sw.Elapsed}]");

            

            modbusDataModel.SaveModel(model, variances, trainingData.Schema, outputFile);
            // Save the model for the future use.
           

        }

        /// <summary>
        /// Variance is computed as v = 1/n \sum_{i=1}^{n}(x_i - x_{mean})^2 .
        /// </summary>
        /// <param name="p">The collection of predictions.</param>
        /// <returns>The variance value.</returns>
        static private float ComputeVariance(IEnumerable<ModbusDataModel.Prediction> p)
        {
            return (float)(p.Sum(s => Math.Pow(s.Distance, 2)) / p.Count());
        }

        [Command("Evaluate-ModbusFlows")]
        public void EvaluateModbusFlows(
            string inputFile,
            string modelFile,
            string outputFile,
            float acceptance = 1.0F
            )
        {
            var modbusDataModel = new ModbusDataModel(new MLContext(seed: 0));

            var model  = modbusDataModel.LoadModel(modelFile, out var inputSchema, out var centroids);

            // Create a predictor.
            var predictor = modbusDataModel.MlContext.Model.CreatePredictionEngine<ModbusDataModel.DataPoint, ModbusDataModel.Prediction>(model);

            // Get flow records:
            using var cmd = new ExtractModbusFlowsCommand
            {
                InputFile = new FileInfo(inputFile),
            };
            var records = ExecuteCommandAsync(cmd).Cast<ConversationRecord<ModbusFlowData>>();
            var datapoints = modbusDataModel.GetDataPoints(records.ToEnumerable());
            using var csv = new CsvWriter(new StreamWriter(new FileInfo(outputFile).Open(FileMode.Create)), CultureInfo.InvariantCulture);

            ModbusDataModel.Prediction Decide(ModbusDataModel.Prediction pred)
            {
                var variance = centroids[pred.ClusterId - 1].Variance;


                pred.Variance = variance;
                pred.Threshold = (float)Math.Sqrt(variance) * acceptance;
                pred.Threshold2 = (float)Math.Sqrt(variance) * acceptance * 2;
                pred.Threshold3 = (float)Math.Sqrt(variance) * acceptance * 3;
                return pred;
            }

            csv.WriteRecords(datapoints.Select(p => Decide(predictor.Predict(p))));
        }
    }
}
