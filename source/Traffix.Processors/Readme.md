# Traffix.Processors

## Frame Processors


## Conversation Processors

All conversation processors have to implement the `IConversationProcessor` interface.
The interface defines a single method that is called for every conversation in the batch of data. The method is given a conversation key and a collection of all associated frames. The methods should return a value. It is up to the implementor to specify the result type. If a conversation cannot be processed the return value should express this situation rather than throwing an exception.

```csharp
public interface IConversationProcessor<T>
{
    T Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames);
}
```

The processor is invoked for each conversation. The conversation is identified by its `FlowKey`and a collection of frames. Each frame is provided as `Memory<byte>` object. 

It is also possible to create a conversation processor from a function: 

```csharp
public static IConversationProcessor<TResult> ConversationProcessor.FromFunction<TResult>(Func<FlowKey, IEnumerable<Memory<byte>>, TResult> function);
```

Before the conversation processor is applied to a conversation it is possible to modify the conversation data. It can be useful when we want to apply a conversation processor only for a subset of frames. instead of creating a new conversation processor implementation, it is possible to apply predefined operators to adjust the collection of frames.

### Operators

There are several operators to adjust the frames enumerable before it is passed to the conversation processor. Using these operators can be useful in various situations when we only want to process a subset of conversation frames. For instance, if the conversation spans several windows, only the frames within the current window are processed to yield the result.
All these operators take existing conversation processor and creates a new instance of the conversation processor.

`ApplyToWindow` operator passes frames that are within the given time window to the conversation processor:

```csharp
public static IConversationProcessor<Target> ApplyToWindow<Target>(this IConversationProcessor<Target> source, DateTime windowStart, TimeSpan duration);
```

`ApplyToTake` returns a specified number of contiguous frames from the start of a sequence.
Take enumerates source and yields elements until count elements have been yielded or source contains no more elements.
If count exceeds the number of elements in source, all elements of source are returned.

```csharp
public static IConversationProcessor<Target> ApplyToTake<Target>(this IConversationProcessor<Target> source, int count);
```

`ApplyToTakeWhile` operator returns frames from a sequence as long as a specified condition is true, and then skips the remaining elements.

```csharp
public static IConversationProcessor<Target> ApplyToTakeWhile<Target>(this IConversationProcessor<Target> source,  Func<TSource,Int32,Boolean> predicate);
```

`ApplyToWhere` operator filters a sequence of frames based on a predicate.

```csharp
public static IConversationProcessor<Target> ApplyToWhere<Target>(this IConversationProcessor<Target> source, Func<TSource,Int32,Boolean> predicate);
```

Several operators can be applied. These are evaluated from the right. The rightmost operator is applied first on the source conversation. The result from the leftmost operator is then passed to the conversation processor:

```csharp
processor.ApplyOpN()...ApplyOp1()
```

### Examples

The following example demonstrates how to build the conversation processor, which scope is limited by the time window:

```csharp
processor = ConversationProcessor.FromFunction<string>((key, frames) => $"{key} : {frames.Count}");
var results = table.ProcessConversations(processor.ApplyToWindow(windowStart, windowSpan));
```

It is possible to apply multiple operators:

```csharp
processor.ApplyToTake(10).ApplyToWindow(start, duration);
```

This processor first filters frames by the window and then selects 10 first frames to which the processor is applied.

## ConversationRecord

While conversation processor can be defined to return any type, the library provides an additional support
for conversation processors of type `ConversationRecord<T>`.

Using `ConversationRecord<T>` provides several advantages:

* `ConversationProcessorBase` preprocess the conversation frames to provide some basic conversation information, such as flow metrics. Also, the frames are parsed into packets and provided as two collections each for either direction.
* `ConversationRecord<T>` implements support for DataView of the shelf. It is thus possible to use the output from the conversation processor as the input for the ML.NET pipeline.

Conversation processors that produces `ConversationRecord<T>` can be implemented either by derive from an abstract class `ConversationProcessorBase` or by providing 
a function to `ConversationProcessor.FromFunction` delegate.