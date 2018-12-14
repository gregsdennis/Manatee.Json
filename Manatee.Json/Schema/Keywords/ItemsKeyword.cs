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
	/// Defines the <code>items</code> JSON Schema keyword.
	/// </summary>
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class ItemsKeyword : List<JsonSchema>, IJsonSchemaKeyword, IEquatable<ItemsKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public string Name => "items";
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
		public SchemaVocabulary Vocabulary => SchemaVocabularies.Assertion;

		/// <summary>
		/// Gets whether this keyword represents a single schema or an array of schemas.
		/// </summary>
		public bool IsArray { get; set; }

		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="context">The context object.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = new SchemaValidationResults(Name, context);

			if (context.Instance.Type != JsonValueType.Array) return results;

			var nestedResults = new List<SchemaValidationResults>();
			var array = context.Instance.Array;
			if (IsArray)
			{
				// have array of schemata: validate in sequence
				var i = 0;
				while (i < array.Count && i < Count)
				{
					var newContext = new SchemaValidationContext
						{
							BaseUri = context.BaseUri,
							Instance = array[i],
							Root = context.Root,
							BaseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(Name, i.ToString()),
							RelativeLocation = context.RelativeLocation.CloneAndAppend(Name, i.ToString()),
							InstanceLocation = context.InstanceLocation.CloneAndAppend(i.ToString())
						};
					nestedResults.Add(this[i].Validate(newContext));
					i++;
					context.LastEvaluatedIndex = Math.Max(context.LastEvaluatedIndex, i);
					context.LocalTierLastEvaluatedIndex = Math.Max(context.LastEvaluatedIndex, i);
				}
			}
			else
			{
				// have single schema: validate all against this
				var baseRelativeLocation = context.BaseRelativeLocation.CloneAndAppend(Name);
				var relativeLocation = context.RelativeLocation.CloneAndAppend(Name);
				var itemValidations = array.Select((jv, i) =>
					{
						var newContext = new SchemaValidationContext
							{
								BaseUri = context.BaseUri,
								Instance = jv,
								Root = context.Root,
								BaseRelativeLocation = baseRelativeLocation,
								RelativeLocation = relativeLocation,
								InstanceLocation = context.InstanceLocation.CloneAndAppend(i.ToString())
							};
						var localResults = this[0].Validate(newContext);
						context.LastEvaluatedIndex = Math.Max(context.LastEvaluatedIndex, i);
						context.LocalTierLastEvaluatedIndex = Math.Max(context.LastEvaluatedIndex, i);
						return localResults;
					});
				nestedResults.AddRange(itemValidations);
			}

			if (nestedResults.Any(r => !r.IsValid))
			{
				results.IsValid = false;
				results.Keyword = Name;
			}

			results.NestedResults.AddRange(nestedResults);

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with <code>$ref</code> keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		public void RegisterSubschemas(Uri baseUri)
		{
			foreach (var schema in this)
			{
				schema.RegisterSubschemas(baseUri);
			}
		}
		/// <summary>
		/// Resolves any subschemas during resolution of a <code>$ref</code> during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			var first = pointer.FirstOrDefault();
			if (first == null) return null;

			if (!int.TryParse(first, out var index) || index < 0 || index >= Count) return null;

			return this[index].ResolveSubschema(new JsonPointer(pointer.Skip(1)), baseUri);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Array)
			{
				AddRange(serializer.Deserialize<List<JsonSchema>>(json));
				IsArray = true;
			}
			else
				Add(serializer.Deserialize<JsonSchema>(json));
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (IsArray)
			{
				var schemas = this.Select(serializer.Serialize).ToJson();
				schemas.EqualityStandard = ArrayEquality.SequenceEqual;

				return schemas;
			}

			return serializer.Serialize(this[0]);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ItemsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ItemsKeyword);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as ItemsKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}