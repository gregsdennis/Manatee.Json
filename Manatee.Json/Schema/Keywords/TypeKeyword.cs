using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class TypeKeyword : IJsonSchemaKeyword, IEquatable<TypeKeyword>
	{
		public string Name => "type";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public JsonSchemaType Value { get; private set; }

		public TypeKeyword() { }
		public TypeKeyword(JsonSchemaType type)
		{
			Value = type;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			bool valid = true;
			switch (context.Instance.Type)
			{
				case JsonValueType.Number:
					if (Value.HasFlag(JsonSchemaType.Number)) break;
					if (context.Instance.Number.IsInt() && Value.HasFlag(JsonSchemaType.Integer)) break;
					valid = false;
					break;
				case JsonValueType.String:
					var expected = Value.ToJson();
					if (expected.Type == JsonValueType.String && expected == context.Instance) break;
					if (expected.Type == JsonValueType.Array && expected.Array.Contains(context.Instance)) break;
					if (Value.HasFlag(JsonSchemaType.String)) break;
					valid = false;
					break;
				case JsonValueType.Boolean:
					if (Value.HasFlag(JsonSchemaType.Boolean)) break;
					valid = false;
					break;
				case JsonValueType.Object:
					if (Value.HasFlag(JsonSchemaType.Object)) break;
					valid = false;
					break;
				case JsonValueType.Array:
					if (Value.HasFlag(JsonSchemaType.Array)) break;
					valid = false;
					break;
				case JsonValueType.Null:
					if (Value.HasFlag(JsonSchemaType.Null)) break;
					valid = false;
					break;
			}

			if (!valid)
			{
				var message = SchemaErrorMessages.Type.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = context.Instance.Type,
						["value"] = context.Instance
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Array)
			{
				var values = json.Array.Select(serializer.Deserialize<JsonSchemaType>);
				Value = values.Aggregate(JsonSchemaType.NotDefined, (current, i) => current | i);
			}
			else
				Value = serializer.Deserialize<JsonSchemaType>(json);
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.ToJson();
		}
		public bool Equals(TypeKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as TypeKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as TypeKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}