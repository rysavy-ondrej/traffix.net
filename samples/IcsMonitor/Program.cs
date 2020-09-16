using ConsoleAppFramework;
using IcsMonitor.Commands;
using IcsMonitor.Modbus;
using Microsoft.PowerShell.Commands;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Traffix.Hosting.Console;

namespace IcsMonitor
{
    public enum DetailLevel { Full, Extended, Compact }
    public enum OutputFormat { Yaml, Csv }
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
            switch(detailLevel)
            {
                case DetailLevel.Compact:
                    await outWriter.WriteOutputAsync(outFormat, records.Select(ConversationRecord<ModbusFlowData>.TransformTo(x=>new ModbusFlowData.Compact(x))));
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
            var records = ExecuteCommandAsync(cmd).Cast <ConversationRecord<Dnp3FlowData>>();
            await outWriter.WriteOutputAsync<Dnp3FlowData>(outFormat, records);
        }
    }
}
