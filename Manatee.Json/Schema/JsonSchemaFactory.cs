/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSchemaFactory.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaFactory
	Purpose:		Defines methods to build schema objects.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
		/// Returns
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IJsonSchema Load(string path)
		{
			var text = File.ReadAllText(path);
			var json = JsonValue.Parse(text);
			var validation = JsonSchema.Draft04.Validate(json);
			if (!validation.Valid)
			{
				var errors = string.Join(Environment.NewLine, validation.Errors.Select(e => e.Message));
				throw new ArgumentException($"The given path does not contain a valid schema.  Errors: \n{errors}");
			}
			return FromJson(json);
		}
		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json)
		{
			if (json == null) return null;
			IJsonSchema schema = new JsonSchema();
			var obj = json.Object;
			if (obj.ContainsKey("type"))
			{
				switch (obj["type"].String)
				{
					case "array":
						schema = new ArraySchema();
						break;
					case "boolean":
						schema = new BooleanSchema();
						break;
					case "integer":
						schema = new IntegerSchema();
						break;
					case "null":
						schema = new NullSchema();
						break;
					case "number":
						schema = new NumberSchema();
						break;
					case "object":
						schema = new ObjectSchema();
						break;
					case "string":
						schema = new StringSchema();
						break;
				}
			}
			else if (obj.ContainsKey("$ref"))
			{
				// if has "$ref" key, select SchemaReference
				schema = new JsonSchemaReference();
			}
			else if (obj.ContainsKey("anyOf"))
			{
				// if has "anyOf" key, select AnyOfSchema
				schema = new AnyOfSchema();
			}
			else if (obj.ContainsKey("allOf"))
			{
				// if has "allOf" key, select AllOfSchema
				schema = new AllOfSchema();
			}
			else if (obj.ContainsKey("oneOf"))
			{
				// if has "oneOf" key, select OneOfSchema
				schema = new OneOfSchema();
			}
			else if (obj.ContainsKey("not"))
			{
				// if has "not" key, select NotSchema
				schema = new NotSchema();
			}
			else if (obj.ContainsKey("enum"))
			{
				// if has "enum" key, select EnumSchema
				schema = new EnumSchema();
			}
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
			var schema = FromType(type, null);
			var objSchema = schema as ObjectSchema;
			if (objSchema?.Definitions != null)
			{
				RemoveSingleDefinitionReferences(objSchema);
			}
			var arrSchema = schema as ArraySchema;
			if (arrSchema?.Definitions != null)
			{
				RemoveSingleDefinitionReferences(arrSchema);
			}
			return schema;
		}

		private static IJsonSchema FromType(Type type, JsonSchemaTypeDefinitionCollection definitions)
		{
			var self = definitions?.FirstOrDefault(d => d.Name == type.FullName);
			if (self?.Definition != null)
				return self.Definition != JsonSchemaReference.Root
					       ? new JsonSchemaReference($"#/definitions/{type.FullName}")
					       : self.Definition;
			var definitionList = definitions ?? new JsonSchemaTypeDefinitionCollection
				{
					new JsonSchemaTypeDefinition(type.FullName) {Definition = JsonSchemaReference.Root}
				};
			// Basic types
			var schema = GetBasicSchema(type);
			if (schema != null) return schema;
			// Enums
			if (typeof (Enum).IsAssignableFrom(type))
				return new EnumSchema {Values = Enum.GetNames(type).Select(n => new JsonSchemaTypeDefinition(n))};
			// Arrays
			var asIEnumerable = type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
			if (asIEnumerable != null)
			{
				var itemType = asIEnumerable.GetGenericArguments().First();
				var itemDefinition = definitionList.FirstOrDefault(d => d.Name == itemType.FullName);
				if (itemDefinition == null)
				{
					itemDefinition = new JsonSchemaTypeDefinition(itemType.FullName)
						{
							ArrayReferences = new List<ArraySchema>()
						};
					definitionList.Add(itemDefinition);
					itemDefinition.Definition = FromType(itemType, definitionList);
				}
				if (itemDefinition.ArrayReferences == null)
					itemDefinition.ArrayReferences = new List<ArraySchema>();
				var arraySchema = new ArraySchema
					{
						Items = new JsonSchemaReference($"#/definitions/{itemType.FullName}"),
						Definitions = definitions == null && definitionList.Any(d => d.Definition != null)
							              ? definitionList
							              : null
					};
				itemDefinition.ArrayReferences.Add(arraySchema);
				return arraySchema;
			}
			// Objects
			var propertyList = new JsonSchemaPropertyDefinitionCollection();
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var property in properties)
			{
				if (!property.CanRead || property.GetIndexParameters().Any()) continue;
				var propertyDefinition = new JsonSchemaPropertyDefinition(property.Name);
				schema = GetBasicSchema(property.PropertyType);
				if (schema != null)
				{
					propertyDefinition.Type = schema;
				}
				else
				{
					var definition = definitionList.FirstOrDefault(d => d.Name == property.PropertyType.FullName);
					if (definition == null)
					{
						definition = new JsonSchemaTypeDefinition(property.PropertyType.FullName);
						definitionList.Add(definition);
						definition.Definition = FromType(property.PropertyType, definitionList);
					}
					if (definition.PropertyReferences == null)
						definition.PropertyReferences = new List<JsonSchemaPropertyDefinition>();
					propertyDefinition.Type = new JsonSchemaReference($"#/definitions/{property.PropertyType.FullName}");
					definition.PropertyReferences.Add(propertyDefinition);
				}
				propertyList.Add(propertyDefinition);
			}
			return new ObjectSchema
				{
					Properties = propertyList.Any() ? propertyList : null,
					Definitions = definitions == null && definitionList.Any(d => d.Definition != null)
						              ? definitionList
						              : null
				};
		}
		private static IJsonSchema GetBasicSchema(Type type)
		{
			if (type == typeof(string))
				return new StringSchema();
			if (type == typeof(bool))
				return new BooleanSchema();
			if (_integerTypes.Contains(type))
				return new IntegerSchema();
			if (_numberTypes.Contains(type))
				return new NumberSchema();
			return null;
		}
		private static void RemoveSingleDefinitionReferences(ObjectSchema objSchema)
		{
			var references = objSchema.Definitions.Where(d => d.ReferenceCount == 1 || d.Definition == JsonSchemaReference.Root).ToList();
			foreach (var definition in references)
			{
				if (definition.PropertyReferences != null && definition.PropertyReferences.Any())
					definition.PropertyReferences[0].Type = definition.Definition;
				if (definition.ArrayReferences != null && definition.ArrayReferences.Any())
					definition.ArrayReferences[0].Items = definition.Definition;
				objSchema.Definitions.Remove(definition);
			}
			if (!objSchema.Definitions.Any())
				objSchema.Definitions = null;
		}
		private static void RemoveSingleDefinitionReferences(ArraySchema arrSchema)
		{
			var references = arrSchema.Definitions.Where(d => d.ReferenceCount == 1 || d.Definition == JsonSchemaReference.Root).ToList();
			foreach (var definition in references)
			{
				if (definition.PropertyReferences != null && definition.PropertyReferences.Any())
					definition.PropertyReferences[0].Type = definition.Definition;
				if (definition.ArrayReferences != null && definition.ArrayReferences.Any())
					definition.ArrayReferences[0].Items = definition.Definition;
				arrSchema.Definitions.Remove(definition);
			}
			if (!arrSchema.Definitions.Any())
				arrSchema.Definitions = null;
		}
	}
}