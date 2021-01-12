using AutoMapper;
using IcsMonitor.Modbus;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Traffix.Hosting.Console;
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
            _flowTable = FasterConversationTable.Create("tmp");
            using (var stream = InputFile.OpenRead())
            {
                _flowTable.LoadFromStream(stream, CancellationTokenSource.Token, null);
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
