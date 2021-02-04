using FASTER.core;
using Traffix.Core;

namespace Traffix.Storage.Faster
{
    internal class FrameKeyFastComparer : IFasterEqualityComparer<FrameKey>
    {
        public bool Equals(ref FrameKey k1, ref FrameKey k2)
        {
            return k1.Address == k2.Address;
        }

        public long GetHashCode64(ref FrameKey k)
        {
            return k.Address;
        }
    }
}
