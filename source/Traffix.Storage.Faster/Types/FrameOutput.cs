using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Traffix.Data;

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
        /// <summary>
        /// Gets the entire memory buffer associated with this object.
        /// <para/>
        /// Becasue memory can be drawn from the memory pool the memory object 
        /// can be larger then <see cref="FrameValue"/> object instantiated.
        /// </summary>
        /// <returns></returns>
        internal Memory<byte> GetBuffer()
        {         
            if (_memoryOwner == null) throw new InvalidOperationException("Buffer not assigned.");
            return this._memoryOwner.Memory;
        }
        /// <summary>
        /// Gets the frame. It consists of <see cref="FrameMetadata"/> header immediately followed by frame bytes.
        /// </summary>
        /// <returns>The memory containing frame data.</returns>
        internal Memory<byte> GetRawFrameWithMetadata()
        {
            if (_memoryOwner == null) throw new InvalidOperationException("Buffer not assigned.");
            return FrameValue.GetFrameMetadataAndBytes(this._memoryOwner.Memory);
        }
    }
}
