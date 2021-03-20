using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace IcsMonitor.Tests
{
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
