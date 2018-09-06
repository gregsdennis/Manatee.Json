using System;
using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ExamplesKeyword : List<JsonValue>, IJsonSchemaKeyword, IEquatable<ExamplesKeyword>
	{
		public virtual string Name => "examples";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public ExamplesKeyword() { }
		public ExamplesKeyword(params JsonValue[] values)
			: base(values) { }
		public ExamplesKeyword(IEnumerable<JsonValue> values)
			: base(values) { }

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this);
		}
		public bool Equals(ExamplesKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ExamplesKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExamplesKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}