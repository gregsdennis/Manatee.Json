﻿using System;
using System.Diagnostics;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class WriteOnlyKeyword : IJsonSchemaKeywordPlus, IEquatable<WriteOnlyKeyword>
	{
		public string Name => "exclusiveMinimum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public bool Value { get; private set; }

		public WriteOnlyKeyword() { }
		public WriteOnlyKeyword(bool value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
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
		public bool Equals(WriteOnlyKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as WriteOnlyKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as WriteOnlyKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}