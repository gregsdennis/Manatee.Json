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
 
	File Name:		JsonSchemaMultiTypeDefinition.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaMultiTypeDefinition
	Purpose:		Supports a collection of types.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	internal class JsonSchemaMultiTypeDefinition : JsonSchemaTypeDefinition
	{
		private readonly bool _nonPrimitiveAllowed;
		private IEnumerable<JsonSchemaTypeDefinition> _definitions;

		public JsonSchemaMultiTypeDefinition(params JsonSchemaTypeDefinition[] definitions)
			: this(false)
		{
			_definitions = definitions.ToList();

			if (_definitions.Except(PrimitiveDefinitions).Any())
				throw new InvalidOperationException("Only primitive types are allowed in type collections.");

			Definition = new JsonSchema { OneOf = _definitions.Select(d => d.Definition) };
		}
		internal JsonSchemaMultiTypeDefinition(bool nonPrimitiveAllowed)
		{
			_nonPrimitiveAllowed = nonPrimitiveAllowed;
		}

		public override void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var typeEntry = json.Array;
			_definitions = typeEntry.Select(jv =>
				{
					if (_nonPrimitiveAllowed) return new JsonSchemaTypeDefinition(JsonSchemaFactory.FromJson(jv));
					var definition = PrimitiveDefinitions.FirstOrDefault(p => p.Name == jv.String);
					if (definition == null)
						throw new InvalidOperationException("Only primitive types are allowed in type collections.");
					return definition;
				}).ToList();

			Definition = new JsonSchema {OneOf = _definitions.Select(d => d.Definition)};
		}
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			return _definitions.ToJson(serializer);
		}
		protected bool Equals(JsonSchemaMultiTypeDefinition other)
		{
			return base.Equals(other) && _definitions.ContentsEqual(other._definitions);
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((JsonSchemaMultiTypeDefinition) obj);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode()*397) ^ (_definitions?.GetCollectionHashCode() ?? 0);
			}
		}
	}
}