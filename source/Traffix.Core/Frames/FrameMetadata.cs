using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Traffix.Core
{


    /// <summary>
    /// Represents the metadata of a single frame.
    /// It is a structure that has fixed size of 20 bytes.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct FrameMetadata
    {
        /// <summary>
        /// The number of ticks that represent the date and time of this frame. 
        /// </summary>
        [FieldOffset(0)]
        public long Ticks;

        /// <summary>
        /// The original length of the frame. 
        /// </summary>
        [FieldOffset(8)]
        public ushort OriginalLength;

        /// <summary>
        /// The link layer of the frame.
        /// </summary>
        [FieldOffset(10)]
        public ushort LinkLayer;

        /// <summary>
        /// The flow key hash computed for the frame.
        /// </summary>
        [FieldOffset(12)]
        public long FlowKeyHash;

        /// <summary>
        /// Gets packet bytes from the provided <paramref name="buffer"/> and copies the frame metadata to 
        /// <paramref name="frameMetadata"/> structure.
        /// <para/>
        /// This is a helper method that simplifies reconstructing a frame from the provided memory bytes.
        /// It provides an efficent way of accessng frames as it does not copy any data nor allocate memory.
        /// </summary>
        /// <param name="buffer">The input memory range with frame metadata and content.</param>
        /// <param name="frameMetadata">The reference to <see cref="FrameMetadata"/> to be populated with Frame metadata.</param>
        /// <returns>The method returns span to the provided  <paramref name="buffer"/> containing the frame bytes. 
        /// Note that its lifetime is the same as the lifetime of the <paramref name="buffer"/>. </returns>
        public static unsafe Span<byte> FromBytes(Span<byte> buffer, ref FrameMetadata frameMetadata)
        {
            fixed (void* ptr = buffer)
            {
                frameMetadata = Unsafe.AsRef<FrameMetadata>(ptr);
            }
            return buffer[Unsafe.SizeOf<FrameMetadata>()..];   // provide the rest of the memory as frame payload
        }
    }
}
