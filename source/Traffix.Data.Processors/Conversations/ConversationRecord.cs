using MessagePack;
using System;
using Traffix.Core.Flows;

namespace Traffix.Processors
{

    /// <summary>
    /// The record produced by conversation processor. It contains a fixed part and 
    /// processor specific data. 
    /// </summary>
    /// <typeparam name="TData">The data specific to flow processor.</typeparam>
    [MessagePackObject]
    public class ConversationRecord<TData>
    {

        /// <summary>
        /// The label is mostly used for classification.
        /// </summary>
        [Key("CONVERSATION_LABEL")]
        public RecordLabel Label;

        /// <summary>
        /// The conversation key.
        /// </summary>
        [Key("CONVERSATION_KEY")]
        public FlowKey Key;


        /// <summary>
        /// A number of flows used to create a converation record.
        /// Usually, forward and reverse flows are used,ie., it equals 2.
        /// <para/>
        /// The conversation record may be an aggregation of multiple flows.
        /// </summary>
        [Key("ORIGINAL_FLOWS_PRESENT")]
        public int OriginalFlowsPresent;
        /// <summary>
        /// The forward flow metrics.
        /// </summary>
        [Key("FORWARD_FLOW_METRICS")]
        public FlowMetrics ForwardMetrics;

        /// <summary>
        /// Teh reverse flow metrics.
        /// </summary>
        [Key("REVERSE_FLOW_METRICS")]
        public FlowMetrics ReverseMetrics;

        /// <summary>
        /// The conversation data.
        /// </summary>
        [Key("CONVERSATION_DATA")]
        public TData Data;


        public ConversationRecord<TTarget> Transform<TTarget>(Func<TData, TTarget> transform)
        {
            return new ConversationRecord<TTarget>
            {
                Label = Label,
                Key = Key,
                ForwardMetrics = ForwardMetrics,
                ReverseMetrics = ReverseMetrics,
                Data = transform(Data)
            };
        }

        public static Func<ConversationRecord<TData>, ConversationRecord<TTarget>> TransformTo<TTarget>(Func<TData, TTarget> transform)
        {
            return x => x.Transform(transform);
        }

        public static ConversationRecord<TData> Combine(Func<TData, TData, TData> combineData, ConversationRecord<TData> left, ConversationRecord<TData> right)
        {
            return new ConversationRecord<TData>
            {
                Key = left.Key,
                OriginalFlowsPresent = left.OriginalFlowsPresent + right.OriginalFlowsPresent,
                ForwardMetrics = FlowMetrics.Combine(left.ForwardMetrics, right.ForwardMetrics),
                ReverseMetrics = FlowMetrics.Combine(left.ReverseMetrics, right.ReverseMetrics),
                Data = combineData(left.Data, right.Data)
            };
        }
    }
}