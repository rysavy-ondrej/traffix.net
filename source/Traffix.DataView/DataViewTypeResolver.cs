using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffix.DataView
{
    public class DataViewTypeResolver : IDataViewTypeResolver
    {
        Dictionary<Type, IDataViewType> _dataViewTypeMap;

        public DataViewTypeResolver(params IDataViewType[] dataViewTypes)
        {
            _dataViewTypeMap = new Dictionary<Type, IDataViewType>(dataViewTypes.Select(k => KeyValuePair.Create(k.DataViewType, k)));
        }
        public DataViewTypeResolver(IEnumerable<IDataViewType> dataViewTypes)
        {
            _dataViewTypeMap = new Dictionary<Type, IDataViewType>(dataViewTypes.Select(k=>KeyValuePair.Create(k.DataViewType, k)));
        }

        public IDataViewType<T> GetDataViewType<T>() => _dataViewTypeMap.GetValueOrDefault(typeof(T)) as IDataViewType<T>;
    }
}
