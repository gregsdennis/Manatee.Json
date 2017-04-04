using System;
using System.Collections.Generic;

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

		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <param name="documentPath">The path to the physical location to this document</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json, Uri documentPath = null)
		{
			if (json == null) return null;
			IJsonSchema schema;
			switch (json.Type)
			{
				case JsonValueType.Object:
					schema = json.Object.ContainsKey("$ref")
								 ? new JsonSchemaReference()
								 : new JsonSchema();
					break;
				case JsonValueType.Array:
					schema = new JsonSchemaCollection();
					break;
				default:
					throw new ArgumentOutOfRangeException("json.Type", "JSON Schema must be objects.");
			}
			schema.DocumentPath = documentPath;
			schema.FromJson(json, null);
			return schema;
		}

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
					schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Array};
					break;
				case "boolean":
					schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Boolean};
					break;
				case "integer":
					schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Integer};
					break;
				case "null":
					schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Null};
					break;
				case "number":
					schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Number};
					break;
				case "object":
					schema = new JsonSchema {Type = JsonSchemaTypeDefinition.Object};
					break;
				case "string":
					schema = new JsonSchema {Type = JsonSchemaTypeDefinition.String};
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
				return new JsonSchema {Type = JsonSchemaTypeDefinition.String};
			if (type == typeof(bool))
				return new JsonSchema {Type = JsonSchemaTypeDefinition.Boolean};
			if (_integerTypes.Contains(type))
				return new JsonSchema {Type = JsonSchemaTypeDefinition.Integer};
			if (_numberTypes.Contains(type))
				return new JsonSchema {Type = JsonSchemaTypeDefinition.Number};
			return null;
		}
	}
}