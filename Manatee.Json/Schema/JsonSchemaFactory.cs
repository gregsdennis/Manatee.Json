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
			if (typeof(T) == typeof(JsonSchema04))
				_schemaFactory = () => new JsonSchema04();
			else if (typeof(T) == typeof(JsonSchema06))
				_schemaFactory = () => new JsonSchema06();
			else
				throw new ArgumentException($"Only {nameof(JsonSchema04)} and {nameof(JsonSchema06)} are supported.");
		}
		
		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <param name="documentPath">The path to the physical location to this document</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json, Uri documentPath = null)
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
						var id = json.Object.TryGetString("id");
						if (id == JsonSchema06.MetaSchema.Id) return JsonSchema06.MetaSchema;
						schema = new JsonSchema06();
					}
					if (json.Object.ContainsKey("$ref"))
						schema = json.Object.Count > 1
							         ? new JsonSchemaReference {Base = schema ?? _schemaFactory()}
							         : new JsonSchemaReference();
					schema = schema ?? _schemaFactory();
					break;
				case JsonValueType.Array:
					schema = new JsonSchemaCollection();
					break;
				case JsonValueType.Boolean:
					if (!JsonSchema06.MetaSchema.Validate(json).Valid)
						throw new SchemaLoadException("Only Draft 06 supports boolean schemata.");
					schema = new JsonSchema06();
					break;
				default:
					throw new SchemaLoadException($"JSON Schema must be objects; actual type: {json.Type}");
			}
			schema.DocumentPath = documentPath;
			schema.FromJson(json, null);
			return schema;
		}

#if !NETSTANDARD1_3
		/// <summary>
		/// Builds a <see cref="IJsonSchema"/> implementation which can validate JSON for a given type.
		/// </summary>
		/// <typeparam name="T">The type to convert to a schema.</typeparam>
		/// <returns>The schema object.</returns>
		public static IJsonSchema FromType<T>()
		{
			return FromType(typeof (T));
		}

		/// <summary>
		/// Builds a <see cref="IJsonSchema"/> implementation which can validate JSON for a given type.
		/// </summary>
		/// <param name="type">The type to convert to a schema.</param>
		/// <returns>The schema object.</returns>
		public static IJsonSchema FromType(Type type)
		{
			throw new NotImplementedException();
		}

		internal static IJsonSchema GetPrimitiveSchema(JsonValue typeEntry)
		{
			IJsonSchema schema = null;
			switch (typeEntry.String)
			{
				case "array":
					schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Array};
					break;
				case "boolean":
					schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Boolean};
					break;
				case "integer":
					schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer};
					break;
				case "null":
					schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Null};
					break;
				case "number":
					schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number};
					break;
				case "object":
					schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.Object};
					break;
				case "string":
					schema = new JsonSchema04 {Type = JsonSchemaTypeDefinition.String};
					break;
			}

			return schema;
		}

		private static IJsonSchema FromType(Type type, JsonSchemaTypeDefinitionCollection definitions)
		{
			throw new NotImplementedException();
		}
		private static IJsonSchema GetBasicSchema(Type type)
		{
			if (type == typeof(string))
				return new JsonSchema04 {Type = JsonSchemaTypeDefinition.String};
			if (type == typeof(bool))
				return new JsonSchema04 {Type = JsonSchemaTypeDefinition.Boolean};
			if (_integerTypes.Contains(type))
				return new JsonSchema04 {Type = JsonSchemaTypeDefinition.Integer};
			if (_numberTypes.Contains(type))
				return new JsonSchema04 {Type = JsonSchemaTypeDefinition.Number};
			return null;
		}
#endif
	}
}