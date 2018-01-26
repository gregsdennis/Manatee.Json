using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Schema
{
	public static class JsonSchemaPropertyValidatorFactory
	{
		private static readonly List<IJsonSchemaPropertyValidator> _validators;

		static JsonSchemaPropertyValidatorFactory()
		{
			_validators = typeof(IJsonSchemaPropertyValidator).GetTypeInfo().Assembly.DefinedTypes
			                                                  .Where(t => typeof(IJsonSchemaPropertyValidator).GetTypeInfo().IsAssignableFrom(t) &&
			                                                              !t.IsAbstract)
			                                                  .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                                  .Cast<IJsonSchemaPropertyValidator>()
			                                                  .ToList();
		}

		public static void RegisterValidator(IJsonSchemaPropertyValidator validator)
		{
			_validators.Add(validator);
		}

		internal static IEnumerable<IJsonSchemaPropertyValidator> Get<T>(T schema, JsonValue json)
			where T : IJsonSchema
		{
			return _validators.Where(v => v.Applies(schema, json));
		}
	}
}
