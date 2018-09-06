using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ExclusiveMinimumDraft04Keyword : IJsonSchemaKeywordPlus, IEquatable<ExclusiveMinimumDraft04Keyword>
	{
		public string Name => "exclusiveMinimum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;
		public int ValidationSequence => 1;

		public bool Value { get; private set; }

		public ExclusiveMinimumDraft04Keyword() { }
		public ExclusiveMinimumDraft04Keyword(bool value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			var keyword = context.Local.OfType<MinimumKeyword>().FirstOrDefault();
			if (keyword == null) return SchemaValidationResults.Valid;

			if (!Value) return SchemaValidationResults.Valid;

			if (context.Instance.Number <= keyword.Value)
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
		public bool Equals(ExclusiveMinimumDraft04Keyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ExclusiveMinimumDraft04Keyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExclusiveMinimumDraft04Keyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}