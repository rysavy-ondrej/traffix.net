using System;
using System.Collections.Generic;

namespace Traffix.DataView
{
    public interface IDataViewType
    {
        IReadOnlyCollection<DataViewColumn> GetColumns();
        Type DataViewType { get; }
    }
    public interface IDataViewType<T> : IDataViewType
    {
    }
}
