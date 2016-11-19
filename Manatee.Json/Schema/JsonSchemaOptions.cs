/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSchemaMultiTypeDefinition.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaMultiTypeDefinition
	Purpose:		Defines options associated with JSON Schema.

***************************************************************************************/
using System;
using System.Net;
#if CORE
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
#if CORE
			get { return _download ?? (_download = uri => new HttpClient().GetStringAsync(uri).Result); }
#else
			get { return _download ?? (_download = uri => new WebClient().DownloadString(uri)); }
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
