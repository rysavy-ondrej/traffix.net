using MessagePack;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Traffix.Core.Flows;

namespace Traffix.Processors
{
    /// <summary>
    /// This static class is used to register member info objects for user data types - we call it records
    /// as they are flatten view on the existing possibly complex and nested classes and structures.
    /// </summary>
    internal static class RecordTypeRegister
    {         
        /// <summary>
        /// Contains cached member information objects.
        /// </summary>
        private static readonly Dictionary<Type, ICollection<RecordMemberInfo>> _registeredRecordTypes = new Dictionary<Type, ICollection<RecordMemberInfo>>();

        /// <summary>                                                   
        /// Gets the member information collection for the given type.
        /// The information is cached by this class thus subsequent calling does not analyze the type again but 
        /// uses the existing information.
        /// <para>
        /// The collection can be used to access the members in an object of the corresponding type.
        /// </para>
        /// </summary>
        /// <param name="type">The type of conversation records.</param>
        /// <returns>A collection of member information objects for the given <paramref name="type"/>.</returns>
        internal static ICollection<RecordMemberInfo> GetRecordInfo(Type type)
        {
            if (!_registeredRecordTypes.TryGetValue(type, out var collection))
            {
                collection = ExtractRecordMembers(type);
                _registeredRecordTypes.Add(type, collection);
            }
            return collection;
        }

        /// <summary>
        /// Gets the collection of member information objects for the given <paramref name="type"/>.
        /// <para/> 
        /// This method enumerates the member in nested classed and structures. It provives a collection of all members found.
        /// The full names of member are given as paths in the root class hierarchy.
        /// </summary>
        /// <param name="type">The type for which to get columns.</param>
        /// <param name="prefix">The path prefix to use for constructing full names of members.</param>
        /// <param name="pathDelimiter">The path delimiter used in member path names.</param>
        /// <returns>A collection of member information objects that each consists of  member path,  member type and  accessor function.</returns>
        public static ICollection<RecordMemberInfo> ExtractRecordMembers(Type type, string prefix = null, string pathDelimiter = "_")
        {
            var members = new List<RecordMemberInfo>();

            /// Combines path with the end element and creates full path string using the given delimiter. 
            string MemberPathCombine(IEnumerable<string> path, string v)
            {
                return String.Join(pathDelimiter, path.Append(v));
            }

            /// Tests if the provided type is a basic type, which can be one of the number type:
            /// Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single
            /// String type
            /// or DateTime, DatTimeOffset and TimeSpan.
            static bool IsBasicValueType(Type type)
            {
                return type.IsEnum
                    || type.IsPrimitive  // 
                    || type == typeof(String)
                    || type == typeof(DateTime)
                    || type == typeof(DateTimeOffset)
                    || type == typeof(TimeSpan);
            }

            /// Gets all members (public properties and fields) that has MessagePack.KeyAttribute.
            static IEnumerable<MemberInfo> GetMembers(Type type)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<KeyAttribute>() != null).Cast<MemberInfo>();
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetCustomAttribute<KeyAttribute>() != null).Cast<MemberInfo>();
                return fields.Concat(props);
            }

            /// Adds properties and fields to the members collection.
            void AddMembers(IEnumerable<string> path, Type typ, Func<object, object> funcAccessor)
            {
                if (typ == typeof(FlowKey))
                {
                    members.Add(new RecordMemberInfo(MemberPathCombine(path, nameof(FlowKey.ProtocolType)), typeof(System.Net.Sockets.ProtocolType), obj => (funcAccessor(obj) as FlowKey).ProtocolType));

                    members.Add(new RecordMemberInfo(MemberPathCombine(path, nameof(FlowKey.SourceIpAddress)), typeof(string), obj => (funcAccessor(obj) as FlowKey).SourceIpAddress.ToString()));

                    members.Add(new RecordMemberInfo(MemberPathCombine(path, nameof(FlowKey.SourcePort)), typeof(ushort), obj => (funcAccessor(obj) as FlowKey).SourcePort));

                    members.Add(new RecordMemberInfo(MemberPathCombine(path, nameof(FlowKey.DestinationIpAddress)), typeof(string), obj => (funcAccessor(obj) as FlowKey).DestinationIpAddress.ToString()));

                    members.Add(new RecordMemberInfo(MemberPathCombine(path, nameof(FlowKey.DestinationPort)), typeof(ushort), obj => (funcAccessor(obj) as FlowKey).DestinationPort));
                }
                else
                {
                    foreach (var info in GetMembers(typ))
                    {
                        var fullName = path.Append(info.Name);
                        var memberName = string.Join(pathDelimiter, fullName);

                        switch (info)
                        {
                            case FieldInfo fieldInfo:
                                Func<object, object> fieldAccessor = x => fieldInfo.GetValue(funcAccessor(x));
                                if (IsBasicValueType(fieldInfo.FieldType))
                                {
                                    members.Add(new RecordMemberInfo(memberName, fieldInfo.FieldType, fieldAccessor));
                                }
                                else
                                {
                                    AddMembers(fullName, fieldInfo.FieldType, fieldAccessor);
                                }
                                break;
                            case PropertyInfo propInfo:
                                Func<object, object> propAccessor = x => propInfo.GetValue(funcAccessor(x));
                                if (IsBasicValueType(propInfo.PropertyType))
                                {
                                    members.Add(new RecordMemberInfo(memberName, propInfo.PropertyType, propAccessor));
                                }
                                else
                                {
                                    AddMembers(fullName, propInfo.PropertyType, propAccessor);
                                }
                                break;
                        }
                    }
                }
            }
            AddMembers(prefix != null ? new[] { prefix } : Array.Empty<string>(), type, x => x);
            return members;
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
            else // anything else is converted to string...
            {
                throw new NotSupportedException($"The type {rawType} does not have corresponding DataViewType.");
            }
        }
        
        /// <summary>
        /// Provides a member information for a single public field and property.
        /// <para/> 
        /// It contains name and type of the member as well as <see cref="ValueGetter"/> function.
        /// </summary>
        public class RecordMemberInfo
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
            internal RecordMemberInfo(string name, Type type, Func<object, object> getter)
            {
                Name = name;
                Type = type;
                ValueGetter = getter;
            }
        }
    }
}