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
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions => _dependencies.Any()
			? JsonSchemaVersion.All
			: JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft2019_04;

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
		/// Provides the validation logic for this dependency.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(PropertyName, context);

			if (context.Instance.Type != JsonValueType.Object) return results;
			if (context.Instance.Object.ContainsKey(PropertyName))
			{
				var missingProperties = _dependencies.Except(context.Instance.Object.Keys).ToList();
				if (missingProperties.Any())
				{
					results.IsValid = false;
					results.Keyword = $"dependencies/{PropertyName}";
					results.AdditionalInfo["missingProperties"] = missingProperties.ToJson();
				}
			}

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with <code>$ref</code> keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		public void RegisterSubschemas(Uri baseUri) { }
		/// <summary>
		/// Resolves any subschemas during resolution of a <code>$ref</code> during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return null;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(PropertyDependency other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(PropertyName, other.PropertyName) &&
			       _dependencies.ContentsEqual(other._dependencies);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaDependency other)
		{
			return Equals(other as PropertyDependency);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as PropertyDependency);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((_dependencies != null ? _dependencies.GetHashCode() : 0) * 397) ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
			}
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var array = _dependencies.ToJson();
			array.Array.EqualityStandard = ArrayEquality.ContentsEqual;

			return array;
		}
	}
}