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
    public class FasterFlowTable : IDisposable
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
        public FasterFlowTable(string folder)
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
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        public void LoadFromStream(Stream stream, CancellationToken cancellationToken, Action<long, Packet> onNextPacket)
        {
            using var conversationsSession = _conversationsDb.NewSession();
            using var framesSession = _framesDb.NewSession();
            using var captureReader = new CaptureFileReader(stream);

            unsafe long UpsertFrame(RawFrame frame, out FlowKey flowKey)
            {
                var size = FrameValue.ComputeLength(frame.IncludedLength);
                using var buffer = _memoryPool.Rent(size);
                fixed (void* bufferPtr = buffer.Memory.Span)
                {
                    ref var frameValue = ref Unsafe.AsRef<FrameValue>(bufferPtr);
                    var frameBytesSpan = new Span<byte>(Unsafe.AsPointer(ref frameValue.Bytes), frame.IncludedLength);
                    var address = captureReader.Position;
                    captureReader.ReadFrameBytes(frameBytesSpan);

                    // @PERF: if PacketDotNet supports Span<byte> or Memory<byte> then we do not need to 
                    // allocated buffer for parsing the packet and obtaining the key.
                    var packet = Packet.ParsePacket(frame.LinkLayer, frameBytesSpan.ToArray());
                    flowKey =  _packetKeyProvider.GetKey(packet);

                    frameValue.Meta.Ticks = frame.Ticks;
                    frameValue.Meta.IncludedLength = (ushort) frame.IncludedLength;
                    frameValue.Meta.OriginalLength = (ushort) frame.OriginalLength;
                    frameValue.Meta.LinkLayer = (ushort) frame.LinkLayer;
                    frameValue.Meta.FlowKeyHash = flowKey.GetHashCode64();

                    onNextPacket?.Invoke(frame.Ticks, packet);

                    var frameKey = new FrameKey { Address = address };
                    framesSession.Upsert(ref frameKey, ref frameValue, null, 0);
                    return address;
                }
            }

            while (captureReader.GetNextFrameHeader(out var frame))
            {
                if (cancellationToken.IsCancellationRequested) break;
                var frameAddress = UpsertFrame(frame, out var frameKey);
                var input = new ConversationInput
                {
                    FrameAddress = frameAddress,
                    FrameSize = frame.OriginalLength,
                    FrameTicks = frame.Ticks,
                    FrameKey = frameKey
                };
                var conversationKey = GetConversationKey(frameKey);
                conversationsSession.RMW(ref conversationKey, ref input, null, 0);
            }

            // complete operations
            conversationsSession.CompletePending(true);
            framesSession.CompletePending(true);
            // persist results 
            _conversationsDb.Log.Flush(true);
            _framesDb.Log.Flush(true);
        }

        /// <summary>
        /// Gets the conversation key from the flow key.
        /// <para/>
        /// It uses port numbers to determine the conversation key.
        /// The assumption is that client has greater port number than server.
        /// </summary>
        /// <param name="frameKey"></param>
        /// <returns></returns>
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

        public IEnumerable<Result> ProcessConversations<Result>(IEnumerable<IConversationKey> keys, IBiflowProcessor<Result> processor)
        {
            using var conversationsSession = _conversationsDb.NewSession();
            using var framesSession = _framesDb.NewSession();
            unsafe Result ProcessConversation(ref ConversationKey key)
            {

                var input = new ConversationInput();
                var output = new ConversationOutput();
                _ = conversationsSession.Read(ref key, ref input, ref output, null, 0);

                var frameKey = new FrameKey();
                var frameInput = new FrameInput() { Pool = MemoryPool<byte>.Shared };
                var frameList = new List<IMemoryOwner<byte>>();
                for(int i = 0; i < output.Value.FrameCount; i++)
                {
                    var frameOutput = new FrameOutput();
                    frameKey.Address = output.Value.FrameAddresses[i];
                    _ = framesSession.Read(ref frameKey, ref frameInput, ref frameOutput, null, 0);
                    frameList.Add(frameOutput.FrameBuffer);
                }
                var result = processor.Invoke(key.FlowKey, frameList.Select(x=>x.Memory));
                foreach (var m in frameList) m?.Dispose();
                return result;
            }
            Result ProcessConversationSafe(IConversationKey key)
            {
                if (key is ConversationKey _key)
                {
                    return ProcessConversation(ref _key);
                }
                return default;
            }
            return keys.Select(key => ProcessConversationSafe(key));
        }

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
    }
}
