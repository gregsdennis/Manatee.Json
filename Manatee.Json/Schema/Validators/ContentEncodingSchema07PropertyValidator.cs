using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Schema.Validators
{
	internal class ContentEncodingSchema07PropertyValidator : IJsonSchemaPropertyValidator<JsonSchema07>
	{
		private static readonly List<string> _validValues =
			typeof(ContentEncoding).GetTypeInfo()
			                       .DeclaredFields
			                       .SelectMany(f => f.GetCustomAttributes<DisplayAttribute>())
			                       .Select(a => a.Description)
			                       .ToList();

		public bool Applies(JsonSchema07 schema, JsonValue json)
		{
			return schema.ContentEncoding != null && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(JsonSchema07 schema, JsonValue json, JsonValue root)
		{
			throw new NotImplementedException();
		}
	}
}