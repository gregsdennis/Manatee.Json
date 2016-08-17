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
 
	File Name:		OnlineSchemaCache.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		OnlineSchemaCache
	Purpose:		Provides caching around downloading schema definitions.

***************************************************************************************/
using System.Collections.Concurrent;
using System.Net;

namespace Manatee.Json.Schema
{
	internal static class OnlineSchemaCache
	{
		private static readonly ConcurrentDictionary<string, IJsonSchema> _schemaLookup;

		static OnlineSchemaCache()
		{
			var draft04Uri = JsonSchema.Draft04.Id.Split('#')[0];
			_schemaLookup = new ConcurrentDictionary<string, IJsonSchema>
				{
					[draft04Uri] = JsonSchema.Draft04
				};
		}

		public static IJsonSchema Get(string uri)
		{
			IJsonSchema schema;
			if (!_schemaLookup.TryGetValue(uri, out schema))
			{
				var schemaJson = new WebClient().DownloadString(uri);
				schema = JsonSchemaFactory.FromJson(JsonValue.Parse(schemaJson));

				_schemaLookup[uri] = schema;
			}

			return schema;
		}
	}
}
