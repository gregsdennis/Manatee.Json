using System.Linq.Expressions;
using System.Reflection;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class FieldExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var member = (MemberExpression)body;
			return new FieldExpression<T>
			{
				Field = (FieldInfo)member.Member,
				Source = ((ConstantExpression)member.Expression).Value
			};
		}
	}
}