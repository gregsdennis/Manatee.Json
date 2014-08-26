/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		Expression.cs
	Namespace:		Manatee.Json.Path.Expressions
	Class Name:		Expression
	Purpose:		Represents an expression in JsonPaths.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Path.Expressions.Translation;
using Manatee.Json.Path.Operators;
using Manatee.StateMachine;
using Manatee.StateMachine.Exceptions;

namespace Manatee.Json.Path.Expressions
{
	internal class Expression<T, TIn>
	{
		private enum State
		{
			Start,
			Value,
			NumberOrPath,
			Operator,
			Comparison,
			ValueOrOperator,
			End
		}

		private static readonly StateMachine<State, JsonPathExpressionInput> StateMachine = new StateMachine<State, JsonPathExpressionInput>();

		private List<ExpressionTreeNode<TIn>> _nodeList;
		private readonly InputStream<JsonPathExpressionInput> _stream = new InputStream<JsonPathExpressionInput>();
		private bool _done;
		private string _source;
		private int _index;
		private JsonPathExpressionInput? _previousInput;

		internal ExpressionTreeNode<TIn> Root { get; set; }

		static Expression()
		{
			StateMachine[State.Start, JsonPathExpressionInput.OpenParenth] = GotStart;
			StateMachine[State.Value, JsonPathExpressionInput.OpenParenth] = GotGroup;
			StateMachine[State.Value, JsonPathExpressionInput.Number] = GotNumber;
			StateMachine[State.Value, JsonPathExpressionInput.Current] = GotPath;
			StateMachine[State.Value, JsonPathExpressionInput.Quote] = GotString;
			StateMachine[State.NumberOrPath, JsonPathExpressionInput.OpenParenth] = GotGroup;
			StateMachine[State.NumberOrPath, JsonPathExpressionInput.Number] = GotNumber;
			StateMachine[State.NumberOrPath, JsonPathExpressionInput.Current] = GotPath;
			StateMachine[State.Operator, JsonPathExpressionInput.Plus] = GotPlus;
			StateMachine[State.Operator, JsonPathExpressionInput.Minus] = GotMinus;
			StateMachine[State.Operator, JsonPathExpressionInput.Star] = GotMultiply;
			StateMachine[State.Operator, JsonPathExpressionInput.Slash] = GotDivide;
			StateMachine[State.Operator, JsonPathExpressionInput.Caret] = GotExponent;
			StateMachine[State.Operator, JsonPathExpressionInput.LessThan] = GotLessThan;
			StateMachine[State.Operator, JsonPathExpressionInput.Equal] = GotEqual;
			StateMachine[State.Operator, JsonPathExpressionInput.GreaterThan] = GotGreaterThan;
			StateMachine[State.Operator, JsonPathExpressionInput.Bang] = GotNot;
			StateMachine[State.Operator, JsonPathExpressionInput.CloseParenth] = CompleteExpression;
			StateMachine[State.Comparison, JsonPathExpressionInput.Equal] = GotEqual;
			StateMachine[State.ValueOrOperator, JsonPathExpressionInput.OpenParenth] = GotGroup;
			StateMachine[State.ValueOrOperator, JsonPathExpressionInput.Number] = GotNumber;
			StateMachine[State.ValueOrOperator, JsonPathExpressionInput.Current] = GotPath;
			StateMachine[State.ValueOrOperator, JsonPathExpressionInput.Equal] = FinalizeComparison;
			StateMachine.UpdateFunction = GetNextInput;
		}

		public T Evaluate(TIn json)
		{
			var result = Root.Evaluate(json);
			if (typeof (T) == typeof (bool) && result == null)
				return (T) (object) false;
			if (typeof (T) == typeof (bool) && result != null && !(result is bool))
				return (T) (object) true;
			return (T)Convert.ChangeType(result, typeof(T));
		}
		public override string ToString()
		{
			return Root.ToString();
		}

		public static implicit operator Expression<T, TIn>(System.Linq.Expressions.Expression<Func<TIn, T>> source)
		{
			return ExpressionTranslator.Translate(source);
		}

		internal int Parse(string source, int i)
		{
			_source = source;
			Parse(i);
			return _index;
		}

		private void Parse(int i)
		{
			_stream.Clear();
			_index = i;
			_done = false;
			try
			{
				StateMachine.Run(this, State.Start, _stream);
				if (!_done)
					throw new JsonSyntaxException(_index);
			}
			catch (InputNotValidForStateException<State, JsonPathExpressionInput>)
			{
				throw new JsonSyntaxException(_index);
			}
			catch (StateNotValidException<State>)
			{
				throw new JsonSyntaxException(_index);
			}
			catch (ActionNotDefinedForStateAndInputException<State, JsonPathExpressionInput>)
			{
				throw new JsonSyntaxException(_index);
			}
		}
		private static void GetNextInput(object owner)
		{
			var obj = owner as Expression<T, TIn>;
			if (obj == null) return;
			if (obj._done || (obj._index == obj._source.Length)) return;
			try
			{
				var next = CharacterConverter.ExpressionItem(obj._source[obj._index++]);
				obj._stream.Add(next);
			}
			catch (KeyNotFoundException)
			{
				throw new JsonSyntaxException(obj._index);
			}
		}
		private static State GotStart(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._nodeList = new List<ExpressionTreeNode<TIn>>();
			return State.Value;
		}
		private static State GotGroup(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			CheckComparison(exp);
			var group = new Expression<T, TIn>();
			exp._index = group.Parse(exp._source, exp._index - 1);
			exp._nodeList.Add(group.Root);
			return State.Value;
		}
		private static State CompleteExpression(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp.Root = BuildTree(exp._nodeList);
			exp._nodeList = null;
			exp._done = true;
			return State.End;
		}
		private static State GotNumber(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			CheckComparison(exp);
			var numString = new string(exp._source.Skip(exp._index - 1).TakeWhile(char.IsNumber).ToArray());
			double num;
			if (!double.TryParse(numString, out num))
				throw new JsonSyntaxException(exp._index);
			exp._index += numString.Length - 1;
			exp._nodeList.Add(new ValueExpression<TIn> {Value = num});
			return State.Operator;
		}
		private static State GotPath(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			CheckComparison(exp);
			var path = new JsonPath();
			exp._index = path.Parse(exp._source, exp._index - 1) - 1;
			var name = path.Last() as NameOperator;
			if (name != null && name.Name == "length")
			{
				path.Remove(name);
				exp._nodeList.Add(new LengthExpression<TIn> {Path = path});
			}
			else
				exp._nodeList.Add(new PathExpression<TIn> {Path = path});
			return State.Operator;
		}
		private static State GotString(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			CheckComparison(exp);
			var value = new string(exp._source.Skip(exp._index).TakeWhile(c => c != '"').ToArray());
			exp._index += value.Length+1;
			exp._nodeList.Add(new ValueExpression<TIn> {Value = value});
			return State.Operator;
		}
		private static State GotPlus(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._nodeList.Add(new AddExpression<TIn>());
			return State.Value;
		}
		private static State GotMinus(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._nodeList.Add(new SubtractExpression<TIn>());
			return State.Value;
		}
		private static State GotMultiply(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._nodeList.Add(new MultiplyExpression<TIn>());
			return State.Value;
		}
		private static State GotDivide(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._nodeList.Add(new DivideExpression<TIn>());
			return State.Value;
		}
		private static State GotExponent(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._nodeList.Add(new ExponentExpression<TIn>());
			return State.Value;
		}
		private static State GotLessThan(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._previousInput = JsonPathExpressionInput.LessThan;
			return State.ValueOrOperator;
		}
		private static State GotEqual(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			if (exp._previousInput.HasValue)
			{
				exp._nodeList.Add(new IsEqualExpression<TIn>());
				exp._previousInput = null;
				return State.Value;
			}
			exp._previousInput = JsonPathExpressionInput.Equal;
			return State.Comparison;
		}
		private static State GotGreaterThan(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._previousInput = JsonPathExpressionInput.GreaterThan;
			return State.ValueOrOperator;
		}
		private static State GotNot(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			exp._previousInput = JsonPathExpressionInput.Bang;
			return State.ValueOrOperator;
		}
		private static State FinalizeComparison(object owner, JsonPathExpressionInput input)
		{
			var exp = owner as Expression<T, TIn>;
			switch (input)
			{
				case JsonPathExpressionInput.LessThan:
					exp._nodeList.Add(new IsLessThanExpression<TIn>());
					break;
				case JsonPathExpressionInput.Equal:
					switch (exp._previousInput)
					{
						case JsonPathExpressionInput.LessThan:
							exp._nodeList.Add(new IsLessThanEqualExpression<TIn>());
							break;
						case JsonPathExpressionInput.GreaterThan:
							exp._nodeList.Add(new IsGreaterThanEqualExpression<TIn>());
							break;
						case JsonPathExpressionInput.Bang:
							exp._nodeList.Add(new IsNotEqualExpression<TIn>());
							break;
						default:
							throw new JsonSyntaxException(exp._index);
					}
					break;
				case JsonPathExpressionInput.GreaterThan:
					exp._nodeList.Add(new IsGreaterThanExpression<TIn>());
					break;
				case JsonPathExpressionInput.Bang:
					throw new NotImplementedException();
					break;
				default:
					throw new JsonSyntaxException(exp._index);
			}
			exp._previousInput = null;
			return State.NumberOrPath;
		}
		private static void CheckComparison(Expression<T, TIn> exp)
		{
			if (exp._previousInput.HasValue)
			{
				var previous = exp._previousInput.Value;
				exp._previousInput = null;
				FinalizeComparison(exp, previous);
			}
		}
		private static ExpressionTreeNode<TIn> BuildTree(IList<ExpressionTreeNode<TIn>> nodes)
		{
			if (!nodes.Any()) return null;
			var minPriority = nodes.Min(n => n.Priority);
			var root = nodes.Last(n => n.Priority == minPriority);
			var branch = root as ExpressionTreeBranch<TIn>;
			if (branch != null)
			{
				var split = nodes.IndexOf(root);
				var left = nodes.Take(split).ToList();
				var right = nodes.Skip(split + 1).ToList();
				branch.Left = BuildTree(left);
				branch.Right = BuildTree(right);
			}
			return root;
		}
	}
}
