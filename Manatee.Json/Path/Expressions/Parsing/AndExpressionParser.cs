namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class AndExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("&&");
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index += 2;
			node = new AndExpression<T>();
			return null;
		}
	}
}