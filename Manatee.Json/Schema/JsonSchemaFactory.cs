/***************************************************************************************

	Copyright 2016 Greg Dennis

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
using Manatee.Json.Internal;
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

#if !IOS
		private static readonly object LoadLock = new object();

		/// <summary>
		/// Returns
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IJsonSchema Load(string path)
		{
			return Load(new Uri(System.IO.Path.GetFullPath(path)));
		}

		/// <summary>
		/// Returns
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IJsonSchema Load(Uri uri)
		{

				var schemaJson = JsonSchemaOptions.Download(uri.ToString());
			    var schemaValue = JsonValue.Parse(schemaJson);
				var validation = JsonSchema.Draft04.Validate(schemaValue);

				if (!validation.Valid)
				{
					var errors = validation.Errors.Select(e => e.Message).Join(Environment.NewLine);
					throw new ArgumentException($"The given path does not contain a valid schema.  Errors: \n{errors}");
				}

			return FromJson(schemaValue, uri);

		}
#endif
		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json)
		{
			return FromJson(json, null);
		}
		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json, Uri documentPath)
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