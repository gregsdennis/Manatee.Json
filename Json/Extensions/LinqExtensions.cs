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
 
	File Name:		LinqExtensions.cs
	Namespace:		Manatee.Json.Extensions
	Class Name:		LinqExtensions
	Purpose:		LINQ for Manatee.Json

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Extensions
{
	/// <summary>
	/// These extension methods cover LINQ compatibility.
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>
		/// Converts an IEnumerable&lt;JsonValue&gt; returned from a LINQ query back into
		/// a JsonArray.
		/// </summary>
		/// <param name="results">An IEnumerable&lt;JsonValue&gt;</param>
		/// <returns>An equivalent JsonArray</returns>
		public static JsonArray ToJson(this IEnumerable<JsonValue> results)
		{
			var json = new JsonArray();
			json.AddRange(results);
			return json;
		}

		/// <summary>
		/// Converts an IEnumerable&lt;KeyValuePair&lt;string, JsonValue&gt;&gt; returned from a
		/// LINQ query back into a JsonObject.
		/// </summary>
		/// <param name="results">An IEnumerable&lt;KeyValuePair&lt;string, JsonValue&gt;&gt;</param>
		/// <returns>An equivalent JsonObject</returns>
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
		/// Converts a collection of strings to a JsonArray.
		/// </summary>
		/// <param name="list">A collection of strings</param>
		/// <returns>A JsonArray containing the strings</returns>
		public static JsonValue ToJson(this IEnumerable<string> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of bools to a JsonArray.
		/// </summary>
		/// <param name="list">A collection of booleans</param>
		/// <returns>A JsonArray containing the booleans</returns>
		public static JsonValue ToJson(this IEnumerable<bool> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of Nullable&lt;bool&gt; to a JsonArray.
		/// </summary>
		/// <param name="list">A collection of Nullable&lt;bool&gt;</param>
		/// <returns>A JsonArray containing the Nullable&lt;bool&gt;</returns>
		public static JsonValue ToJson(this IEnumerable<bool?> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(b => b.HasValue ? new JsonValue(b.Value) : JsonValue.Null));
			return json;
		}
		/// <summary>
		/// Converts a collection of JsonArrays to a JsonArray.
		/// </summary>
		/// <param name="list">A collection of JsonArrays</param>
		/// <returns>A JsonArray containing the JsonArrays</returns>
		public static JsonValue ToJson(this IEnumerable<JsonArray> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of JsonObjects to a JsonArray.
		/// </summary>
		/// <param name="list">A collection of JsonObjects</param>
		/// <returns>A JsonArray containing the JsonObjects</returns>
		public static JsonValue ToJson(this IEnumerable<JsonObject> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Converts a collection of doubles to a JsonArray.
		/// </summary>
		/// <param name="list">A collection of doubles</param>
		/// <returns>A JsonArray containing the doubles</returns>
		public static JsonValue ToJson(this IEnumerable<double> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		/// <summary>
		/// Serializes a collection of objects which implement IJsonCompatible to a JsonArray of equivalent JsonValues.
		/// </summary>
		/// <param name="list">A collection of equivalent JsonValues</param>
		/// <returns>A JsonArray containing the equivalent JsonValues</returns>
		public static JsonValue ToJson(this IEnumerable<IJsonCompatible> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => j == null ? JsonValue.Null : j.ToJson()));
			return json;
		}
		/// <summary>
		/// Deserializes a collection of JsonValues to an IEnumerable of the objects.
		/// </summary>
		/// <typeparam name="T">The type of object contained in the collection</typeparam>
		/// <param name="json">The collection of JsonValues</param>
		/// <returns>A collection of the deserialized objects</returns>
		public static IEnumerable<T> FromJson<T>(this IEnumerable<JsonValue> json) where T : IJsonCompatible, new()
		{
			if (json == null) return null;
			var list = new List<T>();
			foreach (var value in json)
			{
				T item = new T();
				item.FromJson(value);
				list.Add(item);
			}
			return list;
		}
		/// <summary>
		/// Deserializes a JsonValue to its equivalent object.
		/// </summary>
		/// <typeparam name="T">The type of object</typeparam>
		/// <param name="json">The JsonValue to deserialize</param>
		/// <returns>A collection of the deserialized objects</returns>
		public static T FromJson<T>(this JsonObject json) where T : IJsonCompatible, new()
		{
			if (json == null) return default(T);
			T obj = new T();
			obj.FromJson(json);
			return obj;
		}
	}
}
