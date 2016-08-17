/***************************************************************************************

	Copyright 2016 Greg Dennis

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
		public bool ShouldMaintainReferences => true;

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
			var propertyList = ReflectionCache.GetMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = SerializeValues(obj, serializer, propertyList);
			ConstructJsonObject(json, map);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public JsonValue SerializeType<T>(JsonSerializer serializer)
		{
			var json = new JsonObject();
			var type = typeof (T);
			var propertyList = ReflectionCache.GetTypeMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = SerializeTypeValues(serializer, propertyList);
			ConstructJsonObject(json, map);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var obj = JsonSerializationAbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			var type = obj.GetType();
			var propertyList = ReflectionCache.GetMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = DeserializeValues(obj, json, serializer, propertyList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			AssignObjectProperties(obj, map);
			return obj;
		}
		public void DeserializeType<T>(JsonValue json, JsonSerializer serializer)
		{
			var type = typeof (T);
			var propertyList = ReflectionCache.GetTypeMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = DeserializeTypeValues(json, serializer, propertyList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			AssignObjectProperties(null, map);
		}
		private static Dictionary<SerializationInfo, JsonValue> SerializeValues<T>(T obj, JsonSerializer serializer, IEnumerable<SerializationInfo> properties)
		{
			var dict = new Dictionary<SerializationInfo, JsonValue>();
			foreach (var property in properties)
			{
				object value;
				Type type;
				var propertyInfo = property.MemberInfo as PropertyInfo;
				if (propertyInfo != null)
				{
					if (propertyInfo.GetIndexParameters().Any()) continue;
					value = propertyInfo.GetValue(obj, null);
					if (value == null && !serializer.Options.EncodeDefaultValues) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo) property.MemberInfo;
					value = fieldInfo.GetValue(obj);
					if (value == null && !serializer.Options.EncodeDefaultValues) continue;
					type = fieldInfo.FieldType;
				}
				var json = JsonValue.Null;
				if (value != null)
				{
					var serialize = SerializerCache.GetSerializeMethod(type);
					json = (JsonValue) serialize.Invoke(serializer, new[] {value});
				}
				if ((json == JsonValue.Null) && !serializer.Options.EncodeDefaultValues) continue;
				if (serializer.Options.IncludeContentSample && json.Type == JsonValueType.Array)
				{
					AddSample(type, json.Array, serializer);
				}

				dict.Add(property, json);
			}
			return dict;
		}
		private static Dictionary<SerializationInfo, JsonValue> SerializeTypeValues(JsonSerializer serializer, IEnumerable<SerializationInfo> properties)
		{
			var dict = new Dictionary<SerializationInfo, JsonValue>();
			foreach (var memberInfo in properties)
			{
				object value;
				Type type;
				var propertyInfo = memberInfo.MemberInfo as PropertyInfo;
				if (propertyInfo != null)
				{
					if (propertyInfo.GetIndexParameters().Any()) continue;
					value = propertyInfo.GetValue(null, null);
					if (value == null) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo) memberInfo.MemberInfo;
					value = fieldInfo.GetValue(null);
					if (value == null) continue;
					type = fieldInfo.FieldType;
				}
				var serialize = SerializerCache.GetSerializeMethod(type);
				var json = (JsonValue) serialize.Invoke(serializer, new[] {value});
				if ((json == JsonValue.Null) && !serializer.Options.EncodeDefaultValues) continue;
				dict.Add(memberInfo, json);
			}
			return dict;
		}
		private static void ConstructJsonObject(JsonObject json, Dictionary<SerializationInfo, JsonValue> memberMap)
		{
			foreach (var memberInfo in memberMap.Keys)
			{
				json.Add(memberInfo.SerializationName, memberMap[memberInfo]);
			}
		}
		private static Dictionary<SerializationInfo, object> DeserializeValues<T>(T obj, JsonValue json, JsonSerializer serializer, IEnumerable<SerializationInfo> members, bool ignoreCase)
		{
			var dict = new Dictionary<SerializationInfo, object>();
			foreach (var memberInfo in members)
			{
				var name = memberInfo.SerializationName;
				var kvp = json.Object.FirstOrDefault(pair => string.Compare(pair.Key, name, ignoreCase) == 0);
				if (kvp.Key != null)
				{
					var value = kvp.Value;
					MethodInfo deserialize;
					if (memberInfo.MemberInfo is PropertyInfo)
						deserialize = SerializerCache.GetDeserializeMethod(((PropertyInfo) memberInfo.MemberInfo).PropertyType);
					else
						deserialize = SerializerCache.GetDeserializeMethod(((FieldInfo) memberInfo.MemberInfo).FieldType);
					var valueObj = deserialize.Invoke(serializer, new object[] {value});
					if ((value.Type == JsonValueType.Object) && value.Object.ContainsKey(Constants.RefKey))
					{
						var guid = new Guid(value.Object[Constants.RefKey].String);
						var pair = serializer.SerializationMap[guid];
						if (pair.DeserializationIsComplete)
							dict.Add(memberInfo, valueObj);
						else
						{
							if (memberInfo.MemberInfo is PropertyInfo)
								pair.AddReference((PropertyInfo) memberInfo.MemberInfo, obj);
							else
								pair.AddReference((FieldInfo) memberInfo.MemberInfo, obj);
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
		private static Dictionary<SerializationInfo, object> DeserializeTypeValues(JsonValue json, JsonSerializer serializer, IEnumerable<SerializationInfo> members, bool ignoreCase)
		{
			var dict = new Dictionary<SerializationInfo, object>();
			foreach (var memberInfo in members)
			{
				var name = memberInfo.SerializationName;
				var kvp = json.Object.FirstOrDefault(pair => string.Compare(pair.Key, name, ignoreCase) == 0);
				if (kvp.Key != null)
				{
					var value = kvp.Value;
					MethodInfo deserialize;
					if (memberInfo.MemberInfo is PropertyInfo)
						deserialize = SerializerCache.GetDeserializeMethod(((PropertyInfo) memberInfo.MemberInfo).PropertyType);
					else
						deserialize = SerializerCache.GetDeserializeMethod(((FieldInfo) memberInfo.MemberInfo).FieldType);
					var valueObj = deserialize.Invoke(serializer, new object[] {value});
					if ((value.Type == JsonValueType.Object) && value.Object.ContainsKey(Constants.RefKey))
					{
						var guid = new Guid(value.Object[Constants.RefKey].String);
						var pair = serializer.SerializationMap[guid];
						if (pair.DeserializationIsComplete)
							dict.Add(memberInfo, valueObj);
						else
						{
							if (memberInfo.MemberInfo is PropertyInfo)
								pair.AddReference((PropertyInfo) memberInfo.MemberInfo, null);
							else
								pair.AddReference((FieldInfo) memberInfo.MemberInfo, null);
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
		private static void AssignObjectProperties(object obj, Dictionary<SerializationInfo, object> memberMap)
		{
			foreach (var memberInfo in memberMap.Keys)
			{
				if (memberInfo.MemberInfo is PropertyInfo)
					((PropertyInfo) memberInfo.MemberInfo).SetValue(obj, memberMap[memberInfo], null);
				else
					((FieldInfo) memberInfo.MemberInfo).SetValue(obj, memberMap[memberInfo]);
			}
		}
		private static void AddSample(Type type, JsonArray json, JsonSerializer serializer)
		{
			var elementType = GetElementType(type);
			var buildMethod = TemplateGenerator.GetBuildMethod(elementType);
			var value = buildMethod.Invoke(null, new object[] {serializer.Options});
			var serialize = SerializerCache.GetSerializeMethod(elementType);
			json.Add((JsonValue) serialize.Invoke(serializer, new[] {value}));
		}
		private static Type GetElementType(Type collectionType)
		{
			if (collectionType.IsArray)
				return collectionType.GetElementType();
			if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition().InheritsFrom(typeof (IEnumerable<>)))
				return collectionType.GetGenericArguments().First();
			return typeof (object);
		}
	}
}