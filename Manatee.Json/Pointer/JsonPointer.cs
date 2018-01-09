using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Pointer
{
	public class JsonPointer : List<string>
	{
		public JsonPointer() { }
		public JsonPointer(IEnumerable<string> source) : base(source) { }

		public static JsonPointer Parse(string source)
		{
			return new JsonPointer(source.Split('/').Skip(1));
		}

		public JsonValue Evaluate(JsonValue root)
		{
			var current = root;
			foreach (var segment in this)
			{
				current = _EvaulateSegment(current, segment);
				if (current == null) break;
			}

			return current;
		}

		private JsonValue _EvaulateSegment(JsonValue current, string segment)
		{
			if (current.Type == JsonValueType.Array && int.TryParse(segment, out var index))
				return index < current.Array.Count
					       ? current.Array[index]
					       : null;

			return current.Type != JsonValueType.Object || !current.Object.TryGetValue(segment, out var value)
				       ? null
				       : value;
		}
	}
}
