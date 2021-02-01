using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Traffix.Data
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
        /// Reads metadata from the given span.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="metadat"></param>
        public unsafe static void ReadMetadata(Span<byte> bytes, ref FrameMetadata metadata)
        {
            fixed (void* ptr = bytes)
            {
                metadata = Unsafe.AsRef<FrameMetadata>(ptr);
            }
        }
    }
}
