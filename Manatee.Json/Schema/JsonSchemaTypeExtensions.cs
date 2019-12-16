using System;
using System.Linq;

namespace Manatee.Json.Schema
{
	internal static class JsonSchemaTypeExtensions
	{
		public static JsonValue ToJson(this JsonSchemaType type)
		{
			var allValues = Enum.GetValues(typeof(JsonSchemaType)).Cast<JsonSchemaType>()
			                    .Where(v => v != JsonSchemaType.NotDefined);
			var types = allValues.Where(v => type.HasFlag(v))
			                     .Select(_TranslateSingleType)
			                     .ToList();
			if (types.Count == 1)
				return types[0];

			var array = types.ToJson();
			array.Array.EqualityStandard = ArrayEquality.ContentsEqual;

			return array;
		}

		public static JsonSchemaType ToSchemaType(this JsonValue json)
		{
			switch (json.Type)
			{
				case JsonValueType.Array:
					return json.Array.Aggregate(JsonSchemaType.NotDefined, (c, jv) => c | _TranslateSingleType(jv.String));
				case JsonValueType.String:
					return _TranslateSingleType(json.String);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static JsonSchemaType _TranslateSingleType(string type)
		{
			switch (type)
			{
				case "array":
					return JsonSchemaType.Array;
				case "boolean":
					return JsonSchemaType.Boolean;
				case "integer":
					return JsonSchemaType.Integer;
				case "null":
					return JsonSchemaType.Null;
				case "number":
					return JsonSchemaType.Number;
				case "object":
					return JsonSchemaType.Object;
				case "string":
					return JsonSchemaType.String;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static string? _TranslateSingleType(JsonSchemaType type)
		{
			switch (type)
			{
				case JsonSchemaType.NotDefined:
					return null;
				case JsonSchemaType.Array:
					return "array";
				case JsonSchemaType.Boolean:
					return "boolean";
				case JsonSchemaType.Integer:
					return "integer";
				case JsonSchemaType.Null:
					return "null";
				case JsonSchemaType.Number:
					return "number";
				case JsonSchemaType.Object:
					return "object";
				case JsonSchemaType.String:
					return "string";
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}