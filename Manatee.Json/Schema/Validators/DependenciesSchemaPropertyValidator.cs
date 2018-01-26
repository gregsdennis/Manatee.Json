using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class DependenciesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract IEnumerable<IJsonSchemaDependency> GetDependencies(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetDependencies(typed) != null;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			return new SchemaValidationResults(GetDependencies((T)schema).SelectMany(d => d.Validate(json, root).Errors));
		}
	}
	
	internal class DependeciesSchema04PropertyValidator : DependenciesSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IEnumerable<IJsonSchemaDependency> GetDependencies(JsonSchema04 schema)
		{
			return schema.Dependencies;
		}
	}
	
	internal class DependeciesSchema06PropertyValidator : DependenciesSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IEnumerable<IJsonSchemaDependency> GetDependencies(JsonSchema06 schema)
		{
			return schema.Dependencies;
		}
	}
	
	internal class DependeciesSchema07PropertyValidator : DependenciesSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override IEnumerable<IJsonSchemaDependency> GetDependencies(JsonSchema07 schema)
		{
			return schema.Dependencies;
		}
	}
}
