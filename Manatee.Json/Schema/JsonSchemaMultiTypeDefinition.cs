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
		private IEnumerable<IJsonSchema> _definitions;

		public JsonSchemaMultiTypeDefinition(IEnumerable<IJsonSchema> definitions)
		{
			_definitions = definitions.ToList();

			if (_definitions.Any(d => PrimitiveDefinitions.All(p => p.Definition.GetType() != d.GetType())))
				throw new InvalidOperationException("Only primitive types are allowed in type collections.");

			Definition = new OneOfSchema {OneOf = _definitions};
		}

		public void AppendJson(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type != JsonValueType.Object) return;

			// we want to reverse so that the first entry has priority
			var properties = _definitions.Select(d => d.ToJson(serializer))
			                             .SelectMany(jv => jv.Object)
										 .GroupBy(kvp => kvp.Key)
										 .Select(g => g.First())
			                             .ToList();
			foreach (var property in properties)
			{
				json.Object[property.Key] = property.Value;
			}
		}

		public override void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var typeEntry = json.Object["type"].Array;
			var jsonWithoutType = json.Object.Where(kvp => kvp.Key != "type").ToJson();
			_definitions = typeEntry.Select(jv =>
				{
					var schema = JsonSchemaFactory.GetPrimitiveSchema(jv.String);
					schema.FromJson(jsonWithoutType, serializer);
					return schema;
				}).ToList();

			Definition = new OneOfSchema {OneOf = _definitions};
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
			if (obj.GetType() != this.GetType()) return false;
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