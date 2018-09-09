using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class AdditionalItemsKeyword : IJsonSchemaKeyword, IEquatable<AdditionalItemsKeyword>
	{
		public string Name => "additionalItems";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public JsonSchema Value { get; private set; }

		public AdditionalItemsKeyword() { }
		public AdditionalItemsKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			var itemsKeyword = context.Local.OfType<ItemsKeyword>().FirstOrDefault();
			if (itemsKeyword == null || !itemsKeyword.IsArray) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var array = context.Instance.Array;

			if (itemsKeyword.Count < array.Count)
			{
				if (Equals(Value, JsonSchema.False))
				{
					var message = SchemaErrorMessages.Items.ResolveTokens(new Dictionary<string, object>
						{
							["value"] = context.Instance
						});
					errors.Add(new SchemaValidationError(string.Empty, message));
				}
				else if (!Equals(Value, JsonSchema.True))
				{
					errors.AddRange(array.Skip(itemsKeyword.Count).SelectMany(j =>
						{
							var newContext = new SchemaValidationContext
								{
									BaseUri = context.BaseUri,
									Instance = j,
									Root = context.Root
								};
							return Value.Validate(newContext).Errors;
						}));}
			}

			return new SchemaValidationResults(errors);
		}
		public void RegisterSubschemas(Uri baseUri)
		{
			Value.RegisterSubschemas(baseUri);
		}
		public JsonSchema ResolveSubschema(JsonPointer pointer)
		{
			return Value.ResolveSubschema(pointer);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return serializer.Serialize(Value);
		}
		public bool Equals(AdditionalItemsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as AdditionalItemsKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as AdditionalItemsKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}