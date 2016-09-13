﻿/***************************************************************************************

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
 
	File Name:		JsonSchemaPropertyDefinition.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaPropertyDefinition
	Purpose:		Defines a single property within a schema.

***************************************************************************************/

using System;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a single property within a schema.
	/// </summary>
	public class JsonSchemaPropertyDefinition : IJsonSerializable, IEquatable<JsonSchemaPropertyDefinition>
	{
		/// <summary>
		/// Defines the name of the property.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Defines a schema used to represent the type of this property.
		/// </summary>
		public IJsonSchema Type { get; set; }
		/// <summary>
		/// Defines whether this property is required.
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaPropertyDefinition"/> class.
		/// </summary>
		/// <param name="name">The name of the type.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null, empty, or whitespace.</exception>
		public JsonSchemaPropertyDefinition(string name)
		{
			if (name.IsNullOrWhiteSpace())
				throw new ArgumentNullException(nameof(name));

			Name = name;
			Type = JsonSchema.Empty;
		}

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var details = json.Object.First();
			Name = details.Key;
			Type = JsonSchemaFactory.FromJson(details.Value);
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject {{Name, Type.ToJson(serializer)}};
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return ToJson(null).ToString();
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(JsonSchemaPropertyDefinition other)
		{
			return string.Equals(Name, other.Name) &&
			       Equals(Type, other.Type) &&
			       IsRequired == other.IsRequired;
		}
		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
		/// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((JsonSchemaPropertyDefinition) obj);
		}
		/// <summary>Serves as a hash function for a particular type. </summary>
		/// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Name?.GetHashCode() ?? 0;
				hashCode = (hashCode*397) ^ (Type?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ IsRequired.GetHashCode();
				return hashCode;
			}
		}
	}
}