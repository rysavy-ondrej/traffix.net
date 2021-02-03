using System;
using System.Collections.Generic;
using Traffix.Core.Flows;

namespace Traffix.Processors
{
    public class FuncConversationProcessor<T> : ConversationProcessorBase<T>
    {
        private readonly Func<FlowKey, ICollection<Memory<byte>>, T> _processor;

        public FuncConversationProcessor(Func<FlowKey, ICollection<Memory<byte>>,T> processor)
        {
            _processor = processor;
        }

        public override T Invoke(FlowKey flowKey, ICollection<Memory<byte>> frames)
        {
            return _processor(flowKey, frames);
        }
    }
}
