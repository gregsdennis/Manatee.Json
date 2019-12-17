using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
			if (!TryParse(source, ref index, out var path, out var errorMessage))
				throw new JsonPathSyntaxException(path, errorMessage);

			return path;
		}

		public static bool TryParse(string source, ref int index, [NotNullWhen(true)] out JsonPath? path, [NotNullWhen(false)] out string? errorMessage)
		{
			var length = source.Length;
			path = null!;
			while (index < length)
			{
				errorMessage = source.SkipWhiteSpace(ref index, length, out _);
				if (errorMessage != null) return false;

				var i = index;
				var foundParser = _parsers.FirstOrDefault(p => p.Handles(source, i));

				if (foundParser == null)
				{
					errorMessage = "Unrecognized JSON Path element.";
					return false;
				}

				;
				if (!foundParser.TryParse(source, ref index, ref path, out errorMessage)) return false;
			}

			errorMessage = null!;
			return true;
		}
	}
}
