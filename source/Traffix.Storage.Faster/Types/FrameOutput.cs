using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Traffix.Storage.Faster
{
    internal struct FrameOutput
    {
        IMemoryOwner<byte>? _memoryOwner;

        public void SetBuffer(IMemoryOwner<byte> memoryOwner)
        {
            _memoryOwner = memoryOwner;    
        }
        public void ReleaseBuffer()
        {
            _memoryOwner?.Dispose();
            _memoryOwner = default;
        }

        public unsafe ref FrameMetadata ReadMetadata(ref FrameMetadata metadata)
        {
            var src = GetMetadata();
            metadata = Unsafe.AsRef<FrameMetadata>((void*)src.GetPinnableReference());
            return ref metadata;
        }
        public void ReadFrameBytes(Span<byte> targetBuffer)
        {
            var src = GetFrameBytes();
            src.CopyTo(targetBuffer);
        }
        internal Span<byte> GetMetadata()
        {
            if (_memoryOwner == null) throw new InvalidOperationException("Buffer not assigned.");
            return FrameValue.GetMetadataSpan(_memoryOwner.Memory.Span);
        }
        internal Span<byte> GetFrameBytes()
        {
            if (_memoryOwner == null) throw new InvalidOperationException("Buffer not assigned.");
            return FrameValue.GetFrameBytesSpan(_memoryOwner.Memory.Span);
        }

        internal Memory<byte> GetBuffer()
        {         
             if (_memoryOwner == null) throw new InvalidOperationException("Buffer not assigned.");
            return this._memoryOwner.Memory;
        }
    }
}
