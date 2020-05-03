using System;
using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Creates a dependency that is based on a secondary schema.
	/// </summary>
	public class SchemaDependency : IJsonSchemaDependency, IEquatable<SchemaDependency>
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Does not supports any tokens.
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "The schema failed validation.";

		private readonly JsonSchema _schema;

		/// <summary>
		/// Gets or sets the property with the dependency.
		/// </summary>
		public string PropertyName { get; }
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
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
		/// Provides the validation logic for this dependency.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(PropertyName, context)
				{
					Keyword = $"{context.Misc["dependencyParent"]}/{PropertyName}"
				};

			if (context.Instance.Type != JsonValueType.Object)
			{
				Log.Schema(() => "Instance not an object; not applicable");
				return results;
			}

			if (!context.Instance.Object.ContainsKey(PropertyName))
			{
				Log.Schema(() => $"Property {PropertyName} not found; not applicable");
				return results;
			}

			var newContext = new SchemaValidationContext(context)
				{
					BaseRelativeLocation = context.BaseRelativeLocation?.CloneAndAppend(PropertyName),
					RelativeLocation = context.RelativeLocation.CloneAndAppend(PropertyName),
				};

			var nestedResult = _schema.Validate(newContext);
			results.NestedResults = new List<SchemaValidationResults> {nestedResult};

			if (!nestedResult.IsValid)
			{
				Log.Schema(() => $"Property {PropertyName} found, but subschema failed");
				results.IsValid = false;
				results.ErrorMessage = ErrorTemplate;
			}
			else
			{
				if (context.ShouldTrackValidatedValues)
					newContext.LocallyEvaluatedPropertyNames.Add(PropertyName);
				context.UpdateEvaluatedPropertiesAndItemsFromSubschemaValidation(newContext);
			}

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="context">The context object.</param>
		public void RegisterSubschemas(SchemaValidationContext context)
		{
			_schema.RegisterSubschemas(context);
		}
		/// <summary>
		/// Resolves any subschemas during resolution of a `$ref` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <param name="supportedVersions">Indicates the root schema's supported versions.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema? ResolveSubschema(JsonPointer pointer, Uri baseUri, JsonSchemaVersion supportedVersions)
		{
			return _schema.ResolveSubschema(pointer, baseUri, supportedVersions);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(SchemaDependency? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(PropertyName, other.PropertyName) &&
			       Equals(_schema, other._schema);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaDependency? other)
		{
			return Equals(other as SchemaDependency);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj as SchemaDependency);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var value = (_schema != null ? _schema.GetHashCode() : 0) * 397;
				return value ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
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
			return _schema.ToJson(serializer);
		}
	}
}