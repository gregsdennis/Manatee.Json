using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines options associated with JSON Schema.
	/// </summary>
	public class JsonSchemaOptions
	{
		private interface IErrorCollectionCondition
		{
			bool ShouldExcludeChildErrors(IJsonSchemaKeyword keyword, SchemaValidationContext context);
		}

		private class LocationErrorCollectionCondition : IErrorCollectionCondition
		{
			public JsonPointer Location { get; }

			public LocationErrorCollectionCondition(JsonPointer location)
			{
				Location = location;
			}

			public bool ShouldExcludeChildErrors(IJsonSchemaKeyword keyword, SchemaValidationContext context)
			{
				return context.RelativeLocation.IsChildOf(Location);
			}
		}

		private class KeywordErrorCollectionCondition : IErrorCollectionCondition
		{
			public Type Type { get; }

			public KeywordErrorCollectionCondition(Type type)
			{
				Type = type;
			}

			public bool ShouldExcludeChildErrors(IJsonSchemaKeyword keyword, SchemaValidationContext context)
			{
				return Type.IsInstanceOfType(keyword);
			}
		}

		private readonly List<IErrorCollectionCondition> _errorCollectionConditions;
		private static Func<string, string>? _download;

		/// <summary>
		/// Default options used by schema.
		/// </summary>
		public static JsonSchemaOptions Default { get; } = new JsonSchemaOptions();

		/// <summary>
		/// Gets and sets a method used to download online schema.
		/// </summary>
		public static Func<string, string> Download
		{
			get { return _download ??= _BasicDownload; }
			set { _download = value; }
		}

		/// <summary>
		/// Gets or sets whether the "format" schema keyword should be validated.  The default is true.
		/// </summary>
		public bool ValidateFormatKeyword { get; set; } = true;

		/// <summary>
		/// Gets or sets whether unknown string formats are permitted.  If disabled and an unknown format
		/// is found, the system will throw a <see cref="JsonSerializationException"/> while loading the schema.
		/// The default is true.
		/// </summary>
		public bool AllowUnknownFormats { get; set; } = true;

		/// <summary>
		/// Gets or sets the output verbosity.  The default is <see cref="SchemaValidationOutputFormat.Flag"/>.
		/// </summary>
		public SchemaValidationOutputFormat OutputFormat { get; set; } = SchemaValidationOutputFormat.Flag;

		/// <summary>
		/// Determines whether siblings of `$ref` keywords are processed.  This also affects how `$ref` is resolved
		/// when adjacent to an `$id` keyword when a specific draft cannot be identified.  The default is
		/// <see cref="RefResolutionStrategy.ProcessSiblingKeywords"/> to be consistent with the latest draft, 2019-09.
		/// </summary>
		/// <remarks>
		/// As of draft 2019-09, keywords are allowed to be adjacent to `$ref`.  This means that an
		/// adjacent `$id` keyword will now change the base URI whereas in prior drafts it would not
		/// since adjacent keywords were to be ignored.
		///
		/// When Manatee.Json cannot determine the draft a particular schema is using (determined via
		/// the `$schema` keyword or the selection of keywords being used), this option will
		/// determine the behavior for resolving URIs.
		/// </remarks>
		public RefResolutionStrategy RefResolution { get; set; } = RefResolutionStrategy.ProcessSiblingKeywords;

		/// <summary>
		/// Defines a default base URI for root schemas that use a relative URI for their `$id`.  The default is `manatee://json-schema/`.
		/// </summary>
		public Uri DefaultBaseUri { get; set; } = new Uri("manatee://json-schema/", UriKind.Absolute);

		/// <summary>
		/// Gets or sets whether meta-schema validation is enabled. Default is false.
		/// </summary>
		/// <remarks>
		/// A schema is validated against the meta-schemas the first time it is used to validate an instance.
		/// This is done to determine the specific draft that the schema supports.  Logging for this can be
		/// quite verbose, but if you want this to be logged, you can enable this feature.
		/// </remarks>
		public bool LogMetaSchemaValidation { get; set; }

		/// <summary>
		/// Gets or sets a default draft version to use in case a schema can be determined to support
		/// multiple drafts.  Default is latest to earliest.
		/// </summary>
		public List<JsonSchemaVersion> DefaultProcessingVersion { get; set; } =
			new List<JsonSchemaVersion>
				{
					JsonSchemaVersion.Draft2019_09,
					JsonSchemaVersion.Draft07,
					JsonSchemaVersion.Draft06,
					JsonSchemaVersion.Draft04
				};

		internal bool ConfigureForTestOutput { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaOptions"/> class.
		/// </summary>
		public JsonSchemaOptions()
		{
			_errorCollectionConditions = new List<IErrorCollectionCondition>();
		}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaOptions"/> class.
		/// </summary>
		public JsonSchemaOptions(JsonSchemaOptions source)
		{
			_errorCollectionConditions = source._errorCollectionConditions.ToList();
			ValidateFormatKeyword = source.ValidateFormatKeyword;
			AllowUnknownFormats = source.AllowUnknownFormats;
			OutputFormat = source.OutputFormat;
			RefResolution = source.RefResolution;
			DefaultBaseUri = source.DefaultBaseUri;
			LogMetaSchemaValidation = source.LogMetaSchemaValidation;
			DefaultProcessingVersion = source.DefaultProcessingVersion;
			ConfigureForTestOutput = source.ConfigureForTestOutput;
		}

		/// <summary>
		/// Ignores error and annotation collection for children of specific keywords.
		/// </summary>
		/// <typeparam name="T">The keyword type to ignore.</typeparam>
		/// <remarks>
		///	This may help improve performance.  There may be cases where it would be sufficient to only
		/// report on the immediate error rather than all child errors.  An example of this may be a keyword
		/// like `oneOf`, where the client may only want a single error that says, "4 of the 10 subschemas
		/// passed validation, but only 1 was expected."
		/// </remarks>
		public void IgnoreErrorsForChildren<T>()
			where T : IJsonSchemaKeyword
		{
			_errorCollectionConditions.Add(new KeywordErrorCollectionCondition(typeof(T)));
		}

		/// <summary>
		/// Ignores error and annotation collection for children of specific schema locations.
		/// </summary>
		/// <remarks>
		///	This may help improve performance.  There may be cases where it would be sufficient to only
		/// report on the immediate error rather than all child errors.  An example of this may be a keyword
		/// like `oneOf` at a specific location, where the client may only want a single error that says,
		/// "4 of the 10 subschemas passed validation, but only 1 was expected."
		/// </remarks>
		public void IgnoreErrorsForChildren(JsonPointer location)
		{
			_errorCollectionConditions.Add(new LocationErrorCollectionCondition(location));
		}

		/// <summary>
		/// Checks whether child errors and annotations should be reported.  Should be called during validation.
		/// </summary>
		/// <param name="keyword">The keyword currently executing validation.</param>
		/// <param name="context">The validation context.</param>
		/// <returns>`true` if child errors should be included in the error; `false` if the result should only
		/// contain the immediate error.</returns>
		public bool ShouldReportChildErrors(IJsonSchemaKeyword keyword, SchemaValidationContext context)
		{
			return !_errorCollectionConditions.Any(c => c.ShouldExcludeChildErrors(keyword, context));
		}

		private static string _BasicDownload(string path)
		{
			Log.Schema(() => $"Attempting to downloading schema from '{path}'");
			var uri = new Uri(path);

			switch (uri.Scheme)
			{
				case "http":
				case "https":
					return new HttpClient().GetStringAsync(uri).Result;
				case "file":
					var filename = Uri.UnescapeDataString(uri.AbsolutePath);
					return File.ReadAllText(filename);
				case "manatee":
					return null!;
				default:
					throw new Exception($"URI scheme '{uri.Scheme}' is not supported.  Only HTTP(S) and local file system URIs are allowed.");
			}
		}
	}
}
