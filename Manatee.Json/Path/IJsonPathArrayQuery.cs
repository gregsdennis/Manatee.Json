using System.Collections.Generic;

namespace Manatee.Json.Path
{
	internal interface IJsonPathArrayQuery
	{
		IEnumerable<JsonValue> Find(JsonArray json, JsonValue root);
	}
}