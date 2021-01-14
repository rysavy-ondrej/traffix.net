using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Traffix.Hosting.Console;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;

namespace IcsMonitor.Commands
{
    [Cmdlet("Extract", "S7CommConversations")]
    public class ExtractS7CommConversationsCommand : AsyncCmdlet
    {
        public FileInfo InputFile { get; set; }

        FasterConversationTable _flowTable;
        protected override Task BeginProcessingAsync()
        {
            _flowTable = FasterConversationTable.Create("tmp", 100000);

            var frameNumber = 0;
            using (var loader = _flowTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(InputFile.FullName))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame,  ++frameNumber);
                }
                loader.Close();
            }

            return Task.CompletedTask;
        }

        protected override Task EndProcessingAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task ProcessRecordAsync()
        {
            var processor = new S7Comm.S7CommConversationProcessor();
            foreach (var conversationData in _flowTable.ProcessConversations(_flowTable.ConversationKeys.Where(k => k.FlowKey.DestinationPort == 102), processor))
            {
                WriteObject(conversationData);
            }
            return Task.CompletedTask;
        }

        protected override Task StopProcessingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
