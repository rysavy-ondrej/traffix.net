using FASTER.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Traffix.Storage.Faster.FasterConversationTable;

namespace Traffix.Storage.Faster
{

    public class KeyValueStore<TKey, TValue, TInput, TOutput, TFunctions>
        where TKey : new()
        where TValue : new()
        where TInput : new()
        where TOutput : new()
        where TFunctions : IFunctions<TKey, TValue, TInput, TOutput, StoreContext<TOutput>>
    {
        private readonly string _dataFolder;
        private readonly int _logSizeBits;
        private readonly IFasterEqualityComparer<TKey> _keyComparer;
        private readonly TFunctions _functions;
        private readonly VariableLengthStructSettings<TKey, TValue> _variableLenSettings;
        private readonly SerializerSettings<TKey, TValue> _serializerSettings;
        private FasterKV<TKey, TValue, TInput, TOutput, StoreContext<TOutput>, TFunctions> _fasterKvh;
        private IDevice _logDevice;
        private IDevice _objDevice;

        public KeyValueStore(string folder, int logSizeBits, IFasterEqualityComparer<TKey> keyComparer, TFunctions functions, Func<IObjectSerializer<TKey>> keySerializer, Func<IObjectSerializer<TValue>> valueSerializer)
        {
            _dataFolder = folder;
            _logSizeBits = logSizeBits;
            _keyComparer = keyComparer;
            _functions = functions;
            _serializerSettings = new SerializerSettings<TKey, TValue>
            {
                keySerializer = keySerializer,
                valueSerializer = valueSerializer
            };
        }

        public KeyValueStore(string folder, int logSizeBits, IFasterEqualityComparer<TKey> keyComparer, TFunctions functions, IVariableLengthStruct<TKey> keyLength, IVariableLengthStruct<TValue> valueLength)
        {
            _dataFolder = folder;
            _logSizeBits = logSizeBits;
            _keyComparer = keyComparer;
            _functions = functions;
            _variableLenSettings = new VariableLengthStructSettings<TKey, TValue>
            {
                keyLength = keyLength,
                valueLength = valueLength
            };
        }

        public bool InitAndRecover()
        {
            var logSize = 1L << _logSizeBits;
            _logDevice = Devices.CreateLogDevice(@$"{this._dataFolder}\data\Store-hlog.log", preallocateFile: false);
            _objDevice = Devices.CreateLogDevice(@$"{this._dataFolder}\data\Store-hlog-obj.log", preallocateFile: false);

            this._fasterKvh = new FasterKV<TKey, TValue, TInput, TOutput, StoreContext<TOutput>, TFunctions>
                (
                    size: logSize,
                    functions: _functions,
                    logSettings:
                        new LogSettings
                        {
                            LogDevice = this._logDevice,
                            ObjectLogDevice = this._objDevice
                        },
                    comparer: _keyComparer,
                    checkpointSettings:
                        new CheckpointSettings
                        {
                            CheckpointDir = $"{this._dataFolder}/data/checkpoints",
                            CheckPointType = CheckpointType.FoldOver
                        },
                    serializerSettings: _serializerSettings,
                    variableLengthStructSettings: _variableLenSettings
                );

            if (Directory.Exists($"{this._dataFolder}/data/checkpoints"))
            {
                _fasterKvh.Recover();
                return true;
            }
            return false;

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

        public void Dispose()
        {
            _fasterKvh?.Dispose();
            _logDevice?.Dispose();
            _objDevice?.Dispose();
        }


        public KeyValueStoreClient GetClient()
        {
            var session = NewSession();
            return new KeyValueStoreClient(this, session);
        }

        ClientSession<TKey, TValue, TInput, TOutput, StoreContext<TOutput>, TFunctions> NewSession(string sessionId = null, bool threadAffinitized = false)
        {
            if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
            return _fasterKvh.NewSession(sessionId, threadAffinitized);
        }

        public IEnumerable<TResult> ProcessEntries<TResult>(IEntryProcessor<TKey, TValue,TResult> processor)
        {
            if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
            var iterator = _fasterKvh.Iterate() ?? throw new InvalidOperationException("Cannot create conversations database iterator.");
            while (iterator.GetNext(out _))
            {
                var state = processor.Invoke(ref iterator.GetKey(), ref iterator.GetValue(), out var result);
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

        /// <summary>
        /// Gets the number of entries in the store.
        /// </summary>
        public long EntryCount => _fasterKvh.EntryCount;


        public class KeyValueStoreClient : IDisposable
        {
            private KeyValueStore<TKey, TValue, TInput, TOutput, TFunctions> _keyValueStore;
            private ClientSession<TKey, TValue, TInput, TOutput, StoreContext<TOutput>, TFunctions> _session;


            public KeyValueStoreClient(KeyValueStore<TKey, TValue, TInput, TOutput, TFunctions> keyValueStore, ClientSession<TKey, TValue, TInput, TOutput, StoreContext<TOutput>, TFunctions> session)
            {
                _keyValueStore = keyValueStore;
                _session = session;
            }

            public void Dispose()
            {
                ((IDisposable)_session).Dispose();
            }

            public bool TryGet(ref TKey key, ref TInput input, ref TOutput output)
            {
                var context = new StoreContext<TOutput>();
                var status = _session.Read(ref key, ref input, ref output, context, 0);
                if (status == Status.PENDING)
                {
                    _session.CompletePending(true);
                    context.FinalizeRead(ref status, ref output);
                }
                if (status == Status.NOTFOUND)
                {
                    return false;
                }
                if (status != Status.OK)
                {
                    throw new InvalidOperationException("Error ocurred when reading data.");
                }
                return true;
            }

            public bool ContainsKey(ref TKey key)
            {
                var input = new TInput();
                var output = new TOutput();
                var context = new StoreContext<TOutput>();
                var status = _session.Read(ref key, ref input, ref output, context, 0);
                if (status == Status.PENDING)
                {
                    _session.CompletePending(true);
                    context.FinalizeRead(ref status, ref output);
                }
                if (status == Status.NOTFOUND || status == Status.ERROR)
                    return false;
                else
                    return true;
            }

            /// <summary>
            /// Puts a new record or replace the existing one in the store.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void Put(ref TKey key, ref TValue value)
            {
                var context = new StoreContext<TOutput>();
                var status = _session.Upsert(ref key, ref value, context, 0);
                if (status == Status.PENDING)
                {
                    _session.CompletePending(true);
                    context.FinalizeUpdate(ref status);
                }
                if (status == Status.ERROR)
                    throw new InvalidOperationException("Error ocurred when inserting data.");
            }

            /// <summary>
            /// Updates the existing record in the store. If the record does not exists it is created.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="input"></param>
            /// <returns>true if existing record was updated; false if a new record was inserted.</returns>
            public bool Update(ref TKey key, ref TInput input)
            {
                var context = new StoreContext<TOutput>();
                var status = _session.RMW(ref key, ref input, context, 0);
                if (status == Status.PENDING)
                {
                    _session.CompletePending(true);
                    context.FinalizeUpdate(ref status);
                }

                if (status == Status.ERROR)
                    throw new InvalidOperationException("Error ocurred when updating data.");
                if (status == Status.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool CompletePending(bool spinWait=false)
            {
                return _session.CompletePending(spinWait);
            }
        }

        public class StoreFunctions : IFunctions<TKey, TValue, TInput, TOutput, StoreContext<TOutput>>
        {
            public virtual void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint)
            {
            }

            public virtual void ConcurrentReader(ref TKey key, ref TInput input, ref TValue value, ref TOutput dst)
            {
            }

            public virtual bool ConcurrentWriter(ref TKey key, ref TValue src, ref TValue dst)
            {
                return false;
            }

            public virtual void CopyUpdater(ref TKey key, ref TInput input, ref TValue oldValue, ref TValue newValue)
            {
            }

            public virtual void DeleteCompletionCallback(ref TKey key, StoreContext<TOutput> ctx)
            {
                var status = Status.OK;
                ctx.Populate(status);
            }

            public virtual void InitialUpdater(ref TKey key, ref TInput input, ref TValue value)
            {
            }

            public virtual bool InPlaceUpdater(ref TKey key, ref TInput input, ref TValue value)
            {
                return false;
            }

            public virtual void ReadCompletionCallback(ref TKey key, ref TInput input, ref TOutput output, StoreContext<TOutput> ctx, Status status)
            {
                
                ctx.Populate(status, ref output);
            }

            public virtual void RMWCompletionCallback(ref TKey key, ref TInput input, StoreContext<TOutput> ctx, Status status)
            {
                ctx.Populate(status);
            }

            public virtual void SingleReader(ref TKey key, ref TInput input, ref TValue value, ref TOutput dst)
            {
            }

            public virtual void SingleWriter(ref TKey key, ref TValue src, ref TValue dst)
            {
            }

            public virtual void UpsertCompletionCallback(ref TKey key, ref TValue value, StoreContext<TOutput> ctx)
            {
                var status = Status.OK;
                ctx.Populate(status);
            }
        }
    }
    public class StoreContext<TOuput>
    {
        private Status _status;
        private TOuput _output;

        public void Populate(Status status, ref TOuput output)
        {
            this._status = status;
            this._output = output;
        }
        public void Populate(Status status)
        {
            this._status = status;
        }

        public void FinalizeRead(ref Status status, ref TOuput output)
        {
            status = this._status;
            output = this._output;
        }

        public void FinalizeUpdate(ref Status status)
        {
            status = this._status;
        }


    }
}
