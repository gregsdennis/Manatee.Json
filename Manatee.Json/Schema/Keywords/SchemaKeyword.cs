﻿using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class SchemaKeyword : IJsonSchemaKeyword, IEquatable<SchemaKeyword>
	{
		public virtual string Name => "$schema";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public string Value { get; private set; }

		public SchemaKeyword() { }
		public SchemaKeyword(string value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return StringFormat.Uri.Validate(Value)
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, "should be a valid URI");
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.String;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
		public bool Equals(SchemaKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as SchemaKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as SchemaKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}