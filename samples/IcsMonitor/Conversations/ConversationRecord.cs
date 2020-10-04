using MessagePack;
using System;
using System.Security.Cryptography.Xml;
using Traffix.Core.Flows;

namespace IcsMonitor
{


    [MessagePackObject]
    public struct RecordLabel
    {
        [Key("CLASS")]
        public string Class;

        [Key("SCORE")]
        public float Score;
    }

    [MessagePackObject]
    public class ConversationRecord<TData>
    {

        /// <summary>
        /// The label is mostly used for classification.
        /// </summary>
        [Key("CONVERSATION_LABEL")]
        public RecordLabel Label;

        [Key("CONVERSATION_KEY")]
        public FlowKey Key;

        [Key("FORWARD_FLOW_METRICS")]
        public FlowMetrics ForwardMetrics;

        [Key("REVERSE_FLOW_METRICS")]
        public FlowMetrics ReverseMetrics;

        [Key("CONVERSATION_DATA")]
        public TData Data;


        public ConversationRecord<TTarget> Transform<TTarget>(Func<TData, TTarget> transform)
        {
            return new ConversationRecord<TTarget>
            {
                Label = this.Label,
                Key = this.Key,
                ForwardMetrics = this.ForwardMetrics,
                ReverseMetrics = this.ReverseMetrics,
                Data = transform(this.Data)
            };
        }

        public static Func<ConversationRecord<TData>, ConversationRecord<TTarget>> TransformTo<TTarget>(Func<TData, TTarget> transform)
        {
            return x => x.Transform<TTarget>(transform);
        }
    }
}