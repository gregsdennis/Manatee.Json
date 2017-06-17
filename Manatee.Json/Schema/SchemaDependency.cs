using System;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Creates a dependency that is based on a secondary schema.
	/// </summary>
	public class SchemaDependency : IJsonSchemaDependency
	{
		private readonly IJsonSchema _schema;

		/// <summary>
		/// Gets or sets the property with the dependency.
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="SchemaDependency"/> class.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="schema">The schema which must be validated.</param>
		public SchemaDependency(string propertyName, IJsonSchema schema)
		{
			_schema = schema;
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("Must provide a property name.");

			PropertyName = propertyName;
		}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			if (json.Type != JsonValueType.Object || !json.Object.ContainsKey(PropertyName))
				return new SchemaValidationResults();
			return _schema.Validate(json, root);
		}
		/// <summary>
		/// Gets the JSON data to be used as the value portion in the dependency list of the schema.
		/// </summary>
		public JsonValue GetJsonData()
		{
			return _schema.ToJson(null);
		}
	}
}