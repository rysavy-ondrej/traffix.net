using System.Buffers;

namespace Traffix.Storage.Faster
{
    internal struct FrameOutput
    {
        public IMemoryOwner<byte> FrameBuffer;
    }
}
