using MessagePack;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Namotion.Reflection;
using System;
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