using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
			if (!TryParse(source, ref index, out var value, out var errorMessage))
				throw new JsonSyntaxException(source, errorMessage, value);

			if (JsonOptions.RequireIsolatedJsonDuringParse && source.Length != index)
				throw new JsonSyntaxException("Content detected after initial JSON value", value);

			return value;
		}
		public static JsonValue Parse(TextReader stream)
		{
			if (!TryParse(stream, out var value, out var errorMessage))
				throw new JsonSyntaxException(errorMessage, value);

			if (JsonOptions.RequireIsolatedJsonDuringParse && stream.Peek() != -1)
				throw new JsonSyntaxException("Content detected after initial JSON value", value);

			return value;
		}
		public static async Task<JsonValue> ParseAsync(TextReader stream, CancellationToken token = default)
		{
			var (errorMessage, value) = await TryParseAsync(stream, token);
			if (errorMessage != null)
				throw new JsonSyntaxException(errorMessage, value);

			if (JsonOptions.RequireIsolatedJsonDuringParse && stream.Peek() != -1)
				throw new JsonSyntaxException("Content detected after initial JSON value", value);

			return value!;
		}
		public static bool TryParse(string source, ref int index, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage, bool allowExtraChars = false)
		{
			var length = source.Length;
			errorMessage = source.SkipWhiteSpace(ref index, length, out var c);
			if (errorMessage != null)
			{
				value = null;
				return false;
			}

			foreach (var parser in _parsers)
			{
				if (parser.Handles(c))
					return parser.TryParse(source, ref index, out value, out errorMessage, allowExtraChars);
			}

			value = null;
			errorMessage = "Cannot determine type.";
			return false;
		}
		public static bool TryParse(TextReader stream, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage)
		{
			errorMessage = stream.SkipWhiteSpace(out var c);
			if (errorMessage != null)
			{
				value = null;
				return false;
			}

			foreach (var parser in _parsers)
			{
				if (parser.Handles(c))
					return parser.TryParse(stream, out value, out errorMessage);
			}

			value = null;
			errorMessage = "Cannot determine type.";
			return false;
		}
		public static async Task<(string? errorMessage, JsonValue? value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			var errorMessage = stream.SkipWhiteSpace(out var c);
			if (errorMessage != null)
				return (errorMessage, null);

			foreach (var parser in _parsers)
			{
				if (parser.Handles(c))
					return await parser.TryParseAsync(stream, token);
			}

			return ("Cannot determine type.", null);
		}
	}
}