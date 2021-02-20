# Traffix.Processors

# Conversation Processors

A collection conversation processor operators enables to build the required conversation processor.

the conversation processor can be built from a function:

```csharp
public static IConversationProcessor<TResult> ConversationProcessor.FromFunction<TResult>(Func<FlowKey, ICollection<Memory<byte>>, TResult> function)
```

`Window` operator enables to filter frames provided to the conversation processor. Only frames that are wihtin the given time window 
are sent to conversation processor:

```csharp
public static IConversationProcessor<Target> Window<Target>(this IConversationProcessor<Target> source, DateTime windowStart, TimeSpan duration)
```

## Usage
The following example demonstrates how to build the conversation processor, which scope is limited by the time window:

```csharp
processor = ConversationProcessor.FromFunction<string>((key, frames) => $"{key} : {frames.Count}").Window(windowStart, windowSpan);
var results = table.ProcessConversations(processor);
```


