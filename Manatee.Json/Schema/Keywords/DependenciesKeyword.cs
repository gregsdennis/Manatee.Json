using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class DependenciesKeyword : List<IJsonSchemaDependency>, IJsonSchemaKeyword, IEquatable<DependenciesKeyword>
	{
		public virtual string Name => "dependencies";
		public virtual JsonSchemaVersion SupportedVersions => this.Aggregate(JsonSchemaVersion.All, (current, i) => current & i.SupportedVersions);
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return new SchemaValidationResults(this.SelectMany(d =>
				{
					return d.Validate(context).Errors;
				}));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			foreach (var kvp in json.Object)
			{
				if (kvp.Value.Type == JsonValueType.Array)
					Add(new PropertyDependency(kvp.Key, kvp.Value.Array.Select(jv => jv.String)));
				else
					Add(new SchemaDependency(kvp.Key, serializer.Deserialize<JsonSchema>(kvp.Value)));
			}
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(d => d.PropertyName,
			                         d => d.ToJson(serializer))
			           .ToJson();
		}
		public bool Equals(DependenciesKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as DependenciesKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as DependenciesKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}