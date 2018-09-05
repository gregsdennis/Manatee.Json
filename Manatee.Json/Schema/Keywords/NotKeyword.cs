using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class NotKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "not";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public JsonSchema Value { get; private set; }

		public NotKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var newContext = new SchemaValidationContext
				{
					Instance = context.Instance,
					Root = context.Root
				};
			var results = Value.Validate(newContext);
			context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
			if (results.IsValid)
			{
				var message = SchemaErrorMessages.Not.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = context.Instance
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return serializer.Serialize(Value);
		}
	}
}