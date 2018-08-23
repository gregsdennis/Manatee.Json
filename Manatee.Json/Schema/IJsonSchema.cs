using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a type for all schema to implement.
	/// </summary>
	public interface IJsonSchema : IJsonSerializable, IEquatable<IJsonSchema>
	{
		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		SchemaValidationResults Validate(JsonValue json, JsonValue root = null);
		/// <summary>
		/// Identifies the physical path for the schema document (may be different than the ID).
		/// </summary>
		Uri DocumentPath { get; set; }
		/// <summary>
		/// Used to specify which this schema defines.
		/// </summary>
		string Id { get; set; }
		/// <summary>
		/// Used to specify a schema which contains the definitions used by this schema.
		/// </summary>
		/// <remarks>
		/// if left null, the default of http://json-schema.org/draft-04/schema# is used.
		/// </remarks>
		string Schema { get; set; }
	}

	public class JsonSchema : IJsonSchema
	{
		public static JsonSchema Draft04 = new JsonSchema
			{
				Keywords =
					{
						new TypeKeyword()
					}
			};

		public Uri DocumentPath { get; set; }
		public string Id { get; set; }
		public string Schema { get; set; }
		public List<IJsonSchemaKeyword> Keywords { get; } = new List<IJsonSchemaKeyword>();

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Keywords.Select(k => new KeyValuePair<string, JsonValue>(k.Name, k.ToJson(serializer))).ToJson();
		}
		public bool Equals(IJsonSchema other)
		{
			throw new NotImplementedException();
		}
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			return new SchemaValidationResults(Keywords.Select(k => k.Validate(this, json, null)));
		}
	}

	public interface IJsonSchemaKeyword : IJsonSerializable
	{
		string Name { get; }

		SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root);
	}

	public class TypeKeyword : IJsonSchemaKeyword
	{
		public string Name => "type";

		public JsonSchemaType Type { get; set; }

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			bool valid = true;
			switch (json.Type)
			{
				case JsonValueType.Number:
					if (Type.HasFlag(JsonSchemaType.Number)) break;
					if (json.Number.IsInt() && Type.HasFlag(JsonSchemaType.Integer)) break;
					valid = false;
					break;
				case JsonValueType.String:
					var expected = Type.ToJson();
					if (expected.Type == JsonValueType.String && expected == json) break;
					if (expected.Type == JsonValueType.Array && expected.Array.Contains(json)) break;
					if (Type.HasFlag(JsonSchemaType.String)) break;
					valid = false;
					break;
				case JsonValueType.Boolean:
					if (Type.HasFlag(JsonSchemaType.Boolean)) break;
					valid = false;
					break;
				case JsonValueType.Object:
					if (Type.HasFlag(JsonSchemaType.Object)) break;
					valid = false;
					break;
				case JsonValueType.Array:
					if (Type.HasFlag(JsonSchemaType.Array)) break;
					valid = false;
					break;
				case JsonValueType.Null:
					if (Type.HasFlag(JsonSchemaType.Null)) break;
					valid = false;
					break;
			}

			if (!valid)
			{
				var message = SchemaErrorMessages.Type.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Type,
						["actual"] = json.Type,
						["value"] = json
					});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Type = serializer.Deserialize<JsonSchemaType>(json);
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Type.ToJson();
		}
	}

	public class OtherKeyword : IJsonSchemaKeyword
	{
		public string Name { get; set; }

		public JsonValue Content { get; set; }

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Content = json;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Content;
		}
	}
}