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
	Purpose:		Provides extension methods for JsonObjects.

***************************************************************************************/
using Manatee.Json.Enumerations;

namespace Manatee.Json.Extensions
{
	/// <summary>
	/// Provides extension methods for JsonValues.
	/// </summary>
	public static class JsonExtensions
	{
		/// <summary>
		/// Returns a string or null if the key is not found or is not a string.
		/// </summary>
		/// <param name="obj">The object to search</param>
		/// <param name="key">The key</param>
		/// <returns>A string or null if the key is not found or is not a string</returns>
		public static string TryGetString(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type == JsonValueType.String) ? obj[key].String : null;
		}
		/// <summary>
		/// Returns a double or null if the key is not found or is not a string.
		/// </summary>
		/// <param name="obj">The object to search</param>
		/// <param name="key">The key</param>
		/// <returns>A double or null if the key is not found or is not a string</returns>
		public static double? TryGetNumber(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type == JsonValueType.Number) ? obj[key].Number : (double?)null;
		}
		/// <summary>
		/// Returns a bool or null if the key is not found or is not a string.
		/// </summary>
		/// <param name="obj">The object to search</param>
		/// <param name="key">The key</param>
		/// <returns>A bool or null if the key is not found or is not a string</returns>
		public static bool? TryGetBoolean(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].Boolean : (bool?)null;
		}
		/// <summary>
		/// Returns a JsonArray or null if the key is not found or is not a string.
		/// </summary>
		/// <param name="obj">The object to search</param>
		/// <param name="key">The key</param>
		/// <returns>A JsonArray or null if the key is not found or is not a string</returns>
		public static JsonArray TryGetArray(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].Array : null;
		}
		/// <summary>
		/// Returns a JsonObject or null if the key is not found or is not a string.
		/// </summary>
		/// <param name="obj">The object to search</param>
		/// <param name="key">The key</param>
		/// <returns>A JsonObject or null if the key is not found or is not a string</returns>
		public static JsonObject TryGetObject(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type != JsonValueType.Null) ? obj[key].Object : null;
		}
	}
}
