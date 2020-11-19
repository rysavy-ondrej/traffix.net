using Markdig.Syntax.Inlines;
using MessagePack;
using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Traffix.Core.Flows;

namespace IcsMonitor
{
    /// <summary>
    /// Implements various extension method for transformin conversation records 
    /// to be usable as input for ML.NET.
    /// </summary>
    public static class ConversationRecordExtensions
    {
        /// <summary>
        /// Gets the dictionary that represents a flatten version of the current record.
        /// <para>
        /// It can be used to flat the conversation record and to create dynamic object from it.
        /// </para>
        /// </summary>
        /// <returns>A dictionary of fields and values for the passed conversation record.</returns>
        public static Dictionary<string, object> ToDictionary<T>(this ConversationRecord<T> self)
        {
            var dictionary = GetMembersInfo(self.GetType()).ToDictionary(x => x.Path, y => y.Accessor(self));
            return dictionary;
        }

        /// <summary>
        /// Converts an enumerable of conversation records to <see cref="DataFrame"/>. The <see cref="DataFrame"/>
        /// is a high performance memory store for data sets. 
        /// <para>
        /// The <see cref="DataFrame"/> implements <see cref="Microsoft.ML.IDataView"/> interface 
        /// which is consumed by ML.NET pipelines.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of conversation record.</typeparam>
        /// <param name="records">The enumerable of records.</param>
        /// <returns>The new <see cref="DataFrame"/> object representing the source records.</returns>
        public static DataFrame ToDataFrame<T>(this IEnumerable<ConversationRecord<T>> records)
        {
            var recordList = records.ToList();
            var columns = new List<DataFrameColumn>();
            var members = GetMembersInfo(typeof(ConversationRecord<T>));
            foreach (var member in members)
            {
                columns.Add(GetColumn(member.Path, member.MemberType, recordList.Select(member.Accessor)));
            }
            var df = new DataFrame(columns);
            return df;
        }


        /// <summary>
        /// Gets a single column of <see cref="DataFrame"/> including all associated values.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="columnType">The column type.</param>
        /// <param name="values">The collection of values that must be of the <paramref name="columnType"/>.</param>
        /// <returns>The new <see cref="DataFrameColumn"/>for the parameters specified.</returns>
        private static DataFrameColumn GetColumn(string name, Type columnType, IEnumerable<object> values)
        {
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
                return new PrimitiveDataFrameColumn<IntPtr>(name, values.Cast<IntPtr>());
            }
            else if (columnType == typeof(UIntPtr))
            {
                return new PrimitiveDataFrameColumn<UIntPtr>(name, values.Cast<UIntPtr>());
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
                return new PrimitiveDataFrameColumn<DateTime>(name, values.Cast<DateTime>());
            }
            else if (columnType == typeof(string))
            {
                return new StringDataFrameColumn(name, values.Cast<string>());
            }
            else
            {
                return new StringDataFrameColumn(name, values.Select(x => x.ToString()));
            }
        }

        /// <summary>
        /// Gets the member information collection for the given type.
        /// <para>
        /// The collection can be used to access the members in an object of the corresponding type.
        /// </para>
        /// </summary>
        /// <param name="type">The type of conversation records.</param>
        /// <returns>A collection of member information objects for the given <paramref name="type"/>.</returns>
        private static ICollection<(string Path, Type MemberType, Func<object, object> Accessor)> GetMembersInfo(Type type)
        {
            if (!_membersInfoCache.TryGetValue(type, out var collection))
            {
                collection = CreateMembersInfo(type);
                _membersInfoCache.Add(type, collection);
            }
            return collection;
        }

        /// <summary>
        /// Contains cached member information objects.
        /// </summary>
        private static Dictionary<Type, ICollection<(string Path, Type MemberType, Func<object, object>)>> _membersInfoCache = new Dictionary<Type, ICollection<(string Path, Type MemberType, Func<object, object>)>>();

        /// <summary>
        /// Gets the collection of member information objects for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which to get columns.</param>
        /// <returns>A collection of member information objects that each consists of  member path,  member type and  accessor function.</returns>
        private static ICollection<(string Path, Type MemberType, Func<object, object>)> CreateMembersInfo(Type type)
        {
            var members = new List<(string Path, Type MemberType, Func<object, object> Accessor)>();

            /// Combines path with the end element and creates full path string using the given delimiter. 
            static string MemberPathCombine(IEnumerable<string> path, string v, string delimiter = "_")
            {
                return String.Join(delimiter, path.Append(v));
            }

            /// Tests if the provided type canbe considered to be a basic type, which are:
            /// Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single
            /// String and DateTime.
            static bool IsBasicValueType(Type type)
            {
                return type.IsEnum
                    || type.IsPrimitive  // 
                    || type == typeof(String)
                    || type == typeof(DateTime);
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
                    members.Add((MemberPathCombine(path, nameof(FlowKey.ProtocolType)), typeof(System.Net.Sockets.ProtocolType), obj => (funcAccessor(obj) as FlowKey).ProtocolType));

                    members.Add((MemberPathCombine(path, nameof(FlowKey.SourceIpAddress)), typeof(string), obj => (funcAccessor(obj) as FlowKey).SourceIpAddress.ToString()));

                    members.Add((MemberPathCombine(path, nameof(FlowKey.SourcePort)), typeof(ushort), obj => (funcAccessor(obj) as FlowKey).SourcePort));

                    members.Add((MemberPathCombine(path, nameof(FlowKey.DestinationIpAddress)), typeof(string), obj => (funcAccessor(obj) as FlowKey).DestinationIpAddress.ToString()));

                    members.Add((MemberPathCombine(path, nameof(FlowKey.DestinationPort)), typeof(ushort), obj => (funcAccessor(obj) as FlowKey).DestinationPort));
                }
                else                                            
                {
                    foreach (var info in GetMembers(typ))
                    {
                        var fullName = path.Append(info.Name);
                        var memberName = String.Join("_", fullName);

                        switch (info)
                        {
                            case FieldInfo fieldInfo:
                                Func<object, object> fieldAccessor = x => fieldInfo.GetValue(funcAccessor(x));
                                if (IsBasicValueType(fieldInfo.FieldType))
                                {
                                    members.Add((memberName, fieldInfo.FieldType, fieldAccessor));
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
                                    members.Add((memberName, propInfo.PropertyType, propAccessor));
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
            AddMembers(Array.Empty<string>(), type, x => x);
            return members;
        }
    }
}