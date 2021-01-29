# IcsMonitor

IcsMonitor is a tool for capturing, extracting and processing industrial communication. It currently supports Modbus/TCP, DNP3, and Modbus 
but can be easily extended with other protocols. The project is compiled to

* executable application that can be used as a standalone CLI tool
* nuget package that can be used in other project or in C# interactive (Visual Studio Code Notebooks)

One of the scenarios of using the IcsMonitor tool is for [anomaly detection](Docs/Classification.md). 

The documentation of all public classes of the IcsMonitor library is available [here](Docs/IcsMonitor.md).

# Interactive

All public classes are usable in C# Interactive. To support this use case, ```IcsMonitor.Interactive``` class is provided, which 
implements some high-level workflow methods. For example, the dataset for Modbus/TCP communication can be easily created using the following code:

```csharp
#r "nuget:IcsMonitor"
using IcsMonitor;

var ctx = new Interactive();
var dataset = ctx.ComputeModbusDataset(@"C:\Temp\Captures\source.pcap", TimeSpan.FromSeconds(120));
var stat = dataset.Statistics;
stat
```

The computed dataset is an instance of the ```IcsDataset``` class. It is a generic class with a type argument representing a conversation data type. The class exposes two properties for accessing conversations and frames, respectively.

```csharp
public class IcsDataset<TData>
{
  public List<ConversationTable<TData>> ConversationTables { get; }
  public List<RawFrame> Frames { get; }
  ...
}
```

It is possible to use the created dataset as the data source for ML.NET, which accepts ```IDataView``` interface. The dataset provides methods for generating an instance of this interface. ```ConversationTable``` is a collection of ```ConversationRecord``` objects, which provides an extension method ```ToDataFrame```.

```csharp
var records = dataset.ConversationTables.SelectMany(x => x.AsEnumerable());
var dataframe = records.ToDataFrame();
var schema = (dataframe as IDataView).Schema;
```

Variable ```dataframe``` can be used everywhere ```IDataView``` is expected. See https://github.com/dotnet/machinelearning/blob/master/docs/code/IDataViewDesignPrinciples.md for information on ```IDataView``` principles of date representation.
