using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines methods to build schema objects.
	/// </summary>
	public static class JsonSchemaFactory
	{
		private class SchemaFactory
		{
			public Func<IJsonSchema> Factory { get; set; }
			public Type SchemaType { get; set; }
			public string Id { get; set; }
			public string IdKey { get; set; }
			public IJsonSchema MetaSchema { get; set; }
		}

		private static readonly List<SchemaFactory> _schemaFactories;
		private static SchemaFactory _schemaFactory;

		static JsonSchemaFactory()
		{
			_schemaFactories = new List<SchemaFactory>();

			RegisterExtendedSchema(JsonSchema04.MetaSchema.Id, () => new JsonSchema04(), JsonSchema04.MetaSchema);
			RegisterExtendedSchema(JsonSchema06.MetaSchema.Id, () => new JsonSchema06(), JsonSchema06.MetaSchema);
			RegisterExtendedSchema(JsonSchema07.MetaSchema.Id, () => new JsonSchema07(), JsonSchema07.MetaSchema);

			SetDefaultSchemaVersion<JsonSchema04>();
		}

		/// <summary>
		/// Sets the default schema to use when deserializing a schema that doesn't define its version.
		/// </summary>
		/// <typeparam name="T">The schema type.</typeparam>
		public static void SetDefaultSchemaVersion<T>()
			where T : IJsonSchema
		{
			_SetDefaultSchemaVersion(typeof(T));
		}
		/// <summary>
		/// Registers a schema with an extended vocabulary so that it can be deserialized properly.
		/// </summary>
		/// <typeparam name="T">The schema type.</typeparam>
		/// <param name="id">The schema's ID.</param>
		/// <param name="factory">A factory function for creating instances.</param>
		/// <param name="metaSchema">Optional - A meta-schema instance for this schema.</param>
		public static void RegisterExtendedSchema<T>(string id, Func<T> factory, T metaSchema = default(T))
			where T : IJsonSchema
		{
			var schemaFactory = new SchemaFactory
				{
					Factory = () => factory(),
					SchemaType = typeof(T),
					Id = id,
					IdKey = typeof(JsonSchema04).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) ? "id" : "$id",
					MetaSchema = metaSchema
				};
			_schemaFactories.Add(schemaFactory);
		}
		private static void _SetDefaultSchemaVersion(Type type)
		{
			var factory = _schemaFactories.FirstOrDefault(f => f.SchemaType == type);

			_schemaFactory = factory ?? throw new ArgumentException($"Schema type '{type}' is not supported.  Has it been registered?");
		}
		
		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <param name="documentPath">The path to the physical location to this document</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json, Uri documentPath = null)
		{
			return _FromJson(json, _schemaFactory, documentPath);
		}
		/// <summary>
		/// Creates a schema object from its JSON representation, allowing a specific schema version to be used..
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <param name="documentPath">The path to the physical location to this document</param>
		/// <typeparam name="T">The type representing the schema version to create.</typeparam>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson<T>(JsonValue json, Uri documentPath = null)
		{
			return FromJson(json, typeof(T), documentPath);
		}
		/// <summary>
		/// Creates a schema object from its JSON representation, allowing a specific schema version to be used..
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <param name="type">The type representing the schema version to create.</param>
		/// <param name="documentPath">The path to the physical location to this document</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json, Type type, Uri documentPath = null)
		{
			var factory = _schemaFactories.FirstOrDefault(f => f.SchemaType == type);
			if (factory == null)
				throw new ArgumentException($"Schema type '{type}' is not supported.  Has it been registered?");

			return _FromJson(json, factory, documentPath);
		}
		private static IJsonSchema _FromJson(JsonValue json, SchemaFactory schemaFactory, Uri documentPath = null)
		{
			if (json == null) return null;
			IJsonSchema schema = null;
			switch (json.Type)
			{
				case JsonValueType.Object:
					var schemaDeclaration = json.Object.TryGetString("$schema");
					var factory = _schemaFactories.FirstOrDefault(f => f.Id == schemaDeclaration);
					if (factory != null)
					{
						var id = json.Object.TryGetString(factory.IdKey);
						if (id == factory.MetaSchema.Id) return factory.MetaSchema;
						schema = factory.Factory();
					}
					schema = schema ?? schemaFactory.Factory();
					if (json.Object.ContainsKey("$ref"))
						schema = json.Object.Count > 1
							         ? new JsonSchemaReference(schema.GetType()) {Base = schema}
							         : new JsonSchemaReference(schema.GetType());
					break;
				case JsonValueType.Array:
					schema = new JsonSchemaCollection();
					break;
				case JsonValueType.Boolean:
					// not determining draft here is fine
					// True is intended to pass all instances
					// False is intended to fail all instances
					schema = new JsonSchema06();
					break;
				default:
					throw new SchemaLoadException($"JSON Schema must be objects; actual type: {json.Type}");
			}
			schema.DocumentPath = documentPath;
			schema.FromJson(json, null);
			return schema;
		}

		internal static IJsonSchema GetMetaSchema(Type schemaType)
		{
			var factory = _schemaFactories.FirstOrDefault(f => f.SchemaType == schemaType) ??
			              _schemaFactories.FirstOrDefault(f => schemaType.GetTypeInfo().IsSubclassOf(f.SchemaType)) ??
			              throw new ArgumentException($"Schema type '{schemaType}' is not supported.  Has it been registered?");

			return factory.MetaSchema;
		}
	}
}