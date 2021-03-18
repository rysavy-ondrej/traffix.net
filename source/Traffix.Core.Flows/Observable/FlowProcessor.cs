using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Traffix.Core.Observable
{
    /// <summary>
    /// Basic implementation of flow processor.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TFlowKey"></typeparam>
    /// <typeparam name="TFlowRecord"></typeparam>
    public abstract class FlowProcessor<TSource, TFlowKey, TFlowRecord> : IObserver<TSource>
    {
        private readonly Dictionary<TFlowKey, TFlowRecord> _flowDictionary;
        private readonly EventWaitHandle _onCompleteHandle;
        public FlowProcessor()
        {
            _flowDictionary = new Dictionary<TFlowKey, TFlowRecord>(1024);
            _onCompleteHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        protected abstract TFlowRecord Create(TSource source);

        protected abstract void Update(TFlowRecord record, TSource source);

        protected abstract TFlowRecord Aggregate(TFlowRecord arg1, TFlowRecord arg2);

        protected abstract TFlowKey GetKey(TSource source);

        /// <summary>
        /// Gets the collection of flows.
        /// </summary>
        public IEnumerable<KeyValuePair<TFlowKey, TFlowRecord>> Flows => _flowDictionary;

        /// <summary>
        /// Geta a collection of flow keys.
        /// </summary>
        public IEnumerable<TFlowKey> FlowKeys => _flowDictionary.Keys;

        /// <summary>
        /// Gets the number of flows.
        /// </summary>
        public int Count => _flowDictionary.Count;

        public IEnumerable<KeyValuePair<TAggregateKey, TFlowRecord>> AggregateFlows<TAggregateKey>(Func<TFlowKey, TAggregateKey> aggregateKey)
        {
            return _flowDictionary.GroupBy(x => aggregateKey(x.Key)).Select(g => KeyValuePair.Create<TAggregateKey, TFlowRecord>(g.Key, g.Select(p=>p.Value).Aggregate(Aggregate)));
        }

        public void OnNext(TSource source)
        {
            var key = GetKey(source);
            if (_flowDictionary.TryGetValue(key, out var flowRecord))
            {
                Update(flowRecord, source);
            }
            else
            {
                _flowDictionary.Add(key, Create(source));
            }
        }

        public void OnCompleted()
        {
            _onCompleteHandle.Set();    
        }

        public void OnError(Exception error)
        {
            
        }

        /// <summary>
        /// The completion signalizing that input observable completes.
        /// <para/>
        /// Use this method to detect when the source observable was fully processed if the flow processor is subscribed to input observable. 
        /// </summary>
        public Task Completed => WaitOneAsync(_onCompleteHandle);

        //public IEnumerable<> Conversations

        #region Helper methods
        static Task WaitOneAsync(WaitHandle waitHandle)
        {
            if (waitHandle == null)
                throw new ArgumentNullException("waitHandle");

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                delegate { tcs.TrySetResult(true); }, null, -1, true);
            var t = tcs.Task;
            t.ContinueWith((antecedent) => rwh.Unregister(null));
            return t;
        }
        #endregion
    }
}