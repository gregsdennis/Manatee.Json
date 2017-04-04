using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a single schema enumeration value.
	/// </summary>
	public class EnumSchemaValue : IJsonSerializable, IEquatable<EnumSchemaValue>
	{
		private JsonValue _value;

		/// <summary>
		/// Creates a new instance of the <see cref="EnumSchemaValue"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public EnumSchemaValue(JsonValue value)
		{
			_value = value ?? JsonValue.Null;
		}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <returns>The results of the validation.</returns>
		public SchemaValidationResults Validate(JsonValue json)
		{
			if (json == _value) return new SchemaValidationResults();
			return new SchemaValidationResults("value", $"'{json}' does not match the required value.");
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(EnumSchemaValue other)
		{
			return other != null && other._value == _value;
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			_value = json;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return _value;
		}
	}
}