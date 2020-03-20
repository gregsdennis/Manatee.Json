using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
		public JsonPathExpression? LastExpression { get; set; }

		public bool TryCreateExpressionTreeNode<TIn>([NotNullWhen(true)] out ExpressionTreeNode<TIn> root, [NotNullWhen(false)] out string errorMessage)
		{
			if (Output.Count != 1)
			{
				root = null!;
				errorMessage = "No expression to parse.";
				return false;
			}

			if (Operators.Count > 0)
			{
				root = null!;
				errorMessage = "Unbalanced expression.";
				return false;
			}

			var first = Output.Pop();
			if (first == null)
			{
				root = null!;
				errorMessage = "No expression found.";
				return false;
			}
			var node = _Visit<TIn>(first);

			root = _MakeHasPropertyIfNameExpression(node);
			errorMessage = null!;
			return true;
		}
		private static ExpressionTreeNode<TIn> _Visit<TIn>(JsonPathExpression expr)
		{
			if (expr is PathValueExpression<TIn> path) return path.Path;
			if (expr is ValueExpression value) return new ValueExpression<TIn>(value.Value);

			if (!(expr is OperatorExpression op))
				throw new NotSupportedException($"Expressions of type {expr.GetType()} are not supported.");

			var left = _Visit<TIn>(op.Children[0]);

			if (op.Operator == JsonPathOperator.Negate) return _VisitNegate(left);
			if (op.Operator == JsonPathOperator.Not) return new NotExpression<TIn>(_MakeHasPropertyIfNameExpression(left));

			var right = _Visit<TIn>(op.Children.Count > 1 
				                        ? op.Children[1]
				                        : throw new InvalidOperationException($"Operator type {op.Operator} requires two operands."));

			_CheckAndReplaceIfHasPropertyNeeded(op.Operator, ref left, ref right);
			return op.Operator switch
				{
					JsonPathOperator.Add => (ExpressionTreeNode<TIn>) new AddExpression<TIn>(left, right),
					JsonPathOperator.And => new AndExpression<TIn>(left, right),
					JsonPathOperator.Divide => new DivideExpression<TIn>(left, right),
					JsonPathOperator.Exponent => new ExponentExpression<TIn>(left, right),
					JsonPathOperator.Equal => new IsEqualExpression<TIn>(left, right),
					JsonPathOperator.GreaterThan => new IsGreaterThanExpression<TIn>(left, right),
					JsonPathOperator.GreaterThanOrEqual => new IsGreaterThanEqualExpression<TIn>(left, right),
					JsonPathOperator.LessThan => new IsLessThanExpression<TIn>(left, right),
					JsonPathOperator.LessThanOrEqual => new IsLessThanEqualExpression<TIn>(left, right),
					JsonPathOperator.NotEqual => new IsNotEqualExpression<TIn>(left, right),
					JsonPathOperator.Modulo => new ModuloExpression<TIn>(left, right),
					JsonPathOperator.Multiply => new MultiplyExpression<TIn>(left, right),
					JsonPathOperator.Subtract => new SubtractExpression<TIn>(left, right),
					JsonPathOperator.Or => new OrExpression<TIn>(left, right),
					_ => throw new NotSupportedException($"Expressions of type {expr.GetType()} are not supported")
				};
		}

		/// <summary>
		/// Constant terms are negated immediately.
		/// </summary>
		private static ExpressionTreeNode<TIn> _VisitNegate<TIn>(ExpressionTreeNode<TIn> left)
		{
			if (left is ValueExpression<TIn> value && value.Value != null)
			{
				// Fold.
				var negatedValue = _Negate(value.Value);
				if (negatedValue != null)
				{
					return new ValueExpression<TIn>(negatedValue);
				}
			}

			// Always apply Negate to anything other than a negatable value
			return new NegateExpression<TIn>(left);
		}

		private static object? _Negate(object value)
		{
			return value switch
				{
					byte @byte => (object) -@byte,
					sbyte @sbyte => -@sbyte,
					short @short => -@short,
					ushort @ushort => -@ushort,
					int @int => -@int,
					uint @uint => -@uint,
					long @long => -@long,
					ulong _ => null, // can't negate a ulong
					float @float => -@float,
					double @double => -@double,
					decimal @decimal => -@decimal,
					_ => null
				};
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
		private static void _CheckAndReplaceIfHasPropertyNeeded<TIn>(JsonPathOperator op, ref ExpressionTreeNode<TIn> left, ref ExpressionTreeNode<TIn> right)
		{
			var namedLeft = left as NameExpression<TIn>;
			var namedRight = right as NameExpression<TIn>;
			if (namedLeft == null && namedRight == null) return;

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
			}
		}

		private static ExpressionTreeNode<TIn> _MakeHasPropertyIfNameExpression<TIn>(ExpressionTreeNode<TIn> node)
		{
			if (node is NameExpression<TIn> named)
				return new HasPropertyExpression<TIn>(named.Path, named.IsLocal, named.Name);

			return node;
		}

		private static bool _IsBooleanResult<TIn>(ExpressionTreeNode<TIn> node)
		{
			if (node == null) return false;

			// Boolean values are a boolean result.
			if (node is ValueExpression<TIn> value && value.Value is bool) return true;

			// NotExpressions are always a boolean result.
			if (node is NotExpression<TIn>) return true;

			if (node is ExpressionTreeBranch<TIn> branch)
			{
				// Any subexpression returning a boolean 
				// value is obviously a boolean result.
				return branch is AndExpression<TIn> ||
				       branch is OrExpression<TIn> ||
				       branch is IsEqualExpression<TIn> ||
				       branch is IsNotEqualExpression<TIn> ||
				       branch is IsGreaterThanEqualExpression<TIn> ||
				       branch is IsGreaterThanExpression<TIn> ||
				       branch is IsLessThanEqualExpression<TIn> ||
				       branch is IsLessThanExpression<TIn>;
			}

			// All other expressions do not have boolean results.
			return false;
		}
	}
}
