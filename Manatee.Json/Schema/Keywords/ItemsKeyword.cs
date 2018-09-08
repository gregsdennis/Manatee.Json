using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class ItemsKeyword : List<JsonSchema>, IJsonSchemaKeyword, IEquatable<ItemsKeyword>
	{
		public string Name => "items";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public bool IsArray { get; private set; }

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var array = context.Instance.Array;
			if (IsArray)
			{
				// have array of schemata: validate in sequence
				var i = 0;
				while (i < array.Count && i < Count)
				{
					var newContext = new SchemaValidationContext
						{
							BaseUri = context.BaseUri,
							Instance = array[i],
							Root = context.Root
						};
					errors.AddRange(this[i].Validate(newContext).Errors);
					i++;
				}
			}
			else
			{
				// have single schema: validate all against this
				var itemValidations = array.Select(jv =>
					{
						var newContext = new SchemaValidationContext
							{
								BaseUri = context.BaseUri,
								Instance = jv,
								Root = context.Root
							};
						return this[0].Validate(newContext);
					});
				errors.AddRange(itemValidations.SelectMany((v, i) => v.Errors.Select(e => e.PrependPropertySegment($"{i}"))));
			}

			return new SchemaValidationResults(errors);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Array)
			{
				AddRange(serializer.Deserialize<List<JsonSchema>>(json));
				IsArray = true;
			}
			else
				Add(serializer.Deserialize<JsonSchema>(json));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (IsArray)
			{
				var schemas = this.Select(serializer.Serialize).ToJson();
				schemas.EqualityStandard = ArrayEquality.SequenceEqual;

				return schemas;
			}

			return serializer.Serialize(this[0]);
		}
		public bool Equals(ItemsKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as ItemsKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ItemsKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}