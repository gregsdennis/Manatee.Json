using System.Collections.Generic;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationReference
	{
		public JsonPointer Source { get; }
		public object? Object { get; set; }
		public List<JsonPointer> Targets { get; } = new List<JsonPointer>();
		public bool DeserializationIsComplete { get; set; }

		public SerializationReference(JsonPointer source)
		{
			Source = source;
		}
	}
}