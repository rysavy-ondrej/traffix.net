<a name='assembly'></a>
# IcsMonitor

## Contents

- [Compact](#T-IcsMonitor-Modbus-ModbusFlowData-Compact 'IcsMonitor.Modbus.ModbusFlowData.Compact')
- [Complete](#T-IcsMonitor-Modbus-ModbusFlowData-Complete 'IcsMonitor.Modbus.ModbusFlowData.Complete')
- [ConversationRecordExtensions](#T-IcsMonitor-ConversationRecordExtensions 'IcsMonitor.ConversationRecordExtensions')
  - [_membersInfoCache](#F-IcsMonitor-ConversationRecordExtensions-_membersInfoCache 'IcsMonitor.ConversationRecordExtensions._membersInfoCache')
  - [CreateMembersInfo(type)](#M-IcsMonitor-ConversationRecordExtensions-CreateMembersInfo-System-Type- 'IcsMonitor.ConversationRecordExtensions.CreateMembersInfo(System.Type)')
  - [GetColumn(name,columnType,values)](#M-IcsMonitor-ConversationRecordExtensions-GetColumn-System-String,System-Type,System-Collections-IEnumerable- 'IcsMonitor.ConversationRecordExtensions.GetColumn(System.String,System.Type,System.Collections.IEnumerable)')
  - [GetMembersInfo(type)](#M-IcsMonitor-ConversationRecordExtensions-GetMembersInfo-System-Type- 'IcsMonitor.ConversationRecordExtensions.GetMembersInfo(System.Type)')
  - [ToDataFrame\`\`1(records)](#M-IcsMonitor-ConversationRecordExtensions-ToDataFrame``1-System-Collections-Generic-IEnumerable{IcsMonitor-ConversationRecord{``0}}- 'IcsMonitor.ConversationRecordExtensions.ToDataFrame``1(System.Collections.Generic.IEnumerable{IcsMonitor.ConversationRecord{``0}})')
  - [ToDictionary\`\`1()](#M-IcsMonitor-ConversationRecordExtensions-ToDictionary``1-IcsMonitor-ConversationRecord{``0}- 'IcsMonitor.ConversationRecordExtensions.ToDictionary``1(IcsMonitor.ConversationRecord{``0})')
  - [WriteCsv(dataFrame,path,separator,header,encoding,cultureInfo)](#M-IcsMonitor-ConversationRecordExtensions-WriteCsv-Microsoft-Data-Analysis-DataFrame,System-String,System-Char,System-Boolean,System-Text-Encoding,System-Globalization-CultureInfo- 'IcsMonitor.ConversationRecordExtensions.WriteCsv(Microsoft.Data.Analysis.DataFrame,System.String,System.Char,System.Boolean,System.Text.Encoding,System.Globalization.CultureInfo)')
  - [WriteCsv(dataFrame,csvStream,separator,header,encoding,cultureInfo)](#M-IcsMonitor-ConversationRecordExtensions-WriteCsv-Microsoft-Data-Analysis-DataFrame,System-IO-Stream,System-Char,System-Boolean,System-Text-Encoding,System-Globalization-CultureInfo- 'IcsMonitor.ConversationRecordExtensions.WriteCsv(Microsoft.Data.Analysis.DataFrame,System.IO.Stream,System.Char,System.Boolean,System.Text.Encoding,System.Globalization.CultureInfo)')
- [ConversationRecord\`1](#T-IcsMonitor-ConversationRecord`1 'IcsMonitor.ConversationRecord`1')
  - [Data](#F-IcsMonitor-ConversationRecord`1-Data 'IcsMonitor.ConversationRecord`1.Data')
  - [ForwardMetrics](#F-IcsMonitor-ConversationRecord`1-ForwardMetrics 'IcsMonitor.ConversationRecord`1.ForwardMetrics')
  - [Key](#F-IcsMonitor-ConversationRecord`1-Key 'IcsMonitor.ConversationRecord`1.Key')
  - [Label](#F-IcsMonitor-ConversationRecord`1-Label 'IcsMonitor.ConversationRecord`1.Label')
  - [OriginalFlowsPresent](#F-IcsMonitor-ConversationRecord`1-OriginalFlowsPresent 'IcsMonitor.ConversationRecord`1.OriginalFlowsPresent')
  - [ReverseMetrics](#F-IcsMonitor-ConversationRecord`1-ReverseMetrics 'IcsMonitor.ConversationRecord`1.ReverseMetrics')
- [ConversationTable\`1](#T-IcsMonitor-ConversationTable`1 'IcsMonitor.ConversationTable`1')
  - [#ctor(collection)](#M-IcsMonitor-ConversationTable`1-#ctor-System-Collections-Generic-IEnumerable{IcsMonitor-ConversationRecord{`0}}- 'IcsMonitor.ConversationTable`1.#ctor(System.Collections.Generic.IEnumerable{IcsMonitor.ConversationRecord{`0}})')
  - [Interval](#F-IcsMonitor-ConversationTable`1-Interval 'IcsMonitor.ConversationTable`1.Interval')
  - [StartTime](#F-IcsMonitor-ConversationTable`1-StartTime 'IcsMonitor.ConversationTable`1.StartTime')
  - [AggregateConversations\`\`2(conversations,keySelector,accumulator,aggregator)](#M-IcsMonitor-ConversationTable`1-AggregateConversations``2-System-Func{IcsMonitor-ConversationRecord{`0},``1},System-Func{IcsMonitor-ConversationRecord{`0},``0},System-Func{``0,``0,``0}- 'IcsMonitor.ConversationTable`1.AggregateConversations``2(System.Func{IcsMonitor.ConversationRecord{`0},``1},System.Func{IcsMonitor.ConversationRecord{`0},``0},System.Func{``0,``0,``0})')
- [DataPoint](#T-IcsMonitor-ModbusDataModel-DataPoint 'IcsMonitor.ModbusDataModel.DataPoint')
  - [Features](#F-IcsMonitor-ModbusDataModel-DataPoint-Features 'IcsMonitor.ModbusDataModel.DataPoint.Features')
  - [FlowKey](#F-IcsMonitor-ModbusDataModel-DataPoint-FlowKey 'IcsMonitor.ModbusDataModel.DataPoint.FlowKey')
- [Dnp3FlowData](#T-IcsMonitor-Modbus-Dnp3FlowData 'IcsMonitor.Modbus.Dnp3FlowData')
  - [ApplicationControlRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-ApplicationControlRequests 'IcsMonitor.Modbus.Dnp3FlowData.ApplicationControlRequests')
  - [ConfigurationRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-ConfigurationRequests 'IcsMonitor.Modbus.Dnp3FlowData.ConfigurationRequests')
  - [ControlRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-ControlRequests 'IcsMonitor.Modbus.Dnp3FlowData.ControlRequests')
  - [FreezeRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-FreezeRequests 'IcsMonitor.Modbus.Dnp3FlowData.FreezeRequests')
  - [MalformedRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-MalformedRequests 'IcsMonitor.Modbus.Dnp3FlowData.MalformedRequests')
  - [MalformedResponses](#P-IcsMonitor-Modbus-Dnp3FlowData-MalformedResponses 'IcsMonitor.Modbus.Dnp3FlowData.MalformedResponses')
  - [OtherOperationRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-OtherOperationRequests 'IcsMonitor.Modbus.Dnp3FlowData.OtherOperationRequests')
  - [OtherResponses](#P-IcsMonitor-Modbus-Dnp3FlowData-OtherResponses 'IcsMonitor.Modbus.Dnp3FlowData.OtherResponses')
  - [ReservedRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-ReservedRequests 'IcsMonitor.Modbus.Dnp3FlowData.ReservedRequests')
  - [TimeSynchronizationRequests](#P-IcsMonitor-Modbus-Dnp3FlowData-TimeSynchronizationRequests 'IcsMonitor.Modbus.Dnp3FlowData.TimeSynchronizationRequests')
- [DynamicDictionary](#T-IcsMonitor-DynamicDictionary 'IcsMonitor.DynamicDictionary')
- [Extended](#T-IcsMonitor-Modbus-ModbusFlowData-Extended 'IcsMonitor.Modbus.ModbusFlowData.Extended')
- [IcsDataset\`1](#T-IcsMonitor-Interactive-IcsDataset`1 'IcsMonitor.Interactive.IcsDataset`1')
- [Interactive](#T-IcsMonitor-Interactive 'IcsMonitor.Interactive')
  - [#ctor(rootDirectory)](#M-IcsMonitor-Interactive-#ctor-System-IO-DirectoryInfo- 'IcsMonitor.Interactive.#ctor(System.IO.DirectoryInfo)')
  - [CleanUp()](#M-IcsMonitor-Interactive-CleanUp 'IcsMonitor.Interactive.CleanUp')
  - [ComputeDataset\`\`1(inputFile,timeInterval,processor)](#M-IcsMonitor-Interactive-ComputeDataset``1-System-String,System-TimeSpan,System-Func{Traffix-Providers-PcapFile-RawFrame,System-Boolean},Traffix-Storage-Faster-IConversationProcessor{IcsMonitor-ConversationRecord{``0}}- 'IcsMonitor.Interactive.ComputeDataset``1(System.String,System.TimeSpan,System.Func{Traffix.Providers.PcapFile.RawFrame,System.Boolean},Traffix.Storage.Faster.IConversationProcessor{IcsMonitor.ConversationRecord{``0}})')
  - [ComputeModbusDataset(inputFile,timeInterval)](#M-IcsMonitor-Interactive-ComputeModbusDataset-System-String,System-TimeSpan- 'IcsMonitor.Interactive.ComputeModbusDataset(System.String,System.TimeSpan)')
  - [ContainsPacket(table,conversationKey,packet)](#M-IcsMonitor-Interactive-ContainsPacket-Traffix-Storage-Faster-FasterConversationTable,Traffix-Core-Flows-FlowKey,PacketDotNet-Packet- 'IcsMonitor.Interactive.ContainsPacket(Traffix.Storage.Faster.FasterConversationTable,Traffix.Core.Flows.FlowKey,PacketDotNet.Packet)')
  - [CreateCaptureFileReader(path,useManaged)](#M-IcsMonitor-Interactive-CreateCaptureFileReader-System-String,System-Boolean- 'IcsMonitor.Interactive.CreateCaptureFileReader(System.String,System.Boolean)')
  - [CreateConversationTable(frames,conversationTablePath,token)](#M-IcsMonitor-Interactive-CreateConversationTable-System-Collections-Generic-IEnumerable{Traffix-Providers-PcapFile-RawFrame},System-String,System-Nullable{System-Threading-CancellationToken}- 'IcsMonitor.Interactive.CreateConversationTable(System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame},System.String,System.Nullable{System.Threading.CancellationToken})')
  - [CreateConversationTables(frames,conversationTablePath,timeInterval,token)](#M-IcsMonitor-Interactive-CreateConversationTables-System-Collections-Generic-IEnumerable{Traffix-Providers-PcapFile-RawFrame},System-String,System-TimeSpan,System-Nullable{System-Threading-CancellationToken}- 'IcsMonitor.Interactive.CreateConversationTables(System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame},System.String,System.TimeSpan,System.Nullable{System.Threading.CancellationToken})')
  - [GetConversations\`\`1(table,processor)](#M-IcsMonitor-Interactive-GetConversations``1-Traffix-Storage-Faster-FasterConversationTable,Traffix-Storage-Faster-IConversationProcessor{``0}- 'IcsMonitor.Interactive.GetConversations``1(Traffix.Storage.Faster.FasterConversationTable,Traffix.Storage.Faster.IConversationProcessor{``0})')
  - [GetNextFrames(reader,count)](#M-IcsMonitor-Interactive-GetNextFrames-Traffix-Providers-PcapFile-ICaptureFileReader,System-Int32- 'IcsMonitor.Interactive.GetNextFrames(Traffix.Providers.PcapFile.ICaptureFileReader,System.Int32)')
  - [GetPackets(table)](#M-IcsMonitor-Interactive-GetPackets-Traffix-Storage-Faster-FasterConversationTable- 'IcsMonitor.Interactive.GetPackets(Traffix.Storage.Faster.FasterConversationTable)')
  - [LoadFromTsv(context,path)](#M-IcsMonitor-Interactive-LoadFromTsv-Microsoft-ML-MLContext,System-String- 'IcsMonitor.Interactive.LoadFromTsv(Microsoft.ML.MLContext,System.String)')
  - [SaveToTsv(context,dataview,path)](#M-IcsMonitor-Interactive-SaveToTsv-Microsoft-ML-MLContext,Microsoft-ML-IDataView,System-String- 'IcsMonitor.Interactive.SaveToTsv(Microsoft.ML.MLContext,Microsoft.ML.IDataView,System.String)')
  - [WriteToFile(frames,path)](#M-IcsMonitor-Interactive-WriteToFile-System-Collections-Generic-IEnumerable{Traffix-Providers-PcapFile-RawFrame},System-String- 'IcsMonitor.Interactive.WriteToFile(System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame},System.String)')
- [MapAllPublicProperties\`1](#T-IcsMonitor-OutputWriter-MapAllPublicProperties`1 'IcsMonitor.OutputWriter.MapAllPublicProperties`1')
- [PacketHelper](#T-IcsMonitor-PacketHelper 'IcsMonitor.PacketHelper')
  - [GetRawFrame(packet,ticks)](#M-IcsMonitor-PacketHelper-GetRawFrame-PacketDotNet-Packet,System-Int64- 'IcsMonitor.PacketHelper.GetRawFrame(PacketDotNet.Packet,System.Int64)')
  - [Segments(packets)](#M-IcsMonitor-PacketHelper-Segments-System-Collections-Generic-IEnumerable{PacketDotNet-Packet}- 'IcsMonitor.PacketHelper.Segments(System.Collections.Generic.IEnumerable{PacketDotNet.Packet})')
  - [Segments\`\`1(packets)](#M-IcsMonitor-PacketHelper-Segments``1-System-Collections-Generic-IEnumerable{System-ValueTuple{PacketDotNet-Packet,``0}}- 'IcsMonitor.PacketHelper.Segments``1(System.Collections.Generic.IEnumerable{System.ValueTuple{PacketDotNet.Packet,``0}})')
  - [TryGetSegment(packet,tcp)](#M-IcsMonitor-PacketHelper-TryGetSegment-PacketDotNet-Packet,PacketDotNet-TcpPacket@- 'IcsMonitor.PacketHelper.TryGetSegment(PacketDotNet.Packet,PacketDotNet.TcpPacket@)')
- [Prediction](#T-IcsMonitor-ModbusDataModel-Prediction 'IcsMonitor.ModbusDataModel.Prediction')
  - [ClusterId](#P-IcsMonitor-ModbusDataModel-Prediction-ClusterId 'IcsMonitor.ModbusDataModel.Prediction.ClusterId')
  - [Distance](#P-IcsMonitor-ModbusDataModel-Prediction-Distance 'IcsMonitor.ModbusDataModel.Prediction.Distance')
  - [Distances](#P-IcsMonitor-ModbusDataModel-Prediction-Distances 'IcsMonitor.ModbusDataModel.Prediction.Distances')
  - [FlowKey](#P-IcsMonitor-ModbusDataModel-Prediction-FlowKey 'IcsMonitor.ModbusDataModel.Prediction.FlowKey')
  - [Threshold](#P-IcsMonitor-ModbusDataModel-Prediction-Threshold 'IcsMonitor.ModbusDataModel.Prediction.Threshold')
  - [Threshold2](#P-IcsMonitor-ModbusDataModel-Prediction-Threshold2 'IcsMonitor.ModbusDataModel.Prediction.Threshold2')
  - [Threshold3](#P-IcsMonitor-ModbusDataModel-Prediction-Threshold3 'IcsMonitor.ModbusDataModel.Prediction.Threshold3')
  - [Variance](#P-IcsMonitor-ModbusDataModel-Prediction-Variance 'IcsMonitor.ModbusDataModel.Prediction.Variance')
- [Program](#T-IcsMonitor-Program 'IcsMonitor.Program')
  - [ExportTcpPayload(inputFile,port,outputFile)](#M-IcsMonitor-Program-ExportTcpPayload-System-String,System-Nullable{System-Int32},System-String- 'IcsMonitor.Program.ExportTcpPayload(System.String,System.Nullable{System.Int32},System.String)')
  - [TrainModbusModel(inputFile,outputFile,numberOfClusters)](#M-IcsMonitor-Program-TrainModbusModel-System-String,System-String,System-Int32- 'IcsMonitor.Program.TrainModbusModel(System.String,System.String,System.Int32)')
- [S7CommConversationData](#T-IcsMonitor-S7Comm-S7CommConversationData 'IcsMonitor.S7Comm.S7CommConversationData')
  - [AckControlCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-AckControlCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.AckControlCountFieldNumber')
  - [AckDownloadCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-AckDownloadCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.AckDownloadCountFieldNumber')
  - [AckReadVarCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.AckReadVarCountFieldNumber')
  - [AckReadVarSuccessCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarSuccessCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.AckReadVarSuccessCountFieldNumber')
  - [AckUploadCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-AckUploadCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.AckUploadCountFieldNumber')
  - [AckWriteSuccessCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-AckWriteSuccessCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.AckWriteSuccessCountFieldNumber')
  - [AckWriteVarCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-AckWriteVarCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.AckWriteVarCountFieldNumber')
  - [DataLengthSumFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-DataLengthSumFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.DataLengthSumFieldNumber')
  - [ErrorInResponseCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-ErrorInResponseCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.ErrorInResponseCountFieldNumber')
  - [JobControlCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-JobControlCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.JobControlCountFieldNumber')
  - [JobDownloadCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-JobDownloadCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.JobDownloadCountFieldNumber')
  - [JobReadVarCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.JobReadVarCountFieldNumber')
  - [JobReadVarItemCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarItemCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.JobReadVarItemCountFieldNumber')
  - [JobUploadCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-JobUploadCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.JobUploadCountFieldNumber')
  - [JobWriteVarCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.JobWriteVarCountFieldNumber')
  - [JobWriteVarItemCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarItemCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.JobWriteVarItemCountFieldNumber')
  - [ParamLengthSumFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-ParamLengthSumFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.ParamLengthSumFieldNumber')
  - [UnknownRequestCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-UnknownRequestCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.UnknownRequestCountFieldNumber')
  - [UnknownResponseCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-UnknownResponseCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.UnknownResponseCountFieldNumber')
  - [UserDataRequestCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-UserDataRequestCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.UserDataRequestCountFieldNumber')
  - [UserDataResponseCountFieldNumber](#F-IcsMonitor-S7Comm-S7CommConversationData-UserDataResponseCountFieldNumber 'IcsMonitor.S7Comm.S7CommConversationData.UserDataResponseCountFieldNumber')
  - [AckControlCount](#P-IcsMonitor-S7Comm-S7CommConversationData-AckControlCount 'IcsMonitor.S7Comm.S7CommConversationData.AckControlCount')
  - [AckDownloadCount](#P-IcsMonitor-S7Comm-S7CommConversationData-AckDownloadCount 'IcsMonitor.S7Comm.S7CommConversationData.AckDownloadCount')
  - [AckReadVarCount](#P-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarCount 'IcsMonitor.S7Comm.S7CommConversationData.AckReadVarCount')
  - [AckReadVarSuccessCount](#P-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarSuccessCount 'IcsMonitor.S7Comm.S7CommConversationData.AckReadVarSuccessCount')
  - [AckUploadCount](#P-IcsMonitor-S7Comm-S7CommConversationData-AckUploadCount 'IcsMonitor.S7Comm.S7CommConversationData.AckUploadCount')
  - [AckWriteSuccessCount](#P-IcsMonitor-S7Comm-S7CommConversationData-AckWriteSuccessCount 'IcsMonitor.S7Comm.S7CommConversationData.AckWriteSuccessCount')
  - [AckWriteVarCount](#P-IcsMonitor-S7Comm-S7CommConversationData-AckWriteVarCount 'IcsMonitor.S7Comm.S7CommConversationData.AckWriteVarCount')
  - [DataLengthSum](#P-IcsMonitor-S7Comm-S7CommConversationData-DataLengthSum 'IcsMonitor.S7Comm.S7CommConversationData.DataLengthSum')
  - [ErrorInResponseCount](#P-IcsMonitor-S7Comm-S7CommConversationData-ErrorInResponseCount 'IcsMonitor.S7Comm.S7CommConversationData.ErrorInResponseCount')
  - [JobControlCount](#P-IcsMonitor-S7Comm-S7CommConversationData-JobControlCount 'IcsMonitor.S7Comm.S7CommConversationData.JobControlCount')
  - [JobDownloadCount](#P-IcsMonitor-S7Comm-S7CommConversationData-JobDownloadCount 'IcsMonitor.S7Comm.S7CommConversationData.JobDownloadCount')
  - [JobReadVarCount](#P-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarCount 'IcsMonitor.S7Comm.S7CommConversationData.JobReadVarCount')
  - [JobReadVarItemCount](#P-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarItemCount 'IcsMonitor.S7Comm.S7CommConversationData.JobReadVarItemCount')
  - [JobUploadCount](#P-IcsMonitor-S7Comm-S7CommConversationData-JobUploadCount 'IcsMonitor.S7Comm.S7CommConversationData.JobUploadCount')
  - [JobWriteVarCount](#P-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarCount 'IcsMonitor.S7Comm.S7CommConversationData.JobWriteVarCount')
  - [JobWriteVarItemCount](#P-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarItemCount 'IcsMonitor.S7Comm.S7CommConversationData.JobWriteVarItemCount')
  - [ParamLengthSum](#P-IcsMonitor-S7Comm-S7CommConversationData-ParamLengthSum 'IcsMonitor.S7Comm.S7CommConversationData.ParamLengthSum')
  - [UnknownRequestCount](#P-IcsMonitor-S7Comm-S7CommConversationData-UnknownRequestCount 'IcsMonitor.S7Comm.S7CommConversationData.UnknownRequestCount')
  - [UnknownResponseCount](#P-IcsMonitor-S7Comm-S7CommConversationData-UnknownResponseCount 'IcsMonitor.S7Comm.S7CommConversationData.UnknownResponseCount')
  - [UserDataRequestCount](#P-IcsMonitor-S7Comm-S7CommConversationData-UserDataRequestCount 'IcsMonitor.S7Comm.S7CommConversationData.UserDataRequestCount')
  - [UserDataResponseCount](#P-IcsMonitor-S7Comm-S7CommConversationData-UserDataResponseCount 'IcsMonitor.S7Comm.S7CommConversationData.UserDataResponseCount')
- [S7CommConversationDataReflection](#T-IcsMonitor-S7Comm-S7CommConversationDataReflection 'IcsMonitor.S7Comm.S7CommConversationDataReflection')
  - [Descriptor](#P-IcsMonitor-S7Comm-S7CommConversationDataReflection-Descriptor 'IcsMonitor.S7Comm.S7CommConversationDataReflection.Descriptor')
- [S7CommConversationProcessor](#T-IcsMonitor-S7Comm-S7CommConversationProcessor 'IcsMonitor.S7Comm.S7CommConversationProcessor')
  - [UpdateConversation(conversation,packet,direction)](#M-IcsMonitor-S7Comm-S7CommConversationProcessor-UpdateConversation-IcsMonitor-S7Comm-S7CommConversationData,Traffix-Storage-Faster-FrameMetadata,PacketDotNet-Packet,Traffix-Storage-Faster-FlowDirection- 'IcsMonitor.S7Comm.S7CommConversationProcessor.UpdateConversation(IcsMonitor.S7Comm.S7CommConversationData,Traffix.Storage.Faster.FrameMetadata,PacketDotNet.Packet,Traffix.Storage.Faster.FlowDirection)')

<a name='T-IcsMonitor-Modbus-ModbusFlowData-Compact'></a>
## Compact `type`

##### Namespace

IcsMonitor.Modbus.ModbusFlowData

##### Summary

A MODBUS flow extension that can be extracted from the simple MODBUS parser (parsing individual functions is not necessary).

<a name='T-IcsMonitor-Modbus-ModbusFlowData-Complete'></a>
## Complete `type`

##### Namespace

IcsMonitor.Modbus.ModbusFlowData

##### Summary

A MODBUS flow extension.

<a name='T-IcsMonitor-ConversationRecordExtensions'></a>
## ConversationRecordExtensions `type`

##### Namespace

IcsMonitor

##### Summary

Implements various extension methods for manipulation with [ConversationRecord\`1](#T-IcsMonitor-ConversationRecord`1 'IcsMonitor.ConversationRecord`1'),
[DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame'), and other types to support an intergation with ML.NET.

<a name='F-IcsMonitor-ConversationRecordExtensions-_membersInfoCache'></a>
### _membersInfoCache `constants`

##### Summary

Contains cached member information objects.

<a name='M-IcsMonitor-ConversationRecordExtensions-CreateMembersInfo-System-Type-'></a>
### CreateMembersInfo(type) `method`

##### Summary

Gets the collection of member information objects for the given `type`.

##### Returns

A collection of member information objects that each consists of  member path,  member type and  accessor function.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| type | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | The type for which to get columns. |

<a name='M-IcsMonitor-ConversationRecordExtensions-GetColumn-System-String,System-Type,System-Collections-IEnumerable-'></a>
### GetColumn(name,columnType,values) `method`

##### Summary

Gets a single column of [DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame') including all associated values.



As the column can only be primitive type, the method performs necessary conversions.

##### Returns

The new [DataFrameColumn](#T-Microsoft-Data-Analysis-DataFrameColumn 'Microsoft.Data.Analysis.DataFrameColumn')for the parameters specified.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The column name. |
| columnType | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | The column type. |
| values | [System.Collections.IEnumerable](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.IEnumerable 'System.Collections.IEnumerable') | The collection of values that must be of the `columnType`. |

<a name='M-IcsMonitor-ConversationRecordExtensions-GetMembersInfo-System-Type-'></a>
### GetMembersInfo(type) `method`

##### Summary

Gets the member information collection for the given type.

The collection can be used to access the members in an object of the corresponding type.

##### Returns

A collection of member information objects for the given `type`.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| type | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | The type of conversation records. |

<a name='M-IcsMonitor-ConversationRecordExtensions-ToDataFrame``1-System-Collections-Generic-IEnumerable{IcsMonitor-ConversationRecord{``0}}-'></a>
### ToDataFrame\`\`1(records) `method`

##### Summary

Converts an enumerable of conversation records to [DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame'). The [DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame')
is a high performance memory store for data sets.

The [DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame') implements [IDataView](#T-Microsoft-ML-IDataView 'Microsoft.ML.IDataView') interface 
which is consumed by ML.NET pipelines.

##### Returns

The new [DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame') object representing the source records.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| records | [System.Collections.Generic.IEnumerable{IcsMonitor.ConversationRecord{\`\`0}}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{IcsMonitor.ConversationRecord{``0}}') | The enumerable of records. |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| T | The type of conversation record. |

<a name='M-IcsMonitor-ConversationRecordExtensions-ToDictionary``1-IcsMonitor-ConversationRecord{``0}-'></a>
### ToDictionary\`\`1() `method`

##### Summary

Gets the dictionary that represents a flatten version of the current record.

It can be used to flat the conversation record and to create dynamic object from it.

##### Returns

A dictionary of fields and values for the passed conversation record.

##### Parameters

This method has no parameters.

<a name='M-IcsMonitor-ConversationRecordExtensions-WriteCsv-Microsoft-Data-Analysis-DataFrame,System-String,System-Char,System-Boolean,System-Text-Encoding,System-Globalization-CultureInfo-'></a>
### WriteCsv(dataFrame,path,separator,header,encoding,cultureInfo) `method`

##### Summary

Writes a DataFrame into a CSV.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| dataFrame | [Microsoft.Data.Analysis.DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame') | [DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame') |
| path | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | CSV file path |
| separator | [System.Char](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Char 'System.Char') | column separator |
| header | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | has a header or not |
| encoding | [System.Text.Encoding](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Encoding 'System.Text.Encoding') | The character encoding. Defaults to UTF8 if not specified |
| cultureInfo | [System.Globalization.CultureInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Globalization.CultureInfo 'System.Globalization.CultureInfo') | culture info for formatting values |

<a name='M-IcsMonitor-ConversationRecordExtensions-WriteCsv-Microsoft-Data-Analysis-DataFrame,System-IO-Stream,System-Char,System-Boolean,System-Text-Encoding,System-Globalization-CultureInfo-'></a>
### WriteCsv(dataFrame,csvStream,separator,header,encoding,cultureInfo) `method`

##### Summary

Writes a DataFrame into a CSV.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| dataFrame | [Microsoft.Data.Analysis.DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame') | [DataFrame](#T-Microsoft-Data-Analysis-DataFrame 'Microsoft.Data.Analysis.DataFrame') |
| csvStream | [System.IO.Stream](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.IO.Stream 'System.IO.Stream') | stream of CSV data to be write out |
| separator | [System.Char](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Char 'System.Char') | column separator |
| header | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | has a header or not |
| encoding | [System.Text.Encoding](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Encoding 'System.Text.Encoding') | the character encoding. Defaults to UTF8 if not specified |
| cultureInfo | [System.Globalization.CultureInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Globalization.CultureInfo 'System.Globalization.CultureInfo') | culture info for formatting values |

<a name='T-IcsMonitor-ConversationRecord`1'></a>
## ConversationRecord\`1 `type`

##### Namespace

IcsMonitor

##### Summary

The record produced by conversation processor. It contains a fixed part and 
processor specific data.

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TData | The data specific to flow processor. |

<a name='F-IcsMonitor-ConversationRecord`1-Data'></a>
### Data `constants`

##### Summary

The conversation data.

<a name='F-IcsMonitor-ConversationRecord`1-ForwardMetrics'></a>
### ForwardMetrics `constants`

##### Summary

The forward flow metrics.

<a name='F-IcsMonitor-ConversationRecord`1-Key'></a>
### Key `constants`

##### Summary

The conversation key.

<a name='F-IcsMonitor-ConversationRecord`1-Label'></a>
### Label `constants`

##### Summary

The label is mostly used for classification.

<a name='F-IcsMonitor-ConversationRecord`1-OriginalFlowsPresent'></a>
### OriginalFlowsPresent `constants`

##### Summary

A number of flows used to create a converation record.
Usually, forward and reverse flows are used,ie., it equals 2.



The conversation record may be an aggregation of multiple flows.

<a name='F-IcsMonitor-ConversationRecord`1-ReverseMetrics'></a>
### ReverseMetrics `constants`

##### Summary

Teh reverse flow metrics.

<a name='T-IcsMonitor-ConversationTable`1'></a>
## ConversationTable\`1 `type`

##### Namespace

IcsMonitor

##### Summary

A memory representation of the conversation table.

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TData | The data type of the conversation. |

<a name='M-IcsMonitor-ConversationTable`1-#ctor-System-Collections-Generic-IEnumerable{IcsMonitor-ConversationRecord{`0}}-'></a>
### #ctor(collection) `constructor`

##### Summary

Creates a new conversation table from the given collection of conversations.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| collection | [System.Collections.Generic.IEnumerable{IcsMonitor.ConversationRecord{\`0}}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{IcsMonitor.ConversationRecord{`0}}') |  |

<a name='F-IcsMonitor-ConversationTable`1-Interval'></a>
### Interval `constants`

##### Summary

The interval defining the duration of the conversation table.

<a name='F-IcsMonitor-ConversationTable`1-StartTime'></a>
### StartTime `constants`

##### Summary

Start time of the conversation table.

<a name='M-IcsMonitor-ConversationTable`1-AggregateConversations``2-System-Func{IcsMonitor-ConversationRecord{`0},``1},System-Func{IcsMonitor-ConversationRecord{`0},``0},System-Func{``0,``0,``0}-'></a>
### AggregateConversations\`\`2(conversations,keySelector,accumulator,aggregator) `method`

##### Summary

Aggregates conversations by grouping conversations using `keySelector` and then by applying `aggregator` function 
to all conversations in the group. The result is a collection of aggregated conversations.

##### Returns

A collection of conversations. Number of returned items equals to a number of groups created by `keySelector`.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| conversations | [System.Func{IcsMonitor.ConversationRecord{\`0},\`\`1}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{IcsMonitor.ConversationRecord{`0},``1}') | The input collection of conversations. |
| keySelector | [System.Func{IcsMonitor.ConversationRecord{\`0},\`\`0}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{IcsMonitor.ConversationRecord{`0},``0}') | The selector of key fields for the aggregated conversations. |
| accumulator | [System.Func{\`\`0,\`\`0,\`\`0}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{``0,``0,``0}') | The initial value of the aggregation. |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| Tout | The type of the output. |
| TKey | The type of the keys. |

<a name='T-IcsMonitor-ModbusDataModel-DataPoint'></a>
## DataPoint `type`

##### Namespace

IcsMonitor.ModbusDataModel

##### Summary

Represents a single data point usable as an input for ML tasks.

<a name='F-IcsMonitor-ModbusDataModel-DataPoint-Features'></a>
### Features `constants`

##### Summary

Vector of features used by ML algorithms.

<a name='F-IcsMonitor-ModbusDataModel-DataPoint-FlowKey'></a>
### FlowKey `constants`

##### Summary

The flow key used for referencing the original flow.

<a name='T-IcsMonitor-Modbus-Dnp3FlowData'></a>
## Dnp3FlowData `type`

##### Namespace

IcsMonitor.Modbus

##### Summary

The extended flow data type for DNP3 protocol.
It represents both request and response directions of communication.



Because the communication parties have well-defined roles, 
we can distinguish between requests and responses. 
The requests are grouped according to function classes to 
keep the number of fields relatively small.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-ApplicationControlRequests'></a>
### ApplicationControlRequests `property`

##### Summary

Aggregates functions 13-18.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-ConfigurationRequests'></a>
### ConfigurationRequests `property`

##### Summary

Aggregates configuratino related functions (19-22).

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-ControlRequests'></a>
### ControlRequests `property`

##### Summary

Aggregates function codes 3-6.



Functions: Dnp3Select = 3, Dnp3Operate = 4,
Dnp3DirOperate = 5, and Dnp3DirOperateNoResp = 6.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-FreezeRequests'></a>
### FreezeRequests `property`

##### Summary

Aggregates all freeze functions (7-12).

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-MalformedRequests'></a>
### MalformedRequests `property`

##### Summary

Represents count of invalid DNP3 packets.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-MalformedResponses'></a>
### MalformedResponses `property`

##### Summary

Represents count of invalid DNP3 packets.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-OtherOperationRequests'></a>
### OtherOperationRequests `property`

##### Summary

Aggregates all other functions > 128.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-OtherResponses'></a>
### OtherResponses `property`

##### Summary

Represents count of DNP3 response, which function code is 
not 0, 129, or 130.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-ReservedRequests'></a>
### ReservedRequests `property`

##### Summary

Aggregates functions in range 24-128.

<a name='P-IcsMonitor-Modbus-Dnp3FlowData-TimeSynchronizationRequests'></a>
### TimeSynchronizationRequests `property`

##### Summary

Represents time sync requests, i.e., fc=23.

<a name='T-IcsMonitor-DynamicDictionary'></a>
## DynamicDictionary `type`

##### Namespace

IcsMonitor

##### Summary

A simple dictionary-based implementation of dynamic object.

<a name='T-IcsMonitor-Modbus-ModbusFlowData-Extended'></a>
## Extended `type`

##### Namespace

IcsMonitor.Modbus.ModbusFlowData

##### Summary

A MODBUS flow extension that can be extracted from the simple MODBUS parser (parsing individual functions is not necessary).

<a name='T-IcsMonitor-Interactive-IcsDataset`1'></a>
## IcsDataset\`1 `type`

##### Namespace

IcsMonitor.Interactive

##### Summary

The class collects necessary information of the single ICS dataset.



To use data set with ML.NET it is possible to several extensions in [ConversationRecordExtensions](#T-IcsMonitor-ConversationRecordExtensions 'IcsMonitor.ConversationRecordExtensions') class.

<a name='T-IcsMonitor-Interactive'></a>
## Interactive `type`

##### Namespace

IcsMonitor

##### Summary

Interactive class exposes various API methods usable in C# interactive sessions.

<a name='M-IcsMonitor-Interactive-#ctor-System-IO-DirectoryInfo-'></a>
### #ctor(rootDirectory) `constructor`

##### Summary

Creates an interactive object.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| rootDirectory | [System.IO.DirectoryInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.IO.DirectoryInfo 'System.IO.DirectoryInfo') | Root directory. It can be used to access data. |

<a name='M-IcsMonitor-Interactive-CleanUp'></a>
### CleanUp() `method`

##### Summary

Cleans  the environment by deleting objects created during [Interactive](#T-IcsMonitor-Interactive 'IcsMonitor.Interactive') lifetime.

##### Parameters

This method has no parameters.

<a name='M-IcsMonitor-Interactive-ComputeDataset``1-System-String,System-TimeSpan,System-Func{Traffix-Providers-PcapFile-RawFrame,System-Boolean},Traffix-Storage-Faster-IConversationProcessor{IcsMonitor-ConversationRecord{``0}}-'></a>
### ComputeDataset\`\`1(inputFile,timeInterval,processor) `method`

##### Summary

Creates the ICS dataset from the source PCAP file given by `inputFile` name.



This is all-in-one method that reads pcap file, computes conversation tables and extract ICS 
conversations using provided processor.

##### Returns

The dataset computed for the input data using the given conversation processor.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| inputFile | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The source pcap file. |
| timeInterval | [System.TimeSpan](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.TimeSpan 'System.TimeSpan') | The time interval for conversation window. |
| processor | [System.Func{Traffix.Providers.PcapFile.RawFrame,System.Boolean}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Func 'System.Func{Traffix.Providers.PcapFile.RawFrame,System.Boolean}') | The conversation processor to produce results in the dataset. |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TFlowData | The type of the dataset. |

<a name='M-IcsMonitor-Interactive-ComputeModbusDataset-System-String,System-TimeSpan-'></a>
### ComputeModbusDataset(inputFile,timeInterval) `method`

##### Summary

Computes a Modbus Compact IPFIX dataset for the given source pcpa file.

##### Returns

The Modbus ICS Dataset object.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| inputFile | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the input PCAP file. |
| timeInterval | [System.TimeSpan](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.TimeSpan 'System.TimeSpan') | The time interval used to set window size for collecting flows. |

<a name='M-IcsMonitor-Interactive-ContainsPacket-Traffix-Storage-Faster-FasterConversationTable,Traffix-Core-Flows-FlowKey,PacketDotNet-Packet-'></a>
### ContainsPacket(table,conversationKey,packet) `method`

##### Summary

Tests if the `packet` belongs to the given conversation specified by its `conversationKey`.

##### Returns

true if the packet belongs to the conversation; false otherwise

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [Traffix.Storage.Faster.FasterConversationTable](#T-Traffix-Storage-Faster-FasterConversationTable 'Traffix.Storage.Faster.FasterConversationTable') | The conversation table providing the context for the operation. |
| conversationKey | [Traffix.Core.Flows.FlowKey](#T-Traffix-Core-Flows-FlowKey 'Traffix.Core.Flows.FlowKey') | The conversation key. |
| packet | [PacketDotNet.Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet') | The packet. |

<a name='M-IcsMonitor-Interactive-CreateCaptureFileReader-System-String,System-Boolean-'></a>
### CreateCaptureFileReader(path,useManaged) `method`

##### Summary

Creates the capture file reader.

##### Returns

The capture reader.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| path | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The source pcap file to read. |
| useManaged | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Use the managed reader implementation. If false it uses SharpPcap and external library. |

<a name='M-IcsMonitor-Interactive-CreateConversationTable-System-Collections-Generic-IEnumerable{Traffix-Providers-PcapFile-RawFrame},System-String,System-Nullable{System-Threading-CancellationToken}-'></a>
### CreateConversationTable(frames,conversationTablePath,token) `method`

##### Summary

Creates aconversaton table from the given `frames`.

##### Returns

Newly created conversation table.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| frames | [System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame}') | Source frames used to populate conversation table. |
| conversationTablePath | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The path to folder where conversation table is to be saved. |
| token | [System.Nullable{System.Threading.CancellationToken}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Nullable 'System.Nullable{System.Threading.CancellationToken}') | The cancellation token for interrupting the operation. |

<a name='M-IcsMonitor-Interactive-CreateConversationTables-System-Collections-Generic-IEnumerable{Traffix-Providers-PcapFile-RawFrame},System-String,System-TimeSpan,System-Nullable{System-Threading-CancellationToken}-'></a>
### CreateConversationTables(frames,conversationTablePath,timeInterval,token) `method`

##### Summary

Computes a collection of conversation tables by splitting the input frames to specified time intervals.



Each table is computed for a single time interval given by `timeInterval` time span. 
It is thus possible to consider a use case when IPFIX monitoring is used for fixed time intervals, e.g., 1 minute.
In this interval the conversations are computed and can be further analyzed.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| frames | [System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame}') | An input collection of frames. |
| conversationTablePath | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The path to conversation table files. |
| timeInterval | [System.TimeSpan](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.TimeSpan 'System.TimeSpan') | The time interval used for splitting the input frames in conversation tables. |
| token | [System.Nullable{System.Threading.CancellationToken}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Nullable 'System.Nullable{System.Threading.CancellationToken}') | Cancellation token used to cancel the operation. |

<a name='M-IcsMonitor-Interactive-GetConversations``1-Traffix-Storage-Faster-FasterConversationTable,Traffix-Storage-Faster-IConversationProcessor{``0}-'></a>
### GetConversations\`\`1(table,processor) `method`

##### Summary

Gets the data using provided conversation `processor` from the given `table`.

##### Returns

A collection of `TData` records.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [Traffix.Storage.Faster.FasterConversationTable](#T-Traffix-Storage-Faster-FasterConversationTable 'Traffix.Storage.Faster.FasterConversationTable') | The conversation table. |
| processor | [Traffix.Storage.Faster.IConversationProcessor{\`\`0}](#T-Traffix-Storage-Faster-IConversationProcessor{``0} 'Traffix.Storage.Faster.IConversationProcessor{``0}') | The conversation processor used to get conversation from the conversation table. |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| Tdata | Type of data records provided. |

<a name='M-IcsMonitor-Interactive-GetNextFrames-Traffix-Providers-PcapFile-ICaptureFileReader,System-Int32-'></a>
### GetNextFrames(reader,count) `method`

##### Summary

Reads up to the specified `count` of frames using the given `reader`

##### Returns

A collection of [RawFrame](#T-Traffix-Providers-PcapFile-RawFrame 'Traffix.Providers.PcapFile.RawFrame') objects.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| reader | [Traffix.Providers.PcapFile.ICaptureFileReader](#T-Traffix-Providers-PcapFile-ICaptureFileReader 'Traffix.Providers.PcapFile.ICaptureFileReader') | The pcap reader. |
| count | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Number of frames to read. |

<a name='M-IcsMonitor-Interactive-GetPackets-Traffix-Storage-Faster-FasterConversationTable-'></a>
### GetPackets(table) `method`

##### Summary

Get all packets from the conversation `table`.

##### Returns

A collection of packets.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [Traffix.Storage.Faster.FasterConversationTable](#T-Traffix-Storage-Faster-FasterConversationTable 'Traffix.Storage.Faster.FasterConversationTable') | The conversation table. |

<a name='M-IcsMonitor-Interactive-LoadFromTsv-Microsoft-ML-MLContext,System-String-'></a>
### LoadFromTsv(context,path) `method`

##### Summary

Loads the data view from the TSV file.

##### Returns

The data view loaded.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [Microsoft.ML.MLContext](#T-Microsoft-ML-MLContext 'Microsoft.ML.MLContext') | The ML.NET context. |
| path | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The path to source file. |

<a name='M-IcsMonitor-Interactive-SaveToTsv-Microsoft-ML-MLContext,Microsoft-ML-IDataView,System-String-'></a>
### SaveToTsv(context,dataview,path) `method`

##### Summary

Saves the `dataview` to TSV file.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| context | [Microsoft.ML.MLContext](#T-Microsoft-ML-MLContext 'Microsoft.ML.MLContext') | The ML.NET context. |
| dataview | [Microsoft.ML.IDataView](#T-Microsoft-ML-IDataView 'Microsoft.ML.IDataView') | The dataview to save. |
| path | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | the pth of the target file. |

<a name='M-IcsMonitor-Interactive-WriteToFile-System-Collections-Generic-IEnumerable{Traffix-Providers-PcapFile-RawFrame},System-String-'></a>
### WriteToFile(frames,path) `method`

##### Summary

Writes a collection of raw `frames` to PCAP file at the given `path`.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| frames | [System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{Traffix.Providers.PcapFile.RawFrame}') | The source frames. |
| path | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | the path of the pcap file to create. |

<a name='T-IcsMonitor-OutputWriter-MapAllPublicProperties`1'></a>
## MapAllPublicProperties\`1 `type`

##### Namespace

IcsMonitor.OutputWriter

##### Summary

It adds all publlic properties of the given type.

##### Generic Types

| Name | Description |
| ---- | ----------- |
| T |  |

<a name='T-IcsMonitor-PacketHelper'></a>
## PacketHelper `type`

##### Namespace

IcsMonitor

<a name='M-IcsMonitor-PacketHelper-GetRawFrame-PacketDotNet-Packet,System-Int64-'></a>
### GetRawFrame(packet,ticks) `method`

##### Summary

Converts [Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet') object to [RawFrame](#T-Traffix-Providers-PcapFile-RawFrame 'Traffix.Providers.PcapFile.RawFrame').

##### Returns

The [RawFrame](#T-Traffix-Providers-PcapFile-RawFrame 'Traffix.Providers.PcapFile.RawFrame') representation of the packet.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| packet | [PacketDotNet.Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet') | The packet. |
| ticks | [System.Int64](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int64 'System.Int64') | Time information on the packet given as the number of Ticks. |

<a name='M-IcsMonitor-PacketHelper-Segments-System-Collections-Generic-IEnumerable{PacketDotNet-Packet}-'></a>
### Segments(packets) `method`

##### Summary

Gets all non-empty [TcpPacket](#T-PacketDotNet-TcpPacket 'PacketDotNet.TcpPacket') segments for the given collection of [Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet') instances.

##### Returns

The sequence of non-empty Tcp segments.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| packets | [System.Collections.Generic.IEnumerable{PacketDotNet.Packet}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{PacketDotNet.Packet}') | A sequence of packets. |

<a name='M-IcsMonitor-PacketHelper-Segments``1-System-Collections-Generic-IEnumerable{System-ValueTuple{PacketDotNet-Packet,``0}}-'></a>
### Segments\`\`1(packets) `method`

##### Summary

Gets all non-empty [TcpPacket](#T-PacketDotNet-TcpPacket 'PacketDotNet.TcpPacket') segments for the given collection of [Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet') instances.

##### Returns

The sequence of non-empty Tcp segments.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| packets | [System.Collections.Generic.IEnumerable{System.ValueTuple{PacketDotNet.Packet,\`\`0}}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{System.ValueTuple{PacketDotNet.Packet,``0}}') | A sequence of packets. |

<a name='M-IcsMonitor-PacketHelper-TryGetSegment-PacketDotNet-Packet,PacketDotNet-TcpPacket@-'></a>
### TryGetSegment(packet,tcp) `method`

##### Summary

Tries to get a non-empty Tcp segment out of the provided [Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet').

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| packet | [PacketDotNet.Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet') |  |
| tcp | [PacketDotNet.TcpPacket@](#T-PacketDotNet-TcpPacket@ 'PacketDotNet.TcpPacket@') |  |

<a name='T-IcsMonitor-ModbusDataModel-Prediction'></a>
## Prediction `type`

##### Namespace

IcsMonitor.ModbusDataModel

##### Summary

The output of the ML evaluation.

<a name='P-IcsMonitor-ModbusDataModel-Prediction-ClusterId'></a>
### ClusterId `property`

##### Summary

Contains the ID of the predicted cluster.

<a name='P-IcsMonitor-ModbusDataModel-Prediction-Distance'></a>
### Distance `property`

##### Summary

Gets the distance to the precicted cluster centroid.

<a name='P-IcsMonitor-ModbusDataModel-Prediction-Distances'></a>
### Distances `property`

##### Summary

Contains an array with squared Euclidean distances to the cluster centroids. The array length is equal to the number of clusters.

<a name='P-IcsMonitor-ModbusDataModel-Prediction-FlowKey'></a>
### FlowKey `property`

##### Summary

The flow key used for referencing the original flow.

<a name='P-IcsMonitor-ModbusDataModel-Prediction-Threshold'></a>
### Threshold `property`

##### Summary

The treshold value, which is SQRT(variance) * T

<a name='P-IcsMonitor-ModbusDataModel-Prediction-Threshold2'></a>
### Threshold2 `property`

##### Summary

The double treshold value, which is 2 * T * SQRT(variance)

<a name='P-IcsMonitor-ModbusDataModel-Prediction-Threshold3'></a>
### Threshold3 `property`

##### Summary

The triple treshold value, which is 3 * T * SQRT(variance)

<a name='P-IcsMonitor-ModbusDataModel-Prediction-Variance'></a>
### Variance `property`

##### Summary

The computed variance for the cluster.

<a name='T-IcsMonitor-Program'></a>
## Program `type`

##### Namespace

IcsMonitor

<a name='M-IcsMonitor-Program-ExportTcpPayload-System-String,System-Nullable{System-Int32},System-String-'></a>
### ExportTcpPayload(inputFile,port,outputFile) `method`

##### Summary

Export the payload of tcp segments. Each payload is put in a single binary file created in the target zip archive.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| inputFile | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the input pcap file. |
| port | [System.Nullable{System.Int32}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Nullable 'System.Nullable{System.Int32}') | The port number of the TCP packet to be included. |
| outputFile | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the output zip file. If not specified the name is the same as the inpout name but with zip extension. |

<a name='M-IcsMonitor-Program-TrainModbusModel-System-String,System-String,System-Int32-'></a>
### TrainModbusModel(inputFile,outputFile,numberOfClusters) `method`

##### Summary

Trains the Modbus Model (OC-Kmeans), which represents a normal profile.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| inputFile | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Input packet capture file. |
| outputFile | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The model file to be created. |
| numberOfClusters | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | A number of required clusters. Default is 8. |

<a name='T-IcsMonitor-S7Comm-S7CommConversationData'></a>
## S7CommConversationData `type`

##### Namespace

IcsMonitor.S7Comm

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-AckControlCountFieldNumber'></a>
### AckControlCountFieldNumber `constants`

##### Summary

Field number for the "ackControlCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-AckDownloadCountFieldNumber'></a>
### AckDownloadCountFieldNumber `constants`

##### Summary

Field number for the "ackDownloadCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarCountFieldNumber'></a>
### AckReadVarCountFieldNumber `constants`

##### Summary

Field number for the "ackReadVarCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarSuccessCountFieldNumber'></a>
### AckReadVarSuccessCountFieldNumber `constants`

##### Summary

Field number for the "ackReadVarSuccessCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-AckUploadCountFieldNumber'></a>
### AckUploadCountFieldNumber `constants`

##### Summary

Field number for the "ackUploadCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-AckWriteSuccessCountFieldNumber'></a>
### AckWriteSuccessCountFieldNumber `constants`

##### Summary

Field number for the "ackWriteSuccessCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-AckWriteVarCountFieldNumber'></a>
### AckWriteVarCountFieldNumber `constants`

##### Summary

Field number for the "ackWriteVarCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-DataLengthSumFieldNumber'></a>
### DataLengthSumFieldNumber `constants`

##### Summary

Field number for the "dataLengthSum" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-ErrorInResponseCountFieldNumber'></a>
### ErrorInResponseCountFieldNumber `constants`

##### Summary

Field number for the "errorInResponseCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-JobControlCountFieldNumber'></a>
### JobControlCountFieldNumber `constants`

##### Summary

Field number for the "jobControlCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-JobDownloadCountFieldNumber'></a>
### JobDownloadCountFieldNumber `constants`

##### Summary

Field number for the "jobDownloadCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarCountFieldNumber'></a>
### JobReadVarCountFieldNumber `constants`

##### Summary

Field number for the "jobReadVarCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarItemCountFieldNumber'></a>
### JobReadVarItemCountFieldNumber `constants`

##### Summary

Field number for the "jobReadVarItemCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-JobUploadCountFieldNumber'></a>
### JobUploadCountFieldNumber `constants`

##### Summary

Field number for the "jobUploadCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarCountFieldNumber'></a>
### JobWriteVarCountFieldNumber `constants`

##### Summary

Field number for the "jobWriteVarCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarItemCountFieldNumber'></a>
### JobWriteVarItemCountFieldNumber `constants`

##### Summary

Field number for the "jobWriteVarItemCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-ParamLengthSumFieldNumber'></a>
### ParamLengthSumFieldNumber `constants`

##### Summary

Field number for the "paramLengthSum" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-UnknownRequestCountFieldNumber'></a>
### UnknownRequestCountFieldNumber `constants`

##### Summary

Field number for the "unknownRequestCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-UnknownResponseCountFieldNumber'></a>
### UnknownResponseCountFieldNumber `constants`

##### Summary

Field number for the "unknownResponseCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-UserDataRequestCountFieldNumber'></a>
### UserDataRequestCountFieldNumber `constants`

##### Summary

Field number for the "userDataRequestCount" field.

<a name='F-IcsMonitor-S7Comm-S7CommConversationData-UserDataResponseCountFieldNumber'></a>
### UserDataResponseCountFieldNumber `constants`

##### Summary

Field number for the "userDataResponseCount" field.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-AckControlCount'></a>
### AckControlCount `property`

##### Summary

Count of upload job messages.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-AckDownloadCount'></a>
### AckDownloadCount `property`

##### Summary

Count of upload job messages.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarCount'></a>
### AckReadVarCount `property`

##### Summary

Count of Ack Data Read Var responses.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-AckReadVarSuccessCount'></a>
### AckReadVarSuccessCount `property`

##### Summary

Count of success items in Ack Data Read Var responses.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-AckUploadCount'></a>
### AckUploadCount `property`

##### Summary

Count of upload job messages.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-AckWriteSuccessCount'></a>
### AckWriteSuccessCount `property`

##### Summary

Count of success items in Ack Data Write Var responses.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-AckWriteVarCount'></a>
### AckWriteVarCount `property`

##### Summary

Count of Ack Data Write Var responses.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-DataLengthSum'></a>
### DataLengthSum `property`

##### Summary

Sum of all data lengths presented in the header.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-ErrorInResponseCount'></a>
### ErrorInResponseCount `property`

##### Summary

Count of responses that report some error.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-JobControlCount'></a>
### JobControlCount `property`

##### Summary

Count of PLC control messages.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-JobDownloadCount'></a>
### JobDownloadCount `property`

##### Summary

Count of download job messages.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarCount'></a>
### JobReadVarCount `property`

##### Summary

Count of Job Read Var requests.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-JobReadVarItemCount'></a>
### JobReadVarItemCount `property`

##### Summary

Count of items in Job Read Var requests.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-JobUploadCount'></a>
### JobUploadCount `property`

##### Summary

Count of upload job request messages.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarCount'></a>
### JobWriteVarCount `property`

##### Summary

Count of Job Write Var requests.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-JobWriteVarItemCount'></a>
### JobWriteVarItemCount `property`

##### Summary

Count of items in Job Write Var requests.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-ParamLengthSum'></a>
### ParamLengthSum `property`

##### Summary

Sum of all parameter lengths presented in the header.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-UnknownRequestCount'></a>
### UnknownRequestCount `property`

##### Summary

Count of requests with unknow message type or function code.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-UnknownResponseCount'></a>
### UnknownResponseCount `property`

##### Summary

Count of responses with unknow message type or function code.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-UserDataRequestCount'></a>
### UserDataRequestCount `property`

##### Summary

Count of user data request messages.

<a name='P-IcsMonitor-S7Comm-S7CommConversationData-UserDataResponseCount'></a>
### UserDataResponseCount `property`

##### Summary

Count of user data response messages.

<a name='T-IcsMonitor-S7Comm-S7CommConversationDataReflection'></a>
## S7CommConversationDataReflection `type`

##### Namespace

IcsMonitor.S7Comm

##### Summary

Holder for reflection information generated from S7CommConversationData.proto

<a name='P-IcsMonitor-S7Comm-S7CommConversationDataReflection-Descriptor'></a>
### Descriptor `property`

##### Summary

File descriptor for S7CommConversationData.proto

<a name='T-IcsMonitor-S7Comm-S7CommConversationProcessor'></a>
## S7CommConversationProcessor `type`

##### Namespace

IcsMonitor.S7Comm

<a name='M-IcsMonitor-S7Comm-S7CommConversationProcessor-UpdateConversation-IcsMonitor-S7Comm-S7CommConversationData,Traffix-Storage-Faster-FrameMetadata,PacketDotNet-Packet,Traffix-Storage-Faster-FlowDirection-'></a>
### UpdateConversation(conversation,packet,direction) `method`

##### Summary

Updates a conversation with the specified packet.

##### Returns

true if the conversation was updated with the information in the packet; false if the packet does not contain any related information.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| conversation | [IcsMonitor.S7Comm.S7CommConversationData](#T-IcsMonitor-S7Comm-S7CommConversationData 'IcsMonitor.S7Comm.S7CommConversationData') |  |
| packet | [Traffix.Storage.Faster.FrameMetadata](#T-Traffix-Storage-Faster-FrameMetadata 'Traffix.Storage.Faster.FrameMetadata') |  |
| direction | [PacketDotNet.Packet](#T-PacketDotNet-Packet 'PacketDotNet.Packet') |  |
