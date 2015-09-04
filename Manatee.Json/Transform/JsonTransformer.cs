using System;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Path;

namespace Manatee.Json.Transform
{
	public static class JsonTransformer
	{
		public static JsonValue Transform(this JsonValue source, JsonValue template)
		{
			return Transform(source, source, template, -1);
		}

		private static JsonValue Transform(this JsonValue source, JsonValue localSource, JsonValue template, int index)
		{
			switch (template.Type)
			{
				case JsonValueType.Number:
				case JsonValueType.Boolean:
				case JsonValueType.Null:
					return template;
				case JsonValueType.String:
					return TransformString(source, localSource, template.String, index);
				case JsonValueType.Object:
					return TransformObject(source, localSource, template.Object, index);
				case JsonValueType.Array:
					return TransformArray(source, localSource, template.Array, index);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static JsonValue TransformObject(JsonValue source, JsonValue localSource, JsonObject template, int index)
		{
			var obj = new JsonObject();
			foreach (var property in template)
			{
				obj[property.Key] = Transform(source, localSource, property.Value, index);
			}
			return obj;
		}
		private static JsonValue TransformArray(JsonValue source, JsonValue localSource, JsonArray template, int index)
		{
			if (template.Count != 2 || template[0].Type != JsonValueType.String)
				return TransformArrayElements(source, localSource, template, index);
			var path = TryGetPath(template[0].String);
			if (path == null)
				return TransformArrayElements(source, localSource, template, index);
			var items = path.Evaluate(source);
			return new JsonArray(items.Select((item, i) => Transform(source, item, template[1], i)));
		}
		private static JsonValue TransformArrayElements(JsonValue source, JsonValue localSource, JsonArray array, int index)
		{
			return new JsonArray(array.Select(jv => Transform(source, localSource, jv, index)));
		}
		private static JsonValue TransformString(JsonValue source, JsonValue localSource, string template, int index)
		{
			var path = TryGetPath(template);
			if (path == null) return template;
			var pathIsLocal = template[0] == '@';
			var items = path.Evaluate(pathIsLocal ? localSource : source);
			return items.Count == 1 ? items[0] : (index == -1 ? items : items[index]);
		}

		private static JsonPath TryGetPath(string text)
		{
			if (text.IsNullOrWhiteSpace()) return null;
			if (text[0] == '@')
				text = "$" + text.Substring(1);
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
