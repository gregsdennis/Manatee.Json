using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines methods to build schema objects.
	/// </summary>
	public static class JsonSchemaFactory
	{
		private static readonly List<Type> _integerTypes = new List<Type>
			{
				typeof (sbyte),
				typeof (byte),
				typeof (char),
				typeof (short),
				typeof (ushort),
				typeof (int),
				typeof (uint),
				typeof (long),
				typeof (ulong)
			};
		private static readonly List<Type> _numberTypes = new List<Type>
			{
				typeof (sbyte),
				typeof (byte),
				typeof (char),
				typeof (short),
				typeof (ushort),
				typeof (int),
				typeof (uint),
				typeof (long),
				typeof (ulong),
				typeof (double),
				typeof (decimal)
			};

		private static Func<IJsonSchema> _schemaFactory = () => new JsonSchema04();

		public static void SetDefaultSchemaVersion<T>()
			where T : IJsonSchema
		{
			_SetDefaultSchemaVersion(typeof(T));
		}
		private static void _SetDefaultSchemaVersion(Type type)
		{
			if (type == typeof(JsonSchema04))
				_schemaFactory = () => new JsonSchema04();
			else if (type == typeof(JsonSchema06))
				_schemaFactory = () => new JsonSchema06();
			else if (type == typeof(JsonSchema07))
				_schemaFactory = () => new JsonSchema07();
			else
				throw new ArgumentException($"Only {nameof(JsonSchema04)}, {nameof(JsonSchema06)}, and {nameof(JsonSchema07)} are supported.");
		}
		
		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <param name="documentPath">The path to the physical location to this document</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json, Uri documentPath = null)
		{
			return FromJson(json, _schemaFactory, documentPath);
		}
		internal static IJsonSchema FromJson(JsonValue json, Func<IJsonSchema> schemaFactory, Uri documentPath = null)
		{
			if (json == null) return null;
			IJsonSchema schema = null;
			switch (json.Type)
			{
				case JsonValueType.Object:
					var schemaDeclaration = json.Object.TryGetString("$schema");
					if (schemaDeclaration == JsonSchema04.MetaSchema.Id)
					{
						var id = json.Object.TryGetString("id");
						if (id == JsonSchema04.MetaSchema.Id) return JsonSchema04.MetaSchema;
						schema = new JsonSchema04();
					}
					else if (schemaDeclaration == JsonSchema06.MetaSchema.Id)
					{
						var id = json.Object.TryGetString("$id");
						if (id == JsonSchema06.MetaSchema.Id) return JsonSchema06.MetaSchema;
						schema = new JsonSchema06();
					}
					else if (schemaDeclaration == JsonSchema07.MetaSchema.Id)
					{
						var id = json.Object.TryGetString("$id");
						if (id == JsonSchema07.MetaSchema.Id) return JsonSchema07.MetaSchema;
						schema = new JsonSchema07();
					}
					schema = schema ?? schemaFactory();
					if (json.Object.ContainsKey("$ref"))
						schema = json.Object.Count > 1
							         ? new JsonSchemaReference(schema.GetType()) {Base = schema}
							         : new JsonSchemaReference(schema.GetType());
					break;
				case JsonValueType.Array:
					schema = new JsonSchemaCollection();
					break;
				case JsonValueType.Boolean:
					schema = new JsonSchema06();
					break;
				default:
					throw new SchemaLoadException($"JSON Schema must be objects; actual type: {json.Type}");
			}
			schema.DocumentPath = documentPath;
			schema.FromJson(json, null);
			return schema;
		}
	}
}