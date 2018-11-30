using System;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class RecursiveAnchorKeyword : IJsonSchemaKeyword, IEquatable<RecursiveAnchorKeyword>
	{
		public string Name => "$recursiveAnchor";
		public JsonSchemaVersion SupportedVersions => JsonSchemaVersion.Draft08;
		public int ValidationSequence => 0;

		public bool Value { get; set; }

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			context.RecursiveRoot.Push(context.Local);
			
			return SchemaValidationResults.Null;
		}
		public void RegisterSubschemas(Uri baseUri) { }
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return null;
		}

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Boolean;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}

		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as RecursiveAnchorKeyword);
		}
		public bool Equals(RecursiveAnchorKeyword other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as RecursiveAnchorKeyword);
		}
		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}
	}
}
