using System.Collections.Generic;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Defines methods required to query an array in a JSON Path.
	/// </summary>
	public interface IJsonPathArrayQuery
	{
		/// <summary>
		/// Finds matching <see cref="JsonValue"/>s in a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="json">The array to search.</param>
		/// <returns>A collection of matching <see cref="JsonValue"/>s.</returns>
		IEnumerable<JsonValue> Find(JsonArray json);
	}
}