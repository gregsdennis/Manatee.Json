using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines options associated with JSON Schema.
	/// </summary>
	public static class JsonSchemaOptions
	{
		private interface IErrorCollectionCondition
		{
			bool ShouldExcludeChildErrors(IJsonSchemaKeyword keyword, SchemaValidationContext context);
		}

		private class LocationErrorCollectionCondition : IErrorCollectionCondition
		{
			public JsonPointer Location { get; private set; }

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
			public Type Type { get; private set; }

			public KeywordErrorCollectionCondition(Type type)
			{
				Type = type;
			}

			public bool ShouldExcludeChildErrors(IJsonSchemaKeyword keyword, SchemaValidationContext context)
			{
				return Type.IsInstanceOfType(keyword);
			}
		}

		private static readonly List<IErrorCollectionCondition> _errorCollectionConditions;
		private static Func<string, string>? _download;

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
		public static bool ValidateFormatKeyword { get; set; } = true;

		/// <summary>
		/// Gets or sets whether unknown string formats are permitted.  If disabled and an unknown format
		/// is found, the system will throw a <see cref="JsonSerializationException"/> while loading the schema.
		/// The default is true.
		/// </summary>
		public static bool AllowUnknownFormats { get; set; } = true;

		/// <summary>
		/// Gets or sets the output verbosity.  The default is <see cref="SchemaValidationOutputFormat.Flag"/>.
		/// </summary>
		public static SchemaValidationOutputFormat OutputFormat { get; set; } = SchemaValidationOutputFormat.Flag;

		/// <summary>
		/// Determines how `$ref` keywords are resolved when adjacent to an `$id` keyword
		/// when a specific draft cannot be identified.  The default is <see cref="RefResolutionStrategy.ProcessSiblingId"/>.
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
		public static RefResolutionStrategy RefResolution { get; set; } = RefResolutionStrategy.ProcessSiblingId;

		/// <summary>
		/// Defines a default base URI for root schemas that use a relative URI for their `$id`.  The default is `manatee://json-schema/`.
		/// </summary>
		public static Uri DefaultBaseUri { get; set; } = new Uri("manatee://json-schema/", UriKind.Absolute);

		internal static bool ConfigureForTestOutput { get; set; }

		static JsonSchemaOptions()
		{
			_errorCollectionConditions = new List<IErrorCollectionCondition>();
		}

		private static string _BasicDownload(string path)
		{
			Console.WriteLine(path);
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
		public static void IgnoreErrorsForChildren<T>()
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
		public static void IgnoreErrorsForChildren(JsonPointer location)
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
		public static bool ShouldReportChildErrors(IJsonSchemaKeyword keyword, SchemaValidationContext context)
		{
			return !_errorCollectionConditions.Any(c => c.ShouldExcludeChildErrors(keyword, context));
		}
	}
}
