using System;
using System.Collections.Generic;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Processors
{
    internal class FuncConversationProcessor<TData> : IConversationProcessor<TData>
    {
        private Func<FlowKey, IEnumerable<Memory<byte>>, TData> _function;

        public FuncConversationProcessor(Func<FlowKey, IEnumerable<Memory<byte>>, TData> function)
        {
            this._function = function;
        }

        public TData Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
        {
            return _function.Invoke(flowKey, frames);
        }
    }
}