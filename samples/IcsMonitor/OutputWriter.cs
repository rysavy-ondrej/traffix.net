using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using IcsMonitor.Modbus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace IcsMonitor
{
    public sealed class OutputWriter : IDisposable
    {
        private readonly StreamWriter _streamWriter;


        public static OutputWriter Create(FileInfo outputFile)
        {
            return new OutputWriter(outputFile != null ? new StreamWriter(outputFile.Open(FileMode.Create)) : new StreamWriter(Console.OpenStandardOutput(), leaveOpen: true));
        }

        public OutputWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void Dispose()
        {
            ((IDisposable)_streamWriter).Dispose();
        }

        public async Task WriteOutputAsync<T>(OutputFormat outFormat, IAsyncEnumerable<ConversationRecord<T>> records)
        {
            switch (outFormat)
            {
                case OutputFormat.Yaml:
                    {
                        var serializer = new SerializerBuilder()
                            .WithNamingConvention(UnderscoredUpperCaseNamingConvention.Instance)
                            .DisableAliases()
                            .WithTypeConverter(new IPAddressYamlTypeConverter())
                            .Build();
                        await foreach (var obj in records)
                        {
                            _streamWriter.WriteLine("---");
                            serializer.Serialize(_streamWriter, obj);
                        }
                        break;
                    }
                case OutputFormat.Csv:
                    using (var csv = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture, true))
                    {
                        csv.Configuration.RegisterClassMap<MapConversationRecord<Dnp3FlowData>>();
                        csv.Configuration.RegisterClassMap<MapConversationRecord<ModbusFlowData.Compact>>();
                        csv.Configuration.RegisterClassMap<MapConversationRecord<ModbusFlowData.Complete>>();
                        csv.Configuration.RegisterClassMap<MapConversationRecord<ModbusFlowData.Extended>>();
                        await csv.WriteRecordsAsync(records.ToEnumerable());
                    }
                    break;

            }
            await _streamWriter.FlushAsync();
        }
        public class IPAddressYamlTypeConverter : IYamlTypeConverter
        {
            public bool Accepts(Type type)
            {
                return type == typeof(IPAddress);
            }

            public object ReadYaml(YamlDotNet.Core.IParser parser, Type type)
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

      
        sealed class MapConversationRecord<T> : ClassMap<ConversationRecord<T>>
        {
            public MapConversationRecord()
            {
                Map(m => m.Label).Name("Label");
                Map(m => m.Key.ProtocolType).Name("key_ProtocolType");
                Map(m => m.Key.DestinationIpAddress).Name("key_DestinationIpAddress");
                Map(m => m.Key.DestinationPort).Name("key_DestinationPort");
                Map(m => m.Key.SourceIpAddress).Name("key_SourceIpAddress");
                Map(m => m.Key.SourcePort).Name("key_SourcePort");

                Map(m => m.ForwardMetrics.Start).Name("fwd_Start");
                Map(m => m.ForwardMetrics.Duration).Name("fwd_Duration").TypeConverter<TimeSpanConverter>();
                Map(m => m.ForwardMetrics.Packets).Name("fwd_Packets");
                Map(m => m.ForwardMetrics.Octets).Name("fwd_Octets");

                Map(m => m.ReverseMetrics.Start).Name("rev_Start");
                Map(m => m.ReverseMetrics.Duration).Name("rev_Duration").TypeConverter<TimeSpanConverter>();
                Map(m => m.ReverseMetrics.Packets).Name("rev_Packets");
                Map(m => m.ReverseMetrics.Octets).Name("rev_Octets");

                var typ = typeof(ConversationRecord<T>);
                ReferenceMaps.Add(new MemberReferenceMap(typ.GetField(nameof(ConversationRecord<T>.Data)), new MapAllPublicProperties<T>()).Prefix("data_"));
            }
        }
        /// <summary>
        /// It adds all publlic properties of the given type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        sealed class MapAllPublicProperties<T> : ClassMap<T>
        {
            public MapAllPublicProperties()
            {
                var typ = typeof(T);
                var props = typ.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    MemberMaps.Add(MemberMap.CreateGeneric(typ, prop).Name(prop.Name));
                }
            }
        }
   
        class TimeSpanConverter : DefaultTypeConverter
        {
            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return ((TimeSpan)value).TotalSeconds.ToString();
            }
        }
    }
}
