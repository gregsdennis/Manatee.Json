using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal static class JsonPathParser
	{
		private static readonly List<IJsonPathParser> _parsers;

		static JsonPathParser()
		{
			_parsers = typeof(JsonPathParser).GetTypeInfo().Assembly.DefinedTypes
			                                 .Where(t => typeof(IJsonPathParser).GetTypeInfo().IsAssignableFrom(t) && t.IsClass)
			                                 .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                 .Cast<IJsonPathParser>()
			                                 .ToList();
		}

		public static JsonPath Parse(string source)
		{
			var index = 0;
			var errorMessage = Parse(source, ref index, out var path);
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
				var errorMessage = source.SkipWhiteSpace(ref index, length, out _);
				if (errorMessage != null) return errorMessage;

				IJsonPathParser foundParser = null;
				foreach (var parser in _parsers)
				{
					if (parser.Handles(source, index))
					{
						foundParser = parser;
						break;
					}
				}

				if (foundParser == null) return "Unrecognized JSON Path element.";

				errorMessage = foundParser.TryParse(source, ref index, ref path);
				if (errorMessage != null) return errorMessage;
			}

			return null;
		}
	}
}
