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
	public class DefinitionsKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword, IEquatable<DefinitionsKeyword>
	{
		public string Name => "definitions";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
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
		public bool Equals(DefinitionsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			var definitionsMatch = this.LeftOuterJoin(other,
			                                         tk => tk.Key,
			                                         ok => ok.Key,
			                                         (tk, ok) => new { ThisDefinition = tk.Value, OtherDefinition = ok.Value })
				.ToList();

			return definitionsMatch.All(k => Equals(k.ThisDefinition, k.OtherDefinition));

		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as DefinitionsKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as DefinitionsKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}