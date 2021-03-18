using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Traffix.DataView
{
    /// <summary>
    /// Abstract base class for custom DataViewType.
    /// It is enough to implement <see cref="DefineColumns"/> method to provide 
    /// necessary column specification. 
    /// <para/>
    /// The name of implementing class should use DataViewType suffix added to the type name. For instance, 
    /// for FooRecord the name should be FooRecordDataViewType.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataViewType<T> : IDataViewType<T>
    {
        DataViewColumnCollection _columnCollection;

        Type IDataViewType.DataViewType => typeof(T);

        public IReadOnlyCollection<DataViewColumn> GetColumns()
        {
            if (_columnCollection == null)
            {
                _columnCollection = new DataViewColumnCollection();
                DefineColumns(_columnCollection);
            }
            return _columnCollection.Columns;
        }

        /// <summary>
        /// Implemented in the derived class to provide a collection of columns for the 
        /// custom DataViewType.
        /// </summary>
        /// <param name="columns">The column collection used for adding new columns.</param>
        protected abstract void DefineColumns(DataViewColumnCollection columns);

        public class DataViewColumnCollection 
        {
            private List<DataViewColumn> _columns = new List<DataViewColumn>();
            public DataViewColumnCollection AddColumn<TColumn>(string name, Expression<Func<T, TColumn>> col)
            {
                var getter = col.Compile();
                _columns.Add(new DataViewColumn(name, typeof(TColumn), obj => getter((T)obj)));
                return this;
            }
            public DataViewColumnCollection AddColumn<TSourceColumn, TTargetColumn>(string name, Expression<Func<T, TSourceColumn>> col, Func<TSourceColumn, TTargetColumn> transform)
            {
                var getter = col.Compile();
                _columns.Add(new DataViewColumn(name, typeof(TTargetColumn), obj => transform(getter((T)obj))));
                return this;
            }

            public DataViewColumnCollection AddComplexColumn<TInnerType>(string name, Expression<Func<T, TInnerType>> col, IDataViewType<TInnerType> dataViewType)
            {
                foreach (var innerCol in dataViewType.GetColumns())
                {
                    var getter = col.Compile();
                    _columns.Add(new DataViewColumn(name + innerCol.Name, innerCol.Type, obj => innerCol.ValueGetter(getter((T)obj))));
                }
                return this;
            }
            internal IReadOnlyCollection<DataViewColumn> Columns => _columns;
        }
    }
}
