using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Trainers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffix.DataView.Tests
{

    public class SimpleDataRecord
    {
        public int Value1;
        public int Value2;
    }
    public class SimpleDataRecordType : DataViewType<SimpleDataRecord>
    {
        protected override void DefineColumns() =>
             AddColumn(nameof(SimpleDataRecord.Value1), c => c.Value1)
            .AddColumn(nameof(SimpleDataRecord.Value2), c => c.Value2);
    }

    [TestClass]
    public class DataViewTests
    {
        
        [TestMethod]
        public void CreateSimpleDataView()
        {
            var records = Enumerable.Range(1,1000).Select(r=> new SimpleDataRecord { Value1 = r, Value2 = r*2 });
            var dataview = records.AsDataView(new DataViewTypeResolver(new SimpleDataRecordType()));
            foreach (var row in dataview.Preview(100).RowView)
            {
                foreach (var val in row.Values)
                {
                    Console.Write($"{val}\t");
                }
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void CreateCoumpoundDataView()
        {
            var records = Enumerable.Range(1, 1000).Select(r => new Foo { FooCol1 = r, FooCol2 = r * 2, FooCol3 = r * 3, Bar = new Bar {BarCol1 = r, BarCol2 = r* 2 }  });
            var dataview = records.AsDataView(new DataViewTypeResolver(new DataViewFooType(), new DataViewBarType()));
            foreach (var row in dataview.Preview(100).RowView)
            {
                foreach (var val in row.Values)
                {
                    Console.Write($"{val}\t");
                }
                Console.WriteLine();
            }
        }
    }

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
    public class DataViewFooType : DataViewType<Foo>
    {
        protected override void DefineColumns() =>
             AddColumn(nameof(Foo.FooCol1), x => x.FooCol1.ToString())
            .AddColumn(nameof(Foo.FooCol2), x => x.FooCol2)
            .AddColumn(nameof(Foo.FooCol3), x => x.FooCol3)
            .AddComplexColumn(nameof(Foo.Bar), x => x.Bar, new DataViewBarType());
    }

    public class DataViewBarType : DataViewType<Bar>
    {
        protected override void DefineColumns() =>
             AddColumn(nameof(Bar.BarCol1), x => x.BarCol1)
            .AddColumn(nameof(Bar.BarCol2), x => x.BarCol2);
    }
    

}
