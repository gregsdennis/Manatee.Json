using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class RequiredKeyword : List<string>, IJsonSchemaKeyword
	{
		public virtual string Name => "required";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public RequiredKeyword() { }
		public RequiredKeyword(params string[] values)
			: base(values) { }
		public RequiredKeyword(IEnumerable<string> values)
			: base(values) { }

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.Array.Select(jv => jv.String));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return LinqExtensions.ToJson(this);
		}
	}
}