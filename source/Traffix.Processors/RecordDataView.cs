using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffix.Processors
{
    /// <summary>
    /// A common implementation of Record DataViews.
    /// </summary>
    /// <typeparam name="TData">The type of data in underlying collection.</typeparam>
    abstract class RecordDataView<TData> : IDataView
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

        protected RecordDataView(IEnumerable<TData> data)
        {
            _data = data;
            _getters = GetOrCreateGetters(typeof(TData));
            _schema = GetOrCreateSchema(typeof(TData));
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
            private readonly RecordDataView<TData> _parent;

            /// <inheritdoc/>
            public override long Position => _position;

            /// <inheritdoc/>
            public override long Batch => 0;

            /// <inheritdoc/>
            public override DataViewSchema Schema => _schema;

            public Cursor(RecordDataView<TData> parent, params DataViewSchema.Column[] columns)

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
            var members = RecordTypeRegister.GetRecordInfo(type);
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


        /// <summary>
        /// Implements the logic supporting <see cref="ValueGetter{TValue}"/> 
        /// for columns of the Data View.
        /// <para/>
        /// It follows the structure of <see cref="DataViewSchema"/> class.
        /// </summary>
        class DataViewGetters
        {
            readonly Getter[] _getters;
            readonly Dictionary<string, int> _index;
            public DataViewGetters(IEnumerable<Getter> getters)
            {
                _getters = getters.ToArray();
                _index = getters.ToDictionary(x => x.Name, x => x.Index);
            }
            public Getter this[string name] => _getters[_index[name]];
            public Getter this[int columnIndex] => _getters[columnIndex];
            public int Count => _getters.Length;
            public readonly struct Getter
            {
                //
                // Summary:
                //     The name of the column.
                public readonly string Name;
                //
                // Summary:
                //     The column's index in the schema.
                public readonly int Index;
                //
                // Summary:
                //     The type of the column.
                public readonly DataViewType Type;
                //
                // Summary:
                //     The deleagte that can be used to acces the column's value.
                public readonly Func<object, object> AccessValueFunction;

                public Getter(string name, int index, DataViewType type, Func<object, object> accessValueFunction)
                {
                    Name = name;
                    Index = index;
                    Type = type;
                    AccessValueFunction = accessValueFunction;
                }

                /// <summary>
                /// Creates <see cref="ValueGetter{TValue}"/> delegate for the given type.
                /// </summary>
                /// <param name="enumerator">An enumerator used to access the current object value from the generated value getter.</param>
                /// <returns>Delegate of type <see cref="ValueGetter{TValue}"/>.</returns>
                public Delegate CreateDelegate<T>(IEnumerator<T> enumerator)
                {
                    var type = Type;
                    var accessValueFunction = AccessValueFunction;
                    switch (type)
                    {
                        case BooleanDataViewType _:
                            return new ValueGetter<bool>((ref bool value) => accessValueFunction(value = (bool)accessValueFunction.Invoke(enumerator.Current)));
                        case NumberDataViewType number:
                            if (number.RawType == typeof(Byte)) return new ValueGetter<byte>((ref byte value) => value = (byte)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(Double)) return new ValueGetter<Double>((ref Double value) => value = (Double)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(Int16)) return new ValueGetter<Int16>((ref Int16 value) => value = (Int16)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(Int32)) return new ValueGetter<Int32>((ref Int32 value) => value = (Int32)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(Int64)) return new ValueGetter<Int64>((ref Int64 value) => value = (Int64)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(SByte)) return new ValueGetter<SByte>((ref SByte value) => value = (SByte)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(Single)) return new ValueGetter<Single>((ref Single value) => value = (Single)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(UInt16)) return new ValueGetter<UInt16>((ref UInt16 value) => value = (UInt16)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(UInt32)) return new ValueGetter<UInt32>((ref UInt32 value) => value = (UInt32)accessValueFunction.Invoke(enumerator.Current));
                            if (number.RawType == typeof(UInt64)) return new ValueGetter<UInt64>((ref UInt64 value) => value = (UInt64)accessValueFunction.Invoke(enumerator.Current));
                            break;
                        case TextDataViewType _:
                            return new ValueGetter<ReadOnlyMemory<char>>((ref ReadOnlyMemory<char> value) => value = ((string)accessValueFunction.Invoke(enumerator.Current)).AsMemory());
                        case DateTimeDataViewType _:
                            return new ValueGetter<DateTime>((ref DateTime value) => value = (DateTime)accessValueFunction.Invoke(enumerator.Current));
                        case DateTimeOffsetDataViewType _:
                            return new ValueGetter<DateTimeOffset>((ref DateTimeOffset value) => value = (DateTimeOffset)accessValueFunction.Invoke(enumerator.Current));
                        case TimeSpanDataViewType _:
                            return new ValueGetter<TimeSpan>((ref TimeSpan value) => value = (TimeSpan)accessValueFunction.Invoke(enumerator.Current));
                    }
                    throw new NotSupportedException($"The data view type {type} is not supported");
                }
            }

            internal class Builder
            {
                List<Getter> _getters;
                public Builder()
                {
                    _getters = new List<Getter>();
                }

                internal void AddColumn(string name, DataViewType type, Func<object, object> valueGetter)
                {
                    var getter = new Getter(name, _getters.Count, type, valueGetter);
                    _getters.Add(getter);
                }

                internal DataViewGetters ToGetters()
                {
                    return new DataViewGetters(_getters);
                }
            }
            public static DataViewGetters CreateForType(Type type)
            {
                var builder = new DataViewGetters.Builder();
                var members = RecordTypeRegister.GetRecordInfo(type);
                foreach (var member in members)
                {
                    builder.AddColumn(member.Name, member.DataViewType, member.ValueGetter);
                }
                return builder.ToGetters();
            }
        }
    }
}