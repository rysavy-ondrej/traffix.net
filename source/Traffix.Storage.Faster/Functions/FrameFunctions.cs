using FASTER.core;

namespace Traffix.Storage.Faster
{
    internal class FrameFunctions : IFunctions<FrameKey, FrameValue, FrameInput, FrameOutput, FrameContext>
    {
        public void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint)
        {
            throw new System.NotImplementedException();
        }

        public void ConcurrentReader(ref FrameKey key, ref FrameInput input, ref FrameValue value, ref FrameOutput dst)
        {
            var len = value.GetLength();
            dst.FrameBuffer = input.Pool.Rent(len);
            value.CopyTo(dst.FrameBuffer.Memory);
        }

        public bool ConcurrentWriter(ref FrameKey key, ref FrameValue src, ref FrameValue dst)
        {
            throw new System.InvalidOperationException("Each frame must have a unique key.");
        }

        public void CopyUpdater(ref FrameKey key, ref FrameInput input, ref FrameValue oldValue, ref FrameValue newValue)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCompletionCallback(ref FrameKey key, FrameContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public void InitialUpdater(ref FrameKey key, ref FrameInput input, ref FrameValue value)
        {
            throw new System.NotImplementedException();
        }

        public bool InPlaceUpdater(ref FrameKey key, ref FrameInput input, ref FrameValue value)
        {
            throw new System.NotImplementedException();
        }

        public void ReadCompletionCallback(ref FrameKey key, ref FrameInput input, ref FrameOutput output, FrameContext ctx, Status status)
        {
            throw new System.NotImplementedException();
        }

        public void RMWCompletionCallback(ref FrameKey key, ref FrameInput input, FrameContext ctx, Status status)
        {
            throw new System.NotImplementedException();
        }

        public void SingleReader(ref FrameKey key, ref FrameInput input, ref FrameValue value, ref FrameOutput dst)
        {
            var len = value.GetLength();
            dst.FrameBuffer = input.Pool.Rent(len);
            value.CopyTo(dst.FrameBuffer.Memory);
        }

        public void SingleWriter(ref FrameKey key, ref FrameValue src, ref FrameValue dst)
        {
            src.CopyTo(ref dst);
        }

        public void UpsertCompletionCallback(ref FrameKey key, ref FrameValue value, FrameContext ctx)
        {
            throw new System.NotImplementedException();
        }
    }
    internal class FramesStore : StoreDb<FrameKey, FrameValue, FrameInput, FrameOutput, FrameContext, FrameFunctions>
    {
        public FramesStore(string folder) : base(folder, new FrameKeyFastComparer(), new FrameFunctions(), new FrameKeyLength(), new FrameValueLength())
        {
        }
    }
}
