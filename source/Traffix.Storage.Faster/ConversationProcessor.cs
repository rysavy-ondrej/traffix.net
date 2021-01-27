using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{
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
