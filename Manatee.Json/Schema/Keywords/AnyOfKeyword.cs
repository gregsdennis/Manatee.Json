using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class AnyOfKeyword : List<JsonSchema>, IJsonSchemaKeyword, IEquatable<AnyOfKeyword>
	{
		public virtual string Name => "anyOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = this.Select((s, i) =>
				{
					var newContext = new SchemaValidationContext
						{
							BaseUri = context.BaseUri,
							Instance = context.Instance,
							Root = context.Root
						};
					var result = s.Validate(newContext);
					context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
					return result;
				}).ToList();
			return results.Any(r => r.IsValid)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(results);
		}
		public void RegisterSubschemas(Uri baseUri)
		{
			foreach (var schema in this)
			{
				schema.RegisterSubschemas(baseUri);
			}
		}
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			var first = pointer.FirstOrDefault();
			if (first == null) return null;

			if (!int.TryParse(first, out var index) || index < 0 || index >= Count) return null;

			return this[index].ResolveSubschema(new JsonPointer(pointer.Skip(1)), baseUri);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(serializer.Deserialize<JsonSchema>));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var array = this.Select(serializer.Serialize).ToJson();
			array.EqualityStandard = ArrayEquality.ContentsEqual;

			return array;
		}
		public bool Equals(AnyOfKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as AnyOfKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as AnyOfKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}