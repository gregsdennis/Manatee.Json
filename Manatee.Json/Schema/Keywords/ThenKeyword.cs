using System;
using System.Diagnostics;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class ThenKeyword : IJsonSchemaKeyword, IEquatable<ThenKeyword>
	{
		public virtual string Name => "then";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonSchema Value { get; private set; }

		public ThenKeyword() { }
		public ThenKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return serializer.Serialize(Value);
		}
		public bool Equals(ThenKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ThenKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ThenKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}