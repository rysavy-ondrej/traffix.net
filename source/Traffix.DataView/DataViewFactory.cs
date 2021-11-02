using Microsoft.ML;
using System.Collections.Generic;

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
 
}
