using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class PatternKeyword : IJsonSchemaKeyword, IEquatable<PatternKeyword>
	{
		public virtual string Name => "pattern";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public Regex Value { get; private set; }

		public PatternKeyword() { }
		public PatternKeyword(Regex value)
		{
			Value = value;
		}
		public PatternKeyword(string value)
		{
			Value = new Regex(value, RegexOptions.Compiled);
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.String) return SchemaValidationResults.Valid;

			if (!Value.IsMatch(context.Instance.String))
			{
				var message = SchemaErrorMessages.Pattern.ResolveTokens(new Dictionary<string, object>
					{
						["pattern"] = Value.ToString(),
						["value"] = context.Instance
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void RegisterSubschemas(Uri baseUri) { }
		public JsonSchema ResolveSubschema(JsonPointer pointer)
		{
			return null;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = new Regex(json.String, RegexOptions.Compiled);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.ToString();
		}
		public bool Equals(PatternKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as PatternKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as PatternKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}