using System;
using System.Reactive.Linq;
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
    }
}
