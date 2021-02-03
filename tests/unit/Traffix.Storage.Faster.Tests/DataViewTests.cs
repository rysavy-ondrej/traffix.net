using MessagePack;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using Traffix.Processors;

namespace Traffix.Storage.Faster.Tests
{
    [TestClass]
    public class DataViewTests
    {
        
        struct SimpleData
        {
            [Key("Counter")]
            public int Counter;
            [Key("Label")]
            public string Label;
        }

        struct CompoundData
        {
            [Key("Timestamp")]
            public DateTime Timestamp;
            [Key("SimpleData")]
            public SimpleData SimpleData;
        }

        [TestMethod]
        public void GetDataView()
        {
            var collection = new[]
            {
                new ConversationRecord<SimpleData>(),
                new ConversationRecord<SimpleData>(),
            };
            var dataview = collection.AsDataView();
            foreach(var col in dataview.Schema)
            {
                Console.WriteLine($"{col.Index} {col.Name} : {col.Type}");
            }
        }
    }
}
