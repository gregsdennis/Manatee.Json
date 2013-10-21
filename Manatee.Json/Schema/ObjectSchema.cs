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
 
	File Name:		ObjectSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		ObjectSchema
	Purpose:		Defines a schema which expects an object.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Enumerations;
using Manatee.Json.Extensions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects an object.
	/// </summary>
	public class ObjectSchema : JsonSchema
	{
		/// <summary>
		/// Used to specify which this schema defines.
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// Used to specify a schema which contains the definitions used by this schema.
		/// </summary>
		/// <remarks>
		/// if left null, the default of http://json-schema.org/draft-04/schema# is used.
		/// </remarks>
		public string Schema { get; set; }
		/// <summary>
		/// Defines a title for this schema.
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// Defines a description for this schema.
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Defines a collection of schema type definitions.
		/// </summary>
		public JsonSchemaTypeDefinitionCollection Definitions { get; set; }
		/// <summary>
		/// Defines a collection of properties expected by this schema.
		/// </summary>
		public JsonSchemaPropertyDefinitionCollection Properties { get; set; }
		/// <summary>
		/// Defines any additional properties to be expected by this schema.
		/// </summary>
		public IJsonSchema AdditionalProperties { get; set; }
		/// <summary>
		/// Defines property dependencies.
		/// </summary>
		public IDictionary<string, IEnumerable<string>> Dependencies { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="ObjectSchema"/> class.
		/// </summary>
		public ObjectSchema() : base(JsonSchemaTypeDefinition.Object) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a JsonValue.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override bool Validate(JsonValue json, JsonValue root = null)
		{
			if (Properties == null) return true;
			if (json.Type != JsonValueType.Object) return false;
			var obj = json.Object;
			var requiredProperties = Properties.Where(p => p.IsRequired).ToList();
			var otherProperties = Properties.Where(p => !p.IsRequired).ToList();
			var valid = true;
			var jValue = root ?? ToJson();
			foreach (var property in requiredProperties)
			{
				if (!obj.ContainsKey(property.Name)) return false;
				valid &= property.Type.Validate(obj[property.Name], jValue);
			}
			valid &= otherProperties.Where(p => obj.ContainsKey(p.Name)).All(p => p.Type.Validate(obj[p.Name], jValue));
			return valid;
		}
		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public override void FromJson(JsonValue json)
		{
			base.FromJson(json);
			var obj = json.Object;
			Id = obj.TryGetString("id");
			Schema = obj.TryGetString("$schema");
			Title = obj.TryGetString("title");
			Description = obj.TryGetString("description");
			if (obj.ContainsKey("definitions"))
			{
				Definitions = new JsonSchemaTypeDefinitionCollection();
				foreach (var defn in obj["definitions"].Object)
				{
					var definition = new JsonSchemaTypeDefinition(defn.Key) {Definition = JsonSchemaFactory.FromJson(defn.Value)};
					Definitions.Add(definition);
				}
			}
			if (obj.ContainsKey("properties"))
			{
				Properties = new JsonSchemaPropertyDefinitionCollection();
				foreach (var prop in obj["properties"].Object)
				{
					var property = new JsonSchemaPropertyDefinition {Name = prop.Key, Type = JsonSchemaFactory.FromJson(prop.Value)};
					Properties.Add(property);
				}
			}
			if (obj.ContainsKey("additionalProperties")) AdditionalProperties = JsonSchemaFactory.FromJson(obj["additionalProperties"]);
			if (obj.ContainsKey("dependencies"))
			{
				Dependencies = obj["dependencies"].Object.ToDictionary(v => v.Key, v => v.Value.Array.Select(s => s.String));
			}
			if (obj.ContainsKey("required"))
			{
				var requiredProperties = obj["required"].Array.Select(v => v.String);
				foreach (var propertyName in requiredProperties)
				{
					Properties[propertyName].IsRequired = true;
				}
			}
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public override JsonValue ToJson()
		{
			IEnumerable<string> requiredProperties = Enumerable.Empty<string>();
			var json = new JsonObject();
			if (!string.IsNullOrWhiteSpace(Id)) json["id"] = Id;
			if (!string.IsNullOrWhiteSpace(Schema)) json["$schema"] = Schema;
			if (!string.IsNullOrWhiteSpace(Title)) json["title"] = Title;
			if (!string.IsNullOrWhiteSpace(Description)) json["description"] = Description;
			if (Definitions != null) json["definitions"] = Definitions.ToDictionary(d => d.Name, d => d.Definition).ToJson();
			if (Type != null) json["type"] = Type.Name;
			if (Properties != null)
			{
				json["properties"] = Properties.ToDictionary(p => p.Name, p => p.Type).ToJson();
				requiredProperties = Properties.Where(p => p.IsRequired).Select(p => p.Name);
			}
			if (AdditionalProperties != null) json["additionalProperties"] = AdditionalProperties.ToJson();
			if ((Dependencies != null) && Dependencies.Any())
			{
				var jsonDependencies = new JsonObject();
				foreach (var dependency in Dependencies)
				{
					jsonDependencies[dependency.Key] = dependency.Value.ToJson();
				}
				json["dependencies"] = jsonDependencies;
			}
			if (requiredProperties.Any()) json["required"] = requiredProperties.ToJson();
			if (Default != null) json["default"] = Default;
			return json;
		}
	}
}