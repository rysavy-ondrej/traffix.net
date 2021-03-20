using System;
using System.Collections.Generic;
using Traffix.Core.Flows;
using Traffix.DataView;
using Traffix.Processors;

namespace IcsMonitor.Tests
{
    public class ModbusFlowRecord
    {
        public FlowKey FlowKey;
        public FlowMetrics ForwardMetrics;
        public FlowMetrics ReverseMetrics;
        public ModbusData Modbus;
        public ModbusCompact Compact => new ModbusCompact(Modbus);

        public FlowMetrics Metrics => FlowMetrics.Aggregate(ref ForwardMetrics, ref ReverseMetrics);
    }

    class ModbusFlowRecordDataViewType : DataViewType<ModbusFlowRecord>
    {
        protected override void DefineColumns(DataViewColumnCollection columns) => columns
            .AddComplexColumn(nameof(ModbusFlowRecord.FlowKey), m => m.FlowKey, new FlowKeyDataViewType())
            .AddComplexColumn(nameof(ModbusFlowRecord.ForwardMetrics), m => m.ForwardMetrics, new FlowMetricsDataViewType())
            .AddComplexColumn(nameof(ModbusFlowRecord.ReverseMetrics), m => m.ReverseMetrics, new FlowMetricsDataViewType())
            .AddComplexColumn(nameof(ModbusFlowRecord.Compact), m => m.Compact, new ModbusCompactDataViewType());
    }

    internal class ModbusCompactDataViewType : DataViewType<ModbusCompact>
    {
        protected override void DefineColumns(DataViewColumnCollection columns) => columns
            .AddColumn(nameof(ModbusCompact.UnitId), m => m.UnitId)
            .AddColumn(nameof(ModbusCompact.ReadRequests), m => (float)m.ReadRequests)
            .AddColumn(nameof(ModbusCompact.WriteRequests), m => (float)m.WriteRequests)
            .AddColumn(nameof(ModbusCompact.DiagnosticRequests), m => (float)m.DiagnosticRequests)
            .AddColumn(nameof(ModbusCompact.OtherRequests), m => (float)m.OtherRequests)
            .AddColumn(nameof(ModbusCompact.UndefinedRequests), m => (float)m.UndefinedRequests)
            .AddColumn(nameof(ModbusCompact.MalformedRequests), m => (float)m.MalformedRequests)
            .AddColumn(nameof(ModbusCompact.ResponsesSuccess), m => (float)m.ResponsesSuccess)
            .AddColumn(nameof(ModbusCompact.ResponsesError), m => (float)m.ResponsesError)
            .AddColumn(nameof(ModbusCompact.MalformedResponses), m => (float)m.MalformedResponses);
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
            .AddColumn(nameof(FlowMetrics.Duration), m => (float)m.Duration.TotalSeconds)
            .AddColumn(nameof(FlowMetrics.Packets), m => (float)m.Packets)
            .AddColumn(nameof(FlowMetrics.Octets), m => (float)m.Octets);
    }
}