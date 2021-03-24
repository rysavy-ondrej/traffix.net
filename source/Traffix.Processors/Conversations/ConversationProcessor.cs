using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PacketDotNet;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Processors
{
    /// <summary>
    /// Defines the callback method for a conversation processor.
    /// This method is supposed to be called on every conversation. 
    /// </summary>
    /// <param name="flowKey">The flow key of the conversation.</param>
    /// <param name="fwdMetrics">The forward flow metrics.</param>
    /// <param name="revMetrics">The reverse flow metrics.</param>
    /// <param name="fwdPackets">The forward flow packets.</param>
    /// <param name="revPackets">The reverse flow packets.</param>
    /// <typeparam name="Target">The target type.</typeparam>
    /// <returns>When completed, it should return a value of the target type.</returns>
    public delegate Target ProcessConversationCallback<Target>(ref FlowKey flowKey, ref FlowMetrics fwdMetrics, ref FlowMetrics revMetrics, IReadOnlyCollection<MetaPacket> fwdPackets, IReadOnlyCollection<MetaPacket> revPackets);

    /// <summary>
    /// Provides some basic methods for supporting conversation processor operations.
    /// </summary>
    public static class ConversationProcessor
    {
        /// <summary>
        /// Creates a new conversation processor from the given function.
        /// <para/>
        /// This overload works with raw memory representing the every frame. 
        /// </summary>
        /// <param name="function">The function to apply on every conversation.</param>
        /// <typeparam name="TResult">The result type produced by the function.</typeparam>
        /// <returns>New conversation processor implementation using the given function to process conversations.</returns>
        public static IConversationProcessor<TResult> FromFunction<TResult>(Func<FlowKey, IEnumerable<Memory<byte>>, TResult> function)
        {
            return new FuncConversationProcessor<TResult>(function);
        }
        public static IConversationProcessor<ConversationRecord<TResult>> FromFunction<TResult>(ProcessConversationCallback<TResult> function)
        {
            return new PacketConversationProcessor<TResult>(function);
        }
    }
}
