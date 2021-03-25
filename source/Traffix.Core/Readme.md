# Traffix.Core

This package contains the fundamental classes supporting the Traffix infrastructure. 

# Flows

The basic structures and classes for representing flow keys and related utilities.

# Frames

A collection of structures for representing frames:

* `FrameKey` represents the key of a frame. Internally, it is ulong value. The 64-bit key consits of 32-bit Unix Epoch with seconds resolution and 32-bit frame number.
* `FrameMetadata` desribes the metadata of a single frame. It is a structure that has fixed size of 20 bytes and provides timestamp, link layer type, original length and flow key hash.
* `FrameRef` is a reference structure to frame key, metadata and its bytes.

# Observables

Implements observable operators for sequence of packets and flow aggregation:

| Operator             | Description |
|--------------------- | --------- |
| ApplyFlowProcessor   | Applies the flow processor to an observable sequence of flows. |
| GroupFlows           | Projects each element of an observable sequence into the corresponding flow.  |
| GroupConversations   | Projects each element of an observable sequence into the corresponding conversation. |
| GroupFlowsDictionary | Projects each element of an observable sequence into the corresponding flow. |
| TimeSpanWindow       | Projects each element of an observable sequence into consecutive non-overlapping window |

Please, check [the operator documentation](Observable/Readme.md) for more information.

# Processors

Interfaces and base classes for implementation of frame and flow processors.
