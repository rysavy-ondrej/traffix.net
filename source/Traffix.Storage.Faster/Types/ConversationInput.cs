using Traffix.Core.Flows;

namespace Traffix.Storage.Faster
{
    internal struct ConversationInput
    {
        public FlowKey FrameKey;
        public long FrameTicks;
        public int FrameSize;
        public long FrameAddress;
    }
}
