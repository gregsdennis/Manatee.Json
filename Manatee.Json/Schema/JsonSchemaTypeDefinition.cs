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
 
	File Name:		JsonSchemaTypeDefinition.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaTypeDefinition
	Purpose:		Defines a single type definition within a schema.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a single type definition within a schema.
	/// </summary>
	public class JsonSchemaTypeDefinition : IJsonSerializable
	{
		/// <summary>
		/// Defines the array type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Array = new JsonSchemaTypeDefinition("array");
		/// <summary>
		/// Defines the boolean type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Boolean = new JsonSchemaTypeDefinition("boolean");
		/// <summary>
		/// Defines the integer type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Integer = new JsonSchemaTypeDefinition("integer");
		/// <summary>
		/// Defines the null type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Null = new JsonSchemaTypeDefinition("null");
		/// <summary>
		/// Defines the number type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Number = new JsonSchemaTypeDefinition("number");
		/// <summary>
		/// Defines the object type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Object = new JsonSchemaTypeDefinition("object");
		/// <summary>
		/// Defines the string type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition String = new JsonSchemaTypeDefinition("string");

		private bool _isReadOnly;
		private IJsonSchema _definition;

		/// <summary>
		/// Defines the name of the type.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Defines a schema used to define the type.
		/// </summary>
		/// <exception cref="ReadOnlyException">Thrown when attempting to set the definition
		/// of one of the static <see cref="JsonSchemaTypeDefinition"/> fields.</exception>
		public IJsonSchema Definition
		{
			get { return _definition; }
			set
			{
				if (_isReadOnly)
					throw new ReadOnlyException(string.Format("The '{0}' member is not editable.", Name));
				_definition = value;
			}
		}

		static JsonSchemaTypeDefinition()
		{
			Array.Definition = new ArraySchema();
			Array._isReadOnly = true;
			Boolean.Definition = new BooleanSchema();
			Boolean._isReadOnly = true;
			Integer.Definition = new IntegerSchema();
			Integer._isReadOnly = true;
			Null.Definition = new NullSchema();
			Null._isReadOnly = true;
			Number.Definition = new NumberSchema();
			Number._isReadOnly = true;
			Object.Definition = new ObjectSchema();
			Object._isReadOnly = true;
			String.Definition = new StringSchema();
			String._isReadOnly = true;
		}
		internal JsonSchemaTypeDefinition() {}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaTypeDefinition"/> type.
		/// </summary>
		/// <param name="name">The name of the type.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null, empty, or whitespace.</exception>
		public JsonSchemaTypeDefinition(string name)
		{
			if (name.IsNullOrWhiteSpace())
				throw new ArgumentNullException("name");

			Name = name;
		}

		/// <summary>
		/// Builds a <see cref="JsonSchemaTypeDefinition"/> which can be used to represent an enumeration value.
		/// </summary>
		/// <param name="value">A string value.</param>
		/// <returns>A <see cref="JsonSchemaTypeDefinition"/> which can be used in an <see cref="EnumSchema"/>.</returns>
		public static JsonSchemaTypeDefinition CreateEnumValue(string value)
		{
			return new JsonSchemaTypeDefinition(value) {Definition = new StringSchema {Pattern = value}};
		}
		/// <summary>
		/// Builds an <see cref="IEnumerable{T}"/> of <see cref="JsonSchemaTypeDefinition"/> which contains
		/// enumeration definitions for each member of an enumeration type.
		/// </summary>
		/// <typeparam name="T">The enumeration type.</typeparam>
		/// <returns>A collection of <see cref="JsonSchemaTypeDefinition"/>s which can be used in an <see cref="EnumSchema"/>.</returns>
		/// <exception cref="ArgumentException">Thrown when <typeparamref name="T"/> is not an enumeration type.</exception>
		public static IEnumerable<JsonSchemaTypeDefinition> CreateEnumValues<T>()
		{
			if (!typeof(Enum).IsAssignableFrom(typeof(T)))
				throw new ArgumentException("Type '{0}' is not an enumeration.");

			return Enum.GetNames(typeof (T)).Select(CreateEnumValue);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.String)
			{
				Name = json.String;
				Definition = GetWellKnownDefinition(json.String);
				return;
			}
			var details = json.Object.First();
			Name = details.Key;
			Definition = JsonSchemaFactory.FromJson(details.Value);
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (Definition == null || _isReadOnly) return Name;
			return new JsonObject {{Name, Definition.ToJson(null)}};
		}

		private static IJsonSchema GetWellKnownDefinition(string name)
		{
			switch (name)
			{
				case "array":
					return Array.Definition;
				case "boolean":
					return Boolean.Definition;
				case "integer":
					return Integer.Definition;
				case "null":
					return Null.Definition;
				case "number":
					return Number.Definition;
				case "object":
					return Object.Definition;
				case "string":
					return String.Definition;
				default:
					throw new ArgumentOutOfRangeException("name");
			}
		}
	}
}