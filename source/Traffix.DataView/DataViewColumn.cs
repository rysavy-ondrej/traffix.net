using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace Traffix.DataView
{
    /// <summary>
    /// Provides a column information for the given type member.
    /// <para/> 
    /// It contains name and type of the member as well as <see cref="ValueGetter"/> function.
    /// </summary>
    public class DataViewColumn
        {
            /// <summary>
            /// The name of the accessible member.
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// The member type.
            /// </summary>
            public Type Type { get; }
            /// <summary>
            /// The function to read the value of the member.
            /// </summary>
            public Func<object, object> ValueGetter { get; }
            /// <summary>
            /// The <see cref="DataViewType"/> to which this member is convertible.
            /// </summary>
            public DataViewType DataViewType => GetDataViewType(Type);

        /// <summary>
        /// Creates the new member info instance. 
        /// </summary>
        /// <param name="name"> The member type.</param>
        /// <param name="type">The member type.</param>
        /// <param name="getter">The function to read the value of the member.</param>
        internal DataViewColumn(string name, Type type, Func<object, object> getter)
            {
                Name = name;
                Type = type;
                ValueGetter = getter;
            }

            internal DataViewColumn Instantiate(Func<string, string> memberNameFunc, Func<object, object> objectGetterFunc)
            {
                return new DataViewColumn(memberNameFunc(this.Name), this.Type, x => this.ValueGetter(objectGetterFunc(x)));
            }
        /// <summary>
        /// Gets the <see cref="DataViewType"/> that matches the provided <paramref name="rawType"/>. 
        /// </summary>
        /// <param name="rawType">The raw type.</param>
        /// <returns>The <see cref="DataViewType"/> for corresponding raw type.</returns>
        /// <exception cref="NotSupportedException">thrown if the raw type is not supported.</exception>
        private static PrimitiveDataViewType GetDataViewType(Type rawType)
        {
            if (rawType == typeof(bool))
            {
                return BooleanDataViewType.Instance;
            }
            else if (rawType == typeof(byte))
            {
                return NumberDataViewType.Byte;
            }
            else if (rawType == typeof(sbyte))
            {
                return NumberDataViewType.SByte;
            }
            else if (rawType == typeof(short))
            {
                return NumberDataViewType.Int16;
            }
            else if (rawType == typeof(ushort))
            {
                return NumberDataViewType.UInt16;
            }
            else if (rawType == typeof(int))
            {
                return NumberDataViewType.Int32;
            }
            else if (rawType == typeof(uint))
            {
                return NumberDataViewType.UInt32;
            }
            else if (rawType == typeof(long))
            {
                return NumberDataViewType.Int64;
            }
            else if (rawType == typeof(ulong))
            {
                return NumberDataViewType.UInt64;
            }
            else if (rawType == typeof(double))
            {
                return NumberDataViewType.Double;
            }
            else if (rawType == typeof(float))
            {
                return NumberDataViewType.Single;
            }
            else if (rawType == typeof(DateTime))
            {
                return DateTimeDataViewType.Instance;
            }
            else if (rawType == typeof(DateTime?))
            {
                return DateTimeDataViewType.Instance;
            }
            else if (rawType == typeof(DateTimeOffset))
            {
                return DateTimeOffsetDataViewType.Instance;
            }
            else if (rawType == typeof(TimeSpan))
            {
                return TimeSpanDataViewType.Instance;
            }
            else if (rawType == typeof(string))
            {
                return TextDataViewType.Instance;
            }
            else if (rawType.IsEnum)
            {
                return NumberDataViewType.Int32;
            }
            else // anything else is error
            {
                throw new NotSupportedException($"The type {rawType} does not have corresponding DataViewType.");
            }
        }
    }
}