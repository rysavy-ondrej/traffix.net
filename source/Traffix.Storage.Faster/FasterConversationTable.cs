using FASTER.core;
using PacketDotNet;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Traffix.Core.Flows;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// Implements a flow table backed by FASTER Key-Value Database. This class provides API for 
    /// adding packets and accessing conversations.
    /// </summary>
    public class FasterConversationTable : IDisposable
    {
        private readonly string _rootFolder;
        private readonly ConversationsStore _conversationsStore;
        private readonly FramesStore _framesStore;

        /// <summary>
        /// Memory pool used for buffer allocations in this class.
        /// </summary>
        private MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;

        /// <summary>
        /// The packet key provider used for finding keys in frames. 
        /// By default it uses <see cref="PacketKeyProvider"/> based on PacketDotNet package.
        /// </summary>
        IFlowKeyProvider<FlowKey, Packet> _packetKeyProvider = new PacketKeyProvider();

        private bool _disposedValue;


        public static FasterConversationTable Create(string folder)
        {
            // ensure that root folder does exist:
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var table = new FasterConversationTable(folder);
            table._conversationsStore.InitAndRecover();
            table._framesStore.InitAndRecover();
            return table;
        }

        public static FasterConversationTable Open(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException($"Specified folder '{folder}' not found.");
            }
            var table = new FasterConversationTable(folder);
            table._conversationsStore.InitAndRecover();
            table._framesStore.InitAndRecover();
            return table;
        }

        /// <summary>
        /// Creates a store that uses the specified folder for saving data.
        /// If the folder does not exist, it will be created.
        /// </summary>
        /// <param name="folder"></param>
        protected FasterConversationTable(string folder)
        {
            _rootFolder = folder;
            _conversationsStore = new ConversationsStore(Path.Combine(folder, "conversations"));
            _framesStore = new FramesStore(Path.Combine(folder, "frames"));
        }
        #region Dispose Implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // always commit on dispose
                    Commit();
                    _framesStore.Dispose();
                    _conversationsStore.Dispose();
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion


        /// <summary>
        /// Loads frames and identifies their conversations from the provided packet capture stream.
        /// </summary>
        /// <param name="stream">The input stream of tcpdump pcap format.</param>
        /// <param name="cancellationToken"></param>
        public void LoadFromStream(Stream stream, CancellationToken cancellationToken, Action<long, Packet>? onNextPacket = null)
        {
            using var captureReader = new CaptureFileReader(stream) ?? throw new ArgumentException("Invalid stream provided.", nameof(stream));
            using var loader = GetFrameLoader();

            // reads all frames from the capture file stream:
            while (captureReader.GetNextFrameHeader(out var frame))
            {
                using var buffer = _memoryPool.Rent(frame.IncludedLength);    
                // get and use only the required portion of the buffer:
                var bytes = buffer.Memory.Span.Slice(0, frame.IncludedLength);
                var address = captureReader.Position;
                captureReader.ReadFrameBytes(bytes);
                loader.AddFrame(frame, bytes, address);
                if (cancellationToken.IsCancellationRequested) break;
            }

            loader.Close();
        }

        /// <summary>
        /// Commit the current state and persist changes to disk database.
        /// </summary>
        public void Commit()
        {
            _conversationsStore.Checkpoint();
            _framesStore.Checkpoint();
        }

        public FlowKey GetFrameKey(LinkLayers linkLayer, Span<byte> bytes)
        {
            var packet = Packet.ParsePacket(linkLayer, bytes.ToArray());
            var frameKey = _packetKeyProvider.GetKey(packet);
            return frameKey;
        }
        public FlowKey GetPacketKey(Packet packet)
        {
            var packetKey = _packetKeyProvider.GetKey(packet);
            return packetKey;
        }

        private void UpsertConversation(ClientSession<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationContext, ConversationFunctions> conversationsSession, RawFrame frame, FlowKey frameKey, long frameAddress)
        {
            var input = new ConversationInput
            {
                FrameAddress = frameAddress,
                FrameSize = frame.OriginalLength,
                FrameTicks = frame.Ticks,
                FrameKey = frameKey
            };
            var conversationKey = GetConversationKey(frameKey);
            conversationsSession.RMW(ref conversationKey, ref input, ConversationContext.Empty, 0);
        }

        private unsafe long UpsertFrame(ClientSession<FrameKey, FrameValue, FrameInput, FrameOutput, FrameContext, FrameFunctions> framesSession, RawFrame frame, Span<byte> bytes, long address, FlowKey frameFlowKey)
        {
            var size = FrameValue.ComputeLength(frame.IncludedLength);
            using var buffer = _memoryPool.Rent(size);
            fixed (void* bufferPtr = buffer.Memory.Span)
            {
                ref var frameValue = ref Unsafe.AsRef<FrameValue>(bufferPtr);

                frameValue.Meta.Ticks = frame.Ticks;
                frameValue.Meta.IncludedLength = (ushort)frame.IncludedLength;
                frameValue.Meta.OriginalLength = (ushort)frame.OriginalLength;
                frameValue.Meta.LinkLayer = (ushort)frame.LinkLayer;

                var frameBytesSpan = new Span<byte>(Unsafe.AsPointer(ref frameValue.Bytes), frame.IncludedLength);
                bytes.TryCopyTo(frameBytesSpan);

                frameValue.Meta.FlowKeyHash = frameFlowKey.GetHashCode64();

                var frameKey = new FrameKey { Address = address };
                framesSession.Upsert(ref frameKey, ref frameValue, FrameContext.Empty, 0);
                return address;
            }
        }

        /// <summary>
        /// Gets the loader that can be used for the batch insertation of new frames to the table.
        /// Call <see cref="FrameStreamer.Close"/> to complete all pending insert operations.
        /// <para>
        /// The FrameLoader should be  properly disposed when no longer in use.  
        /// </para>
        /// </summary>
        /// <returns>The loader instance.</returns>
        public FrameStreamer GetFrameLoader()
        {
            using var conversationsSession = _conversationsStore.NewSession() ?? throw new InvalidOperationException("Cannot create conversation session.");
            using var framesSession = _framesStore.NewSession() ?? throw new InvalidOperationException("Cannot create conversation session.");
            return new FrameStreamer(this, conversationsSession, framesSession);
        }


        /// <summary>
        /// Gets the conversation key from the flow key.
        /// <para/>
        /// It uses port numbers to determine the conversation key.
        /// The assumption is that client has greater port number than server.
        /// </summary>
        /// <param name="frameKey">The frame key.</param>
        /// <returns>The conversation key for the frame.</returns>
        private static ConversationKey GetConversationKey(FlowKey frameKey)
        {
            if (frameKey.SourcePort > frameKey.DestinationPort)
            {
                return new ConversationKey(frameKey);
            }
            else
            {
                return new ConversationKey(frameKey.Reverse());
            }
        }

        /// <summary>
        /// Processes conversations given by their <paramref name="keys"/> using the specigied <paramref name="processor"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of resulting objects produced by the processor.</typeparam>
        /// <param name="keys">The collection of conversation keys to process.</param>
        /// <param name="processor">The conversation processor used to transform the conversation to the object of type <typeparamref name="TResult"/>.</param>
        /// <returns>The enumerable collection of resulting objects produced by the <paramref name="processor"/>.</returns>
        public IEnumerable<TResult> ProcessConversations<TResult>(IEnumerable<IConversationKey> keys, IConversationProcessor<TResult> processor)
        {
            using var conversationsSession = _conversationsStore.NewSession() ?? throw new InvalidOperationException("Cannot create conversations DB session.");
            using var framesSession = _framesStore.NewSession() ?? throw new InvalidOperationException("Cannot create frames DB session.");


            unsafe TResult ProcessConversation(ref ConversationKey key)
            {

                var input = new ConversationInput();
                var output = new ConversationOutput();
                _ = conversationsSession.Read(ref key, ref input, ref output, ConversationContext.Empty, 0);

                var frameKey = new FrameKey();
                var frameInput = new FrameInput() { Pool = MemoryPool<byte>.Shared };
                var frameList = new List<IMemoryOwner<byte>>();
                for(int i = 0; i < output.Value.FrameCount; i++)
                {
                    var frameOutput = new FrameOutput();
                    frameKey.Address = output.Value.FrameAddresses[i];
                    _ = framesSession.Read(ref frameKey, ref frameInput, ref frameOutput, FrameContext.Empty, 0);
                    frameList.Add(frameOutput.FrameBuffer);
                }
                var result = processor.Invoke(key.FlowKey, frameList.Select(x=>x.Memory));
                foreach (var m in frameList) m?.Dispose();
                return result;
            }

            TResult ProcessConversationSafe(IConversationKey key)
            {
                if (key is ConversationKey _key)
                {
                    return ProcessConversation(ref _key);
                }
                return default;
            }
            return keys.Select(key => ProcessConversationSafe(key));
        }



        /// <summary>
        /// Indicates the result of processing the object.
        /// </summary>
        public enum ProcessingState
        {
            /// <summary>
            /// The object has been successfully processed.
            /// </summary>
            Success,
            /// <summary>
            /// The object was skipped and should not be a part of the output.
            /// </summary>
            Skip, 
            /// <summary>
            /// The object was not processed and the entire processing should be terminated.
            /// </summary>
            Terminate
        }
        public struct ProcessingResult<TResult>
        {
            public ProcessingState State;
            public TResult Result;
        }

        /// <summary>
        /// Applies the provided <paramref name="processor"/> to all frames in the table yielding to a collection of results produced by the processor. 
        /// </summary>
        /// <typeparam name="TResult">The type of resulting objects.</typeparam>
        /// <param name="processor">The processor function to apply.</param>
        /// <returns>A collection of results produced by the processor.</returns>
        public IEnumerable<TResult> ProcessFrames<TResult>(Func<FrameMetadata, Memory<byte>, ProcessingResult<TResult>> processor)
        {
            ProcessingState GetResult(FrameValue frame, out TResult result)
            {
                using var buffer = _memoryPool.Rent(frame.Meta.IncludedLength);
                frame.GetFrameBytes(buffer.Memory.Span);
                var x = processor.Invoke(frame.Meta, buffer.Memory);
                result = x.Result;
                return x.State;
            }
            foreach (var item in _framesStore.Items)
            {
                switch (GetResult(item.Value, out var result))
                {
                    case ProcessingState.Success:
                        yield return result;
                        break;
                    case ProcessingState.Skip:
                        continue;
                    case ProcessingState.Terminate:
                        yield break;
                }
            }
        }


        /// <summary>
        /// Provides enumerable for all stored conversations.
        /// </summary>
        /// <returns>An enumerable of conversations.</returns>
        public IEnumerable<KeyValuePair<IConversationKey, IConversationValue>> Conversations
        {
            get
            {
                foreach(var item in _conversationsStore.Items)
                {
                    yield return KeyValuePair.Create(item.Key as IConversationKey, item.Value as IConversationValue);
                }
            }
        }

        /// <summary>
        /// Gets all stored conversation keys.
        /// </summary>
        /// <returns>An enumerable of conversation keys.</returns>
        public IEnumerable<IConversationKey> ConversationKeys
        {
            get
            {
                foreach (var item in _conversationsStore.Items)
                {
                    yield return item.Key;
                }
            }
        }

        public IEnumerable<RawFrame> Frames
        {
            get
            {
                int frameNumber = 0;
                ProcessingResult<RawFrame> GetRawFrame(FrameMetadata meta, Memory<byte> frame)
                {
                    return new ProcessingResult<RawFrame>
                    {
                        State = ProcessingState.Success,
                        Result = new RawFrame((PacketDotNet.LinkLayers)meta.LinkLayer, ++frameNumber, meta.Ticks, 0, meta.OriginalLength, frame.ToArray())
                    };
                }
                return ProcessFrames(GetRawFrame);
            }
        }

        class RawFrameProcessor : ConversationProcessor<IEnumerable<RawFrame>>
        {
            public override IEnumerable<RawFrame> Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
            {
                FrameMetadata meta = new FrameMetadata();
                int frameNumber = 0;
                foreach (var frame in frames)
                {
                    var data = GetFrame(frame, ref meta);
                    yield return new RawFrame((PacketDotNet.LinkLayers)meta.LinkLayer, ++frameNumber, meta.Ticks, 0, meta.OriginalLength, data.ToArray()); 
                }
            }
        }

        public IEnumerable<RawFrame> GetFrames(IConversationKey conversation)
        {
            var result = ProcessConversations(new[] { conversation }, new RawFrameProcessor());
            return result.FirstOrDefault();

        }


        /// <summary>
        /// Frame streamer is responsible for streaming external frame into table. 
        /// It uses mapping keys to conversations responsible for the frame provides by the parent table.
        /// It achieves optimized resource utilization by properly buffering resources and updates. 
        /// </summary>
        public class FrameStreamer : IDisposable
        {
            private readonly FasterConversationTable _table;
            private ClientSession<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationContext, ConversationFunctions> _conversationsSession;
            private ClientSession<FrameKey, FrameValue, FrameInput, FrameOutput, FrameContext, FrameFunctions> _framesSession;
            private bool _closed;

            internal FrameStreamer(FasterConversationTable table, ClientSession<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationContext, ConversationFunctions> conversationsSession, ClientSession<FrameKey, FrameValue, FrameInput, FrameOutput, FrameContext, FrameFunctions> framesSession)
            {
                _table = table;
                _conversationsSession = conversationsSession;
                _framesSession = framesSession;
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
            /// <param name="frameBytes">Data bytes of the raw frame.</param>
            /// <param name="address">Offset/key of the frame. Can be used to refer to the frame to the external data source.</param>
            /// <exception cref="InvalidOperationException">Raises when the stremer is closed.</exception>
            public void AddFrame(RawFrame frame, Span<byte> frameBytes, long address)
            {
                if (_closed) throw new InvalidOperationException("Cannot add new data. The stream is closed.");
                var frameKey = _table.GetFrameKey(frame.LinkLayer, frameBytes);
                var frameAddress = _table.UpsertFrame(_framesSession, frame, frameBytes, address, frameKey);
                _table.UpsertConversation(_conversationsSession, frame, frameKey, frameAddress);
            }

            /// <summary>
            /// Streams any remaining data, but doesn't close the streamer.
            /// </summary>
            public void Flush()
            {
                _conversationsSession.CompletePending(true);
                _framesSession.CompletePending(true);
            }

            /// <summary>
            /// Streams any remaining data and closes this streamer.
            /// </summary>
            public void Close()
            {
                // complete operations
                Flush();
                _closed = true;
            }
            public void Dispose()
            {
                if (!_closed) Close();
                ((IDisposable)_conversationsSession).Dispose();
                ((IDisposable)_framesSession).Dispose();
            }
        }
    }
}
