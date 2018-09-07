using System;
using System.Diagnostics;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class IdKeyword : IJsonSchemaKeyword, IEquatable<IdKeyword>
	{
		public virtual string Name => "$id";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public string Value { get; private set; }

		public IdKeyword() { }
		public IdKeyword(string value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return StringFormat.Uri.Validate(Value)
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, "should be a valid URI");
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.String;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(IdKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Name == other.Name && Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as IdKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as IdKeyword);
		}
		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}
	}
}