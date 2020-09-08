using MessagePack;
using Traffix.Core.Flows;

namespace IcsMonitor
{

    [MessagePackObject]
    public class ConversationRecord<TData>
    {
        [Key("CONVERSATION_KEY")]
        public FlowKey Key;

        [Key("FORWARD_FLOW_METRICS")]
        public FlowMetrics ForwardMetrics;

        [Key("REVERSE_FLOW_METRICS")]
        public FlowMetrics ReverseMetrics;

        [Key("CUSTOM_DATA")]
        public TData CustomData;
    }
}