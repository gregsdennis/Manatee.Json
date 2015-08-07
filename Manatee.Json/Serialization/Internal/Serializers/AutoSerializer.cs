/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		AutoSerializer.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		AutoSerializer
	Purpose:		Converts objects to and from JsonValues based on available
					properties.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class AutoSerializer : ISerializer, ITypeSerializer
	{
		public bool ShouldMaintainReferences { get { return true; } }

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var json = new JsonObject();
			var type = typeof (T);
			if ((serializer.Options.TypeNameSerializationBehavior != TypeNameSerializationBehavior.Never) &&
				(type.IsAbstract || type.IsInterface || (serializer.Options.TypeNameSerializationBehavior == TypeNameSerializationBehavior.Always)))
			{
				type = obj.GetType();
				json.Add(Constants.TypeKey, type.AssemblyQualifiedName);
			}
			var propertyInfoList = GetMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = SerializeValues(obj, serializer, propertyInfoList);
			ConstructJsonObject(json, map);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public JsonValue SerializeType<T>(JsonSerializer serializer)
		{
			var json = new JsonObject();
			var type = typeof (T);
			var propertyInfoList = GetTypeMembers(type, serializer.Options.AutoSerializeFields);
			var map = SerializeTypeValues(serializer, propertyInfoList);
			ConstructJsonObject(json, map);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var obj = JsonSerializationAbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			var type = obj.GetType();
			var propertyInfoList = GetMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = DeserializeValues(obj, json, serializer, propertyInfoList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			AssignObjectProperties(obj, map);
			return obj;
		}
		public void DeserializeType<T>(JsonValue json, JsonSerializer serializer)
		{
			var type = typeof (T);
			var propertyInfoList = GetTypeMembers(type, serializer.Options.AutoSerializeFields);
			var map = DeserializeTypeValues(json, serializer, propertyInfoList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			AssignObjectProperties(null, map);
		}

		private static IEnumerable<MemberInfo> GetMembers(Type type, PropertySelectionStrategy propertyTypes, bool includeFields)
		{
			var members = GetProperties(type, propertyTypes);
			if (includeFields)
				members = members.Concat(GetFields(type));
			return members;
		}
		private static IEnumerable<MemberInfo> GetProperties(Type type, PropertySelectionStrategy propertyTypes)
		{
			var properties = new List<PropertyInfo>();
			if ((propertyTypes & PropertySelectionStrategy.ReadWriteOnly) != 0)
				properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
										.Where(p => p.GetSetMethod() != null)
										.Where(p => p.GetGetMethod() != null)
										.Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any()));
			if ((propertyTypes & PropertySelectionStrategy.ReadOnly) != 0)
				properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
										.Where(p => p.GetSetMethod() == null)
										.Where(p => p.GetGetMethod() != null)
										.Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any()));
#if NET35 || NET35C
			return properties.Cast<MemberInfo>();
#elif NET4 || NET4C || NET45
			return properties;
#endif
		}
		private static IEnumerable<MemberInfo> GetFields(Type type)
		{
			var fields = new List<FieldInfo>();
			fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public)
								.Where(p => !p.IsInitOnly)
								.Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any()));
#if NET35 || NET35C
			return fields.Cast<MemberInfo>();
#elif NET4 || NET4C || NET45
			return fields;
#endif
		}
		private static IEnumerable<MemberInfo> GetTypeMembers(Type type, bool includeFields)
		{
			var members = GetTypeProperties(type);
			if (includeFields)
				members = members.Concat(GetTypeFields(type));
			return members;
		}
		private static IEnumerable<MemberInfo> GetTypeProperties(Type type)
		{
			var propertyInfoList = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
									   .Where(p => p.GetSetMethod() != null)
									   .Where(p => p.GetGetMethod() != null)
									   .Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any());
#if NET35 || NET35C
			return propertyInfoList.Cast<MemberInfo>();
#elif NET4 || NET4C || NET45
			return propertyInfoList;
#endif
		}
		private static IEnumerable<MemberInfo> GetTypeFields(Type type)
		{
			var fieldInfoList = type.GetFields(BindingFlags.Static | BindingFlags.Public)
									   .Where(p => !p.IsInitOnly)
									   .Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any());
#if NET35 || NET35C
			return fieldInfoList.Cast<MemberInfo>();
#elif NET4 || NET4C || NET45
			return fieldInfoList;
#endif
		}
		private static Dictionary<MemberInfo, JsonValue> SerializeValues<T>(T obj, JsonSerializer serializer, IEnumerable<MemberInfo> properties)
		{
			var dict = new Dictionary<MemberInfo, JsonValue>();
			foreach (var memberInfo in properties)
			{
				object value;
				Type type;
				var propertyInfo = memberInfo as PropertyInfo;
				if (propertyInfo != null)
				{
					if (propertyInfo.GetIndexParameters().Any()) continue;
					value = propertyInfo.GetValue(obj, null);
					if (value == null && !serializer.Options.EncodeDefaultValues) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo) memberInfo;
					value = fieldInfo.GetValue(obj);
					if (value == null && !serializer.Options.EncodeDefaultValues) continue;
					type = fieldInfo.FieldType;
				}
				var json = JsonValue.Null;
				if (value != null)
				{
					var serialize = SerializerCache.Default.GetSerializeMethod(type);
					json = (JsonValue) serialize.Invoke(serializer, new[] {value});
				}
				if ((json == JsonValue.Null) && !serializer.Options.EncodeDefaultValues) continue;
				if (serializer.Options.IncludeContentSample && json.Type == JsonValueType.Array)
				{
					AddSample(type, json.Array, serializer);
				}

				dict.Add(memberInfo, json);
			}
			return dict;
		}
		private static Dictionary<MemberInfo, JsonValue> SerializeTypeValues(JsonSerializer serializer, IEnumerable<MemberInfo> properties)
		{
			var dict = new Dictionary<MemberInfo, JsonValue>();
			foreach (var memberInfo in properties)
			{
				object value;
				Type type;
				var propertyInfo = memberInfo as PropertyInfo;
				if (propertyInfo != null)
				{
					if (propertyInfo.GetIndexParameters().Any()) continue;
					value = propertyInfo.GetValue(null, null);
					if (value == null) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo) memberInfo;
					value = fieldInfo.GetValue(null);
					if (value == null) continue;
					type = fieldInfo.FieldType;
				}
				var serialize = SerializerCache.Default.GetSerializeMethod(type);
				var json = (JsonValue) serialize.Invoke(serializer, new[] {value});
				if ((json == JsonValue.Null) && !serializer.Options.EncodeDefaultValues) continue;
				dict.Add(memberInfo, json);
			}
			return dict;
		}
		private static void ConstructJsonObject(JsonObject json, Dictionary<MemberInfo, JsonValue> memberMap)
		{
			foreach (var memberInfo in memberMap.Keys)
			{
				var mapper = (JsonMapToAttribute) memberInfo.GetCustomAttributes(typeof (JsonMapToAttribute), false).FirstOrDefault();
				string name = mapper != null ? mapper.MapToKey : memberInfo.Name;
				json.Add(name, memberMap[memberInfo]);
			}
		}
		private static Dictionary<MemberInfo, object> DeserializeValues<T>(T obj, JsonValue json, JsonSerializer serializer, IEnumerable<MemberInfo> members, bool ignoreCase)
		{
			var dict = new Dictionary<MemberInfo, object>();
			foreach (var memberInfo in members)
			{
				string name = memberInfo.Name;
				var mapper = (JsonMapToAttribute) memberInfo.GetCustomAttributes(typeof (JsonMapToAttribute), false).FirstOrDefault();
				if (mapper != null)
					name = mapper.MapToKey;
				var kvp = json.Object.FirstOrDefault(pair => string.Compare(pair.Key, name, ignoreCase) == 0);
				if (kvp.Key != null)
				{
					var value = kvp.Value;
					MethodInfo deserialize;
					if (memberInfo is PropertyInfo)
						deserialize = SerializerCache.Default.GetDeserializeMethod(((PropertyInfo) memberInfo).PropertyType);
					else
						deserialize = SerializerCache.Default.GetDeserializeMethod(((FieldInfo) memberInfo).FieldType);
					var valueObj = deserialize.Invoke(serializer, new object[] {value});
					if ((value.Type == JsonValueType.Object) && value.Object.ContainsKey(Constants.RefKey))
					{
						var guid = new Guid(value.Object[Constants.RefKey].String);
						var pair = serializer.SerializationMap[guid];
						if (pair.DeserializationIsComplete)
							dict.Add(memberInfo, valueObj);
						else
						{
							if (memberInfo is PropertyInfo)
								pair.AddReference((PropertyInfo) memberInfo, obj);
							else
								pair.AddReference((FieldInfo) memberInfo, obj);
						}
						value.Object.Remove(Constants.RefKey);
					}
					else
						dict.Add(memberInfo, valueObj);
					json.Object.Remove(name);
				}
			}
			return dict;
		}
		private static Dictionary<MemberInfo, object> DeserializeTypeValues(JsonValue json, JsonSerializer serializer, IEnumerable<MemberInfo> members, bool ignoreCase)
		{
			var dict = new Dictionary<MemberInfo, object>();
			foreach (var memberInfo in members)
			{
				string name = memberInfo.Name;
				var mapper = (JsonMapToAttribute) memberInfo.GetCustomAttributes(typeof (JsonMapToAttribute), false).FirstOrDefault();
				if (mapper != null)
					name = mapper.MapToKey;
				var kvp = json.Object.FirstOrDefault(pair => string.Compare(pair.Key, name, ignoreCase) == 0);
				if (kvp.Key != null)
				{
					var value = kvp.Value;
					MethodInfo deserialize;
					if (memberInfo is PropertyInfo)
						deserialize = SerializerCache.Default.GetDeserializeMethod(((PropertyInfo) memberInfo).PropertyType);
					else
						deserialize = SerializerCache.Default.GetDeserializeMethod(((FieldInfo) memberInfo).FieldType);
					var valueObj = deserialize.Invoke(serializer, new object[] {value});
					if ((value.Type == JsonValueType.Object) && value.Object.ContainsKey(Constants.RefKey))
					{
						var guid = new Guid(value.Object[Constants.RefKey].String);
						var pair = serializer.SerializationMap[guid];
						if (pair.DeserializationIsComplete)
							dict.Add(memberInfo, valueObj);
						else
						{
							if (memberInfo is PropertyInfo)
								pair.AddReference((PropertyInfo) memberInfo, null);
							else
								pair.AddReference((FieldInfo) memberInfo, null);
						}
						value.Object.Remove(Constants.RefKey);
					}
					else
						dict.Add(memberInfo, valueObj);
					json.Object.Remove(name);
				}
			}
			return dict;
		}
		private static void AssignObjectProperties(object obj, Dictionary<MemberInfo, object> memberMap)
		{
			foreach (var memberInfo in memberMap.Keys)
			{
				if (memberInfo is PropertyInfo)
					((PropertyInfo) memberInfo).SetValue(obj, memberMap[memberInfo], null);
				else
					((FieldInfo) memberInfo).SetValue(obj, memberMap[memberInfo]);
			}
		}
		private static void AddSample(Type type, JsonArray json, JsonSerializer serializer)
		{
			var elementType = GetElementType(type);
			var buildMethod = TemplateGenerator.GetBuildMethod(elementType);
			var value = buildMethod.Invoke(null, new object[] { serializer.Options });
			var serialize = SerializerCache.Default.GetSerializeMethod(elementType);
			json.Add((JsonValue)serialize.Invoke(serializer, new[] { value }));
		}
		private static Type GetElementType(Type collectionType)
		{
			if (collectionType.IsArray)
				return collectionType.GetElementType();
			if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition().InheritsFrom(typeof(IEnumerable<>)))
				return collectionType.GetGenericArguments().First();
			return typeof(object);
		}
	}
}