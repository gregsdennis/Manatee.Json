using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal static class JsonParser
	{
		private static readonly List<IJsonParser> _parsers;

		static JsonParser()
		{
			_parsers = typeof(JsonParser).GetTypeInfo().Assembly.DefinedTypes
			                             .Where(t => typeof(IJsonParser).GetTypeInfo().IsAssignableFrom(t) && t.IsClass)
			                             .Select(ti => Activator.CreateInstance(ti.AsType()))
			                             .Cast<IJsonParser>()
			                             .ToList();
		}

		public static JsonValue Parse(string source)
		{
			var index = 0;
			var errorMessage = Parse(source, ref index, out JsonValue value);
			if (errorMessage != null)
				throw new JsonSyntaxException(source, errorMessage, value);
			return value;
		}
		public static JsonValue Parse(TextReader stream)
		{
			var errorMessage = Parse(stream, out JsonValue value);
			if (errorMessage != null)
				throw new JsonSyntaxException(errorMessage, value);
			return value;
		}
		public static async Task<JsonValue> ParseAsync(TextReader stream, CancellationToken token = default(CancellationToken))
		{
			var (errorMessage, value) = await TryParseAsync(stream, token);
			if (errorMessage != null)
				throw new JsonSyntaxException(errorMessage, value);
			return value;
		}
		public static string Parse(string source, ref int index, out JsonValue value, bool allowExtraChars = false)
		{
			var length = source.Length;
			var errorMessage = source.SkipWhiteSpace(ref index, length, out char c);
			if (errorMessage != null)
			{
				value = null;
				return errorMessage;
			}

			foreach (var parser in _parsers)
			{
				if (parser.Handles(c))
					return parser.TryParse(source, ref index, out value, allowExtraChars);
			}

			value = null;
			return "Cannot determine type.";
		}
		public static string Parse(TextReader stream, out JsonValue value)
		{
			var errorMessage = stream.SkipWhiteSpace(out char c);
			if (errorMessage != null)
			{
				value = null;
				return errorMessage;
			}

			foreach (var parser in _parsers)
			{
				if (parser.Handles(c))
					return parser.TryParse(stream, out value);
			}

			value = null;
			return "Cannot determine type.";
		}
		public static async Task<(string errorMessage, JsonValue value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			var errorMessage = stream.SkipWhiteSpace(out char c);
			if (errorMessage != null)
			{
				return (errorMessage, null);
			}

			foreach (var parser in _parsers)
			{
				if (parser.Handles(c))
					return await parser.TryParseAsync(stream, token);
			}

			return ("Cannot determine type.", null);
		}
	}
}