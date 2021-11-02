using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffix.Processors
{
    /// <summary>
    /// This is an implementation of <see cref="IDataView"/> that wraps an
    /// <see cref="IEnumerable{T}"/> of <see cref="ConversationRecord{TData}"/>.
    /// <para/>
    /// Instances of this class cannot be created directly. Instead, 
    /// use the extension method <see cref="ConversationRecordExtensions.AsDataView{T}(IEnumerable{ConversationRecord{T}})"/>.
    /// <para/>
    /// We need this implementation as <see cref="ConversationRecord{TData}"/>
    /// is not a flat structure with public fields suitable for direct use as <see cref="IDataView"/>.
    /// Using nested object as a flat record compatible with IDataView is 
    /// done through using reflection for this moment. 
    /// </summary>
    internal sealed class ConversationRecordDataView<TData> : RecordDataView<ConversationRecord<TData>>
    {
        internal ConversationRecordDataView(IEnumerable<ConversationRecord<TData>> data) : base(data)
        { }
    }
}