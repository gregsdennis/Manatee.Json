using System.Collections.Generic;
using System.Globalization;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class MaxLengthKeyword : IJsonSchemaKeyword
	{
		public string Name => "maxLength";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public uint Value { get; private set; }

		public MaxLengthKeyword() { }
		public MaxLengthKeyword(uint value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.String) return SchemaValidationResults.Valid;

			var length = new StringInfo(context.Instance.String).LengthInTextElements;
			if (length > Value)
			{
				var message = SchemaErrorMessages.MaxLength.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = context.Instance.String.Length,
						["value"] = context.Instance
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