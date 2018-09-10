using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class UniqueItemsKeyword : IJsonSchemaKeyword, IEquatable<UniqueItemsKeyword>
	{
		public virtual string Name => "uniqueItems";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public bool Value { get; private set; }

		public UniqueItemsKeyword() { }
		public UniqueItemsKeyword(bool value = true)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;
			if (context.Instance.Array.Distinct().Count() == context.Instance.Array.Count) return SchemaValidationResults.Valid;

			return new SchemaValidationResults(Name, SchemaErrorMessages.UniqueItems.ResolveTokens(new Dictionary<string, object>
				{
					["value"] = context.Instance
				}));
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
		public bool Equals(UniqueItemsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as UniqueItemsKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as UniqueItemsKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}