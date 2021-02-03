using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Processors
{
    public abstract class ConversationProcessorBase<T> : IConversationProcessor<T>
    {
        public abstract T Invoke(FlowKey flowKey, ICollection<Memory<byte>> frames);

        /// <summary>
        /// Creates a new conversation processor that produces values of <typeparamref name="Target"/> objects 
        /// by applying the given transofrmation function.
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        /// <returns></returns>
        public IConversationProcessor<Target> Transform<Target>(Func<T, Target> transform)
        {
            return new TransformConversationProcessor<T, Target>(this, transform);
        }
    }
}
