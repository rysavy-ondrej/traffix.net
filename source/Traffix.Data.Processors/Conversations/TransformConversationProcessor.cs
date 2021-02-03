using System;
using System.Collections.Generic;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Processors
{
    class TransformConversationProcessor<TSource, TTarget> : IConversationProcessor<TTarget>
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
}
