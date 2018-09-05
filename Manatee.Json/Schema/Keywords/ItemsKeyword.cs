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

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var array = context.Instance.Array;
			if (IsArray)
			{
				// have array of schemata: validate in sequence
				var i = 0;
				while (i < array.Count && i < Count)
				{
					var newContext = new SchemaValidationContext
						{
							Instance = array[i],
							Root = context.Root
						};
					errors.AddRange(this[i].Validate(newContext).Errors);
					i++;
				}
			}
			else
			{
				// have single schema: validate all against this
				var itemValidations = array.Select(jv =>
					{
						var newContext = new SchemaValidationContext
							{
								Instance = jv,
								Root = context.Root
							};
						return this[0].Validate(newContext);
					});
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