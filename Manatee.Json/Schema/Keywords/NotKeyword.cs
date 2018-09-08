using System;
using System.Collections.Generic;
using System.Diagnostics;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class NotKeyword : IJsonSchemaKeyword, IEquatable<NotKeyword>
	{
		public virtual string Name => "not";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public JsonSchema Value { get; private set; }

		public NotKeyword() { }
		public NotKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var newContext = new SchemaValidationContext
				{
					BaseUri = context.BaseUri,
					Instance = context.Instance,
					Root = context.Root
				};
			var results = Value.Validate(newContext);
			context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
			if (results.IsValid)
			{
				var message = SchemaErrorMessages.Not.ResolveTokens(new Dictionary<string, object>
					{
						["value"] = context.Instance
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return serializer.Serialize(Value);
		}
		public bool Equals(NotKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as NotKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as NotKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}