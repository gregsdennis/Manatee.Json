using System;
using System.IO;
using System.Net;
using Manatee.Json.Serialization;

#if !NET45
using System.Net.Http;
#endif

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines options associated with JSON Schema.
	/// </summary>
	public static class JsonSchemaOptions
	{
		private static Func<string, string> _download;

		/// <summary>
		/// Gets and sets a method used to download online schema.
		/// </summary>
		public static Func<string, string> Download
		{
			get { return _download ?? (_download = _BasicDownload); }
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
		/// Gets or sets the output verbosity.  The default is <see cref="SchemaValidationOutputFormat.Hierarchy"/>.
		/// </summary>
		public static SchemaValidationOutputFormat OutputFormat { get; set; } = SchemaValidationOutputFormat.Hierarchy;

		private static string _BasicDownload(string path)
		{
			Console.WriteLine(path);
			var uri = new Uri(path);

			switch (uri.Scheme)
			{
				case "http":
				case "https":
#if NET45
					return new WebClient().DownloadString(uri);
#else
					return new HttpClient().GetStringAsync(uri).Result;
#endif
				case "file":
					var filename = Uri.UnescapeDataString(uri.AbsolutePath);
					return File.ReadAllText(filename);
				default:
					throw new Exception($"URI scheme {uri.Scheme} is not supported.  Only HTTP(S) and local file system URIs are allowed.");
			}
		}
	}
}
