using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationReference
	{

		public JsonValue Json { get; set; }
		public object Object { get; set; }
		public JsonPointer Reference { get; set; }
		public int UsageCount { get; set; }
		public bool DeserializationIsComplete { get; set; }

		public override string ToString()
		{
			return $"Usage: {UsageCount}, Object: {Object}, Json: {Json}";
		}
	}
}