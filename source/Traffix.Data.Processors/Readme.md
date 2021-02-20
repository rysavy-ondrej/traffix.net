# Traffix.Processors

# Conversation Processors

A collection conversation processor operators enables to build the required conversation processor.

the conversation processor can be built from a function:

```csharp
public static IConversationProcessor<TResult> ConversationProcessor.FromFunction<TResult>(Func<FlowKey, ICollection<Memory<byte>>, TResult> function)
```

To adjust the type of results of the conversation processor `Transform` operator can be applied:

```csharp
public static IConversationProcessor<Target> Transform<TSource, Target>(this IConversationProcessor<TSource> source, Func<TSource, Target> transform) 
```

`Window` operator enable to filter frames provided to the conversation processor. Only frames that are wihtin the given time window 
are send to conversation processor:

```csharp
public static IConversationProcessor<Target> Window<Target>(this IConversationProcessor<Target> source, DateTime windowStart, TimeSpan duration)
```

## Usage
the following example demonstrates the usage of conversation processor operators.

```csharp
processor = ConversationProcessor.FromFunction<string>((key, frames) => $"{key} : {frames.Count}").Window(windowStart, windowSpan);
var results = table.ProcessConversations(processor);
```


