using System;
using System.Diagnostics;
using System.Linq;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class PropertyNamesKeyword : IJsonSchemaKeyword, IEquatable<PropertyNamesKeyword>
	{
		public virtual string Name => "propertyNames";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonSchema Value { get; private set; }

		public PropertyNamesKeyword() { }
		public PropertyNamesKeyword(JsonSchema value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Object) return SchemaValidationResults.Valid;

			var results = context.Instance.Object.Keys.Select(propertyName =>
				{
					var newContext = new SchemaValidationContext
						{
							BaseUri = context.BaseUri,
							Instance = propertyName,
							Root = context.Root
						};
					var result = Value.Validate(newContext);

					return result;
				});

			return new SchemaValidationResults(results);
		}
		public void RegisterSubschemas(Uri baseUri)
		{
			Value.RegisterSubschemas(baseUri);
		}
		public JsonSchema ResolveSubschema(JsonPointer pointer, Uri baseUri)
		{
			return Value.ResolveSubschema(pointer, baseUri);
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = serializer.Deserialize<JsonSchema>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value.ToJson(serializer);
		}
		public bool Equals(PropertyNamesKeyword other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public bool Equals(IJsonSchemaKeyword other)
		{
			return Equals(other as PropertyNamesKeyword);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as PropertyNamesKeyword);
		}
		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}
	}
}