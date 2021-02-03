using Microsoft.Data.Analysis;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace Traffix.Processors
{
    /// <summary>
    /// Implements extension methods for creating <see cref="DataFrame"/> and <see cref="IDataView"/>  from 
    /// <see cref="ConversationRecord{TData}"/>.
    /// </summary>
    public static class ConversationRecordExtensions
    {
        /// <summary>
        /// Gets the dictionary that represents a flatten version of the current record.
        /// <para>
        /// It can be used to flat the conversation record and to create dynamic object from it.
        /// </para>
        /// </summary>
        /// <returns>A dictionary of fields and values for the passed conversation record.</returns>
        public static Dictionary<string, object> AsDictionary<T>(this ConversationRecord<T> self)
        {
            var dictionary = RecordTypeRegister.GetRecordInfo(self.GetType()).ToDictionary(x => x.Name, y => y.ValueGetter.Invoke(self));
            return dictionary;
        }

        /// <summary>
        /// Converts an enumerable of conversation records to <see cref="DataFrame"/>. The <see cref="DataFrame"/>
        /// is a high performance memory store for data sets. 
        /// <para>
        /// The <see cref="DataFrame"/> implements <see cref="Microsoft.ML.IDataView"/> interface 
        /// which is consumed by ML.NET pipelines.
        /// </para>
        /// The difference to <see cref="AsDataView{T}(IEnumerable{ConversationRecord{T}})"/> is that 
        /// DataFrame is fully created in memory from the source records while Data View 
        /// is wrapper around the enumeration possibly evaluated lazily.
        /// </summary>
        /// <typeparam name="T">The type of conversation record.</typeparam>
        /// <param name="records">The enumerable of records.</param>
        /// <returns>The new <see cref="DataFrame"/> object representing the source records.</returns>
        public static DataFrame ToDataFrame<T>(this IEnumerable<ConversationRecord<T>> records)
        {
            var recordList = records.ToList();
            var builder = new DataFrameBuilder();
            var members = RecordTypeRegister.GetRecordInfo(typeof(ConversationRecord<T>));
            foreach (var member in members)
            {
                builder.AddColumn(member, recordList.Select(member.ValueGetter));
            }
            return builder.ToDataFrame(); 
        }

        /// <summary>
        /// Gets the Data View from a collection of <see cref="ConversationRecord{TData}"/> objects.
        /// <para/>
        /// The Data View uses a lazy access to the enumerable. Thus this method does nothing
        /// it just wraps the enumeration in the data view.
        /// </summary>
        /// <typeparam name="T">The type of converation records.</typeparam>
        /// <param name="records">A collection of records to be used as the basis for the data view.</param>
        /// <returns>The dataview for the given enumerable.</returns>
        public static IDataView AsDataView<T>(this IEnumerable<ConversationRecord<T>> records)
        {
            return new ConversationRecordDataView<T>(records);
        }
    }

    public static class DataOperationsCatalogExtensions
    {
        public static void SaveAsMd(this DataOperationsCatalog _, DataDebuggerPreview preview, TextWriter writer)
        {
            writer.WriteLine($"| {String.Join(" | ", preview.Schema.Select(s => s.Name))} |");
            writer.WriteLine($"| {String.Join(" | ", preview.Schema.Select(s => "------"))} |");
            foreach (var row in preview.RowView)
            {
                writer.WriteLine($" | {String.Join(" | ", row.Values.Select(x => x.Value.ToString()))} | ");
            }
        }
    }
}