using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class IfKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "if";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonSchema Value { get; private set; }

		public IfKeyword() { }
		public IfKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var then = context.Local.OfType<ThenKeyword>().FirstOrDefault();
			var @else = context.Local.OfType<ElseKeyword>().FirstOrDefault();

			if (then == null && @else == null) return SchemaValidationResults.Valid;

			var newContext = new SchemaValidationContext
				{
					Instance = context.Instance,
					Root = context.Root
				};

			var ifResults = Value.Validate(newContext);

			if (ifResults.IsValid)
			{
				if (then == null) return SchemaValidationResults.Valid;

				var thenResults = then.Value.Validate(newContext);
				if (thenResults.IsValid) return SchemaValidationResults.Valid;

				var message = SchemaErrorMessages.Then.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = context.Instance
					});
				return new SchemaValidationResults("then", message);
			}
			else
			{
				if (@else == null) return SchemaValidationResults.Valid;

				var elseResults = @else.Value.Validate(newContext);
				if (elseResults.IsValid) return SchemaValidationResults.Valid;

				var message = SchemaErrorMessages.Else.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = context.Instance
					});
				return new SchemaValidationResults("else", message);
			}
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
