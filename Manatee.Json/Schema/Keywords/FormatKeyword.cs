using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class FormatKeyword : IJsonSchemaKeyword, IEquatable<FormatKeyword>
	{
		public virtual string Name => "format";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public StringFormat Value { get; private set; }

		public FormatKeyword() { }
		public FormatKeyword(StringFormat value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.String) return SchemaValidationResults.Valid;

			var format = Value;
			if (!format.Validate(context.Instance.String))
			{
				var message = SchemaErrorMessages.Format.ResolveTokens(new Dictionary<string, object>
					{
						["format"] = format.Key,
						["value"] = context.Instance
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = StringFormat.GetFormat(json.String);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.Key;
		}
		public bool Equals(FormatKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as FormatKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as FormatKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}