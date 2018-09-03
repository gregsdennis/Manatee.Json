using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Patch;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides a registry in which JSON schema can be saved to be referenced by the system.
	/// </summary>
	public static class JsonSchemaRegistry
	{
		private static readonly Dictionary<string, JsonSchema> _schemaLookup;

		/// <summary>
		/// Initializes the <see cref="JsonSchemaRegistry"/> class.
		/// </summary>
		static JsonSchemaRegistry()
		{
			_schemaLookup = new Dictionary<string, JsonSchema>();
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
					schema = JsonSchemaFactory.FromJson(schemaValue, new Uri(uri));

					var metaSchemas = new[]
						{
							MetaSchemas.Draft04,
							MetaSchemas.Draft06,
							MetaSchemas.Draft07,
							MetaSchemas.Draft08
						};

					SchemaValidationResults validation = null;
					if (schema.Schema != null)
					{
						var bySchema = metaSchemas.FirstOrDefault(s => s.Id == schema.Schema);
						if (bySchema != null)
							validation = bySchema.Validate(schemaValue);
					}
					else
					{
						foreach (var metaSchema in metaSchemas)
						{
							validation = metaSchema.Validate(schemaValue);
							if (validation.IsValid) break;
						}
					}

					if (validation != null && !validation.IsValid)
					{
						var errors = string.Join(Environment.NewLine, validation.Errors.Select(e => e.Message));
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
				_schemaLookup.Remove(schema.Id);
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
				_schemaLookup.Remove(uri);
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
			var draft08Uri = MetaSchemas.Draft08.Id.Split('#')[0];
			var patchUri = JsonPatch.Schema.Id.Split('#')[0];
			lock (_schemaLookup)
			{
				_schemaLookup[draft04Uri] = MetaSchemas.Draft04;
				_schemaLookup[draft06Uri] = MetaSchemas.Draft06;
				_schemaLookup[draft07Uri] = MetaSchemas.Draft07;
				_schemaLookup[draft08Uri] = MetaSchemas.Draft08;
				_schemaLookup[patchUri] = JsonPatch.Schema;
			}
		}
	}
}
