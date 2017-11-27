using System.Collections.Generic;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class DefinitionsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IJsonSchema GetMetaSchema();
		
		public bool Applies(T schema, JsonValue json)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey("definitions");
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var definitions = json.Object["definitions"];
			if (definitions.Type != JsonValueType.Object)
				errors.Add(new SchemaValidationError("definitions", "Property 'definitions' must contain an object."));
			var metaSchema = GetMetaSchema();
			foreach (var value in definitions.Object.Values)
			{
				errors.AddRange(metaSchema.Validate(value).Errors);
			}
			return new SchemaValidationResults(errors);
		}
	}
	
	internal class DefinitionsSchema04PropertyValidator : DefinitionsSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IJsonSchema GetMetaSchema()
		{
			return JsonSchema04.MetaSchema;
		}
	}
	
	internal class DefinitionsSchema06PropertyValidator : DefinitionsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IJsonSchema GetMetaSchema()
		{
			return JsonSchema06.MetaSchema;
		}
	}
	
	internal class DefinitionsSchema07PropertyValidator : DefinitionsSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IJsonSchema GetMetaSchema()
		{
			return JsonSchema07.MetaSchema;
		}
	}
}
