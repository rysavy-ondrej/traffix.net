using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using Traffix.Hosting.Console;
using Traffix.Storage.Faster;

namespace IcsMonitor
{
    /// <summary>
    /// Interactive class exposes various API methods usable in C# interactive sessions. 
    /// </summary>
    public class Interactive
    {
        public FasterConversationTable ReadToConversationTable(string pcapFile, string conversationTablePath, CancellationToken token)
        {
            using var stream = new FileInfo(pcapFile).OpenRead();
            return ReadToConversationTable(stream, conversationTablePath, token);
        }
        public FasterConversationTable ReadToConversationTable(Stream stream, string conversationTablePath, CancellationToken token)
        {
            var flowTable = new FasterConversationTable(conversationTablePath);
            flowTable.LoadFromStream(stream, token, null);
            return flowTable;
        }

        public IEnumerable<T> GetConversations<T> (FasterConversationTable table, IConversationProcessor<T> processor)
        {
            return table.ProcessConversations<T>(table.ConversationKeys, processor);

        }
    }
}
