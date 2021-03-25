using SharpPcap;
using System;
using Traffix.Core;
using Traffix.Data;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster
{
    public partial class FasterFrameTable
    {
        /// <summary>
        /// Frame streamer is responsible for streaming external frame into table. 
        /// It uses mapping keys to conversations responsible for the frame provides by the parent table.
        /// It achieves optimized resource utilization by properly buffering resources and updates. 
        /// </summary>
        public class FrameStreamer : IDisposable
        {
            private readonly FasterFrameTable _table;
            private readonly RawFramesStore.ClientSession _framesStoreClient;
            private readonly int _autoFlushRecordCount;
            private bool _closed;
            private int _outstandingRequests;

            internal FrameStreamer(FasterFrameTable table,
                RawFramesStore.ClientSession framesStoreClient,
                int autoFlushRecordCount)
            {
                _table = table;
                _framesStoreClient = framesStoreClient;
                _autoFlushRecordCount = autoFlushRecordCount;
            }

            /// <summary>
            /// Inserts a frame to the table doing all necessary processing. 
            /// <para>
            /// While <see cref="RawFrame"/> object can contain byte array in <see cref="RawFrame.Data"/> field
            /// this method uses separate parameter <paramref name="frameBytes"/> to provide frame bytes.
            /// This parameter, however, can refer to <see cref="RawFrame.Data"/> bytes.
            /// </para>
            /// </summary>
            /// <param name="frame">The raw frame object.</param>
            /// <param name="frameNumber">The frame number.</param>
            /// <exception cref="InvalidOperationException">Raises when the stremer is closed.</exception>
            public void AddFrame(RawCapture frame)
            {
                if (_closed) throw new InvalidOperationException("Cannot add new data. The stream is closed.");
                var frameFlowKey = _table.GetFlowKey(frame.LinkLayerType, frame.Data);

                var frameMeta = GetFrameMetadata(frame, frameFlowKey);  // stack allocated struct 
                var frameKey = new FrameKey(frameMeta.Ticks, (uint)_table._framesCount);
                _table.InsertFrame(_framesStoreClient, ref frameKey, ref frameFlowKey, ref frameMeta, frame.Data);
                _outstandingRequests++;
                if (_outstandingRequests > _autoFlushRecordCount) CompletePending();
            }

            private FrameMetadata GetFrameMetadata(RawCapture rawCapture, Core.Flows.FlowKey frameFlowKey)
            {
                return new FrameMetadata()
                {
                    Ticks = rawCapture.Timeval.Date.Ticks,
                    OriginalLength = (ushort)rawCapture.Data.Length,
                    LinkLayer = (ushort)rawCapture.LinkLayerType,
                    FlowKeyHash = frameFlowKey.GetHashCode64()
                };
            }

            /// <summary>
            /// Waits until all pending operations are completed.
            /// <para/>
            /// This causes that pending operations of the loader is completed but it does not mean that 
            /// they are persisted in the storage. It is necessary to call <seealso cref="FasterConversationTable.SaveChanges"/>
            /// method.
            /// </summary>
            public void CompletePending()
            {
                _framesStoreClient.CompletePending(true);
                _outstandingRequests = 0;
            }

            /// <summary>
            /// Waits until all pending operations are completed and closes this streamer.
            /// </summary>
            public void Close()
            {
                CompletePending();
                _closed = true;
            }
            public void Dispose()
            {
                if (!_closed) Close();
                ((IDisposable)_framesStoreClient).Dispose();
            }
        }
    }
}
