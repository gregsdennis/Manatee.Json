using System.Collections.Generic;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ExamplesKeyword : List<JsonValue>, IJsonSchemaKeyword
	{
		public virtual string Name => "examples";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public ExamplesKeyword() { }
		public ExamplesKeyword(params JsonValue[] values)
			: base(values) { }
		public ExamplesKeyword(IEnumerable<JsonValue> values)
			: base(values) { }

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this);
		}
	}
}