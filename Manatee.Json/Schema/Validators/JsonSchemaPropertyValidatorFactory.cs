using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Schema.Validators
{
	internal static class JsonSchemaPropertyValidatorFactory
	{
		private static readonly IEnumerable<IJsonSchemaPropertyValidator<JsonSchema04>> _schema04Validators;
		private static readonly IEnumerable<IJsonSchemaPropertyValidator<JsonSchema06>> _schema06Validators;

		static JsonSchemaPropertyValidatorFactory()
		{
			var allValidators = typeof(IJsonSchemaPropertyValidator).GetTypeInfo().Assembly.DefinedTypes
			                                                        .Where(t => typeof(IJsonSchemaPropertyValidator).GetTypeInfo().IsAssignableFrom(t) &&
			                                                                    !t.IsAbstract &&
			                                                                    t.IsClass)
			                                                        .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                                        .Cast<IJsonSchemaPropertyValidator>()
			                                                        .ToList();
			_schema04Validators = allValidators.OfType<IJsonSchemaPropertyValidator<JsonSchema04>>().ToList();
			_schema06Validators = allValidators.OfType<IJsonSchemaPropertyValidator<JsonSchema06>>().ToList();
		}

		public static IEnumerable<IJsonSchemaPropertyValidator<JsonSchema04>> Get(JsonSchema04 schema, JsonValue json)
		{
			return _schema04Validators.Where(v => v.Applies(schema, json));
		}

		public static IEnumerable<IJsonSchemaPropertyValidator<JsonSchema06>> Get(JsonSchema06 schema, JsonValue json)
		{
			return _schema06Validators.Where(v => v.Applies(schema, json));
		}
	}
}
