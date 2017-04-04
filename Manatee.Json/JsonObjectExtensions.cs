using System;

namespace Manatee.Json
{
	/// <summary>
	/// Provides extension methods for <see cref="JsonValue"/>s.
	/// </summary>
	public static class JsonObjectExtensions
	{
		/// <summary>
		/// Returns a <see cref="string"/> or null if the key is not found or is not a <see cref="string"/>.
		/// </summary>
		/// <param name="obj">The <see cref="JsonObject"/> to search</param>
		/// <param name="key">The key</param>
		/// <returns>A <see cref="string"/> or null if the key is not found or is not a <see cref="string"/></returns>
		public static string TryGetString(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type == JsonValueType.String) ? obj[key].String : null;
		}
		/// <summary>
		/// Returns a <see cref="Nullable{Double}"/> or null if the key is not found or is not a double.
		/// </summary>
		/// <param name="obj">The <see cref="JsonObject"/> to search</param>
		/// <param name="key">The key</param>
		/// <returns>A <see cref="Nullable{Double}"/> or null if the key is not found or is not a <see cref="Nullable{Double}"/></returns>
		public static double? TryGetNumber(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type == JsonValueType.Number) ? obj[key].Number : (double?)null;
		}
		/// <summary>
		/// Returns a <see cref="Nullable{Boolan}"/> or null if the key is not found or is not a <see cref="bool"/>.
		/// </summary>
		/// <param name="obj">The <see cref="JsonObject"/> to search</param>
		/// <param name="key">The key</param>
		/// <returns>A <see cref="Nullable{Boolan}"/> or null if the key is not found or is not a <see cref="Nullable{Boolan}"/></returns>
		public static bool? TryGetBoolean(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type == JsonValueType.Boolean) ? obj[key].Boolean : (bool?)null;
		}
		/// <summary>
		/// Returns a <see cref="JsonArray"/> or null if the key is not found or is not a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="obj">The <see cref="JsonObject"/> to search</param>
		/// <param name="key">The key</param>
		/// <returns>A <see cref="JsonArray"/> or null if the key is not found or is not a <see cref="JsonArray"/></returns>
		public static JsonArray TryGetArray(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type == JsonValueType.Array) ? obj[key].Array : null;
		}
		/// <summary>
		/// Returns a <see cref="JsonObject"/> or null if the key is not found or is not a <see cref="JsonObject"/>.
		/// </summary>
		/// <param name="obj">The <see cref="JsonObject"/> to search</param>
		/// <param name="key">The key</param>
		/// <returns>A <see cref="JsonObject"/> or null if the key is not found or is not a <see cref="JsonObject"/></returns>
		public static JsonObject TryGetObject(this JsonObject obj, string key)
		{
			return (obj == null) ? null : obj.ContainsKey(key) && (obj[key].Type == JsonValueType.Object) ? obj[key].Object : null;
		}
	}
}
