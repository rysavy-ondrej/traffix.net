using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Traffix.Core.Observable
{
    /// <summary>
    /// Basic implementation of flow processor with the support for flow aggregation and conversations.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TFlowKey">The flow key type.</typeparam>
    /// <typeparam name="TConversationKey">The conversation key type.</typeparam>
    /// <typeparam name="TFlowRecord">The flow record type.</typeparam>
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

        protected abstract TFlowKey GetFlowKey(TSource source);

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

        /// <summary>
        /// Aggregates the flow using user defined function.
        /// </summary>
        /// <typeparam name="TAggregateKey">The aggregation key type.</typeparam>
        /// <param name="aggregateKey">The aggregation function.</param>
        /// <returns>A collection of aggregated flows.</returns>
        public IEnumerable<KeyValuePair<TAggregateKey, TFlowRecord>> AggregateFlows<TAggregateKey>(Func<TFlowKey, TAggregateKey> aggregateKey)
        {
            return Flows.GroupBy(x => aggregateKey(x.Key)).Select(g => KeyValuePair.Create(g.Key, g.Select(p=>p.Value).Aggregate(Aggregate)));
        }

        /// <inheritdoc/>
        public void OnNext(TSource source)
        {
            var key = GetFlowKey(source);
            if (_flowDictionary.TryGetValue(key, out var flowRecord))
            {
                Update(flowRecord, source);
            }
            else
            {
                _flowDictionary.Add(key, Create(source));
            }
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
            _onCompleteHandle.Set();    
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            
        }

        /// <summary>
        /// The completion signalizing that input observable completes.
        /// <para/>
        /// Use this method to detect when the source observable was fully processed if the flow processor is subscribed to input observable. 
        /// </summary>
        public Task Completed => WaitOneAsync(_onCompleteHandle);

        /// <summary>
        /// Gets the collection of conversations. 
        /// </summary>
        

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

    public static class FlowProcessorExtensions
    {
        /// <summary>
        /// Gets the conversation for flows collected in the flow processor using the default flow aggregation function of the flow processor.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TFlowKey">The flow key type.</typeparam>
        /// <typeparam name="TFlowRecord">The flow record type.</typeparam>
        /// <typeparam name="TConversationKey">The conversation key type.</typeparam>
        /// <param name="flowProcessor">The source flow processor.</param>
        /// <param name="getConversationKey">The conversation key function.</param>
        /// <returns>The collection of conversations.</returns>
        public static IEnumerable<KeyValuePair<TConversationKey, TFlowRecord>> GetConversations<TSource, TFlowKey, TFlowRecord, TConversationKey>(this FlowProcessor<TSource, TFlowKey, TFlowRecord> flowProcessor, Func<TFlowKey, TConversationKey> getConversationKey )
            => flowProcessor.AggregateFlows(getConversationKey);

        /// <summary>
        /// Gets the conversation for flows collected in the flow processor using the provided aggregation function.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TFlowKey">The flow key type.</typeparam>
        /// <typeparam name="TFlowRecord">The flow record type.</typeparam>
        /// <typeparam name="TConversationKey">The conversation key type.</typeparam>
        /// <typeparam name="TAccumulate">The accumulated conversation record type.</typeparam>
        /// <param name="flowProcessor">The source flow processor.</param>
        /// <param name="getConversationKey">The conversation key function.</param>
        /// <param name="seed">The seed value for conversation record.</param>
        /// <param name="func">The accumulate function.</param>
        /// <returns>The collection of conversations.</returns>
        public static IEnumerable<KeyValuePair<TConversationKey, TAccumulate>> GetConversations<TFlowKey, TFlowRecord, TConversationKey, TAccumulate>(this IEnumerable<KeyValuePair<TFlowKey, TFlowRecord>> flows, Func<TFlowKey, TConversationKey> getConversationKey, TAccumulate seed, Func<TAccumulate, KeyValuePair<TFlowKey, TFlowRecord>, TAccumulate> func)
        {
            return flows.GroupBy(x => getConversationKey(x.Key))
                        .Select(c=> KeyValuePair.Create(c.Key, c.Aggregate(seed, func)));
        }
    }

}