using System;

namespace Traffix.Core
{
    /// <summary>
    /// Represents a reference structure to frame key, metadata and its bytes.
    /// </summary>
    public ref struct FrameRef
    {
        public FrameKey Key;
        public FrameMetadata Metadata;
        public Span<byte> Bytes;
    }
}
