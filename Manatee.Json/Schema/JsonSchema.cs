using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Encapsulates and models JSON Schema.
	/// </summary>
	public class JsonSchema : List<IJsonSchemaKeyword>, IJsonSerializable, IEquatable<JsonSchema>
	{
		/// <summary>
		/// Gets or sets the error message template used for `false` schemas.
		/// </summary>
		/// <remarks>
		/// Does not supports any tokens.
		/// </remarks>
		public static string ErrorTemplate { get; set; } = "No value is valid against the false schema.";

		/// <summary>
		/// Defines the empty schema.  Analogous to <see cref="True"/>.
		/// </summary>
		public static readonly JsonSchema Empty = new JsonSchema();
		/// <summary>
		/// Defines the True schema.  Validates all JSON instances.
		/// </summary>
		public static readonly JsonSchema True = new JsonSchema(true);
		/// <summary>
		/// Defines the False schema.  Validates no JSON instances.
		/// </summary>
		public static readonly JsonSchema False = new JsonSchema(false);

		private bool? _inherentValue;
		private Uri? _documentPath;
		private bool _hasRegistered;
		private MetaSchemaValidationResults? _metaSchemaResults;

		/// <summary>
		/// Defines the document path.  If not explicitly provided, it will be derived from the <see cref="Id"/> property.
		/// </summary>
		public Uri? DocumentPath
		{
			get => _documentPath ??= _BuildDocumentPath();
			set => _documentPath = value;
		}
		/// <summary>
		/// Gets the `$id` (or `id` for draft-04) property value, if declared.
		/// </summary>
		public string? Id => this.Get<IdKeyword?>()?.Value;
		/// <summary>
		/// Gets the `$schema` property, if declared.
		/// </summary>
		public string? Schema => this.Get<SchemaKeyword?>()?.Value;
		/// <summary>
		/// 
		/// </summary>
		public JsonSchemaVersion SupportedVersions
		{
			get
			{
				_metaSchemaResults ??= ValidateSchema();
				return _metaSchemaResults.SupportedVersions;
			}
			internal set
			{
				_metaSchemaResults = new MetaSchemaValidationResults {SupportedVersions = value};
			}
		}
		/// <summary>
		/// Gets other data that may be present in the schema but unrelated to any known keywords.
		/// </summary>
		public JsonObject OtherData { get; set; } = new JsonObject();

		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchema"/> class.
		/// </summary>
		public JsonSchema() { }
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchema"/> class, explicitly set to a true/false value.
		/// </summary>
		public JsonSchema(bool value)
		{
			_inherentValue = value;
		}

		/// <summary>
		/// Validates that the schema object represents a valid schema in accordance with a known meta-schema.
		/// </summary>
		/// <returns>Validation results.</returns>
		public MetaSchemaValidationResults ValidateSchema()
		{
			if (_metaSchemaResults != null) return _metaSchemaResults;

			var results = new MetaSchemaValidationResults();

			JsonSchemaVersion startVersion;
			if (Schema == MetaSchemas.Draft04.Id)
				startVersion = JsonSchemaVersion.Draft04;
			else if (Schema == MetaSchemas.Draft06.Id)
				startVersion = JsonSchemaVersion.Draft06;
			else if (Schema == MetaSchemas.Draft07.Id)
				startVersion = JsonSchemaVersion.Draft07;
			else if (Schema == MetaSchemas.Draft2019_09.Id)
				startVersion = JsonSchemaVersion.Draft2019_09;
			else
			{
				startVersion = JsonSchemaVersion.All;
				var schemaDeclaration = Schema;
				if (!string.IsNullOrEmpty(schemaDeclaration))
				{
					var metaSchema = JsonSchemaRegistry.Get(schemaDeclaration);
					if (metaSchema != null)
					{
						var asJson = ToJson(new JsonSerializer());
						var context = new SchemaValidationContext(metaSchema, asJson, new JsonPointer("#"), new JsonPointer("#"), new JsonPointer("#"))
							{
								IsMetaSchemaValidation = true
							};
						var metaValidation = metaSchema.Validate(context);
						results.MetaSchemaValidations[schemaDeclaration] = metaValidation;
					}
				}
			}

			var supportedVersions = this.Aggregate(startVersion, (version, keyword) => version & keyword.SupportedVersions);
			if (supportedVersions == JsonSchemaVersion.None)
				results.OtherErrors.Add("The provided keywords do not support a common schema version.");
			else
			{
				var asJson = ToJson(new JsonSerializer());
				var context = new SchemaValidationContext(this, asJson, new JsonPointer("#"), new JsonPointer("#"), new JsonPointer("#"))
					{
						IsMetaSchemaValidation = true
					};
				if (supportedVersions.HasFlag(JsonSchemaVersion.Draft04))
				{
					context.Root = MetaSchemas.Draft04;
					var metaValidation = MetaSchemas.Draft04.Validate(context);
					results.MetaSchemaValidations[MetaSchemas.Draft04.Id!] = metaValidation;
					if (metaValidation.IsValid)
						results.SupportedVersions |= JsonSchemaVersion.Draft04;
				}
				if (supportedVersions.HasFlag(JsonSchemaVersion.Draft06))
				{
					context.Root = MetaSchemas.Draft06;
					var metaValidation = MetaSchemas.Draft06.Validate(context);
					results.MetaSchemaValidations[MetaSchemas.Draft06.Id!] = metaValidation;
					if (metaValidation.IsValid)
						results.SupportedVersions |= JsonSchemaVersion.Draft06;
				}
				if (supportedVersions.HasFlag(JsonSchemaVersion.Draft07))
				{
					context.Root = MetaSchemas.Draft07;
					var metaValidation = MetaSchemas.Draft07.Validate(context);
					results.MetaSchemaValidations[MetaSchemas.Draft07.Id!] = metaValidation;
					if (metaValidation.IsValid)
						results.SupportedVersions |= JsonSchemaVersion.Draft07;
				}
				if (supportedVersions.HasFlag(JsonSchemaVersion.Draft2019_09))
				{
					context.Root = MetaSchemas.Draft2019_09;
					var metaValidation = MetaSchemas.Draft2019_09.Validate(context);
					results.MetaSchemaValidations[MetaSchemas.Draft2019_09.Id!] = metaValidation;
					if (metaValidation.IsValid)
						results.SupportedVersions |= JsonSchemaVersion.Draft2019_09;
				}
			}

			results.SupportedVersions = supportedVersions;

			var duplicateKeywords = this.GroupBy(k => k.Name)
			                            .Where(g => g.Count() > 1)
			                            .Select(g => g.Key)
			                            .ToList();
			if (duplicateKeywords.Any())
				results.OtherErrors.Add($"The following keywords have been entered more than once: {string.Join(", ", duplicateKeywords)}");

			_metaSchemaResults = results;

			return results;
		}
		/// <summary>
		/// Provides the validation logic for this keyword.
		/// </summary>
		/// <param name="json">The instance to validate.</param>
		/// <returns>Results object containing a final result and any errors that may have been found.</returns>
		public SchemaValidationResults Validate(JsonValue json)
		{
			var results = Validate(new SchemaValidationContext(this, json, new JsonPointer("#"), new JsonPointer("#"), new JsonPointer("#")));

			switch (JsonSchemaOptions.OutputFormat)
			{
				case SchemaValidationOutputFormat.Flag:
					results.AdditionalInfo = new JsonObject();
					results.RelativeLocation = null!;
					results.AbsoluteLocation = null;
					results.InstanceLocation = null!;
					results.NestedResults = new List<SchemaValidationResults>();
					break;
				case SchemaValidationOutputFormat.Basic:
					results = results.Flatten();
					break;
				case SchemaValidationOutputFormat.Detailed:
					results = results.Condense();
					break;
				case SchemaValidationOutputFormat.Verbose:
					break;
			}

			return results;
		}
		/// <summary>
		/// Used register any subschemas during validation.  Enables look-forward compatibility with `$ref` keywords.
		/// </summary>
		/// <param name="baseUri">The current base URI</param>
		/// <param name="localRegistry"></param>
		public void RegisterSubschemas(Uri? baseUri, JsonSchemaRegistry localRegistry)
		{
			if (_hasRegistered) return;
			_hasRegistered = true;

			localRegistry.RegisterLocal(this);

			var address = Id;
			if (baseUri != null && address != null)
				address = new Uri(baseUri, address).OriginalString;
			if (_documentPath == null && address != null && !address.StartsWith("#"))
			{
				if (!Uri.TryCreate(address, UriKind.Absolute, out var uri))
					uri = new Uri(JsonSchemaOptions.DefaultBaseUri, address);
				DocumentPath = uri;
				JsonSchemaRegistry.Register(this);
				baseUri = uri;
			}

			foreach (var keyword in this)
			{
				keyword.RegisterSubschemas(baseUri, localRegistry);
			}
		}
		/// <summary>
		/// Resolves any subschemas during resolution of a `$ref` during validation.
		/// </summary>
		/// <param name="pointer">A <see cref="JsonPointer"/> to the target schema.</param>
		/// <param name="baseUri">The current base URI.</param>
		/// <returns>The referenced schema, if it exists; otherwise null.</returns>
		public JsonSchema? ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			var first = pointer.FirstOrDefault<string?>();
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
			Log.Schema(() => $"Begin validation of {context.InstanceLocation} by {context.RelativeLocation}");
			if (_inherentValue.HasValue)
			{
				if (_inherentValue.Value)
				{
					Log.Schema(() => "`true` schema; all instances valid");
					return new SchemaValidationResults(context);
				}
				Log.Schema(() => "`false` schema; all instances invalid");
				return new SchemaValidationResults(context)
					{
						IsValid = false,
						ErrorMessage = ErrorTemplate
					};
			}

			RegisterSubschemas(null, context.LocalRegistry);
			context.LocalRegistry.RegisterLocal(this);

			context.Local = this;

			var refKeyword = this.Get<RefKeyword?>();
			if (refKeyword == null ||
			    JsonSchemaOptions.RefResolution == RefResolutionStrategy.ProcessSiblingKeywords ||
			    context.Root.SupportedVersions == JsonSchemaVersion.Draft2019_09)
			{
				if (context.BaseUri == null)
					context.BaseUri = DocumentPath;
				else if (DocumentPath != null)
				{
					if (DocumentPath.IsAbsoluteUri)
						context.BaseUri = DocumentPath;
					else
						context.BaseUri = new Uri(context.BaseUri, DocumentPath);
				}
			}

			if (context.BaseUri != null && context.BaseUri.OriginalString.EndsWith("#"))
				context.BaseUri = new Uri(context.BaseUri.OriginalString.TrimEnd('#'), UriKind.RelativeOrAbsolute);

			if (refKeyword != null && !context.Root.SupportedVersions.HasFlag(JsonSchemaVersion.Draft2019_09))
				return refKeyword.Validate(context);

			var nestedResults = new List<SchemaValidationResults>();

			foreach (var keyword in this.OrderBy(k => k.ValidationSequence))
			{
				Log.Schema(() => $"Processing `{keyword.Name}`");
				var localResults = keyword.Validate(context);
				Log.Schema(() => $"`{keyword.Name}` complete: {(localResults.IsValid ? "valid" : "invalid")}");
				if (JsonSchemaOptions.OutputFormat == SchemaValidationOutputFormat.Flag && !localResults.IsValid)
				{
					Log.Schema(() => "Found fail condition with flag output enabled; halting validation early.");
					return new SchemaValidationResults {IsValid = false};
				}
				nestedResults.Add(localResults);
			}

			var results = new SchemaValidationResults(context);
			if (nestedResults.Any(r => !r.IsValid))
				results.IsValid = false;

			results.NestedResults = nestedResults;

			Log.Schema(() => $"Validation of {context.InstanceLocation} by {context.RelativeLocation} complete: {(results.IsValid ? "valid" : "invalid")}");
			return results;
		}

		private Uri? _BuildDocumentPath()
		{
			return Id != null
				? new Uri(Id, UriKind.RelativeOrAbsolute)
				: null;
		}

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Boolean)
			{
				_inherentValue = json.Boolean;
				return;
			}

			var toAdd = new List<IJsonSchemaKeyword>();

			foreach (var kvp in json.Object)
			{
				var keyword = SchemaKeywordCatalog.Build(kvp.Key, kvp.Value, serializer);
				if (keyword != null)
					toAdd.Add(keyword);
				else
					OtherData[kvp.Key] = kvp.Value;
			}

			AddRange(toAdd);
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (_inherentValue.HasValue) return _inherentValue;

			var obj = this.Select(k => new KeyValuePair<string, JsonValue?>(k.Name, k.ToJson(serializer))).ToJson();

			if (OtherData != null)
			{
				foreach (var kvp in OtherData)
				{
					obj[kvp.Key] = kvp.Value;
				}
			}

			return obj;
		}

		/// <summary>
		/// Implicitly converts a boolean into a boolean schema.
		/// </summary>
		public static implicit operator JsonSchema(bool value)
		{
			return value ? True : False;
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(JsonSchema? other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;

			var keywordMatch = this.FullOuterJoin(other,
			                                      tk => tk.Name,
			                                      ok => ok.Name,
			                                      (tk, ok) => new {ThisKeyword = tk, OtherKeyword = ok})
				.ToList();

			return _inherentValue == other._inherentValue &&
			       Equals(OtherData, other.OtherData) &&
			       keywordMatch.All(k =>
				       {
					       var equals = Equals(k.ThisKeyword, k.OtherKeyword);
					       return equals;
				       });
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object? obj)
		{
			return Equals(obj as JsonSchema);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.GetCollectionHashCode();
		}
		/// <summary>
		/// Overloads the equals operator for <see cref="JsonSchema"/>.
		/// </summary>
		/// <returns>true if the two values represent the same schema; false otherwise</returns>
		public static bool operator ==(JsonSchema? left, JsonSchema? right)
		{
			return Equals(left, right);
		}
		/// <summary>
		/// Overloads the not-equal operator for <see cref="JsonSchema"/>.
		/// </summary>
		/// <returns>false if the two values represent the same schema; true otherwise</returns>
		public static bool operator !=(JsonSchema? left, JsonSchema? right)
		{
			return !Equals(left, right);
		}
	}
}