using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Results object for schema meta-validations.
	/// </summary>
	public class MetaSchemaValidationResults : IJsonSerializable
	{
		/// <summary>
		/// Gets or sets the JSON Schema draft versions supported by this schema.
		/// </summary>
		public JsonSchemaVersion SupportedVersions { get; set; }
		/// <summary>
		/// Gets a set of results produced by validating this schema against the draft meta-schemas.
		/// </summary>
		public Dictionary<string, SchemaValidationResults> MetaSchemaValidations { get; } = new Dictionary<string, SchemaValidationResults>();
		/// <summary>
		/// Gets other errors that may have been found.
		/// </summary>
		public List<string> OtherErrors { get; } = new List<string>();

		/// <summary>
		/// Gets whether this schema is valid according to the drafts.
		/// </summary>
		public bool IsValid => MetaSchemaValidations.All(v => v.Value.IsValid) &&
		                       !OtherErrors.Any();

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new System.NotImplementedException();
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject
				{
					["supportedVersions"] = serializer.Serialize(SupportedVersions),
					["valid"] = IsValid,
					["validations"] = serializer.Serialize(MetaSchemaValidations),
					["otherErrors"] = OtherErrors.ToJson()
				};
		}
	}
}