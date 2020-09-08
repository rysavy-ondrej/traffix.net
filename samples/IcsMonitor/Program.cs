using ConsoleAppFramework;
using IcsMonitor.Commands;
using IcsMonitor.Modbus;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Traffix.Hosting.Console;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace IcsMonitor
{
    public enum ModbusRecordFormat { Raw, Extended, Compact }
    public partial class Program : TraffixConsoleApp
    {
        public static async Task Main(string[] args)
        {
            await RunApplicationAsync(args).ConfigureAwait(false);    
        }



        [Command("Extract-ModbusFlows")]
        public async Task ExtractModbusFlows(string inputFile, ModbusRecordFormat format = ModbusRecordFormat.Raw, string outFile = null)
        {
            using var cmd = new ExtractModbusFlowsCommand
            {
                InputFile = inputFile,
                Aggregator = GetModbusAggregator(format)
            };

            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredUpperCaseNamingConvention.Instance)
                .DisableAliases()
                .WithTypeConverter(new IPAddressYamlTypeConverter())
                .Build();

            using var outWriter = outFile != null ? new StreamWriter(outFile) : new StreamWriter(Console.OpenStandardOutput(), leaveOpen: true);
            await foreach (var obj in ExecuteCommandAsync(cmd).ConfigureAwait(false))
            {
                outWriter.WriteLine("---");
                serializer.Serialize(outWriter, obj);
            }
        }

        private IModbusAggregator GetModbusAggregator(ModbusRecordFormat aggregate)
        {
            switch(aggregate)
            {
                case ModbusRecordFormat.Extended:
                    return new ModbusExtendedAggregator();
                case ModbusRecordFormat.Compact:
                    return new ModbusCompactAggregator();
                default:
                    return null;
            }
        }
        public class IPAddressYamlTypeConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return type == typeof(IPAddress);
            }

            public object ReadYaml(IParser parser, Type type)
            {
                var scalar = (YamlDotNet.Core.Events.Scalar)parser.Current;
                var ipString = (scalar.Value);
                parser.MoveNext();
                return IPAddress.Parse(ipString);
            }

            public void WriteYaml(IEmitter emitter, object value, Type type)
            {
                var ipString = (value as IPAddress).ToString();
                emitter.Emit(new YamlDotNet.Core.Events.Scalar(ipString));
            }
        }
    }
}
