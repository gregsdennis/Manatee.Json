using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal static class JsonParser
	{
		private static readonly List<IJsonParser> Parsers;

		static JsonParser()
		{
			Parsers = typeof(JsonParser).TypeInfo().Assembly.DefinedTypes
				// TODO: optimize
			                            .Where(t => typeof(IJsonParser).GetTypeInfo().IsAssignableFrom(t) && t.IsClass)
			                            .Select(ti => Activator.CreateInstance(ti.AsType()))
			                            .Cast<IJsonParser>()
			                            .ToList();
		}

		public static JsonValue Parse(string source)
		{
			var index = 0;
			JsonValue value;
			var errorMessage = Parse(source, ref index, out value);
			if (errorMessage != null)
				throw new JsonSyntaxException(errorMessage, value);
			return value;
		}
		public static JsonValue Parse(StreamReader stream)
		{
			JsonValue value;
			var errorMessage = Parse(stream, out value);
			if (errorMessage != null)
				throw new JsonSyntaxException(errorMessage, value);
			return value;
		}
		public static string Parse(string source, ref int index, out JsonValue value, bool allowExtraChars = false)
		{
			var length = source.Length;
			char c;
			var errorMessage = source.SkipWhiteSpace(ref index, length, out  c);
			if (errorMessage != null)
			{
				value = null;
				return errorMessage;
			}
			var parser = Parsers.FirstOrDefault(p => p.Handles(c));
			if (parser == null)
			{
				value = null;
				return "Cannot determine type.";
			}
			errorMessage = parser.TryParse(source, ref index, out value, allowExtraChars);
			return errorMessage;
		}
		public static string Parse(StreamReader stream, out JsonValue value)
		{
			char c;
			var errorMessage = stream.SkipWhiteSpace(out  c);
			if (errorMessage != null)
			{
				value = null;
				return errorMessage;
			}
			var parser = Parsers.FirstOrDefault(p => p.Handles(c));
			if (parser == null)
			{
				value = null;
				return "Cannot determine type.";
			}
			errorMessage = parser.TryParse(stream, out value);
			return errorMessage;
		}
	}
}