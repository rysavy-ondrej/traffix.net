using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffix.DataView
{
    /// <summary>
    /// A common implementation of Record DataViews.
    /// </summary>
    /// <typeparam name="TData">The type of data in underlying collection.</typeparam>
    internal partial class DataView<TData> : IDataView
    {
        private readonly IEnumerable<TData> _data;
        private readonly DataViewSchema _schema;
        private readonly DataViewGetters _getters;
        /// <summary>
        /// Gets an enumerable behind this data view.
        /// </summary>
        internal IEnumerable<TData> Data => _data;

        /// <inheritdoc/>
        public DataViewSchema Schema => _schema;

        /// <inheritdoc/>
        public bool CanShuffle => false;

        internal DataView(IEnumerable<TData> data, DataViewGetters getters, DataViewSchema schema)
        {
            _data = data;
            _getters = getters;
            _schema = schema;
        }

        /// <inheritdoc/>
        public long? GetRowCount() => null;

        /// <inheritdoc/>
        public DataViewRowCursor GetRowCursor(
            IEnumerable<DataViewSchema.Column> columnsNeeded,
            Random rand = null)

            => new Cursor(this, columnsNeeded.ToArray());

        /// <inheritdoc/>
        public DataViewRowCursor[] GetRowCursorSet(
            IEnumerable<DataViewSchema.Column> columnsNeeded, int n,
            Random rand = null)

            => new[] { GetRowCursor(columnsNeeded, rand) };


        private sealed class Cursor : DataViewRowCursor
        {
            private bool _disposed;
            private long _position;
            private readonly IEnumerator<TData> _enumerator;
            private readonly Delegate[] _getters;
            private readonly DataViewSchema _schema;
            private readonly DataView<TData> _parent;

            /// <inheritdoc/>
            public override long Position => _position;

            /// <inheritdoc/>
            public override long Batch => 0;

            /// <inheritdoc/>
            public override DataViewSchema Schema => _schema;

            public Cursor(DataView<TData> parent, params DataViewSchema.Column[] columns)

            {
                var schemaBuilder = new DataViewSchema.Builder();
                schemaBuilder.AddColumns(columns);
                _schema = schemaBuilder.ToSchema();
                _parent = parent;
                _position = -1;
                _enumerator = parent.Data.GetEnumerator();
                _getters = columns.Select(col => parent._getters[col.Index].CreateDelegate(_enumerator)).ToArray();
            }

            protected override void Dispose(bool disposing)
            {
                if (_disposed)
                    return;
                if (disposing)
                {
                    _enumerator.Dispose();
                    _position = -1;
                }
                _disposed = true;
                base.Dispose(disposing);
            }

            private void IdGetterImplementation(ref DataViewRowId id)
                => id = new DataViewRowId((ulong)_position, 0);

            /// <inheritdoc/>
            public override ValueGetter<TValue> GetGetter<TValue>(
                DataViewSchema.Column column)
            {
                if (!IsColumnActive(column))
                    throw new ArgumentOutOfRangeException(nameof(column));
                return (ValueGetter<TValue>)_getters[column.Index];
            }

            /// <inheritdoc/>
            public override ValueGetter<DataViewRowId> GetIdGetter()
                => IdGetterImplementation;

            /// <inheritdoc/>
            public override bool IsColumnActive(DataViewSchema.Column column)
                => _getters[column.Index] != null;

            /// <inheritdoc/>
            public override bool MoveNext()
            {
                if (_disposed)
                    return false;
                if (_enumerator.MoveNext())
                {
                    _position++;
                    return true;
                }
                Dispose();
                return false;
            }
        }
    }

    /*
        static readonly Dictionary<Type, DataViewGetters> _gettersDictionary = new Dictionary<Type, DataViewGetters>();
        static readonly Dictionary<Type, DataViewSchema> _schemaDictionary = new Dictionary<Type, DataViewSchema>();
        private static DataViewSchema GetOrCreateSchema(Type type)
        {
            if (!_schemaDictionary.TryGetValue(type, out var schema))
            {
                schema = CreateSchema(type);
                _schemaDictionary[type] = schema;
            }
            return schema;
        }

        private static DataViewSchema CreateSchema(Type type)
        {
            var builder = new DataViewSchema.Builder();
            var members = DataViewTypeRegister.GetRecordInfo(type);
            foreach (var member in members)
            {
                builder.AddColumn(member.Name, member.DataViewType);
            }
            return builder.ToSchema();
        }

        private static DataViewGetters GetOrCreateGetters(Type type)
        {
            if (!_gettersDictionary.TryGetValue(type, out var getters))
            {
                getters = DataViewGetters.CreateForType(type);
                _gettersDictionary[type] = getters;
            }
            return getters;
        }
    */
}