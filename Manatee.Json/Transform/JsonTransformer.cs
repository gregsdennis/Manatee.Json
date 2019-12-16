using System;
using System.Linq;
using Manatee.Json.Path;

namespace Manatee.Json.Transform
{
	/// <summary>
	/// Performs transformations between JSON schemas.
	/// </summary>
	public static class JsonTransformer
	{
		/// <summary>
		/// Performs transformations between JSON schemas.
		/// </summary>
		/// <param name="source">The source JSON.</param>
		/// <param name="template">A template defining the transformation.</param>
		/// <returns>The transformed JSON.</returns>
		public static JsonValue Transform(this JsonValue source, JsonValue template)
		{
			return _Transform(source, source, template, -1);
		}

		private static JsonValue _Transform(this JsonValue source, JsonValue localSource, JsonValue template, int index)
		{
			switch (template.Type)
			{
				case JsonValueType.Number:
				case JsonValueType.Boolean:
				case JsonValueType.Null:
					return template;
				case JsonValueType.String:
					return _TransformString(source, localSource, template.String, index);
				case JsonValueType.Object:
					return _TransformObject(source, localSource, template.Object, index);
				case JsonValueType.Array:
					return _TransformArray(source, localSource, template.Array, index);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static JsonValue _TransformObject(JsonValue source, JsonValue localSource, JsonObject template, int index)
		{
			var obj = new JsonObject();
			foreach (var property in template)
			{
				obj[property.Key] = _Transform(source, localSource, property.Value, index);
			}
			return obj;
		}
		private static JsonValue _TransformArray(JsonValue source, JsonValue localSource, JsonArray template, int index)
		{
			if (template.Count != 2 || template[0].Type != JsonValueType.String)
				return _TransformArrayElements(source, localSource, template, index);
			var path = _TryGetPath(template[0].String);
			if (path == null)
				return _TransformArrayElements(source, localSource, template, index);
			var items = path.Evaluate(source);
			return new JsonArray(items.Select((item, i) => _Transform(source, item, template[1], i)));
		}
		private static JsonValue _TransformArrayElements(JsonValue source, JsonValue localSource, JsonArray array, int index)
		{
			return new JsonArray(array.Select(jv => _Transform(source, localSource, jv, index)));
		}
		private static JsonValue _TransformString(JsonValue source, JsonValue localSource, string template, int index)
		{
			var path = _TryGetPath(template);
			if (path == null) return template;
			var pathIsLocal = template[0] == '@';
			var items = path.Evaluate(pathIsLocal ? localSource : source);
			return items.Count == 1 ? items[0] : (index == -1 ? items : items[index]);
		}

		private static JsonPath? _TryGetPath(string text)
		{
			if (string.IsNullOrWhiteSpace(text)) return null;
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
