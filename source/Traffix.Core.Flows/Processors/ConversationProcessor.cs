using System;
using System.Runtime.CompilerServices;

namespace Traffix.Data
{
    /// <summary>
    /// Provides some basic methods for supporting conversation processor operations.
    /// </summary>
    public static class ConversationProcessor
    {
        /// <summary>
        /// Gets packet bytes from the provided <paramref name="buffer"/> and copies the frame metadata to 
        /// <paramref name="frame"/> structure.
        /// <para/>
        /// This is a helper method that simplifies reconstructing a frame from the provided memory bytes.
        /// It provides an efficent way of accessng frames as it does not copy any data nor allocate memory.
        /// </summary>
        /// <param name="buffer">The input memory range with frame metadata and content.</param>
        /// <param name="frame">The reference to <see cref="FrameMetadata"/> to be populated with Frame metadata.</param>
        /// <returns>The method returns span to the provided  <paramref name="buffer"/> containing the frame bytes. 
        /// Note that its lifetime is the same as the lifetime of the <paramref name="buffer"/>. </returns>
        public static unsafe Span<byte> GetFrameFromMemory(Memory<byte> buffer, ref FrameMetadata frame)
        {
            FrameMetadata.ReadMetadata(buffer.Span, ref frame);
            return buffer.Span[Unsafe.SizeOf<FrameMetadata>()..];   // provide the rest of the memory as frame payload
        }
    }
}
