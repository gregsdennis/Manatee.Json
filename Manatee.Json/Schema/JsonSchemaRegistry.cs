using System;
using System.Collections.Concurrent;
using Manatee.Json.Patch;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides a registry in which JSON schema can be saved to be referenced by the system.
	/// </summary>
	public static class JsonSchemaRegistry
	{
		// TODO: opportunity to refine the caching of resolved schemas:
		private class RegisteredSchema
		{
			public string BaseUri { get; set; }
			public ConcurrentDictionary<string, JsonSchema> LocalReferences { get; } = new ConcurrentDictionary<string, JsonSchema>();
		}

		private static readonly ConcurrentDictionary<string, JsonSchema> _schemaLookup;
		private static readonly JsonSerializer _serializer;

		/// <summary>
		/// Initializes the <see cref="JsonSchemaRegistry"/> class.
		/// </summary>
		static JsonSchemaRegistry()
		{
			_schemaLookup = new ConcurrentDictionary<string, JsonSchema>();
			_serializer = new JsonSerializer();
			Clear();
		}

		/// <summary>
		/// Downloads and registers a schema at the specified URI.
		/// </summary>
		public static JsonSchema Get(string uri)
		{
			JsonSchema schema;
			lock (_schemaLookup)
			{
				uri = uri.TrimEnd('#');
				if (!_schemaLookup.TryGetValue(uri, out schema))
				{
					var schemaJson = JsonSchemaOptions.Download(uri);
				    var schemaValue = JsonValue.Parse(schemaJson);
					schema = new JsonSchema {DocumentPath = new Uri(uri)};
					schema.FromJson(schemaValue, _serializer);

					var schemaStructureValidation = schema.ValidateSchema();
					if (!schemaStructureValidation.IsValid)
					{
						var errors = string.Join(Environment.NewLine, schemaStructureValidation.Errors);
						throw new ArgumentException($"The given path does not contain a valid schema.  Errors: \n{errors}");
					}

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
			if (string.IsNullOrWhiteSpace(schema.Id)) return;
			lock (_schemaLookup)
			{
				_schemaLookup[schema.DocumentPath.ToString()] = schema;
			}
		}

		/// <summary>
		/// Removes a schema from the registry.
		/// </summary>
		public static void Unregister(JsonSchema schema)
		{
			if (string.IsNullOrWhiteSpace(schema.Id)) return;
			lock (_schemaLookup)
			{
				_schemaLookup.TryRemove(schema.Id, out _);
			}
		}

		/// <summary>
		/// Removes a schema from the registry.
		/// </summary>
		public static void Unregister(string uri)
		{
			if (string.IsNullOrWhiteSpace(uri)) return;
			lock (_schemaLookup)
			{
				_schemaLookup.TryRemove(uri, out _);
			}
		}

		/// <summary>
		/// Clears the registry of all types.
		/// </summary>
		public static void Clear()
		{
			var draft04Uri = MetaSchemas.Draft04.Id.Split('#')[0];
			var draft06Uri = MetaSchemas.Draft06.Id.Split('#')[0];
			var draft07Uri = MetaSchemas.Draft07.Id.Split('#')[0];
			//var draft08Uri = MetaSchemas.Draft08.Id.Split('#')[0];
			var patchUri = JsonPatch.Schema.Id.Split('#')[0];
			lock (_schemaLookup)
			{
				_schemaLookup.Clear();
				_schemaLookup[draft04Uri] = MetaSchemas.Draft04;
				_schemaLookup[draft06Uri] = MetaSchemas.Draft06;
				_schemaLookup[draft07Uri] = MetaSchemas.Draft07;
				//_schemaLookup[draft08Uri] = MetaSchemas.Draft08;
				_schemaLookup[patchUri] = JsonPatch.Schema;
			}
		}
	}
}
