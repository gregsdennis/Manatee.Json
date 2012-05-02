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
 
	File Name:		JsonSerializer.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonSerializer
	Purpose:		Serializes and deserializes objects and types to and from
					JSON structures.

***************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Manatee.Json.Serialization.Attributes;
using Manatee.Json.Serialization.Cache;
using Manatee.Json.Serialization.Enumerations;
using Manatee.Json.Serialization.Exceptions;
using Manatee.Json.Serialization.Helpers;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Serializes and deserializes objects and types to and from JSON structures.
	/// </summary>
	public class JsonSerializer
	{
		private const string TypeKey = "#Type";
		private const string ValueKey = "#Value";

		public JsonSerializerOptions Options { get; set; }

		#region Public Methods
		/// <summary>
		/// Serializes an object to a JSON structure.
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize.</typeparam>
		/// <param name="obj">The object to serialize.</param>
		/// <returns>The JSON representation of the object.</returns>
		public JsonValue Serialize<T>(T obj)
		{
			VerifyOptions();
			if (typeof(IJsonCompatible).IsAssignableFrom(typeof(T)))
				return ((IJsonCompatible) obj).ToJson();
			if (EqualsDefaultValue(obj) && !Options.EncodeDefaultValues) return JsonValue.Null;
			var tryPrimitive = PrimitiveMapper.MapToJson(obj);
			if (tryPrimitive != JsonValue.Null)
				return tryPrimitive;
			var converter = JsonSerializationTypeRegistry.GetToJsonConverter<T>();
			return converter == null ? AutoSerializeObject(obj) : converter(obj);
		}
		/// <summary>
		/// Serializes the public static properties of a type to a JSON structure.
		/// </summary>
		/// <typeparam name="T">The type to serialize.</typeparam>
		/// <returns>The JSON representation of the type.</returns>
		public JsonValue SerializeType<T>()
		{
			VerifyOptions();
			var json = new JsonObject();
			var propertyInfoList = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public)
											 .Where(p => p.GetSetMethod() != null)
											 .Where(p => p.GetGetMethod() != null)
											 .Where(p => p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Count() == 0);
			foreach (var propertyInfo in propertyInfoList)
			{
				var value = propertyInfo.GetValue(null, null);
				if (value == null)
				{
					json.Add(propertyInfo.Name, JsonValue.Null);
				}
				else
				{
					var type = propertyInfo.PropertyType.IsAbstract || propertyInfo.PropertyType.IsInterface
								? value.GetType()
								: propertyInfo.PropertyType;
					var serialize = SerializerCache.Instance.GetSerializer(type);
					json.Add(propertyInfo.Name, (JsonValue)serialize.Invoke(this, new[] { value }));
				}
			}
			return json.Count == 0 ? JsonValue.Null : json;
		}
		/// <summary>
		/// Deserializes a JSON structure to an object of the appropriate type.
		/// </summary>
		/// <typeparam name="T">The type of the object that the JSON structure represents.</typeparam>
		/// <param name="json">The JSON representation of the object.</param>
		/// <returns>The deserialized object.</returns>
		public T Deserialize<T>(JsonValue json)
		{
			VerifyOptions();
			if (typeof(IJsonCompatible).IsAssignableFrom(typeof(T)))
			{
				var obj = (T)Activator.CreateInstance(typeof (T));
				((IJsonCompatible)obj).FromJson(json);
				return obj;
			}
			if (json == JsonValue.Null)
				return default(T);
			if (PrimitiveMapper.IsPrimitive(typeof(T)))
				return PrimitiveMapper.MapFromJson<T>(json);
			var converter = JsonSerializationTypeRegistry.GetFromJsonConverter<T>();
			return converter == null ? AutoDeserializeObject<T>(json) : converter(json);
		}
		/// <summary>
		/// Deserializes a JSON structure to the public static properties of a type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize.</typeparam>
		/// <param name="json">The JSON representation of the type.</param>
		public void DeserializeType<T>(JsonValue json)
		{
			VerifyOptions();
			if (json == JsonValue.Null)
				return;
			var propertyInfoList = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public)
											 .Where(p => p.GetSetMethod() != null)
											 .Where(p => p.GetGetMethod() != null)
											 .Where(p => p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Count() == 0);
			foreach (var propertyInfo in propertyInfoList)
			{
				if (json.Object.ContainsKey(propertyInfo.Name))
				{
					var deserialize = SerializerCache.Instance.GetDeserializer(propertyInfo.PropertyType);
					propertyInfo.SetValue(null, deserialize.Invoke(this, new[] { json.Object[propertyInfo.Name] }), null);
					json.Object.Remove(propertyInfo.Name);
				}
			}
			if (json.Object.Count > 0)
				throw new TypeDoesNotContainPropertyException(typeof(T), json);
		}
		#endregion

		#region Support Methods
		private static bool EqualsDefaultValue<T>(T value)
		{
			return EqualityComparer<T>.Default.Equals(value, default(T));
		}
		private void VerifyOptions()
		{
			if (Options == null)
				Options = JsonSerializerOptions.Default;
		}
		private JsonValue AutoSerializeObject<T>(T obj)
		{
			var json = new JsonObject();
			var propertyInfoList = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
											.Where(p => p.GetSetMethod() != null)
											.Where(p => p.GetGetMethod() != null)
											.Where(p => p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Count() == 0);
			foreach (var propertyInfo in propertyInfoList)
			{
				var value = propertyInfo.GetValue(obj, null);
				if (value == null) continue;
				var type = propertyInfo.PropertyType.IsAbstract || propertyInfo.PropertyType.IsInterface
					        ? value.GetType()
					        : propertyInfo.PropertyType;
				var serialize = SerializerCache.Instance.GetSerializer(type);
				var jsonProp = (JsonValue) serialize.Invoke(this, new[] {value});
				if ((jsonProp == JsonValue.Null) && !Options.EncodeDefaultValues) continue;
				json.Add(propertyInfo.Name,
				         type == propertyInfo.PropertyType
				         	? jsonProp
				         	: new JsonObject {{TypeKey, type.AssemblyQualifiedName}, {ValueKey, jsonProp}});
			}
			return json.Count == 0 ? JsonValue.Null : json;
		}
		private T AutoDeserializeObject<T>(JsonValue json)
		{
			var propertyInfoList = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
											.Where(p => p.GetSetMethod() != null)
											.Where(p => p.GetGetMethod() != null)
											.Where(p => p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Count() == 0);
			var obj = (T)Activator.CreateInstance(typeof(T));
			foreach (var propertyInfo in propertyInfoList)
			{
				if (json.Object.ContainsKey(propertyInfo.Name))
				{
					var value = json.Object[propertyInfo.Name];
					if (value.Type == JsonValueType.Object)
					{
						var instanceType = Type.GetType(value.Object[TypeKey].String);
						var instanceJson = value.Object[ValueKey];
						var deserialize = SerializerCache.Instance.GetDeserializer(instanceType);
						propertyInfo.SetValue(obj, deserialize.Invoke(this, new[] { instanceJson }), null);
					}
					else
					{
						var deserialize = SerializerCache.Instance.GetDeserializer(propertyInfo.PropertyType);
						propertyInfo.SetValue(obj, deserialize.Invoke(this, new[] { json.Object[propertyInfo.Name] }), null);
					}
					json.Object.Remove(propertyInfo.Name);
				}
			}
			if ((json.Object.Count > 0) && (Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(typeof (T), json);
			return obj;
		}
		#endregion
	}
}
