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
    /// <summary>
    /// Indicates the result of processing the object.
    /// </summary>
    public enum ProcessingState
    {
        /// <summary>
        /// The object has been successfully processed.
        /// </summary>
        Success,
        /// <summary>
        /// The object was skipped and should not be a part of the output.
        /// </summary>
        Skip,
        /// <summary>
        /// The object was not processed and the entire processing should be terminated.
        /// </summary>
        Terminate
    }
}