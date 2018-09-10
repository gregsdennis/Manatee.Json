using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Declares a dependency that is based on the presence of other properties in the JSON.
	/// </summary>
	public class PropertyDependency : IJsonSchemaDependency, IEquatable<PropertyDependency>
	{
		private readonly IEnumerable<string> _dependencies;

		/// <summary>
		/// Gets or sets the property with the dependency.
		/// </summary>
		public string PropertyName { get; }
		public JsonSchemaVersion SupportedVersions => _dependencies.Any()
			? JsonSchemaVersion.All
			: JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		/// <summary>
		/// Creates a new instance of the <see cref="PropertyDependency"/> class.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="dependencies">A collection of properties on which <paramref name="propertyName"/> is dependent.</param>
		public PropertyDependency(string propertyName, IEnumerable<string> dependencies)
		{
			if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
			var dependencyList = dependencies as IList<string> ?? dependencies.ToList();

			PropertyName = propertyName;
			_dependencies = dependencyList;
		}
		/// <summary>
		/// Creates a new instance of the <see cref="PropertyDependency"/> class.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="firstDependency">A minimal required property dependency.</param>
		/// <param name="otherDependencies">Additional property dependencies.</param>
		public PropertyDependency(string propertyName, string firstDependency, params string[] otherDependencies)
			: this(propertyName, new[] {firstDependency}.Concat(otherDependencies)) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var errors = new List<SchemaValidationError>();
			if (context.Instance.Type == JsonValueType.Object && context.Instance.Object.ContainsKey(PropertyName))
			{
				errors.AddRange(_dependencies.Except(context.Instance.Object.Keys)
				                             .Select(d =>new SchemaValidationError(string.Empty, $"When property '{PropertyName}' exists, property '{d}' should exist as well.")));
			}
			return new SchemaValidationResults(errors);
		}
		public void RegisterSubschemas(Uri baseUri) { }
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return null;
		}
		public bool Equals(PropertyDependency other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(PropertyName, other.PropertyName) &&
			       _dependencies.ContentsEqual(other._dependencies);
		}
		public bool Equals(IJsonSchemaDependency other)
		{
			return Equals(other as PropertyDependency);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as PropertyDependency);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return ((_dependencies != null ? _dependencies.GetHashCode() : 0) * 397) ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
			}
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var array = _dependencies.ToJson();
			array.Array.EqualityStandard = ArrayEquality.ContentsEqual;

			return array;
		}
	}
}