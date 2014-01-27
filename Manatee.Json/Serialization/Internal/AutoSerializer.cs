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
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		AutoSerializer
	Purpose:		Converts objects to and from JsonValues based on available
					properties.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal class AutoSerializer : ISerializer, ITypeSerializer
	{
		public bool ShouldMaintainReferences { get { return true; } }

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			var json = new JsonObject();
			var type = typeof(T);
			if (type.IsAbstract || type.IsInterface || serializer.Options.AlwaysSerializeTypeName)
			{
				type = obj.GetType();
				json.Add(Constants.TypeKey, type.AssemblyQualifiedName);
			}
			var propertyInfoList = GetProperties(type);
			var map = SerializeValues(obj, serializer, propertyInfoList);
			ConstructJsonObject(json, map);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public JsonValue SerializeType<T>(JsonSerializer serializer)
		{
			var json = new JsonObject();
			var type = typeof(T);
			var propertyInfoList = GetTypeProperties(type);
			var map = SerializeTypeValues(serializer, propertyInfoList);
			ConstructJsonObject(json, map);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			var obj = JsonSerializationAbstractionMap.CreateInstance<T>(json, serializer.Options.Resolver);
			var type = obj.GetType();
			var propertyInfoList = GetProperties(type);
			var map = DeserializeValues(obj, json, serializer, propertyInfoList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			AssignObjectProperties(obj, map);
			return obj;
		}
		public void DeserializeType<T>(JsonValue json, JsonSerializer serializer)
		{
			var type = typeof (T);
			var propertyInfoList = GetTypeProperties(type);
			var map = DeserializeTypeValues(json, serializer, propertyInfoList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			AssignObjectProperties(null, map);
		}

		private static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			var propertyInfoList = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
									   .Where(p => p.GetSetMethod() != null)
									   .Where(p => p.GetGetMethod() != null)
									   .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
									   .ToList();
			return propertyInfoList;
		}
		private static IEnumerable<PropertyInfo> GetTypeProperties(Type type)
		{
			var propertyInfoList = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
			                           .Where(p => p.GetSetMethod() != null)
			                           .Where(p => p.GetGetMethod() != null)
			                           .Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any());
			return propertyInfoList;
		}
		private static Dictionary<PropertyInfo, JsonValue> SerializeValues<T>(T obj, JsonSerializer serializer, IEnumerable<PropertyInfo> properties)
		{
			var dict = new Dictionary<PropertyInfo, JsonValue>();
			foreach (var propertyInfo in properties)
			{
				if (propertyInfo.GetIndexParameters().Any()) continue;
				var value = propertyInfo.GetValue(obj, null);
				if (value == null) continue;
				var propType = propertyInfo.PropertyType;
				var serialize = SerializerCache.Default.GetSerializeMethod(propType);
				var json = (JsonValue)serialize.Invoke(serializer, new[] { value });
				if ((json == JsonValue.Null) && !serializer.Options.EncodeDefaultValues) continue;
				dict.Add(propertyInfo, json);
			}
			return dict;
		}
		private static Dictionary<PropertyInfo, JsonValue> SerializeTypeValues(JsonSerializer serializer, IEnumerable<PropertyInfo> properties)
		{
			var dict = new Dictionary<PropertyInfo, JsonValue>();
			foreach (var propertyInfo in properties)
			{
				var value = propertyInfo.GetValue(null, null);
				if (value == null) continue;
				var propType = propertyInfo.PropertyType;
				var serialize = SerializerCache.Default.GetSerializeMethod(propType);
				var json = (JsonValue)serialize.Invoke(serializer, new[] { value });
				if ((json == JsonValue.Null) && !serializer.Options.EncodeDefaultValues) continue;
				dict.Add(propertyInfo, json);
			}
			return dict;
		}
		private static void ConstructJsonObject(JsonObject json, Dictionary<PropertyInfo, JsonValue> propertyMap)
		{
			foreach (var propertyInfo in propertyMap.Keys)
			{
				var mapper = (JsonMapToAttribute) propertyInfo.GetCustomAttributes(typeof (JsonMapToAttribute), false).FirstOrDefault();
				string name = mapper != null ? mapper.MapToKey : propertyInfo.Name;
				json.Add(name, propertyMap[propertyInfo]);
			}
		}
		private static Dictionary<PropertyInfo, object> DeserializeValues<T>(T obj, JsonValue json, JsonSerializer serializer, IEnumerable<PropertyInfo> properties, bool ignoreCase)
		{
			var dict = new Dictionary<PropertyInfo, object>();
			foreach (var propertyInfo in properties)
			{
				string name = propertyInfo.Name;
				var mapper =
					(JsonMapToAttribute) propertyInfo.GetCustomAttributes(typeof (JsonMapToAttribute), false).FirstOrDefault();
				if (mapper != null)
					name = mapper.MapToKey;
				if (json.Object.Keys.Any(key => string.Compare(key, name, ignoreCase) == 0))
				{
					var value = json.Object[name];
					var deserialize = SerializerCache.Default.GetDeserializeMethod(propertyInfo.PropertyType);
					var valueObj = deserialize.Invoke(serializer, new object[] {value});
					if ((value.Type == JsonValueType.Object) && value.Object.ContainsKey(Constants.RefKey))
					{
						var guid = Guid.Parse(value.Object[Constants.RefKey].String);
						var pair = serializer.SerializationMap[guid];
						if (pair.DeserializationIsComplete)
							dict.Add(propertyInfo, valueObj);
						else
							pair.AddReference(propertyInfo, obj);
						value.Object.Remove(Constants.RefKey);
					}
					else
						dict.Add(propertyInfo, valueObj);
					json.Object.Remove(name);
				}
			}
			return dict;
		}
		private static Dictionary<PropertyInfo, object> DeserializeTypeValues(JsonValue json, JsonSerializer serializer, IEnumerable<PropertyInfo> properties, bool ignoreCase)
		{
			var dict = new Dictionary<PropertyInfo, object>();
			foreach (var propertyInfo in properties)
			{
				string name = propertyInfo.Name;
				var mapper =
					(JsonMapToAttribute)propertyInfo.GetCustomAttributes(typeof(JsonMapToAttribute), false).FirstOrDefault();
				if (mapper != null)
					name = mapper.MapToKey;
				if (json.Object.Keys.Any(key => string.Compare(key, name, ignoreCase) == 0))
				{
					var value = json.Object[name];
					var deserialize = SerializerCache.Default.GetDeserializeMethod(propertyInfo.PropertyType);
					var valueObj = deserialize.Invoke(serializer, new object[] { value });
					if ((value.Type == JsonValueType.Object) && value.Object.ContainsKey(Constants.RefKey))
					{
						var guid = Guid.Parse(value.Object[Constants.RefKey].String);
						var pair = serializer.SerializationMap[guid];
						if (pair.DeserializationIsComplete)
							dict.Add(propertyInfo, valueObj);
						else
							pair.AddReference(propertyInfo, null);
						value.Object.Remove(Constants.RefKey);
					}
					else
						dict.Add(propertyInfo, valueObj);
					json.Object.Remove(name);
				}
			}
			return dict;
		}
		private static void AssignObjectProperties(object obj, Dictionary<PropertyInfo, object> propertyMap)
		{
			foreach (var propertyInfo in propertyMap.Keys)
			{
				propertyInfo.SetValue(obj, propertyMap[propertyInfo], null);
			}
		}
	}
}