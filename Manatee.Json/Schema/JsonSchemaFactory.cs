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

		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <param name="documentPath">The path to the physical location to this document</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json, Uri documentPath = null)
		{
			if (json == null) return null;
			// NOTE: I wonder if there's an issue with the compiler.  I have to assign null here
			//		 because it thinks that schema is not initialized, but I can't find the path
			//		 where it's not.
			IJsonSchema schema = null;
			switch (json.Type)
			{
				case JsonValueType.Object:
					if (json.Object.ContainsKey("$ref"))
						schema = new JsonSchemaReference();
					else
					{
						var schemaDeclaration = json.Object.TryGetString("$schema");
						IJsonSchema validator = null;
						if (schemaDeclaration == JsonSchema04.MetaSchema.Schema)
						{
							validator = JsonSchema04.MetaSchema;
							schema = new JsonSchema04();
						}
						else if (schemaDeclaration == JsonSchema06.MetaSchema.Schema)
						{
							validator = JsonSchema06.MetaSchema;
							schema = new JsonSchema06();
						}
						if (validator != null)
						{
							var results = validator.Validate(json);
							if (!results.Valid) throw new ArgumentException($"Schema specifies '{schemaDeclaration}' but does not validate.");
						}
						// not specified; auto detect, default to 04
						else if (JsonSchema04.MetaSchema.Validate(json).Valid)
							schema = new JsonSchema04();
						else if (JsonSchema06.MetaSchema.Validate(json).Valid)
							schema = new JsonSchema06();
						else
							throw new NotImplementedException("Cannot determine JSON Schema version.  Only Drafts 04 and 06 are supported.");
					}
					break;
				case JsonValueType.Array:
					schema = new JsonSchemaCollection();
					break;
				case JsonValueType.Boolean:
					if (!JsonSchema06.MetaSchema.Validate(json).Valid)
						throw new NotImplementedException("Only Draft 06 supports boolean schemata.");
					schema = new JsonSchema06();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(json.Type), "JSON Schema must be objects.");
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