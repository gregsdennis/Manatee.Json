using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the `properties` JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class PropertiesKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword, IEquatable<PropertiesKeyword>
	{
		/// <summary>
		/// Gets or sets the error message template.
		/// </summary>
		/// <remarks>
		/// Does not supports any tokens.
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "At least one subschema failed validation.";

		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "properties";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		/// <summary>
		/// Gets the a value indicating the sequence in which this keyword will be evaluated.
		/// </summary>
		public int ValidationSequence => 1;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Validation;

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(Name, context);

			if (context.Instance.Type != JsonValueType.Object)
			{
				Log.Schema(() => "Instance not an object; not applicable");
				return results;
			}

			var valid = true;
			var reportChildErrors = context.Options.ShouldReportChildErrors(this, context);
			var obj = context.Instance.Object;
			var nestedResults = new List<SchemaValidationResults>();
			foreach (var property in this)
			{
				if (!obj.ContainsKey(property.Key))
				{
					Log.Schema(() => $"Property {property.Key} not found; skipping");
					continue;
				}

				if (context.ShouldTrackValidatedValues)
					context.EvaluatedPropertyNames.Add(property.Key);

				context.LocallyEvaluatedPropertyNames.Add(property.Key);
				var newContext = new SchemaValidationContext(context)
					{
						Instance = obj[property.Key],
						BaseRelativeLocation = context.BaseRelativeLocation?.CloneAndAppend(Name, property.Key),
						RelativeLocation = context.RelativeLocation.CloneAndAppend(Name, property.Key),
						InstanceLocation = context.InstanceLocation.CloneAndAppend(property.Key),
					};
				var localResults = property.Value.Validate(newContext);

				if (context.ShouldTrackValidatedValues)
				{
					context.EvaluatedPropertyNames.UnionWith(newContext.EvaluatedPropertyNames);
					context.EvaluatedPropertyNames.UnionWith(newContext.LocallyEvaluatedPropertyNames);
				}

				valid &= localResults.IsValid;

				if (context.Options.OutputFormat == SchemaValidationOutputFormat.Flag)
				{
					if (!valid)
					{
						Log.Schema(() => "Subschema failed; halting validation early");
						break;
					}
				}
				else if (reportChildErrors)
					nestedResults.Add(localResults);
			}

			results.IsValid = valid;
			results.NestedResults = nestedResults;

			if (!results.IsValid)
				results.ErrorMessage = ErrorTemplate;

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="context">The context object.</param>
		public void RegisterSubschemas(SchemaValidationContext context)
		{
			foreach (var schema in Values)
			{
				schema.RegisterSubschemas(context);
			}
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
			var first = pointer.FirstOrDefault();
			if (first == null) return null;

			if (!TryGetValue(first, out var schema)) return null;

			return schema.ResolveSubschema(new JsonPointer(pointer.Skip(1)), baseUri, supportedVersions);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			foreach (var kvp in json.Object)
			{
				this[kvp.Key] = serializer.Deserialize<JsonSchema>(kvp.Value);
			}
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(kvp => kvp.Key,
									 kvp => serializer.Serialize(kvp.Value))!
					   .ToJson();
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(PropertiesKeyword? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			var propertiesMatch = this.FullOuterJoin(other,
			                                         tk => tk.Key,
			                                         ok => ok.Key,
			                                         (tk, ok) => new {ThisProperty = tk.Value, OtherProperty = ok.Value})
				.ToList();

			return propertiesMatch.All(k => Equals(k.ThisProperty, k.OtherProperty));
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword? other)
		{
			return Equals(other as PropertiesKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj as PropertiesKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}