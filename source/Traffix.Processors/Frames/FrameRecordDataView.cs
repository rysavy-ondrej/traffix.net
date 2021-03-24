using Microsoft.ML;
using System.Collections.Generic;

namespace Traffix.Processors
{
    /// <summary>
    /// This is an implementation of <see cref="IDataView"/> that wraps an
    /// <see cref="IEnumerable{T}"/> of <see cref="FrameRecord{TData}"/>.
    /// <para/>
    /// Instances of this class cannot be created directly. Instead, 
    /// use the extension method <see cref="FrameRecordExtensions.AsDataView{T}(IEnumerable{FrameRecord{T}})"/>.
    /// <para/>
    /// We need this implementation as <see cref="FrameRecord{TData}"/>
    /// is not a flat structure with public fields suitable for direct use as <see cref="IDataView"/>.
    /// Using nested object as a flat record compatible with IDataView is 
    /// done through using reflection for this moment. 
    /// </summary>
    internal sealed class FrameRecordDataView<TData> : RecordDataView<FrameRecord<TData>>
    {
        internal FrameRecordDataView(IEnumerable<FrameRecord<TData>> data) : base(data) { }    
    }
}