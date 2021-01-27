using MessagePack;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.ML;
using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Traffix.Core.Flows;
using Traffix.Storage.Faster;

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