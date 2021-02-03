using MessagePack;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace Traffix.Processors
{
    [MessagePackObject]
    public class FlowMetrics
    {

        [Key("METRICS_START")]
        public DateTime Start;

        [Key("METRICS_END")]
        public DateTime End;

        [Key("METRICS_DURATION")]
        public TimeSpan Duration => End - Start;

        [Key("METRICS_PACKETS")]
        public int Packets;

        [Key("METRICS_OCTETS")]
        public long Octets; 

        public static FlowMetrics Combine(FlowMetrics x, FlowMetrics y)
        {
            return new FlowMetrics
            {
                Start = x.Start < y.Start ? x.Start : y.Start,
                End = x.End > y.End ? x.End : y.End,
                Packets = x.Packets + y.Packets,
                Octets = x.Octets + y.Octets
            };
        }
        internal static void AddToSchema(DataViewSchema.Builder builder, string prefix)
        {
            builder.AddColumn($"{prefix}_Start", DateTimeDataViewType.Instance);
            builder.AddColumn($"{prefix}_End", DateTimeDataViewType.Instance);
            builder.AddColumn($"{prefix}_Duration", NumberDataViewType.Single);
            builder.AddColumn($"{prefix}_Packets", NumberDataViewType.UInt32);
            builder.AddColumn($"{prefix}_Octets", NumberDataViewType.UInt64);
        }
    }
}