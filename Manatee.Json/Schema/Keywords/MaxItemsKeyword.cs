using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class MaxItemsKeyword : IJsonSchemaKeyword, IEquatable<MaxItemsKeyword>
	{
		public string Name => "maxItems";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public uint Value { get; private set; }

		public MaxItemsKeyword() { }
		public MaxItemsKeyword(uint value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			if (context.Instance.Array.Count > Value)
			{
				var message = SchemaErrorMessages.MaxItems.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = context.Instance.Array.Count,
						["value"] = context.Instance
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = (uint) json.Number;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(MaxItemsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as MaxItemsKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as MaxItemsKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}