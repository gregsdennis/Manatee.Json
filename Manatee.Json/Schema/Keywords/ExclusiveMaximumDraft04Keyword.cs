using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ExclusiveMaximumDraft04Keyword : IJsonSchemaKeyword
	{
		public string Name => "exclusiveMaximum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;
		public int ValidationSequence => 1;

		public bool Value { get; private set; }

		public ExclusiveMaximumDraft04Keyword() { }
		public ExclusiveMaximumDraft04Keyword(bool value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			var keyword = context.Local.OfType<MaximumKeyword>().FirstOrDefault();
			if (keyword == null) return SchemaValidationResults.Valid;

			if (!Value) return SchemaValidationResults.Valid;
			
			if (context.Instance.Number >= keyword.Value)
			{
				var message = SchemaErrorMessages.ExclusiveMaximum.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = context.Instance.Number,
						["value"] = context.Instance
				});

				return new SchemaValidationResults(Name, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Boolean;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}