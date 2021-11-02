using SharpPcap;
using System.Collections.Generic;
using System.Threading;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;

namespace Traffix.Interactive
{
    public sealed class ConversationOperations
    {
        private readonly Interactive _interactive;

        internal ConversationOperations(Interactive interactive)
        {
            this._interactive = interactive;
        }
        /// <summary>
        /// Creates a conversation table from the given collection of <paramref name="frames"/>. 
        /// </summary>
        /// <param name="frames">Source frames used to populate conversation table.</param>
        /// <param name="conversationTablePath">The path to folder where conversation table is to be saved.</param>
        /// <param name="token">The cancellation token for interrupting the operation.</param>
        /// <returns>Newly created conversation table.</returns>
        public FasterConversationTable CreateConversationTable(IEnumerable<RawCapture> frames, string conversationTablePath, int framesCapacity, CancellationToken? token = null)
        {
            var flowTable = FasterConversationTable.Create(conversationTablePath, framesCapacity);
            using (var loader = flowTable.GetStreamer())
            {
                foreach (var frame in frames)
                {
                    loader.AddFrame(frame);
                    if (token?.IsCancellationRequested ?? false) break;
                }
            }
            flowTable.SaveChanges();
            return flowTable;
        }
    }
}