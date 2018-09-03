using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ItemsKeyword : List<JsonSchema>, IJsonSchemaKeyword
	{
		public string Name => "items";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public bool IsArray { get; private set; }

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			if (json.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var array = json.Array;
			if (IsArray)
			{
				// have array of schemata: validate in sequence
				var i = 0;
				while (i < array.Count && i < Count)
				{
					errors.AddRange(this[i].Validate(array[i], root).Errors);
					i++;
				}
			}
			else
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
			{
				AddRange(serializer.Deserialize<List<JsonSchema>>(json));
				IsArray = true;
			}
			else
				Add(serializer.Deserialize<JsonSchema>(json));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var schemas = this.Select(serializer.Serialize).ToJson();

			return !IsArray ? schemas : schemas[0];
		}
	}
}