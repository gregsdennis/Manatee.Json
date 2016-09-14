using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantNumberExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return char.IsDigit(input[0]);
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			double? number;
			var error = source.GetNumber(ref index, out number);
			if (error != null)
			{
				node = null;
				return error;
			}

			node = new ValueExpression<T> {Value = number};
			return null;
		}
	}
}