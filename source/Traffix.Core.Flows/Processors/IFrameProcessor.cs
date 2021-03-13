using System;
using Traffix.Core;

namespace Traffix.Data
{
    /// <summary>
    /// A delegate type for frame processor methods. 
    /// </summary>
    /// <typeparam name="TValue">The resulting type of the processor.</typeparam>
    /// <param name="frameKey">The frame key.</param>
    /// <param name="frameMetadata">The frame metadata.</param>
    /// <param name="frameBytes">The span of bytes representing frame content.</param>
    /// <returns>The value of <typeparamref name="TValue"/> type for the given frame.</returns>
    public delegate TValue FrameProcessor<TValue>(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes);

    /// <summary>
    /// An interface that every frame processor should implement.
    /// </summary>
    /// <typeparam name="TValue">The resulting type of the processor.</typeparam>
    public interface IFrameProcessor<TValue>
    {
        /// <summary>
        /// A method to be invoked on every frame. 
        /// </summary>
        /// <param name="frameKey">The frame key.</param>
        /// <param name="frameMetadata">The frame metadata.</param>
        /// <param name="frameBytes">The span of bytes representing frame content.</param>
        /// <returns>The value of <typeparamref name="TValue"/> type for the given frame.</returns>
        TValue Invoke(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes);
    }
}
