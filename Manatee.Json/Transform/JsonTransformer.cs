using System;
using System.Linq;
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
				return TransformArrayElements(source, template);
			var path = TryGetPath(template[0].String);
			if (path == null)
				return TransformArrayElements(source, template);
			var items = path.Evaluate(source);
			return new JsonArray(items.Select(item => Transform(item, template[1])));
		}
		private static JsonValue TransformArrayElements(JsonValue source, JsonArray array)
		{
			return new JsonArray(array.Select(jv => Transform(source, jv)));
		}
		private static JsonValue TransformString(JsonValue source, string template)
		{
			var path = TryGetPath(template);
			if (path == null) return template;
			var items = path.Evaluate(source);
			return items.Count == 1 ? items[0] : items;
		}

		private static JsonPath TryGetPath(string text)
		{
			try
			{
				return JsonPath.Parse(text);
			}
			catch
			{
				return null;
			}
		}
	}
}
