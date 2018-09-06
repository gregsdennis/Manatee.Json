using System;
using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class MinimumKeyword : IJsonSchemaKeyword, IEquatable<MinimumKeyword>
	{
		public string Name => "minimum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public double Value { get; private set; }

		public MinimumKeyword() { }
		public MinimumKeyword(double value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if (context.Instance.Number < Value)
			{
				var message = SchemaErrorMessages.Minimum.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = context.Instance.Number,
						["value"] = context.Instance
				});

				return new SchemaValidationResults(Name, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Number;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(MinimumKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as MinimumKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as MinimumKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}