using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Traffix.Core.Observable
{
    public static class ObservableFlows
    {
        /// <summary>
        /// Projects each element of an observable sequence into consecutive non-overlapping windows. 
        /// The projection is controlled by time provided by <paramref name="getTicks"/> and the 
        /// <paramref name="timeSpan"/> interval.
        /// </summary>
        /// <typeparam name="T">The type of source.</typeparam>
        /// <param name="observable">The source sequence to produce windows over.</param>
        /// <param name="getTicks">The function to get time value of the element.</param>
        /// <param name="timeSpan">The time interval of windows produced.</param>
        /// <returns>An observable sequence of windows.</returns>
        public static IObservable<IObservable<TSource>> TimeWindow<TSource>(this IObservable<TSource> observable, Func<TSource, long> getTicks, TimeSpan timeSpan)
        {
            // implemented using side-effect operations, which is not nice, but it is short and efficient.
            var shared = observable.Publish().RefCount();
            var index = shared.Select(x => getTicks(x)).Publish().RefCount();
            long? windowEdgeTicks = null;
            long timeSpanTicks = timeSpan.Ticks;
            return shared.Window(() => index.Do(ticks => windowEdgeTicks ??= ticks + timeSpanTicks).SkipWhile(ticks => ticks < windowEdgeTicks).Do(ticks => { windowEdgeTicks = ticks + timeSpanTicks; }));
        }

        /// <summary>
        /// Applies the flow processor to an observable sequence of flows. 
        /// </summary>
        /// <typeparam name="TFlowKey">The type of flow key.</typeparam>
        /// <typeparam name="TSource">The packet type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="flows">The source sequence of flows.</param>
        /// <param name="flowProcessor">The flow processor to apply.</param>
        /// <returns>An observable sequence of results from the application of flow processor to each source flow.</returns>
        public static IObservable<TResult> ApplyFlowProcessor<TFlowKey, TSource, TResult>(this IObservable<IGroupedObservable<TFlowKey, TSource>> flows, Func<TFlowKey, IObservable<TSource>, Task<TResult>> flowProcessor)
        {
            return flows.Select(flow => flowProcessor(flow.Key, flow)).Merge();
        }

        /// <summary>
        /// Projects each element of an observable sequence into the corresponding flow.  
        /// </summary>
        /// <typeparam name="TFlowKey">The type of flow key.</typeparam>
        /// <typeparam name="TSource">The packet type.</typeparam>
        /// <param name="observable">The source sequence of packets.</param>
        /// <param name="getFlowKey">The function to get flow key from the element.</param>
        /// <returns>An observable sequence of flows.</returns>
        public static IObservable<IGroupedObservable<TFlowKey, TSource>> GroupFlows<TFlowKey, TSource>(this IObservable<TSource> observable, Func<TSource, TFlowKey> getFlowKey)
        {
            return observable.GroupBy(packet => getFlowKey(packet));
        }

        /// <summary>
        /// Projects each element of an observable sequence into the corresponding conversation.  
        /// </summary>
        /// <typeparam name="TFlowKey">The type of flow key.</typeparam>
        /// <typeparam name="TConversationKey">The type of conversation key.</typeparam>
        /// <typeparam name="TSource">The packet type.</typeparam>
        /// <param name="observable">The source sequence of packets.</param>
        /// <param name="getFlowKey">The function to get a flow key from the element.</param>
        /// <param name="getConversationKey">The function to get a conversation key fro the flow key.</param>
        /// <returns>An observable sequence of conversations.</returns>
        public static IObservable<IGroupedObservable<TConversationKey, IGroupedObservable<TFlowKey, TSource>>> GroupConversations<TFlowKey, TConversationKey, TSource>(this IObservable<TSource> observable, Func<TSource, TFlowKey> getFlowKey, Func<TFlowKey, TConversationKey> getConversationKey)
        {
            var flows = observable.GroupFlows(getFlowKey);
            return flows.GroupBy(flow => getConversationKey(flow.Key));
        }


        /// <summary>
        /// Projects each element of an observable sequence into consecutive non-overlapping windows. 
        /// The projection is controlled by time provided by <paramref name="getTicks"/> and the 
        /// <paramref name="timeSpan"/> interval.
        /// </summary>
        /// <typeparam name="T">The type of source.</typeparam>
        /// <param name="observable">The source sequence to produce windows over.</param>
        /// <param name="getTicks">The function to get time value of the element.</param>
        /// <param name="timeSpan">The time interval of windows produced.</param>
        /// <returns>An observable sequence of windows.</returns>
        public static IObservable<IObservable<TSource>> TickIntervalWindow<TSource>(this IObservable<TSource> source, Func<TSource, long> getTicks, TimeSpan timeSpan)
        {
            return System.Reactive.Linq.Observable.Create<IObservable<TSource>>(observer =>
            {
                long? _windowEdgeTicks = null;
                Subject<TSource>? _currentWindow = new Subject<TSource>();
                return source.Subscribe(value =>
                {
                    var ticks = getTicks(value);
                    _windowEdgeTicks ??= ticks + timeSpan.Ticks;

                    if (ticks < _windowEdgeTicks)
                    {
                        _currentWindow?.OnNext(value);
                    }
                    else
                    {
                        _currentWindow?.OnCompleted();

                        _windowEdgeTicks += timeSpan.Ticks;
                        _currentWindow = new Subject<TSource>();
                        observer.OnNext(_currentWindow);
                    }
                }, observer.OnError, observer.OnCompleted);
            });
        }
    }
}