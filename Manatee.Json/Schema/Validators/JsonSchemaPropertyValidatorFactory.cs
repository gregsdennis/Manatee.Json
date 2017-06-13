﻿using System;
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
			AllValidators = typeof(IJsonSchemaPropertyValidator).TypeInfo().Assembly.DefinedTypes
			// TODO: optimize
														.Where(t => typeof(IJsonSchemaPropertyValidator).GetTypeInfo().IsAssignableFrom(t) &&
																	!t.IsAbstract &&
																	t.IsClass)
			                                            .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                            .Cast<IJsonSchemaPropertyValidator>()
			                                            .ToList();
		}

		public static IEnumerable<IJsonSchemaPropertyValidator> Get(JsonSchema schema, JsonValue json)
		{
			return AllValidators.Where(v => v.Applies(schema, json));
		}
	}
}
