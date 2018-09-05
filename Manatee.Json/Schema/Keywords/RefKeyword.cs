using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class RefKeyword : IJsonSchemaKeyword
	{
		public static readonly RefKeyword Root = new RefKeyword("#");

		public string Name => "$ref";
		public JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public string Reference { get; private set; }

		public JsonSchema Resolved { get; private set; }

		public RefKeyword() { }
		public RefKeyword(string reference)
		{
			Reference = reference;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			throw new NotImplementedException();
		}

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Reference = json.String;
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Reference;
		}
	}
}