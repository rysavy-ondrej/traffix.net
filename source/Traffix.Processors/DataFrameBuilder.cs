using Microsoft.Data.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Traffix.Processors
{
    internal class DataFrameBuilder
    {
        List<DataFrameColumn> _columns;

        public DataFrameBuilder()
        {
            _columns = new List<DataFrameColumn>();
        }


        public void AddColumn(RecordMemberInfo member, IEnumerable values)
        {
            _columns.Add(CreateColumn(member, values));
        }
        /// <summary>
        /// Gets a single column of <see cref="DataFrame"/> including all associated values.
        /// <para/>
        /// As the column can only be primitive type, the method performs necessary conversions.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="columnType">The column type.</param>
        /// <param name="values">The collection of values that must be of the <paramref name="columnType"/>.</param>
        /// <returns>The new <see cref="DataFrameColumn"/>for the parameters specified.</returns>
        public static DataFrameColumn CreateColumn(RecordMemberInfo member, IEnumerable values)
        {
            var columnType = member.Type;
            var name = member.Name;
            if (columnType == typeof(bool))
            {
                return new PrimitiveDataFrameColumn<bool>(name, values.Cast<bool>());
            }
            else if (columnType == typeof(byte))
            {
                return new PrimitiveDataFrameColumn<byte>(name, values.Cast<byte>());
            }
            else if (columnType == typeof(sbyte))
            {
                return new PrimitiveDataFrameColumn<sbyte>(name, values.Cast<sbyte>());
            }
            else if (columnType == typeof(short))
            {
                return new PrimitiveDataFrameColumn<short>(name, values.Cast<short>());
            }
            else if (columnType == typeof(ushort))
            {
                return new PrimitiveDataFrameColumn<ushort>(name, values.Cast<ushort>());
            }
            else if (columnType == typeof(int))
            {
                return new PrimitiveDataFrameColumn<int>(name, values.Cast<int>());
            }
            else if (columnType == typeof(uint))
            {
                return new PrimitiveDataFrameColumn<uint>(name, values.Cast<uint>());
            }
            else if (columnType == typeof(long))
            {
                return new PrimitiveDataFrameColumn<long>(name, values.Cast<long>());
            }
            else if (columnType == typeof(ulong))
            {
                return new PrimitiveDataFrameColumn<ulong>(name, values.Cast<ulong>());
            }
            else if (columnType == typeof(IntPtr))
            {
                return new PrimitiveDataFrameColumn<long>(name, values.Cast<IntPtr>().Select(p => p.ToInt64()));
            }
            else if (columnType == typeof(UIntPtr))
            {
                return new PrimitiveDataFrameColumn<ulong>(name, values.Cast<UIntPtr>().Select(p => p.ToUInt64()));
            }
            else if (columnType == typeof(char))
            {
                return new PrimitiveDataFrameColumn<char>(name, values.Cast<char>());
            }
            else if (columnType == typeof(double))
            {
                return new PrimitiveDataFrameColumn<double>(name, values.Cast<double>());
            }
            else if (columnType == typeof(float))
            {
                return new PrimitiveDataFrameColumn<float>(name, values.Cast<float>());
            }
            else if (columnType == typeof(DateTime))
            {
                return new PrimitiveDataFrameColumn<long>(name, values.Cast<DateTime>().Select(d => d.Ticks));
            }
            else if (columnType == typeof(string))
            {
                return new StringDataFrameColumn(name, values.Cast<string>());
            }
            else
            {
                return new StringDataFrameColumn(name, values.Cast<object>().Select(x => x.ToString()));
            }
        }

        internal DataFrame ToDataFrame()
        {
            return new DataFrame(_columns);
        }
    }
}