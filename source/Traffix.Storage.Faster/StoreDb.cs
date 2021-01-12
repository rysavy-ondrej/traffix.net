using FASTER.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Traffix.Storage.Faster
{
    internal class StoreDb<TKey, TValue, TInput, TOutput, TContext, TFunctions> 
        where TKey : new()
        where TValue : new()
        where TFunctions : IFunctions<TKey, TValue, TInput, TOutput, TContext>
    {
        private readonly string _dataFolder;
        private readonly IFasterEqualityComparer<TKey> _keyComparer;
        private readonly TFunctions _functions;
        private readonly VariableLengthStructSettings<TKey, TValue> _variableLenSettings;
        private readonly SerializerSettings<TKey, TValue> _serializerSettings;
        public FasterKV<TKey, TValue, TInput, TOutput, TContext, TFunctions> _fasterKvh;
        public IDevice _logDevice;
        public IDevice _objDevice;

        public StoreDb(string folder, IFasterEqualityComparer<TKey> keyComparer, TFunctions functions, Func<IObjectSerializer<TKey>> keySerializer, Func<IObjectSerializer<TValue>>  valueSerializer )
        {
            _dataFolder = folder;
            _keyComparer = keyComparer;
            _functions = functions;
            _serializerSettings = new SerializerSettings<TKey, TValue>
            {
                keySerializer = keySerializer,
                valueSerializer = valueSerializer
            };
        }

        public StoreDb(string folder, IFasterEqualityComparer<TKey> keyComparer, TFunctions functions, IVariableLengthStruct<TKey> keyLength, IVariableLengthStruct<TValue> valueLength)
        {
            _dataFolder = folder;
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
            var logSize = 1L << 20;
            _logDevice = Devices.CreateLogDevice(@$"{this._dataFolder}\data\Store-hlog.log", preallocateFile: false);
            _objDevice = Devices.CreateLogDevice(@$"{this._dataFolder}\data\Store-hlog-obj.log", preallocateFile: false);

            this._fasterKvh = new FasterKV<TKey, TValue, TInput, TOutput, TContext, TFunctions>
                (
                    size: logSize,
                    functions: _functions,
                    logSettings:
                        new LogSettings
                        {
                            LogDevice = this._logDevice,
                            ObjectLogDevice = this._objDevice,
                            MutableFraction = 0.3,
                            PageSizeBits = 15,
                            MemorySizeBits = 20
                        },
                    comparer: _keyComparer,
                    checkpointSettings:
                        new CheckpointSettings
                        {
                            CheckpointDir = $"{this._dataFolder}/data/checkpoints"
                        },
                    serializerSettings: _serializerSettings,
                    variableLengthStructSettings: _variableLenSettings
                );

            if (Directory.Exists($"{this._dataFolder}/data/checkpoints"))
            {
                Console.WriteLine("call recover db");
                _fasterKvh.Recover();
                return false;
            }
            return true;
        }

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
            _logDevice?.Close();
            _objDevice?.Close();
        }

        public ClientSession<TKey, TValue, TInput, TOutput, TContext, TFunctions> NewSession(string sessionId = null, bool threadAffinitized = false)
        {
            if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
            return _fasterKvh.NewSession(sessionId, threadAffinitized);
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Items
        {
            get
            {
                if (_fasterKvh == null) throw new InvalidOperationException("The store is closed.");
                var iterator = _fasterKvh.Iterate() ?? throw new InvalidOperationException("Cannot create conversations database iterator.");
                while (iterator.GetNext(out _))
                {
                    var key = iterator.GetKey();
                    var value = iterator.GetValue();
                    yield return KeyValuePair.Create(key , value );
                }
                iterator.Dispose();     
            }
        }
    }
}
