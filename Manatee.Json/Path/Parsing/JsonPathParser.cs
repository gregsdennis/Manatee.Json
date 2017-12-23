using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal static class JsonPathParser
	{
		private static readonly List<IJsonPathParser> Parsers;

		static JsonPathParser()
		{
			Parsers = typeof(JsonPathParser).GetTypeInfo().Assembly.DefinedTypes
			                                .Where(t => typeof(IJsonPathParser).GetTypeInfo().IsAssignableFrom(t) && t.IsClass)
			                                .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                .Cast<IJsonPathParser>()
			                                .ToList();
		}

		public static JsonPath Parse(string source)
		{
			var index = 0;
			var errorMessage = Parse(source, ref index, out JsonPath path);
			if (errorMessage != null)
				throw new JsonPathSyntaxException(path, errorMessage);
			return path;
		}
		public static string Parse(string source, ref int index, out JsonPath path)
		{
			var length = source.Length;
			path = null;
			while(index < length)
			{
				var errorMessage = source.SkipWhiteSpace(ref index, length, out char c);
				if (errorMessage != null) return errorMessage;

				var substring = source.Substring(index);

				IJsonPathParser foundParser = null;
				foreach (var parser in Parsers)
				{
					if (parser.Handles(source, index))
					{
						foundParser = parser;
						break;
					}
				}

				if (foundParser == null)
					return "Unrecognized JSON Path element.";

				errorMessage = foundParser.TryParse(source, ref index, ref path);
				if (errorMessage != null) return errorMessage;
			}

			return null;
		}
	}
}
