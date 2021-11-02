using System;

namespace Traffix.Core
{
    /// <summary>
    /// The key of a frame. It is represented as ulong value internally.
    /// <para/>
    /// The 64-bit key consists of: 
    /// ● 32-bit Unix Epoch with seconds resolution 
    /// ● 32-bit frame number  
    /// </summary>
    public readonly struct FrameKey
    {
        public readonly ulong Address;
        public FrameKey(ulong address)
        {
            Address = address;
        }
        public FrameKey(long ticks, uint frameNumber)
        {
            var epochSeconds = (ulong)new DateTimeOffset(ticks, TimeSpan.Zero).ToUnixTimeSeconds();
            Address = epochSeconds << 32 | frameNumber;
        }
        public long Epoch => (long)(Address >> 32);
        public long Number => (long)(Address & 0xffffffff);
    }
}
