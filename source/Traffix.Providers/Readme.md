# Traffix.Providers

This library contains implementations of various PCAP file readers and other sources of packet capture data. 


## Observable

Packet capture readers can also be used to create observable sequences, which are widely used in other Traffix packages.
The code snippet to create the observable sequence of packets is as follows:

```csharp
using Traffix.Providers.PcapFile;

using var reader = new SharpPcapReader("source.pcap");
var observable = reader.CreateObservable();

// ... use RawFrame observable here ...
```

The observable is a sequence of `RawFrame` objects.

## ManagedPcapReader

A simple (and fast) PCAP file forward only reader. It enables to read frames from the traditional PCAP file (tcpdump format) only.
The usage is very simple:

```csharp
using(var reader = new ManagedPcapReader(stream))
{
    while(reader.GetNextFrame(out var frame))
    {
        // do what you need with the new frame object here
        // ...
    }
}
```

The PCAP file contains a header followed by the packet records.
The header is read immediately when the reader is created thus it 
is assumed that the stream is properly positioned. 

PCAP Header:

| Field | Size | Description |
| ----- | ---- | ------------|
| Magic Number | U4 | Magic number, which is always `D4C3B2A1`. |
| Version      | U4 | Version number, usually `02000400`. |
| Zone         | U4 | Time zone information. |
| SIG FIGS     | U4 | ??? |
| Snap Len     | U4 | The snapshot length.If the snapshot length is set to snaplen, and snaplen is less than the size of a packet that is captured, only the first snaplen bytes of that packet will be captured and provided as packet data. |
| Network Type | U4 | The type of link layer. `01000000` for Ethernet |

Each packet is recorded with 16 bytes header prepending the frame bytes:

| Field | Size | Description |
| ----- | ---- | ------------|
| Timestamp Seconds | U4 | Unix timestamp value is seconds. |
| Timestamp Microseconds | U4 | Fraction of seconds of the timestamp. |
| Included Length | U4 | The length of the packet byte array. | 
| Original Length | U4 | The original size of the frame. It can be different to Included Length is Snap Len is set. |