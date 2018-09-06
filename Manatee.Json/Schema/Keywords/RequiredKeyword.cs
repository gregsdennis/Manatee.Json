using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class RequiredKeyword : List<string>, IJsonSchemaKeyword, IEquatable<RequiredKeyword>
	{
		public virtual string Name => "required";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public RequiredKeyword() { }
		public RequiredKeyword(params string[] values)
			: base(values) { }
		public RequiredKeyword(IEnumerable<string> values)
			: base(values) { }

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var errors = new List<SchemaValidationError>();
			var obj = context.Instance.Object;
			foreach (var propertyName in this)
			{
				if (!obj.ContainsKey(propertyName))
				{
					var message = SchemaErrorMessages.Required.ResolveTokens(new Dictionary<string, object>
						{
							["property"] = propertyName,
							["value"] = context.Instance
					});
					errors.Add(new SchemaValidationError(propertyName, message));
				}
			}

			return new SchemaValidationResults(errors);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(jv => jv.String));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return LinqExtensions.ToJson(this);
		}
		public bool Equals(RequiredKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.ContentsEqual(other);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as RequiredKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as RequiredKeyword);
		}
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
	}
}