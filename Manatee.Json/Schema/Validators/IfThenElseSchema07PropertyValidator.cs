using System;

namespace Manatee.Json.Schema.Validators
{
	internal class IfThenElseSchema07PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema07>
	{
		public bool Applies(JsonSchema07 schema, JsonValue json)
		{
			return schema.If != null || schema.Then != null || schema.Else != null;
		}
		public SchemaValidationResults Validate(JsonSchema07 schema, JsonValue json, JsonValue root)
		{
			throw new NotImplementedException();
		}
	}
}