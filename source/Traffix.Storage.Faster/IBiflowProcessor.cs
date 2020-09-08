using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{

    public abstract class BiflowProcessor<T> : IBiflowProcessor<T>
    {
        /// <summary>
        /// Gets the frame from the provided <see cref="Memory{byte}"/> reference.
        /// </summary>
        /// <param name="buffer">The input memory range with frame metadta and content.</param>
        /// <param name="frame">The reference to <see cref="FrameValue"/> to be populated with Frame metadata.</param>
        /// <returns>The data bytes as <see cref="Span{T}"/> of the frame.</returns>
       protected unsafe Span<byte> GetFrame(Memory<byte> buffer, ref FrameMetadata frame)
       {
            fixed (void* ptr = buffer.Span)
            {
                frame = Unsafe.AsRef<FrameMetadata>(ptr);
            }
            return  FrameValue.GetFrameData(buffer.Span, frame.IncludedLength);
        }

        protected unsafe Packet GetPacket(Memory<byte> buffer)
        {
            fixed (void* ptr = buffer.Span)
            {
                ref var frame = ref Unsafe.AsRef<FrameValue>(ptr);
                return frame.GetPacket();
            }
        }


        public abstract T Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames);
    }

    public interface IBiflowProcessor<T>
    {
        /// <summary>
        /// When applied to a flow and its frames, it creates the result of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="flowKey">The flow key.</param>
        /// <param name="frames">The collection of raw frames of the flow.</param>
        /// <returns>The result of type <typeparamref name="T"/> or <see langword="null"/>.</returns>
        T Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames);
    }
}
