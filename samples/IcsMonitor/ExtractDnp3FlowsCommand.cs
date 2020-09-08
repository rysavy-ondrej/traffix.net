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
    [Cmdlet("Extract", "Dnp3Flows")]
    public class ExtractDnp3FlowsCommand : AsyncCmdlet
    {
        public string InputFile { get; set; }

        public IModbusAggregator Aggregator { get; set; }

        FasterFlowTable _flowTable;
        private int _framesCount;
        private long _bytesUploaded;
        private readonly Mapper _flowDataMapper;

        public ExtractDnp3FlowsCommand()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<long, DateTime>().ConvertUsing<TicksToDateTimeConverter>();
                cfg.CreateMap<long, TimeSpan>().ConvertUsing<TicksToTimeSpanConverter>();
                    });
            _flowDataMapper = new Mapper(config);
        }

        protected override Task BeginProcessingAsync()
        {
            _flowTable = new FasterFlowTable("tmp");
            using (var stream = File.OpenRead(InputFile))
            {
                _flowTable.LoadFromStream(stream, CancellationTokenSource.Token, null);
            }
            return Task.CompletedTask;
        }

        protected override Task EndProcessingAsync()
        {
            //_flowTable.Reset();
            return Task.CompletedTask;
        }

        protected override Task ProcessRecordAsync()
        {
            var dnp3Processor = new Dnp3BiflowProcessor();
            foreach (var dnp3flowData in _flowTable.ProcessConversations(_flowTable.ConversationKeys.Where(k => k.FlowKey.DestinationPort == 20000), dnp3Processor))
            {
                WriteObject(dnp3flowData);
            }
            return Task.CompletedTask;
        }

        protected override Task StopProcessingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
