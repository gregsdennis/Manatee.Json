namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class AndExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (index + 1 >= input.Length)
				return false;

			return input[index] == '&'
				&& input[index + 1] == '&';
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index += 2;
			node = new AndExpression<T>();
			return null;
		}
	}
}