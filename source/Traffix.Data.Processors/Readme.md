# Traffix.Processors

# Conversation Processors

All conversation processors have to implement `IConversationProcessor` interface. 

```csharp
public interface IConversationProcessor<T>
{
    T Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames);
}
```
Processor is invoked on each conversation. The conversation is identified by its `FlowKey`
and a collection of frames. Each frame is provided as `Memory<byte>` object. 

It is also possible to create conversation processor from a function:

```csharp
public static IConversationProcessor<TResult> ConversationProcessor.FromFunction<TResult>(Func<FlowKey, IEnumerable<Memory<byte>>, TResult> function)
```

Before the conversation processor is applied to a conversation it is possible 
to modify the conversation data. It can be useful when we want to apply conversation processor
only for a subset of frames.

### Operators

There are several predefined operators. 

`ApplyToWindow` operator passed frames that are within the given time window to the conversation processor:

```csharp
public static IConversationProcessor<Target> ApplyToWindow<Target>(this IConversationProcessor<Target> source, DateTime windowStart, TimeSpan duration)
```

`ApplyToTake` operator passes up to n-first frames to the conversation processor:

```csharp
public static IConversationProcessor<Target> ApplyToTake<Target>(this IConversationProcessor<Target> source, int count)
```

## Usage
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
