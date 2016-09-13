namespace Manatee.Json.Path.Expressions.Parsing
{
	internal interface IJsonPathExpressionParser
	{
		bool Handles(char input);
		string TryParse<T, TIn>(string source, ref int index, ref Expression<T, TIn> path);
	}
}
