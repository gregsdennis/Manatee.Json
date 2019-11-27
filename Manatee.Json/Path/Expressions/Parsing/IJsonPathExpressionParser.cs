using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal interface IJsonPathExpressionParser
	{
		bool Handles(string input, int index);

		bool TryParse<TIn>(string source, ref int index, [NotNullWhen(true)] out JsonPathExpression? expression, [NotNullWhen(false)] out string? errorMessage);
	}
}
