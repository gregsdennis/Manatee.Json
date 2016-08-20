using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Json.Schema.Validators
{
	internal static class JsonSchemaValidatorFactory
	{
		private static readonly IEnumerable<IJsonSchemaValidator> AllValidators;

		static JsonSchemaValidatorFactory()
		{
			AllValidators = typeof(IJsonSchemaValidator).Assembly.GetTypes()
			                                            .Where(t => typeof(IJsonSchemaValidator).IsAssignableFrom(t) &&
			                                                        !t.IsAbstract &&
			                                                        t.IsClass)
			                                            .Select(Activator.CreateInstance)
			                                            .Cast<IJsonSchemaValidator>()
			                                            .ToList();
		}

		public static IEnumerable<IJsonSchemaValidator> Get(JsonSchema schema)
		{
			return AllValidators.Where(v => v.Applies(schema));
		}
	}
}
