using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides the validation system with validators.
	/// </summary>
	public static class JsonSchemaPropertyValidatorFactory
	{
		private static readonly List<JsonSchemaPropertyValidator> _validators;

		static JsonSchemaPropertyValidatorFactory()
		{
			_validators = typeof(JsonSchemaPropertyValidator).GetTypeInfo().Assembly.DefinedTypes
			                                                  .Where(t => typeof(JsonSchemaPropertyValidator).GetTypeInfo().IsAssignableFrom(t) &&
			                                                              !t.IsAbstract)
			                                                  .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                                  .Cast<JsonSchemaPropertyValidator>()
			                                                  .ToList();
		}

		/// <summary>
		/// Registers a new validator to be executed during schema validation.
		/// </summary>
		/// <param name="validator"></param>
		public static void RegisterValidator(JsonSchemaPropertyValidator validator)
		{
			_validators.Add(validator);
		}

		internal static IEnumerable<JsonSchemaPropertyValidator> Get<T>(T schema, JsonValue json)
			where T : JsonSchema
		{
			return _validators.Where(v => v.Applies(schema, json));
		}
	}
}
