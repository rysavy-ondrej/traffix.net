using FASTER.core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Traffix.Data;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// Implements a key-value store for raw frames.
    /// <para/>
    /// The store contains raw frames indexed by <see cref="FrameKey"/> address (ulong type). 
    /// </summary>
    public class RawFramesStore : IDisposable
        {
            private readonly string _dataFolder;
            private readonly int _logSizeBits;
            private readonly IDevice _logDevice;
            private readonly FasterKV<ulong, SpanByte> _fasterKvh;

            /// <summary>
            /// Gets the number of records in the store.
            /// </summary>
            public int GetRecordCount() => ProcessEntriesRaw(null);

            /// <summary>
            /// Creates a new instance of the frame store.
            /// </summary>
            /// <param name="folder">Folder to persist changes.</param>
            /// <param name="capacity">The expected capacity.</param>
            public RawFramesStore(string folder, long capacity)
            {
                _dataFolder = folder;
                _logSizeBits = (int)Math.Log(capacity, 2) + 1;
                _logDevice = Devices.CreateLogDevice(@$"{this._dataFolder}\data\Store-hlog.log", preallocateFile: false);
                _fasterKvh = new FasterKV<ulong, SpanByte>
                    (
                        size: 1L << _logSizeBits,
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
            }
            /// <inheritdoc />
            public void Dispose()
            {
                _fasterKvh.Dispose();
                _logDevice.Dispose();
            }

            /// <summary>
            /// Performs the incremental checkpoint, which persists all unsaved changes.
            /// </summary>
            /// <returns>The Id of the checkpoint created.</returns>
            public Guid Checkpoint()
            {
                if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
                _fasterKvh.TakeFullCheckpoint(out Guid token);
                _fasterKvh.CompleteCheckpointAsync().GetAwaiter().GetResult();
                return token;
            }

            /// <summary>
            /// Gets the <see cref="ClientSession"/> that can be used to access data in the store.
            /// </summary>
            /// <returns>New client session object.</returns>
            public ClientSession GetClient()
            {
                var s = _fasterKvh.For(new Functions()).NewSession<Functions>();
                if (s == null) throw new InvalidOperationException("Cannot create a session object.");
                return new ClientSession(this, s);
            }

            /// <summary>
            /// Called to intialize and possibly recover the storage from the checkpoint if exists.
            /// </summary>
            /// <returns>true if the store was recovered, false otherwise (new store created).</returns>
            public bool InitAndRecover()
            {
                if (Directory.Exists($"{this._dataFolder}/data/checkpoints"))
                {
                    _fasterKvh.Recover();
                    return true;
                }
                else
                {
                    /* no operation is required to initialize the store */
                    return false;
                }
            }


            /// <summary>
            /// An interface for implementing custom context as a collection of callback methods.
            /// </summary>
            /// <typeparam name="TData"></typeparam>
            internal interface IFunctionContext<TData>
            {
                /// <summary>
                /// Called from <see cref="Functions.ReadCompletionCallback"/> 
                /// when it complets with <see cref="Status.OK"/>.
                /// </summary>
                /// <param name="data">The result of read operation.</param>
                public void EndRead(ref TData data);
                /// <summary>
                /// Called from <see cref="Functions.ReadCompletionCallback"/>
                /// when the it ends with some error, that is status != <see cref="Status.OK"/>.
                /// </summary>
                /// <param name="status">The status of the operation.</param>
                public void EndRead(Status status);

                /// <summary>
                /// Called from <see cref="Functions.UpsertCompletionCallback"/>.
                /// </summary>
                /// <param name="key">The key of the value that was inserted</param>
                public void EndUpsert(ulong key);
                /// <summary>
                /// Called from <see cref="Functions.RMWCompletionCallback"/>.
                /// </summary>
                /// <param name="status">The status of the operation.</param>
                public void EndRMW(Status status);
                /// <summary>
                /// Called from <see cref="Functions.DeleteCompletionCallback"/>.
                /// </summary>
                /// <param name="key">The key of the value that was deleted.</param>
                public void EndDelete(ulong key);
            }
            /// <summary>
            /// Callback functions for int key and SpanByte value. It creates byte array as an output.
            /// It uses callback context object.
            /// <para/>
            /// Function object is shared in a session. The context object is supplied for each operation.
            /// </summary>

            internal sealed class Functions : FunctionsBase<ulong, SpanByte, Empty, byte[], IFunctionContext<byte[]>>
            {
                /// <inheritdoc />
                public override void SingleWriter(ref ulong key, ref SpanByte src, ref SpanByte dst)
                {
                    src.CopyTo(ref dst);
                }

                /// <inheritdoc />
                public override bool ConcurrentWriter(ref ulong key, ref SpanByte src, ref SpanByte dst)
                {
                    return src.TryCopyTo(ref dst);
                }
                /// <inheritdoc />
                public override void ReadCompletionCallback(ref ulong key, ref Empty input, ref byte[] output, IFunctionContext<byte[]> ctx, Status status)
                {
                    if (status != Status.OK)
                    {
                        ctx.EndRead(status);
                    }
                    ctx.EndRead(ref output);
                }
                /// <inheritdoc />
                public override void SingleReader(ref ulong key, ref Empty input, ref SpanByte value, ref byte[] dst)
                {
                    dst = value.ToByteArray();
                }
                /// <inheritdoc />
                public override void ConcurrentReader(ref ulong key, ref Empty input, ref SpanByte value, ref byte[] dst)
                {
                    dst = value.ToByteArray();
                }
                /// <inheritdoc />
                public override void DeleteCompletionCallback(ref ulong key, IFunctionContext<byte[]> ctx)
                {
                    ctx.EndDelete(key);
                }
                public override void RMWCompletionCallback(ref ulong key, ref Empty input, IFunctionContext<byte[]> ctx, Status status)
                {
                    ctx.EndRMW(status);
                }
                public override void UpsertCompletionCallback(ref ulong key, ref SpanByte value, IFunctionContext<byte[]> ctx)
                {
                    ctx.EndUpsert(key);
                }
            }

            public class ClientSession : IDisposable
            {
                private RawFramesStore _framesStore;
                private ClientSession<ulong, SpanByte, Empty, byte[], IFunctionContext<byte[]>, Functions> _session;

                internal ClientSession(RawFramesStore framesStore, ClientSession<ulong, SpanByte, Empty, byte[], IFunctionContext<byte[]>, Functions> session)
                {
                    this._framesStore = framesStore;
                    this._session = session;
                }
                public void Dispose()
                {
                    ((IDisposable)_session).Dispose();
                }
                public bool TryGet(ulong key, out byte[]? array)
                {
                    array = null;
                    Status status = Status.OK;
                    var context = Context.WithRead(v => { status = Status.OK; }, s => { status = s; });

                    var input = Empty.Default;
                    status = _session?.Read(ref key, ref input, ref array, context, 0) ?? Status.ERROR;
                    if (status == Status.PENDING)
                    {
                        _session?.CompletePending(true);
                        System.Diagnostics.Debug.Assert(status != Status.OK || array != null);
                    }
                    switch (status)
                    {
                        case Status.OK:
                            return true;
                        case Status.NOTFOUND:
                            return false;
                        case Status.PENDING:
                        case Status.ERROR:
                        default:
                            throw new InvalidOperationException("Error ocurred when reading data.");
                    }
                }

                /// <summary>
                /// Puts a new record or replace the existing one in the store.
                /// </summary>
                /// <param name="key">The key of the record.</param>
                /// <param name="value">The record value.</param>
                public void Put(ulong key, ref Span<byte> value)
                {
                    var status = Status.OK;
                    var context = Context.WithUpsert(s => { status = s; });
                    var fixedSpan = SpanByte.FromFixedSpan(value);
                    status = _session?.Upsert(ref key, ref fixedSpan, context, 0) ?? Status.ERROR;
                    if (status == Status.PENDING)
                    {
                        _session?.CompletePending(true);
                    }
                    if (status == Status.ERROR)
                        throw new InvalidOperationException("Error ocurred when inserting data.");
                }

                /// <summary>
                /// Completes pending operations on the store.
                /// </summary>
                /// <param name="spinWait">Spin-wait for all pending operations on session to complete.</param>
                /// <returns></returns>
                public bool CompletePending(bool spinWait = false)
                {
                    return _session.CompletePending(spinWait);
                }

                /// <summary>
                /// This class implements a function context using callback functions. 
                /// <para/>
                /// While <see cref="Functions"/> object is shared in the session, 
                /// <see cref="Context"/> is valid only for the single operation. 
                /// Using callback functions it is possible to access result of asynchronous operation 
                /// in the caller context. 
                /// </summary>
                class Context : IFunctionContext<byte[]>
                {
                    private readonly Action<ulong>? _onKey;
                    private readonly Action<byte[]>? _onValue;
                    private readonly Action<Status>? _onStatus;

                    private Context(Action<ulong>? onKey = null, Action<byte[]>? onValue = null, Action<Status>? onStatus = null)
                    {
                        this._onKey = onKey;
                        this._onValue = onValue;
                        this._onStatus = onStatus;
                    }

                    internal static Context WithUpsert(Action<Status> onUpsert)
                    {
                        return new Context(onStatus: onUpsert);
                    }
                    internal static Context WithRead(Action<byte[]> onSuccess, Action<Status> onError)
                    {
                        return new Context(onValue: onSuccess, onStatus: onError);
                    }

                    public void EndDelete(ulong key)
                    {
                        _onKey?.Invoke(key);
                    }

                    public void EndRead(ref byte[] data)
                    {
                        _onValue?.Invoke(data);
                    }

                    public void EndRead(Status status)
                    {
                        _onStatus?.Invoke(status);
                    }

                    public void EndRMW(Status status)
                    {
                        _onStatus?.Invoke(status);
                    }

                    public void EndUpsert(ulong key)
                    {
                        _onKey?.Invoke(key);
                    }
                }
            }

            /// <summary>
            /// Applies <paramref name="processor"/> to all frames in the store. The processor provides results that are collected and 
            /// returned as the lazy collection.
            /// <para/>
            /// The processor is given a memory object that has a limited life time. The processor should not reference this object as it is disposed after finishing the processor's Invoke method. If necessary, the processor can create a copy of the data.
            /// <para/>
            /// The processor can control the computation by returning <see cref="ProcessingState"/> value: 
            /// ● <see cref="ProcessingState.Success"/> means that the result is a part of the returned collection 
            /// ● <see cref="ProcessingState.Skip"/> means that the result is ignored  
            /// ● <see cref="ProcessingState.Terminate"/> stops the iteration.
            /// </summary>
            /// <typeparam name="TResult">The type of results</typeparam>
            /// <param name="processor">The entry processor implementation.</param>
            /// <returns>A lazy collection of  results generated by the processor.</returns>
            internal IEnumerable<TResult> ProcessEntries<TResult>(IEntryProcessor<ulong, Memory<byte>, TResult> processor)
            {
                if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
                var iterator = _fasterKvh.Iterate() ?? throw new InvalidOperationException("Cannot create conversations database iterator.");
                while (iterator.GetNext(out _))
                {
                    var memAndLen = iterator.GetValue().ToMemoryOwner(MemoryPool<byte>.Shared);
                    var memory = memAndLen.memory.Memory.Slice(0, memAndLen.length);
                    var state = processor.Invoke(ref iterator.GetKey(), ref memory, out var result);
                    memAndLen.memory.Dispose();
                    switch (state)
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
                iterator.Dispose();
            }

            private int ProcessEntriesRaw(Action<IFasterScanIterator<ulong, SpanByte>>? onNextValue)
            {
                if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
                var iterator = _fasterKvh.Iterate() ?? throw new InvalidOperationException("Cannot create conversations database iterator.");
                var entriesCount = 0;
                while (iterator.GetNext(out _))
                {
                    entriesCount++;
                    onNextValue?.Invoke(iterator);
                }
                iterator.Dispose();
                return entriesCount;
            }

            /// <summary>
            /// Carves out frame metadata and frame bytes from the provided memory image of the frame. 
            /// </summary>
            /// <param name="memory">The memory that contains frame image.</param>
            /// <param name="frameMetadata">The reference to frame metadata to be filled with required information.</param>
            /// <returns>The memory containing the frame bytes.</returns>
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
