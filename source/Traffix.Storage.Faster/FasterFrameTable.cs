using FASTER.core;
using Microsoft.StreamProcessing;
using PacketDotNet;
using System;
using System.Buffers;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Traffix.Core;
using Traffix.Core.Flows;
using Traffix.Data;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// Faster conversation table enables to store full packet captures in  a key-value database.
    /// The store is optimized for fast insert and access.
    /// It should be faster to read frames from this table than original pcap file.
    /// </summary>
    public partial class FasterFrameTable : IDisposable
    {
        private readonly string _rootFolder;
        private readonly Configuration _config;
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
        /// Creates a new conversation table.
        /// </summary>
        /// <param name="folder">The destination folder to persist conversation table.</param>
        /// <param name="framesCapacity">The expected frame capacity of the table. The conversation table offerst bets performance and space 
        /// if the actual frame number is around this value.</param>
        /// <param name="flowFrameRatio">The expected ratio between flows and frames. It can be computed as "1 / average frames per flow".</param>
        /// <returns>New instance of the conversation table.</returns>
        public static FasterFrameTable Create(string folder, long framesCapacity)
        {
            // ensure that root folder does exist:
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var config = new Configuration(Path.Combine(folder, "settings.json"))
            {
                FramesCapacity = framesCapacity,
            };
            config.Save();  // need to save as we create a new configuration
            var table = new FasterFrameTable(folder, config);
            table._framesStore.InitAndRecover();
            return table;
        }

        /// <summary>
        /// Opens an existing conversation table persisted in the specified folder.
        /// </summary>
        /// <param name="folder">The folder containing conversation table files.</param>
        /// <returns>New instance of the conversation table.</returns>
        public static FasterFrameTable Open(string folder)
        {
            if (!Directory.Exists(folder))
            {
                throw new DirectoryNotFoundException($"Specified folder '{folder}' not found.");
            }
            var config = new Configuration(Path.Combine(folder, "settings.json")).Load();
            var table = new FasterFrameTable(folder, config);
            table._framesStore.InitAndRecover();
            return table;
        }



        /// <summary>
        /// Creates a store that uses the specified folder for saving data.
        /// If the folder does not exist, it will be created.
        /// </summary>
        /// <param name="folder"></param>
        private FasterFrameTable(string folder, Configuration config)
        {
            _rootFolder = folder;
            _config = config;
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
        /// Enables to subscribe to the stream of frames produced by the current table.
        /// <para/>
        /// It is possible to create an observable as follows:
        /// <code>
        /// FasterFrameTable table;
        /// var observable = Observable.Create(table.GetSubscriber(processor));
        /// </code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="observer"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public Func<IObserver<TValue>, IDisposable> GetSubscriber<TValue>(FrameProcessor<TValue> processor)
        {
            return observer => new FrameProvider<TValue>(_framesStore, observer, processor);
        }

        /// <summary>
        /// Provides observable sequence of frames processed by the application of the given <paramref name="processor"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processor"></param>
        /// <returns></returns>
        public IObservable<TValue> GetObservable<TValue>(FrameProcessor<TValue> processor)
        {
            return Observable.Create(GetSubscriber(processor));
        }

        class TickFrameProcessor<TValue>
        {
            private readonly FrameProcessor<TValue> processor;

            public TickFrameProcessor(FrameProcessor<TValue> processor)
            {
                this.processor = processor;
            }

            public (long Ticks, TValue Value) Invoke(ref FrameKey frameKey, ref FrameMetadata frameMetadata, Span<byte> frameBytes)
            {
                return (frameMetadata.Ticks, processor(ref frameKey, ref frameMetadata, frameBytes));
            }
        }
        /// <summary>
        /// Provides streamable sequence of frames processed by the application of the given <paramref name="processor"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="processor"></param>
        /// <returns></returns>
        public IStreamable<Microsoft.StreamProcessing.Empty, TValue> GetStreamable<TValue>(FrameProcessor<TValue> processor)
        {
            var observable = Observable.Create(GetSubscriber(new TickFrameProcessor<TValue>(processor).Invoke)).Select(x => new StreamEvent<TValue>(x.Ticks, x.Ticks + 1, x.Value));
            return Streamable.ToStreamable(observable);
        }

        class FrameProvider<TValue> : IDisposable
        {
            private CancellationTokenSource _cancel;
            private RawFramesStore _framesStore;
            private readonly IObserver<TValue> _observer;
            private readonly FrameProcessor<TValue> _processor;

            public FrameProvider(RawFramesStore framesStore, IObserver<TValue> observer, FrameProcessor<TValue> processor)
            {
                this._cancel = new CancellationTokenSource();
                this._framesStore = framesStore;
                this._observer = observer;
                this._processor = processor;
                // The frames generator  will be run on another thread
                Task.Factory.StartNew(new Action(Generate));
            }

            public void Dispose()
            {
                _cancel.Cancel();
            }
            void Generate()
            {
                void onNextFrame(ref ulong key, ref SpanByte value)
                {
                    FrameKey frameKey = new FrameKey(key);
                    FrameMetadata frameMetadata = default;
                    var frameBytes = FrameMetadata.FromBytes(value.AsSpan(), ref frameMetadata);
                    _observer.OnNext(_processor(ref frameKey, ref frameMetadata, frameBytes));
                }
                _framesStore.ProcessEntries(onNextFrame, _cancel.Token);
                _observer.OnCompleted();
            }
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
            var framesClient = _framesStore.GetClient() ?? throw new InvalidOperationException("Cannot create conversation session.");
            return new FrameStreamer(this, framesClient, autoFlush);
        }
    }
}
