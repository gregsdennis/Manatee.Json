using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[ExperimentalType]
	[FeedbackWelcome]
	public class JsonSchema : List<IJsonSchemaKeyword>, IJsonSchema
	{
		public Uri DocumentPath { get; set; }
		public string Id
		{
			get { return this.OfType<IdKeyword>().FirstOrDefault()?.Value; }
			set { }
		}
		public string Schema
		{
			get { return this.OfType<SchemaKeyword>().FirstOrDefault()?.Value; }
			set { }
		}

		public SchemaValidationResults ValidateSchema()
		{
			var errors = new List<string>();
			var supportedVersions = this.Aggregate(JsonSchemaVersion.All, (version, keyword) => version & keyword.SupportedVersions);
			if (supportedVersions == JsonSchemaVersion.None)
				errors.Add("The provided keywords do not support a common schema version.");

			var duplicateKeywords = this.GroupBy(k => k.Name)
			                            .Where(g => g.Count() > 1)
			                            .Select(g => g.Key)
			                            .ToList();
			if (duplicateKeywords.Any())
				errors.Add($"The following keywords have been entered more than once: {string.Join(", ", duplicateKeywords)}");

			return errors.Any()
				       ? new SchemaValidationResults(errors)
				       : SchemaValidationResults.Valid;
		}

		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			return new SchemaValidationResults(this.Select(k => k.Validate(this, json, null)));
		}
		public bool Equals(IJsonSchema other)
		{
			throw new NotImplementedException();
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return this.Select(k => new KeyValuePair<string, JsonValue>(k.Name, k.ToJson(serializer))).ToJson();
		}
	}

	public static class JsonSchemaStandards
	{
		public static JsonSchema Draft04 = new JsonSchema
			{
				new IdKeywordDraft04("http://json-schema.org/draft-04/schema#"),
				new SchemaKeywordDraft04("http://json-schema.org/draft-04/schema#"),
				new TypeKeyword(JsonSchemaType.Object),
			};
		public static JsonSchema Draft06 = new JsonSchema
			{
				new IdKeyword("http://json-schema.org/draft-06/schema#"),
				new SchemaKeyword("http://json-schema.org/draft-06/schema#"),
				new TypeKeyword(JsonSchemaType.Object | JsonSchemaType.Boolean),
			};

		public static JsonSchema Draft07 = new JsonSchema
			{
				new IdKeyword("http://json-schema.org/draft-07/schema#"),
				new SchemaKeyword("http://json-schema.org/draft-07/schema#"),
				new TypeKeyword(JsonSchemaType.Object | JsonSchemaType.Boolean),
			};

		public static JsonSchema Draft08 = new JsonSchema
			{
				new IdKeyword("http://json-schema.org/draft-08/schema#"),
				new SchemaKeyword("http://json-schema.org/draft-08/schema#"),
				new TypeKeyword(JsonSchemaType.Object | JsonSchemaType.Boolean),
			};

	}
}