using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class JsonSchemaCollection : List<IJsonSchema>, IJsonSchema
	{
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			return new SchemaValidationResults(this.Select(s => s.Validate(json, root)));
		}
		public bool Equals(IJsonSchema other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			var collection = other as JsonSchemaCollection;
			return collection != null && this.SequenceEqual(collection);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(JsonSchemaFactory.FromJson));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this.Select(s => s.ToJson(serializer)));
		}
	}
}
