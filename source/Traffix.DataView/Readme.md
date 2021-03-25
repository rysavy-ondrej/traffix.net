# Traffix.DataView

Implements a support for creating `IDataView` compatible collections from complex classes. 
The goal is to provide a simple and efficient way to generate data view records from the results
produced by frame and flow processors.


## Usage

The typical scenario for using this extension is for converting complex objects to data view records.
Consider that we have the following two classes and the enumerable of `Foo` objects. Each `Foo` objects 
contains an instance of `Bar`. 

```csharp
public class Foo
{
    public int FooCol1;
    public int FooCol2;
    public int FooCol3;
    public Bar Bar;
}
public class Bar
{
    public int BarCol1;
    public int BarCol2;
}

var records = Enumerable.Range(1, 1000).Select(r => new Foo { FooCol1 = r, FooCol2 = r * 2, FooCol3 = r * 3, Bar = new Bar {BarCol1 = r, BarCol2 = r* 2 }  });
```

In order to create a data view from an enumerable of `Foo` objects we need to have defined data view types of all realted classes. As our complex objects consists of
two `Foo` and inner `Bar` we have define two data view types. Each data view type definition 
is represented as a class derived from `DataViewType<T>`. It is required to implement method `DefineColumns` which 
defines which columns should be a part of resulting data view. 

```csharp
public class FooDataViewType : DataViewType<Foo>
{
    protected override void DefineColumns(DataViewColumnCollection columns) => columns
        .AddColumn(nameof(Foo.FooCol1), x => x.FooCol1.ToString())
        .AddColumn(nameof(Foo.FooCol2), x => x.FooCol2)
        .AddColumn(nameof(Foo.FooCol3), x => x.FooCol3)
        .AddComplexColumn(nameof(Foo.Bar), x => x.Bar, new BarDataViewType());
}

public class BarDataViewType : DataViewType<Bar>
{
    protected override void DefineColumns(DataViewColumnCollection columns) => columns
        .AddColumn(nameof(Bar.BarCol1), x => x.BarCol1)
        .AddColumn(nameof(Bar.BarCol2), x => x.BarCol2);
}
```

Column definition uses fluent syntax. Each column is defined in terms of its name and the getter function 
to access the column value. For column that consists of a complex type `AddComplexColumn` method is used, which references
the data view type of the complex type. The presented data type definitions generates the type equivalent to the following record:

```
record _Foo(
    string FooCol1,
    int FooCol2,
    int FooCol3,
    int BarBarCol1,
    int BarBarCol2
);
```

Creating the data view from the enumerable is straightforward by calling `AsDataView` extension method, which requires 
an instance of data view type resolver. The resolver has a single goal, which is to provide the required data view type instances for
transforming the complex objects to data view. In our case it is enough to create the resolver with `FooDataViewType` and `BarDataViewType` objects.

```csharp
var dataview = records.AsDataView(new DataViewTypeResolver(new FooDataViewType(), new BarDataViewType()));
```

To see the resulting data view it is possible to use get the Preview and iterate trhough all rows: 

``` csharp
foreach (var row in dataview.Preview(100).RowView)
{
    foreach (var val in row.Values)
    {
        Console.Write($"{val}\t");
    }
    Console.WriteLine();
}
```