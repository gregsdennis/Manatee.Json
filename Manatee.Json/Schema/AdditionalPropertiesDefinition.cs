﻿/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		AdditionalProperties.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		AdditionalProperties
	Purpose:		Defines additional properties for ObjectSchema.

***************************************************************************************/

using System;
using System.Data;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines additional properties for ObjectSchema.
	/// </summary>
	public class AdditionalProperties : IJsonSerializable, IEquatable<AdditionalProperties>
	{
		/// <summary>
		/// Allows any additional property to be added to the schema.
		/// </summary>
		public static readonly AdditionalProperties True;
		/// <summary>
		/// Prohibits additional properties in the schema.
		/// </summary>
		public static readonly AdditionalProperties False;

		private bool _isReadOnly;
		private IJsonSchema _definition;

		/// <summary>
		/// Defines a schema to which any additional properties must validate.
		/// </summary>
		/// <exception cref="ReadOnlyException">Thrown when attempting to set the definition
		/// of one of the static <see cref="AdditionalProperties"/> fields.</exception>
		public IJsonSchema Definition
		{
			get { return _definition; }
			set
			{
				if (_isReadOnly)
					throw new ReadOnlyException($"The '{(Equals(True) ? "True" : "False")}' member is not editable.");
				_definition = value;
			}
		}

		static AdditionalProperties()
		{
			True = new AdditionalProperties {Definition = new ObjectSchema(), _isReadOnly = true};
			False = new AdditionalProperties {_isReadOnly = true};
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(AdditionalProperties other)
		{
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
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((AdditionalProperties) obj);
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
			unchecked
			{
				return (_isReadOnly.GetHashCode() * 397) ^ (_definition?.GetHashCode() ?? 0);
			}
		}
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			if (Equals(True)) return ((JsonValue) true).ToString();
			if (Equals(False)) return ((JsonValue) false).ToString();
			return Definition.ToString();
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Boolean)
			{
				if (json.Boolean) Definition = new ObjectSchema();
			}
			else
			{
				Definition = JsonSchemaFactory.FromJson(json);
			}
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (Equals(True)) return true;
			if (Equals(False)) return false;
			return Definition.ToJson(serializer);
		}
	}
}