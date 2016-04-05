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
 
	File Name:		EnumSchemaValue.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		EnumSchemaValue
	Purpose:		Defines a single schema enumeration value.

***************************************************************************************/

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