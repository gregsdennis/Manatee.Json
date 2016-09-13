namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ExponentExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("^");
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new ExponentExpression<T>();
			return null;
		}
	}
}