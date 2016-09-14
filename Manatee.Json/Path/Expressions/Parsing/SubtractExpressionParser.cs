namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class SubtractExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			// TODO: Determine how to identify negations separately from subtractions.
			return input.StartsWith("-");
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			index++;
			node = new SubtractExpression<T>();
			return null;
		}
	}
}