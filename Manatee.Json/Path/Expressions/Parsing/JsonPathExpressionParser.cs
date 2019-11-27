using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal static class JsonPathExpressionParser
	{
		private static readonly List<IJsonPathExpressionParser> _parsers;

		static JsonPathExpressionParser()
		{
			_parsers = typeof(JsonPathExpressionParser).GetTypeInfo().Assembly.DefinedTypes
			                                           .Where(t => typeof(IJsonPathExpressionParser).GetTypeInfo().IsAssignableFrom(t) && t.IsClass)
			                                           .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                           .Cast<IJsonPathExpressionParser>()
			                                           .ToList();
		}

		public static bool TryParse<T, TIn>(string source, ref int index, [NotNullWhen(true)] out Expression<T, TIn>? expr, [NotNullWhen(false)] out string? errorMessage)
		{
			if (!_TryParse<TIn>(source, ref index, out var root, out errorMessage))
			{
				expr = null!;
				return false;
			}

			expr = new Expression<T, TIn>(root);
			return true;
		}

		private static bool _TryParse<TIn>(string source, ref int index, [NotNullWhen(true)] out ExpressionTreeNode<TIn>? root, [NotNullWhen(false)] out string? errorMessage)
		{
			var context = new JsonPathExpressionContext();
			while (index < source.Length)
			{
				errorMessage = source.SkipWhiteSpace(ref index, source.Length, out _);
				if (errorMessage != null)
				{
					root = null!;
					return false;
				}

				IJsonPathExpressionParser? foundParser = null;
				foreach (var parser in _parsers)
				{
					if (parser.Handles(source, index))
					{
						foundParser = parser;
						break;
					}
				}

				if (foundParser == null)
				{
					root = null!;
					errorMessage = "Unrecognized JSON Path Expression element.";
					return false;
				}

				if (!foundParser.TryParse<TIn>(source, ref index, out var expression, out errorMessage))
				{
					root = null!;
					return false;
				}
				if (expression == null) break;

				//
				// Implements the Shunting-yard Algorithm
				//
				if (expression is ValueExpression value)
				{
					// Values go immediately onto the output stack
					context.Output.Push(value);
				}
				else if (expression is OperatorExpression op)
				{
					if (op.Operator == JsonPathOperator.GroupStart)
					{
						// Push open parenthesis onto the operator stack
						context.Operators.Push(new OperatorExpression { Operator = JsonPathOperator.GroupStart });
					}
					else if (op.Operator == JsonPathOperator.GroupEnd)
					{
						// Resolve all operators from the closing parenthesis
						// back to the matching open parenthesis.

						// While:
						//   1. We have operators
						//   2. and, they are not the open parentheses...
						while (context.Operators.Count > 0
							&& context.Operators.Peek().Operator != JsonPathOperator.GroupStart)
						{
							// Get the operator...
							var expr = context.Operators.Pop();

							// ...pop its children from the stack...
							_GetRequiredChildrenFromOutput(context, expr);

							// ...and push the completed operator sub-tree onto the output stack
							context.Output.Push(expr);
						}

						if (context.Operators.Count == 0
						    || context.Operators.Pop().Operator != JsonPathOperator.GroupStart)
						{
							errorMessage = "Unbalanced parentheses.";
							root = null!;
							return false;
						}
					}
					else
					{
						if (op.Operator == JsonPathOperator.Subtract)
						{
							// Check if we need to switch this to a negation operator
							if (context.LastExpression is OperatorExpression lastOp
								&& lastOp.Operator != JsonPathOperator.GroupEnd)
							{
								// ...if the prior expression is an operator,
								// but not the end of a grouping expression
								// then this subtraction is actually a negation
								// of the next term.
								op.Operator = JsonPathOperator.Negate;
							}
						}

						// For all other operators, resolve any operators
						// of greater (or equal right-assoc) precedence 
						// before pushing ourselves onto the operator stack
						while (context.Operators.Count > 0)
						{
							var top = context.Operators.Peek();

							// While:
							//   1. There is an op with greater precedence
							//   2. or, the operator has equal and is left-assoc
							//   3. and, it is not group start...
							var precedence = _Compare(op, top);
							if ((precedence < 0 || precedence == 0 && !_IsRightAssociative(top.Operator)) && top.Operator != JsonPathOperator.GroupStart)
							{
								// Get the operator...
								var expr = context.Operators.Pop();

								// ...pop its children from the stack...
								_GetRequiredChildrenFromOutput(context, expr);

								// ...and push the completed operator sub-tree onto the output stack
								context.Output.Push(expr);
							}
							else break;
						}

						context.Operators.Push(op);
					}
				}

				context.LastExpression = expression;
			}

			// Convert the expression into an ExpressionTreeNode
			return context.TryCreateExpressionTreeNode(out root, out errorMessage);
		}

		private static void _GetRequiredChildrenFromOutput(JsonPathExpressionContext context, OperatorExpression expr)
		{
			if (expr.IsBinary)
			{
				var second = context.Output.Pop();
				var first = context.Output.Pop();
				expr.Children.Add(first);
				expr.Children.Add(second);
			}
			else // unary
				expr.Children.Add(context.Output.Pop());
		}

		private static int _Compare(OperatorExpression a, OperatorExpression b)
		{
			return _Precedence(a.Operator).CompareTo(_Precedence(b.Operator));
		}

		private static int _Precedence(JsonPathOperator op)
		{
			// Based on JavaScript's operator precedence
			switch (op)
			{
				case JsonPathOperator.GroupStart:
				case JsonPathOperator.GroupEnd:
					return 20;
				case JsonPathOperator.Negate:
				case JsonPathOperator.Not:
					return 16;
				case JsonPathOperator.Exponent:
					return 15;
				case JsonPathOperator.Multiply:
				case JsonPathOperator.Divide:
				case JsonPathOperator.Modulo:
					return 14;
				case JsonPathOperator.Add:
				case JsonPathOperator.Subtract:
					return 13;
				case JsonPathOperator.GreaterThan:
				case JsonPathOperator.GreaterThanOrEqual:
				case JsonPathOperator.LessThan:
				case JsonPathOperator.LessThanOrEqual:
					return 11;
				case JsonPathOperator.Equal:
				case JsonPathOperator.NotEqual:
					return 10;
				case JsonPathOperator.And:
					return 6;
				case JsonPathOperator.Or:
					return 5;
				default:
					return 0;
			}
		}

		private static bool _IsRightAssociative(JsonPathOperator op)
		{
			// Based on JavaScript's operator associativity
			switch (op)
			{
				case JsonPathOperator.Negate:
				case JsonPathOperator.Not:
				case JsonPathOperator.Exponent:
					return true;
				default:
					return false;
			}
		}
	}
}
