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
 
	File Name:		JsonSchemaRegistry.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaRegistry
	Purpose:		Provides a registry in which JSON schema can be saved to be
					referenced by the system.

***************************************************************************************/
using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides a registry in which JSON schema can be saved to be referenced by the
	/// system.
	/// </summary>
	public static class JsonSchemaRegistry
	{
		private static readonly Dictionary<string, IJsonSchema> _schemaLookup;

		/// <summary>
		/// Initializes the <see cref="JsonSchemaRegistry"/> class.
		/// </summary>
		static JsonSchemaRegistry()
		{
			var draft04Uri = JsonSchema.Draft04.Id.Split('#')[0];
			_schemaLookup = new Dictionary<string, IJsonSchema>
				{
					[draft04Uri] = JsonSchema.Draft04
				};
		}

		/// <summary>
		/// Downloads and registers a schema at the specified URI.
		/// </summary>
		public static IJsonSchema Get(string uri)
		{
			IJsonSchema schema;
			lock (_schemaLookup)
			{
				if (!_schemaLookup.TryGetValue(uri, out schema))
				{
					var schemaJson = JsonSchemaOptions.Download(uri);
					schema = JsonSchemaFactory.FromJson(JsonValue.Parse(schemaJson));

					_schemaLookup[uri] = schema;
				}
			}

			return schema;
		}

		/// <summary>
		/// Explicitly registers an existing schema.
		/// </summary>
		public static void Register(JsonSchema schema)
		{
			if (schema.Id.IsNullOrWhiteSpace()) return;
			lock (_schemaLookup)
			{
				_schemaLookup[schema.Id] = schema;
			}
		}

		/// <summary>
		/// Removes a schema from the registry.
		/// </summary>
		public static void Unregister(JsonSchema schema)
		{
			if (schema.Id.IsNullOrWhiteSpace()) return;
			lock (_schemaLookup)
			{
				_schemaLookup.Remove(schema.Id);
			}
		}

		/// <summary>
		/// Removes a schema from the registry.
		/// </summary>
		public static void Unregister(string uri)
		{
			if (uri.IsNullOrWhiteSpace()) return;
			lock (_schemaLookup)
			{
				_schemaLookup.Remove(uri);
			}
		}
	}
}
