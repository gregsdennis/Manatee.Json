using System.Collections.Generic;

namespace Manatee.Json.Path
{
	internal interface IJsonPathSearchParameter
	{
		IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json, JsonValue root);
	}
}