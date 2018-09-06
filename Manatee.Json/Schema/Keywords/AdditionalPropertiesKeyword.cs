using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AdditionalPropertiesKeyword : IJsonSchemaKeyword, IEquatable<AdditionalPropertiesKeyword>
	{
		public virtual string Name => "additionalProperties";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 3;

		public JsonSchema Value { get; private set; }

		public AdditionalPropertiesKeyword() { }
		public AdditionalPropertiesKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var obj = context.Instance.Object;
			var toEvaluate = obj.Where(kvp => !context.EvaluatedPropertyNames.Contains(kvp.Key)).ToJson();

			var errors = new List<SchemaValidationError>();

			if (Equals(Value, JsonSchema.False) && toEvaluate.Any())
				errors.AddRange(toEvaluate.Keys.Select(k =>
					{
						var message = SchemaErrorMessages.AdditionalProperties_False.ResolveTokens(new Dictionary<string, object>
							{
								["property"] = k,
								["value"] = context.Instance
						});
						return new SchemaValidationError(k, message);
					}));
			else
			{
				foreach (var key in toEvaluate.Keys)
				{
					var newContext = new SchemaValidationContext
						{
							Instance = toEvaluate[key],
							Root = context.Root
						};
					var extraErrors = Value.Validate(newContext).Errors;
					errors.AddRange(extraErrors.Select(e => e.PrependPropertySegment(key)));
				}
			}

			return new SchemaValidationResults(errors);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.ToJson(serializer);
		}
		public bool Equals(AdditionalPropertiesKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			throw new NotImplementedException();
		}
		public override bool Equals(object obj)
		{
			return Equals((AdditionalPropertiesKeyword) obj);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}