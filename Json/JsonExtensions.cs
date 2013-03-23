/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonExtensions.cs
	Namespace:		Manatee.Json
	Class Name:		JsonExtensions
	Purpose:		Provides extension methods for Manatee.Json.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Enumerations;
using Manatee.Json.Serialization;

namespace Manatee.Json
{
	internal static class JsonExtensions
	{

		#region Try Get

		public static string TryGetString(this JsonObject obj, string key)
		{
			return obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].String : null;
		}
		public static double? TryGetNumber(this JsonObject obj, string key)
		{
			return obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].Number : (double?) null;
		}
		public static bool? TryGetBoolean(this JsonObject obj, string key)
		{
			return obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].Boolean : (bool?) null;
		}
		public static JsonArray TryGetArray(this JsonObject obj, string key)
		{
			return obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].Array : null;
		}
		public static JsonObject TryGetObject(this JsonObject obj, string key)
		{
			return obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].Object : null;
		}

		#endregion

		#region From JSON

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
		public static T FromJson<T>(this JsonObject json) where T : IJsonCompatible, new()
		{
			if (json == null) return default(T);
			T obj = new T();
			obj.FromJson(json);
			return obj;
		}

		#endregion

		#region To JSON

		public static JsonValue ToJson(this IEnumerable<string> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		public static JsonValue ToJson(this IEnumerable<bool> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		public static JsonValue ToJson(this IEnumerable<bool?> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(b => b.HasValue ? new JsonValue(b.Value) : JsonValue.Null));
			return json;
		}
		public static JsonValue ToJson(this IEnumerable<JsonArray> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		public static JsonValue ToJson(this IEnumerable<JsonObject> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		public static JsonValue ToJson(this IEnumerable<double> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => new JsonValue(j)));
			return json;
		}
		public static JsonValue ToJson(this IEnumerable<IJsonCompatible> list)
		{
			if (list == null) return JsonValue.Null;
			var json = new JsonArray();
			json.AddRange(list.Select(j => j == null ? JsonValue.Null : j.ToJson()));
			return json;
		}

		#endregion
	}
}
