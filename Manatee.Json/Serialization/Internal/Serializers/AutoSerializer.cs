using System;
using System.Collections;
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
			var objectType = obj.GetType();
			bool typeKeyAdded = false;
			if (serializer.Options.TypeNameSerializationBehavior != TypeNameSerializationBehavior.Never &&
				(serializer.Options.TypeNameSerializationBehavior == TypeNameSerializationBehavior.Always ||
				 type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface ||
				 (type != objectType && serializer.Options.TypeNameSerializationBehavior != TypeNameSerializationBehavior.OnlyForAbstractions)))
			{
				typeKeyAdded = true;
				json.Add(Constants.TypeKey, objectType.AssemblyQualifiedName);
			}
			if (typeKeyAdded || !serializer.Options.OnlyExplicitProperties)
				type = obj.GetType();
			var propertyList = ReflectionCache.GetMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = _SerializeValues(obj, serializer, propertyList);
			_ConstructJsonObject(json, map, serializer.Options);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public JsonValue SerializeType<T>(JsonSerializer serializer)
		{
			var json = new JsonObject();
			var type = typeof (T);
			var propertyList = ReflectionCache.GetTypeMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = _SerializeTypeValues(serializer, propertyList);
			_ConstructJsonObject(json, map, serializer.Options);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var obj = serializer.AbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			var type = obj.GetType();
			var propertyList = ReflectionCache.GetMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = _DeserializeValues(obj, json, serializer, propertyList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			_AssignObjectProperties(obj, map);
			return obj;
		}
		public void DeserializeType<T>(JsonValue json, JsonSerializer serializer)
		{
			var type = typeof (T);
			var propertyList = ReflectionCache.GetTypeMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = _DeserializeTypeValues(json, serializer, propertyList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			_AssignObjectProperties(null, map);
		}
		private static Dictionary<SerializationInfo, JsonValue> _SerializeValues<T>(T obj, JsonSerializer serializer, IEnumerable<SerializationInfo> properties)
		{
			var dict = new Dictionary<SerializationInfo, JsonValue>();
			foreach (var property in properties)
			{
				object value;
				Type type;
				if (property.MemberInfo is PropertyInfo propertyInfo)
				{
					if (propertyInfo.GetIndexParameters().Any()) continue;
					value = propertyInfo.GetValue(obj, null);
					if (value == null && !serializer.Options.EncodeDefaultValues) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo)property.MemberInfo;
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
					_AddSample(type, json.Array, serializer);
				}

				dict.Add(property, json);
			}
			return dict;
		}
		private static Dictionary<SerializationInfo, JsonValue> _SerializeTypeValues(JsonSerializer serializer, IEnumerable<SerializationInfo> properties)
		{
			var dict = new Dictionary<SerializationInfo, JsonValue>();
			foreach (var memberInfo in properties)
			{
				object value;
				Type type;
				if (memberInfo.MemberInfo is PropertyInfo propertyInfo)
				{
					if (propertyInfo.GetIndexParameters().Any()) continue;
					value = propertyInfo.GetValue(null, null);
					if (value == null) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo)memberInfo.MemberInfo;
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
	    private static void _ConstructJsonObject(JsonObject json, Dictionary<SerializationInfo, JsonValue> memberMap, JsonSerializerOptions options)
	    {
	        foreach (var memberInfo in memberMap.Keys)
	        {
	            var name = memberInfo.SerializationName;
	            if (memberInfo.ShouldTransform)
	                name = options.SerializationNameTransform(name);
	            json.Add(name, memberMap[memberInfo]);
	        }
	    }
	    private static Dictionary<SerializationInfo, object> _DeserializeValues<T>(T obj, JsonValue json, JsonSerializer serializer, IEnumerable<SerializationInfo> members, bool ignoreCase)
		{
			var dict = new Dictionary<SerializationInfo, object>();
			foreach (var memberInfo in members)
			{
				var name = memberInfo.SerializationName;
				if (memberInfo.ShouldTransform)
					name = serializer.Options.DeserializationNameTransform(name);

				if (_TryGetKeyValue(json, name, ignoreCase, out var value))
				{
					MethodInfo deserialize;
					if (memberInfo.MemberInfo is PropertyInfo info)
						deserialize = SerializerCache.GetDeserializeMethod(info.PropertyType);
					else
						deserialize = SerializerCache.GetDeserializeMethod(((FieldInfo)memberInfo.MemberInfo).FieldType);
					var valueObj = deserialize.Invoke(serializer, new object[] { value });
					if (value.Type == JsonValueType.Object && value.Object.ContainsKey(Constants.RefKey))
					{
						var guid = new Guid(value.Object[Constants.RefKey].String);
						var pair = serializer.SerializationMap[guid];
						if (pair.DeserializationIsComplete)
							dict.Add(memberInfo, valueObj);
						else
						{
							if (memberInfo.MemberInfo is PropertyInfo property)
								pair.AddReference(property, obj);
							else
								pair.AddReference((FieldInfo)memberInfo.MemberInfo, obj);
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
		private static Dictionary<SerializationInfo, object> _DeserializeTypeValues(JsonValue json, JsonSerializer serializer, IEnumerable<SerializationInfo> members, bool ignoreCase)
		{
			var dict = new Dictionary<SerializationInfo, object>();
			foreach (var memberInfo in members)
			{
				var name = memberInfo.SerializationName;

				if (_TryGetKeyValue(json, name, ignoreCase, out var value))
				{
					MethodInfo deserialize;
					if (memberInfo.MemberInfo is PropertyInfo)
						deserialize = SerializerCache.GetDeserializeMethod(((PropertyInfo) memberInfo.MemberInfo).PropertyType);
					else
						deserialize = SerializerCache.GetDeserializeMethod(((FieldInfo) memberInfo.MemberInfo).FieldType);
					var valueObj = deserialize.Invoke(serializer, new object[] {value});
					if (value.Type == JsonValueType.Object && value.Object.ContainsKey(Constants.RefKey))
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
		private static bool _TryGetKeyValue(JsonValue json, string name, bool ignoreCase, out JsonValue value)
		{
			var key = name;
			value = null;

			// PERF: attempt to read the name AS-IS before searching
			if (!json.Object.TryGetValue(name, out value))
			{
				key = null;

				// PERF: if we didn't have an exact match, we must be 'ignore case'.
				//       Otherwise the key does not exist and we should not go
				//       looking for the key.
				if (ignoreCase)
				{
					foreach (var kvp in json.Object)
					{
						if (string.Compare(kvp.Key, name, StringComparison.OrdinalIgnoreCase) != 0) continue;

						key = kvp.Key;
						value = kvp.Value;
						break;
					}
				}
			}

			return key != null;
		}
		private static void _AssignObjectProperties(object obj, Dictionary<SerializationInfo, object> memberMap)
		{
			foreach (var entry in memberMap)
			{
				var memberInfo = entry.Key;
				if (memberInfo.MemberInfo is PropertyInfo info)
				{
					if (info.CanWrite)
						info.SetValue(obj, entry.Value, null);
					else if (typeof(IList).GetTypeInfo().IsAssignableFrom(info.PropertyType.GetTypeInfo()))
					{
						var list = (IList) info.GetValue(obj);
						foreach (var value in (IList)entry.Value)
						{
							list.Add(value);
						}
					}
					else if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(info.PropertyType.GetTypeInfo()))
					{
						var dictionary = (IDictionary) info.GetValue(obj);
						foreach (DictionaryEntry kvp in (IDictionary)entry.Value)
						{
							dictionary.Add(kvp.Key, kvp.Value);
						}
					}
				}
				else
					((FieldInfo) memberInfo.MemberInfo).SetValue(obj, entry.Value);
			}
		}
		private static void _AddSample(Type type, JsonArray json, JsonSerializer serializer)
		{
			var elementType = _GetElementType(type);
			var buildMethod = TemplateGenerator.GetBuildMethod(elementType);
			var value = buildMethod.Invoke(null, new object[] {serializer.Options});
			var serialize = SerializerCache.GetSerializeMethod(elementType);
			json.Add((JsonValue) serialize.Invoke(serializer, new[] {value}));
		}
		private static Type _GetElementType(Type collectionType)
		{
			if (collectionType.IsArray)
				return collectionType.GetElementType();
			if (collectionType.GetTypeInfo().IsGenericType && collectionType.GetGenericTypeDefinition().InheritsFrom(typeof(IEnumerable<>)))
				return collectionType.GetTypeArguments().First();
			return typeof (object);
		}
	}
}