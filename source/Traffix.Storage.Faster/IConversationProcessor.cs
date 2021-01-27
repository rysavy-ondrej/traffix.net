using PacketDotNet;
using System;
using System.Collections.Generic;
using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{
    public interface IConversationProcessor<T>
    {
        /// <summary>
        /// When applied to a flow and its frames, it creates the result of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="flowKey">The flow key.</param>
        /// <param name="frames">The collection of byte arrays that constains metadata and bytes of frames of the flow.</param>
        /// <returns>The result of type <typeparamref name="T"/> or <see langword="null"/>.</returns>
        T Invoke(FlowKey flowKey, ICollection<Memory<byte>> frames);
    }
}
