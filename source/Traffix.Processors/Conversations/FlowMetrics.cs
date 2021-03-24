using MessagePack;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace Traffix.Processors
{
    [MessagePackObject]
    public struct FlowMetrics
    {

        [Key("METRICS_START")]
        public DateTime? Start;

        [Key("METRICS_END")]
        public DateTime? End;

        [Key("METRICS_DURATION")]
        public TimeSpan Duration => (End - Start) ?? new TimeSpan(0);

        [Key("METRICS_PACKETS")]
        public int Packets;

        [Key("METRICS_OCTETS")]
        public long Octets; 

        public static FlowMetrics Aggregate(ref FlowMetrics x, ref FlowMetrics y)
        {
            var newMetrics = new FlowMetrics
            {
                Start = DateTimeMin(x.Start,y.Start),
                End = DateTimeMax(x.End, y.End),
                Packets = x.Packets + y.Packets,
                Octets = x.Octets + y.Octets
            };
            return newMetrics;
        }

        public static void Update(ref FlowMetrics forwardMetrics, int octets, long ticks)
        {
            forwardMetrics.Packets++;
            forwardMetrics.Octets += octets;
            if (forwardMetrics.Start == null || ticks < forwardMetrics.Start?.Ticks)
                forwardMetrics.Start = new DateTime(ticks);
            if (forwardMetrics.End == null || ticks > forwardMetrics.End?.Ticks)
                forwardMetrics.End = new DateTime(ticks);
        }
        static DateTime? DateTimeMin(DateTime? x, DateTime? y)
        {
            if (x == null) return y;
            if (y == null) return x;
            return x < y ? x : y;
        }
        static DateTime? DateTimeMax(DateTime? x, DateTime? y)
        {
            if (x == null) return y;
            if (y == null) return x;
            return x > y ? x : y;
        }
    }
}