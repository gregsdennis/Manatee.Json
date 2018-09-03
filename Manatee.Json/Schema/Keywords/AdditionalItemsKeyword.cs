using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AdditionalItemsKeyword : IJsonSchemaKeyword
	{
		public string Name => "additionalItems";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public JsonSchema Value { get; private set; }

		public AdditionalItemsKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			if (json.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			var itemsKeyword = local.OfType<ItemsKeyword>().FirstOrDefault();
			if (itemsKeyword == null || !itemsKeyword.IsArray) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var array = json.Array;

			var i = itemsKeyword.Count;
			if (i < array.Count)
				if (Equals(Value, JsonSchema.False))
				{
					var message = SchemaErrorMessages.Items.ResolveTokens(new Dictionary<string, object>
						{
							["value"] = json
						});
					errors.Add(new SchemaValidationError(string.Empty, message));
				}
				else if (!Equals(Value, JsonSchema.True))
					errors.AddRange(array.Skip(i).SelectMany(j => Value.Validate(j, root).Errors));

			return new SchemaValidationResults(errors);
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