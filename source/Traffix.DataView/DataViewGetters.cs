using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Traffix.DataView
{
    /// <summary>
    /// Implements the logic supporting <see cref="ValueGetter{TValue}"/> 
    /// for columns of the Data View.
    /// <para/>
    /// It follows the structure of <see cref="DataViewSchema"/> class.
    /// </summary>
    public class DataViewGetters
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

        public class Builder
        {
            List<Getter> _getters;
            public Builder()
            {
                _getters = new List<Getter>();
            }

            public void AddColumn(string name, DataViewType type, Func<object, object> valueGetter)
            {
                var getter = new Getter(name, _getters.Count, type, valueGetter);
                _getters.Add(getter);
            }
            public void AddColumn(DataViewColumn column)
            {
                var getter = new Getter(column.Name, _getters.Count, column.DataViewType, column.ValueGetter);
                _getters.Add(getter);
            }
            public void AddColumns(IEnumerable<DataViewColumn> columns)
            {
                foreach (var col in columns)
                    AddColumn(col);
            }

            public DataViewGetters ToGetters()
            {
                return new DataViewGetters(_getters);
            }
        }
    }
}