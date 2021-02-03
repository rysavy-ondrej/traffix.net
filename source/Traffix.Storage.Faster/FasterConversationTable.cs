using FASTER.core;
using Microsoft.Extensions.Configuration.Json;
using PacketDotNet;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Traffix.Core.Flows;
using Traffix.Data;
using Traffix.Providers.PcapFile;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// Implements a flow table backed by the FASTER Key-Value Database. This class provides API for 
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
        private MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;

        /// <summary>
        /// The packet key provider used for finding keys in frames. 
        /// By default it uses <see cref="PacketKeyProvider"/> based on PacketDotNet package.
        /// </summary>
        IFlowKeyProvider<FlowKey, Packet> _packetKeyProvider = new PacketKeyProvider();

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
                frameBytes.CopyTo(frameValue.Slice(Unsafe.SizeOf<FrameMetadata>()));
                client.Put(frameKey.Address, ref frameValue);
            }
            else
            
            {
                using var buffer = _memoryPool.Rent(bufferSize);
                fixed (byte* bufferPtr = buffer.Memory.Span)
                {
                    var frameValue = new Span<byte>(bufferPtr, bufferSize);
                    Unsafe.Copy(bufferPtr, ref frameMetadata);
                    frameBytes.CopyTo(frameValue.Slice(Unsafe.SizeOf<FrameMetadata>()));
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
                        long frameAddress = output.Value.FrameAddresses[i];
                        if (framesClient.TryGet(frameAddress, out var frameOutput))
                        {
                            frameList.Add(frameOutput);
                        }
                    }
                    var result = processor.Invoke(key.FlowKey, frameList.Select(x => new Memory<byte>(x)).ToList());
                    // we have to dispose buffers rented from memory pool before we can return a result
                    return result;
                }
                else
                {
                    return default;
                }
            }
            return keys.Select(key => ProcessConversation(ref key));
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

        class FrameProcessor<TResult> : IEntryProcessor<long, Memory<byte>, TResult>
        {
            private readonly Func<FrameMetadata, Memory<byte>, ProcessingResult<TResult>> _processor;

            public FrameProcessor(Func<FrameMetadata, Memory<byte>, ProcessingResult<TResult>> processor)
            {
                _processor = processor;
            }

            unsafe public ProcessingState Invoke(ref long key, ref Memory<byte> memory, out TResult result)
            {
                FrameMetadata frameMetadata = default;
                var frameBytes = RawFramesStore.GetFrameFromMemory(ref memory, ref frameMetadata);
                var x = _processor.Invoke(frameMetadata, frameBytes);
                result = x.Result;
                return x.State;
            }
        }

        /// <summary>
        /// Applies the provided <paramref name="processor"/> to all frames in the table yielding to a collection of results produced by the processor. 
        /// </summary>
        /// <typeparam name="TResult">The type of resulting objects.</typeparam>
        /// <param name="processor">The processor function to apply.</param>
        /// <returns>A collection of results produced by the processor.</returns>
        public IEnumerable<TResult> ProcessFrames<TResult>(Func<FrameMetadata, Memory<byte>, ProcessingResult<TResult>> processor)
        {
            return _framesStore.ProcessEntries<TResult>(new FrameProcessor<TResult>(processor));
        }


        /// <summary>
        /// Provides enumerable for all stored conversations.
        /// </summary>
        /// <returns>An enumerable of conversations.</returns>
        public IEnumerable<KeyValuePair<ConversationKey, ConversationValue>> Conversations
        {
            get
            {
                return _conversationsStore.ProcessEntries<KeyValuePair<ConversationKey, ConversationValue>>(new KeyValueConversationProcessor());
            }
        }

        /// <summary>
        /// Gets all stored conversation keys.
        /// </summary>
        /// <returns>An enumerable of conversation keys.</returns>
        public IEnumerable<ConversationKey> ConversationKeys
        {
            get
            {
                return _conversationsStore.ProcessEntries<KeyValuePair<ConversationKey, ConversationValue>>(new KeyValueConversationProcessor()).Select(x => x.Key);
            }
        }

        public int FramesCount => (int)_framesStore.EntryCount;

        public int ConversationsCount => (int)_conversationsStore.EntryCount;


        public IEnumerable<RawFrame> GetRawFrames()
        {
            int frameNumber = 0;
            ProcessingResult<RawFrame> GetRawFrame(FrameMetadata meta, Memory<byte> frame)
            {
                return new ProcessingResult<RawFrame>
                {
                    State = ProcessingState.Success,
                    Result = new RawFrame((LinkLayers)meta.LinkLayer, ++frameNumber, meta.Ticks, 0, meta.OriginalLength, frame.ToArray())
                };
            }
            return ProcessFrames(GetRawFrame);
        }

        public IEnumerable<RawFrame> GetFrames(ConversationKey conversation)
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
            private ConversationsStore.KeyValueStoreClient _conversationsStoreClient;
            private RawFramesStore.ClientSession _framesStoreClient;
            private readonly int _autoFlushRequests;
            private bool _closed;
            private int _outstandingRequests;
            internal FrameStreamer(FasterConversationTable table, 
                ConversationsStore.KeyValueStoreClient conversationsStoreClient,
                RawFramesStore.ClientSession framesStoreClient,
                int autoFlush)
            {
                _table = table;
                _conversationsStoreClient = conversationsStoreClient;
                _framesStoreClient = framesStoreClient;
                _autoFlushRequests = autoFlush;
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
            /// <param name="address">Offset/key of the frame. Can be used to refer to the frame to the external data source.</param>
            /// <exception cref="InvalidOperationException">Raises when the stremer is closed.</exception>
            public void AddFrame(RawFrame frame, long address)
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

                var frameKey = new FrameKey { Address = address };
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
                if (_outstandingRequests > _autoFlushRequests) CompletePending();
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
                this._provider = new JsonConfigurationProvider(new JsonConfigurationSource { Path = Path.GetFullPath(path) }); ;
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
                System.IO.File.WriteAllText(_provider.Source.Path, json);
                return this;
            }

            public Configuration Load()
            {
                using var stream = File.OpenRead(_provider.Source.Path);
                _provider.Load(stream);
                return this;
            }
        }

        private class KeyValueConversationProcessor : IEntryProcessor<ConversationKey, ConversationValue, KeyValuePair<ConversationKey, ConversationValue>>
        {
            public ProcessingState Invoke(ref ConversationKey key, ref ConversationValue value, out KeyValuePair<ConversationKey, ConversationValue> result)
            {
                result = KeyValuePair.Create(key, value);
                return ProcessingState.Success;
            }
        }
        private class KeyConversationProcessor : IEntryProcessor<ConversationKey, ConversationValue, ConversationKey>
        {
            public ProcessingState Invoke(ref ConversationKey key, ref ConversationValue value, out ConversationKey result)
            {
                result = key;
                return ProcessingState.Success;
            }
        }
        private class RawFrameProcessor : IConversationProcessor<IEnumerable<RawFrame>>
        {
            public IEnumerable<RawFrame> Invoke(FlowKey flowKey, ICollection<Memory<byte>> frames)
            {
                FrameMetadata meta = new FrameMetadata();
                int frameNumber = 0;
                foreach (var frame in frames)
                {
                    var data = ConversationProcessor.GetFrameFromMemory(frame, ref meta);
                    yield return new RawFrame((PacketDotNet.LinkLayers)meta.LinkLayer, ++frameNumber, meta.Ticks, 0, meta.OriginalLength, data.ToArray());
                }
            }
        }
    }
}
