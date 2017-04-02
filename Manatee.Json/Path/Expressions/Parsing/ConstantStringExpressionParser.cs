using Manatee.Json.Internal;
using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class ConstantStringExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input[0].In('"', '\'');
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			string value;
			var error = source.GetKey(ref index, out value);
			if (error != null)
			{
				node = null;
				return error;
			}
			node = new ValueExpression<T> {Value = value};
			return null;
		}
	}
}
