using System;
using System.Collections.Generic;
using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{
    public class CustomConversationProcessor<T> : ConversationProcessor<T>
    {
        private readonly Func<FlowKey, ICollection<Memory<byte>>, T> _processor;

        public CustomConversationProcessor(Func<FlowKey, ICollection<Memory<byte>>,T> processor)
        {
            _processor = processor;
        }

        public override T Invoke(FlowKey flowKey, ICollection<Memory<byte>> frames)
        {
            return _processor(flowKey, frames);
        }
    }
}
