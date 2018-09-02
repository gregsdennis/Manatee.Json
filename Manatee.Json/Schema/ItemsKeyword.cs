using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ItemsKeyword : List<JsonSchema>, JsonSchemaKeyword
	{
		public string Name => "items";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			var errors = new List<SchemaValidationError>();
			var array = json.Array;
			if (Count >= 1)
			{
				// have array of schemata: validate in sequence
				var i = 0;
				while (i < array.Count && i < Count)
				{
					errors.AddRange(this[i].Validate(array[i], root).Errors);
					i++;
				}

				var additionalItems = local.AdditionalItems();
				if (i < array.Count && additionalItems != null)
					if (Equals(additionalItems, JsonSchema.False))
					{
						var message = SchemaErrorMessages.Items.ResolveTokens(new Dictionary<string, object>
							{
								["value"] = json
							});
						errors.Add(new SchemaValidationError(string.Empty, message));
					}
					else if (!Equals(additionalItems, JsonSchema.True))
						errors.AddRange(array.Skip(i).SelectMany(j => additionalItems.Validate(j, root).Errors));
			}
			else if (Count == 1)
			{
				// have single schema: validate all against this
				var itemValidations = array.Select(v => this[0].Validate(v, root));
				errors.AddRange(itemValidations.SelectMany((v, i) => v.Errors.Select(e => e.PrependPropertySegment($"{i}"))));
			}

			return new SchemaValidationResults(errors);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Array)
				AddRange(serializer.Deserialize<List<JsonSchema>>(json));
			else
				Add(serializer.Deserialize<JsonSchema>(json));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.Select(serializer.Serialize).ToJson();
		}
	}
}