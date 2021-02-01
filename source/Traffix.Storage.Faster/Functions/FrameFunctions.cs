using System;

namespace Traffix.Storage.Faster
{


    internal class FrameFunctions : KeyValueStore<FrameKey, FrameValue, FrameInput, FrameOutput, FrameFunctions>.StoreFunctions
    {
        public override void ConcurrentReader(ref FrameKey key, ref FrameInput input, ref FrameValue value, ref FrameOutput output)
        {
            var len = value.Length;
            var buffer = input.Pool.Rent(len);
            value.CopyTo(buffer.Memory.Span);
            output.SetBuffer(buffer);
        }

        public override bool ConcurrentWriter(ref FrameKey key, ref FrameValue src, ref FrameValue dst)
        {
            throw new System.InvalidOperationException("Each frame must have a unique key.");
        }

        public override void SingleReader(ref FrameKey key, ref FrameInput input, ref FrameValue value, ref FrameOutput output)
        {
            var len = value.Length;
            var buffer = input.Pool.Rent(len);
            value.CopyTo(buffer.Memory.Span);
            output.SetBuffer(buffer);
        }

        public override void SingleWriter(ref FrameKey key, ref FrameValue src, ref FrameValue dst)
        {
            src.CopyTo(ref dst);
            Written++;
        }
        public static int Written;
    }
    internal class FramesStore : KeyValueStore<FrameKey, FrameValue, FrameInput, FrameOutput, FrameFunctions>
    {
        public int Written => FrameFunctions.Written;
        public FramesStore(string folder, long capacity) : base(folder, (int)Math.Log(capacity, 2) + 1, new FrameKeyFastComparer(), new FrameFunctions(), new FrameKeyLength(), new FrameValueLength())
        {
        }
    }
}