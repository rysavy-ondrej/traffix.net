# Traffix.Core

This package contains the fundamental classes supporting the Traffix infrastructure. 

# Flows

The basic structures and classes for representing flow keys and related utilities.

# Frames

A collection of structures for representing frames:

* `FrameKey` --  The key of a frame. It is represented as ulong value internally. The 64-bit long key consits of 32-bit Unix Epoch with seconds resolution and 32-bit frame number.
* `FrameMetadata` -- represents the metadata of a single frame. It is a structure that has fixed size of 20 bytes and provides timestamp, link layer type, original length and flow key hash.
* `FrameRef` -- Represents a reference structure to frame key, metadata and its bytes.

# Observables

Implements observable operators for sequence of packets and flow aggregation. 

# Processors

Interfaces and base classes for implementation of frame and flow processors.
