using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class LengthExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			bool isLocal;
			var method = body as MethodCallExpression;
			return method == null
				       ? new LengthExpression<T>()
				       : new LengthExpression<T>
					       {
						       Path = BuildPath(method, out isLocal),
						       IsLocal = isLocal
					       };
		}
	}
}