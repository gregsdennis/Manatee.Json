/***************************************************************************************

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

using System.Data;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class AdditionalProperties : IJsonSerializable
	{
		public static readonly AdditionalProperties True;

		public static readonly AdditionalProperties False;

		private bool _isReadOnly;
		private IJsonSchema _definition;

		public IJsonSchema Definition
		{
			get { return _definition; }
			set
			{
				if (_isReadOnly)
					throw new ReadOnlyException(string.Format("The '{0}' member is not editable.", Equals(True) ? "True" : "False"));
				_definition = value;
			}
		}

		static AdditionalProperties()
		{
			True = new AdditionalProperties {Definition = new ObjectSchema(), _isReadOnly = true};
			False = new AdditionalProperties();
		}

		public bool Equals(AdditionalProperties other)
		{
			return Equals(_definition, other._definition);
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((AdditionalProperties) obj);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return (_isReadOnly.GetHashCode() * 397) ^ (_definition != null ? _definition.GetHashCode() : 0);
			}
		}
		public override string ToString()
		{
			if (Equals(True)) return ((JsonValue) true).ToString();
			if (Equals(False)) return ((JsonValue) false).ToString();
			return Definition.ToString();
		}
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
		public JsonValue ToJson(JsonSerializer serializer)
		{
			if (Equals(True)) return true;
			if (Equals(False)) return false;
			return Definition.ToJson(serializer);
		}
	}
}