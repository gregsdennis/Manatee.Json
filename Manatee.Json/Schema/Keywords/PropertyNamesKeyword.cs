using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class PropertyNamesKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "propertyNames";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public JsonSchema Value { get; private set; }

		public PropertyNamesKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
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