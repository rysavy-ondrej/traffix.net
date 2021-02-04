using System;
using Traffix.Core;

namespace Traffix.Data
{
    /// <summary>
    /// An interface that every frame processor should implement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFrameProcessor<T>
    {
        T Invoke(FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes);
    }
}
