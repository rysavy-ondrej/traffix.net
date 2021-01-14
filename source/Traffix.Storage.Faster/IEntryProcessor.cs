using static Traffix.Storage.Faster.FasterConversationTable;

namespace Traffix.Storage.Faster
{
    /// <summary>
    /// Defines the entry processor for in the KeyValueStore.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IEntryProcessor<TKey, TValue, TResult>
        where TKey : new()
        where TValue : new()
    {
         ProcessingState Invoke(ref TKey key, ref TValue value, out TResult result);
    }
}