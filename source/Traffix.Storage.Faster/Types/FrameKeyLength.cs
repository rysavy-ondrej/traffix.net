using FASTER.core;
using System.Runtime.CompilerServices;
using Traffix.Core;

namespace Traffix.Storage.Faster
{
    internal class FrameKeyLength : IVariableLengthStruct<FrameKey>
    {
        public int GetInitialLength()
        {
            return Unsafe.SizeOf<FrameKey>();
        }

        public int GetLength(ref FrameKey t)
        {
            return Unsafe.SizeOf<FrameKey>();
        }
    }
}
