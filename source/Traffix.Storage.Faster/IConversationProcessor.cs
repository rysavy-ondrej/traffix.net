using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{

    /// <summary>
    /// Represents a flowdirection within a conversation.
    /// </summary>
    public enum FlowDirection
    {
        /// <summary>
        /// Forward flow consists of packets sent by the client to the server.
        /// </summary>
        Forward, 
        /// <summary>
        /// Reverse flow consists of packets sent by the server to the client. 
        /// </summary>
        Reverse
    }
    public abstract class ConversationProcessor<T> : IConversationProcessor<T>
    {
        /// <summary>
        /// Gets packet bytes from the provided <paramref name="buffer"/> and copies the frame metadata to 
        /// <paramref name="frame"/> structure.
        /// <para/>
        /// 
        /// </summary>
        /// <param name="buffer">The input memory range with frame metadata and content.</param>
        /// <param name="frame">The reference to <see cref="FrameMetadata"/> to be populated with Frame metadata.</param>
        /// <returns>The method returns span to the provided  <paramref name="buffer"/> containing the frame bytes. 
        /// Note that its lifetime is the same as the lifetime of the <paramref name="buffer"/>. </returns>
        public static unsafe Span<byte> GetFrame(Memory<byte> buffer, ref FrameMetadata frame)
        {
            fixed (void* ptr = FrameValue.GetMetadataSpan(buffer.Span))
            {
                frame = Unsafe.AsRef<FrameMetadata>(ptr);   // copy the struct from buffer to provided location
                return FrameValue.GetFrameBytesSpan(buffer.Span);
            }
        }
        public abstract T Invoke(FlowKey flowKey, ICollection<Memory<byte>> frames);
    }

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
