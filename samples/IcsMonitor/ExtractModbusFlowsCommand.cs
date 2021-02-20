using AutoMapper;
using IcsMonitor.Modbus;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Traffix.Hosting.Console;
using Traffix.Providers.PcapFile;
using Traffix.Storage.Faster;

namespace IcsMonitor.Commands
{
    [Cmdlet("Extract", "ModbusFlows")]
    public class ExtractModbusFlowsCommand : AsyncCmdlet
    {
        private FasterConversationTable _flowTable;
        public FileInfo InputFile { get; set; }

        protected override Task BeginProcessingAsync()
        {
            _flowTable = FasterConversationTable.Create("tmp", 100000);

            var frameNumber = 0;
            using (var loader = _flowTable.GetStreamer())
            using (var pcapReader = new SharpPcapReader(InputFile.FullName))
            {
                while (pcapReader.GetNextFrame(out var rawFrame))
                {
                    loader.AddFrame(rawFrame);
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
            var modbusProcessor = new ModbusBiflowProcessor();
            foreach (var modbus in _flowTable.ProcessConversations(_flowTable.ConversationKeys.Where(k => k.FlowKey.DestinationPort == 502), modbusProcessor))
            {
                WriteObject(modbus);
            }
            return Task.CompletedTask;
        }

        protected override Task StopProcessingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
