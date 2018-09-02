using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[ExperimentalType]
	[FeedbackWelcome]
	public class JsonSchema : List<JsonSchemaKeyword>
	{
		public static readonly JsonSchema True = new JsonSchema(true);
		public static readonly JsonSchema False = new JsonSchema(false);

		private readonly bool? _inherentValue;

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

		public JsonSchema() { }
		public JsonSchema(bool value)
		{
			_inherentValue = value;
		}

		// TODO: This should be a SchemaEvaluationResults that reports supported schema versions and other metadata.
		public SchemaValidationResults ValidateSchema()
		{
			if (_inherentValue.HasValue)
			{
				if (_inherentValue.Value) return SchemaValidationResults.Valid;
				return new SchemaValidationResults(new[]{"All instances are invalid for the false schema."});
			}

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

		public SchemaValidationResults Validate(JsonValue json, JsonSchema root = null)
		{
			return new SchemaValidationResults(this.Select(k => k.Validate(this, null, json)));
		}
		public bool Equals(JsonSchema other)
		{
			throw new NotImplementedException();
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (_inherentValue.HasValue) return _inherentValue;

			return this.Select(k => new KeyValuePair<string, JsonValue>(k.Name, k.ToJson(serializer))).ToJson();
		}
	}
}