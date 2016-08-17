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
 
	File Name:		ObjectSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		ObjectSchema
	Purpose:		Defines a schema which expects an object.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects an object.
	/// </summary>
	public class ObjectSchema : JsonSchema
	{
		/// <summary>
		/// Used to specify a schema which contains the definitions used by this schema.
		/// </summary>
		/// <remarks>
		/// if left null, the default of http://json-schema.org/draft-04/schema# is used.
		/// </remarks>
		public string Schema { get; set; }
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
		public AdditionalProperties AdditionalProperties { get; set; }
		/// <summary>
		/// Defines additional properties based on regular expression matching of the property name.
		/// </summary>
		public Dictionary<Regex, IJsonSchema> PatternProperties { get; set; }
		/// <summary>
		/// Defines property dependencies.
		/// </summary>
		public Dictionary<string, IEnumerable<string>> Dependencies { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="ObjectSchema"/> class.
		/// </summary>
		public ObjectSchema()
			: base(JsonSchemaTypeDefinition.Object) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			if (json.Type != JsonValueType.Object)
				return new SchemaValidationResults(string.Empty, $"Expected: Object; Actual: {json.Type}.");
			var obj = json.Object;
			var errors = new List<SchemaValidationError>();
			var jValue = root ?? ToJson(null);
			var properties = Properties ?? new JsonSchemaPropertyDefinitionCollection();
			foreach (var property in properties)
			{
				if (!obj.ContainsKey(property.Name))
				{
					if (property.IsRequired)
						errors.Add(new SchemaValidationError(property.Name, "Required property not found."));
					continue;
				}
				var result = property.Type.Validate(obj[property.Name], jValue);
				if (!result.Valid)
					errors.AddRange(result.Errors.Select(e => e.PrependPropertyName(property.Name)));
			}
			var extraData = obj.Where(kvp => properties.All(p => p.Name != kvp.Key))
			                   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			if (AdditionalProperties != null && AdditionalProperties.Equals(AdditionalProperties.False))
			{
				if (PatternProperties != null)
				{
					foreach (var patternProperty in PatternProperties)
					{
						var pattern = patternProperty.Key;
						var schema = patternProperty.Value;
						var matches = extraData.Keys.Where(k => pattern.IsMatch(k));
						foreach (var match in matches)
						{
							var matchErrors = schema.Validate(extraData[match], jValue).Errors;
							errors.AddRange(matchErrors.Select(e => new SchemaValidationError(match, e.Message)));
						}
						extraData = extraData.Where(kvp => !pattern.IsMatch(kvp.Key))
						                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
					}
				}
				errors.AddRange(extraData.Keys.Select(k => new SchemaValidationError(k, "Cannot find a match within Properties or PatternProperties.")));
			}
			else if (AdditionalProperties != null)
			{
				var schema = AdditionalProperties.Definition;
				foreach (var key in extraData.Keys)
				{
					var extraErrors = schema.Validate(extraData[key], jValue).Errors;
					errors.AddRange(extraErrors.Select(e => e.PrependPropertyName(key)));
				}
			}
			return new SchemaValidationResults(errors);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public override void FromJson(JsonValue json, JsonSerializer serializer)
		{
			base.FromJson(json, serializer);
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
					var property = new JsonSchemaPropertyDefinition(prop.Key) {Type = JsonSchemaFactory.FromJson(prop.Value)};
					Properties.Add(property);
				}
			}
			if (obj.ContainsKey("additionalProperties"))
			{
				if (obj["additionalProperties"].Type == JsonValueType.Boolean)
					AdditionalProperties = obj["additionalProperties"].Boolean ? AdditionalProperties.True : AdditionalProperties.False;
				else
				{
					AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaFactory.FromJson(obj["additionalProperties"])};
				}
			}
			if (obj.ContainsKey("patternProperties"))
			{
				var patterns = obj["patternProperties"].Object;
				PatternProperties = patterns.ToDictionary(kvp => new Regex(kvp.Key), kvp => JsonSchemaFactory.FromJson(kvp.Value));
			}
			if (obj.ContainsKey("dependencies"))
			{
				Dependencies = obj["dependencies"].Object.ToDictionary(v => v.Key, v => v.Value.Array.Select(s => s.String));
			}
			if (obj.ContainsKey("required"))
			{
				var requiredProperties = obj["required"].Array.Select(v => v.String);
				foreach (var propertyName in requiredProperties)
				{
					var property = Properties.FirstOrDefault(p => p.Name == propertyName);
					if (property != null)
						property.IsRequired = true;
				}
			}
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			var requiredProperties = new List<string>();
			var json = new JsonObject();
			if (!Id.IsNullOrWhiteSpace()) json["id"] = Id;
			if (!Schema.IsNullOrWhiteSpace()) json["$schema"] = Schema;
			if (!Title.IsNullOrWhiteSpace()) json["title"] = Title;
			if (!Description.IsNullOrWhiteSpace()) json["description"] = Description;
			if (Definitions != null) json["definitions"] = Definitions.ToDictionary(d => d.Name, d => d.Definition).ToJson(serializer);
			if (Type != null) json["type"] = Type.Name;
			if (Properties != null)
			{
				json["properties"] = Properties.ToDictionary(p => p.Name, p => p.Type).ToJson(serializer);
				requiredProperties = Properties.Where(p => p.IsRequired).Select(p => p.Name).ToList();
			}
			if (AdditionalProperties != null)
				json["additionalProperties"] = AdditionalProperties.ToJson(serializer);
			if (PatternProperties != null && PatternProperties.Any())
				json["patternProperties"] = PatternProperties.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value).ToJson(serializer);
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
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public override bool Equals(IJsonSchema other)
		{
			var schema = other as ObjectSchema;
			if (schema == null) return false;
			if (!base.Equals(schema)) return false;
			if (Id != schema.Id) return false;
			if (Schema != schema.Schema) return false;
			if (Title != schema.Title) return false;
			if (Description != schema.Description) return false;
			if (!Definitions.ContentsEqual(schema.Definitions)) return false;
			if (!Properties.ContentsEqual(schema.Properties)) return false;
			if (!(AdditionalProperties == null && schema.AdditionalProperties == null) &&
			    AdditionalProperties != null && !AdditionalProperties.Equals(schema.AdditionalProperties)) return false;
			if (!PatternProperties.ContentsEqual(PatternProperties)) return false;
			return Dependencies.ContentsEqual(schema.Dependencies);
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
				int hashCode = base.GetHashCode();
				hashCode = (hashCode*397) ^ (Schema?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Definitions?.ToList().GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Properties?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (AdditionalProperties?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (PatternProperties?.GetCollectionHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (Dependencies?.GetCollectionHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}