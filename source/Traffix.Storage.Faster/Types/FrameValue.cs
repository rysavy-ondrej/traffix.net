using FASTER.core;
using PacketDotNet;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Traffix.Storage.Faster
{

    [StructLayout(LayoutKind.Explicit)]
    public struct FrameMetadata
    {
        [FieldOffset(0)]
        public long Ticks;

        [FieldOffset(8)]
        public ushort IncludedLength;

        [FieldOffset(10)]
        public ushort OriginalLength;

        [FieldOffset(12)]
        public ushort LinkLayer;

        [FieldOffset(16)]
        public long FlowKeyHash;
    }
    /// <summary>
    /// Represents a variable length frame value. It has direct access to 
    /// its <see cref="IncludedLength"/> and <see cref="Ticks"/> fields.
    /// The payload starts at <see cref="Bytes"/> field. 
    /// </summary>
    /// <remarks>
    /// This structure is intentionally the same shape as 
    /// the record header strucutre of PCAP file. It enables 
    /// to use the input buffer when reading PCAP file without copying. 
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct FrameValue
    {
        [FieldOffset(0)]
        public FrameMetadata Meta;
        [FieldOffset(24)]
        public byte Bytes;

        /// <summary>
        /// Copies the frame bytes to the provided byte span. 
        /// If the provided span is less than <see cref="IncludedLength"/>
        /// then only the portion of bytes is copied.
        /// </summary>
        /// <param name="dst">The array of bytes to copy.</param>
        public int GetFrameBytes(Span<byte> dst)
        {
            var len = Math.Min(Meta.IncludedLength, dst.Length);
            var src = (byte*)Unsafe.AsPointer(ref this.Bytes);
            for (int i = 0; i < len; i++)
            {
                dst[i] = *src;
                src++;
            }
            return len;
        }

        public void CopyTo(ref FrameValue dst)
        {
            var fulllength = ComputeLength(Meta.IncludedLength);
            Buffer.MemoryCopy(Unsafe.AsPointer(ref this),
                Unsafe.AsPointer(ref dst), fulllength, fulllength);
        }
        /// <summary>
        /// Copy the current value to the specified destination. 
        ///  
        /// </summary>
        /// <param name="dst">The memory location to which the structure content will be copied.</param>
        /// <param name="maxlen">The maximum number of bytes to copy.</param>
        /// <returns>Returns the number of bytes copied.</returns>
        public unsafe void CopyTo(Memory<byte> dst)
        {
            fixed (void* bp = dst.Span)
            {
                Buffer.MemoryCopy(Unsafe.AsPointer(ref this), bp, dst.Length, ComputeLength(Meta.IncludedLength));
            }
        }

        internal static int ComputeLength(int length)
        {
            return (Unsafe.SizeOf<FrameMetadata>() + length);
        }

        internal int GetLength()
        {
            return (Unsafe.SizeOf<FrameMetadata>() + Meta.IncludedLength);
        }


        public Packet GetPacket()
        {
            var bytes = new byte[Meta.IncludedLength];
            GetFrameBytes(new Span<byte>(bytes));
            return Packet.ParsePacket((LinkLayers)Meta.LinkLayer, bytes);
        }


        /// <summary>
        /// Gets the span of bytes of the frame if allocated within the array.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Span<byte> GetFrameData(Span<byte> src, int includedLength)
        {
            return src.Slice(Unsafe.SizeOf<FrameMetadata>(), includedLength);
        }
    }

    public struct FrameValueLength : IVariableLengthStruct<FrameValue>
    {
        public int GetInitialLength()
        {
            return Unsafe.SizeOf<FrameValue>();
        }

        public int GetLength(ref FrameValue t)
        {
            return t.GetLength();
        }
    }
}
