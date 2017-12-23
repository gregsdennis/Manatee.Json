using System;
using System.Collections.Generic;
using System.Text;

namespace Manatee.Json.Path.Expressions.Parsing
{
	/// <summary>
	/// Provides context for the Shunting-yard Algorithm implementation.
	/// </summary>
    internal class JsonPathExpressionContext
    {
		/// <summary>
		/// Output expression stack.
		/// </summary>
		public Stack<JsonPathExpression> Output { get; } = new Stack<JsonPathExpression>();

		/// <summary>
		/// Operator expression stack.
		/// </summary>
		public Stack<OperatorExpression> Operators { get; } = new Stack<OperatorExpression>();

		/// <summary>
		/// The last encountered expression.
		/// </summary>
		/// <remarks>
		/// Used to differentiate unary negation from subtraction.
		/// </remarks>
		public JsonPathExpression LastExpression { get; set; }

		public string CreateExpressionTreeNode<TIn>(out ExpressionTreeNode<TIn> root)
		{
			root = null;
			if (Output.Count != 1)
				return "No expression to parse.";
			if (Operators.Count > 0)
				return "Unbalanced expression.";

			root = _MakeHasPropertyIfNameExpression(_Visit<TIn>(Output.Pop(), null));
			return null;
		}
		private ExpressionTreeNode<TIn> _Visit<TIn>(JsonPathExpression expr, OperatorExpression parentExpr)
		{
			if (expr is null)
				return null;

			if (expr is PathValueExpression<TIn> path)
			{
				return path.Path;
			}
			if (expr is ValueExpression value)
			{
				return new ValueExpression<TIn> { Value = value.Value };
			}
			else if (expr is OperatorExpression op)
			{
				var left = _Visit<TIn>(op.Children[0], op);
				var right = _Visit<TIn>(op.Children.Count > 1 ? op.Children[1] : null, op);

				_CheckAndReplaceIfHasPropertyNeeded(op.Operator, ref left, ref right);
				switch (op.Operator)
				{
					case JsonPathOperator.Add:
						return new AddExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.And:
						return new AndExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Divide:
						return new DivideExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Exponent:
						return new ExponentExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Equal:
						return new IsEqualExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.GreaterThan:
						return new IsGreaterThanExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.GreaterThanOrEqual:
						return new IsGreaterThanEqualExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.LessThan:
						return new IsLessThanExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.LessThanOrEqual:
						return new IsLessThanEqualExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.NotEqual:
						return new IsNotEqualExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Modulo:
						return new ModuloExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Multiply:
						return new MultiplyExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Subtract:
						return new SubtractExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Or:
						return new OrExpression<TIn> { Left = left, Right = right, };
					case JsonPathOperator.Negate:
						return _VisitNegate(left);
					case JsonPathOperator.Not:
						return new NotExpression<TIn> { Root = _MakeHasPropertyIfNameExpression(left) };
					default:
						break;
				}
			}

			throw new NotSupportedException($"Expressions of type {expr.GetType()} are not supported");
		}
		/// <summary>
		/// Constant terms are negated immediately.
		/// </summary>
		private ExpressionTreeNode<TIn> _VisitNegate<TIn>(ExpressionTreeNode<TIn> left)
		{
			// Always apply Negate to path values
			if (left is PathValueExpression<TIn>)
				return new NegateExpression<TIn> { Root = left };

			if (left is ValueExpression<TIn> value && value.Value != null)
			{
				// Fold.
				var negatedValue = _Negate(value.Value);
				if (negatedValue != null)
				{
					return new ValueExpression<TIn> { Value = negatedValue };
				}
			}

			// Always apply Negate to anything other than a negatable value
			return new NegateExpression<TIn> { Root = left };
		}
		private object _Negate(object value)
		{
			if (value is byte @byte) return -@byte;
			if (value is sbyte @sbyte) return -@sbyte;
			if (value is short @short) return -@short;
			if (value is ushort @ushort) return -@ushort;
			if (value is int @int) return -@int;
			if (value is uint @uint) return -@uint;
			if (value is long @long) return -@long;
			if (value is ulong) return null; // can't negate a ulong
			if (value is float @float) return -@float;
			if (value is double @double) return -@double;
			if (value is decimal @decimal) return -@decimal;
			return null;
		}
		/// <summary>
		/// Converts <paramref name="left"/> and <paramref name="right"/> to <see cref="HasPropertyExpression{T}"/>
		/// nodes if either are <see cref="NameExpression{T}"/> nodes being used in boolean contexts. Otherwise,
		/// it leaves the nodes as-is.
		/// </summary>
		/// <typeparam name="TIn">Type of the resulting JSON expression.</typeparam>
		/// <param name="op">Operator applied to <paramref name="left"/> and <paramref name="right"/>.</param>
		/// <param name="left">Left hand side of <paramref name="op"/>.</param>
		/// <param name="right">Right hand side of <paramref name="op"/>.</param>
		private void _CheckAndReplaceIfHasPropertyNeeded<TIn>(JsonPathOperator op, ref ExpressionTreeNode<TIn> left, ref ExpressionTreeNode<TIn> right)
		{
			if (left is NameExpression<TIn> || right is NameExpression<TIn>)
			{
				var namedLeft = left as NameExpression<TIn>;
				var namedRight = right as NameExpression<TIn>;
				switch (op)
				{
					case JsonPathOperator.And:
					case JsonPathOperator.Or:
					case JsonPathOperator.Not:
						// Explicit boolean context, convert one or both sides
						// as necessary.
						left = _MakeHasPropertyIfNameExpression(left);
						right = _MakeHasPropertyIfNameExpression(right);
						break;

					case JsonPathOperator.Equal:
					case JsonPathOperator.NotEqual:
						// Convert only if one side is a named expression and the other
						// is a boolean expression tree, but not if both are named 
						// expressions.
						// 
						// Why? If we've got == or != with two named expressions
						// we're comparing their values and should not convert both
						// to HasPropertyExpression's.
						if (namedLeft == null ^ namedRight == null)
						{
							var isBoolean = namedLeft == null
										  ? _IsBooleanResult(left) 
										  : _IsBooleanResult(right);
							if (isBoolean)
							{
								// We've evaluated that the context of our Equal/NotEqual
								// operands is boolean. Convert whichever side is a 
								// named expression to a HasProperty expression.
								left = _MakeHasPropertyIfNameExpression(left);
								right = _MakeHasPropertyIfNameExpression(right);
							}
						}
						break;

					default:
						// All other operators (relational too) imply a value comparison rather
						// than a comparison against HasProperty.
						break;
				}
			}
		}
		private static ExpressionTreeNode<TIn> _MakeHasPropertyIfNameExpression<TIn>(ExpressionTreeNode<TIn> node)
		{
			if (node is NameExpression<TIn> named)
			{
				return new HasPropertyExpression<TIn>
				{
					Path = named.Path,
					IsLocal = named.IsLocal,
					Name = named.Name
				};
			}

			return node;
		}
		private bool _IsBooleanResult<TIn>(ExpressionTreeNode<TIn> node)
		{
			if (node == null)
				return false;

			// Boolean values are a boolean result.
			if (node is ValueExpression<TIn> value && value.Value != null && (value.Value is bool || value.Value is bool?))
				return true;

			// NotExpressions are always a boolean result.
			if (node is NotExpression<TIn>)
				return true;

			if (node is ExpressionTreeBranch<TIn> branch)
			{
				// Any subexpression returning a boolean 
				// value is obviously a boolean result.
				return branch is AndExpression<TIn>
					|| branch is OrExpression<TIn>
					|| branch is IsEqualExpression<TIn>
					|| branch is IsNotEqualExpression<TIn>
					|| branch is IsGreaterThanEqualExpression<TIn>
					|| branch is IsGreaterThanExpression<TIn>
					|| branch is IsLessThanEqualExpression<TIn>
					|| branch is IsLessThanExpression<TIn>;
			}

			// All other expressions do not have boolean results.
			return false;
		}
	}
}
