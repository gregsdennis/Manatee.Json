using System;
using System.Collections.Generic;
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

		internal static readonly IEnumerable<JsonSchemaTypeDefinition> PrimitiveDefinitions = new[]
			{
				Array,
				Boolean,
				Integer,
				Null,
				Number,
				Object,
				String
			};

		private IJsonSchema _definition;
		private bool _isReadOnly;

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
					throw new ReadOnlyException($"The '{Name}' member is not editable.");
				_definition = value;
			}
		}

		static JsonSchemaTypeDefinition()
		{
			Array.Definition = new JsonSchema {Type = Array};
			Array._isReadOnly = true;
			Boolean.Definition = new JsonSchema {Type = Boolean};
			Boolean._isReadOnly = true;
			Integer.Definition = new JsonSchema {Type = Integer};
			Integer._isReadOnly = true;
			Null.Definition = new JsonSchema {Type = Null};
			Null._isReadOnly = true;
			Number.Definition = new JsonSchema {Type = Number};
			Number._isReadOnly = true;
			Object.Definition = new JsonSchema {Type = Object};
			Object._isReadOnly = true;
			String.Definition = new JsonSchema {Type = String};
			String._isReadOnly = true;
		}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaTypeDefinition"/> type.
		/// </summary>
		/// <param name="name">The name of the type.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null, empty, or whitespace.</exception>
		public JsonSchemaTypeDefinition(string name)
		{
			if (name.IsNullOrWhiteSpace())
				throw new ArgumentNullException(nameof(name));

			Name = name;
		}
		internal JsonSchemaTypeDefinition(IJsonSchema definition = null)
		{
			Definition = definition;
		}

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public virtual void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.String)
			{
				Name = json.String;
				Definition = new JsonSchema {Type = String, Pattern = json.String};
				return;
			}
			var details = json.Object.First();
			Name = details.Key;
			Definition = new JsonSchema();
			Definition.FromJson(details.Value, null);
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			if (!Name.IsNullOrWhiteSpace()) return Name;

			return ToJson(null).ToString();
		}

		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public virtual JsonValue ToJson(JsonSerializer serializer)
		{
			if (Definition == null || _isReadOnly) return Name;
			if (Name == null) return Definition.ToJson(null);
			return new JsonObject {{Name, Definition.ToJson(null)}};
		}
		/// <summary>
		/// Determines whether the specified <see cref="JsonSchemaTypeDefinition"/> is equal to the current <see cref="JsonSchemaTypeDefinition"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="JsonSchemaTypeDefinition"/> is equal to the current <see cref="JsonSchemaTypeDefinition"/>; otherwise, false.
		/// </returns>
		/// <param name="other">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		protected bool Equals(JsonSchemaTypeDefinition other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (PrimitiveDefinitions.Any(d => ReferenceEquals(d, this) || ReferenceEquals(d, other))) return false;
			return Equals(_definition, other._definition);
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			return Equals(obj as JsonSchemaTypeDefinition);
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			if (_isReadOnly) return Name?.GetHashCode() ?? 0;
			return _definition?.GetHashCode() ?? 0;
		}
	}
}