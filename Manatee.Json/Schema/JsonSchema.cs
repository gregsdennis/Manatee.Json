using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[Experimental]
	[FeedbackWelcome]
	public class JsonSchema : List<IJsonSchemaKeyword>, IJsonSerializable, IEquatable<JsonSchema>
	{
		public static readonly JsonSchema Empty = new JsonSchema();
		public static readonly JsonSchema True = new JsonSchema(true);
		public static readonly JsonSchema False = new JsonSchema(false);

		private bool? _inherentValue;
		private Uri _documentPath;
		private bool _hasRegistered;

		public Uri DocumentPath
		{
			get => _documentPath ?? (_documentPath = Id == null ? null : new Uri(Id, UriKind.RelativeOrAbsolute));
			set => _documentPath = value;
		}
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
		public JsonObject OtherData { get; } = new JsonObject();

		public JsonSchema() { }
		public JsonSchema(bool value)
		{
			_inherentValue = value;
		}

		// TODO: This should be a SchemaEvaluationResults that reports supported schema versions and other metadata.
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

		public SchemaValidationResults Validate(JsonValue json)
		{
			return Validate(new SchemaValidationContext
				{
					Instance = json,
					Root = this
				});
		}
		public void RegisterSubschemas(Uri baseUri)
		{
			if (_hasRegistered) return;
			_hasRegistered = true;

			if (Uri.TryCreate(Id, UriKind.Absolute, out var uri))
			{
				JsonSchemaRegistry.Register(this);
				baseUri = uri;
			}

			foreach (var keyword in this)
			{
				keyword.RegisterSubschemas(baseUri);
			}
		}
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			var first = pointer.FirstOrDefault();
			if (first == null)
			{
				DocumentPath = DocumentPath != null ? new Uri(baseUri, DocumentPath) : baseUri;
				return this;
			}

			if (Uri.TryCreate(Id, UriKind.Absolute, out var uri))
				baseUri = uri;
			else if (Uri.TryCreate(Id, UriKind.Relative, out uri))
				baseUri = new Uri(baseUri, uri);

			var keyword = this.FirstOrDefault(k => k.Name == first);

			var keywordSchema = keyword?.ResolveSubschema(new JsonPointer(pointer.Skip(1)), baseUri);
			if (keywordSchema != null) return keywordSchema;

			var found = pointer.Evaluate(OtherData);
			if (found.Result == null) return null;

			var serializer = new JsonSerializer();
			var foundSchema = serializer.Deserialize<JsonSchema>(found.Result);

			return foundSchema;

		}

		internal SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (_inherentValue.HasValue)
			{
				if (_inherentValue.Value) return SchemaValidationResults.Valid;
				return new SchemaValidationResults(new[]{"All instances are invalid for the false schema."});
			}

			RegisterSubschemas(null);

			// TODO: Verify that $ref can work alongside other keywords (draft-08) and allow it if so.
			context.Local = this;

			if (context.BaseUri == null)
				context.BaseUri = DocumentPath;
			else if (DocumentPath != null)
				context.BaseUri = DocumentPath.IsAbsoluteUri
					? DocumentPath
					: new Uri(context.BaseUri, DocumentPath);

			if (Id != null && Uri.TryCreate(Id, UriKind.Absolute, out _))
				JsonSchemaRegistry.Register(this);

			var refKeyword = this.OfType<RefKeyword>().FirstOrDefault();
			if (refKeyword != null) return refKeyword.Validate(context);

			return new SchemaValidationResults(this.OrderBy(k => k.ValidationSequence)
				                                   .Select(k => k.Validate(context)));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Boolean)
			{
				_inherentValue = json.Boolean;
				return;
			}

			AddRange(json.Object.Select(kvp =>
					         {
								 var keyword = SchemaKeywordCatalog.Build(kvp.Key, kvp.Value, serializer);
						         if (keyword == null)
						         {
									 OtherData[kvp.Key] = kvp.Value;
						         }

						         return keyword;
					         })
				         .Where(k => k != null));
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (_inherentValue.HasValue) return _inherentValue;

			var obj = this.Select(k => new KeyValuePair<string, JsonValue>(k.Name, k.ToJson(serializer))).ToJson();

			if (OtherData != null)
			{
				foreach (var kvp in OtherData)
				{
					obj[kvp.Key] = kvp.Value;
				}
			}

			return obj;
		}

		public static implicit operator JsonSchema(bool value)
		{
			return value ? True : False;
		}
		public bool Equals(JsonSchema other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			var keywordMatch = this.LeftOuterJoin(other,
			                                      tk => tk.Name,
			                                      ok => ok.Name,
			                                      (tk, ok) => new {ThisKeyword = tk, OtherKeyword = ok})
				.ToList();

			return _inherentValue == other._inherentValue &&
			       Equals(OtherData, other.OtherData) &&
			       keywordMatch.All(k => Equals(k.ThisKeyword, k.OtherKeyword));
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as JsonSchema);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public static bool operator ==(JsonSchema left, JsonSchema right)
		{
			return Equals(left, right);
		}
		public static bool operator !=(JsonSchema left, JsonSchema right)
		{
			return !Equals(left, right);
		}
	}
}