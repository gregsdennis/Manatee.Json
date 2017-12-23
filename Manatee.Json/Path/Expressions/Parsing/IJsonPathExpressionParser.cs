namespace Manatee.Json.Path.Expressions.Parsing
{
	internal interface IJsonPathExpressionParser
	{
		bool Handles(string input, int index);

		string TryParse<TIn>(string source, ref int index, out JsonPathExpression expression);
	}
}
