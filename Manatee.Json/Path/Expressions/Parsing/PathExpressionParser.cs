using System;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Path.ArrayParameters;
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
			var length = path.Operators.Last() as LengthOperator;
			var array = path.Operators.Last() as ArrayOperator;
			if (name != null)
			{
				path.Operators.Remove(name);
				node = new NameExpression<T>
					{
						Path = path,
						IsLocal = isLocal,
						Name = name.Name
					};
			}
			else if (indexOf != null)
			{
				path.Operators.Remove(indexOf);
				// TODO: Get indexOf parameter
				node = new IndexOfExpression<T>
					{
						Path = path,
						IsLocal = isLocal
					};
			}
			else if (length != null)
			{
				path.Operators.Remove(length);
				node = new LengthExpression<T>
					{
						Path = path,
						IsLocal = isLocal
					};
			}
			else if (array != null)
			{
				index--;
				path.Operators.Remove(array);
				var query = array.Query as SliceQuery;
				var constant = query?.Slices.FirstOrDefault()?.Index;
				if (query == null || query.Slices.Count() != 1 || !constant.HasValue)
				{
					node = null;
					return "JSON Path expression indexers only support single constant values.";
				}
				node = new ArrayIndexExpression<T>
					{
						Path = path,
						IsLocal = isLocal,
						Index = constant.Value
					};
			}
			else
				throw new NotImplementedException();

			return null;
		}
	}
}