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
 
	File Name:		JsonPath.cs
	Namespace:		Manatee.Json.Path
	Class Name:		JsonPath
	Purpose:		Provides primary functionality for JSON Path objects.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Operators;
using Manatee.Json.Path.Parsing;
using Manatee.Json.Path.SearchParameters;
using Manatee.StateMachine;
using Manatee.StateMachine.Exceptions;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Provides primary functionality for JSON Path objects.
	/// </summary>
	public class JsonPath : List<IJsonPathOperator>
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

		private static readonly StateMachine<State, JsonPathInput> StateMachine = new StateMachine<State, JsonPathInput>();

		private bool _gotObject;
		private string _source;
		private int _index;
		private List<int> _indices;
		private Dictionary<int, int> _slice;
		private int _colonCount;
		private bool _done;
		private readonly InputStream<JsonPathInput> _stream = new InputStream<JsonPathInput>();
		private bool _isSearch;

		static JsonPath()
		{
			StateMachine[State.Start, JsonPathInput.Dollar] = GotStart;
			StateMachine[State.ObjectOrArray, JsonPathInput.OpenBracket] = GotArray;
			StateMachine[State.ObjectOrArray, JsonPathInput.Period] = GotObject;
			StateMachine[State.ObjectOrArray, JsonPathInput.End] = CompletePath;
			StateMachine[State.ArrayContent, JsonPathInput.Star] = GotArrayWildCard;
			StateMachine[State.ArrayContent, JsonPathInput.OpenParenth] = ParseIndexExpression;
			StateMachine[State.ArrayContent, JsonPathInput.Question] = ParseFilterExpression;
			StateMachine[State.ArrayContent, JsonPathInput.Number] = GotIndexOrSlice;
			StateMachine[State.ArrayContent, JsonPathInput.Colon] = GotSlice;
			StateMachine[State.IndexOrSlice, JsonPathInput.Colon] = GotSlice;
			StateMachine[State.IndexOrSlice, JsonPathInput.Comma] = GotIndex;
			StateMachine[State.Colon, JsonPathInput.CloseBracket] = FinalizeSlice;
			StateMachine[State.Colon, JsonPathInput.Number] = GotSliceValue;
			StateMachine[State.Colon, JsonPathInput.Colon] = GotSlice;
			StateMachine[State.SliceValue, JsonPathInput.CloseBracket] = FinalizeSlice;
			StateMachine[State.SliceValue, JsonPathInput.Colon] = GotSlice;
			StateMachine[State.Comma, JsonPathInput.Number] = GotIndexValue;
			StateMachine[State.IndexValue, JsonPathInput.CloseBracket] = FinalizeIndex;
			StateMachine[State.IndexValue, JsonPathInput.Comma] = GotIndex;
			StateMachine[State.CloseArray, JsonPathInput.CloseBracket] = FinalizeArray;
			StateMachine[State.ObjectContent, JsonPathInput.Period] = GotSearch;
			StateMachine[State.ObjectContent, JsonPathInput.Star] = GotObjectWildCard;
			StateMachine[State.ObjectContent, JsonPathInput.Letter] = GotName;
			StateMachine[State.Search, JsonPathInput.OpenBracket] = GotSearchArray;
			StateMachine[State.Search, JsonPathInput.Star] = GotSearchWildCard;
			StateMachine[State.Search, JsonPathInput.Letter] = GotSearchName;
			StateMachine.UpdateFunction = GetNextInput;
		}
		internal JsonPath() {}
		~JsonPath()
		{
			StateMachine.UnregisterOwner(this);
		}

		public static JsonPath Parse(string s)
		{
			var path = new JsonPath {_source = s};
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
			var found = this.Aggregate(current, (c, o) => o.Evaluate(c));
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
			return string.Format("${0}", GetRawString());
		}

		internal string GetRawString()
		{
			return this.Select(o => o.ToString()).Join(string.Empty);
		}

		private int Parse(int i)
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
			catch (InputNotValidForStateException<State, JsonInput>)
			{
				throw new JsonSyntaxException(_index);
			}
			catch (StateNotValidException<State>)
			{
				throw new JsonSyntaxException(_index);
			}
			catch (ActionNotDefinedForStateAndInputException<State, JsonInput>)
			{
				throw new JsonSyntaxException(_index);
			}
			return _index;
		}
		private static void GetNextInput(object owner)
		{
			var obj = owner as JsonPath;
			if (obj == null)
				return;
			if (obj._done)
				return;
			if (obj._index == obj._source.Length)
			{
				obj._stream.Add(JsonPathInput.End);
				return;
			}
			try
			{
				var next = CharacterConverter.PathItem(obj._source[obj._index++]);
				obj._stream.Add(next);
			}
			catch (KeyNotFoundException)
			{
				throw new JsonSyntaxException(obj._index);
			}
		}
		private static State GotStart(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
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
				path.Add(new SearchOperator(new ArraySearchParameter(WildCardQuery.Instance)));
			}
			else
				path.Add(new ArrayOperator(WildCardQuery.Instance));
			return State.CloseArray;
		}
		private static State ParseIndexExpression(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			return State.CloseArray;
		}
		private static State ParseFilterExpression(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
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
				throw new JsonSyntaxException(path._index-1);
			int? start = path._slice.ContainsKey(0) ? path._slice[0] : (int?)null;
			int? end = path._slice.ContainsKey(1) ? path._slice[1] : (int?)null;
			int? step = path._slice.ContainsKey(2) ? path._slice[2] : (int?)null;
			if (path._isSearch)
			{
				path._isSearch = false;
				path.Add(new SearchOperator(new ArraySearchParameter(new SliceQuery(start, end, step))));
			}
			else
				path.Add(new ArrayOperator(new SliceQuery(start, end, step)));
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
				path.Add(new SearchOperator(new ArraySearchParameter(new IndexQuery(path._indices.ToArray()))));
			}
			else
				path.Add(new ArrayOperator(new IndexQuery(path._indices.ToArray())));
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
			path.Add(WildCardOperator.Instance);
			return State.ObjectOrArray;
		}
		private static State GotName(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var name = GetName(path);
			path.Add(new NameOperator(name));
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
			path.Add(new SearchOperator(WildCardSearchParameter.Instance));
			return State.ObjectOrArray;
		}
		private static State GotSearchName(object owner, JsonPathInput input)
		{
			var path = owner as JsonPath;
			path._gotObject = false;
			var name = GetName(path);
			path.Add(new SearchOperator(new NameSearchParameter(name)));
			return State.ObjectOrArray;
		}
		private static string GetName(JsonPath path)
		{
			var chars = new List<char>();
			path._index--;
			while (path._index < path._source.Length && char.IsLetter(path._source[path._index]))
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
			while (path._index < path._source.Length && char.IsNumber(path._source[path._index]))
			{
				chars.Add(path._source[path._index]);
				path._index++;
			}
			var name = new string(chars.ToArray());
			return int.Parse(name);
		}
	}
}