using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal interface IExpressionTranslator
	{
		ExpressionTreeNode<T> Translate<T>(Expression body);
	}
}