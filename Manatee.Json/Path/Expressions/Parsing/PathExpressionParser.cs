using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Path.Operators;
using Manatee.Json.Path.Parsing;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal class PathExpressionParser : IJsonPathExpressionParser
	{
		public bool Handles(string input)
		{
			return input[0].In('@', '$');
		}
		public string TryParse<T>(string source, ref int index, out ExpressionTreeNode<T> node)
		{
			var isLocal = source[index] == '@';
			JsonPath path;
			var error = JsonPathParser.Parse(source, ref index, out path);
			// Swallow this error from the path parser and assume the path just ended.
			// If it's really a syntax error, the expression parser should catch it.
			if (error != null && error != "Unrecognized JSON Path element.")
			{
				node = null;
				return error;
			}

			var name = path.Operators.Last() as NameOperator;
			var indexOf = path.Operators.Last() as IndexOfOperator;
			if (name != null && name.Name == "length")
			{
				path.Operators.Remove(name);
				index -= 7; // back up to allow the LengthExpressionParser handle it.
			}
			else if (indexOf != null)
			{
				path.Operators.Remove(indexOf);
				index -= 8; // back up to allow the IndexOfExpressionParser handle it.
			}

			node = new PathExpression<T> {Path = path, IsLocal = isLocal};
			return null;
		}
	}
}