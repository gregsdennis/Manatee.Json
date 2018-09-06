using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class OneOfKeyword : List<JsonSchema>, IJsonSchemaKeyword, IEquatable<OneOfKeyword>
	{
		public virtual string Name => "oneOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			var results = this.Select(s =>
				{
					var newContext = new SchemaValidationContext
						{
							Instance = context.Instance,
							Root = context.Root
						};
					var result = s.Validate(newContext);
					context.EvaluatedPropertyNames.AddRange(newContext.EvaluatedPropertyNames);
					return result;
				}).ToList();
			var validCount = results.Count(r => r.IsValid);
			switch (validCount)
			{
				case 0:
					return new SchemaValidationResults(results);
				case 1:
					return new SchemaValidationResults();
				default:
					var message = SchemaErrorMessages.OneOf.ResolveTokens(new Dictionary<string, object>
						{
							["value"] = context.Instance
						});
					return new SchemaValidationResults(string.Empty, message);
			}
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(serializer.Deserialize<JsonSchema>));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.Select(serializer.Serialize).ToJson();
		}
		public bool Equals(OneOfKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as OneOfKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as OneOfKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}