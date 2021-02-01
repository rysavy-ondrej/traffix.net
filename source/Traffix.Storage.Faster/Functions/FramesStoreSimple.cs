using FASTER.core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Traffix.Data;
using static Traffix.Storage.Faster.FasterConversationTable;

namespace Traffix.Storage.Faster
{
    public class FramesStoreSimple : IDisposable
    {
        private readonly string _dataFolder;
        private readonly int _logSizeBits;
        private IDevice _logDevice;
        private FasterKV<long, SpanByte>? _fasterKvh;

        public long EntryCount => _fasterKvh.EntryCount;

        public FramesStoreSimple(string folder, long capacity)
        {
            _dataFolder = folder;
            _logSizeBits = (int)Math.Log(capacity, 2) + 1;
        }
        public void Dispose()
        {
            _fasterKvh?.Dispose();
            _logDevice.Dispose();
            
        }

        /// <summary>
        /// Performs the incremental checkpoint, which cause 
        /// to persist unsaved changes.
        /// </summary>
        /// <returns>The Id of the checkpoint created.</returns>
        public Guid Checkpoint()
        {
            if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
            _fasterKvh.TakeFullCheckpoint(out Guid token);
            _fasterKvh.CompleteCheckpointAsync().GetAwaiter().GetResult();
            return token;
        }

        public ClientSession GetClient()
        {
            var s = _fasterKvh?.For(new Functions()).NewSession<Functions>();
            return new ClientSession(this, s);
        }
        public bool InitAndRecover()
        {
            var logSize = 1L << _logSizeBits;
            _logDevice = Devices.CreateLogDevice(@$"{this._dataFolder}\data\Store-hlog.log", preallocateFile: false);

            this._fasterKvh = new FasterKV<long, SpanByte>
                (
                    size: logSize,
                    logSettings:
                        new LogSettings
                        {
                            LogDevice = this._logDevice,
                        },
                    checkpointSettings:
                        new CheckpointSettings
                        {
                            CheckpointDir = $"{this._dataFolder}/data/checkpoints",
                            CheckPointType = CheckpointType.FoldOver
                        }
                );

            if (Directory.Exists($"{this._dataFolder}/data/checkpoints"))
            {
                _fasterKvh.Recover();
                return true;
            }
            return false;
        }


        /// <summary>
        /// An interface for implementing custom context.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        public interface IFunctionContext<TData>
        {
            public void EndRead(ref TData data);
            public void EndRead(Status status);

            public void EndUpsert(Status status);
            public void EndRMW(Status status);

            public void EndDelete(Status status);

            public void EndCheckpoint(Status status);
        }
        /// <summary>
        /// Callback functions for int key and SpanByte value. It creates byte array as an output.
        /// </summary>
        public sealed class Functions : FunctionsBase<long, SpanByte, Empty, byte[], IFunctionContext<byte[]>>
        {
            /// <inheritdoc />
            public override void SingleWriter(ref long key, ref SpanByte src, ref SpanByte dst)
            {
                src.CopyTo(ref dst);
            }

            /// <inheritdoc />
            public override bool ConcurrentWriter(ref long key, ref SpanByte src, ref SpanByte dst)
            {
                return src.TryCopyTo(ref dst);
            }
            /// <inheritdoc />
            public override void ReadCompletionCallback(ref long key, ref Empty input, ref byte[] output, IFunctionContext<byte[]> ctx, Status status)
            {
                if (status != Status.OK)
                {
                    ctx.EndRead(status);
                }
                ctx.EndRead(ref output);
            }
            /// <inheritdoc />
            public override void SingleReader(ref long key, ref Empty input, ref SpanByte value, ref byte[] dst)
            {
                dst = value.ToByteArray();
            }

            /// <inheritdoc />
            public override void ConcurrentReader(ref long key, ref Empty input, ref SpanByte value, ref byte[] dst)
            {
                dst = value.ToByteArray();
            }
        }

        public class ClientSession : IDisposable
        {
            private FramesStoreSimple framesStoreSimple;
            private ClientSession<long, SpanByte, Empty, byte[], IFunctionContext<byte[]>, Functions>? _session;

            public ClientSession(FramesStoreSimple framesStoreSimple, ClientSession<long, SpanByte, Empty, byte[], IFunctionContext<byte[]>, Functions>? session)
            {
                this.framesStoreSimple = framesStoreSimple;
                this._session = session;
            }
            public void Dispose()
            {
                ((IDisposable)_session).Dispose();
            }
            public bool TryGet(long key, out byte[]? array)
            {
                byte[]? output = null;
                void OnRead(byte[] bytes)
                {
                    output = bytes;
                }
                var context = new Context(OnRead);
                var input = Empty.Default;
                var status = _session?.Read(ref key, ref input, ref output, context, 0) ?? Status.ERROR;
                if (status == Status.PENDING)
                {
                    _session?.CompletePending(true);
                }
                if (status == Status.NOTFOUND)
                {
                    array = null;
                    return false;
                }
                if (status != Status.OK)
                {
                    throw new InvalidOperationException("Error ocurred when reading data.");
                }
                array = output;
                return true;
            }

            /// <summary>
            /// Puts a new record or replace the existing one in the store.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void Put(long key, ref Span<byte> value)
            {
                var status = Status.OK;
                void OnUpsert(Status s)
                {
                    status = s;
                }
                var context = Context.WithUpsert(OnUpsert);
                var fixedSpan = SpanByte.FromFixedSpan(value);
                status = _session?.Upsert(ref key, ref fixedSpan, context, 0) ?? Status.ERROR;
                if (status == Status.PENDING)
                {
                    _session?.CompletePending(true);
                }
                if (status == Status.ERROR)
                    throw new InvalidOperationException("Error ocurred when inserting data.");
            }

            public bool CompletePending(bool spinWait = false)
            {
                return _session.CompletePending(spinWait);
            }

            class Context : IFunctionContext<byte[]>
            {
                private readonly Action<byte[]>? _onRead;
                private readonly Action<Status>? _onStatus;

                public Context(Action<byte[]>? onRead = null, Action<Status>? onStatus = null)
                {
                    this._onRead = onRead;
                    this._onStatus = onStatus;
                }

                internal static Context WithUpsert(Action<Status> onUpsert)
                {
                    return new Context(onStatus: onUpsert);
                }

                public void EndCheckpoint(Status status)
                {
                    _onStatus?.Invoke(status);
                }

                public void EndDelete(Status status)
                {
                    _onStatus?.Invoke(status);
                }

                public void EndRead(ref byte[] data)
                {
                    _onRead?.Invoke(data);
                }

                public void EndRead(Status status)
                {
                    _onStatus?.Invoke(status);
                }

                public void EndRMW(Status status)
                {
                    _onStatus?.Invoke(status);
                }

                public void EndUpsert(Status status)
                {
                    _onStatus?.Invoke(status);
                }
            }
        }

        internal IEnumerable<(ProcessingState State, TResult Result)> ProcessEntries<TResult>(IEntryProcessor<long, Memory<byte>, TResult> processor)
        {
            if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
            var iterator = _fasterKvh.Iterate() ?? throw new InvalidOperationException("Cannot create conversations database iterator.");
            while (iterator.GetNext(out _))
            {
                var memAndLen = iterator.GetValue().ToMemoryOwner(MemoryPool<byte>.Shared);
                var memory = memAndLen.memory.Memory.Slice(0, memAndLen.length);
                var state = processor.Invoke(ref iterator.GetKey(), ref memory, out var result);
                memAndLen.memory.Dispose();
                yield return (state, result);
            }
            iterator.Dispose();
        }

        internal unsafe static Memory<byte> GetFrameFromMemory(ref Memory<byte> memory, ref FrameMetadata frameMetadata)
        {
            fixed (void* ptr = memory.Span)
            {
                frameMetadata = Unsafe.AsRef<FrameMetadata>(ptr);       // makes a copy of the metadata from memory
                return memory.Slice(Unsafe.SizeOf<FrameMetadata>());    // gets the rest of the memory as frame bytes
            }
        }
    }
}