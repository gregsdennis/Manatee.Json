using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal static class JsonPathExpressionParser
	{
		private static readonly List<IJsonPathExpressionParser> Parsers;

		static JsonPathExpressionParser()
		{
			Parsers = typeof(JsonPathExpressionParser).Assembly.GetTypes()
			                                          .Where(t => typeof(IJsonPathExpressionParser).IsAssignableFrom(t) && t.IsClass)
			                                          .Select(Activator.CreateInstance)
			                                          .Cast<IJsonPathExpressionParser>()
			                                          .ToList();
		}

		public static string Parse<T, TIn>(string source, ref int index, out Expression<T, TIn> expr)
		{
			var length = source.Length;
			expr = null;
			while (index < length)
			{
				char c;
				var errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return errorMessage;
				var i = index;
				var parser = Parsers.FirstOrDefault(p => p.Handles(source[i]));
				if (parser == null) return "Unrecognized JSON Path Expression element.";
				errorMessage = parser.TryParse(source, ref index, ref expr);
				if (errorMessage != null) return errorMessage;
			}
			return null;
		}
	}
}