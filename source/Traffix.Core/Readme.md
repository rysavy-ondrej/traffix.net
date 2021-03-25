# Traffix.Core

This package contains the fundamental classes supporting the Traffix infrastructure. 

## Flows

The basic structures and classes for representing flow keys and related utilities.

## Frames

A collection of structures for representing frames:

* `FrameKey` represents the key of a frame. Internally, it is ulong value. The 64-bit key consits of 32-bit Unix Epoch with seconds resolution and 32-bit frame number.
* `FrameMetadata` desribes the metadata of a single frame. It is a structure that has fixed size of 20 bytes and provides timestamp, link layer type, original length and flow key hash.
* `FrameRef` is a reference structure to frame key, metadata and its bytes.

## Observables

Implements observable operators for sequence of packets and flow aggregation:

| Operator             | Description |
|--------------------- | --------- |
| ApplyFlowProcessor   | Applies the flow processor to an observable sequence of flows. |
| GroupFlows           | Projects each element of an observable sequence into the corresponding flow.  |
| GroupConversations   | Projects each element of an observable sequence into the corresponding conversation. |
| GroupFlowsDictionary | Projects each element of an observable sequence into the corresponding flow. |
| TimeSpanWindow       | Projects each element of an observable sequence into consecutive non-overlapping window |

Please, check [the operator documentation](Observable/Readme.md) for more information.

## Processors

Provides interfaces and base classes for implementation of frame and flow processors.

`FlowProcessor<TSource, TFlowKey, TFlowRecord>` is an abstract class that implements `IObserver<T>`. 
It accepts the sequence of source objects (packets) and collects flow records. To implement the 
concrete flow processor the derived class must provide the following methods:

| Method               | Description |
|--------------------- | ----------- |
| `TFlowKey GetFlowKey(TSource source)` | Gets the flow key for the given source element. |
| `TFlowRecord Create(TSource source)` | Creates a new flow record from the first source element. |
| `void Update(TFlowRecord record, TSource source)` | Updates the existing flow record with a new element.  |
| `TFlowRecord Aggregate(TFlowRecord arg1, TFlowRecord arg2)` | Aggregates flow records and creates a new resulting record. |

### Example
The following example demonstrates the implementation of the simple NetFlow-like flow processor.

```csharp
class NetFlowProcessor : FlowProcessor<(long Ticks, FlowKey Key, Packet Packet), FlowKey, NetFlowRecord>
```

Source objects are given by tuple that for each packet provides its timestamp, precomputed flow key and the packet itself. 
The flow record is defined as `NetFlowRecord` class that internally uses structure to store the flow record values:

```csharp
public class NetFlowRecord
{
    public readonly struct _
    {
        public readonly long Octets;
        public readonly int Packets;
        public readonly long FirstSeen;
        public readonly long LastSeen;

        public _(int packets, long octets, long firstSeen, long lastSeen)
        {
            Packets = packets;
            Octets = octets;
            FirstSeen = firstSeen;
            LastSeen = lastSeen;
        }

        public static _ Aggregate(_ x, _ y)
        {
            return new _(x.Packets + x.Packets, x.Octets + y.Octets, Math.Min(x.FirstSeen, y.FirstSeen), Math.Max(x.LastSeen, y.LastSeen)); 
        }
    }

    public _ Value { get; private set; }

    public static NetFlowRecord Create((long, FlowKey, Packet) packet)
    {
        return new NetFlowRecord { Value = new _(packet.Item3.TotalPacketLength, 1, packet.Item1, packet.Item1) };
    }

    public static void Update(NetFlowRecord record, (long, FlowKey, Packet) packet)
    {
        record.Value = _.Aggregate(record.Value, new _(packet.Item3.TotalPacketLength, 1, packet.Item1, packet.Item1));
    }

    public static NetFlowRecord Aggregate(NetFlowRecord arg1, NetFlowRecord arg2)
    {
        return new NetFlowRecord { Value = _.Aggregate(arg1.Value, arg2.Value) };
    }
}
```

Then the implementation of `NetFlowProcessor` implements required abstract methods with the help of `NetFlowRecord` class.

```csharp
class NetFlowProcessor : FlowProcessor<(long Ticks, FlowKey Key, Packet Packet), FlowKey, NetFlowRecord>
{
    protected override NetFlowRecord Aggregate(NetFlowRecord arg1, NetFlowRecord arg2)
    {
        return  NetFlowRecord.Aggregate(arg1, arg2);
    }

    protected override NetFlowRecord Create((long, FlowKey, Packet) source)
    {
        return NetFlowRecord.Create(source);
    }

    protected override FlowKey GetFlowKey((long, FlowKey, Packet) source)
    {
        return source.Item2;
    }

    protected override void Update(NetFlowRecord record, (long, FlowKey, Packet) source)
    {
        NetFlowRecord.Update(record, source);
    }
}
```

The implemented flow processor can be used in the following snippet to collect flows for all packets from the input sequence:

```csharp
var observable = SharpPcapReader.CreateObservable(pcapPath).Select(TestHelperFunctions.GetPacket);
var flowProcessor = new NetFlowProcessor();

observable.Subscribe(flowProcessor);
// need to wait for the end of input sequence to get all flows
await flowProcessor.Completed;   

// ...use flowProcessor to get list of identified flows... 
```

After the flow processor consumes all packets, it is possible to query the processor to get all flows: 

```csharp
foreach (var flow in flowProcessor.Flows)
{
    // ... do something with flow ...    
}
```

It is easy to obtain conversations (bidirectional flows) from the flow processor with the help of 
method to generate a conversation key from the flow key, for instance:

```
ConversationKey GetConversationKey(FlowKey flowKey)
{
    if (flowKey.SourcePort > flowKey.DestinationPort)
    {
        return new ConversationKey(flowKey);
    }
    else
    {
        return new ConversationKey(flowKey.Reverse());
    }
}

foreach (var conversation in flowProcessor.GetConversations(GetConversationKey))
{
    // ... do something with conversation ...
}
```














