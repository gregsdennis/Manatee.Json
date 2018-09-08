using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class AllOfKeyword : List<JsonSchema>, IJsonSchemaKeyword, IEquatable<AllOfKeyword>
	{
		public virtual string Name => "allOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return new SchemaValidationResults(this.Select((s, i) =>
				{
					var newContext = new SchemaValidationContext
						{
							BaseUri = context.BaseUri,
							Instance = context.Instance,
							Root = context.Root
						};
					var result = s.Validate(newContext);
					context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
					return result;
				}));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(serializer.Deserialize<JsonSchema>));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var array = this.Select(serializer.Serialize).ToJson();
			array.EqualityStandard = ArrayEquality.ContentsEqual;

			return array;
		}
		public bool Equals(AllOfKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as AllOfKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as AllOfKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}