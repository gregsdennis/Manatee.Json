using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manatee.Json.Pointer
{
	public class JsonPointer : List<string>
	{
		private static readonly Regex _generalEscapePattern = new Regex("%(?<Value>[0-9A-F]{2})", RegexOptions.IgnoreCase);

		public JsonPointer() { }
		public JsonPointer(IEnumerable<string> source) : base(source) { }

		public static JsonPointer Parse(string source)
		{
			return new JsonPointer(source.Split('/').Skip(1).SkipWhile(s => s == "#").Select(_Unescape));
		}

		public PointerEvaluationResults Evaluate(JsonValue root)
		{
			var upTo = new JsonPointer();
			var current = root;
			foreach (var segment in this)
			{
				upTo.Add(segment);
				current = _EvaulateSegment(current, segment);
				if (current == null)
					return new PointerEvaluationResults($"No value found at '{upTo}'");
			}

			return new PointerEvaluationResults(current);
		}

		public override string ToString()
		{
			return "/" + string.Join("/", this);
		}

		private static JsonValue _EvaulateSegment(JsonValue current, string segment)
		{
			if (current.Type == JsonValueType.Array)
			{
				if (int.TryParse(segment, out var index))
				{
					return (segment != "0" && segment.StartsWith("0")) ||
					       (0 > index || index >= current.Array.Count)
						       ? null
						       : current.Array[index];
				}

				if (segment == "-")
					return current.Array[current.Array.Count-1];
			}

			return current.Type != JsonValueType.Object || !current.Object.TryGetValue(segment, out var value)
				       ? null
				       : value;
		}

		private static string _Unescape(string reference)
		{
			var unescaped = reference.Replace("~1", "/")
			                         .Replace("~0", "~");
			var matches = _generalEscapePattern.Matches(unescaped);
			foreach (Match match in matches)
			{
				var value = int.Parse(match.Groups["Value"].Value, NumberStyles.HexNumber);
				var ch = (char)value;
				unescaped = Regex.Replace(unescaped, match.Value, new string(ch, 1));
			}
			return unescaped;
		}
	}
}
