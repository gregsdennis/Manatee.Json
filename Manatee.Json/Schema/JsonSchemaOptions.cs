using System;
using System.Net;
using System.Net.Http;
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
			get { return _download ?? (_download = uri => new HttpClient().GetStringAsync(uri).Result); }
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
