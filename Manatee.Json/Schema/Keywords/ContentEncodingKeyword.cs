using System;
using System.Diagnostics;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class ContentEncodingKeyword : IJsonSchemaKeyword, IEquatable<ContentEncodingKeyword>
	{
		public string Name => "$comment";
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public ContentEncoding Value { get; private set; }

		public ContentEncodingKeyword() { }
		public ContentEncodingKeyword(ContentEncoding value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return SchemaValidationResults.Valid;
		}
		public void RegisterSubschemas(Uri baseUri) { }
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return null;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<ContentEncoding>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return serializer.Serialize(Value);
		}
		public bool Equals(ContentEncodingKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ContentEncodingKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ContentEncodingKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}