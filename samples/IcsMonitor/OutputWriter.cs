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
using Traffix.Core.Flows;
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

        static INamingConvention NamingConvention { get; set; } = UnderscoredUpperCaseNamingConvention.Instance;
        sealed class MapConversationRecord<T> : ClassMap<ConversationRecord<T>>
        {
           
            public MapConversationRecord()
            {
                var recordType = typeof(ConversationRecord<T>);
                References(typeof(MapAllPublicFields<RecordLabel>), recordType.GetField(nameof(ConversationRecord<T>.Label)), "Label");

                Map(m => m.Key.ProtocolType).Name(NamingConvention.Apply("KeyProtocolType"));
                Map(m => m.Key.DestinationIpAddress).Name(NamingConvention.Apply("KeyDestinationIpAddress"));
                Map(m => m.Key.DestinationPort).Name(NamingConvention.Apply("KeyDestinationPort"));
                Map(m => m.Key.SourceIpAddress).Name(NamingConvention.Apply("KeySourceIpAddress"));
                Map(m => m.Key.SourcePort).Name(NamingConvention.Apply("KeySourcePort"));

                Map(m => m.ForwardMetrics.Start).Name(NamingConvention.Apply("FwdStart")).TypeConverter<DateTimeToUnixConverter>();
                Map(m => m.ForwardMetrics.Duration).Name(NamingConvention.Apply("FwdDuration")).TypeConverter<TimeSpanToFloatConverter>();
                Map(m => m.ForwardMetrics.Packets).Name(NamingConvention.Apply("FwdPackets"));
                Map(m => m.ForwardMetrics.Octets).Name(NamingConvention.Apply("FwdOctets"));

                Map(m => m.ReverseMetrics.Start).Name(NamingConvention.Apply("RevStart")).TypeConverter<DateTimeToUnixConverter>(); ;
                Map(m => m.ReverseMetrics.Duration).Name(NamingConvention.Apply("RevDuration")).TypeConverter<TimeSpanToFloatConverter>();
                Map(m => m.ReverseMetrics.Packets).Name(NamingConvention.Apply("RevPackets"));
                Map(m => m.ReverseMetrics.Octets).Name(NamingConvention.Apply("RevOctets"));

                References(typeof(MapAllPublicProperties<T>), recordType.GetField(nameof(ConversationRecord<T>.Data)), "Data");
            }
        }
        /// <summary>
        /// It adds all publlic properties of the given type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        sealed class MapAllPublicProperties<T> : ClassMap<T>
        {
            public MapAllPublicProperties(string prefix)
            {
                var typ = typeof(T);
                var props = typ.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    MemberMaps.Add(MemberMap.CreateGeneric(typ, prop).Name(NamingConvention.Apply($"{prefix}{prop.Name}")));
                }
            }
        }
        sealed class MapAllPublicFields<T> : ClassMap<T> //where T :struct
        {
            public MapAllPublicFields(string prefix)
            {
                var typ = typeof(T);
                var fields = typ.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    MemberMaps.Add(MemberMap.CreateGeneric(typ, field).Name(NamingConvention.Apply($"{prefix}{field.Name}")));
                }
            }
        }
   
        class TimeSpanToFloatConverter : DefaultTypeConverter
        {
            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return ((TimeSpan)value).TotalSeconds.ToString();
            }
        }
        class DateTimeToUnixConverter : DefaultTypeConverter
        {
            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return new DateTimeOffset(((DateTime)value).Ticks, TimeSpan.Zero).ToUnixTimeMilliseconds().ToString();
            }
        }
    }
}
