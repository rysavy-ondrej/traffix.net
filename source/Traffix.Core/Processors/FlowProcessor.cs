using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Traffix.Core.Observable
{
    /// <summary>
    /// Basic implementation of flow processor.
    /// <para/>
    /// The flow processor is used to apply the processor on each input element and update the flow record. 
    /// The class implements <see cref="IObserver{T}"/> interface for consuming the input sequence.
    /// <para/>
    /// The concrete implementation needs to provide four methods <see cref="Create(TSource)"/>, <see cref="Update(TFlowRecord, TSource)"/>, 
    /// <see cref="Aggregate(TFlowRecord, TFlowRecord)"/>, and <see cref="GetFlowKey(TSource)"/>.
    /// <para/>
    /// The extension methods enable to aggregate the created flows and also to get conversations (biflows). 
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TFlowKey">The flow key type.</typeparam>
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


        /// <summary>
        /// Gets the flow key for the given source element.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <returns>The flow key for the source object.</returns>
        protected abstract TFlowKey GetFlowKey(TSource source);

        /// <summary>
        /// Creates a new flow record from the first source element.
        /// </summary>
        /// <param name="source">The source element.</param>
        /// <returns>A new flow record.</returns>
        protected abstract TFlowRecord Create(TSource source);

        /// <summary>
        /// Updates the existing flow record with a new element.  
        /// <para/>
        /// An in-place update is considered. It is thus necessary to wrap the 
        /// record in an object if record value cannot be updated in-place.
        /// </summary>
        /// <param name="record">The record to update.</param>
        /// <param name="source">The source element used to update.</param>
        protected abstract void Update(TFlowRecord record, TSource source);

        /// <summary>
        /// Aggregates flow records and creates a new resulting record.
        /// </summary>
        /// <param name="arg1">The first flow record to aggregate.</param>
        /// <param name="arg2">The second flow record to aggregate.</param>
        /// <returns>The new aggregated flow record.</returns>
        protected abstract TFlowRecord Aggregate(TFlowRecord arg1, TFlowRecord arg2);



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
        /// Aggregates the flow using the user provided function.
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
            Error?.Invoke(this, new ErrorEventArgs(error));
        }

        /// <summary>
        /// Called when an error ocurred during source observable processing. 
        /// </summary>
        public event EventHandler<ErrorEventArgs>? Error;

        /// <summary>
        /// The completion signalizing that input observable completes.
        /// <para/>
        /// Use this method to detect when the source observable was fully processed if the flow processor is subscribed to input observable. 
        /// </summary>
        public Task Completed => WaitOneAsync(_onCompleteHandle);        

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