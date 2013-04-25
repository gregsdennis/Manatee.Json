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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Attributes;
using Manatee.Json.Helpers;
using Manatee.Json.Enumerations;
using Manatee.Json.Exceptions;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Serializes and deserializes objects and types to and from JSON structures.
	/// </summary>
	public class JsonSerializer
	{
		internal const string TypeKey = "#Type";
		internal const string ValueKey = "#Value";
		internal const string RefKey = "#Ref";
		internal const string DefKey = "#Define";

		private static readonly ISerializerCache SerializerCache = new SerializerCache();
		private static readonly IPrimitiveMapper PrimitiveMapper = new PrimitiveMapper(new ObjectCaster());

		private readonly ISerializerReferenceCache _referenceCache = new SerializerReferenceCache();
		private int _callCount;
		private JsonSerializerOptions _options;

		/// <summary>
		/// Gets or sets a set of options for the serializer.
		/// </summary>
		public JsonSerializerOptions Options
		{
			get { return _options ?? (_options = JsonSerializerOptions.Default); }
			set { _options = value ?? JsonSerializerOptions.Default; }
		}

		#region Public Methods
		/// <summary>
		/// Serializes an object to a JSON structure.
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize.</typeparam>
		/// <param name="obj">The object to serialize.</param>
		/// <returns>The JSON representation of the object.</returns>
		public JsonValue Serialize<T>(T obj)
		{
			_callCount++;
			var json = ManagedSerialize(obj);
			if (--_callCount == 0)
			{
				_referenceCache.ReconcileJsonReferences();
				_referenceCache.Clear();
			}
			return json;
		}
		/// <summary>
		/// Serializes the public static properties of a type to a JSON structure.
		/// </summary>
		/// <typeparam name="T">The type to serialize.</typeparam>
		/// <returns>The JSON representation of the type.</returns>
		public JsonValue SerializeType<T>()
		{
			_callCount++;
			var json = new JsonObject();
			var propertyInfoList = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public)
											.Where(p => p.GetSetMethod() != null)
											.Where(p => p.GetGetMethod() != null)
											.Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any());
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
					var serialize = SerializerCache.GetSerializer(type);
					json.Add(propertyInfo.Name, (JsonValue) serialize.Invoke(this, new[] {value}));
				}
			}
			if (--_callCount == 0)
			{
				_referenceCache.ReconcileObjectReferences();
				_referenceCache.Clear();
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
			_callCount++;
			var obj = ManagedDeserialize<T>(json);
			if (--_callCount == 0)
			{
				_referenceCache.ReconcileObjectReferences();
				_referenceCache.Clear();
			}
			return obj;
		}
		/// <summary>
		/// Deserializes a JSON structure to the public static properties of a type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize.</typeparam>
		/// <param name="json">The JSON representation of the type.</param>
		public void DeserializeType<T>(JsonValue json)
		{
			_callCount++;
			if (json == JsonValue.Null)
				return;
			var propertyInfoList = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public)
											.Where(p => p.GetSetMethod() != null)
											.Where(p => p.GetGetMethod() != null)
											.Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any());
			foreach (var propertyInfo in propertyInfoList)
			{
				if (json.Object.ContainsKey(propertyInfo.Name))
				{
					var deserialize = SerializerCache.GetDeserializer(propertyInfo.PropertyType);
					propertyInfo.SetValue(null, deserialize.Invoke(this, new object[] {json.Object[propertyInfo.Name]}), null);
					json.Object.Remove(propertyInfo.Name);
				}
			}
			if (--_callCount == 0)
			{
				_referenceCache.ReconcileObjectReferences();
				_referenceCache.Clear();
			}
			if (json.Object.Count > 0)
				throw new TypeDoesNotContainPropertyException(typeof(T), json);
		}
		#endregion

		#region Support Methods
		private JsonValue ManagedSerialize<T>(T obj)
		{
			var match = _referenceCache.FindRecord(obj);
			if (match != null)
			{
				match.IsReferenced = true;
				return new JsonObject {{RefKey, match.ReferenceId.ToString()}};
			}
			match = new SerializerReferenceRecord {Object = obj};
			_referenceCache.Add(match);
			if (typeof(IJsonCompatible).IsAssignableFrom(typeof(T)))
			{
				match.Json = ((IJsonCompatible)obj).ToJson();
			}
			else
			{
				if (EqualsDefaultValue(obj) && !Options.EncodeDefaultValues) return JsonValue.Null;
				match.Json = PrimitiveMapper.MapToJson(obj);
				if (match.Json == JsonValue.Null)
				{
					JsonValue json;
					match.Json = this.TryEncode(obj, out json) ? json : AutoSerializeObject(obj);
				}
			}
			return match.Json;
		}
		private T ManagedDeserialize<T>(JsonValue json)
		{
			SerializerReferenceRecord match = null;
			switch (json.Type)
			{
				case JsonValueType.Object:
					if (json.Object.ContainsKey(DefKey))
						_referenceCache.Add(match = new SerializerReferenceRecord(json.Object[DefKey].String));
					break;
				case JsonValueType.Array:
					var def = json.Array.FirstOrDefault(jv => (jv.Type == JsonValueType.Object) &&
					                                          (jv.Object.Count == 1) &&
					                                          jv.Object.ContainsKey(DefKey));
					if (def != null)
						_referenceCache.Add(match = new SerializerReferenceRecord(def.Object[DefKey].String));
					break;
			}
			T obj;
			var mappedType = JsonSerializationAbstractionMap.GetMap(typeof (T));
			if (typeof(IJsonCompatible).IsAssignableFrom(mappedType))
			{
				obj = JsonSerializationAbstractionMap.CreateInstance<T>(json);
				((IJsonCompatible)obj).FromJson(json);
			}
			else if (json == JsonValue.Null)
			{
				obj = default(T);
			}
			else if (PrimitiveMapper.IsPrimitive(typeof(T)))
			{
				obj = PrimitiveMapper.MapFromJson<T>(json);
			}
			else if (!this.TryDecode(json, out obj))
			{
				obj = AutoDeserializeObject<T>(json);
			}
			if (match != null)
				match.Object = obj;
			return obj;
		}
		private static bool EqualsDefaultValue<T>(T value)
		{
			return EqualityComparer<T>.Default.Equals(value, default(T));
		}
		private JsonValue AutoSerializeObject<T>(T obj)
		{
			var json = new JsonObject();
			var type = typeof (T);
			if (type.IsAbstract || type.IsInterface)
			{
				type = obj.GetType();
				json.Add(TypeKey, type.AssemblyQualifiedName);
			}
			var propertyInfoList = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
									   .Where(p => p.GetSetMethod() != null)
									   .Where(p => p.GetGetMethod() != null)
									   .Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any());
			foreach (var propertyInfo in propertyInfoList)
			{
				var value = propertyInfo.GetValue(obj, null);
				if (value == null) continue;
				var propType = propertyInfo.PropertyType;
				var serialize = SerializerCache.GetSerializer(propType);
				var jsonProp = (JsonValue) serialize.Invoke(this, new[] {value});
				if ((jsonProp == JsonValue.Null) && !Options.EncodeDefaultValues) continue;
				var mapper = (JsonMapToAttribute)propertyInfo.GetCustomAttributes(typeof(JsonMapToAttribute), false).FirstOrDefault();
				string name = mapper != null ? mapper.MapToKey : propertyInfo.Name;
				json.Add(name, jsonProp);
			}
			return json.Count == 0 ? JsonValue.Null : json;
		}
		private T AutoDeserializeObject<T>(JsonValue json)
		{
			var obj = JsonSerializationAbstractionMap.CreateInstance<T>(json);
			var type = obj.GetType();
			var propertyInfoList = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
									   .Where(p => p.GetSetMethod() != null)
									   .Where(p => p.GetGetMethod() != null)
									   .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any());
			foreach (var propertyInfo in propertyInfoList)
			{
				string name = propertyInfo.Name;
				var mapper = (JsonMapToAttribute)propertyInfo.GetCustomAttributes(typeof(JsonMapToAttribute), false).FirstOrDefault();
				if (mapper != null)
					name = mapper.MapToKey;
				if (json.Object.ContainsKey(name))
				{
					var value = json.Object[name];
					if ((value.Type == JsonValueType.Object) && (value.Object.ContainsKey(RefKey)))
					{
						_referenceCache.AddReference(value.Object[RefKey].String, obj, propertyInfo);
					}
					else
					{
						var deserialize = SerializerCache.GetDeserializer(propertyInfo.PropertyType);
						propertyInfo.SetValue(obj, deserialize.Invoke(this, new object[] {json.Object[name]}), null);
					}
					json.Object.Remove(name);
				}
			}
			if ((json.Object.Count > 0) && (Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			return obj;
		}
		#endregion
	}
}
