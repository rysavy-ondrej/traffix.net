using MessagePack;
using System;

namespace IcsMonitor
{
    [MessagePackObject]
    public class FlowMetrics
    {
        [Key("METRICS_START")]
        public DateTime Start { get; set; }

        [Key("METRICS_END")]
        public DateTime End { get; set; }

        [Key("METRICS_DURATION")]
        public TimeSpan Duration => End - Start;

        [Key("METRICS_PACKETS")]
        public int Packets { get; set; }

        [Key("METRICS_OCTETS")]
        public long Octets { get; set; }
    }
}