using System.Collections.Generic;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class PatternKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "oneOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public Regex Value { get; private set; }

		public PatternKeyword(Regex value)
		{
			Value = value;
		}
		public PatternKeyword(string value)
		{
			Value = new Regex(value, RegexOptions.Compiled);
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.String) return SchemaValidationResults.Valid;

			if (!Value.IsMatch(context.Instance.String))
			{
				var message = SchemaErrorMessages.Pattern.ResolveTokens(new Dictionary<string, object>
					{
						["pattern"] = Value.ToString(),
						["value"] = context.Instance
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = new Regex(json.String, RegexOptions.Compiled);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.ToString();
		}
	}
}