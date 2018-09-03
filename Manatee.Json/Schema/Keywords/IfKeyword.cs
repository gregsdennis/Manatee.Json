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

		public JsonSchema Value { get; private set; }

		public IfKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			var then = local.OfType<ThenKeyword>().FirstOrDefault();
			var @else = local.OfType<ElseKeyword>().FirstOrDefault();

			if (then == null && @else == null) return SchemaValidationResults.Valid;

			var ifResults = Value.Validate(json, root);

			if (ifResults.IsValid)
			{
				var thenResults = then.Value.Validate(json, root);
				if (thenResults.IsValid) return SchemaValidationResults.Valid;

				var message = SchemaErrorMessages.Then.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = json
					});
				return new SchemaValidationResults("then", message);
			}
			else
			{
				var elseResults = @else.Value.Validate(json, root);
				if (elseResults.IsValid) return SchemaValidationResults.Valid;

				var message = SchemaErrorMessages.Else.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = json
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
