using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AdditionalPropertiesKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "$id";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public JsonSchema Value { get; private set; }

		public AdditionalPropertiesKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			throw new NotImplementedException();
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.ToJson(serializer);
		}
	}
}