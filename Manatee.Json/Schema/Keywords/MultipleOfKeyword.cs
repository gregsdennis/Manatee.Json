using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class MultipleOfKeyword : IJsonSchemaKeyword, IEquatable<MultipleOfKeyword>
	{
		public string Name => "multipleOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public double Value { get; private set; }

		public MultipleOfKeyword() { }
		public MultipleOfKeyword(double value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if ((decimal)context.Instance.Number % (decimal?) Value != 0)
			{
				var message = SchemaErrorMessages.MultipleOf.ResolveTokens(new Dictionary<string, object>
					{
						["multipleOf"] = Value,
						["actual"] = context.Instance.Number % Value,
						["value"] = context.Instance
				});

				return new SchemaValidationResults(Name, message);
			}

			return SchemaValidationResults.Valid;
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
		public bool Equals(MultipleOfKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as MultipleOfKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as MultipleOfKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}