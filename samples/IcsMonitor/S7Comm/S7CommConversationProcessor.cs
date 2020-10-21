using Kaitai;
using Microsoft.Extensions.Logging;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using Traffix.Extensions.Decoders.Industrial;
using Traffix.Storage.Faster;

namespace IcsMonitor.S7Comm
{

    public class S7CommConversationProcessor : CustomConversationProcessor<S7CommConversationData>
    {
        Logger<S7CommConversationProcessor> _logger = null; 
        public S7CommConversationProcessor(ILoggerFactory loggerFactory = null)
        {
            if (loggerFactory != null)
                _logger = new Logger<S7CommConversationProcessor>(loggerFactory);
        }

        protected override S7CommConversationData Invoke(IReadOnlyCollection<(FrameMetadata Meta, Packet Packet)> fwdPackets, IReadOnlyCollection<(FrameMetadata Meta, Packet Packet)> revPackets)
        {
            var conversation = new S7CommConversationData();
            foreach (var packet in fwdPackets) UpdateConversation(conversation, packet.Meta, packet.Packet, FlowDirection.Forward);
            foreach (var packet in revPackets) UpdateConversation(conversation, packet.Meta, packet.Packet, FlowDirection.Reverse);
            return conversation;
        }

        /// <summary>
        /// Updates a conversation with the specified packet.
        /// </summary>
        /// <param name="conversation"></param>
        /// <param name="packet"></param>
        /// <param name="direction"></param>
        /// <returns>true if the conversation was updated with the information in the packet; false if the packet does not contain any related information.</returns>
        bool UpdateConversation(S7CommConversationData conversation, FrameMetadata meta, Packet packet, FlowDirection direction)
        {
            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket.PayloadData?.Length != 0)
            {
                var tptkStream = new KaitaiStream(tcpPacket.PayloadData);
                if (TryParseTptk(tptkStream, out var tptkPacket, out var tptkError) && tptkPacket.Cotp.PduType == TpktPacket.CotpType.DataTransfer && tptkPacket.Payload?.Length > 0)
                {
                    var s7stream = new KaitaiStream(tptkPacket.Payload);
                    if (TryParseS7Comm(s7stream, out var s7Packet, out var s7error))
                    {
                        UpdateFlow(conversation, s7Packet, direction);
                    }
                    else
                    {
                        if (direction == FlowDirection.Forward)
                            conversation.UnknownRequestCount++;
                        else
                            conversation.UnknownResponseCount++;
                    }
                }
                return true;
            }
            return false;
        }

        private bool TryParseTptk(KaitaiStream stream, out TpktPacket packet, out Exception error)
        {
            try
            {
                packet = new TpktPacket(stream);
                error = null;
                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError("TPTK parser errror: {error}", e.Message);
                packet = null;
                error = e;
                return false;
            }
        }

        private static bool TryParseS7Comm(KaitaiStream stream, out S7commPacket packet, out Exception exception)
        {
            try
            {
                packet = new S7commPacket(stream);
                exception = null;
                return true;
            }
            catch(Exception e)
            {
                packet = null;
                exception = e;
                return false;
            }
        }

        private void UpdateFlow(S7CommConversationData flow, S7commPacket packet, FlowDirection direction)
        {
            flow.DataLengthSum += packet.DataLength;
            flow.ParamLengthSum += packet.ParametersLength;

            switch (packet.Message)
            {
                case S7commPacket.JobRequestMessage jobRequestMessage:
                    switch (jobRequestMessage.Function)
                    {
                        case S7commPacket.JobReadVariable jobReadVariable:
                            flow.JobReadVarCount++;
                            flow.JobReadVarItemCount += jobReadVariable.ItemCount;
                            break;
                        case S7commPacket.JobWriteVariable jobWriteVariable:
                            flow.JobWriteVarCount++;
                            flow.JobWriteVarItemCount += jobWriteVariable.ItemCount;
                            break;
                        default:
                            switch(jobRequestMessage.FunctionCode)
                            {
                                case S7commPacket.S7FunctionCode.RequestDownload:
                                case S7commPacket.S7FunctionCode.DownloadBlock:
                                case S7commPacket.S7FunctionCode.DownloadEnded:
                                    flow.JobDownloadCount++;
                                    break;
                                case S7commPacket.S7FunctionCode.StartUpload:
                                case S7commPacket.S7FunctionCode.Upload:
                                case S7commPacket.S7FunctionCode.EndUpload:
                                    flow.JobUploadCount++;
                                    break;
                                case S7commPacket.S7FunctionCode.PlcControl:
                                case S7commPacket.S7FunctionCode.PlcStop:
                                    flow.JobControlCount++;
                                    break;
                            }
                            break;
                    }
                    break;
                case S7commPacket.AckDataMessage ackDataMessage:
                    if (packet.Error?.ErrorClass != 0) flow.ErrorInResponseCount++;

                    switch (ackDataMessage.Function)
                    {
                        case S7commPacket.AckDataReadVariable ackReadVariable:
                            flow.AckReadVarCount++;
                            flow.AckReadVarSuccessCount += (uint)ackReadVariable.Data.Count(x => x.ReturnCode == S7commPacket.ReturnCode.Success);
                            break;
                        case S7commPacket.AckDataWriteVariable ackWriteVariable:
                            flow.AckWriteVarCount++;
                            flow.AckWriteSuccessCount += (uint)ackWriteVariable.Items.Count(x => x == S7commPacket.ReturnCode.Success);
                            break;
                        default:
                            switch (ackDataMessage.FunctionCode)
                            {
                                case S7commPacket.S7FunctionCode.RequestDownload:
                                case S7commPacket.S7FunctionCode.DownloadBlock:
                                case S7commPacket.S7FunctionCode.DownloadEnded:
                                    flow.AckDownloadCount++;
                                    break;
                                case S7commPacket.S7FunctionCode.StartUpload:
                                case S7commPacket.S7FunctionCode.Upload:
                                case S7commPacket.S7FunctionCode.EndUpload:
                                    flow.AckUploadCount++;
                                    break;
                                case S7commPacket.S7FunctionCode.PlcControl:
                                case S7commPacket.S7FunctionCode.PlcStop:
                                    flow.AckControlCount++;
                                    break;
                            }
                            break;
                    }
                    break;
                default:
                    if (packet.MessageType == S7commPacket.S7MessageType.UserData)
                    {
                        if (direction == FlowDirection.Forward)
                        {
                            flow.UserDataRequestCount++;
                        }
                        else
                        {
                            flow.UserDataResponseCount++;
                        }
                    }
                    break;
            }
        }
    }
}
