# IcsMonitor

IcsMonitor is a tool for capturing, extracting and processing industrial communication. It currently supports Modbus/TCP, DNP3, and Modbus 
but can be easily extended with other protocols. The project is compiled to

* executable application that can be used as a standalone CLI tool
* nuget package that can be used in other project or in C# interactive (Visual Studio Code Notebooks)

# Interactive

All public classes are usable in C# Interactive. In order to support this use case, ```IcsMonitor.Interactive``` class is provided, which 
implements some hihg-level workflow methods. 

The datatset for Modbus/Tcp communication can be easily created using the followin code:
```csharp
#r "nuget:IcsMonitor"
using IcsMonitor;

var ctx = new Interactive();
var dataset = ctx.ComputeModbusDataset(@"C:\Temp\Captures\source.pcap", TimeSpan.FromSeconds(120));
var stat = dataset.Statistics;
stat
```
