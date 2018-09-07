using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class EnumKeyword : List<JsonValue>, IJsonSchemaKeyword, IEquatable<EnumKeyword>
	{
		public virtual string Name => "enum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public EnumKeyword() { }
		public EnumKeyword(params JsonValue[] values)
			: base(values) { }
		public EnumKeyword(IEnumerable<JsonValue> values)
			: base(values) { }

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return Contains(context.Instance)
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, SchemaErrorMessages.Enum.ResolveTokens(new Dictionary<string, object>
					       {
						       ["value"] = context.Instance
					   }));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this) {EqualityStandard = ArrayEquality.ContentsEqual};
		}
		public bool Equals(EnumKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as EnumKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as EnumKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}