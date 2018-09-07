using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class ExclusiveMaximumDraft04Keyword : IJsonSchemaKeywordPlus, IEquatable<ExclusiveMaximumDraft04Keyword>
	{
		public string Name => "exclusiveMaximum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;
		public int ValidationSequence => 1;

		public bool Value { get; private set; }

		public ExclusiveMaximumDraft04Keyword() { }
		public ExclusiveMaximumDraft04Keyword(bool value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			var keyword = context.Local.OfType<MaximumKeyword>().FirstOrDefault();
			if (keyword == null) return SchemaValidationResults.Valid;

			if (!Value) return SchemaValidationResults.Valid;
			
			if (context.Instance.Number >= keyword.Value)
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
			return value.Type == JsonValueType.Boolean;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Boolean;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(ExclusiveMaximumDraft04Keyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ExclusiveMaximumDraft04Keyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExclusiveMaximumDraft04Keyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}