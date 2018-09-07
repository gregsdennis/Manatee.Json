using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class ContainsKeyword : IJsonSchemaKeyword, IEquatable<ContainsKeyword>
	{
		public string Name => "contains";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonSchema Value { get; private set; }

		public ContainsKeyword() { }
		public ContainsKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			var results = context.Instance.Array.Select(jv =>
				{
					var newContext = new SchemaValidationContext
						{
							Instance = jv,
							Root = context.Root
						};
					var result = Value.Validate(newContext);
					return result;
				}).ToList();
			if (results.Any(r => r.IsValid)) return new SchemaValidationResults();

			return new SchemaValidationResults(Name, SchemaErrorMessages.Contains.ResolveTokens(new Dictionary<string, object>
				{
					["expected"] = Value,
					["value"] = context.Instance
				}));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return serializer.Serialize(Value);
		}
		public bool Equals(ContainsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ContainsKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ContainsKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}