using System;
using System.Collections.Generic;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Processors
{
    internal class TransformConversationProcessor<TSource, TTarget> : IConversationProcessor<TTarget>
    {
        private readonly IConversationProcessor<TSource> processor;
        private readonly Func<TSource, TTarget> transform;

        public TransformConversationProcessor(IConversationProcessor<TSource> processor, Func<TSource, TTarget> transform)
        {
            this.processor = processor;
            this.transform = transform;
        }

        public TTarget Invoke(FlowKey flowKey, ICollection<Memory<byte>> frames)
        {
            var intermediate = processor.Invoke(flowKey, frames);
            return transform.Invoke(intermediate);
        }
    }
    public static class TransformConversationProcessor
    {
        /// <summary>
        /// Creates a new conversation processor that produces values of <typeparamref name="Target"/> objects 
        /// by applying the given transofrmation function.
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        /// <returns></returns>
        public static IConversationProcessor<Target> Transform<TSource, Target>(this IConversationProcessor<TSource> source, Func<TSource, Target> transform)
        {
            return new TransformConversationProcessor<TSource, Target>(source, transform);
        }
    }
}
