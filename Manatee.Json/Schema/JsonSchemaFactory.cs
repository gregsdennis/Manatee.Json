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
		public static IJsonSchema FromTypeBeta<T>()
		{
			return FromTypeBeta(typeof (T));
		}

		/// <summary>
		/// Builds a <see cref="IJsonSchema"/> implementation which can validate JSON for a given type.
		/// </summary>
		/// <param name="type">The type to convert to a schema.</param>
		/// <returns>The schema object.</returns>
		public static IJsonSchema FromTypeBeta(Type type)
		{
			var schema = FromType(type, null);
			ReplaceReferences(schema, string.Format("#/definitions/{0}", type.FullName));
			var objSchema = schema as ObjectSchema;
			if (objSchema != null)
			{
				objSchema.Definitions.Remove(objSchema.Definitions[type.FullName]);
			}
			return schema;
		}

		private static IJsonSchema FromType(Type type, JsonSchemaTypeDefinitionCollection definitions)
		{
			var definitionList = definitions ?? new JsonSchemaTypeDefinitionCollection
				{
					new JsonSchemaTypeDefinition(type.FullName)
				};
			var schema = GetBasicSchema(type, definitionList);
			if (schema != null) return schema;
			var propertyList = new JsonSchemaPropertyDefinitionCollection();
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var property in properties)
			{
				if (!property.CanRead || property.GetIndexParameters().Any()) continue;
				var propertyDefinition = new JsonSchemaPropertyDefinition(property.Name);
				schema = GetBasicSchema(property.PropertyType, definitionList);
				if (schema != null)
				{
					propertyDefinition.Type = schema;
				}
				else if (definitionList.Any(d => d.Name == property.PropertyType.FullName))
				{
					propertyDefinition.Type = new JsonSchemaReference(string.Format("#/definitions/{0}", property.PropertyType.FullName));
				}
				else if (typeof (Enum).IsAssignableFrom(type))
				{
					var schemaDefinition = new JsonSchemaTypeDefinition(property.PropertyType.FullName)
						{
							Definition = new EnumSchema {Values = Enum.GetNames(type).Select(n => new JsonSchemaTypeDefinition(n))}
						};
					definitionList.Add(schemaDefinition);
					propertyDefinition.Type = new JsonSchemaReference(string.Format("#/definitions/{0}", property.PropertyType.FullName));
				}
				else
				{
					var schemaDefinition = new JsonSchemaTypeDefinition(property.PropertyType.FullName);
					definitionList.Add(schemaDefinition);
					schemaDefinition.Definition = FromType(property.PropertyType, definitionList);
					propertyDefinition.Type = new JsonSchemaReference(string.Format("#/definitions/{0}", property.PropertyType.FullName));
				}
				propertyList.Add(propertyDefinition);
			}
			return new ObjectSchema
				{
					Properties = propertyList.Any() ? propertyList : null,
					Definitions = definitions == null
									  ? (definitionList.Any() ? definitionList : null)
									  : null
				};
		}
		private static IJsonSchema GetBasicSchema(Type type, JsonSchemaTypeDefinitionCollection definitions)
		{
			if (type == typeof(string))
				return new StringSchema();
			if (type == typeof(bool))
				return new BooleanSchema();
			if (_integerTypes.Contains(type))
				return new IntegerSchema();
			if (_numberTypes.Contains(type))
				return new NumberSchema();
			var implemented = type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof (IEnumerable<>)));
			if (implemented != null)
				return new ArraySchema {Items = FromType(implemented.GetGenericArguments().First(), definitions)};
			return null;
		}
		private static void ReplaceReferences(IJsonSchema schema, string target)
		{
			var objSchema = schema as ObjectSchema;
			if (objSchema == null) return;
			if (objSchema.Definitions != null)
			{
				foreach (var definition in objSchema.Definitions)
				{
					if ((definition.Definition is JsonSchemaReference) &&
						((JsonSchemaReference) definition.Definition).Reference == target)
					{
						definition.Definition = JsonSchemaReference.Root;
					}
					else
					{
						ReplaceReferences(definition.Definition, target);
					}
				}
			}
			if (objSchema.Properties != null)
			{
				foreach (var property in objSchema.Properties)
				{
					if ((property.Type is JsonSchemaReference) &&
						((JsonSchemaReference)property.Type).Reference == target)
					{
						property.Type = JsonSchemaReference.Root;
					}
					else
					{
						ReplaceReferences(property.Type, target);
					}
				}
			}
		}
	}
}