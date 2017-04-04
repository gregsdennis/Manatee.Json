using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal static class JsonSchemaPropertyValidatorFactory
	{
		private static readonly IEnumerable<IJsonSchemaPropertyValidator> AllValidators;

		static JsonSchemaPropertyValidatorFactory()
		{
			AllValidators = typeof(IJsonSchemaPropertyValidator).TypeInfo().Assembly.GetTypes()
														.Where(t => typeof(IJsonSchemaPropertyValidator).IsAssignableFrom(t) &&
																	!t.TypeInfo().IsAbstract &&
																	t.TypeInfo().IsClass)
			                                            .Select(Activator.CreateInstance)
			                                            .Cast<IJsonSchemaPropertyValidator>()
			                                            .ToList();
		}

		public static IEnumerable<IJsonSchemaPropertyValidator> Get(JsonSchema schema, JsonValue json)
		{
			return AllValidators.Where(v => v.Applies(schema, json));
		}
	}
}
