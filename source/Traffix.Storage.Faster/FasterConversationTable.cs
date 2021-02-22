using FASTER.core;
using Microsoft.Extensions.Configuration.Json;
using PacketDotNet;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Traffix.Core;
using Traffix.Core.Flows;
using Traffix.Data;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// Implements a flow table backed by the FASTER key-value store. This class provides API for 
    /// adding packets and accessing conversations and frames.
    /// </summary>
    public class FasterConversationTable : IDisposable
    {
        private readonly string _rootFolder;
        private readonly Configuration _config;
        private readonly ConversationsStore _conversationsStore;
        private readonly RawFramesStore _framesStore;

        /// <summary>
        /// Memory pool used for buffer allocations in this class.
        /// </summary>
        private readonly MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;

        /// <summary>
        /// The packet key provider used for finding keys in frames. 
        /// By default it uses <see cref="PacketKeyProvider"/> based on PacketDotNet package.
        /// </summary>
        readonly IFlowKeyProvider<FlowKey, Packet> _packetKeyProvider = new PacketKeyProvider();

        private bool _disposedValue;
        /// <summary>
        /// The default number for avg. frames per flow. This is according 
        /// to http://rboutaba.cs.uwaterloo.ca/Papers/Conferences/2004/Kim04.pdf
        /// </summary>
        private const double DefaultFlowsFrameRatio = 0.04;
            
        /// <summary>
        /// Creates a new conversation table.
        /// </summary>
        /// <param name="folder">The destination folder to persist conversation table.</param>
        /// <param name="framesCapacity">The expected frame capacity of the table. The conversation table offerst bets performance and space 
        /// if the actual frame number is around this value.</param>
        /// <param name="flowFrameRatio">The expected ratio between flows and frames. It can be computed as "1 / average frames per flow".</param>
        /// <returns>New instance of the conversation table.</returns>
        public static FasterConversationTable Create(string folder, long framesCapacity, double flowFrameRatio = DefaultFlowsFrameRatio)
        {
            // ensure that root folder does exist:
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var config = new Configuration(Path.Combine(folder, "settings.json"))
            {
                FramesCapacity = framesCapacity,
                ConversationsCapacity = (long)(framesCapacity * flowFrameRatio)
            };
            config.Save();  // need to save as we create a new configuration
            var table = new FasterConversationTable(folder, config);
            table._conversationsStore.InitAndRecover();
            table._framesStore.InitAndRecover();
            return table;
        }

        /// <summary>
        /// Opens an existing conversation table persisted in the specified folder.
        /// </summary>
        /// <param name="folder">The folder containing conversation table files.</param>
        /// <returns>New instance of the conversation table.</returns>
        public static FasterConversationTable Open(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException($"Specified folder '{folder}' not found.");
            }
            var config = new Configuration(Path.Combine(folder, "settings.json")).Load();
            var table = new FasterConversationTable(folder, config);
            table._conversationsStore.InitAndRecover();
            table._framesStore.InitAndRecover();
            return table;
        }

 

        /// <summary>
        /// Creates a store that uses the specified folder for saving data.
        /// If the folder does not exist, it will be created.
        /// </summary>
        /// <param name="folder"></param>
        private FasterConversationTable(string folder,  Configuration config)
        {
            _rootFolder = folder;
            _config = config;
            _conversationsStore = new ConversationsStore(Path.Combine(folder, "conversations"), config.ConversationsCapacity);
            _framesStore = new RawFramesStore(Path.Combine(folder, "frames"), (int)config.FramesCapacity);
        }
        #region Dispose Implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
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
        /// Persists the current state. 
        /// <para/>
        /// After new data are added by the <seealso cref="FrameStreamer"/>
        /// it is necessary to call <seealso cref="SaveChanges"/> to 
        /// save them in the persistent location. If this is not called then 
        /// data are volatile and cannot be restored.
        /// </summary>
        public void SaveChanges()
        {
            _conversationsStore.Checkpoint();
            _framesStore.Checkpoint();
        }

        /// <summary>
        /// Ges the flow key for the given <paramref name="frame"/>.
        /// </summary>
        /// <param name="linkLayer">The link layer of the frame.</param>
        /// <param name="frame">The frame bytes.</param>
        /// <returns>The flow kye of the frame.</returns>
        public FlowKey GetFlowKey(LinkLayers linkLayer, Span<byte> frame)
        {
            var packet = Packet.ParsePacket(linkLayer, frame.ToArray());
            return _packetKeyProvider.GetKey(packet);
        }
        /// <summary>
        /// Gets the flow key for the given <paramref name="packet"/>.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public FlowKey GetFlowKey(Packet packet)
        {
            var packetKey = _packetKeyProvider.GetKey(packet);
            return packetKey;
        }

        /// <summary>
        /// Updates the conversation with the provided frame metadata.
        /// </summary>
        /// <param name="client">The client of the conversation store used to update the conversation record.</param>
        /// <param name="flowKey">The flow key of the conversation.</param>
        /// <param name="input">The information to update the conversation.</param>
        /// <returns>true if the conversation was update; false if new conversation was created.</returns>
        private bool UpdateConversationWithFrame(ConversationsStore.KeyValueStoreClient client, ref FlowKey flowKey, ref ConversationInput input)
        { 
            var conversationKey = GetConversationKey(flowKey);
            return client.Update(ref conversationKey, ref input);
        }

        /// <summary>
        /// Insert a new frame to the frame store.
        /// </summary>
        /// <param name="client">The client used to insert the frame to the store.</param>
        /// <param name="frame"></param>
        /// <param name="bytes"></param>
        /// <param name="address"></param>
        /// <param name="frameFlowKey"></param>
        /// <returns></returns>
        private unsafe void InsertFrame(RawFramesStore.ClientSession client, ref FrameKey frameKey, ref FlowKey frameFlowKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
        {
            var bufferSize = Unsafe.SizeOf<FrameMetadata>() + frameBytes.Length;
            
 
            if (bufferSize < 128)
            {
                var bufferPtr = stackalloc byte[bufferSize];

                var frameValue = new Span<byte>(bufferPtr, bufferSize);
                Unsafe.Copy(bufferPtr, ref frameMetadata);
                frameBytes.CopyTo(frameValue[Unsafe.SizeOf<FrameMetadata>()..]);
                client.Put(frameKey.Address, ref frameValue);
            }
            else
            
            {
                using var buffer = _memoryPool.Rent(bufferSize);
                fixed (byte* bufferPtr = buffer.Memory.Span)
                {
                    var frameValue = new Span<byte>(bufferPtr, bufferSize);
                    Unsafe.Copy(bufferPtr, ref frameMetadata);
                    frameBytes.CopyTo(frameValue[Unsafe.SizeOf<FrameMetadata>()..]);
                    client.Put(frameKey.Address, ref frameValue);
                }
            }
        }

        /// <summary>
        /// Gets the streamer object that can be used for the batch insertation of new frames to the table.
        /// Call <see cref="FrameStreamer.Close"/> to complete all pending operations.
        /// <para>
        /// The FrameStreamer should be properly disposed when no longer in use. It is possible to open multiple streamers 
        /// to insert data.
        /// </para>
        /// </summary>
        /// <returns>The new streamer instance.</returns>
        public FrameStreamer GetStreamer(int autoFlush = 1024)
        {
            var conversationsClient = _conversationsStore.GetClient() ?? throw new InvalidOperationException("Cannot create conversation session.");
            var framesClient = _framesStore.GetClient() ?? throw new InvalidOperationException("Cannot create conversation session.");
            return new FrameStreamer(this, conversationsClient, framesClient, autoFlush);
        }


        /// <summary>
        /// Gets the conversation key from the flow key.
        /// <para/>
        /// It uses port numbers to determine the conversation key.
        /// The assumption is that client has greater port number than server.
        /// </summary>
        /// <param name="flowKey">The flow key.</param>
        /// <returns>The conversation key forthe given flow key.</returns>
        private static ConversationKey GetConversationKey(FlowKey flowKey)
        {
            if (flowKey.SourcePort > flowKey.DestinationPort)
            {
                return new ConversationKey(flowKey);
            }
            else
            {
                return new ConversationKey(flowKey.Reverse());
            }
        }

        /// <summary>
        /// Processes conversations given by their <paramref name="keys"/> using the specigied <paramref name="processor"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of resulting objects produced by the processor.</typeparam>
        /// <param name="keys">The collection of conversation keys to process.</param>
        /// <param name="processor">The conversation processor used to transform the conversation to the object of type <typeparamref name="TResult"/>.</param>
        /// <returns>The enumerable collection of resulting objects produced by the <paramref name="processor"/>.</returns>
        public IEnumerable<TResult> ProcessConversations<TResult>(IEnumerable<ConversationKey> keys, IConversationProcessor<TResult> processor)
        {
            using var conversationsClient = _conversationsStore.GetClient() ?? throw new InvalidOperationException("Cannot create conversations DB session.");
            using var framesClient = _framesStore.GetClient() ?? throw new InvalidOperationException("Cannot create frames DB session.");

            TResult ProcessConversation(ref ConversationKey key)
            {
                ConversationInput input = new ConversationInput();
                ConversationOutput output = new ConversationOutput();
                if (conversationsClient.TryGet(ref key, ref input, ref output))
                {
                    var frameList = new List<byte[]>();
                    for (int i = 0; i < output.Value.FrameCount; i++)
                    {
                        ulong frameAddress = output.Value.FrameAddresses[i];
                        if (framesClient.TryGet(frameAddress, out var frameOutput))
                        {
                            frameList.Add(frameOutput);
                        }
                    }
                    var result = processor.Invoke(key.FlowKey, frameList.Select(x => new Memory<byte>(x)).ToList());
                    return result;
                }
                else
                {
                    return default;
                }
            }
            return keys.Select(key => ProcessConversation(ref key));
        }

        public IEnumerable<TResult> ProcessFrames<TResult>(IEnumerable<FrameKey> keys, IFrameProcessor<TResult> processor)
        {
            using var framesClient = _framesStore.GetClient() ?? throw new InvalidOperationException("Cannot create frames DB session.");
            foreach (var key in keys)
            {
                if (framesClient.TryGet(key.Address, out var bytes))
                {
                    FrameKey frameKey = key;
                    FrameMetadata frameMetadata = default;
                    var frameBytes = FrameMetadata.GetFrameFromMemory(bytes, ref frameMetadata);
                    var result = processor.Invoke(ref frameKey, ref frameMetadata, frameBytes);
                    yield return result;
                }
            }
        }

        /// <summary>
        /// Gets the number of frames in the table.
        /// </summary>
        public int FramesCount => _framesStore.GetRecordCount();

        /// <summary>
        /// Gets the number of conversations in the table.
        /// </summary>
        public int ConversationsCount => (int)_conversationsStore.GetRecordCount();

        /// <summary>
        /// Gets all stored conversation keys.
        /// </summary>
        public IEnumerable<ConversationKey> ConversationKeys
        {
            get
            {
                return _conversationsStore.ProcessEntries<KeyValuePair<ConversationKey, ConversationValue>>(new KeyValueConversationProcessor()).Select(x => x.Key);
            }
        }

        /// <summary>
        /// Gets all stored conversations.
        /// </summary>
        public IEnumerable<(ConversationKey Key, long FirstSeen, long LastSeen, uint Packets, ulong Octets)> Conversations
        {
            get
            {
                return _conversationsStore.ProcessEntries<KeyValuePair<ConversationKey, ConversationValue>>(new KeyValueConversationProcessor()).Select(x => (x.Key, x.Value.FirstSeen, x.Value.LastSeen, x.Value.Packets, x.Value.Octets));
            }
        }

        /// <summary>
        /// Gets all frame keys. 
        /// </summary>
        public IEnumerable<FrameKey> FrameKeys =>
            _framesStore.ProcessEntries(new FrameKeyProcessor());

        /// <summary>
        /// Frame streamer is responsible for streaming external frame into table. 
        /// It uses mapping keys to conversations responsible for the frame provides by the parent table.
        /// It achieves optimized resource utilization by properly buffering resources and updates. 
        /// </summary>
        public class FrameStreamer : IDisposable
        {
            private readonly FasterConversationTable _table;
            private readonly ConversationsStore.KeyValueStoreClient _conversationsStoreClient;
            private readonly RawFramesStore.ClientSession _framesStoreClient;
            private readonly int _autoFlushRecordCount;
            private bool _closed;
            private int _outstandingRequests;
            internal FrameStreamer(FasterConversationTable table, 
                ConversationsStore.KeyValueStoreClient conversationsStoreClient,
                RawFramesStore.ClientSession framesStoreClient,
                int autoFlushRecordCount)
            {
                _table = table;
                _conversationsStoreClient = conversationsStoreClient;
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
            public void AddFrame(RawFrame frame)
            {
                if (_closed) throw new InvalidOperationException("Cannot add new data. The stream is closed.");
                var frameFlowKey = _table.GetFlowKey(frame.LinkLayer, frame.Data);

                var frameMeta = new FrameMetadata   // stack allocated struct 
                {
                    Ticks = frame.Ticks,
                    OriginalLength = (ushort)frame.OriginalLength,
                    LinkLayer = (ushort)frame.LinkLayer,
                    FlowKeyHash = frameFlowKey.GetHashCode64()
                };

                var frameKey = new FrameKey(frame.Ticks, (uint)frame.Number);
                 _table.InsertFrame(_framesStoreClient, ref frameKey, ref frameFlowKey, ref frameMeta, frame.Data);

                var conversationUpdate = new ConversationInput  // stack allocated struct
                {
                    FrameAddress = frameKey.Address,
                    FrameSize = frame.OriginalLength,
                    FrameTicks = frame.Ticks,
                    FrameKey = frameFlowKey
                };

                _table.UpdateConversationWithFrame(_conversationsStoreClient, ref frameFlowKey, ref conversationUpdate);
                
                _outstandingRequests++;
                if (_outstandingRequests > _autoFlushRecordCount) CompletePending();
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
                _conversationsStoreClient.CompletePending(true);
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
                ((IDisposable)_conversationsStoreClient).Dispose();
                ((IDisposable)_framesStoreClient).Dispose();
            }
        }
        public sealed class Configuration 
        {
            private JsonConfigurationProvider _provider;

            public Configuration(string path)
            {
                _provider = new JsonConfigurationProvider(new JsonConfigurationSource { Path = Path.GetFullPath(path) }); ;
            }
            public long FramesCapacity
            {
                get
                {
                    return _provider.TryGet(nameof(FramesCapacity), out var value) ? long.Parse(value) : 100000;
                }
                set
                {
                    _provider.Set(nameof(FramesCapacity), value.ToString());
                }
            }
            public long ConversationsCapacity
            {
                get
                {
                    return _provider.TryGet(nameof(ConversationsCapacity), out var value) ? long.Parse(value) : 5000;
                }
                set
                {
                    _provider.Set(nameof(ConversationsCapacity), value.ToString());
                }
            }

            /// <summary>
            /// Saves the configration file.
            /// </summary>
            public Configuration Save()
            {
                string json = System.Text.Json.JsonSerializer.Serialize<Configuration>(this, new System.Text.Json.JsonSerializerOptions { WriteIndented = true } );
                File.WriteAllText(_provider.Source.Path, json);
                return this;
            }

            public Configuration Load()
            {
                using var stream = File.OpenRead(_provider.Source.Path);
                _provider.Load(stream);
                return this;
            }
        }

        #region Private entry processors:
        class KeyValueConversationProcessor : IEntryProcessor<ConversationKey, ConversationValue, KeyValuePair<ConversationKey, ConversationValue>>
        {
            public ProcessingState Invoke(ref ConversationKey key, ref ConversationValue value, out KeyValuePair<ConversationKey, ConversationValue> result)
            {
                result = KeyValuePair.Create(key, value);
                return ProcessingState.Success;
            }
        }
        class KeyConversationProcessor : IEntryProcessor<ConversationKey, ConversationValue, ConversationKey>
        {
            public ProcessingState Invoke(ref ConversationKey key, ref ConversationValue value, out ConversationKey result)
            {
                result = key;
                return ProcessingState.Success;
            }
        }

        class FrameKeyProcessor : IEntryProcessor<ulong, Memory<byte>, FrameKey>
        {
            public ProcessingState Invoke(ref ulong key, ref Memory<byte> value, out FrameKey result)
            {
                result = new FrameKey(key);
                return ProcessingState.Success;
            }
        }
        #endregion
        public class RawFrameConversationProcessor : IConversationProcessor<IEnumerable<RawFrame>>
        {
            public IEnumerable<RawFrame> Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
            {
                var _frameProcessor = new RawFrameProcessor();
                FrameMetadata frameMetadata = default;
                uint frameNumber = 0;
                foreach (var frame in frames)
                {
                    var frameBytes = FrameMetadata.GetFrameFromMemory(frame, ref frameMetadata);
                    var frameKey = new FrameKey(frameMetadata.Ticks, ++frameNumber);
                    yield return _frameProcessor.Invoke(ref frameKey, ref frameMetadata, frameBytes.ToArray());
                }
            }
        }
        public class PacketConversationProcessor : IConversationProcessor<(FlowKey Key, IReadOnlyCollection<Packet> Packets)>
        {
            (FlowKey Key, IReadOnlyCollection<Packet> Packets) IConversationProcessor<(FlowKey Key, IReadOnlyCollection<Packet> Packets)>.Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames)
            {
                Packet GetPacket(Memory<byte> frameBuffer)
                {
                    FrameMetadata frameMetadata = default;
                    var frameBytes = FrameMetadata.GetFrameFromMemory(frameBuffer, ref frameMetadata);
                    return Packet.ParsePacket((LinkLayers)frameMetadata.LinkLayer, frameBytes.ToArray());
                }
                var packets = frames.Select(GetPacket);
                return (flowKey, packets.ToArray());
            }
        }

        public class RawFrameProcessor : IFrameProcessor<RawFrame>
        {
            public RawFrame Invoke(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
            {
                return new RawFrame((LinkLayers)frameMetadata.LinkLayer, (int)frameKey.Number, frameMetadata.Ticks, 0, frameMetadata.OriginalLength, frameBytes.ToArray());
            }
        }
        public class PacketFrameProcessor : IFrameProcessor<Packet>
        {
            public Packet Invoke(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
            {
                return Packet.ParsePacket((LinkLayers)frameMetadata.LinkLayer, frameBytes.ToArray());
            }
        }
    }

    public static class ConversationTableWindowed
    {
        /// <summary>
        /// Gets conversations splitted in windows of the specified duration.
        /// </summary>
        /// <param name="timeOrigin">The time origin.</param>
        /// <param name="windowSpan">The duration of each window.</param>
        /// <returns>Grouping consisting of non-empty windows. Each window has a list of conversation keys that were active in window's interval.</returns>
        public static IEnumerable<IGrouping<DateTime, ConversationKey>> GroupByWindow(this IEnumerable<(ConversationKey Key, long FirstSeen, long LastSeen, uint Packets, ulong Octets)> conversations, DateTime timeOrigin, TimeSpan windowSpan)
        {
            var processor = new WindowConversationProcessor(timeOrigin, windowSpan);
            DateTime GetTime(int i)
            {
                return new DateTime(windowSpan.Ticks * i + timeOrigin.Ticks);
            }
            IEnumerable<(DateTime, ConversationKey)> GetRecords((ConversationKey key, int first, int last) record)
            {
                for (int i = record.first; i <= record.last; i++)
                {
                    yield return (GetTime(i), record.key);
                }
            }
            var windowConversations = conversations.Select(processor.Invoke).SelectMany(GetRecords);
            return windowConversations.GroupBy(x => x.Item1, x => x.Item2).OrderBy(x => x.Key);
        }
        /// <summary>
        /// Provides window operator on the collection of conversations.
        /// </summary>
        class WindowConversationProcessor
        {
            private readonly DateTime _timeOrigin;
            private readonly TimeSpan _windowSpan;

            /// <summary>
            /// Creates the processor using the specified parameters.
            /// </summary>
            /// <param name="timeOrigin"></param>
            /// <param name="windowSpan"></param>
            public WindowConversationProcessor(DateTime timeOrigin, TimeSpan windowSpan)
            {
                this._timeOrigin = timeOrigin;
                this._windowSpan = windowSpan;
            }

            public (ConversationKey key, int first, int last) Invoke((ConversationKey Key, long FirstSeen, long LastSeen, uint Packets, ulong Octets) value)
            {
                var firstSeen = value.FirstSeen;
                var lastSeen = value.LastSeen;
                var firstSeenOffset = firstSeen - _timeOrigin.Ticks;
                var lastSeenOffset = lastSeen - _timeOrigin.Ticks;
                var firstWindow = firstSeenOffset / _windowSpan.Ticks;
                var lastWindow = lastSeenOffset / _windowSpan.Ticks;
                return (value.Key, (int)firstWindow, (int)lastWindow);
            }
        }
    }
}
