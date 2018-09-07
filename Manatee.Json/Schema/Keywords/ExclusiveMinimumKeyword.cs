using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class ExclusiveMinimumKeyword : IJsonSchemaKeywordPlus, IEquatable<ExclusiveMinimumKeyword>
	{
		public string Name => "exclusiveMinimum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public double Value { get; private set; }

		public ExclusiveMinimumKeyword() { }
		public ExclusiveMinimumKeyword(double value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if (context.Instance.Number <= Value)
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
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Number;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(ExclusiveMinimumKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ExclusiveMinimumKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExclusiveMinimumKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}