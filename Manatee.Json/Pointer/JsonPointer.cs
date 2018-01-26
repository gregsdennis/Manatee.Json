using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manatee.Json.Pointer
{
	/// <summary>
	/// Represents a JSON Pointer.
	/// </summary>
	public class JsonPointer : List<string>
	{
		private static readonly Regex _generalEscapePattern = new Regex("%(?<Value>[0-9A-F]{2})", RegexOptions.IgnoreCase);

		/// <summary>
		/// Creates a new <see cref="JsonPointer"/> instance.
		/// </summary>
		public JsonPointer() { }
		/// <summary>
		/// Creates a new <see cref="JsonPointer"/> instance.
		/// </summary>
		/// <param name="source">A collection of strings representing the segments of the pointer.</param>
		public JsonPointer(IEnumerable<string> source) : base(source) { }

		/// <summary>
		/// Parses a string containing a JSON Pointer.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>A <see cref="JsonPointer"/> instance.</returns>
		public static JsonPointer Parse(string source)
		{
			return new JsonPointer(source.Split('/').Skip(1).SkipWhile(s => s == "#").Select(_Unescape));
		}

		/// <summary>
		/// Evaluates the pointer against a JSON instance.
		/// </summary>
		/// <param name="root">The JSON instance.</param>
		/// <returns>The element the pointer references, if any.</returns>
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

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
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
