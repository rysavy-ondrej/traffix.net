using FASTER.core;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Traffix.Core;
using Traffix.Data;

namespace Traffix.Storage.Faster
{
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

        /// <summary>
        /// Represents the total length of this structure. 
        /// <para/> 
        /// This value is important for allocating the copies of the instance and
        /// delimiting the end of the data part of the object.
        /// </summary>
        [FieldOffset(0)]
        internal int Length;
        /// <summary>
        /// The frame metadata.
        /// </summary>
        [FieldOffset(metadataOffset)]
        internal FrameMetadata Meta;
        /// <summary>
        /// The first byte of frame content.
        /// </summary>
        [FieldOffset(bytesOffset)]
        internal byte Bytes;

        /// <summary>
        /// Gets the length of frame bytes.
        /// </summary>
        internal int BytesLength => Length - bytesOffset;

        /// <summary>
        /// Gets the span that contains only the frame bytes.
        /// </summary>
        /// <param name="span">The source span that contains the entire <seealso cref="FrameValue"/> object.</param>
        /// <returns>The span that contains to the frame bytes.</returns>
        internal static Span<byte> GetFrameBytesSpan(Span<byte> span)
        {
            var length = BitConverter.ToInt32(span);
            return span.Slice(bytesOffset, length-bytesOffset);
        }

        /// <summary>
        /// Gets the metadata followed by entire frame content.
        /// </summary>
        /// <param name="memory">The input memory with byte representation of <see cref="FrameValue"/> structure.</param>
        /// <returns>A memory slice of metadata followed by entire frame content.</returns>
        internal static Memory<byte> GetFrameMetadataAndBytes(Memory<byte> memory)
        {
            var length = BitConverter.ToInt32(memory.Span);
            return memory.Slice(metadataOffset, length-metadataOffset);
        }
        /// <summary>
        /// Gets the span that contains <see cref="FrameMetadata"/> structure.
        /// <para/>
        /// This method enables to access the internal byte representation of <see cref="FrameMetadata"/> structure
        /// of the <seealso cref="FrameValue"/>. 
        /// </summary>
        /// <param name="span">The source span that contains the entire <seealso cref="FrameValue"/> object.</param>
        /// <returns>The span that contains <see cref="FrameMetadata"/> structure.</returns>
        internal static Span<byte> GetMetadataSpan(Span<byte> span)
        {
            return span.Slice(metadataOffset, Unsafe.SizeOf<FrameMetadata>());
        }

        /// <summary>
        /// Copies the current object to the specified <see cref="FrameValue"/> object.
        /// <para/>
        /// The caller is responsible that the destination <see cref="FrameValue"/> is backed with 
        /// a byte array (on stack or heap) of the sufficient size. The size of the destination needs to be at least <see cref="FrameValue.Length"/> bytes.
        /// </summary>
        /// <param name="dst">The destionation <see cref="FrameValue"/> object to be filled with bytes of the current object.</param>
        unsafe internal void CopyTo(ref FrameValue dst)
        {
            Buffer.MemoryCopy(Unsafe.AsPointer(ref this.Length),Unsafe.AsPointer(ref dst.Length), Length, Length);
        }

        /// <summary>
        /// Copies the current value to the specified memory buffer. 
        /// <para/>This operation is safe as it checks the size of the target memory buffer.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided Span does not contain enough space 
        /// to accomodate the copy of the current object.</exception>
        /// <param name="dst">The memory location to which the structure content will be copied.</param>
        /// <returns>Returns the number of bytes copied.</returns>
        internal unsafe void CopyTo(Span<byte> span)
        {
            if (span.Length < Length) throw new ArgumentOutOfRangeException("The provided Span is too small.");
            fixed (void* dstPtr = span)
            {
                Buffer.MemoryCopy(Unsafe.AsPointer(ref this.Length), dstPtr, Length, Length);
            }
        }
        internal void CopyFrameBytesTo(Span<byte> span)
        {
            if (span.Length < this.BytesLength) throw new ArgumentOutOfRangeException("The provided Span is too small.");
            fixed (void* dstPtr = span)
            {
                Buffer.MemoryCopy(Unsafe.AsPointer(ref this.Bytes), dstPtr, this.BytesLength, this.BytesLength);
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
        /// Creates a new <see cref="FrameValue"/> for the given metadata and frame bytes.
        /// <para/>
        /// As <see cref="FrameValue"/> can only be created in the allocated byte buffer (on stack or heap)
        /// it is required to provide the uninitialized object as a parameter. 
        /// The underlying byte array must be pinned or stack allocated.
        /// This object will then  
        /// be intialized using the provided <paramref name="frameMetadata"/> and <paramref name="frameBytes"/>. 
        /// </summary>
        /// <param name="frameValue">The allocated space for <see cref="FrameValue"/> structure. 
        /// The underlying byte array must be pinned or stack allocated.</param>
        /// <param name="frameMetadata">The metadata.</param>
        /// <param name="frameBytes">The frame bytes.</param>
        /// <returns>Reference to newly initialzied <see cref="FrameValue"/> object.</returns>
        internal static void Create(ref FrameValue frameValue, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
        {
            frameValue.Length = GetRequiredSize(frameBytes.Length);
            frameValue.Meta = frameMetadata;
            frameBytes.CopyTo(new Span<byte>(Unsafe.AsPointer(ref frameValue.Bytes),frameBytes.Length));
        }

        internal byte[] DumpEthernetHeader()
        {
            var span = new Span<byte>(Unsafe.AsPointer(ref this.Bytes), 18);
            return span.ToArray();
        }
        internal PacketDotNet.Packet DumpPacket()
        {
            var span = new Span<byte>(Unsafe.AsPointer(ref this.Bytes), this.BytesLength);
            return PacketDotNet.Packet.ParsePacket((PacketDotNet.LinkLayers)this.Meta.LinkLayer, span.ToArray());
        }
        internal static PacketDotNet.Packet DumpPacket(Span<byte> bytes)
        {
            fixed (void* ptr = bytes)
            {
                return Unsafe.AsRef<FrameValue>(ptr).DumpPacket();
            }
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
