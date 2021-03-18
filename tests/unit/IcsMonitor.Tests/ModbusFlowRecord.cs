using System;
using System.Collections.Generic;
using Traffix.Core.Flows;
using Traffix.DataView;
using Traffix.Processors;

namespace IcsMonitor.Tests
{
    class ModbusFlowRecord
    {
        public FlowKey FlowKey;
        public FlowMetrics Metrics;
        public ModbusData Modbus;
        public ModbusCompact Compact => new ModbusCompact(Modbus); 
    }

    class ModbusFlowRecordDataViewType : DataViewType<ModbusFlowRecord>
    {
        protected override void DefineColumns(DataViewColumnCollection columns) => columns
            .AddComplexColumn(nameof(ModbusFlowRecord.FlowKey), m => m.FlowKey, new FlowKeyDataViewType())
            .AddComplexColumn(nameof(ModbusFlowRecord.Metrics), m => m.Metrics, new FlowMetricsDataViewType())
            .AddComplexColumn(nameof(ModbusFlowRecord.Compact), m => m.Compact, new ModbusCompactDataViewType());
    }

    internal class ModbusCompactDataViewType : DataViewType<ModbusCompact>
    {
        protected override void DefineColumns(DataViewColumnCollection columns) => columns
            .AddColumn(nameof(ModbusCompact.UnitId), m => m.UnitId)
            .AddColumn(nameof(ModbusCompact.ReadRequests), m => m.ReadRequests)
            .AddColumn(nameof(ModbusCompact.WriteRequests), m => m.WriteRequests)
            .AddColumn(nameof(ModbusCompact.DiagnosticRequests), m => m.DiagnosticRequests)
            .AddColumn(nameof(ModbusCompact.OtherRequests), m => m.OtherRequests)
            .AddColumn(nameof(ModbusCompact.UndefinedRequests), m => m.UndefinedRequests)
            .AddColumn(nameof(ModbusCompact.MalformedRequests), m => m.MalformedRequests)
            .AddColumn(nameof(ModbusCompact.ResponsesSuccess), m => m.ResponsesSuccess)
            .AddColumn(nameof(ModbusCompact.ResponsesError), m => m.ResponsesError)
            .AddColumn(nameof(ModbusCompact.MalformedResponses), m => m.MalformedResponses);
    }

    internal class FlowKeyDataViewType : DataViewType<FlowKey>
    {
        protected override void DefineColumns(DataViewColumnCollection columns) => columns
            .AddColumn(nameof(FlowKey.ProtocolType), m => m.ProtocolType)
            .AddColumn(nameof(FlowKey.SourceIpAddress), m => m.SourceIpAddress.ToString())
            .AddColumn(nameof(FlowKey.SourcePort), m => m.SourcePort)
            .AddColumn(nameof(FlowKey.DestinationIpAddress), m => m.DestinationIpAddress.ToString())
            .AddColumn(nameof(FlowKey.DestinationPort), m => m.DestinationPort);
    }


    internal class FlowMetricsDataViewType : DataViewType<FlowMetrics>
    {
        protected override void DefineColumns(DataViewColumnCollection columns) => columns
            .AddColumn(nameof(FlowMetrics.Start), m => m.Start)
            .AddColumn(nameof(FlowMetrics.Duration), m => m.Duration)
            .AddColumn(nameof(FlowMetrics.Packets), m => m.Packets)
            .AddColumn(nameof(FlowMetrics.Octets), m => m.Octets);
    }
}