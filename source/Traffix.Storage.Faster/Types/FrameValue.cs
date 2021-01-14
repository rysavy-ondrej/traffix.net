using FASTER.core;
using PacketDotNet;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Traffix.Storage.Faster
{

    /// <summary>
    /// Represents the metadata of a frame.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct FrameMetadata
    {
        [FieldOffset(0)]
        public  long Ticks;

        [FieldOffset(8)]
        public  ushort OriginalLength;

        [FieldOffset(10)]
        public  ushort LinkLayer;

        [FieldOffset(12)]
        public  long FlowKeyHash;
    }
    /// <summary>
    /// Represents a variable length frame value. It has direct access to 
    /// its <see cref="IncludedLength"/> and <see cref="Ticks"/> fields.
    /// The payload starts at <see cref="Bytes"/> field. 
    /// 
    /// <para/>
    /// This structure can be used for both stack-allocating and heap allocated 
    /// frame objects. First, it is necessary to allocate a buffer for the strucutre (either on heap or stack).
    /// Then the structure can be recreated using 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct FrameValue
    {
        const int metadataOffset = 4;
        const int bytesOffset = 24;

        [FieldOffset(0)]
        internal int Length;
        [FieldOffset(metadataOffset)]
        internal FrameMetadata Meta;
        [FieldOffset(bytesOffset)]
        internal byte Bytes;

        public int BytesLength => Length - 24;

        internal static Span<byte> GetFrameBytesSpan(Span<byte> span)
        {
            return span[bytesOffset..];
        }
        internal static Span<byte> GetMetadataSpan(Span<byte> span)
        {
            return span[metadataOffset..Unsafe.SizeOf<FrameMetadata>()];
        }
        internal void CopyTo(ref FrameValue dst)
        {
            var fullLength = Length * sizeof(int);
            Buffer.MemoryCopy(Unsafe.AsPointer(ref this),
                Unsafe.AsPointer(ref dst), fullLength, fullLength);
        }

        /// <summary>
        /// Copies the current value to the specified memory buffer. 
        /// <para/>This operation is safe as it check the size of the target memory buffer.
        /// </summary>
        /// <param name="dst">The memory location to which the structure content will be copied.</param>
        /// <returns>Returns the number of bytes copied.</returns>
        internal unsafe void CopyTo(Span<byte> dst)
        {
            fixed (void* bp = dst)
            {
                var fullLength = Length * sizeof(int);
                Buffer.MemoryCopy(Unsafe.AsPointer(ref this),
                bp, fullLength, fullLength);
            }
        }

        /// <summary>
        /// Gets the size of buffer to accomodate the frame of the given length.
        /// </summary>
        /// <param name="frameBytesLength">The length of the frame.</param>
        /// <returns></returns>
        internal static int GetRequiredSize(int frameBytesLength)
        {
            return 24 + frameBytesLength;
        }

        /// <summary>
        /// Creates a new <seealso cref="FrameValue"/> for the given metadata and frame bytes.
        /// </summary>
        /// <param name="frameValue">The allocated space for <see cref="FrameValue"/> structure.</param>
        /// <param name="frameMetadata"The metadata.></param>
        /// <param name="frameBytes">The frame bytes.</param>
        /// <returns>Reference to newly initialzied <see cref="FrameValue"/> object.</returns>
        internal static ref FrameValue Create(ref FrameValue frameValue, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
        {
            frameValue.Length = GetRequiredSize(frameBytes.Length);
            frameValue.Meta = frameMetadata;
            frameBytes.CopyTo(new Span<byte>(Unsafe.AsPointer(ref frameValue.Bytes),frameBytes.Length));
            return ref frameValue;
        }
    }

    internal struct FrameValueLength : IVariableLengthStruct<FrameValue>
    {
        public int GetInitialLength()
        {
            return Unsafe.SizeOf<FrameValue>();
        }

        public int GetLength(ref FrameValue t)
        {
            return Unsafe.SizeOf<FrameValue>() + t.Length;
        }
    }
}
