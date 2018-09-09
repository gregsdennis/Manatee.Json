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
	public class PropertiesKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword, IEquatable<PropertiesKeyword>
	{
		public string Name => "properties";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var obj = context.Instance.Object;
			var errors = new List<SchemaValidationError>();
			foreach (var property in this)
			{
				if (!obj.ContainsKey(property.Key)) continue;

				context.EvaluatedPropertyNames.Add(property.Key);
				var newContext = new SchemaValidationContext
					{
						BaseUri = context.BaseUri,
						Instance = obj[property.Key],
						Root = context.Root
					};
				var result = property.Value.Validate(newContext);
				if (result != null && !result.IsValid)
					errors.AddRange(result.Errors.Select(e => e.PrependPropertySegment(property.Key)));
			}
			return new SchemaValidationResults(errors);
		}
		public void RegisterSubschemas(Uri baseUri)
		{
			foreach (var schema in Values)
			{
				schema.RegisterSubschemas(baseUri);
			}
		}
		public JsonSchema ResolveSubschema(JsonPointer pointer)
		{
			var first = pointer.FirstOrDefault();
			if (first == null) return null;

			if (!TryGetValue(first, out var schema)) return null;

			return schema.ResolveSubschema(new JsonPointer(pointer.Skip(1)));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			foreach (var kvp in json.Object)
			{
				this[kvp.Key] = serializer.Deserialize<JsonSchema>(kvp.Value);
			}
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(kvp => kvp.Key,
			                         kvp => serializer.Serialize(kvp.Value))
			           .ToJson();
		}
		public bool Equals(PropertiesKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			var propertiesMatch = this.LeftOuterJoin(other,
			                                      tk => tk.Key,
			                                      ok => ok.Key,
			                                      (tk, ok) => new { ThisProperty = tk.Value, OtherProperty = ok.Value })
				.ToList();

			return propertiesMatch.All(k => Equals(k.ThisProperty, k.OtherProperty));
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as PropertiesKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as PropertiesKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}