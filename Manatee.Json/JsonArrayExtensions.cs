using System;
using System.Linq;

namespace Manatee.Json
{
	/// <summary>
	/// Provides extension methods for <see cref="JsonArray"/>s.
	/// </summary>
	public static class JsonArrayExtensions
	{
		/// <summary>
		/// Returns a <see cref="JsonArray"/> containing only the <see cref="JsonValue"/>s of a specified type from a given <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="arr">The array to search</param>
		/// <param name="type">The type of value to return</param>
		/// <returns>A <see cref="JsonArray"/> containing only the <see cref="JsonValue"/>s of a specified type</returns>
		public static JsonArray OfType(this JsonArray? arr, JsonValueType type)
		{
			if (arr == null) return null!;

			var retVal = new JsonArray();
			retVal.AddRange(arr.Where(j => j.Type == type));
			return retVal;
		}
	}
}