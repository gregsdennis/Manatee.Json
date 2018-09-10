using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class ExclusiveMaximumKeyword : IJsonSchemaKeywordPlus, IEquatable<ExclusiveMaximumKeyword>
	{
		public string Name => "exclusiveMaximum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public double Value { get; private set; }

		public ExclusiveMaximumKeyword() { }
		public ExclusiveMaximumKeyword(double value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if (context.Instance.Number >= Value)
			{
				var message = SchemaErrorMessages.ExclusiveMaximum.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = context.Instance.Number,
						["value"] = context.Instance
				});

				return new SchemaValidationResults(Name, message);
			}

			return SchemaValidationResults.Valid;
		}
		bool IJsonSchemaKeywordPlus.Handles(JsonValue value)
		{
			return value.Type == JsonValueType.Number;
		}
		public void RegisterSubschemas(Uri baseUri) { }
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return null;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Number;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(ExclusiveMaximumKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ExclusiveMaximumKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExclusiveMaximumKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}