# Traffix.Extensions.Decoders

Provides an implementation of parsers for several Internet protocols. Binary protocol parsers are generated
from Kaitai specifications. The protocols are organized in groups depedning on their use:
* Base - collection of fundamental Internet protocols.
* Core - the most used Internet protocols. 
* Common - commonly used Internet protocols. 
* IoT - protocols developed for communication in Internet-based IoT systems. 
* Industrial - industrial protocols that communicates above the TCP/IP stack.

## Usage
The following code shows parsing SNMP packet from the raw frame data with the help of PacketDotNet. 

```csharp
var bytes = ...RAW FRAME BYTES...
var packet = Packet.ParsePacket(LinkLayers.Ethernet, bytes);
var app = packet.Extract(typeof(PacketDotNet.ApplicationPacket)) as PacketDotNet.ApplicationPacket;
var snmp = new Snmp(new KaitaiStream(app.Bytes));
```



## Compile Decoders
The Kaitai Struct Compiler needs to be installed on the system in order to recompile parsers. 
Kaitai is available from http://kaitai.io/#download. 

In each subfolder, there are `compile.cmd` and `compile.sh` scripts that generates CS source codes from Kaitai specifications.  


## Kaitai Notes
[Kaitai.IDE](https://ide.kaitai.io/) is a nice environment for developing and debugging Kaitai protocol specifications. However, sometimes you may encounter 
errors when trying to compile a kaitai specification, which seems to be correct in the IDE:
* expression needs to be enclosed in parenthesis, for instance:
```
    - id: data
      size: "(_io.pos + 18 <= _io.size) ? 16 : (_io.size - _io.pos) - 2"
```  