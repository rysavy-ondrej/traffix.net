using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Processors
{
    /// <summary>
    /// Provides some basic methods for supporting conversation processor operations.
    /// </summary>
    public static class ConversationProcessor
    {
        public static IConversationProcessor<TResult> FromFunction<TResult>(Func<FlowKey, IEnumerable<Memory<byte>>, TResult> function)
        {
            return new FuncConversationProcessor<TResult>(function);
        }
    }
}
