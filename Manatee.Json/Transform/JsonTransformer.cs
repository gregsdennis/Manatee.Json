using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Path;

namespace Manatee.Json.Transform
{
	public static class JsonTransformer
	{
		public static JsonValue Transform(this JsonValue source, JsonValue template)
		{
			switch (template.Type)
			{
				case JsonValueType.Number:
				case JsonValueType.Boolean:
				case JsonValueType.Null:
					return template;
				case JsonValueType.String:
					return TransformString(source, template.String);
				case JsonValueType.Object:
					return TransformObject(source, template.Object);
				case JsonValueType.Array:
					return TransformArray(source, template.Array);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static JsonValue TransformObject(JsonValue source, JsonObject template)
		{
			var obj = new JsonObject();
			foreach (var property in template)
			{
				obj[property.Key] = Transform(source, property.Value);
			}
			return obj;
		}
		private static JsonValue TransformArray(JsonValue source, JsonArray template)
		{
			if (template.Count != 2 || template[0].Type != JsonValueType.String)
				throw new ArgumentException("Arrays in templates must contain only two values: an item source path, and a template for each item.");
			var path = JsonPath.Parse(template[0].String);
			var items = path.Evaluate(source);
			return new JsonArray(items.Select(item => Transform(item, template[1])));
		}
		private static JsonValue TransformString(JsonValue source, string template)
		{
			JsonPath path;
			try
			{
				path = JsonPath.Parse(template);
			}
			catch
			{
				return template;
			}
			var items = path.Evaluate(source);
			return items.Count == 1 ? items[0] : items;
		}
	}
}
