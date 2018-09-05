using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class EnumKeyword : List<JsonValue>, IJsonSchemaKeyword
	{
		public virtual string Name => "enum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public EnumKeyword() { }
		public EnumKeyword(params JsonValue[] values)
			: base(values) { }
		public EnumKeyword(IEnumerable<JsonValue> values)
			: base(values) { }

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			return Contains(context.Instance)
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, SchemaErrorMessages.Enum.ResolveTokens(new Dictionary<string, object>
					       {
						       ["value"] = context.Instance
					   }));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonArray(this);
		}
	}
}