using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal static class ExpressionTranslator
	{
		private static readonly Dictionary<Type, Func<Expression, IExpressionTranslator>> Translators =
			new Dictionary<Type, Func<Expression, IExpressionTranslator>>
				{
						{typeof(ConstantExpression), GetValueTranslator},
						{typeof(BinaryExpression), e => GetNodeTypeBasedTranslator(e.NodeType)},
						{typeof(UnaryExpression), e => GetNodeTypeBasedTranslator(e.NodeType)},
						{typeof(MethodCallExpression), GetMethodCallTranslator},
						{typeof(MemberExpression), GetMemberTranslator},
				};

		private static IExpressionTranslator GetValueTranslator(Expression e)
		{
			var type = e.Type;
			if (type == typeof(bool))
				return new BooleanValueExpressionTranslator();
			if (type == typeof(string))
				return new StringValueExpressionTranslator();
			if (type.In(typeof(sbyte), typeof(byte), typeof(char), typeof(short), typeof(ushort), typeof(int),
			            typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)))
				return new NumberValueExpressionTranslator();
			var constant = (ConstantExpression) e;
			if (constant.Value == null)
				return new NullValueExpressionTranslator();
			throw new NotSupportedException($"Values of type '{type}' are not supported.");
		}
		private static IExpressionTranslator GetNodeTypeBasedTranslator(ExpressionType type)
		{
			switch (type)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					return new AddExpressionTranslator();
				case ExpressionType.Divide:
					return new DivideExpressionTranslator();
				case ExpressionType.Modulo:
					return new ModuloExpressionTranslator();
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					return new MultiplyExpressionTranslator();
				case ExpressionType.Power:
					return new ExponentExpressionTranslator();
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					return new SubtractExpressionTranslator();
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					return new ConversionExpressionTranslator();
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					return new NegateExpressionTranslator();
				case ExpressionType.Not:
					return new NotExpressionTranslator();
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					return new AndExpressionTranslator();
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					return new OrExpressionTranslator();
				case ExpressionType.Equal:
					return new IsEqualExpressionTranslator();
				case ExpressionType.LessThan:
					return new IsLessThanExpressionTranslator();
				case ExpressionType.LessThanOrEqual:
					return new IsLessThanEqualExpressionTranslator();
				case ExpressionType.GreaterThan:
					return new IsGreaterThanExpressionTranslator();
				case ExpressionType.GreaterThanOrEqual:
					return new IsGreaterThanEqualExpressionTranslator();
				case ExpressionType.NotEqual:
					return new IsNotEqualExpressionTranslator();
				// keeping these in here for reference
				case ExpressionType.ArrayLength:
				case ExpressionType.ArrayIndex:
				case ExpressionType.Call:
				case ExpressionType.Coalesce:
				case ExpressionType.Conditional:
				case ExpressionType.Constant:
				case ExpressionType.ExclusiveOr:
				case ExpressionType.Invoke:
				case ExpressionType.Lambda:
				case ExpressionType.LeftShift:
				case ExpressionType.ListInit:
				case ExpressionType.MemberAccess:
				case ExpressionType.MemberInit:
				case ExpressionType.UnaryPlus:
				case ExpressionType.New:
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
				case ExpressionType.Parameter:
				case ExpressionType.Quote:
				case ExpressionType.RightShift:
				case ExpressionType.TypeAs:
				case ExpressionType.TypeIs:
#if NET4 || NET45
				case ExpressionType.Assign:
				case ExpressionType.Block:
				case ExpressionType.DebugInfo:
				case ExpressionType.Decrement:
				case ExpressionType.Dynamic:
				case ExpressionType.Default:
				case ExpressionType.Extension:
				case ExpressionType.Goto:
				case ExpressionType.Increment:
				case ExpressionType.Index:
				case ExpressionType.Label:
				case ExpressionType.RuntimeVariables:
				case ExpressionType.Loop:
				case ExpressionType.Switch:
				case ExpressionType.Throw:
				case ExpressionType.Try:
				case ExpressionType.Unbox:
				case ExpressionType.AddAssign:
				case ExpressionType.AndAssign:
				case ExpressionType.DivideAssign:
				case ExpressionType.ExclusiveOrAssign:
				case ExpressionType.LeftShiftAssign:
				case ExpressionType.ModuloAssign:
				case ExpressionType.MultiplyAssign:
				case ExpressionType.OrAssign:
				case ExpressionType.PowerAssign:
				case ExpressionType.RightShiftAssign:
				case ExpressionType.SubtractAssign:
				case ExpressionType.AddAssignChecked:
				case ExpressionType.MultiplyAssignChecked:
				case ExpressionType.SubtractAssignChecked:
				case ExpressionType.PreIncrementAssign:
				case ExpressionType.PreDecrementAssign:
				case ExpressionType.PostIncrementAssign:
				case ExpressionType.PostDecrementAssign:
				case ExpressionType.TypeEqual:
				case ExpressionType.OnesComplement:
				case ExpressionType.IsTrue:
				case ExpressionType.IsFalse:
#endif
					break;
			}
			throw new NotSupportedException($"Expression type '{type}' is not supported.");
		}
		private static IExpressionTranslator GetMethodCallTranslator(Expression exp)
		{
			var method = (MethodCallExpression) exp;
			switch (method.Method.Name)
			{
				case "Length":
					return new LengthExpressionTranslator();
				case "HasProperty":
					return new HasPropertyExpressionTranslator();
				case "Name":
					return new NameExpressionTranslator();
				case "ArrayIndex":
					return new ArrayIndexExpressionTranslator();
				case "IndexOf":
					return new IndexOfExpressionTranslator();
			}
			throw new NotSupportedException($"The method '{method.Method.Name}' is not supported.");
		}
		private static IExpressionTranslator GetMemberTranslator(Expression exp)
		{
			var member = (MemberExpression) exp;
			if (member.Member is FieldInfo && member.Expression is ConstantExpression)
				return new FieldExpressionTranslator();
			throw new NotSupportedException("Properties and static fields are not supported.");
		}

		public static ExpressionTreeNode<T> TranslateNode<T>(Expression source)
		{
			var type = source.GetType();
			var expressionKey = Translators.Keys.FirstOrDefault(t => t.IsAssignableFrom(type));
			if (expressionKey != null)
			{
				var translator = Translators[expressionKey](source);
				if (translator != null)
					return translator.Translate<T>(source);
			}
			throw new NotSupportedException($"Expression type '{type}' is not supported.");
		}
		public static Expression<T, JsonValue> Translate<T>(Expression<Func<JsonPathValue, T>> source)
		{
			return new Expression<T, JsonValue>(TranslateNode<JsonValue>(source.Body));
		}
		public static Expression<T, JsonArray> Translate<T>(Expression<Func<JsonPathArray, T>> source)
		{
			return new Expression<T, JsonArray>(TranslateNode<JsonArray>(source.Body));
		}
	}
}