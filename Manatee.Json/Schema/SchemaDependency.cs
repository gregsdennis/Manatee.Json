using System;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Creates a dependency that is based on a secondary schema.
	/// </summary>
	public class SchemaDependency : IJsonSchemaDependency, IEquatable<SchemaDependency>
	{
		private readonly JsonSchema _schema;

		/// <summary>
		/// Gets or sets the property with the dependency.
		/// </summary>
		public string PropertyName { get; }
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		/// <summary>
		/// Creates a new instance of the <see cref="SchemaDependency"/> class.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="schema">The schema which must be validated.</param>
		public SchemaDependency(string propertyName, JsonSchema schema)
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("Must provide a property name.");

			_schema = schema;
			PropertyName = propertyName;
		}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object || !context.Instance.Object.ContainsKey(PropertyName))
				return new SchemaValidationResults();
			var newContext = new SchemaValidationContext
				{
					Instance = context.Instance,
					Root = context.Root
				};
			return _schema.Validate(newContext);
		}
		public void RegisterSubschemas(Uri baseUri)
		{
			_schema.RegisterSubschemas(baseUri);
		}
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return _schema.ResolveSubschema(pointer, baseUri);
		}
		public bool Equals(SchemaDependency other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(PropertyName, other.PropertyName) &&
			       Equals(_schema, other._schema);
		}
		public bool Equals(IJsonSchemaDependency other)
		{
			return Equals(other as SchemaDependency);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as SchemaDependency);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return ((_schema != null ? _schema.GetHashCode() : 0) * 397) ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
			}
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return _schema.ToJson(serializer);
		}
	}
}