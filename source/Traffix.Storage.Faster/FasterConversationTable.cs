using FASTER.core;
using PacketDotNet;
using System;
using System.Buffers;
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

        private readonly IDevice _conversationsDevice;
        private readonly IDevice _conversationsObjectsDevice;
        private readonly FasterKV<ConversationKey, ConversationValue, ConversationInput, ConversationOutput, ConversationContext, ConversationFunctions> _conversationsDb;

        private readonly IDevice _framesDevice;
        private readonly IDevice _framesObjectsDevice;
        private readonly FasterKV<FrameKey, FrameValue, FrameInput, FrameOutput, FrameContext, FrameFunctions> _framesDb;

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

        /// <summary>
        /// Creates a store that uses the specified folder for saving data.
        /// </summary>
        /// <param name="folder"></param>
        public FasterConversationTable(string folder)
        {
            _rootFolder = folder;

            _conversationsDevice = new LocalStorageDevice(Path.Combine(folder, "conversations.log"));
            _conversationsObjectsDevice = new LocalStorageDevice(Path.Combine(folder, "conversations.obj"));
            _conversationsDb = ConversationFunctions.CreateFaster(_conversationsDevice, _conversationsObjectsDevice);


            _framesDevice = new LocalStorageDevice(Path.Combine(folder, "frames.log"));
            _framesObjectsDevice = new LocalStorageDevice(Path.Combine(folder, "frames.obj"));
            _framesDb = FrameFunctions.CreateFaster(_framesDevice, _framesObjectsDevice);
        }
        #region Dispose Implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Flush();
                    ((IDisposable)_framesDb).Dispose();
                    ((IDisposable)_conversationsDb).Dispose();
                    _conversationsDevice.Close();
                    _conversationsObjectsDevice.Close();
                    _framesDevice.Close();
                    _framesObjectsDevice.Close();
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
        /// Persist changes to disk database.
        /// </summary>
        public void Flush()
        {
            _conversationsDb.Log.Flush(true);
            _framesDb.Log.Flush(true);
        }

        private FlowKey GetFrameKey(LinkLayers linkLayer, Span<byte> bytes)
        {
            var packet = Packet.ParsePacket(linkLayer, bytes.ToArray());
            var frameKey = _packetKeyProvider.GetKey(packet);
            return frameKey;
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
            using var conversationsSession = _conversationsDb.NewSession() ?? throw new InvalidOperationException("Cannot create conversation session.");
            using var framesSession = _framesDb.NewSession() ?? throw new InvalidOperationException("Cannot create conversation session.");
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
            using var conversationsSession = _conversationsDb.NewSession() ?? throw new InvalidOperationException("Cannot create conversations DB session."); ;
            using var framesSession = _framesDb.NewSession() ?? throw new InvalidOperationException("Cannot create frames DB session.");


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
        /// Applies the provided <paramref name="processor"/> to all frames in the table yielding to a collection of results produced by the processor. 
        /// </summary>
        /// <typeparam name="TResult">The type of resulting objects.</typeparam>
        /// <param name="processor">The processor function to apply.</param>
        /// <returns>A collection of results produced by the processor.</returns>
        public IEnumerable<TResult> ProcessFrames<TResult>(Func<FrameMetadata, Memory<byte>, TResult> processor)
        {
            TResult GetResult(ref FrameValue frame)
            {
                using var buffer = _memoryPool.Rent(frame.Meta.IncludedLength);
                frame.GetFrameBytes(buffer.Memory.Span);
                return processor.Invoke(frame.Meta, buffer.Memory);
            }

            var iterator = _framesDb.Iterate();
            while(iterator.GetNext(out _))
            {
                var result = GetResult(ref iterator.GetValue());
                yield return result;
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
                var iterator = _conversationsDb.Iterate();
                while (iterator.GetNext(out _))
                {
                    var key = iterator.GetKey();
                    var value = iterator.GetValue();
                    yield return KeyValuePair.Create(key as IConversationKey, value as IConversationValue);
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
                var iterator = _conversationsDb.Iterate();
                while (iterator.GetNext(out _))
                {
                    yield return iterator.GetKey();
                }
            }
        }

        /// <summary>
        /// Gets the number of frames.
        /// </summary>
        public long FrameCount => _framesDb.EntryCount;

        /// <summary>
        /// Gets the number of conversations.
        /// </summary>
        public int ConversationCount => (int)_conversationsDb.EntryCount;

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
