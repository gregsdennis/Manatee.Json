using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json
{
	/// <summary>
	/// These extension methods cover LINQ compatibility.
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>
		/// Converts an <see cref="IEnumerable{JsonValue}"/> returned from a LINQ query back into
		/// a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="results">An <see cref="IEnumerable{JsonValue}"/></param>
		/// <returns>An equivalent <see cref="JsonArray"/></returns>
		public static JsonArray ToJson(this IEnumerable<JsonValue> results)
		{
			var json = new JsonArray();
			json.AddRange(results);
			return json;
		}
		/// <summary>
		/// Converts an <see cref="IDictionary{String, JsonValue}"/> into a <see cref="JsonObject"/>.
		/// </summary>
		/// <param name="results">An <see cref="IDictionary{String, JsonValue}"/></param>
		/// <returns>An equivalent <see cref="JsonObject"/></returns>
		public static JsonObject ToJson(this IDictionary<string, JsonValue> results)
		{
			return new JsonObject(results);
		}
		/// <summary>
		/// Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String, JsonValue}"/> returned from a
		/// LINQ query back into a <see cref="JsonObject"/>.
		/// </summary>
		/// <param name="results">An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String, JsonValue}"/></param>
		/// <returns>An equivalent <see cref="JsonObject"/></returns>
		public static JsonObject ToJson(this IEnumerable<KeyValuePair<string, JsonValue>> results)
		{
			var json = new JsonObject();
			foreach (var keyValuePair in results)
			{
				json.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return json;
		}
		/// <summary>
		/// Converts a collection of strings to a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="list">A collection of strings</param>
		/// <returns>A <see cref="JsonArray"/> containing the strings</returns>
		public static JsonValue ToJson(this IEnumerable<string> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of bools to a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="list">A collection of booleans</param>
		/// <returns>A <see cref="JsonArray"/> containing the booleans</returns>
		public static JsonValue ToJson(this IEnumerable<bool> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of <see cref="Nullable{Boolean}"/> to a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="list">A collection of <see cref="Nullable{Boolean}"/></param>
		/// <returns>A <see cref="JsonArray"/> containing the <see cref="Nullable{Boolean}"/></returns>
		public static JsonValue ToJson(this IEnumerable<bool?> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(b => b.HasValue ? new JsonValue(b.Value) : JsonValue.Null));
			return json;
		}
		/// <summary>
		/// Converts a collection of <see cref="JsonArray"/>s to a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="list">A collection of <see cref="JsonArray"/>s</param>
		/// <returns>A <see cref="JsonArray"/> containing the <see cref="JsonArray"/>s</returns>
		public static JsonValue ToJson(this IEnumerable<JsonArray> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of <see cref="JsonObject"/>s to a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="list">A collection of <see cref="JsonObject"/>s</param>
		/// <returns>A <see cref="JsonArray"/> containing the <see cref="JsonObject"/>s</returns>
		public static JsonValue ToJson(this IEnumerable<JsonObject> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of doubles to a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="list">A collection of doubles</param>
		/// <returns>A <see cref="JsonArray"/> containing the doubles</returns>
		public static JsonValue ToJson(this IEnumerable<double> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of doubles to a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="list">A collection of doubles</param>
		/// <returns>A <see cref="JsonArray"/> containing the doubles</returns>
		public static JsonValue ToJson(this IEnumerable<double?> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Serializes a collection of objects which implement <see cref="IJsonSerializable"/> to a <see cref="JsonArray"/> of equivalent JsonValues.
		/// </summary>
		/// <param name="list">A collection of equivalent <see cref="JsonValue"/>s</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>A <see cref="JsonArray"/> containing the equivalent JsonValues</returns>
		public static JsonValue ToJson<T>(this IEnumerable<T> list, JsonSerializer serializer)
			where T : IJsonSerializable
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => j == null ? JsonValue.Null : j.ToJson(serializer)));
			return json;
		}
		/// <summary>
		/// Converts an <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String, JsonValue}"/> returned from a
		/// LINQ query back into a <see cref="JsonObject"/>.
		/// </summary>
		/// <param name="results">An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{String, JsonValue}"/></param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>An equivalent <see cref="JsonObject"/></returns>
		public static JsonObject ToJson<T>(this IEnumerable<KeyValuePair<string, T>> results, JsonSerializer serializer)
			where T : IJsonSerializable
		{
			var json = new JsonObject();
			foreach (var keyValuePair in results)
			{
				json.Add(keyValuePair.Key, keyValuePair.Value == null ? JsonValue.Null : keyValuePair.Value.ToJson(serializer));
			}
			return json;
		}
		/// <summary>
		/// Deserializes a collection of <see cref="JsonValue"/>s to an <see cref="IEnumerable{T}"/> of the objects.
		/// </summary>
		/// <typeparam name="T">The type of object contained in the collection</typeparam>
		/// <param name="json">The collection of <see cref="JsonValue"/>s</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>A collection of the deserialized objects</returns>
		public static IEnumerable<T> FromJson<T>(this IEnumerable<JsonValue> json, JsonSerializer serializer)
			where T : IJsonSerializable, new()
		{
			if (json == null) throw new ArgumentNullException(nameof(json));

			foreach (var value in json)
			{
				T item = new T();
				item.FromJson(value, serializer);
				yield return item;
			}
		}
		/// <summary>
		/// Deserializes a <see cref="JsonValue"/> to its equivalent object.
		/// </summary>
		/// <typeparam name="T">The type of object</typeparam>
		/// <param name="json">The <see cref="JsonValue"/> to deserialize</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>A collection of the deserialized objects</returns>
		public static T FromJson<T>(this JsonObject json, JsonSerializer serializer)
			where T : IJsonSerializable, new()
		{
			if (json == null) return default(T);

			T obj = new T();
			obj.FromJson(json, serializer);
			return obj;
		}
	}
}
