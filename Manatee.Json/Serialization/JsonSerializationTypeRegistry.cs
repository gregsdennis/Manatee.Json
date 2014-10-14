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
 
	File Name:		JsonSerializationTypeRegistry.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonSerializationTypeRegistry
	Purpose:		Manages methods for serializing object types which do not
					implement IJsonCompatible and cannot be automatically serialized.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Manages methods for serializing object types which do not implement <see cref="IJsonSerializable"/> and
	/// cannot be automatically serialized.
	/// </summary>
	public static class JsonSerializationTypeRegistry
	{
		#region Operational Items
		/// <summary>
		/// Declares the required signature for a serializer method.
		/// </summary>
		/// <typeparam name="T">The type which the method serializes.</typeparam>
		/// <param name="input">The object to be serialized.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the object.</returns>
		public delegate JsonValue ToJsonDelegate<in T>(T input, JsonSerializer serializer);
		/// <summary>
		/// Declares the required signature for a deserializer method.
		/// </summary>
		/// <typeparam name="T">The type which the method deserializes.</typeparam>
		/// <param name="json">The JSON representation of the object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The deserialized object.</returns>
		public delegate T FromJsonDelegate<out T>(JsonValue json, JsonSerializer serializer);

		private static readonly Dictionary<Type, Delegate> ToJsonConverters;
		private static readonly Dictionary<Type, Delegate> FromJsonConverters;
		private static readonly object LockHolder = new object();

		static JsonSerializationTypeRegistry()
		{
			ToJsonConverters = new Dictionary<Type, Delegate>();
			FromJsonConverters = new Dictionary<Type, Delegate>();
			RegisterLocalTypes();
		}

		/// <summary>
		/// Gets whether a given type has been entered into the registry.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <returns>True if an entry exists for the type; otherwise false.</returns>
		public static bool IsRegistered<T>()
		{
			return IsRegistered(typeof(T));
		}
		/// <summary>
		/// Gets whether a given type has been entered into the registry.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>True if an entry exists for the type; otherwise false.</returns>
		public static bool IsRegistered(Type type)
		{
			return ToJsonConverters.ContainsKey(type);
		}

		internal static void Encode<T>(this JsonSerializer serializer, T obj, out JsonValue json)
		{
			var converter = GetToJsonConverter<T>();
			if (converter == null)
			{
				json = null;
				return;
			}
			lock (LockHolder)
			{
				json = converter(obj, serializer);
			}
		}
		internal static void Decode<T>(this JsonSerializer serializer, JsonValue json, out T obj)
		{
			var converter = GetFromJsonConverter<T>();
			if (converter == null)
			{
				obj = default(T);
				return;
			}
			lock (LockHolder)
			{
				obj = converter(json, serializer);
			}
		}

		private static void RegisterLocalTypes()
		{
			RegisterType(EncodeDateTime, DecodeDateTime);
			RegisterType(EncodeTimeSpan, DecodeTimeSpan);
			RegisterType(EncodeGuid, DecodeGuid);
		}
		private static ToJsonDelegate<T> GetToJsonConverter<T>()
		{
			var type = typeof (T);
			return ToJsonConverters.ContainsKey(type) ? (ToJsonDelegate<T>)ToJsonConverters[type] : null;
		}
		private static FromJsonDelegate<T> GetFromJsonConverter<T>()
		{
			var type = typeof(T);
			return FromJsonConverters.ContainsKey(type) ? (FromJsonDelegate<T>)FromJsonConverters[type] : null;
		}
		#endregion

		#region Registration
		/// <summary>
		/// Registers an encode/decode method pair for a specific type.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="toJson">The serializer method.</param>
		/// <param name="fromJson">The deserializer method.</param>
		/// <exception cref="TypeRegistrationException">Thrown if either, but not both, <paramref name="toJson"/>
		/// or <paramref name="fromJson"/> is null.</exception>
		public static void RegisterType<T>(ToJsonDelegate<T> toJson, FromJsonDelegate<T> fromJson)
		{
			if (((toJson == null) && (fromJson != null)) ||
				((toJson != null) && (fromJson == null)))
				throw new TypeRegistrationException(typeof (T));
			var type = typeof (T);
			ToJsonConverters[type] = toJson;
			FromJsonConverters[type] = fromJson;
		}
		/// <summary>
		/// Registers an encode/decode method pair for a nullable type.
		/// </summary>
		/// <typeparam name="T">The underlying type of the nullable.</typeparam>
		/// <remarks>To register a nullable int, the call should be RegisterNullableType{int}()</remarks>
		public static void RegisterNullableType<T>() where T : struct
		{
			var type = typeof(T);
			var nullableType = typeof(T?);
			var toJson = typeof(JsonSerializationTypeRegistry).GetMethod("EncodeNullable").MakeGenericMethod(type);
			var fromJson = typeof(JsonSerializationTypeRegistry).GetMethod("DecodeNullable").MakeGenericMethod(type);
			ToJsonConverters[nullableType] = Delegate.CreateDelegate(typeof(ToJsonDelegate<T?>), toJson);
			FromJsonConverters[nullableType] = Delegate.CreateDelegate(typeof(FromJsonDelegate<T?>), fromJson);
		}
		/// <summary>
		/// Registers an encode/decode method pair for a typed array.
		/// </summary>
		/// <typeparam name="T">The underlying type of the array.</typeparam>
		/// <remarks>To register a array of ints, the call should be RegisterArrayType{int}()</remarks>
		public static void RegisterArrayType<T>()
		{
			var type = typeof(T);
			var arrayType = typeof(T[]);
			var toJson = typeof(JsonSerializationTypeRegistry).GetMethod("EncodeGenericArray").MakeGenericMethod(type);
			var fromJson = typeof(JsonSerializationTypeRegistry).GetMethod("DecodeGenericArray").MakeGenericMethod(type);
			ToJsonConverters[arrayType] = Delegate.CreateDelegate(typeof(ToJsonDelegate<T[]>), toJson);
			FromJsonConverters[arrayType] = Delegate.CreateDelegate(typeof(FromJsonDelegate<T[]>), fromJson);
		}
		/// <summary>
		/// Registers an encode/decode method pair for a typed list.
		/// </summary>
		/// <typeparam name="T">The underlying type of the list.</typeparam>
		/// <remarks>To register a list of ints, the call should be RegisterListType{int}()</remarks>
		public static void RegisterListType<T>()
		{
			var type = typeof(T);
			var listType = typeof(List<T>);
			var toJson = typeof(JsonSerializationTypeRegistry).GetMethod("EncodeGenericList").MakeGenericMethod(type);
			var fromJson = typeof(JsonSerializationTypeRegistry).GetMethod("DecodeGenericList").MakeGenericMethod(type);
			ToJsonConverters[listType] = Delegate.CreateDelegate(typeof(ToJsonDelegate<List<T>>), toJson);
			FromJsonConverters[listType] = Delegate.CreateDelegate(typeof(FromJsonDelegate<List<T>>), fromJson);
		}
		/// <summary>
		/// Registers an encode/decode method pair for a typed dictionary.
		/// </summary>
		/// <typeparam name="TKey">The underlying type used as the key for the dictionary.</typeparam>
		/// <typeparam name="TValue">The underlying type used as the value for the dictionary.</typeparam>
		/// <remarks>To register a dictionary of ints keyed by strings, the call should be RegisterDictionaryType{string, int}()</remarks>
		public static void RegisterDictionaryType<TKey, TValue>()
		{
			var keyType = typeof(TKey);
			var valueType = typeof (TValue);
			var dictType = typeof(Dictionary<TKey, TValue>);
			var toJson = typeof(JsonSerializationTypeRegistry).GetMethod("EncodeGenericDictionary").MakeGenericMethod(keyType, valueType);
			var fromJson = typeof(JsonSerializationTypeRegistry).GetMethod("DecodeGenericDictionary").MakeGenericMethod(keyType, valueType);
			ToJsonConverters[dictType] = Delegate.CreateDelegate(typeof(ToJsonDelegate<Dictionary<TKey, TValue>>), toJson);
			FromJsonConverters[dictType] = Delegate.CreateDelegate(typeof(FromJsonDelegate<Dictionary<TKey, TValue>>), fromJson);
		}
		/// <summary>
		/// Registers an encode/decode method pair for a typed queue.
		/// </summary>
		/// <typeparam name="T">The underlying type of the queue.</typeparam>
		/// <remarks>To register a queue of ints, the call should be RegisterQueueType{int}()</remarks>
		public static void RegisterQueueType<T>()
		{
			var type = typeof(T);
			var queueType = typeof(Queue<T>);
			var toJson = typeof(JsonSerializationTypeRegistry).GetMethod("EncodeGenericQueue").MakeGenericMethod(type);
			var fromJson = typeof(JsonSerializationTypeRegistry).GetMethod("DecodeGenericQueue").MakeGenericMethod(type);
			ToJsonConverters[queueType] = Delegate.CreateDelegate(typeof(ToJsonDelegate<Queue<T>>), toJson);
			FromJsonConverters[queueType] = Delegate.CreateDelegate(typeof(FromJsonDelegate<Queue<T>>), fromJson);
		}
		/// <summary>
		/// Registers an encode/decode method pair for a typed stack.
		/// </summary>
		/// <typeparam name="T">The underlying type of the stack.</typeparam>
		/// <remarks>To register a stack of ints, the call should be RegisterStackType{int}()</remarks>
		public static void RegisterStackType<T>()
		{
			var type = typeof(T);
			var stackType = typeof(Stack<T>);
			var toJson = typeof(JsonSerializationTypeRegistry).GetMethod("EncodeGenericStack").MakeGenericMethod(type);
			var fromJson = typeof(JsonSerializationTypeRegistry).GetMethod("DecodeGenericStack").MakeGenericMethod(type);
			ToJsonConverters[stackType] = Delegate.CreateDelegate(typeof(ToJsonDelegate<Stack<T>>), toJson);
			FromJsonConverters[stackType] = Delegate.CreateDelegate(typeof(FromJsonDelegate<Stack<T>>), fromJson);
		}
		#endregion

		#region Specified Types
		#region DateTime
		/// <summary>
		/// Encodes a <see cref="DateTime"/> object to its JSON representation.
		/// </summary>
		/// <param name="dt">A <see cref="DateTime"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="DateTime"/>.</returns>
		public static JsonValue EncodeDateTime(DateTime dt, JsonSerializer serializer)
		{
			if (serializer.Options == null)
				return dt.ToString();
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return dt.ToString("s");
				case DateTimeSerializationFormat.JavaConstructor:
					return string.Format("/Date({0})/", dt.Ticks/TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return dt.Ticks / TimeSpan.TicksPerMillisecond;
				case DateTimeSerializationFormat.Custom:
					return dt.ToString(serializer.Options.CustomDateTimeSerializationFormat);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		/// <summary>
		/// Decodes a <see cref="DateTime"/> object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON representation of a <see cref="DateTime"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="DateTime"/> object.</returns>
		public static DateTime DecodeDateTime(JsonValue json, JsonSerializer serializer)
		{
			if (serializer.Options == null)
				return DateTime.Parse(json.String);
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return DateTime.Parse(json.String);
				case DateTimeSerializationFormat.JavaConstructor:
					return new DateTime(long.Parse(json.String.Substring(6, json.String.Length - 8))*TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return new DateTime((long) json.Number*TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Custom:
					return DateTime.ParseExact(json.String, serializer.Options.CustomDateTimeSerializationFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		#endregion
		#region TimeSpan
		/// <summary>
		/// Encodes a <see cref="TimeSpan"/> object to its JSON representation.
		/// </summary>
		/// <param name="ts">A <see cref="TimeSpan"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="TimeSpan"/>.</returns>
		public static JsonValue EncodeTimeSpan(TimeSpan ts, JsonSerializer serializer)
		{
			return ts.ToString();
		}
		/// <summary>
		/// Decodes a <see cref="TimeSpan"/> object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON representation of a <see cref="TimeSpan"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="TimeSpan"/> object.</returns>
		public static TimeSpan DecodeTimeSpan(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? TimeSpan.Parse(json.String) : default(TimeSpan);
		}
		#endregion
		#region Guid
		/// <summary>
		/// Encodes a <see cref="Guid"/> object to its JSON representation.
		/// </summary>
		/// <param name="guid">A <see cref="Guid"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="Guid"/>.</returns>
		public static JsonValue EncodeGuid(Guid guid, JsonSerializer serializer)
		{
			return guid.ToString();
		}
		/// <summary>
		/// Decodes a <see cref="Guid"/> object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON representation of a <see cref="Guid"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="Guid"/> object.</returns>
		public static Guid DecodeGuid(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? new Guid(json.String) : default(Guid);
		}
		#endregion
		#endregion

		#region Generic Types
		#region Nullable<T>
		/// <summary>
		/// Encodes a <see cref="Nullable{T}"/> to its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the nullable value.</typeparam>
		/// <param name="nullable">The <see cref="Nullable{T}"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="Nullable{T}"/>.</returns>
		public static JsonValue EncodeNullable<T>(T? nullable, JsonSerializer serializer) where T : struct
		{
			return nullable.HasValue ? serializer.Serialize(nullable.Value) : JsonValue.Null;
		}
		/// <summary>
		/// Decodes a <see cref="Nullable{T}"/> object from its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the nullable value.</typeparam>
		/// <param name="json">A JSON representation of a <see cref="Nullable{T}"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="Nullable{T}"/> object.</returns>
		public static T? DecodeNullable<T>(JsonValue json, JsonSerializer serializer) where T : struct
		{
			if (json == JsonValue.Null) return null;
			T? nullable = serializer.Deserialize<T>(json);
			return nullable;
		}
		#endregion
		#region Array<T>
		/// <summary>
		/// Encodes a <see cref="T"/>[] to its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the array.</typeparam>
		/// <param name="list">The <see cref="T"/>[] object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="T"/>[].</returns>
		public static JsonValue EncodeGenericArray<T>(T[] list, JsonSerializer serializer)
		{
			var array = new JsonArray();
			array.AddRange(list.Select(serializer.Serialize));
			return array;
		}
		/// <summary>
		/// Decodes a <see cref="T"/>[] object from its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the array.</typeparam>
		/// <param name="json">A JSON representation of a <see cref="T"/>[].</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="T"/>[] object.</returns>
		public static T[] DecodeGenericArray<T>(JsonValue json, JsonSerializer serializer)
		{
			var list = new List<T>();
			list.AddRange(json.Array.Select(serializer.Deserialize<T>));
			return list.ToArray();
		}
		#endregion
		#region List<T>
		/// <summary>
		/// Encodes a <see cref="List{T}"/> to its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the list.</typeparam>
		/// <param name="list">The <see cref="List{T}"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="List{T}"/>.</returns>
		public static JsonValue EncodeGenericList<T>(List<T> list, JsonSerializer serializer)
		{
			var array = new JsonArray();
			array.AddRange(list.Select(serializer.Serialize));
			return array;
		}
		/// <summary>
		/// Decodes a <see cref="List{T}"/> object from its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the list.</typeparam>
		/// <param name="json">A JSON representation of a <see cref="List{T}"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="List{T}"/> object.</returns>
		public static List<T> DecodeGenericList<T>(JsonValue json, JsonSerializer serializer)
		{
			var list = new List<T>();
			list.AddRange(json.Array.Select(serializer.Deserialize<T>));
			return list;
		}
		#endregion
		#region Dictionary<TKey, TValue>
		/// <summary>
		/// Encodes a <see cref="Dictionary{TKey, TValue}"/> to its JSON representation.
		/// </summary>
		/// <typeparam name="TKey">The underlying type used as the key for the dictionary.</typeparam>
		/// <typeparam name="TValue">The underlying type used as the value for the dictionary.</typeparam>
		/// <param name="dict">The <see cref="Dictionary{TKey, TValue}"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="Dictionary{TKey, TValue}"/>.</returns>
		public static JsonValue EncodeGenericDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, JsonSerializer serializer)
		{
			var array = new JsonArray();
			array.AddRange(dict.Select(item => (JsonValue) (new JsonObject
				{
					{"Key", serializer.Serialize(item.Key)},
					{"Value", serializer.Serialize(item.Value)}
				})));
			return array;
		}
		/// <summary>
		/// Decodes a <see cref="Dictionary{TKey, TValue}"/> object from its JSON representation.
		/// </summary>
		/// <typeparam name="TKey">The underlying type used as the key for the dictionary.</typeparam>
		/// <typeparam name="TValue">The underlying type used as the value for the dictionary.</typeparam>
		/// <param name="json">A JSON representation of a <see cref="Dictionary{TKey, TValue}"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="Dictionary{TKey, TValue}"/> object.</returns>
		public static Dictionary<TKey, TValue> DecodeGenericDictionary<TKey, TValue>(JsonValue json, JsonSerializer serializer)
		{
			return json.Array.ToDictionary(jv => serializer.Deserialize<TKey>(jv.Object["Key"]),
										   jv => serializer.Deserialize<TValue>(jv.Object["Value"]));
		}

		#endregion
		#region Queue<T>
		/// <summary>
		/// Encodes a <see cref="Queue{T}"/> to its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the queue.</typeparam>
		/// <param name="queue">The <see cref="Queue{T}"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="Queue{T}"/>.</returns>
		public static JsonValue EncodeGenericQueue<T>(Queue<T> queue, JsonSerializer serializer)
		{
			var array = new JsonArray();
			for (int i = 0; i < queue.Count; i++)
			{
				array.Add(serializer.Serialize(queue.ElementAt(i)));
			}
			return array;
		}
		/// <summary>
		/// Decodes a <see cref="Queue{T}"/> object from its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the queue.</typeparam>
		/// <param name="json">A JSON representation of a <see cref="Queue{T}"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="Queue{T}"/> object.</returns>
		public static Queue<T> DecodeGenericQueue<T>(JsonValue json, JsonSerializer serializer)
		{
			var queue = new Queue<T>();
			for (int i = 0; i < json.Array.Count; i++)
			{
				queue.Enqueue(serializer.Deserialize<T>(json.Array[i]));
			}
			return queue;
		}
		#endregion
		#region Stack<T>
		/// <summary>
		/// Encodes a <see cref="Stack{T}"/> to its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the stack.</typeparam>
		/// <param name="stack">The <see cref="Stack{T}"/> object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the <see cref="Stack{T}"/>.</returns>
		public static JsonValue EncodeGenericStack<T>(Stack<T> stack, JsonSerializer serializer)
		{
			var array = new JsonArray();
			for (int i = 0; i < stack.Count; i++)
			{
				array.Add(serializer.Serialize(stack.ElementAt(i)));
			}
			return array;
		}
		/// <summary>
		/// Decodes a <see cref="Stack{T}"/> object from its JSON representation.
		/// </summary>
		/// <typeparam name="T">The underlying type of the stack.</typeparam>
		/// <param name="json">A JSON representation of a <see cref="Stack{T}"/>.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The <see cref="Stack{T}"/> object.</returns>
		public static Stack<T> DecodeGenericStack<T>(JsonValue json, JsonSerializer serializer)
		{
			var stack = new Stack<T>();
			for (int i = 0; i < json.Array.Count; i++)
			{
				stack.Push(serializer.Deserialize<T>(json.Array[i]));
			}
			return stack;
		}
		#endregion
		#endregion
	}
}
