using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}; Count={Count}")]
	public class PatternPropertiesKeyword : Dictionary<string, JsonSchema>, IJsonSchemaKeyword, IEquatable<PatternPropertiesKeyword>
	{
		public string Name => "patternProperties";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 2;

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var obj = context.Instance.Object;

			foreach (var patternProperty in this)
			{
				var pattern = new Regex(patternProperty.Key);
				var localSchema = patternProperty.Value;
				var matches = obj.Keys.Where(k => pattern.IsMatch(k));
				foreach (var match in matches)
				{
					context.EvaluatedPropertyNames.Add(match);
					var newContext = new SchemaValidationContext
						{
							BaseUri = context.BaseUri,
							Instance = obj[match],
							Root = context.Root
						};
					var result = localSchema.Validate(newContext);
					errors.AddRange(result.Errors.Select(e => new SchemaValidationError(match, e.Message)));
				}
			}

			return new SchemaValidationResults(errors);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			foreach (var kvp in json.Object)
			{
				this[kvp.Key] = serializer.Deserialize<JsonSchema>(kvp.Value);
			}
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.ToDictionary(kvp => kvp.Key,
			                         kvp => serializer.Serialize<JsonSchema>(kvp.Value))
				.ToJson();
		}
		public bool Equals(PatternPropertiesKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as PatternPropertiesKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as PatternPropertiesKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}