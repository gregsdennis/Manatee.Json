using System.Collections.Generic;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Defines methods required to perform a recursive search in a JSON Path.
	/// </summary>
	public interface IJsonPathSearchParameter
	{
		/// <summary>
		/// Finds matching <see cref="JsonValue"/>s in a <see cref="JsonArray"/>.
		/// </summary>
		/// <param name="json">A collection of <see cref="JsonValue"/>s to search.</param>
		/// <returns>A collection of matching <see cref="JsonValue"/>s.</returns>
		IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json);
	}
}