using System;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class PropertyNamesKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "propertyNames";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonSchema Value { get; private set; }

		public PropertyNamesKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var results = context.Instance.Object.Keys.Select(propertyName =>
				{
					var newContext = new SchemaValidationContext
						{
							Instance = propertyName,
							Root = context.Root
						};
					var result = Value.Validate(newContext);

					return result;
				});

			return new SchemaValidationResults(results);
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