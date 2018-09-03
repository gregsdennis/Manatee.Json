using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class MinLengthKeyword : IJsonSchemaKeyword
	{
		public string Name => "minLength";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public uint Value { get; private set; }

		public MinLengthKeyword(uint value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			if (json.Type != JsonValueType.String) return SchemaValidationResults.Valid;

			if (json.String.Length < Value)
			{
				var message = SchemaErrorMessages.MaxLength.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = json.String.Length,
						["value"] = json
					});
				return new SchemaValidationResults(string.Empty, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = (uint) json.Number;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}