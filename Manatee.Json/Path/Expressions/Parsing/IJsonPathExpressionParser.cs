namespace Manatee.Json.Path.Expressions.Parsing
{
	internal interface IJsonPathExpressionParser
	{
		bool Handles(string input, int index);

		string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node);
	}
}
