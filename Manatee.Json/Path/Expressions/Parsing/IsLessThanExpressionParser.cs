namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class IsLessThanExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input, int index)
		{
			if (input[index] != '<') return false;

			return index + 1 >= input.Length || input[index + 1] != '=';
		}

		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new IsLessThanExpression<T>();
			return null;
		}
	}
}