using System.Collections.Generic;

namespace Traffix.Storage.Faster
{
    internal class ConversationContext
    {
        public List<ConversationOutput> OutputValues { get; } = new List<ConversationOutput>();

        public static ConversationContext Create()
        {
            return new ConversationContext();
        }
    }
}
