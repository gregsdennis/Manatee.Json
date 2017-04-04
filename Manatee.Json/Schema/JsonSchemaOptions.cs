using System;
using System.Net;
#if IOS || CORE
using System.Net.Http;
#endif
using System.Text;

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
#if IOS || CORE
			get { return _download ?? (_download = uri => new HttpClient().GetStringAsync(uri).Result); }
#else
			get { return _download ?? (_download = uri => new WebClient {Encoding = Encoding.UTF8}.DownloadString(uri)); }
#endif
			set { _download = value; }
		}

		/// <summary>
		/// Gets or sets whether the "format" schema keyword should be validated.  The default is true.
		/// </summary>
		public static bool ValidateFormat { get; }

		/// <summary>
		/// Initializes all properties.
		/// </summary>
		static JsonSchemaOptions()
		{
			ValidateFormat = true;
		}
	}
}
