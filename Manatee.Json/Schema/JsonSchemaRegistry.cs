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
		private static readonly Dictionary<string, IJsonSchema> _schemaLookup;

		/// <summary>
		/// Initializes the <see cref="JsonSchemaRegistry"/> class.
		/// </summary>
		static JsonSchemaRegistry()
		{
			_schemaLookup = new Dictionary<string, IJsonSchema>();
			Clear();
		}

		/// <summary>
		/// Downloads and registers a schema at the specified URI.
		/// </summary>
		public static IJsonSchema Get(string uri)
		{
			IJsonSchema schema;
			lock (_schemaLookup)
			{
				uri = uri.TrimEnd('#');
				if (!_schemaLookup.TryGetValue(uri, out schema))
				{
					var schemaJson = JsonSchemaOptions.Download(uri);
				    var schemaValue = JsonValue.Parse(schemaJson);
					schema = JsonSchemaFactory.FromJson(schemaValue, new Uri(uri));

					var metaSchemas = new IJsonSchema[]
						{
							JsonSchema07.MetaSchema,
							JsonSchema06.MetaSchema,
							JsonSchema04.MetaSchema
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
							if (validation.Valid) break;
						}
					}

					if (validation != null && !validation.Valid)
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
		public static void Register(IJsonSchema schema)
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
		public static void Unregister(IJsonSchema schema)
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
			var draft04Uri = JsonSchema04.MetaSchema.Id.Split('#')[0];
			var draft06Uri = JsonSchema06.MetaSchema.Id.Split('#')[0];
			var draft07Uri = JsonSchema07.MetaSchema.Id.Split('#')[0];
			var patchUri = JsonPatch.Schema.Id.Split('#')[0];
			lock (_schemaLookup)
			{
				_schemaLookup[draft04Uri] = JsonSchema04.MetaSchema;
				_schemaLookup[draft06Uri] = JsonSchema06.MetaSchema;
				_schemaLookup[draft07Uri] = JsonSchema07.MetaSchema;
				_schemaLookup[patchUri] = JsonPatch.Schema;
			}
		}
	}
}
