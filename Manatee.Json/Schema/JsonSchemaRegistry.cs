using System;
using System.Collections.Generic;
using System.Linq;
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
				    var  schemaValue = JsonValue.Parse(schemaJson);
					schema = JsonSchemaFactory.FromJson(schemaValue, new Uri(uri));

					var validation = JsonSchema.Draft04.Validate(schemaValue);

					if (!validation.Valid)
					{
						var errors = validation.Errors.Select(e => e.Message).Join(Environment.NewLine);
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
