# Traffix.Processors

## Conversation Processors

All conversation processors have to implement `IConversationProcessor` interface. 

```csharp
public interface IConversationProcessor<T>
{
    T Invoke(FlowKey flowKey, IEnumerable<Memory<byte>> frames);
}
```

The processor is invoked for each conversation. The conversation is identified by its `FlowKey`and a collection of frames. Each frame is provided as `Memory<byte>` object. 

It is also possible to create a conversation processor from a function:

```csharp
public static IConversationProcessor<TResult> ConversationProcessor.FromFunction<TResult>(Func<FlowKey, IEnumerable<Memory<byte>>, TResult> function)
```

Before the conversation processor is applied to a conversation it is possible to modify the conversation data. It can be useful when we want to apply a conversation processor only for a subset of frames.

### Operators

There are several predefined operators. 

`ApplyToWindow` operator passes frames that are within the given time window to the conversation processor:

```csharp
public static IConversationProcessor<Target> ApplyToWindow<Target>(this IConversationProcessor<Target> source, DateTime windowStart, TimeSpan duration)
```

`ApplyToTake` operator takes up to n-first frames and applies the conversation processor:

```csharp
public static IConversationProcessor<Target> ApplyToTake<Target>(this IConversationProcessor<Target> source, int count)
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
