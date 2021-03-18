using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Traffix.DataView
{
    public static class DataViewFactory
    {
        /// <summary>
        /// Gets the Data View from a collection of <see cref="FrameRecord{TData}"/> objects.
        /// <para/>
        /// The Data View uses a lazy access to the enumerable. Thus this method does nothing
        /// it just wraps the enumeration in the data view.
        /// </summary>
        /// <typeparam name="T">The type of converation records.</typeparam>
        /// <param name="records">A collection of records to be used as the basis for the data view.</param>
        /// <returns>The dataview for the given enumerable.</returns>
        public static IDataView AsDataView<T>(this IEnumerable<T> records, IDataViewTypeResolver dataViewTypeResolver)
        {
            var d = dataViewTypeResolver.GetDataViewType<T>();
            var columns = d.GetColumns();
            var getters = new DataViewGetters.Builder();
            var schema = new DataViewSchema.Builder();
            foreach (var column in columns)
            {
                getters.AddColumn(column);
                schema.AddColumn(column.Name, column.DataViewType);
            }
            return new DataView<T>(records, getters.ToGetters(), schema.ToSchema());
        }
    }

    public interface IDataViewType
    {
        IReadOnlyCollection<DataViewColumn> GetColumns();
        Type DataViewType { get; }
    }
    public interface IDataViewType<T> : IDataViewType
    {
    }

    public abstract class DataViewType<T> : IDataViewType<T>
    {
        List<DataViewColumn> _columns;

        protected DataViewType()
        {
        }

        Type IDataViewType.DataViewType => typeof(T);

        public IReadOnlyCollection<DataViewColumn> GetColumns()
        {
            if (_columns == null)
            {
                _columns = new List<DataViewColumn>();
                DefineColumns();
            }
            return _columns;
        }

        protected abstract void DefineColumns();

        public DataViewType<T> AddColumn<TColumn>(string name, Expression<Func<T, TColumn>> col)
        {
            
            var getter = col.Compile();
            _columns.Add(new DataViewColumn(name, typeof(TColumn), obj => getter((T)obj)));
            return this;
        }
        public DataViewType<T> AddColumn<TSourceColumn, TTargetColumn>(string name, Expression<Func<T, TSourceColumn>> col, Func<TSourceColumn, TTargetColumn> transform)
        {
            var getter = col.Compile();
            _columns.Add(new DataViewColumn(name, typeof(TTargetColumn), obj => transform(getter((T)obj))));
            return this;
        }

        public DataViewType<T> AddComplexColumn<TInnerType>(string name, Expression<Func<T, TInnerType>> col, IDataViewType<TInnerType> dataViewType)
        {
            foreach(var innerCol in dataViewType.GetColumns())
            {
                var getter = col.Compile();
                _columns.Add(new DataViewColumn(name + innerCol.Name, innerCol.Type, obj => innerCol.ValueGetter(getter((T)obj))));
            }
            return this;
        }
    }

    /// <summary>
    /// An IDataViewTypeResolver is storage of typed records. 
    /// </summary>
    public interface IDataViewTypeResolver
    {
        IDataViewType<T> GetDataViewType<T>();
    }

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
