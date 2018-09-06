using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class DefaultKeyword : IJsonSchemaKeyword, IEquatable<DefaultKeyword>
	{
		public virtual string Name => "default";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonValue Value { get; private set; }

		public DefaultKeyword() { }
		public DefaultKeyword(JsonValue value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(DefaultKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as DefaultKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as DefaultKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}