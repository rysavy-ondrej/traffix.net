using Microsoft.Data.Analysis;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Traffix.Processors
{
    public static class DataFrameExtensions
    {
        /// <summary>
        /// Writes a DataFrame into a CSV.
        /// </summary>
        /// <param name="dataFrame"><see cref="DataFrame"/></param>
        /// <param name="path">CSV file path</param>
        /// <param name="separator">column separator</param>
        /// <param name="header">has a header or not</param>
        /// <param name="encoding">The character encoding. Defaults to UTF8 if not specified</param>
        /// <param name="cultureInfo">culture info for formatting values</param>
        public static void WriteCsv(this DataFrame dataFrame, string path,
                                   char separator = ',', bool header = true,
                                   Encoding encoding = null, CultureInfo cultureInfo = null)
        {
            using (FileStream csvStream = new FileStream(path, FileMode.Create))
            {
                WriteCsv(dataFrame: dataFrame, csvStream: csvStream,
                           separator: separator, header: header,
                           encoding: encoding, cultureInfo: cultureInfo);
            }
        }

        /// <summary>
        /// Writes a DataFrame into a CSV.
        /// </summary>
        /// <param name="dataFrame"><see cref="DataFrame"/></param>
        /// <param name="csvStream">stream of CSV data to be write out</param>
        /// <param name="separator">column separator</param>
        /// <param name="header">has a header or not</param>
        /// <param name="encoding">the character encoding. Defaults to UTF8 if not specified</param>
        /// <param name="cultureInfo">culture info for formatting values</param>
        public static void WriteCsv(this DataFrame dataFrame, Stream csvStream,
                           char separator = ',', bool header = true,
                           Encoding encoding = null, CultureInfo cultureInfo = null)
        {
            if (cultureInfo is null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            if (cultureInfo.NumberFormat.NumberDecimalSeparator.Equals(separator.ToString()))
            {
                throw new ArgumentException("Decimal separator cannot match the column separator");
            }

            if (encoding is null)
            {
                encoding = Encoding.ASCII;
            }

            using (StreamWriter csvFile = new StreamWriter(csvStream, encoding, 4096, leaveOpen: true))
            {
                if (dataFrame != null)
                {
                    var columnNames = dataFrame.Columns.Select(c => c.Name).ToList();

                    if (header)
                    {
                        var headerColumns = string.Join(separator.ToString(), columnNames);
                        csvFile.WriteLine(headerColumns);
                    }

                    var record = new StringBuilder();

                    foreach (var row in dataFrame.Rows)
                    {
                        bool firstRow = true;
                        foreach (var cell in row)
                        {
                            if (!firstRow)
                            {
                                record.Append(separator);
                            }
                            else
                            {
                                firstRow = false;
                            }

                            Type t = cell?.GetType();

                            if (t == typeof(bool))
                            {
                                record.AppendFormat(cultureInfo, "{0}", cell);
                                continue;
                            }

                            if (t == typeof(float))
                            {
                                record.AppendFormat(cultureInfo, "{0:G9}", cell);
                                continue;
                            }

                            if (t == typeof(double))
                            {
                                record.AppendFormat(cultureInfo, "{0:G17}", cell);
                                continue;
                            }

                            if (t == typeof(decimal))
                            {
                                record.AppendFormat(cultureInfo, "{0:G31}", cell);
                                continue;
                            }

                            record.Append(cell);
                        }

                        csvFile.WriteLine(record);

                        record.Clear();
                    }
                }
            }
        }
    }
}