using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Pointer
{
	/// <summary>
	/// Represents a JSON Pointer.
	/// </summary>
	[DebuggerDisplay("{ToString()}")]
	public class JsonPointer : List<string>, IJsonSerializable, IEquatable<JsonPointer>
	{
		private static readonly Regex _generalEscapePattern = new Regex("%(?<Value>[0-9A-F]{2})", RegexOptions.IgnoreCase);

		private bool _usesHash;

		/// <summary>
		/// Creates a new <see cref="JsonPointer"/> instance.
		/// </summary>
		public JsonPointer() { }
		/// <summary>
		/// Creates a new <see cref="JsonPointer"/> instance.
		/// </summary>
		/// <param name="source">A collection of strings representing the segments of the pointer.</param>
		public JsonPointer(params string[] source)
			: this((IEnumerable<string>) source) { }
		/// <summary>
		/// Creates a new <see cref="JsonPointer"/> instance.
		/// </summary>
		/// <param name="source">A collection of strings representing the segments of the pointer.</param>
		public JsonPointer(IEnumerable<string> source)
			: base(source.SkipWhile(s => s == "#"))
		{
			_usesHash = source.FirstOrDefault() == "#";

		}

		/// <summary>
		/// Parses a string containing a JSON Pointer.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <returns>A <see cref="JsonPointer"/> instance.</returns>
		public static JsonPointer Parse(string source)
		{
			var pointer = new JsonPointer();

			var parts = source.Split('/');
			if (parts.Length == 0) return pointer;


			if (parts[0] == "#")
				pointer._usesHash = true;
			else
				parts = parts.Skip(1).ToArray();

			pointer.AddRange(parts.SkipWhile(s => s == "#").Select(_Unescape));

			return pointer;
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
				current = _EvaluateSegment(current, segment);
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
			var asString = _usesHash
				? $"#/{string.Join("/", this.Select(_Escape))}"
				: $"/{string.Join("/", this.Select(_Escape))}";

			return asString.TrimEnd('/');
		}

		public JsonPointer Clone()
		{
			return new JsonPointer(this){_usesHash = _usesHash};
		}

		public JsonPointer CloneAndAppend(params string[] append)
		{
			var clone = new JsonPointer(this){_usesHash = _usesHash};
			clone.AddRange(append);

			return clone;
		}

		private static JsonValue _EvaluateSegment(JsonValue current, string segment)
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

		private static string _Escape(string reference)
		{
			return reference.Replace("~", "~0")
				.Replace("/", "~1");
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

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			AddRange(json.String.Split('/').Skip(1).SkipWhile(s => s == "#").Select(_Unescape));
		}

		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return ToString();
		}
		public bool Equals(JsonPointer other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return _usesHash == other._usesHash &&
			       this.SequenceEqual(other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as JsonPointer);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _usesHash.GetHashCode();
				hashCode = (hashCode * 397) ^ this.GetCollectionHashCode();
				return hashCode;
			}
		}
	}
}
