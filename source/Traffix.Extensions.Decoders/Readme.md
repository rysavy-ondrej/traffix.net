# Ndx.Packets

The project provides an implementation of commonly used application protocol parsers. Binary protocol parsers are generated
from Kaitai specification. 

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


* To recompile BASE Internet protocols:
```
Kaitai/base# kaitai-struct-compiler -t csharp --dotnet-namespace Loiret.Domain.Captures.Decoders.Base -d ../../Base *.ksy
```

* To recompile common internet protocols
```
Kaitai/common# kaitai-struct-compiler -t csharp --dotnet-namespace Loiret.Domain.Captures.Decoders.Common -d ../../Common *.ksy
```

* To recompile common internet protocols
```
Kaitai/core# kaitai-struct-compiler -t csharp --dotnet-namespace Loiret.Domain.Captures.Decoders.Core -d ../../Core *.ksy
```

* To recompile industrial protocol parsers:
```
Kaitai/dlms# kaitai-struct-compiler -t csharp --dotnet-namespace Loiret.Domain.Captures.Decoders.Industrial -d ../../Industrial/Dlms *.ksy
Kaitai/modbus# kaitai-struct-compiler -t csharp --dotnet-namespace Loiret.Domain.Captures.Decoders.Industrial -d ../../Industrial *.ksy
```

* To recompile IoT parsers:
```
Kaitai/iot# kaitai-struct-compiler -t csharp --dotnet-namespace Loiret.Domain.Captures.Decoders.IoT -d ../../IoT *.ksy
```