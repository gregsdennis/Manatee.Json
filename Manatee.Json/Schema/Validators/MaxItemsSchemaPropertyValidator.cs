using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class MaxItemsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract uint? GetMaxItems(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMaxItems(typed).HasValue && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var maxItems = GetMaxItems((T)schema);
			if (json.Array.Count > maxItems)
			{
				var message = SchemaErrorMessages.MaxItems.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = maxItems,
						["actual"] = json.Array.Count
					});
				return new SchemaValidationResults(string.Empty, message);
			}
			return new SchemaValidationResults();
		}
	}
	
	internal class MaxItemsSchema04PropertyValidator : MaxItemsSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMaxItems(JsonSchema04 schema)
		{
			return schema.MaxItems;
		}
	}
	
	internal class MaxItemsSchema06PropertyValidator : MaxItemsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMaxItems(JsonSchema06 schema)
		{
			return schema.MaxItems;
		}
	}
	
	internal class MaxItemsSchema07PropertyValidator : MaxItemsSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override uint? GetMaxItems(JsonSchema07 schema)
		{
			return schema.MaxItems;
		}
	}
}
