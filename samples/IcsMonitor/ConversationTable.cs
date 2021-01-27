using System;
using System.Collections.Generic;
using System.Linq;

namespace IcsMonitor
{
    /// <summary>
    /// A memory representation of the conversation table. 
    /// </summary>
    /// <typeparam name="TData">The data type of the conversation.</typeparam>
    public class ConversationTable<TData> : List<ConversationRecord<TData>>
    {
        /// <summary>
        /// Start time of the conversation table.
        /// </summary>
        public DateTime StartTime;
        /// <summary>
        /// The interval defining the duration of the conversation table.
        /// </summary>
        public TimeSpan Interval;

        /// <summary>
        /// Creates a new conversation table from the given collection of conversations.
        /// </summary>
        /// <param name="collection"></param>
        public ConversationTable(IEnumerable<ConversationRecord<TData>> collection) : base(collection)
        {
        }
        /// <summary>
        /// Aggregates conversations by grouping conversations using <paramref name="keySelector"/> and then by applying <paramref name="aggregator"/> function 
        /// to all conversations in the group. The result is a collection of aggregated conversations.
        /// </summary>
        /// <typeparam name="Tout">The type of the output.</typeparam>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <param name="conversations">The input collection of conversations.</param>
        /// <param name="keySelector">The selector of key fields for the aggregated conversations.</param>
        /// <param name="accumulator">The initial value of the aggregation.</param>
        /// <param name="aggregator">The aggregattor function that implements a way to add a new conversation to the aggregation.</param>
        /// <returns>A collection of conversations. Number of returned items equals to a number of groups created by <paramref name="keySelector"/>.</returns>
        public IEnumerable<Tout> AggregateConversations<Tout, TKey>(Func<ConversationRecord<TData>, TKey> keySelector, Func<ConversationRecord<TData>, Tout> getTarget, Func<Tout, Tout, Tout> aggregator)
        {
            var groups = this.GroupBy(keySelector);
            foreach (var g in groups)
            {
                yield return g.Select(getTarget).Aggregate(aggregator);
            }
        }
    }
}
