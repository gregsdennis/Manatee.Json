/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonPath.cs
	Namespace:		Manatee.Json.Path
	Class Name:		JsonPath
	Purpose:		Provides primary functionality for JSON Path objects.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Manatee.Json.Internal;
using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Expressions;
using Manatee.Json.Path.Operators;
using Manatee.Json.Path.SearchParameters;
using Manatee.StateMachine;
using Manatee.StateMachine.Exceptions;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Provides primary functionality for JSON Path objects.
	/// </summary>
	public class JsonPath
	{
		private enum State
		{
			Start,
			ObjectOrArray,
			ArrayContent,
			IndexOrSlice,
			Colon,
			SliceValue,
			Comma,
			IndexValue,
			CloseArray,
			ObjectContent,
			Search,
			End
		}

		private static readonly StateMachine<State, JsonPathInput> _stateMachine = new StateMachine<State, JsonPathInput>();

		private bool _gotObject;
		private string _source;
		private int _index;
		private List<int> _indices;
		private Dictionary<int, int> _slice;
		private int _colonCount;
		private bool _done;
		private readonly InputStream<JsonPathInput> _stream = new InputStream<JsonPathInput>();
		private bool _isSearch;
		private bool _allowLocalRoot;

		internal List<IJsonPathOperator> Operators { get; } = new List<IJsonPathOperator>();

		static JsonPath()
		{
			_stateMachine[State.Start, JsonPathInput.Dollar] = GotRoot;
			_stateMachine[State.Start, JsonPathInput.Current] = GotCurrent;
			_stateMachine[State.ObjectOrArray, JsonPathInput.OpenBracket] = GotArray;
			_stateMachine[State.ObjectOrArray, JsonPathInput.Period] = GotObject;
			_stateMachine[State.ObjectOrArray, JsonPathInput.OpenParenth] = GotFunction;
			_stateMachine[State.ObjectOrArray, JsonPathInput.Number] = CompletePath;
			_stateMachine[State.ObjectOrArray, JsonPathInput.End] = CompletePath;
			_stateMachine[State.ArrayContent, JsonPathInput.Star] = GotArrayWildCard;
			_stateMachine[State.ArrayContent, JsonPathInput.OpenParenth] = ParseIndexExpression;
			_stateMachine[State.ArrayContent, JsonPathInput.Question] = ParseFilterExpression;
			_stateMachine[State.ArrayContent, JsonPathInput.Number] = GotIndexOrSlice;
			_stateMachine[State.ArrayContent, JsonPathInput.Colon] = GotSlice;
			_stateMachine[State.IndexOrSlice, JsonPathInput.Colon] = GotSlice;
			_stateMachine[State.IndexOrSlice, JsonPathInput.Comma] = GotIndex;
			_stateMachine[State.IndexOrSlice, JsonPathInput.CloseBracket] = FinalizeIndex;
			_stateMachine[State.Colon, JsonPathInput.CloseBracket] = FinalizeSlice;
			_stateMachine[State.Colon, JsonPathInput.Number] = GotSliceValue;
			_stateMachine[State.Colon, JsonPathInput.Colon] = GotSlice;
			_stateMachine[State.SliceValue, JsonPathInput.CloseBracket] = FinalizeSlice;
			_stateMachine[State.SliceValue, JsonPathInput.Colon] = GotSlice;
			_stateMachine[State.Comma, JsonPathInput.Number] = GotIndexValue;
			_stateMachine[State.IndexValue, JsonPathInput.CloseBracket] = FinalizeIndex;
			_stateMachine[State.IndexValue, JsonPathInput.Comma] = GotIndex;
			_stateMachine[State.CloseArray, JsonPathInput.CloseBracket] = FinalizeArray;
			_stateMachine[State.ObjectContent, JsonPathInput.Period] = GotSearch;
			_stateMachine[State.ObjectContent, JsonPathInput.Star] = GotObjectWildCard;
			_stateMachine[State.ObjectContent, JsonPathInput.Letter] = GotName;
			_stateMachine[State.Search, JsonPathInput.OpenBracket] = GotSearchArray;
			_stateMachine[State.Search, JsonPathInput.Star] = GotSearchWildCard;
			_stateMachine[State.Search, JsonPathInput.Letter] = GotSearchName;
			_stateMachine.UpdateFunction = GetNextInput;
		}
		/// <summary>
		/// Finalizes memory management responsibilities.
		/// </summary>
		~JsonPath()
		{
			_stateMachine.UnregisterOwner(this);
		}

		/// <summary>
		/// Parses a <see cref="string"/> containing a JSON path.
		/// </summary>
		/// <param name="source">the <see cref="string"/> to parse.</param>
		/// <returns>The JSON path represented by the <see cref="string"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="source"/> is empty or whitespace.</exception>
		/// <exception cref="JsonPathSyntaxException">Thrown if <paramref name="source"/> contains invalid JSON path syntax.</exception>
		public static JsonPath Parse(string source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source.IsNullOrWhiteSpace())
				throw new ArgumentException("Source string contains no data.");
			var path = new JsonPath {_source = Regex.Replace(source, @"\s+", string.Empty)};
			path.Parse(0);
			return path;
		}
		/// <summary>
		/// Evaluates a JSON value using the path.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> to evaulate.</param>
		/// <returns></returns>
		public JsonArray Evaluate(JsonValue json)
		{
			var current = new JsonArray {json};
			var found = Operators.Aggregate(current, (c, o) => o.Evaluate(c, json));
			return found;
		}
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return $"${GetRawString()}";
		}

		internal string GetRawString()
		{
			return Operators.Select(o => o.ToString()).Join(string.Empty);
		}
		internal int Parse(string source, int i)
		{
			_source = source;
			_allowLocalRoot = true;
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
				_stateMachine.Run(this, State.Start, _stream);
				if (!_done)
					throw new JsonPathSyntaxException(this, "Found incomplete JSON path.");
			}
			catch (InputNotValidForStateException<State, JsonPathInput> e)
			{
				if (e.Input == JsonPathInput.Star && _allowLocalRoot)
				{
					_done = true;
					return;
				}
				switch (e.State)
				{
					case State.Start:
						throw new JsonPathSyntaxException(this, "Paths must start with a '$', or optionally a '@' if inside an expression.");
					case State.ObjectOrArray:
						throw new JsonPathSyntaxException(this, "Expected '.', '..', or '['.");
					case State.ArrayContent:
						throw new JsonPathSyntaxException(this, "Expected '?', '(', ':', or an integer.");
					case State.IndexOrSlice:
						throw new JsonPathSyntaxException(this, "Expected ',', ':', or ']'.");
					case State.Colon:
						if (_colonCount == 2)
							throw new JsonPathSyntaxException(this, "Expected ']' or an integer.");
						throw new JsonPathSyntaxException(this, "Expected ']', ':', or an integer.");
					case State.SliceValue:
						throw new JsonPathSyntaxException(this, "Expected ':' or ']'.");
					case State.Comma:
						throw new JsonPathSyntaxException(this, "Expected an integer.");
					case State.IndexValue:
						throw new JsonPathSyntaxException(this, "Expected ',' or ']'.");
					case State.CloseArray:
						throw new JsonPathSyntaxException(this, "Expected ']'.");
					case State.ObjectContent:
						throw new JsonPathSyntaxException(this, "Expected a key name.  Note: quoted keys and keys which start with numbers are not currently supported.");
					case State.Search:
						throw new JsonPathSyntaxException(this, "Expected a key name or '['.  Note: quoted keys and keys which start with numbers are not currently supported.");
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (StateNotValidException<State>)
			{
				throw new JsonPathSyntaxException(this, "An unrecoverable error occurred while parsing a JSON path. Please report to littlecrabsolutions@yahoo.com.");
			}
			catch (ActionNotDefinedForStateAndInputException<State, JsonPathInput>)
			{
				throw new JsonPathSyntaxException(this, "An unrecoverable error occurred while parsing a JSON path. Please report to littlecrabsolutions@yahoo.com.");
			}
		}
		private static void GetNextInput(object owner)
		{
			var obj = owner as JsonPath;
			if (obj == null) return;
			if (obj._done) return;
			if (obj._index == obj._source.Length)
			{
				obj._stream.Add(JsonPathInput.End);
				return;
			}
			var c = default(char);
			try
			{
				c = obj._source[obj._index++];
				var next = CharacterConverter.PathItem(c);
				obj._stream.Add(next);
			}
			catch (KeyNotFoundException)
			{
				throw new JsonPathSyntaxException(obj, "Unrecognized character '{0}' in input string.", c);
			}
		}
		private static State GotRoot(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			return State.ObjectOrArray;
		}
		private static State GotCurrent(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			if (!path._allowLocalRoot)
				throw new JsonPathSyntaxException(path, "Local paths (starting with '@') are only allowed within the context of an expression.");
			path._gotObject = false;
			return State.ObjectOrArray;
		}
		private static State GotArray(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			path._colonCount = 0;
			return State.ArrayContent;
		}
		private static State GotObject(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			if (path._gotObject)
			{
				path._gotObject = false;
				return State.Search;
			}
			path._gotObject = true;
			return State.ObjectContent;
		}
		private static State GotFunction(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var exp = new Expression<JsonValue, JsonArray>();
			path._index = exp.Parse(path._source, path._index - 1);
			var last = path.Operators.Last() as NameOperator;
			if (last != null && last.Name != "indexOf")
				throw new NotSupportedException("Currently, 'indexOf()' is the only supported function.");
			path.Operators.Remove(last);
			path.Operators.Add(new IndexOfOperator(exp));
			return State.ObjectOrArray;
		}
		private static State CompletePath(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._done = true;
			return State.End;
		}
		private static State GotArrayWildCard(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			if (path._isSearch)
			{
				path._isSearch = false;
				path.Operators.Add(new SearchOperator(new ArraySearchParameter(WildCardQuery.Instance)));
			}
			else
				path.Operators.Add(new ArrayOperator(WildCardQuery.Instance));
			return State.CloseArray;
		}
		private static State ParseIndexExpression(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var exp = new Expression<int, JsonArray>();
			path._index = exp.Parse(path._source, path._index - 1);
			path.Operators.Add(new ArrayOperator(new IndexExpressionQuery(exp)));
			return State.CloseArray;
		}
		private static State ParseFilterExpression(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var exp = new Expression<bool, JsonValue>();
			path._index = exp.Parse(path._source, path._index);
			path.Operators.Add(new ArrayOperator(new FilterExpressionQuery(exp)));
			return State.CloseArray;
		}
		private static State GotIndexOrSlice(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var value = GetNumber(path);
			path._indices = new List<int> {value};
			return State.IndexOrSlice;
		}
		private static State GotSliceValue(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var value = GetNumber(path);
			path._slice[path._colonCount] = value;
			return State.SliceValue;
		}
		private static State FinalizeSlice(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			if (path._colonCount > 2)
				throw new JsonPathSyntaxException(path, "Array slice format incorrect.  Parameters are 'start:end:increment'.");
			int? start = path._slice.ContainsKey(0) ? path._slice[0] : (int?)null;
			int? end = path._slice.ContainsKey(1) ? path._slice[1] : (int?)null;
			int? step = path._slice.ContainsKey(2) ? path._slice[2] : (int?)null;
			if (path._isSearch)
			{
				path._isSearch = false;
				path.Operators.Add(new SearchOperator(new ArraySearchParameter(new SliceQuery(start, end, step))));
			}
			else
				path.Operators.Add(new ArrayOperator(new SliceQuery(start, end, step)));
			return State.ObjectOrArray;
		}
		private static State GotSlice(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			if (path._slice == null)
				path._slice = new Dictionary<int, int>();
			if (path._indices != null && path._indices.Any())
			{
				path._slice.Add(path._colonCount, path._indices.First());
				path._indices = null;
			}
			path._colonCount++;
			return State.Colon;
		}
		private static State GotIndexValue(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var value = GetNumber(path);
			path._indices.Add(value);
			return State.IndexValue;
		}
		private static State FinalizeIndex(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			if (path._isSearch)
			{
				path._isSearch = false;
				path.Operators.Add(new SearchOperator(new ArraySearchParameter(new IndexQuery(path._indices.ToArray()))));
			}
			else
				path.Operators.Add(new ArrayOperator(new IndexQuery(path._indices.ToArray())));
			return State.ObjectOrArray;
		}
		private static State GotIndex(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			return State.Comma;
		}
		private static State FinalizeArray(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			return State.ObjectOrArray;
		}
		private static State GotSearch(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			return State.Search;
		}
		private static State GotObjectWildCard(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			path.Operators.Add(WildCardOperator.Instance);
			return State.ObjectOrArray;
		}
		private static State GotName(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var name = GetName(path);
			path.Operators.Add(new NameOperator(name));
			return State.ObjectOrArray;
		}
		private static State GotSearchArray(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			path._isSearch = true;
			return State.ArrayContent;
		}
		private static State GotSearchWildCard(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			path.Operators.Add(new SearchOperator(WildCardSearchParameter.Instance));
			return State.ObjectOrArray;
		}
		private static State GotSearchName(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var name = GetName(path);
			path.Operators.Add(new SearchOperator(new NameSearchParameter(name)));
			return State.ObjectOrArray;
		}
		private static string GetName(JsonPath path)
		{
			var chars = new List<char>();
			path._index--;
			while (path._index < path._source.Length && (char.IsLetterOrDigit(path._source[path._index]) || path._source[path._index] == '_'))
			{
				chars.Add(path._source[path._index]);
				path._index++;
			}
			var name = new string(chars.ToArray());
			return name;
		}
		private static int GetNumber(JsonPath path)
		{
			var chars = new List<char>();
			path._index--;
			var negate = path._source[path._index] == '-';
			int sign = 1;
			if (negate)
			{
				sign = -1;
				path._index++;
			}
			while (path._index < path._source.Length && char.IsNumber(path._source[path._index]))
			{
				chars.Add(path._source[path._index]);
				path._index++;
			}
			var name = new string(chars.ToArray());
			return int.Parse(name)*sign;
		}
	}
}